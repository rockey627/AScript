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
