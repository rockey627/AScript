using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Python3.TokenHandlers
{
	public class Python3InTokenHandler : ITokenHandler
	{
		public static readonly Python3InTokenHandler Instance = new Python3InTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			var opNode = PoolManage.CreateOperatorNode("in", 2, DefaultSyntaxAnalyzer.OperatorPriorities["."] - 1);
			e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, opNode);
		}
	}
}
