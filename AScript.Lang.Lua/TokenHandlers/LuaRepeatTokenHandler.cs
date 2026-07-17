using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// repeat
	///     body
	/// until condition
	/// </summary>
	public class LuaRepeatTokenHandler : ITokenHandler
	{
		public static readonly LuaRepeatTokenHandler Instance = new LuaRepeatTokenHandler();

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
			var body = LuaLang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);

			// until condition
			analyzer.ValidateNextToken(e.TokenReader, "until");
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);

			if (!e.Ignore)
			{
				// repeat...until 等价于 do...while(false) + break check
				var whileNode = new WhileNode { Condition = condition, Body = body };
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, whileNode);
			}
		}
	}
}
