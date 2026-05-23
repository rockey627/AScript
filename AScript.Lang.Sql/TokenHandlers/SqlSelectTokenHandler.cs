using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	public class SqlSelectTokenHandler : ITokenHandler
	{
		public static readonly SqlSelectTokenHandler Instance = new SqlSelectTokenHandler();

		private static readonly HashSet<string> _EndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "as" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var list = e.Ignore ? null : new List<ITreeNode>();
			Token? nextToken;
			while (true)
			{
				var node = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _EndTokens);

				nextToken = e.TokenReader.Read();
				if (!nextToken.HasValue)
				{
					if (node != null && list != null)
					{
						list.Add(node);
					}
					break;
				}
				if (nextToken.Value.IsSymbol("as", StringComparison.OrdinalIgnoreCase))
				{
					var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					list.Add(new OperatorNode("=", DefaultSyntaxAnalyzer.OperatorPriorities["="], 2) { Left = new VariableNode(nameToken.Value.Value), Right = node });
					nextToken = e.TokenReader.Read();
				}
				else if (node != null && list != null)
				{
					list.Add(node);
				}
				if (nextToken.Value.IsSymbol(",")) continue;
				break;
			}
			if (!nextToken.HasValue)
			{
				if (e.Ignore) return;
				if (list.Count == 1)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, list[0]);
				}
				else
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new NewNode { InitProperties = list });
				}
				return;
			}
			e.TokenReader.Push(nextToken.Value);
			if (nextToken.Value.IsSymbol("from", StringComparison.OrdinalIgnoreCase))
			{
				var fromBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				if (!e.Ignore)
				{
					var queryNode = (QueryNode)((fromBuilder is TreeBuilder treeBuilder) ? treeBuilder.Root : fromBuilder);
					if (list.Count == 1)
					{
						queryNode.AddSelect(list[0]);
					}
					else
					{
						queryNode.AddSelect(new NewNode { InitProperties = list });
					}
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, queryNode);
				}
			}
		}
	}
}
