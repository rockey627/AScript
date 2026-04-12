using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Test.MSTests.Sql
{
	/// <summary>
	/// <![CDATA[age>20 and age<50]]>
	/// </summary>
	public class AndTokenHandler : ITokenHandler
	{
		public static readonly AndTokenHandler Instance = new AndTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				var op = new OperatorNode("and", DefaultSyntaxAnalyzer.OperatorPriorities["&&"], 2);
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
			}
		}
	}
}
