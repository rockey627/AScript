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

		private ETemplateState _state = ETemplateState.Text;
		private int _braceDepth;
		private int _textLine;
		private int _textColumn;

		/// <summary>
		/// 输出流变量名
		/// </summary>
		public string OutVar { get; set; } = "out";
		/// <summary>
		/// 输出流方法名
		/// </summary>
		public string OutMethod { get; set; } = "Write";

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
				var tokens = BuildWriteCall(BuildTextToken());
				_state = ETemplateState.End;
				return Emit(tokens);
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
			var tokens = new List<Token>();
			while (true)
			{
				var t = base.ReadTokenFromStream();
				if (!t.HasValue) break;
				if (IsSymbol(t.Value, "}"))
				{
					_braceDepth--;
					// 结束脚本块，块尾的'}'不输出
					if (_braceDepth <= 0) break;
					tokens.Add(t.Value);
				}
				else
				{
					if (IsSymbol(t.Value, "{")) _braceDepth++;
					tokens.Add(t.Value);
				}
			}
			_state = ETemplateState.Text;
			return Emit(tokens);
		}

		/// <summary>
		/// 读取表达式${...}，转换为 out.Write(表达式)
		/// </summary>
		private Token? ReadExpression()
		{
			var tokens = new List<Token>();
			while (true)
			{
				var t = base.ReadTokenFromStream();
				if (!t.HasValue) break;
				if (IsSymbol(t.Value, "{"))
				{
					_braceDepth++;
					tokens.Add(t.Value);
				}
				else if (IsSymbol(t.Value, "}"))
				{
					_braceDepth--;
					if (_braceDepth <= 0) break;
					tokens.Add(t.Value);
				}
				else
				{
					tokens.Add(t.Value);
				}
			}
			_state = ETemplateState.Text;
			if (tokens.Count == 0)
			{
				throw new Exceptions.ScriptAnalyzingException($"模板表达式为空 ({this.CharReader.CurrentLine},{this.CharReader.CurrentColumn})");
			}
			return Emit(BuildWriteCall(tokens));
		}

		/// <summary>
		/// 读取脚本语句@...，整行记号原样透传
		/// </summary>
		private Token? ReadScriptLine()
		{
			var startLine = this.CharReader.CurrentLine;
			var startColumn = this.CharReader.CurrentColumn;
			var line = new StringBuilder();
			while (true)
			{
				var p = this.CharReader.Peek();
				// 换行符不计入脚本语句，保留给文本处理
				if (!p.HasValue || p.Value == '\n' || p.Value == '\r') break;
				this.CharReader.Read();
				line.Append(p.Value);
			}
			_state = ETemplateState.Text;
			return Emit(TokenizeLine(line.ToString(), startLine, startColumn));
		}

		/// <summary>
		/// 将脚本语句行转换为记号
		/// </summary>
		private static List<Token> TokenizeLine(string line, int startLine, int startColumn)
		{
			var tokens = new List<Token>();
			if (string.IsNullOrEmpty(line)) return tokens;
			using (var tokenStream = new DefaultTokenStream(line))
			{
				while (true)
				{
					var t = tokenStream.Next();
					if (!t.HasValue) break;
					tokens.Add(new Token(t.Value.Value, t.Value.Type, startLine, startColumn + t.Value.Column));
				}
			}
			return tokens;
		}

		/// <summary>
		/// 切换到新的状态，如果当前有文本则先输出文本
		/// </summary>
		private Token? EnterState(ETemplateState state, int braceDepth)
		{
			_braceDepth = braceDepth;
			if (_buffer.Length > 0)
			{
				var tokens = BuildWriteCall(BuildTextToken());
				_state = state;
				return Emit(tokens);
			}
			_state = state;
			return null;
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
			return token;
		}

		/// <summary>
		/// 组合 out.Write(...) 调用
		/// </summary>
		private List<Token> BuildWriteCall(Token arg)
		{
			return BuildWriteCall(new[] { arg });
		}

		/// <summary>
		/// 组合 out.Write(...) 调用
		/// </summary>
		private List<Token> BuildWriteCall(IEnumerable<Token> args)
		{
			var tokens = new List<Token>
			{
				new Token(OutVar, ETokenType.Word),
				new Token(".", ETokenType.Operator),
				new Token(OutMethod, ETokenType.Word),
				new Token("(", ETokenType.None),
			};
			tokens.AddRange(args);
			tokens.Add(new Token(")", ETokenType.None));
			tokens.Add(new Token(";", ETokenType.None));
			return tokens;
		}

		/// <summary>
		/// 将记号压入栈，并返回第一个记号
		/// </summary>
		private Token? Emit(List<Token> tokens)
		{
			if (tokens.Count == 0) return null;
			for (int i = tokens.Count - 1; i >= 1; i--)
			{
				Push(tokens[i]);
			}
			return tokens[0];
		}

		private static bool IsSymbol(Token token, string value)
		{
			return token.Type != ETokenType.String && token.Type != ETokenType.Number && token.Value == value;
		}

		protected override Task<Token?> ReadTokenFromStreamAsync(CancellationToken cancellationToken = default)
		{
			return base.ReadTokenFromStreamAsync(cancellationToken);
		}
	}
}
