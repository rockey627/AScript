using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Test.MSTests.Sql
{
	/// <summary>
	/// id=6
	/// </summary>
	public class EqualTokenHandler : ITokenHandler
	{
		public static readonly EqualTokenHandler Instance = new EqualTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				var op = new OperatorNode("=", DefaultSyntaxAnalyzer.OperatorPriorities["=="], 2);
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
			}
		}
	}
}
