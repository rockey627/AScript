using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Readers
{
	/// <summary>
	/// <para>模板记号读取器，将模板转换为脚本记号流</para>
	/// <para>@{...} 内部为脚本块，块内记号原样透传</para>
	/// <para>${...} 内部为表达式，转换为 out.Write(表达式)</para>
	/// <para>@ 开头为脚本语句，整行记号原样透传</para>
	/// <para>其他为文本，转换为 out.Write('文本')</para>
	/// </summary>
	public class TemplateTokenReader : TokenReader
	{
		private enum ETemplateState
		{
			/// <summary>文本</summary>
			Text,
			/// <summary>脚本块@{...}</summary>
			ScriptBlock,
			/// <summary>表达式${...}</summary>
			Expression,
			/// <summary>脚本语句@...</summary>
			ScriptLine,
			/// <summary>结束</summary>
			End,
		}

		private readonly StringBuilder _buffer = new StringBuilder();
		private readonly List<Token> _tokenBuffer = new List<Token>();

		private ETemplateState _state = ETemplateState.Text;
		private int _braceDepth;
		private int _textLine;
		private int _textColumn;
		private bool _textEmitted;

		/// <summary>
		/// 输出流变量名
		/// </summary>
		public string OutputVar { get; set; } = "output";
		/// <summary>
		/// 输出流方法名
		/// </summary>
		public string OutputMethod { get; set; } = "Write";

		public TemplateTokenReader(ITokenStream stream, bool autoDisposeStream) : base(stream, autoDisposeStream)
		{
		}

		protected override Token? ReadTokenFromStream()
		{
			while (true)
			{
				switch (_state)
				{
					case ETemplateState.Text:
						{
							var token = ReadText();
							if (token.HasValue) return token;
							if (_state == ETemplateState.End) return null;
							break;
						}
					case ETemplateState.ScriptBlock:
						{
							var token = ReadScriptBlock();
							if (token.HasValue) return token;
							break;
						}
					case ETemplateState.Expression:
						return ReadExpression();
					case ETemplateState.ScriptLine:
						{
							var token = ReadScriptLine();
							if (token.HasValue) return token;
							break;
						}
					default:
						return null;
				}
			}
		}

		/// <summary>
		/// 读取文本，直到遇到模板标记或模板结束
		/// </summary>
		private Token? ReadText()
		{
			var c = this.CharReader.Read();
			while (c.HasValue && c.Value != '@' && c.Value != '$')
			{
				AppendText(c.Value);
				c = this.CharReader.Read();
			}
			if (!c.HasValue)
			{
				// 模板结束
				if (_buffer.Length == 0)
				{
					_state = ETemplateState.End;
					return null;
				}
				_state = ETemplateState.End;
				return EmitWriteCall(BuildTextToken());
			}
			if (c.Value == '$')
			{
				var next = this.CharReader.Peek();
				if (next.HasValue && next.Value == '{')
				{
					// 表达式${...}
					this.CharReader.Read();
					return EnterState(ETemplateState.Expression, 1);
				}
				// '$' 为普通文本
				AppendText(c.Value);
				return null;
			}
			// '@'
			var nextChar = this.CharReader.Peek();
			if (nextChar.HasValue && nextChar.Value == '{')
			{
				// 脚本块@{...}
				this.CharReader.Read();
				return EnterState(ETemplateState.ScriptBlock, 1);
			}
			// 脚本语句@...
			return EnterState(ETemplateState.ScriptLine, 0);
		}

		/// <summary>
		/// 读取脚本块@{...}，块内记号原样透传
		/// </summary>
		private Token? ReadScriptBlock()
		{
			_tokenBuffer.Clear();
			while (true)
			{
				var t = base.ReadTokenFromStream();
				if (!t.HasValue) break;
				if (t.Value.IsSymbol("}"))
				{
					_braceDepth--;
					// 结束脚本块，块尾的'}'不输出
					if (_braceDepth <= 0) break;
					_tokenBuffer.Add(t.Value);
				}
				else
				{
					if (t.Value.IsSymbol("{")) _braceDepth++;
					_tokenBuffer.Add(t.Value);
				}
			}
			_state = ETemplateState.Text;
			// 脚本块的换行符不输出
			SkipLineBreak();
			return Emit(_tokenBuffer);
		}

		/// <summary>
		/// 读取表达式${...}，转换为 out.Write(表达式)
		/// </summary>
		private Token? ReadExpression()
		{
			_tokenBuffer.Clear();
			while (true)
			{
				var t = base.ReadTokenFromStream();
				if (!t.HasValue) break;
				if (t.Value.IsSymbol("{"))
				{
					_braceDepth++;
					_tokenBuffer.Add(t.Value);
				}
				else if (t.Value.IsSymbol("}"))
				{
					_braceDepth--;
					if (_braceDepth <= 0) break;
					_tokenBuffer.Add(t.Value);
				}
				else
				{
					_tokenBuffer.Add(t.Value);
				}
			}
			_state = ETemplateState.Text;
			return EmitWriteCall(_tokenBuffer);
		}

		/// <summary>
		/// 读取脚本语句@...，整行记号原样透传
		/// </summary>
		private Token? ReadScriptLine()
		{
			_tokenBuffer.Clear();
			while (true)
			{
				var c = this.CharReader.Read();
				while (c.HasValue && (c.Value == ' ' || c.Value == '\r' || c.Value == '\t'))
				{
					c = this.CharReader.Read();
				}
				if (!c.HasValue || c.Value == '\n') break;
				this.CharReader.Push(c.Value);
				var t = base.ReadTokenFromStream();
				if (!t.HasValue) break;
				_tokenBuffer.Add(t.Value);
			}
			_state = ETemplateState.Text;
			// 脚本语句的换行符不输出
			SkipLineBreak();
			return Emit(_tokenBuffer);
		}

		/// <summary>
		/// 跳过换行符，脚本块和脚本语句的换行符不输出
		/// </summary>
		private void SkipLineBreak()
		{
			var c = this.CharReader.Peek();
			if (!c.HasValue) return;
			if (c.Value == '\r')
			{
				this.CharReader.Read();
				var next = this.CharReader.Peek();
				if (next.HasValue && next.Value == '\n')
				{
					this.CharReader.Read();
				}
			}
			else if (c.Value == '\n')
			{
				this.CharReader.Read();
			}
		}

		/// <summary>
		/// 切换到新的状态，如果当前有文本则先输出文本
		/// </summary>
		private Token? EnterState(ETemplateState state, int braceDepth)
		{
			_braceDepth = braceDepth;
			if (state == ETemplateState.ScriptBlock || state == ETemplateState.ScriptLine)
			{
				// 脚本块和脚本语句前的缩进属于脚本，不输出
				RemoveIndent();
			}
			if (_buffer.Length > 0)
			{
				_state = state;
				return EmitWriteCall(BuildTextToken());
			}
			_state = state;
			return null;
		}

		/// <summary>
		/// 移除行首缩进（缩进位于换行符之后，或位于模板开头）
		/// </summary>
		private void RemoveIndent()
		{
			int end = _buffer.Length;
			int start = end;
			while (start > 0 && (_buffer[start - 1] == ' ' || _buffer[start - 1] == '\t'))
			{
				start--;
			}
			if (start == end) return;
			if (start > 0)
			{
				if (_buffer[start - 1] != '\n' && _buffer[start - 1] != '\r') return;
			}
			else if (_textEmitted)
			{
				// 不是模板开头的缩进，而是表达式等之后的空白
				return;
			}
			_buffer.Remove(start, end - start);
		}

		private void AppendText(char c)
		{
			if (_buffer.Length == 0)
			{
				_textLine = this.CharReader.CurrentLine;
				_textColumn = this.CharReader.CurrentColumn;
			}
			_buffer.Append(c);
		}

		private Token BuildTextToken()
		{
			var token = new Token(_buffer.ToString(), ETokenType.String, _textLine, _textColumn);
			_buffer.Clear();
			_textEmitted = true;
			return token;
		}

		/// <summary>
		/// 组合 out.Write(...) 调用
		/// </summary>
		private Token EmitWriteCall(Token arg)
		{
			Push(new Token(";", ETokenType.None));
			Push(new Token(")", ETokenType.None));
			Push(arg);
			Push(new Token("(", ETokenType.None));
			Push(new Token(OutputMethod, ETokenType.Word));
			Push(new Token(".", ETokenType.Operator));
			return new Token(OutputVar, ETokenType.Word);
		}

		/// <summary>
		/// 组合 out.Write(...) 调用
		/// </summary>
		private Token? EmitWriteCall(IList<Token> args)
		{
			if (args == null || args.Count == 0) return null;
			Push(new Token(";", ETokenType.None));
			Push(new Token(")", ETokenType.None));
			for (int i = args.Count - 1; i >= 0; i--)
			{
				Push(args[i]);
			}
			Push(new Token("(", ETokenType.None));
			Push(new Token(OutputMethod, ETokenType.Word));
			Push(new Token(".", ETokenType.Operator));
			return new Token(OutputVar, ETokenType.Word);
		}

		/// <summary>
		/// 将记号压入栈，并返回第一个记号
		/// </summary>
		private Token? Emit(List<Token> tokens)
		{
			if (tokens == null || tokens.Count == 0) return null;
			for (int i = tokens.Count - 1; i >= 1; i--)
			{
				Push(tokens[i]);
			}
			return tokens[0];
		}

		protected override Task<Token?> ReadTokenFromStreamAsync(CancellationToken cancellationToken = default)
		{
			return base.ReadTokenFromStreamAsync(cancellationToken);
		}
	}
}
