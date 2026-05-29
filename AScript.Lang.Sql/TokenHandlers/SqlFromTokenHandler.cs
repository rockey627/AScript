using AScript.Lang.Sql.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// FROM table1 AS a1, table2 AS a2 WHERE a1.Id=a2.Id
	/// LEFT JOIN table3 AS a3 ON a1.Id=a3.Id
	/// GROUP BY a1.Name,a3.Id
	/// ORDER BY a1.Name DESC
	/// </summary>
	public class SqlFromTokenHandler : ITokenHandler
	{
		public static readonly SqlFromTokenHandler Instance = new SqlFromTokenHandler();

		private static readonly HashSet<string> _GroupByEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "having" };
		private static readonly HashSet<string> _Keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "where", "left", "right", "inner", "join", "order", "group" };
		private static readonly HashSet<string> _OrderbyEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "where", "left", "right", "inner", "join", "order", "group", "asc", "desc" };
		private static readonly HashSet<string> _TableEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "where", "left", "right", "inner", "join", "order", "group", "as" };

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
				else if (token.Value.IsSymbol("right", StringComparison.OrdinalIgnoreCase))
				{
					BuildRightJoin(analyzer, e, createFullOptions, queryNode);
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
					if (string.IsNullOrEmpty(itemName))
					{
						itemName = $"__table__";
					}
#if NETSTANDARD
					tables.Add((table, itemName));
#else
					tables.Add(Tuple.Create(table, itemName));
#endif
				}
				if (nextToken.Value.IsSymbol(",")) continue;
				if (nextToken.Value.Type == ETokenType.Word && _Keywords.Contains(nextToken.Value.Value)) break;
				break;
				//throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
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
			if (queryNode != null)
			{
				condition = new SqlTreeNodeVisitor(e.BuildContext, e.ScriptContext, queryNode).Visit(condition);
				queryNode.AddWhere(condition);
			}
		}

		private void BuildInnerJoin(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			analyzer.ValidateNextToken(e.TokenReader, "join", StringComparison.OrdinalIgnoreCase);
			var table = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _TableEndTokens);
			analyzer.ValidateNextToken(e.TokenReader, "as", StringComparison.OrdinalIgnoreCase);
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			analyzer.ValidateNextToken(e.TokenReader, "on", StringComparison.OrdinalIgnoreCase);
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			if (queryNode != null)
			{
				GroupCondition(condition, nameToken.Value.Value, out var key1, out var key2);
				queryNode.AddJoin(nameToken.Value.Value, table, key1, key2);
			}
		}

		private void BuildLeftJoin(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			analyzer.ValidateNextToken(e.TokenReader, "join", StringComparison.OrdinalIgnoreCase);
			var table = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _TableEndTokens);
			analyzer.ValidateNextToken(e.TokenReader, "as", StringComparison.OrdinalIgnoreCase);
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			analyzer.ValidateNextToken(e.TokenReader, "on", StringComparison.OrdinalIgnoreCase);
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			if (queryNode != null)
			{
				GroupCondition(condition, nameToken.Value.Value, out var key1, out var key2);
				queryNode.AddLeftJoin(nameToken.Value.Value, table, key1, key2);
			}
		}

		private void BuildRightJoin(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			analyzer.ValidateNextToken(e.TokenReader, "join", StringComparison.OrdinalIgnoreCase);
			var table = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _TableEndTokens);
			analyzer.ValidateNextToken(e.TokenReader, "as", StringComparison.OrdinalIgnoreCase);
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			analyzer.ValidateNextToken(e.TokenReader, "on", StringComparison.OrdinalIgnoreCase);
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			if (queryNode != null)
			{
				GroupCondition(condition, nameToken.Value.Value, out var key1, out var key2);
				queryNode.AddRightJoin(nameToken.Value.Value, table, key1, key2);
			}
		}

		private void GroupCondition(ITreeNode condition, string key2Name, out ITreeNode key1, out ITreeNode key2)
		{
			var list1 = new List<ITreeNode>();
			var list2 = new List<ITreeNode>();
			GroupCondition(condition, key2Name, list1, list2);
			if (list1.Count == 1)
			{
				key1 = list1[0];
				key2 = list2[0];
			}
			else
			{
				key1 = new NewNode { InitProperties = list1 };
				key2 = new NewNode { InitProperties = list2 };
			}
		}

		private void GroupCondition(ITreeNode condition, string key2Name, List<ITreeNode> list1, List<ITreeNode> list2)
		{
			if (!(condition is OperatorNode opNode))
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near join on");
			}
			var left = opNode.Left;
			var right = opNode.Right;
			if (opNode.Name == "==")
			{
				if (IsMatchKey(left, key2Name))
				{
					list1.Add(right);
					list2.Add(left);
				}
				else
				{
					list1.Add(left);
					list2.Add(right);
				}
				return;
			}
			GroupCondition(left, key2Name, list1, list2);
			GroupCondition(right, key2Name, list1, list2);
		}

		private bool IsMatchKey(ITreeNode node, string keyName)
		{
			if (!(node is OperatorNode opNode)) return false;
			if (opNode.Name != ".") return false;
			if (opNode.Left is VariableNode varNode) return keyName.Equals(varNode.Name);
			return IsMatchKey(opNode.Left, keyName);
		}

		private void BuildGroup(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			analyzer.ValidateNextToken(e.TokenReader, "by", StringComparison.OrdinalIgnoreCase);
			var list = queryNode == null ? null : new List<ITreeNode>();
			Token? nextToken;
			while (true)
			{
				var node = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _GroupByEndTokens);
				list?.Add(node);
				nextToken = e.TokenReader.Read();
				if (!nextToken.HasValue) break;
				if (nextToken.Value.IsSymbol(",")) continue;
				break;
			}
			if (nextToken.HasValue &&
				(nextToken.Value.Type == ETokenType.String || !_GroupByEndTokens.Contains(nextToken.Value.Value)))
			{
				e.TokenReader.Push(nextToken.Value);
			}
			if (queryNode != null)
			{
				new SqlTreeNodeVisitor(e.BuildContext, e.ScriptContext, queryNode).Visit(list);
				var key = list.Count == 1 ? list[0] : new NewNode { InitProperties = list };
				string intoName = "__group__";
				queryNode.AddGroup(key, null, intoName);
			}
		}

		private void BuildOrder(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			analyzer.ValidateNextToken(e.TokenReader, "by", StringComparison.OrdinalIgnoreCase);
			bool f = true;
			while (true)
			{
				var key = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _OrderbyEndTokens);
				var token = e.TokenReader.Read();
				string mode = null;

				if (token.HasValue && (token.Value.IsSymbol("asc") || token.Value.IsSymbol("desc")))
				{
					mode = token.Value.Value;
					token = e.TokenReader.Read();
				}

				if (queryNode != null)
				{
					if (f)
					{
						queryNode.AddOrderby(key, mode);
						f = false;
					}
					else queryNode.AddThenby(key, mode);
				}

				if (!token.HasValue)
				{
					break;
				}

				if (token.Value.IsSymbol(","))
				{
					continue;
				}

				e.TokenReader.Push(token.Value);
				break;
			}
		}
	}
}
