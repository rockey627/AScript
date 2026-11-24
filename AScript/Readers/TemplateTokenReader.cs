using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Readers
{
	/// <summary>
	/// <para>模板记号读取器，将模板转换为脚本记号流</para>
	/// <para>@{...} 内部为脚本块，块内记号原样透传</para>
	/// <para>@(...) 内部为脚本块，功能与@{...}一致，只是结束符为')'</para>
	/// <para>@*...*@ 内部为注释，注释内容不输出</para>
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
			/// <summary>脚本块@{...} 或 @(...)</summary>
			ScriptBlock,
			/// <summary>表达式${...}，输出 output.Write( 的前缀</summary>
			ExpressionBegin,
			/// <summary>表达式${...}的内容</summary>
			Expression,
			/// <summary>脚本行@...</summary>
			ScriptLine,
			/// <summary>注释块@*...*@</summary>
			Comment,
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
			/// <summary>宿主脚本块的结束符（'}' 或 ')'）</summary>
			public char BlockEnd { get; set; }
			/// <summary>宿主表达式记号数</summary>
			public int ExpressionTokenCount { get; set; }
			/// <summary>宿主当前行是否有静态文本</summary>
			public bool LineHasText { get; set; }
			/// <summary>宿主当前行是否有输出内容</summary>
			public bool LineHasOutput { get; set; }
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
		/// <summary>
		/// 当前脚本块的结束符（'}' 或 ')'），@{...} 为 '}'，@(...) 为 ')'
		/// </summary>
		private char _blockEnd = '}';
		private int _expressionTokenCount;
		private int _textLine;
		private int _textColumn;
		/// <summary>
		/// 当前行是否包含静态文本
		/// </summary>
		private bool _lineHasText;
		/// <summary>
		/// 当前行是否包含输出内容（表达式或include）
		/// </summary>
		private bool _lineHasOutput;
		/// <summary>
		/// 当前脚本行是否单独占一行（行内没有其他内容时，移除该行的换行符）
		/// </summary>
		private bool _lineOnlyScript => !_lineHasText && !_lineHasOutput;
		/// <summary>
		/// 上一个返回的脚本记号（用于排除成员访问，如 obj.include(...)）
		/// </summary>
		private Token? _lastToken;

		public Template.IOutput Output { get; set; }
		/// <summary>
		/// 输出流变量名
		/// </summary>
		public string OutputVar { get; set; } = "output";
		/// <summary>
		/// 输出流方法名
		/// </summary>
		public MethodInfo OutputMethod { get; set; }
		/// <summary>
		/// 异步输出流方法名
		/// </summary>
		public MethodInfo OutputMethodAsync { get; set; }
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
							// 表达式结束（空表达式）或已切换到包含的模板文件，继续读取
							if (!token.HasValue) break;
							if (IsIncludeToken(token.Value))
							{
								// 表达式为include调用，包含的模板内容直接输出，不加output.Write包装
								// 先清零表达式记号数，使包含模板结束后宿主表达式能正确结束
								var tokenCount = _expressionTokenCount;
								_expressionTokenCount = 0;
								if (TryReadInclude(token.Value, false)) break;
								// 不是include('路径')格式，按普通表达式处理
								_expressionTokenCount = tokenCount;
							}
							_state = ETemplateState.Expression;
							// output.Write(token0
							Push(token.Value);
							Push(new Token("(", ETokenType.None));
							Push(new Token(OutputMethod.Name, ETokenType.Word));
							Push(new Token(".", ETokenType.Operator));
							return this.Output == null ? new Token(this.OutputVar, ETokenType.Word) : new Token(this.Output);
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
					case ETemplateState.Comment:
						{
							SkipComment();
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
						//token = EmitWriteCall(BuildTextToken());
						token = BuildWriteStringToken();
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
				//return EmitWriteCall(BuildTextToken());
				return BuildWriteStringToken();
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
			if (nextChar.HasValue && nextChar.Value == '(')
			{
				// 脚本块@(...)，功能与@{...}一致，只是结束符为')'
				this.CharReader.Read();
				return EnterState(ETemplateState.ScriptBlock, 1, ')');
			}
			if (nextChar.HasValue && nextChar.Value == '*')
			{
				// 注释块@*...*@
				this.CharReader.Read();
				return EnterState(ETemplateState.Comment, 0);
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
		/// 读取脚本块@{...}或@(...)的内容，每次读取一个记号
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
			if (IsBlockEnd(t.Value))
			{
				_braceDepth--;
				if (_braceDepth <= 0)
				{
					// 脚本块结束，块尾的结束符不输出
					_state = ETemplateState.Text;
					// 脚本块的换行符不输出
					SkipLineBreak();
					return null;
				}
			}
			else if (IsBlockBegin(t.Value))
			{
				_braceDepth++;
			}
			_lastToken = t.Value;
			return t.Value;
		}

		/// <summary>
		/// 判断记号是否为脚本块的结束符（'}' 或 ')'）
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool IsBlockEnd(Token token)
		{
			return token.IsSymbol(_blockEnd == ')' ? ")" : "}");
		}

		/// <summary>
		/// 判断记号是否为脚本块的开始符（'{' 或 '('），与结束符配对
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool IsBlockBegin(Token token)
		{
			return _blockEnd == ')' ? token.IsSymbol("(") : token.IsSymbol("{");
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
				// 脚本行结束
				if (_lineOnlyScript)
				{
					// 脚本行单独占一行，移除一个换行符
					if (c.Value == '\r')
					{
						var next = this.CharReader.Peek();
						if (next.HasValue && next.Value == '\n')
						{
							this.CharReader.Read();
						}
					}
					// 换行符被移除，进入新的一行
					_lineHasText = false;
					_lineHasOutput = false;
				}
				else
				{
					// 脚本行未单独占一行，换行符保留，作为文本输出
					this.CharReader.Push(c.Value);
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
		/// 跳过注释块@*...*@的内容，注释内容不输出
		/// </summary>
		private void SkipComment()
		{
			while (true)
			{
				var c = this.CharReader.Read();
				if (!c.HasValue)
				{
					// 注释未闭合
					throw new Exceptions.ScriptAnalyzingException($"模板注释未闭合 ({this.CharReader.CurrentLine},{this.CharReader.CurrentColumn})");
				}
				if (c.Value != '*') continue;
				var next = this.CharReader.Peek();
				if (next.HasValue && next.Value == '@')
				{
					// 注释结束
					this.CharReader.Read();
					_state = ETemplateState.Text;
					// 注释独占一行时，移除该行的换行符
					if (_lineOnlyScript) SkipLineBreak();
					return;
				}
			}
		}

		/// <summary>
		/// 判断记号是否为 include 方法名
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool IsIncludeToken(Token token)
		{
			if (string.IsNullOrEmpty(this.IncludeMethod)) return false;
			return token.Type == ETokenType.Word && token.Value == this.IncludeMethod;
		}

		/// <summary>
		/// 识别 include 调用，识别成功则切换字符流到包含的模板文件
		/// </summary>
		/// <param name="token">脚本记号</param>
		/// <param name="checkMemberAccess">是否检查成员访问</param>
		/// <returns></returns>
		private bool TryReadInclude(Token token, bool checkMemberAccess = true)
		{
			if (!IsIncludeToken(token)) return false;
			// 成员访问（如 obj.include(...)）不作为 include 处理
			if (checkMemberAccess && _lastToken.HasValue && _lastToken.Value.IsSymbol(".")) return false;
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
		/// <param name="checkMemberAccess">是否检查成员访问</param>
		/// <returns></returns>
		private async Task<bool> TryReadIncludeAsync(Token token, bool checkMemberAccess, CancellationToken cancellationToken)
		{
			if (!IsIncludeToken(token)) return false;
			// 成员访问（如 obj.include(...)）不作为 include 处理
			if (checkMemberAccess && _lastToken.HasValue && _lastToken.Value.IsSymbol(".")) return false;
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
		/// 字符级确认 include 后面是否紧跟 '('（允许空白和换行），避免记号级前瞻跨行读取
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
				if (IsWhiteSpace(c.Value)) continue;
				var isCall = c.Value == '(';
				UnreadChars(consumed);
				return isCall;
			}
		}

		/// <summary>
		/// 异步字符级确认 include 后面是否紧跟 '('（允许空白和换行），避免记号级前瞻跨行读取
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
				if (IsWhiteSpace(c.Value)) continue;
				var isCall = c.Value == '(';
				UnreadChars(consumed);
				return isCall;
			}
		}

		/// <summary>
		/// 是否空白字符
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private static bool IsWhiteSpace(char c)
		{
			return c == ' ' || c == '\t' || c == '\r' || c == '\n';
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
			// 包含的模板文件会输出内容
			_lineHasOutput = true;
			// 保存宿主信息
			_includeStack.Push(new IncludeFrame
			{
				Stream = this.TokenStream,
				State = _state,
				BraceDepth = _braceDepth,
				BlockEnd = _blockEnd,
				ExpressionTokenCount = _expressionTokenCount,
				LineHasText = _lineHasText,
				LineHasOutput = _lineHasOutput,
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
			_blockEnd = frame.BlockEnd;
			_expressionTokenCount = frame.ExpressionTokenCount;
			_lineHasText = frame.LineHasText;
			_lineHasOutput = frame.LineHasOutput;
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
			else
			{
				// 不是换行符
				return;
			}
			// 换行符被移除，进入新的一行
			_lineHasText = false;
			_lineHasOutput = false;
		}

		/// <summary>
		/// 切换到新的状态，如果当前有文本则先输出文本
		/// </summary>
		private Token? EnterState(ETemplateState state, int braceDepth, char blockEnd = '}', CancellationToken? cancellationToken = null)
		{
			_braceDepth = braceDepth;
			if (state == ETemplateState.ScriptBlock)
			{
				// 脚本块的结束符（'}' 或 ')'）
				_blockEnd = blockEnd;
			}
			if (state == ETemplateState.ScriptBlock || state == ETemplateState.ScriptLine || state == ETemplateState.Comment)
			{
				// 脚本块、脚本行和注释块前的缩进不属于输出内容，不输出
				RemoveIndent();
			}
			if (state == ETemplateState.ExpressionBegin)
			{
				_expressionTokenCount = 0;
				// 表达式会输出内容
				_lineHasOutput = true;
			}
			if (_buffer.Length > 0)
			{
				_state = state;
				//return EmitWriteCall(BuildTextToken());
				return cancellationToken.HasValue ? BuildWriteStringAsyncToken(cancellationToken.Value) : BuildWriteStringToken();
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
			else if (!_lineOnlyScript)
			{
				// 整个缓冲区都是空白，但当前行已有内容（表达式、输出等），不是行首缩进
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
			// 记录当前行的内容，用于判断脚本行是否单独占一行
			if (c == '\n' || c == '\r')
			{
				// 换行符，新的一行
				_lineHasText = false;
				_lineHasOutput = false;
			}
			else if (!char.IsWhiteSpace(c))
			{
				_lineHasText = true;
			}
		}

		//private Token BuildTextToken()
		//{
		//	var token = new Token(_buffer.ToString(), ETokenType.String, _textLine, _textColumn);
		//	_buffer.Clear();
		//	_textEmitted = true;
		//	return token;
		//}

		private Token BuildWriteStringToken()
		{
			var s = _buffer.ToString();
			var callFuncNode = new CallFuncNode
			{
				Method = this.OutputMethod,
				Args = new[] { PoolManage.CreateObjectNode(s) }
			};
			if (this.Output == null)
			{
				callFuncNode.Target = PoolManage.CreateVariableNode(this.OutputVar);
			}
			else
			{
				callFuncNode.Target = this.Output;
				callFuncNode.Delegate = new Action(() => this.Output.Write(s));
			}
			_buffer.Clear();
			return new Token(callFuncNode, _textLine, _textColumn);
		}

		private Token BuildWriteStringAsyncToken(CancellationToken cancellationToken)
		{
			var s = _buffer.ToString();
			var callFuncNode = new CallFuncNode
			{
				Method = this.OutputMethodAsync,
				Args = new[] { PoolManage.CreateObjectNode(s), PoolManage.CreateObjectNode(cancellationToken) }
			};
			if (this.Output == null)
			{
				callFuncNode.Target = PoolManage.CreateVariableNode(this.OutputVar);
				// 编译缓存不能用参数中的cancellationToken
				((ObjectNode)callFuncNode.Args[1]).Data = default(CancellationToken);
			}
			else
			{
				callFuncNode.Target = this.Output;
				callFuncNode.Delegate = new Func<Task>(() => this.Output.WriteAsync(s, cancellationToken));
			}
			var awaitFuncNode = new CallFuncNode
			{
				Name = "await",
				Args = new ITreeNode[] { callFuncNode }
			};
			_buffer.Clear();
			return new Token(awaitFuncNode, _textLine, _textColumn);
		}

		///// <summary>
		///// 组合 output.Write(...) 调用
		///// </summary>
		//private Token EmitWriteCall(Token arg)
		//{
		//	Push(new Token(";", ETokenType.None));
		//	Push(new Token(")", ETokenType.None));
		//	Push(arg);
		//	Push(new Token("(", ETokenType.None));
		//	Push(new Token(OutputMethod.Name, ETokenType.Word));
		//	Push(new Token(".", ETokenType.Operator));
		//	return this.Output == null ? new Token(this.OutputVar, ETokenType.Word) : new Token(this.Output);
		//}

		///// <summary>
		///// 组合 await output.WriteAsync(...) 调用
		///// </summary>
		//private Token EmitWriteCallForAsync(Token arg)
		//{
		//	Push(new Token(";", ETokenType.None));
		//	Push(new Token(")", ETokenType.None));
		//	Push(arg);
		//	Push(new Token("(", ETokenType.None));
		//	Push(new Token(OutputMethod.Name, ETokenType.Word));
		//	Push(new Token(".", ETokenType.Operator));
		//	Push(this.Output == null ? new Token(this.OutputVar, ETokenType.Word) : new Token(this.Output));
		//	return new Token("await", ETokenType.Word);
		//}

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
							// 表达式结束（空表达式）或已切换到包含的模板文件，继续读取
							if (!token.HasValue) break;
							if (IsIncludeToken(token.Value))
							{
								// 表达式为include调用，包含的模板内容直接输出，不加output.Write包装
								// 先清零表达式记号数，使包含模板结束后宿主表达式能正确结束
								var tokenCount = _expressionTokenCount;
								_expressionTokenCount = 0;
								if (await TryReadIncludeAsync(token.Value, false, cancellationToken).ConfigureAwait(false)) break;
								// 不是include('路径')格式，按普通表达式处理
								_expressionTokenCount = tokenCount;
							}
							_state = ETemplateState.Expression;
							// await output.WriteAsync(token0
							Push(token.Value);
							Push(new Token("(", ETokenType.None));
							Push(new Token(OutputMethodAsync.Name, ETokenType.Word));
							Push(new Token(".", ETokenType.Operator));
							Push(this.Output == null ? new Token(this.OutputVar, ETokenType.Word) : new Token(this.Output));
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
					case ETemplateState.Comment:
						{
							await SkipCommentAsync(cancellationToken).ConfigureAwait(false);
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
						//token = EmitWriteCallForAsync(BuildTextToken());
						token = BuildWriteStringAsyncToken(cancellationToken);
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
				//return EmitWriteCallForAsync(BuildTextToken());
				return BuildWriteStringAsyncToken(cancellationToken);
			}
			if (c.Value == '$')
			{
				var next = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
				if (next.HasValue && next.Value == '{')
				{
					// 表达式${...}
					await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					return EnterState(ETemplateState.ExpressionBegin, 1, cancellationToken: cancellationToken);
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
				return EnterState(ETemplateState.ScriptBlock, 1, cancellationToken: cancellationToken);
			}
			if (nextChar.HasValue && nextChar.Value == '(')
			{
				// 脚本块@(...)，功能与@{...}一致，只是结束符为')'
				await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				return EnterState(ETemplateState.ScriptBlock, 1, ')', cancellationToken: cancellationToken);
			}
			if (nextChar.HasValue && nextChar.Value == '*')
			{
				// 注释块@*...*@
				await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				return EnterState(ETemplateState.Comment, 0, cancellationToken: cancellationToken);
			}
			// 脚本行@...
			return EnterState(ETemplateState.ScriptLine, 0, cancellationToken: cancellationToken);
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
					Push(new Token(")", ETokenType.None, t.Value.Line, t.Value.Column));
					Push(new Token("@@CancellationToken", ETokenType.Word));
					return new Token(",", ETokenType.None);
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
		/// 异步读取脚本块@{...}或@(...)的内容，每次读取一个记号
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
			if (await TryReadIncludeAsync(t.Value, true, cancellationToken).ConfigureAwait(false))
			{
				// 已切换到包含的模板文件
				return null;
			}
			if (IsBlockEnd(t.Value))
			{
				_braceDepth--;
				if (_braceDepth <= 0)
				{
					// 脚本块结束，块尾的结束符不输出
					_state = ETemplateState.Text;
					// 脚本块的换行符不输出
					await SkipLineBreakAsync(cancellationToken).ConfigureAwait(false);
					return null;
				}
			}
			else if (IsBlockBegin(t.Value))
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
				// 脚本行结束
				if (_lineOnlyScript)
				{
					// 脚本行单独占一行，移除一个换行符
					if (c.Value == '\r')
					{
						var next = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
						if (next.HasValue && next.Value == '\n')
						{
							await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
						}
					}
					// 换行符被移除，进入新的一行
					_lineHasText = false;
					_lineHasOutput = false;
				}
				else
				{
					// 脚本行未单独占一行，换行符保留，作为文本输出
					this.CharReader.Push(c.Value);
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
			if (await TryReadIncludeAsync(t.Value, true, cancellationToken).ConfigureAwait(false))
			{
				// 已切换到包含的模板文件
				return null;
			}
			_lastToken = t.Value;
			return t.Value;
		}

		/// <summary>
		/// 异步跳过注释块@*...*@的内容，注释内容不输出
		/// </summary>
		private async Task SkipCommentAsync(CancellationToken cancellationToken)
		{
			while (true)
			{
				var c = await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				if (!c.HasValue)
				{
					// 注释未闭合
					throw new Exceptions.ScriptAnalyzingException($"模板注释未闭合 ({this.CharReader.CurrentLine},{this.CharReader.CurrentColumn})");
				}
				if (c.Value != '*') continue;
				var next = await this.CharReader.PeekAsync(cancellationToken).ConfigureAwait(false);
				if (next.HasValue && next.Value == '@')
				{
					// 注释结束
					await this.CharReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					_state = ETemplateState.Text;
					// 注释独占一行时，移除该行的换行符
					if (_lineOnlyScript) await SkipLineBreakAsync(cancellationToken).ConfigureAwait(false);
					return;
				}
			}
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
			else
			{
				// 不是换行符
				return;
			}
			// 换行符被移除，进入新的一行
			_lineHasText = false;
			_lineHasOutput = false;
		}
	}
}
