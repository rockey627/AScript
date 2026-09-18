using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	public class GoMapTokenHandler : ITokenHandler
	{
		public static readonly GoMapTokenHandler Instance = new GoMapTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var mapType = GoLang.ParseMapType(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Ignore);
			var mapValue = GoLang.ParseMapValue(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, mapType);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, mapValue);
			}
		}
	}
}
