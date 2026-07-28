using AScript.Readers;
using System;

namespace AScript.Lang.Lua
{
	public class LuaTokenStream : DefaultTokenStream
	{
		public LuaTokenStream(CharReader charReader) : base(charReader)
		{
		}

		protected override bool TryParseAnnotate(char currentChar)
		{
			if (currentChar == '-')
			{
				var c2 = CharReader.Read();
				if (c2.HasValue && c2.Value == '-')
				{
					var c3 = CharReader.Read();
					if (c3.HasValue && c3.Value == '[')
					{
						var c4 = CharReader.Read();
						if (c4.HasValue && c4.Value == '[')
						{
							// Lua多行注释：--[[ ... ]]
							SkipUntil("]]");
							return true;
						}
						CharReader.Push(c4.Value);
					}
					CharReader.Push(c3.Value);
					// Lua单行注释：--
					SkipLine();
					return true;
				}
				CharReader.Push(c2.Value);
			}
			return false;
		}

		protected override bool TryCustomParse(char currentChar, out ETokenType? tokenType)
		{
			// 处理Lua字符串
			if (currentChar == '#')
			{
				tokenType = ETokenType.Operator;
				_buffer.Append(currentChar);
				return true;
			}

			// [[ ]]字符串
			if (currentChar == '[')
			{
				var c2 = CharReader.Peek();
				if (c2.HasValue && c2.Value == '[')
				{
					CharReader.Read(); // consume second [
					// Parse until ]]
					var c = CharReader.Read();
					int matchIndex = 0; // 0=first ], 1=second ]
					while (c.HasValue)
					{
						if (c.Value == '\\')
						{
							var next = CharReader.Read();
							if (!next.HasValue) break;
							switch (next.Value)
							{
								case 'n': _buffer.Append('\n'); break;
								case 't': _buffer.Append('\t'); break;
								case 'r': _buffer.Append('\r'); break;
								case '\\': _buffer.Append('\\'); break;
								case 'a': _buffer.Append('\a'); break;
								case 'b': _buffer.Append('\b'); break;
								case 'f': _buffer.Append('\f'); break;
								case 'v': _buffer.Append('\v'); break;
								case 'z':
									// \z skips all whitespace following it
									var ws = CharReader.Peek();
									while (ws.HasValue && (ws.Value == ' ' || ws.Value == '\t' || ws.Value == '\n' || ws.Value == '\r'))
									{
										CharReader.Read();
										ws = CharReader.Peek();
									}
									break;
								case 'x':
									// \xHH - hex escape
									var h1 = CharReader.Read();
									var h2 = CharReader.Read();
									if (h1.HasValue && h2.HasValue &&
										((h1.Value >= '0' && h1.Value <= '9') || (h1.Value >= 'A' && h1.Value <= 'F') || (h1.Value >= 'a' && h1.Value <= 'f')) &&
										((h2.Value >= '0' && h2.Value <= '9') || (h2.Value >= 'A' && h2.Value <= 'F') || (h2.Value >= 'a' && h2.Value <= 'f')))
									{
										_buffer.Append((char)Convert.ToInt32(new string(new[] { h1.Value, h2.Value }), 16));
									}
									else
									{
										if (h1.HasValue) _buffer.Append(h1.Value);
										if (h2.HasValue) _buffer.Append(h2.Value);
									}
									break;
								default:
									if (next.Value >= '0' && next.Value <= '9')
									{
										// \ddd - octal escape (up to 3 digits)
										var d1 = next;
										var d2 = CharReader.Peek();
										var d3 = CharReader.Peek();
										int digits = 1;
										if (d2.HasValue && d2.Value >= '0' && d2.Value <= '7') { digits++; d2 = CharReader.Read(); }
										if (d3.HasValue && d3.Value >= '0' && d3.Value <= '7') { digits++; d3 = CharReader.Read(); }
										var octalStr = d1.Value.ToString();
										if (digits >= 2) octalStr += d2.Value.ToString();
										if (digits >= 3) octalStr += d3.Value.ToString();
										_buffer.Append((char)Convert.ToInt32(octalStr, 8));
									}
									else
									{
										_buffer.Append(next.Value);
									}
									break;
							}
							c = CharReader.Read();
							continue;
						}
						if (c.Value == ']' && matchIndex == 0)
						{
							matchIndex = 1;
						}
						else if (c.Value == ']' && matchIndex == 1)
						{
							break; // found ]]
						}
						else
						{
							if (matchIndex > 0)
							{
								// we had a ] but this char is not ], so output the ]
								_buffer.Append(']');
								matchIndex = 0;
							}
							_buffer.Append(c.Value);
						}
						c = CharReader.Read();
					}
					tokenType = ETokenType.String;
					return true;
				}
			}

			tokenType = null;
			return false;
		}
	}
}
