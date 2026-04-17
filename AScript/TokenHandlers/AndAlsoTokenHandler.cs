using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// <![CDATA[age>20 and age<50]]>
	/// </summary>
	public class AndAlsoTokenHandler : ITokenHandler
	{
		public static readonly AndAlsoTokenHandler Instance = new AndAlsoTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				var op = new OperatorNode(e.CurrentToken.Value, DefaultSyntaxAnalyzer.OperatorPriorities["&&"], 2);
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
			}
		}
	}
}
