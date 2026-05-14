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
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, QueryNode.Keywords);
			if (!e.Ignore)
			{
				(e.TreeBuilder.Current as QueryNode).AddSelect(condition);
			}
		}
	}
}
