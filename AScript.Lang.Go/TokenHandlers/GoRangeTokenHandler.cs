using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	public class GoRangeTokenHandler : ITokenHandler
	{
		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var arr = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			if (!e.Ignore)
			{
				var callNode = new CallFuncNode { Name = "range", Args = new[] { arr } };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, callNode);
			}
		}
	}
}
