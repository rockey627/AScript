using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class ForTokenHandler : ITokenHandler
	{
		public static readonly ForTokenHandler Instance = new ForTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			// 
			var createTreeNodeOnlyOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			if (e.Ignore)
			{
				analyzer.ValidateNextToken(e.TokenReader, "(", e.CurrentToken.Line, e.CurrentToken.Column);
				// init
				analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, null, ignore: true);
				analyzer.TrySkipNextToken(e.TokenReader, ";");
				// condition
				analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, null, ignore: true);
				analyzer.TrySkipNextToken(e.TokenReader, ";");
				// post
				analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, null, ignore: true);
				analyzer.ValidateNextToken(e.TokenReader, ")");
				// body
				analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, null, ignore: true);
				return;
			}

			analyzer.ValidateNextToken(e.TokenReader, "(", e.CurrentToken.Line, e.CurrentToken.Column);
			// 执行初始化语句
			var initBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.TrySkipNextToken(e.TokenReader, ";");
			// 获取条件语句
			var conditionBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.TrySkipNextToken(e.TokenReader, ";");
			// 获取后置语句
			var postBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			// 获取循环体语句
			var bodyBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
			var forNode = new ForNode { Init = initBuilder, Condition = conditionBuilder, Body = bodyBuilder, Post = postBuilder };
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forNode);
		}
	}
}
