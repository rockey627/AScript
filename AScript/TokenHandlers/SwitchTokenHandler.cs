using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// switch(value) {
	/// case v1:
	/// case v2:
	/// default:
	/// }
	/// </summary>
	public class SwitchTokenHandler : ITokenHandler
	{
		public static readonly SwitchTokenHandler Instance = new SwitchTokenHandler();

		private static readonly HashSet<string> _BodyEndTokens = new HashSet<string> { "case", "default" };
		private static readonly HashSet<string> _TestEndTokens = new HashSet<string> { ":" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			analyzer.ValidateNextToken(e.TokenReader, "(");
			var switchValue = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			analyzer.ValidateNextToken(e.TokenReader, "{");
			ITreeNode defaultBody = null;
			List<Tuple<ITreeNode, ITreeNode>> cases = null;
			while (true)
			{
				var token = e.TokenReader.Read();
				if (token.Value.IsSymbol("}")) break;
				if (token.Value.IsSymbol("case"))
				{
					var test = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _TestEndTokens);
					analyzer.ValidateNextToken(e.TokenReader, ":");
					var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _BodyEndTokens);
					if (!e.Ignore)
					{
						if (cases == null) cases = new List<Tuple<ITreeNode, ITreeNode>>();
						cases.Add(Tuple.Create(test, body));
					}
				}
				else if (token.Value.IsSymbol("default"))
				{
					analyzer.ValidateNextToken(e.TokenReader, ":");
					defaultBody = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				}
				else
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
			}
			if (!e.Ignore)
			{
				var switchNode = new SwitchNode { SwitchValue = switchValue, DefaultBody = defaultBody, Cases = cases };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, switchNode);
			}
		}
	}
}
