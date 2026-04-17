using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// name='tom' or id=8
	/// </summary>
	public class OrElseTokenHandler : ITokenHandler
	{
		public static readonly OrElseTokenHandler Instance = new OrElseTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				var op = new OperatorNode(e.CurrentToken.Value, DefaultSyntaxAnalyzer.OperatorPriorities["||"], 2);
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
			}
		}
	}
}
