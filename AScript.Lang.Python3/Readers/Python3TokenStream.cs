using AScript.Readers;
using System;

namespace AScript.Lang.Python3.Readers
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
			// 由三个单引号 ''' 或三个双引号 """ 包围的文本块用作多行注释
			if (currentChar == '\'' || currentChar == '"')
			{
				var c2 = this.CharReader.Read();
				if (!c2.HasValue) return false;
				if (c2.Value != currentChar)
				{
					this.CharReader.Push(c2.Value);
					return false;
				}
				var c3 = this.CharReader.Read();
				if (!c3.HasValue) return false;
				if (c3.Value != currentChar)
				{
					this.CharReader.Push(c3.Value);
					this.CharReader.Push(c2.Value);
					return false;
				}

				var d = this.CharReader.Read();
				while (d.HasValue)
				{
					if (d != currentChar)
					{
						d = this.CharReader.Read();
						continue;
					}

					var d2 = this.CharReader.Read();
					if (!d2.HasValue) break;
					if (d2.Value != currentChar)
					{
						d = this.CharReader.Read();
						continue;
					}

					var d3 = this.CharReader.Read();
					if (!d3.HasValue) break;
					if (d3.Value != currentChar)
					{
						d = this.CharReader.Read();
						continue;
					}

					break;
				}

				return true;
			}
			return false;
		}
	}
}
