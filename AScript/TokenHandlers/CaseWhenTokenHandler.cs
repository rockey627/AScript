using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// case(value) {
	/// when v1:
	/// when v2:
	/// default:
	/// }
	/// </summary>
	public class CaseWhenTokenHandler : ITokenHandler
	{
		public static readonly CaseWhenTokenHandler Instance = new CaseWhenTokenHandler();

		private static readonly HashSet<string> _BodyEndTokens = new HashSet<string> { "when", "default" };
		private static readonly HashSet<string> _TestEndTokens = new HashSet<string> { ":" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			analyzer.ValidateNextToken(e.TokenReader, "(");
			var caseValue = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			analyzer.ValidateNextToken(e.TokenReader, "{");
			ITreeNode defaultBody = null;
			List<Tuple<IList<ITreeNode>, ITreeNode>> whens = null;
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			IList<ITreeNode> currentTests = null;
			bool hasCase = false;
			while (true)
			{
				var token = e.TokenReader.Read();
				if (token.Value.IsSymbol("}")) break;
				if (token.Value.IsSymbol("when"))
				{
					hasCase = true;
					var test = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _TestEndTokens);
					analyzer.ValidateNextToken(e.TokenReader, ":");
					var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _BodyEndTokens);
					analyzer.TrySkipNextToken(e.TokenReader, ";");
					if (!e.Ignore)
					{
						if (whens == null) whens = new List<Tuple<IList<ITreeNode>, ITreeNode>>();
						//cases.Add(Tuple.Create(test, body));
						if (currentTests == null) currentTests = new List<ITreeNode>();
						currentTests.Add(test);
						if (body != null && (!(body is TreeBuilder treeBuilder) || treeBuilder.Root != null))
						{
							whens.Add(Tuple.Create(currentTests, body));
							currentTests = null;
						}
					}
				}
				else if (token.Value.IsSymbol("default"))
				{
					analyzer.ValidateNextToken(e.TokenReader, ":");
					defaultBody = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				}
				else
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
			}
			if (!hasCase)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression case at ({e.CurrentToken.Line},{e.CurrentToken.Column}), expect when");
			}
			if (!e.Ignore)
			{
				var switchNode = new CaseWhenNode { CaseValue = caseValue, DefaultBody = defaultBody, Whens = whens };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, switchNode);
			}
		}
	}
}
