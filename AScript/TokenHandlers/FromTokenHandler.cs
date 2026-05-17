using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// from a in query1
	/// from b in query2
	/// join c in query3 on a.Id equals c.Id into cc
	/// from c in cc.DefaultIfEmpty
	/// orderby b.Age desc
	/// group a by a.Name into g
	/// select new { Name = g.Key, Count = g.Count() }
	/// </summary>
	public class FromTokenHandler : ITokenHandler
	{
		public static readonly FromTokenHandler Instance = new FromTokenHandler();

		private static readonly HashSet<string> _OnTokens = new HashSet<string> { "on" };
		private static readonly HashSet<string> _EqualsTokens = new HashSet<string> { "equals" };
		private static readonly HashSet<string> _ByTokens = new HashSet<string> { "by" };
		private static readonly HashSet<string> _Keywords = new HashSet<string> { "from", "where", "join", "select", "orderby", "group" };
		private static readonly HashSet<string> _JoinEndTokens = new HashSet<string> { "from", "where", "join", "select", "orderby", "group", "into" };
		private static readonly HashSet<string> _OrderbyEndTokens = new HashSet<string> { "from", "where", "join", "select", "orderby", "group", "ascending", "descending", "asc", "desc" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			var queryNode = e.Ignore ? null : new QueryNode();
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			// 解析from语句
			BuildFrom(analyzer, e, createFullOptions, queryNode);
			// 解析后续linq语句
			while (true)
			{
				var token = e.TokenReader.Read();
				if (!token.HasValue) break;
				if (token.Value.IsSymbol(";"))
				{
					e.TokenReader.Push(token.Value);
					break;
				}
				if (token.Value.IsSymbol("from"))
				{
					BuildFrom(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("join"))
				{
					BuildJoin(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("where"))
				{
					BuildWhere(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("select"))
				{
					BuildSelect(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("group"))
				{
					BuildGroup(analyzer, e, createFullOptions, queryNode);
				}
				else if (token.Value.IsSymbol("orderby"))
				{
					BuildOrderby(analyzer, e, createFullOptions, queryNode);
				}
				else
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression near from, unknow {token.Value.Value} at ({token.Value.Line},{token.Value.Column})");
				}
			}
			// 将LINQ语句添加到语法树中
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, queryNode);
			}
		}

		/// <summary>
		/// from a in q1
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		/// <param name="createFullOptions"></param>
		/// <param name="queryNode"></param>
		/// <exception cref="Exceptions.ScriptAnalyzingException"></exception>
		private void BuildFrom(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			analyzer.ValidateNextToken(e.TokenReader, "in");
			var source = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			queryNode?.AddFrom(varToken.Value.Value, source);
		}

		/// <summary>
		/// from a in q1
		/// join b in q2 on a.Id equals b.Id into cc
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		/// <param name="createFullOptions"></param>
		/// <param name="queryNode"></param>
		/// <exception cref="Exceptions.ScriptAnalyzingException"></exception>
		private void BuildJoin(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			analyzer.ValidateNextToken(e.TokenReader, "in");

			var source = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _OnTokens);

			analyzer.ValidateNextToken(e.TokenReader, "on");

			var key1 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _EqualsTokens);
			analyzer.ValidateNextToken(e.TokenReader, "equals");
			var key2 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _JoinEndTokens);

			string intoName = null;
			var intoToken = e.TokenReader.Read();
			if (intoToken.HasValue)
			{
				if (intoToken.Value.IsSymbol("into"))
				{
					var intoNameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					intoName = intoNameToken.Value.Value;
				}
				else
				{
					e.TokenReader.Push(intoToken.Value);
				}
			}
			queryNode?.AddJoin(varToken.Value.Value, source, key1, key2, intoName);
		}

		private void BuildWhere(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			queryNode?.AddWhere(condition);
		}

		private void BuildSelect(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			var selector = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			queryNode?.AddSelect(selector);
		}

		/// <summary>
		/// from a in q1
		/// group a.Name by a.Age into g
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		/// <param name="createFullOptions"></param>
		/// <param name="queryNode"></param>
		private void BuildGroup(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			var element = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _ByTokens);
			analyzer.ValidateNextToken(e.TokenReader, "by");
			var key = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _JoinEndTokens);
			string intoName = null;
			var intoToken = e.TokenReader.Read();
			if (intoToken.HasValue)
			{
				if (intoToken.Value.IsSymbol("into"))
				{
					var intoNameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					intoName = intoNameToken.Value.Value;
				}
				else
				{
					e.TokenReader.Push(intoToken.Value);
				}
			}
			queryNode?.AddGroup(key, element, intoName);
		}

		/// <summary>
		/// from a in q
		/// orderby a.Age ascending
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		/// <param name="createFullOptions"></param>
		/// <param name="queryNode"></param>
		private void BuildOrderby(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, QueryNode queryNode)
		{
			var key = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _OrderbyEndTokens);
			var token = e.TokenReader.Read();
			string mode = null;
			if (token.HasValue)
			{
				if (token.Value.IsSymbol("ascending") || token.Value.IsSymbol("asc") || token.Value.IsSymbol("descending") || token.Value.IsSymbol("desc"))
				{
					mode = token.Value.Value;
				}
				else
				{
					e.TokenReader.Push(token.Value);
				}
			}
			queryNode?.AddOrderby(key, mode);
		}
	}
}
