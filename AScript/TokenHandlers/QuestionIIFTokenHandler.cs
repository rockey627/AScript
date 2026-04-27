using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class QuestionIIFTokenHandler : ITokenHandler
	{
		public static readonly QuestionIIFTokenHandler Instance = new QuestionIIFTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			// 解析 ? : 操作符
			if (e.TreeBuilder.Current == null)
			{
				throw new Exception($"invalid expression '{e.CurrentToken.Value}' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			e.IsHandled = true;
			e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode(e.CurrentToken.Value, 2, DefaultSyntaxAnalyzer.OperatorPriorities[e.CurrentToken.Value]));

			var createFullNodeOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var value1 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullNodeOptions, e.TokenReader, e.Control, e.Ignore);
			if (value1 == null)
			{
				throw new Exception($"invalid expression '?' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			analyzer.ValidateNextToken(e.TokenReader, ":");
			var value2 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullNodeOptions, e.TokenReader, e.Control, e.Ignore);
			if (value2 == null)
			{
				throw new Exception($"invalid expression '?' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			var right = PoolManage.CreateOperatorNode(":", 2, DefaultSyntaxAnalyzer.OperatorPriorities[e.CurrentToken.Value] + 1);
			right.Left = value1;
			right.Right = value2;
			e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, right);
		}
	}
}
