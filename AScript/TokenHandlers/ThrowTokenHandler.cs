using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	public class ThrowTokenHandler : ITokenHandler
	{
		public static readonly ThrowTokenHandler Instance = new ThrowTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			var ex = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new ThrowNode(ex));
			}
		}
	}
}
