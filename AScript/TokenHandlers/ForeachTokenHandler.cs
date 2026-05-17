using System;
using System.Collections.Generic;
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
			var nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			var itemType = nextToken.Value.Value;
			string itemName = null;
			List<DefineVarNode> items = null;
			// 变量名
			nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken.Value} expression at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}), expect var word");
			}
			if (nextToken.Value.Type == ETokenType.Word)
			{
				itemName = nextToken.Value.Value;
			}
			else if (nextToken.Value.IsSymbol("("))
			{
				items = new List<DefineVarNode>();
				while (true)
				{
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken.Value} expression at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
					}
					if (nextToken.Value.Type != ETokenType.Word)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken.Value} expression '{nextToken.Value.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}), expect var word");
					}
					items.Add(PoolManage.CreateDefineVarNode(nextToken.Value.Value, null, typeof(object)));
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken.Value} expression at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
					}
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol(")")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken.Value} expression '{nextToken.Value.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}), expect ')'");
				}
			}
			else
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken.Value} expression '{nextToken.Value.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}), expect var word");
			}
			// in
			analyzer.ValidateNextToken(e.TokenReader, "in");

			var listBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			var createFullTreeNodeOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var bodyBuilder = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullTreeNodeOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

			if (e.Ignore) return;

			var foreachNode = new ForeachNode
			{
				VarDefine = items == null ? PoolManage.CreateDefineVarNode(itemName, itemType) : null,
				VarDefines = items,
				Collection = listBuilder,
				Body = bodyBuilder
			};
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
		}
	}
}
