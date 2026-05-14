using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	public class SelectTokenHandler : ITokenHandler
	{
		public static readonly SelectTokenHandler Instance = new SelectTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				if (!(e.TreeBuilder.Current is QueryNode))
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
				}
			}
			var buildOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var selector = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, QueryNode.Keywords);
			if (!e.Ignore)
			{
				(e.TreeBuilder.Current as QueryNode).AddSelect(selector);
			}
		}
	}
}
