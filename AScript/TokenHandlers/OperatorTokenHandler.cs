using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// 
	/// </summary>
	public class OperatorTokenHandler : ITokenHandler
	{
		public string TargetOperator { get; private set; }

		public OperatorTokenHandler(string targetOperator)
		{
			this.TargetOperator = targetOperator;
		}

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				var op = new OperatorNode(e.CurrentToken.Value, DefaultSyntaxAnalyzer.OperatorPriorities[this.TargetOperator], 2);
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
			}
		}
	}
}
