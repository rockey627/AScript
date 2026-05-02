using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class WhileTokenHandler : ITokenHandler
	{
		public static readonly WhileTokenHandler Instance = new WhileTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			
			var createTreeNodeOnlyOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			if (e.Ignore)
			{
				analyzer.ValidateNextToken(e.TokenReader, "(");
				analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, null, ignore: true);
				analyzer.ValidateNextToken(e.TokenReader, ")");
				analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, null, ignore: true);
				return;
			}

			analyzer.ValidateNextToken(e.TokenReader, "(");
			var conditionBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, null, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			var bodyBuilder = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, null, e.Ignore, noblock: true);
			var whileNode = new WhileNode { Condition = conditionBuilder, Body = bodyBuilder };
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, whileNode);
		}
	}
}
