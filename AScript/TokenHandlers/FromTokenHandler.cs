using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// from a in query1
	/// </summary>
	public class FromTokenHandler : ITokenHandler
	{
		public static readonly FromTokenHandler Instance = new FromTokenHandler();

		private static readonly HashSet<string> _OnTokens = new HashSet<string> { "on" };
		private static readonly HashSet<string> _EqualsTokens = new HashSet<string> { "equals" };
		private static readonly HashSet<string> _Keywords = new HashSet<string> { "from", "where", "join", "into", "select", "orderby", "group" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			//if (e.TreeBuilder.IsFullStatement())
			//{
			//	e.End = true;
			//	e.TokenReader.Push(e.CurrentToken);
			//	return;
			//}
			var queryNode = e.Ignore ? null : new QueryNode();
			BuildFrom(analyzer, e, queryNode);
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
					BuildFrom(analyzer, e, queryNode);
				}
				else if (token.Value.IsSymbol("join"))
				{
					BuildJoin(analyzer, e, queryNode);
				}
				else if (token.Value.IsSymbol("where"))
				{
					BuildWhere(analyzer, e, queryNode);
				}
				else if (token.Value.IsSymbol("select"))
				{
					BuildSelect(analyzer, e, queryNode);
				}
			}
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, queryNode);
			}
		}

		private void BuildFrom(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, QueryNode queryNode)
		{
			var varToken = e.TokenReader.Read();
			if (!varToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at {e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}");
			}
			if (varToken.Value.Type != ETokenType.Word)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' {varToken.Value.Value} at {varToken.Value.Line},{varToken.Value.Column}");
			}
			analyzer.ValidateNextToken(e.TokenReader, "in");
			var buildOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var source = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			queryNode?.AddFrom(varToken.Value.Value, source);
		}

		private void BuildJoin(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, QueryNode queryNode)
		{
			var varToken = e.TokenReader.Read();
			if (!varToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at {e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}");
			}
			if (varToken.Value.Type != ETokenType.Word)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' {varToken.Value.Value} at {varToken.Value.Line},{varToken.Value.Column}");
			}
			analyzer.ValidateNextToken(e.TokenReader, "in");

			var source = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _OnTokens);

			analyzer.ValidateNextToken(e.TokenReader, "on");

			var buildOptions = e.Options.CreateFullTreeNode ?? false ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var key1 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, _EqualsTokens);
			analyzer.ValidateNextToken(e.TokenReader, "equals");
			var key2 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);

			string intoName = null;
			var intoToken = e.TokenReader.Read();
			if (intoToken.HasValue)
			{
				if (intoToken.Value.IsSymbol("into"))
				{
					var intoNameToken = e.TokenReader.Read();
					if (!intoNameToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at {e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}");
					}
					if (intoNameToken.Value.Type != ETokenType.Word)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' {intoNameToken.Value.Value} at {intoNameToken.Value.Line},{intoNameToken.Value.Column}");
					}
					intoName = intoNameToken.Value.Value;
				}
				else
				{
					e.TokenReader.Push(intoToken.Value);
				}
			}
			queryNode?.AddJoin(varToken.Value.Value, source, key1, key2, intoName);
		}

		private void BuildWhere(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, QueryNode queryNode)
		{
			var buildOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			queryNode?.AddWhere(condition);
		}

		private void BuildSelect(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, QueryNode queryNode)
		{
			var buildOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var selector = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, _Keywords);
			queryNode?.AddSelect(selector);
		}
	}
}
