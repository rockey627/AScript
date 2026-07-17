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
			if (currentChar == '\'' || currentChar == '"')
			{
				tokenType = null;
				char quote = currentChar;
				var d = CharReader.Read();
				while (d.HasValue && d.Value != quote)
				{
					if (d.Value == '\\')
					{
						var next = CharReader.Read();
						if (next.HasValue)
						{
							_buffer.Append(next.Value);
							d = CharReader.Read();
							continue;
						}
						break;
					}
					_buffer.Append(d.Value);
					d = CharReader.Read();
				}
				if (d.HasValue && d.Value == quote)
				{
					tokenType = ETokenType.String;
					return true;
				}
				CharReader.Push(d.Value);
				return false;
			}

			tokenType = null;
			return false;
		}
	}
}
