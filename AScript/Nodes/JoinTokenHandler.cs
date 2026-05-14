using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Nodes
{
	/// <summary>
	/// join b in query on a.Id equals b.Id into bb
	/// </summary>
	public class JoinTokenHandler : ITokenHandler
	{
		public static readonly JoinTokenHandler Instance = new JoinTokenHandler();

		private static readonly HashSet<string> _OnTokens = new HashSet<string> { "on" };
		private static readonly HashSet<string> _EqualsTokens = new HashSet<string> { "equals" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				if (!(e.TreeBuilder.Current is QueryNode))
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
				}
			}

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

			var buildOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var key1 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, _EqualsTokens);
			analyzer.ValidateNextToken(e.TokenReader, "equals");
			var key2 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, QueryNode.Keywords);

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

			if (!e.Ignore)
			{
				if (string.IsNullOrEmpty(intoName))
				{
					(e.TreeBuilder.Current as QueryNode).AddJoin(varToken.Value.Value, source, key1, key2);
				}
				else
				{
					(e.TreeBuilder.Current as QueryNode).AddGroupJoin(varToken.Value.Value, source, key1, key2, intoName);
				}
			}
		}
	}
}
