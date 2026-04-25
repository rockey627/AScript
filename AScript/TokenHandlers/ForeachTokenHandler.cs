using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class ForeachTokenHandler : ITokenHandler
	{
		public static readonly ForeachTokenHandler Instance = new ForeachTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			analyzer.ValidateNextToken(e.TokenReader, "(");
			// 类型
			var nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word, expect: "type word");
			var itemType = nextToken.Value.Value;
			// 变量名
			nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word, expect: "var word");
			string itemName = nextToken.Value.Value;
			// in
			analyzer.ValidateNextToken(e.TokenReader, "in");

			var listBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			var createFullTreeNodeOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var bodyBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullTreeNodeOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

			if (e.Ignore) return;

			var foreachNode = new ForeachNode
			{
				VarDefine = PoolManage.CreateDefineVarNode(itemName, itemType),
				Collection = listBuilder,
				Body = bodyBuilder
			};
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
		}
	}
}
