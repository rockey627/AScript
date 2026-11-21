using System;
using System.Collections.Generic;
using System.IO;
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
	/// <para>include('文件') 出现在脚本中时，切换字符流到指定的模板文件，读取完毕后恢复宿主字符流</para>
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

		/// <summary>
		/// 包含模板文件时的宿主信息
		/// </summary>
		private sealed class IncludeFrame
		{
			/// <summary>宿主记号流</summary>
			public ITokenStream Stream { get; set; }
			/// <summary>宿主状态</summary>
			public ETemplateState State { get; set; }
			/// <summary>宿主大括号深度</summary>
			public int BraceDepth { get; set; }
			/// <summary>宿主基准目录</summary>
			public string BaseDirectory { get; set; }
			/// <summary>宿主上一个脚本记号</summary>
			public Token? LastToken { get; set; }
		}

		private readonly StringBuilder _buffer = new StringBuilder();

		/// <summary>
		/// 包含模板文件的栈
		/// </summary>
		private readonly Stack<IncludeFrame> _includeStack = new Stack<IncludeFrame>();

		private ETemplateState _state = ETemplateState.Text;
		private int _braceDepth;
		private int _expressionTokenCount;
		private int _textLine;
		private int _textColumn;
		private bool _textEmitted;
		/// <summary>
		/// 上一个返回的脚本记号（用于排除成员访问，如 obj.include(...)）
		/// </summary>
		private Token? _lastToken;

		/// <summary>
		/// 输出流变量名
		/// </summary>
		public string OutputVar { get; set; } = "output";
		/// <summary>
		/// 输出流方法名
		/// </summary>
		public string OutputMethod { get; set; } = "Write";
		/// <summary>
		/// 包含模板文件的方法名
		/// </summary>
		public string IncludeMethod { get; set; } = "include";
		/// <summary>
		/// 包含模板文件的基准目录（相对路径的基准）
		/// </summary>
		public string BaseDirectory { get; set; }

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
				if (_includeStack.Count > 0)
				{
					// 包含的模板文件读取完毕，恢复宿主字符流
					Token? token = null;
					if (_buffer.Length > 0)
					{
						token = EmitWriteCall(BuildTextToken());
					}
					RestoreHostStream();
					return token;
				}
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
				if (_includeStack.Count > 0)
				{
					// 包含的模板文件提前结束，恢复宿主字符流
					RestoreHostStream();
					return null;
				}
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			if (TryReadInclude(t.Value))
			{
				// 已切换到包含的模板文件
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
			_lastToken = t.Value;
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
				if (_includeStack.Count > 0)
				{
					// 包含的模板文件提前结束，恢复宿主字符流
					RestoreHostStream();
					return null;
				}
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			if (TryReadInclude(t.Value))
			{
				// 已切换到包含的模板文件
				return null;
			}
			_lastToken = t.Value;
			return t.Value;
		}

		/// <summary>
		/// 识别 include 调用，识别成功则切换字符流到包含的模板文件
		/// </summary>
		/// <param name="token">脚本记号</param>
		/// <returns></returns>
		private bool TryReadInclude(Token token)
		{
			if (string.IsNullOrEmpty(this.IncludeMethod)) return false;
			if (token.Type != ETokenType.Word || token.Value != this.IncludeMethod) return false;
			// 成员访问（如 obj.include(...)）不作为 include 处理
			if (_lastToken.HasValue && _lastToken.Value.IsSymbol(".")) return false;
			// 确认后面紧跟 '('，避免记号级前瞻跨行读取
			if (!TryPeekIncludeCall()) return false;
			var path = ReadIncludePath();
			// 不是 include('路径') 格式，按普通脚本处理
			if (path == null) return false;
			// 切换字符流到包含的模板文件
			SwitchToInclude(path);
			return true;
		}

		/// <summary>
		/// 异步识别 include 调用，识别成功则切换字符流到包含的模板文件
		/// </summary>
		/// <param name="token">脚本记号</param>
		/// <returns></returns>
		private async Task<bool> TryReadIncludeAsync(Token token, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(this.IncludeMethod)) return false;
			if (token.Type != ETokenType.Word || token.Value != this.IncludeMethod) return false;
			// 成员访问（如 obj.include(...)）不作为 include 处理
			if (_lastToken.HasValue && _lastToken.Value.IsSymbol(".")) return false;
			// 确认后面紧跟 '('，避免记号级前瞻跨行读取
			if (!await TryPeekIncludeCallAsync(cancellationToken).ConfigureAwait(false)) return false;
			var path = await ReadIncludePathAsync(cancellationToken).ConfigureAwait(false);
			// 不是 include('路径') 格式，按普通脚本处理
			if (path == null) return false;
			// 切换字符流到包含的模板文件
			SwitchToInclude(path);
			return true;
		}

		/// <summary>
		/// 字符级确认 include 后面是否紧跟 '('，避免记号级前瞻跨行读取
		/// </summary>
		/// <returns></returns>
		private bool TryPeekIncludeCall()
		{
			var consumed = new StringBuilder();
			while (true)
			{
				var c = this.CharReader.Read();
				if (!c.HasValue) { UnreadChars(consumed); return false; }
				consumed.Append(c.Value);
				if (c.Value == ' ' || c.Value == '\t') continue;
				var isCall = c.Value == '(';
				UnreadChars(consumed);
				return isCall;
			}
		}

		/// <summary>
		/// 异步字符级确认 include 后面是否紧跟 '('，避免记号级前瞻跨行读取
		/// </summary>
		/// <returns></returns>
		private async Task<bool> TryPeekIncludeCallAsync(CancellationToken cancellationToken)
		{
			var consumed = new StringBuilder();
			while (true)
			{
				var c = await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				if (!c.HasValue) { UnreadChars(consumed); return false; }
				consumed.Append(c.Value);
				if (c.Value == ' ' || c.Value == '\t') continue;
				var isCall = c.Value == '(';
				UnreadChars(consumed);
				return isCall;
			}
		}

		/// <summary>
		/// 读取 include 的参数：( '路径' )，路径为字符串字面量；格式不匹配时返回null
		/// </summary>
		/// <returns></returns>
		private string ReadIncludePath()
		{
			var tokens = new List<Token>();
			// '('
			var t1 = base.ReadTokenFromStream();
			if (t1.HasValue) tokens.Add(t1.Value);
			if (!t1.HasValue || !t1.Value.IsSymbol("("))
			{
				UnreadTokens(tokens);
				return null;
			}
			// 路径（字符串字面量）
			var t2 = base.ReadTokenFromStream();
			if (t2.HasValue) tokens.Add(t2.Value);
			if (!t2.HasValue || t2.Value.Type != ETokenType.String)
			{
				UnreadTokens(tokens);
				return null;
			}
			// ')'
			var t3 = base.ReadTokenFromStream();
			if (t3.HasValue) tokens.Add(t3.Value);
			if (!t3.HasValue || !t3.Value.IsSymbol(")"))
			{
				UnreadTokens(tokens);
				return null;
			}
			return t2.Value.Value;
		}

		/// <summary>
		/// 异步读取 include 的参数：( '路径' )，路径为字符串字面量；格式不匹配时返回null
		/// </summary>
		/// <returns></returns>
		private async Task<string> ReadIncludePathAsync(CancellationToken cancellationToken)
		{
			var tokens = new List<Token>();
			// '('
			var t1 = await base.ReadTokenFromStreamAsync(cancellationToken).ConfigureAwait(false);
			if (t1.HasValue) tokens.Add(t1.Value);
			if (!t1.HasValue || !t1.Value.IsSymbol("("))
			{
				UnreadTokens(tokens);
				return null;
			}
			// 路径（字符串字面量）
			var t2 = await base.ReadTokenFromStreamAsync(cancellationToken).ConfigureAwait(false);
			if (t2.HasValue) tokens.Add(t2.Value);
			if (!t2.HasValue || t2.Value.Type != ETokenType.String)
			{
				UnreadTokens(tokens);
				return null;
			}
			// ')'
			var t3 = await base.ReadTokenFromStreamAsync(cancellationToken).ConfigureAwait(false);
			if (t3.HasValue) tokens.Add(t3.Value);
			if (!t3.HasValue || !t3.Value.IsSymbol(")"))
			{
				UnreadTokens(tokens);
				return null;
			}
			return t2.Value.Value;
		}

		/// <summary>
		/// 退回记号（按读取的逆序压栈，读取时顺序不变）
		/// </summary>
		/// <param name="tokens"></param>
		private void UnreadTokens(List<Token> tokens)
		{
			if (tokens == null || tokens.Count == 0) return;
			for (int i = tokens.Count - 1; i >= 0; i--)
			{
				Push(tokens[i]);
			}
		}

		/// <summary>
		/// 退回已读取的字符
		/// </summary>
		/// <param name="consumed"></param>
		private void UnreadChars(StringBuilder consumed)
		{
			if (consumed == null || consumed.Length == 0) return;
			this.CharReader.Push(consumed.ToString());
		}

		/// <summary>
		/// 切换字符流到包含的模板文件
		/// </summary>
		/// <param name="path">模板文件路径</param>
		private void SwitchToInclude(string path)
		{
			if (!Path.IsPathRooted(path) && !string.IsNullOrEmpty(this.BaseDirectory))
			{
				path = Path.Combine(this.BaseDirectory, path);
			}
			var file = new FileInfo(path);
			if (!file.Exists)
			{
				throw new Exceptions.ScriptAnalyzingException($"包含的模板文件不存在：{path}");
			}
			// 保存宿主信息
			_includeStack.Push(new IncludeFrame
			{
				Stream = this.TokenStream,
				State = _state,
				BraceDepth = _braceDepth,
				BaseDirectory = this.BaseDirectory,
				LastToken = _lastToken,
			});
			var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
			this.TokenStream = new DefaultTokenStream(new StreamCharStream(stream, true));
			this.BaseDirectory = file.DirectoryName;
			_state = ETemplateState.Text;
		}

		/// <summary>
		/// 恢复宿主的字符流
		/// </summary>
		private void RestoreHostStream()
		{
			if (_includeStack.Count == 0) return;
			var frame = _includeStack.Pop();
			this.TokenStream = frame.Stream;
			this.BaseDirectory = frame.BaseDirectory;
			_state = frame.State;
			_braceDepth = frame.BraceDepth;
			_lastToken = frame.LastToken;
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
			if (state == ETemplateState.ExpressionBegin)
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
				if (_includeStack.Count > 0)
				{
					// 包含的模板文件读取完毕，恢复宿主字符流
					Token? token = null;
					if (_buffer.Length > 0)
					{
						token = EmitWriteCallForAsync(BuildTextToken());
					}
					RestoreHostStream();
					return token;
				}
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
				if (_includeStack.Count > 0)
				{
					// 包含的模板文件提前结束，恢复宿主字符流
					RestoreHostStream();
					return null;
				}
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			if (await TryReadIncludeAsync(t.Value, cancellationToken).ConfigureAwait(false))
			{
				// 已切换到包含的模板文件
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
			_lastToken = t.Value;
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
				if (_includeStack.Count > 0)
				{
					// 包含的模板文件提前结束，恢复宿主字符流
					RestoreHostStream();
					return null;
				}
				// 模板结束
				_state = ETemplateState.End;
				return null;
			}
			if (await TryReadIncludeAsync(t.Value, cancellationToken).ConfigureAwait(false))
			{
				// 已切换到包含的模板文件
				return null;
			}
			_lastToken = t.Value;
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
