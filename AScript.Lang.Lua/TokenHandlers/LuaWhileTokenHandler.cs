using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// while condition do
	///     body
	/// end
	/// </summary>
	public class LuaWhileTokenHandler : ITokenHandler
	{
		public static readonly LuaWhileTokenHandler Instance = new LuaWhileTokenHandler();

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
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);
			analyzer.ValidateNextToken(e.TokenReader, "do");
			var body = LuaLang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);

			if (!e.Ignore)
			{
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, new WhileNode { Condition = condition, Body = body });
			}
		}
	}
}
