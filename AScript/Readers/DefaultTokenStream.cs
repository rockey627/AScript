using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Readers
{
	public class DefaultTokenStream : ITokenStream, IDisposable
	{
		private static readonly HashSet<char> _SpaceChars = new HashSet<char> { ' ', '\r', '\t', '\n' };
		private static readonly HashSet<char> _OperatorChars = new HashSet<char> { '=', '+', '-', '*', '/', '&', '|', '>', '<', '!', '^', '%', '~', '.', '?', ':' };
		private static readonly HashSet<char> _SingleChars = new HashSet<char> { ';', ',', '(', ')', '{', '}', '[', ']' };

		private readonly StringBuilder _buffer = new StringBuilder();

		private readonly CharReader _reader;

		public CharReader CharReader => _reader;

		public DefaultTokenStream(string expression) : this(new StringCharStream(expression)) { }
		public DefaultTokenStream(Stream expression, bool autoDisposeStream) : this(new StreamCharStream(expression, autoDisposeStream)) { }
		public DefaultTokenStream(ICharStream stream) : this(new CharReader(stream, true)) { }
		public DefaultTokenStream(CharReader charReader)
		{
			_reader = charReader;
		}

		public Token? Next()
		{
			var c = _reader.Read();
			var startLine = _reader.CurrentLine;
			var startColumn = _reader.CurrentColumn;
			while (c.HasValue)
			{
				if (TryParseAnnotate(c.Value))
				{
					if (_buffer.Length > 0) break;
					c = _reader.Read();
					continue;
				}
				// 
				if (c.Value == '.')
				{
					if (_buffer.Length > 0)
					{
						char startChar = _buffer[0];
						// 如果前面是数字，则判断小数点后面如果不是数字则返回
						if (IsNumber(startChar))
						{
							var nextChar2 = _reader.Read();
							if (nextChar2.HasValue && !IsNumber(nextChar2.Value))
							{
								_reader.Push(nextChar2.Value);
								_reader.Push(c.Value);
								break;
							}
							_buffer.Append(c.Value);
							_buffer.Append(nextChar2.Value);
							c = _reader.Read();
							continue;
						}
						else
						{
							_reader.Push(c.Value);
							// 如果前面不是数字，则返回前面的
							break;
						}
					}
					var nextChar = _reader.Peek();
					if (nextChar.HasValue && !IsNumber(nextChar.Value))
					{
						_buffer.Append(c.Value);
						break;
					}
					_buffer.Append(c.Value);
					c = _reader.Read();
					continue;
				}
				// 
				if (IsString(c.Value))
				{
					if (_buffer.Length > 0)
					{
						_reader.Push(c.Value);
						// 返回前面的token
						break;
					}
					ParseString(c.Value);
					string s = _buffer.ToString();
					_buffer.Clear();
					return new Token(s, ETokenType.String, startLine, startColumn);
				}
				// 
				if (IsSpace(c.Value))
				{
					if (_buffer.Length > 0)
					{
						_reader.Push(c.Value);
						break;
					}
					c = _reader.Read();
					startLine = _reader.CurrentLine;
					startColumn = _reader.CurrentColumn;
					continue;
				}
				if (IsSingleChar(c.Value))
				{
					if (_buffer.Length == 0)
					{
						_buffer.Append(c.Value);
					}
					else
					{
						_reader.Push(c.Value);
					}
					break;
				}
				if (IsOperator(c.Value))
				{
					if (_buffer.Length > 0 && !IsOperator(_buffer[0]))
					{
						_reader.Push(c.Value);
						break;
					}
					_buffer.Append(c.Value);
					c = _reader.Read();
					continue;
				}
				if (_buffer.Length > 0)
				{
					char startChar2 = _buffer[0];
					if (startChar2 == '.' && IsNumber(c.Value))
					{
						_buffer.Append(c.Value);
						c = _reader.Read();
						continue;
					}
					if (IsOperator(startChar2))
					{
						_reader.Push(c.Value);
						break;
					}
				}
				_buffer.Append(c.Value);
				c = _reader.Read();
			}
			// 
			if (_buffer.Length == 0) return null;
			string v = _buffer.ToString();
			_buffer.Clear();
			return new Token(v, GetTokenType(v), startLine, startColumn);
		}

		public async Task<Token?> NextAsync()
		{
			var c = await _reader.ReadAsync().ConfigureAwait(false);
			var startLine = _reader.CurrentLine;
			var startColumn = _reader.CurrentColumn;
			while (c.HasValue)
			{
				if (TryParseAnnotate(c.Value))
				{
					if (_buffer.Length > 0) break;
					c = await _reader.ReadAsync().ConfigureAwait(false);
					continue;
				}
				// 
				if (c.Value == '.')
				{
					if (_buffer.Length > 0)
					{
						char startChar = _buffer[0];
						// 如果前面是数字，则判断小数点后面如果不是数字则返回
						if (IsNumber(startChar))
						{
							var nextChar2 = await _reader.ReadAsync().ConfigureAwait(false);
							if (nextChar2.HasValue && !IsNumber(nextChar2.Value))
							{
								_reader.Push(nextChar2.Value);
								_reader.Push(c.Value);
								break;
							}
							_buffer.Append(c.Value);
							_buffer.Append(nextChar2.Value);
							c = await _reader.ReadAsync().ConfigureAwait(false);
							continue;
						}
						else
						{
							_reader.Push(c.Value);
							// 如果前面不是数字，则返回前面的
							break;
						}
					}
					var nextChar = await _reader.PeekAsync().ConfigureAwait(false);
					if (nextChar.HasValue && !IsNumber(nextChar.Value))
					{
						_buffer.Append(c.Value);
						break;
					}
					_buffer.Append(c.Value);
					c = await _reader.ReadAsync().ConfigureAwait(false);
					continue;
				}
				// 
				if (IsString(c.Value))
				{
					if (_buffer.Length > 0)
					{
						_reader.Push(c.Value);
						// 返回前面的token
						break;
					}
					ParseString(c.Value);
					string s = _buffer.ToString();
					_buffer.Clear();
					return new Token(s, ETokenType.String, startLine, startColumn);
				}
				// 
				if (IsSpace(c.Value))
				{
					if (_buffer.Length > 0)
					{
						break;
					}
					c = await _reader.ReadAsync().ConfigureAwait(false);
					startLine = _reader.CurrentLine;
					startColumn = _reader.CurrentColumn;
					continue;
				}
				if (IsSingleChar(c.Value))
				{
					if (_buffer.Length == 0)
					{
						_buffer.Append(c.Value);
					}
					else
					{
						_reader.Push(c.Value);
					}
					break;
				}
				if (IsOperator(c.Value))
				{
					if (_buffer.Length > 0 && !IsOperator(_buffer[0]))
					{
						_reader.Push(c.Value);
						break;
					}
					_buffer.Append(c.Value);
					c = await _reader.ReadAsync().ConfigureAwait(false);
					continue;
				}
				if (_buffer.Length > 0)
				{
					char startChar2 = _buffer[0];
					if (startChar2 == '.' && IsNumber(c.Value))
					{
						_buffer.Append(c.Value);
						c = await _reader.ReadAsync().ConfigureAwait(false);
						continue;
					}
					if (IsOperator(startChar2))
					{
						_reader.Push(c.Value);
						break;
					}
				}
				_buffer.Append(c.Value);
				c = await _reader.ReadAsync().ConfigureAwait(false);
			}
			// 
			if (_buffer.Length == 0) return null;
			string v = _buffer.ToString();
			_buffer.Clear();
			return new Token(v, GetTokenType(v), startLine, startColumn);
		}

		protected virtual bool TryParseAnnotate(char currentChar)
		{
			if (currentChar != '/') return false;
			var nextChar = _reader.Peek();
			if (!nextChar.HasValue) return false;
			// 行注释
			if (nextChar == '/')
			{
				_reader.Read();
				//nextChar = _reader.Read();
				//while (nextChar.HasValue && nextChar.Value != '\n')
				//{
				//	nextChar = _reader.Read();
				//}
				SkipLine();
				return true;
			}
			// 块注释
			if (nextChar == '*')
			{
				_reader.Read();
				nextChar = _reader.Read();
				while (nextChar.HasValue)
				{
					if (nextChar.Value == '*')
					{
						nextChar = _reader.Peek();
						if (nextChar.HasValue && nextChar.Value == '/') break;
					}
					nextChar = _reader.Read();
				}
				_reader.Read();
				return true;
			}
			// 
			return false;
		}

		/// <summary>
		/// 跳过行
		/// </summary>
		protected void SkipLine()
		{
			var nextChar = _reader.Read();
			while (nextChar.HasValue && nextChar.Value != '\n')
			{
				nextChar = _reader.Read();
			}
			if (nextChar.HasValue)
			{
				_reader.Push(nextChar.Value);
			}
		}

		protected virtual void ParseString(char startChar)
		{
			// 读取字符串，需要注意字符串中的转义字符，如：\' \" \r \n \t 等
			bool prevEscape = false;
			var c = _reader.Read();
			while (c.HasValue)
			{
				if (c == '\\')
				{
					if (prevEscape)
					{
						prevEscape = false;
						_buffer.Append(c);
					}
					else
					{
						prevEscape = true;
					}
					c = _reader.Read();
					continue;
				}
				if (c == startChar && !prevEscape)
				{
					break;
				}

				if (!prevEscape)
				{
					_buffer.Append(c);
					c = _reader.Read();
					continue;
				}

				prevEscape = false;
				if (c == startChar)
				{
					_buffer.Append(c);
				}
				else if (c == 'n')
				{
					_buffer.Append('\n');
				}
				else if (c == 'r')
				{
					_buffer.Append('\r');
				}
				else if (c == 't')
				{
					_buffer.Append('\t');
				}
				else throw new Exception("unknown string escape:\\" + c);

				c = _reader.Read();
			}
			if (!c.HasValue)
			{
				throw new Exception($"invalid string at ({_reader.CurrentLine},{_reader.CurrentColumn}), expect {startChar}");
			}
		}

		public virtual ETokenType GetTokenType(string token)
		{
			if (string.IsNullOrEmpty(token)) return ETokenType.None;
			char c0 = token[0];
			if (c0 == '.')
			{
				if (token.Length > 1 && IsNumber(token[1]))
				{
					return ETokenType.Number;
				}
				return ETokenType.Operator;
			}
			if (IsNumber(c0)) return ETokenType.Number;
			if (IsVar(c0)) return ETokenType.Word;
			if (IsSingleChar(c0)) return ETokenType.None;
			//if (IsString(c0)) return ETokenType.String;
			if (IsOperator(c0)) return ETokenType.Operator;
			return ETokenType.None;
		}

		public virtual bool IsNumber(char c)
		{
			return c >= '0' && c <= '9';
		}

		public virtual bool IsVar(char c)
		{
			return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_' || c == '@' || c >= '\u4e00' && c <= '\u9fff';
		}

		public virtual bool IsString(char c0)
		{
			return c0 == '\'' || c0 == '"';
		}

		public virtual bool IsSpace(char c)
		{
			return _SpaceChars.Contains(c);
		}

		public virtual bool IsOperator(char c)
		{
			//return !IsVar(c) && !IsNumber(c);
			return _OperatorChars.Contains(c);
		}

		public virtual bool IsSingleChar(char c)
		{
			return _SingleChars.Contains(c);
		}

		public void Dispose()
		{
			(_reader as IDisposable)?.Dispose();
		}
	}
}
