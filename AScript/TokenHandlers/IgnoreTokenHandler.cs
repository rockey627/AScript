using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	public class IgnoreTokenHandler : ITokenHandler
	{
		public static readonly IgnoreTokenHandler Instance = new IgnoreTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
		}
	}
}
