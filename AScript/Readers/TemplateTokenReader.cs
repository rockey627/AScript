using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Readers
{
	/// <summary>
	/// <para>模板记号读取器，将模板转换为脚本记号流</para>
	/// <para>@{...} 内部为脚本块，块内记号原样透传</para>
	/// <para>${...} 内部为表达式，转换为 output.Write(表达式)</para>
	/// <para>@ 开头为脚本行，整行记号原样透传</para>
	/// <para>其他为文本，转换为 output.Write('文本')</para>
	/// <para>每次只读取一个记号，通过状态记录当前解析进度</para>
	/// </summary>
	public class TemplateTokenReader : TokenReader
	{
		private enum ETemplateState
		{
			/// <summary>文本</summary>
			Text,
			/// <summary>脚本块@{...}</summary>
			ScriptBlock,
			/// <summary>表达式${...}，输出 output.Write( 的前缀</summary>
			ExpressionBegin,
			/// <summary>表达式${...}的内容</summary>
			Expression,
			/// <summary>脚本行@...</summary>
			ScriptLine,
			/// <summary>结束</summary>
			End,
		}

		private readonly StringBuilder _buffer = new StringBuilder();

		private ETemplateState _state = ETemplateState.Text;
		private int _braceDepth;
		private int _expressionTokenCount;
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
					case ETemplateState.ExpressionBegin:
						{
							var token = ReadExpressionToken();
							if (!token.HasValue) return null;
							_state = ETemplateState.Expression;
							// output.Write(token0
							Push(token.Value);
							Push(new Token("(", ETokenType.None));
							Push(new Token(OutputMethod, ETokenType.Word));
							Push(new Token(".", ETokenType.Operator));
							return new Token(OutputVar, ETokenType.Word);
						}
					case ETemplateState.Expression:
						{
							var token = ReadExpressionToken();
							if (token.HasValue) return token;
							break;
						}
					case ETemplateState.ScriptBlock:
						{
							var token = ReadScriptBlockToken();
							if (token.HasValue) return token;
							break;
						}
					case ETemplateState.ScriptLine:
						{
							var token = ReadScriptLineToken();
							if (token.HasValue) return token;
							break;
						}
					default:
						return null;
				}
			}
		}

		/// <summary>
		/// 可转义的字符，只有这三个字符可以用 '\' 转义
		/// </summary>
		private static bool IsEscapeChar(char c)
		{
			return c == '\\' || c == '@' || c == '$';
		}

		/// <summary>
		/// 读取文本，直到遇到模板标记或模板结束
		/// </summary>
		private Token? ReadText()
		{
			var c = this.CharReader.Read();
			while (c.HasValue && c.Value != '@' && c.Value != '$')
			{
				if (c.Value == '\\')
				{
					// 转义字符
					var esc = this.CharReader.Peek();
					if (esc.HasValue && IsEscapeChar(esc.Value))
					{
						this.CharReader.Read();
						AppendText(esc.Value);
						c = this.CharReader.Read();
						continue;
					}
				}
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
					return EnterState(ETemplateState.ExpressionBegin, 1);
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
			// 脚本行@...
			return EnterState(ETemplateState.ScriptLine, 0);
		}

		/// <summary>
		/// 读取表达式${...}的内容，每次读取一个记号
		/// </summary>
		private Token? ReadExpressionToken()
		{
			var t = base.ReadTokenFromStream();
			if (!t.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"模板表达式未闭合 ({this.CharReader.CurrentLine},{this.CharReader.CurrentColumn})");
			}
			if (t.Value.IsSymbol("}"))
			{
				_braceDepth--;
				if (_braceDepth <= 0)
				{
					// 表达式结束，输出 ) ;
					_state = ETemplateState.Text;
					if (_expressionTokenCount == 0) return null;
					Push(new Token(";", ETokenType.None));
					return new Token(")", ETokenType.None, t.Value.Line, t.Value.Column);
				}
			}
			else
			{
				_expressionTokenCount++;
				if (t.Value.IsSymbol("{")) _braceDepth++;
			}
			return t.Value;
		}

		/// <summary>
		/// 读取脚本块@{...}的内容，每次读取一个记号
		/// </summary>
		private Token? ReadScriptBlockToken()
		{
			var t = base.ReadTokenFromStream();
			if (!t.HasValue)
			{
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			if (t.Value.IsSymbol("}"))
			{
				_braceDepth--;
				if (_braceDepth <= 0)
				{
					// 脚本块结束，块尾的'}'不输出
					_state = ETemplateState.Text;
					// 脚本块的换行符不输出
					SkipLineBreak();
					return null;
				}
			}
			else if (t.Value.IsSymbol("{"))
			{
				_braceDepth++;
			}
			return t.Value;
		}

		/// <summary>
		/// 读取脚本行@...的内容，每次读取一个记号
		/// </summary>
		private Token? ReadScriptLineToken()
		{
			// 跳过行首空白
			var c = this.CharReader.Read();
			while (c.HasValue && (c.Value == ' ' || c.Value == '\t'))
			{
				c = this.CharReader.Read();
			}
			if (!c.HasValue)
			{
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			if (c.Value == '\r' || c.Value == '\n')
			{
				// 脚本行结束，换行符不输出
				if (c.Value == '\r')
				{
					var next = this.CharReader.Peek();
					if (next.HasValue && next.Value == '\n')
					{
						this.CharReader.Read();
					}
				}
				_state = ETemplateState.Text;
				return null;
			}
			this.CharReader.Push(c.Value);
			var t = base.ReadTokenFromStream();
			if (!t.HasValue)
			{
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			return t.Value;
		}

		/// <summary>
		/// 跳过换行符，脚本块和脚本行的换行符不输出
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
				// 脚本块和脚本行前的缩进属于脚本，不输出
				RemoveIndent();
			}
			else if (state == ETemplateState.ExpressionBegin)
			{
				_expressionTokenCount = 0;
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
		/// 组合 output.Write(...) 调用
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
		/// 组合 await output.WriteAsync(...) 调用
		/// </summary>
		private Token EmitWriteCallForAsync(Token arg)
		{
			Push(new Token(";", ETokenType.None));
			Push(new Token(")", ETokenType.None));
			Push(arg);
			Push(new Token("(", ETokenType.None));
			Push(new Token(OutputMethod, ETokenType.Word));
			Push(new Token(".", ETokenType.Operator));
			Push(new Token(OutputVar, ETokenType.Word));
			return new Token("await", ETokenType.Word);
		}

		/// <summary>
		/// 异步读取记号，状态机与同步版本一致，每次只读取一个记号
		/// </summary>
		protected override async Task<Token?> ReadTokenFromStreamAsync(CancellationToken cancellationToken = default)
		{
			while (true)
			{
				switch (_state)
				{
					case ETemplateState.Text:
						{
							var token = await ReadTextAsync(cancellationToken).ConfigureAwait(false);
							if (token.HasValue) return token;
							if (_state == ETemplateState.End) return null;
							break;
						}
					case ETemplateState.ExpressionBegin:
						{
							var token = await ReadExpressionTokenAsync(cancellationToken).ConfigureAwait(false);
							if (!token.HasValue) return null;
							_state = ETemplateState.Expression;
							// await output.WriteAsync(token0
							Push(token.Value);
							Push(new Token("(", ETokenType.None));
							Push(new Token(OutputMethod, ETokenType.Word));
							Push(new Token(".", ETokenType.Operator));
							Push(new Token(OutputVar, ETokenType.Word));
							return new Token("await", ETokenType.Word);
						}
					case ETemplateState.Expression:
						{
							var token = await ReadExpressionTokenAsync(cancellationToken).ConfigureAwait(false);
							if (token.HasValue) return token;
							break;
						}
					case ETemplateState.ScriptBlock:
						{
							var token = await ReadScriptBlockTokenAsync(cancellationToken).ConfigureAwait(false);
							if (token.HasValue) return token;
							break;
						}
					case ETemplateState.ScriptLine:
						{
							var token = await ReadScriptLineTokenAsync(cancellationToken).ConfigureAwait(false);
							if (token.HasValue) return token;
							break;
						}
					default:
						return null;
				}
			}
		}

		/// <summary>
		/// 异步读取文本，直到遇到模板标记或模板结束
		/// </summary>
		private async Task<Token?> ReadTextAsync(CancellationToken cancellationToken)
		{
			var c = await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			while (c.HasValue && c.Value != '@' && c.Value != '$')
			{
				if (c.Value == '\\')
				{
					// 转义字符
					var esc = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
					if (esc.HasValue && IsEscapeChar(esc.Value))
					{
						await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
						AppendText(esc.Value);
						c = await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
						continue;
					}
				}
				AppendText(c.Value);
				c = await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
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
				return EmitWriteCallForAsync(BuildTextToken());
			}
			if (c.Value == '$')
			{
				var next = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
				if (next.HasValue && next.Value == '{')
				{
					// 表达式${...}
					await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					return EnterState(ETemplateState.ExpressionBegin, 1);
				}
				// '$' 为普通文本
				AppendText(c.Value);
				return null;
			}
			// '@'
			var nextChar = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
			if (nextChar.HasValue && nextChar.Value == '{')
			{
				// 脚本块@{...}
				await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				return EnterState(ETemplateState.ScriptBlock, 1);
			}
			// 脚本行@...
			return EnterState(ETemplateState.ScriptLine, 0);
		}

		/// <summary>
		/// 异步读取表达式${...}的内容，每次读取一个记号
		/// </summary>
		private async Task<Token?> ReadExpressionTokenAsync(CancellationToken cancellationToken)
		{
			var t = await base.ReadTokenFromStreamAsync(cancellationToken).ConfigureAwait(false);
			if (!t.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"模板表达式未闭合 ({this.CharReader.CurrentLine},{this.CharReader.CurrentColumn})");
			}
			if (t.Value.IsSymbol("}"))
			{
				_braceDepth--;
				if (_braceDepth <= 0)
				{
					// 表达式结束，输出 ) ;
					_state = ETemplateState.Text;
					if (_expressionTokenCount == 0) return null;
					Push(new Token(";", ETokenType.None));
					return new Token(")", ETokenType.None, t.Value.Line, t.Value.Column);
				}
			}
			else
			{
				_expressionTokenCount++;
				if (t.Value.IsSymbol("{")) _braceDepth++;
			}
			return t.Value;
		}

		/// <summary>
		/// 异步读取脚本块@{...}的内容，每次读取一个记号
		/// </summary>
		private async Task<Token?> ReadScriptBlockTokenAsync(CancellationToken cancellationToken)
		{
			var t = await base.ReadTokenFromStreamAsync(cancellationToken).ConfigureAwait(false);
			if (!t.HasValue)
			{
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			if (t.Value.IsSymbol("}"))
			{
				_braceDepth--;
				if (_braceDepth <= 0)
				{
					// 脚本块结束，块尾的'}'不输出
					_state = ETemplateState.Text;
					// 脚本块的换行符不输出
					await SkipLineBreakAsync(cancellationToken).ConfigureAwait(false);
					return null;
				}
			}
			else if (t.Value.IsSymbol("{"))
			{
				_braceDepth++;
			}
			return t.Value;
		}

		/// <summary>
		/// 异步读取脚本行@...的内容，每次读取一个记号
		/// </summary>
		private async Task<Token?> ReadScriptLineTokenAsync(CancellationToken cancellationToken)
		{
			// 跳过行首空白
			var c = await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			while (c.HasValue && (c.Value == ' ' || c.Value == '\t'))
			{
				c = await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			}
			if (!c.HasValue)
			{
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			if (c.Value == '\r' || c.Value == '\n')
			{
				// 脚本行结束，换行符不输出
				if (c.Value == '\r')
				{
					var next = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
					if (next.HasValue && next.Value == '\n')
					{
						await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					}
				}
				_state = ETemplateState.Text;
				return null;
			}
			this.CharReader.Push(c.Value);
			var t = await base.ReadTokenFromStreamAsync(cancellationToken).ConfigureAwait(false);
			if (!t.HasValue)
			{
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			return t.Value;
		}

		/// <summary>
		/// 异步跳过换行符，脚本块和脚本行的换行符不输出
		/// </summary>
		private async Task SkipLineBreakAsync(CancellationToken cancellationToken)
		{
			var c = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
			if (!c.HasValue) return;
			if (c.Value == '\r')
			{
				await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				var next = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
				if (next.HasValue && next.Value == '\n')
				{
					await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			else if (c.Value == '\n')
			{
				await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			}
		}
	}
}
