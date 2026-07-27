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

			tokenType = null;
			return false;
		}
	}
}
