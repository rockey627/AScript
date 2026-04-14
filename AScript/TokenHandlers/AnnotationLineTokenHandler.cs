using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// 行注释
	/// </summary>
	public class AnnotationLineTokenHandler : ITokenHandler
	{
		public static readonly AnnotationLineTokenHandler Instance = new AnnotationLineTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			// 跳过行注释
			var charReader = e.TokenReader.CharReader;
			var c = charReader.Read();
			while (c.HasValue && c.Value != '\n')
			{
				c = charReader.Read();
			}
		}
	}
}
