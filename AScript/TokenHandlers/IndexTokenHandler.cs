using System;
using System.Collections.Generic;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class IndexTokenHandler : ITokenHandler
	{
		public static readonly IndexTokenHandler Instance = new IndexTokenHandler();

		private static readonly HashSet<string> _EndTokens = new HashSet<string> { ":" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			if (e.TreeBuilder.Current == null)
			{
				throw new Exception($"invalid expression '[' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			e.IsHandled = true;
			var statement0 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _EndTokens);
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exception($"invalid expression at {e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}, expect ']'");
			}
			if (nextToken.Value.Type == ETokenType.String)
			{
				throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ']'");
			}
			if (nextToken.Value.Value == ":")
			{
				// [1:3]
				var end = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				analyzer.ValidateNextToken(e.TokenReader, "]", e.CurrentToken.Line, e.CurrentToken.Column);
				if (!e.Ignore)
				{
					//e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("[:]", 3, DefaultSyntaxAnalyzer.OperatorPriorities["["]));
					//e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, statement0);
					//e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, end);
					var target = e.TreeBuilder.Pop();
					var funcNode = new CallFuncNode { Name = "[:]", Args = new[] { target, statement0, end } };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, funcNode);
				}
				return;
			}
			if (nextToken.Value.Value != "]")
			{
				throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ']'");
			}
			//analyzer.ValidateNextToken(e.TokenReader, "]", e.CurrentToken.Line, e.CurrentToken.Column);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("[]", 2, DefaultSyntaxAnalyzer.OperatorPriorities["["]));
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, statement0);
			}
		}
	}
}
