using AScript.Readers;
using System;

namespace AScript.Lang.Go
{
	/// <summary>
	/// Go语言的词法分析器
	/// </summary>
	public class GoTokenStream : DefaultTokenStream
	{
		public GoTokenStream(CharReader charReader) : base(charReader)
		{
		}

	}
}
