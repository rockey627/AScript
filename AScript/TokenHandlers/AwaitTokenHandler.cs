using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	public class AwaitTokenHandler : ITokenHandler
	{
		public static readonly AwaitTokenHandler Instance = new AwaitTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("await", 1, DefaultSyntaxAnalyzer.OperatorPriorities["."] - 1));
			}
		}
	}
}
