using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class IndexTokenHandler : ITokenHandler
	{
		public static readonly IndexTokenHandler Instance = new IndexTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			if (e.TreeBuilder.Current == null)
			{
				throw new Exception($"invalid expression '[' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			e.IsHandled = true;
			var statement0 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, "]", e.CurrentToken.Line, e.CurrentToken.Column);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("[]", 2, DefaultSyntaxAnalyzer.OperatorPriorities["["]));
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, statement0);
			}
		}
	}
}
