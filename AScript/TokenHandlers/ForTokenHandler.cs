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
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			analyzer.ValidateNextToken(e.TokenReader, "(");
			// 执行初始化语句
			var initBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.TrySkipNextToken(e.TokenReader, ";");
			// 获取条件语句
			var conditionBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.TrySkipNextToken(e.TokenReader, ";");
			// 获取后置语句
			var postBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			// 获取循环体语句
			var bodyBuilder = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
			if (!e.Ignore)
			{
				var forNode = new ForNode { Init = initBuilder, Condition = conditionBuilder, Body = bodyBuilder, Post = postBuilder };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forNode);
			}
		}
	}
}
