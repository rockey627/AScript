using AScript.Lang.Sql.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// from table1 as a1, table2 as a2 where a1.Id=a2.Id
	/// </summary>
	public class SqlFromTokenHandler : ITokenHandler
	{
		public static readonly SqlFromTokenHandler Instance = new SqlFromTokenHandler();

		private static readonly HashSet<string> _OnTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "on" };
		private static readonly HashSet<string> _EqualsTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "equals" };
		private static readonly HashSet<string> _ByTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "by" };
		private static readonly HashSet<string> _Keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "where", "left", "inner", "join", "order", "group" };
		private static readonly HashSet<string> _OrderbyEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "where", "left", "inner", "join", "order", "group", "asc", "desc" };
		private static readonly HashSet<string> _TableEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "where", "left", "inner", "join", "order", "group", "as" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			var queryNode = e.Ignore ? null : new QueryNode();
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			BuildFrom(analyzer, e, createFullOptions, queryNode);
			while (true)
			{
				var token = e.TokenReader.Read();
				if (!token.HasValue) break;
				if (token.Value.IsSymbol("join", StringComparison.OrdinalIgnoreCase))
				{
					e.TokenReader.Push(token.Value);
					BuildInnerJoin(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("inner", StringComparison.OrdinalIgnoreCase))
				{
					BuildInnerJoin(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("left", StringComparison.OrdinalIgnoreCase))
				{
					BuildLeftJoin(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("where", StringComparison.OrdinalIgnoreCase))
				{
					BuildWhere(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("group", StringComparison.OrdinalIgnoreCase))
				{
					BuildGroup(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("order", StringComparison.OrdinalIgnoreCase))
				{
					BuildOrder(analyzer, e, createFullOptions, queryNode);
				}
				else
				{

					e.TokenReader.Push(token.Value);
					break;
					//throw new Exceptions.ScriptAnalyzingException($"invalid expression near from, unknow {token.Value.Value} at ({token.Value.Line},{token.Value.Column})");
				}
			}

			// 将LINQ语句添加到语法树中
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, queryNode);
			}
		}

		private void BuildFrom(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
#if NETSTANDARD
			var tables = queryNode == null ? null : new List<(ITreeNode, string)>();
#else
			var tables = queryNode == null ? null : new List<Tuple<ITreeNode, string>>();
#endif

			Token? nextToken;
			while (true)
			{
				var table = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _TableEndTokens);
				string itemName = null;
				nextToken = e.TokenReader.Read();
				if (!nextToken.HasValue) break;

				if (nextToken.Value.IsSymbol("as", StringComparison.OrdinalIgnoreCase))
				{
					nextToken = e.TokenReader.Read();
					itemName = nextToken.Value.Value;
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue) break;
				}
				if (tables != null)
				{
#if NETSTANDARD
					tables.Add((table, itemName));
#else
					tables.Add(Tuple.Create(table, itemName));
#endif
				}
				if (nextToken.Value.IsSymbol(",")) continue;
				if (nextToken.Value.Type == ETokenType.Word && _Keywords.Contains(nextToken.Value.Value)) break;
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			if (nextToken.HasValue)
			{
				e.TokenReader.Push(nextToken.Value);
			}

			if (tables != null && tables.Count > 0)
			{
				foreach (var item in tables)
				{
					queryNode.AddFrom(item.Item2, item.Item1);
				}
			}
		}

		private void BuildWhere(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			queryNode?.AddWhere(condition);
		}

		private void BuildInnerJoin(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{

		}

		private void BuildLeftJoin(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{

		}

		private void BuildGroup(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{

		}

		private void BuildOrder(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{

		}
	}
}
