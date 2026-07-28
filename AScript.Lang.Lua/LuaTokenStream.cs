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
