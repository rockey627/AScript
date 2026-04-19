using AScript.Readers;
using System;

namespace AScript.Lang.Python3
{
	public class Python3TokenStream : DefaultTokenStream
	{
		public Python3TokenStream(CharReader charReader) : base(charReader)
		{
		}

		protected override bool TryParseAnnotate(char currentChar)
		{
			if (currentChar == '#')
			{
				// 行注释
				SkipLine();
				return true;
			}
			return false;
		}

		protected override bool TryCustomParse(char currentChar, out ETokenType? tokenType)
		{
			// 由三个单引号 ''' 或三个双引号 """ 包围的文本块用作多行文本
			if (currentChar == '\'' || currentChar == '"')
			{
				tokenType = null;
				var c2 = CharReader.Read();
				if (!c2.HasValue) return false;
				if (c2.Value != currentChar)
				{
					CharReader.Push(c2.Value);
					return false;
				}
				var c3 = CharReader.Read();
				if (!c3.HasValue) return false;
				if (c3.Value != currentChar)
				{
					CharReader.Push(c3.Value);
					CharReader.Push(c2.Value);
					return false;
				}

				var d = CharReader.Read();
				while (d.HasValue)
				{
					if (d != currentChar)
					{
						_buffer.Append(d.Value);
						d = CharReader.Read();
						continue;
					}

					var d2 = CharReader.Read();
					if (!d2.HasValue) break;
					if (d2.Value != currentChar)
					{
						_buffer.Append(d.Value);
						_buffer.Append(d2.Value);
						d = CharReader.Read();
						continue;
					}

					var d3 = CharReader.Read();
					if (!d3.HasValue) break;
					if (d3.Value != currentChar)
					{
						_buffer.Append(d.Value);
						_buffer.Append(d2.Value);
						_buffer.Append(d3.Value);
						d = CharReader.Read();
						continue;
					}

					break;
				}

				tokenType = ETokenType.String;
				return true;
			}
			return base.TryCustomParse(currentChar, out tokenType);
		}
	}
}
