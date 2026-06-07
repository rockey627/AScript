using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// 语法1：CASE field WHEN value1 THEN .. WHEN value2 THEN .. ELSE .. END
	/// 语法2：CASE WHEN condition1 THEN .. WHEN condition2 THEN .. ELSE .. END
	/// </summary>
	public class SqlCaseTokenHandler : ITokenHandler
	{
		public static readonly SqlCaseTokenHandler Instance = new SqlCaseTokenHandler();

		private static readonly HashSet<string> _CaseEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "when" };
		private static readonly HashSet<string> _WhenEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "then" };
		private static readonly HashSet<string> _ThenEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "when", "else", "end" };
		private static readonly HashSet<string> _ElseEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "end" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var switchValue = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _CaseEndTokens);
			bool isSwitchValueEmpty = switchValue == null || (switchValue is TreeBuilder switchTreeBuilder) && switchTreeBuilder.IsEmpty();
			var whenList = e.Ignore || !isSwitchValueEmpty ? null : new List<Tuple<ITreeNode, ITreeNode>>();
			var caseList = e.Ignore || isSwitchValueEmpty ? null : new List<Tuple<IList<ITreeNode>, ITreeNode>>();
			ITreeNode defaultBody = null;
			bool hasWhen = false;
			bool hasElse = false;
			while (true)
			{
				var token = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				if (token.Value.Value.Equals("when", StringComparison.OrdinalIgnoreCase))
				{
					if (hasElse) throw new Exceptions.ScriptAnalyzingException($"invalid expression {token.Value.Value} at ({token.Value.Line},{token.Value.Column})");
					hasWhen = true;
					var test = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _WhenEndTokens);
					analyzer.ValidateNextToken(e.TokenReader, "then", StringComparison.OrdinalIgnoreCase);
					var body = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _ThenEndTokens);
					if (whenList != null) whenList.Add(Tuple.Create(test, body));
					else if (caseList != null) caseList.Add(Tuple.Create<IList<ITreeNode>, ITreeNode>(new[] { test }, body));
				}
				else if (token.Value.Value.Equals("else", StringComparison.OrdinalIgnoreCase))
				{
					if (!hasWhen) throw new Exceptions.ScriptAnalyzingException($"invalid expression {token.Value.Value} at ({token.Value.Line},{token.Value.Column})");
					hasElse = true;
					defaultBody = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _ElseEndTokens);
				}
				else if (token.Value.Value.Equals("end", StringComparison.OrdinalIgnoreCase))
				{
					break;
				}
				else
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression {token.Value.Value} at ({token.Value.Line},{token.Value.Column})");
				}
			}

			if (whenList != null)
			{
				// if语句
				IfNode root = null;
				IfNode ifNode = null;
				foreach (var item in whenList)
				{
					if (ifNode == null)
					{
						ifNode = new IfNode { Condition = item.Item1, Body = item.Item2, ReturnValue = true };
						root = ifNode;
					}
					else
					{
						var elseNode = new IfNode { Condition = item.Item1, Body = item.Item2, ReturnValue = true };
						ifNode.Else = elseNode;
						ifNode = elseNode;
					}
				}
				ifNode.Else = defaultBody;
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, root);
			}
			else if (caseList != null)
			{
				// switch语句
				var switchNode = new SwitchNode { SwitchValue = switchValue, DefaultBody = defaultBody, Cases = caseList };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, switchNode);
			}
		}
	}
}
