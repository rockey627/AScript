using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Python3.TokenHandlers
{
	/// <summary>
	/// while condition :
	///     body
	/// </summary>
	public class Python3WhileTokenHandler : ITokenHandler
	{
		public static readonly Python3WhileTokenHandler Instance = new Python3WhileTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: Python3Lang.EndTokens);
			analyzer.ValidateNextToken(e.TokenReader, ":");
			var body = Python3Lang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, Python3Lang.EndTokens);
			//
			if (!e.Ignore)
			{
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, new WhileNode { Condition = condition, Body = body });
			}
		}
	}
}
