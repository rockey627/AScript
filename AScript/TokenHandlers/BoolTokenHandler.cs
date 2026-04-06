using System;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class BoolTokenHandler : ITokenHandler
	{
		public static readonly BoolTokenHandler Instance = new BoolTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.Ignore) return;

			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, Convert.ToBoolean(e.CurrentToken.Value), typeof(bool));
		}
	}
}
