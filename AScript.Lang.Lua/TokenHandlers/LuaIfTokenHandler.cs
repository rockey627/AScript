using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// <![CDATA[
	/// if condition then
	///     body
	/// elseif condition then
	///     body
	/// else
	///     body
	/// end
	/// ]]>
	/// </summary>
	public class LuaIfTokenHandler : ITokenHandler
	{
		public static readonly LuaIfTokenHandler Instance = new LuaIfTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);
			analyzer.ValidateNextToken(e.TokenReader, "then");
			var body = analyzer.BuildMultiStatement( e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);

			ITreeNode elseNode = null;
			var token = analyzer.ValidateNextToken(e.TokenReader);
			if (token.Value.IsSymbol("elseif"))
			{
				e.TokenReader.Push(new Token("if", ETokenType.Word, token.Value.Line, token.Value.Column));
				elseNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);
			}
			else if (token.Value.IsSymbol("else"))
			{
				elseNode = analyzer.BuildMultiStatement( e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);
			}
			else
			{
				e.TokenReader.Push(token.Value);
			}
			analyzer.ValidateNextToken(e.TokenReader, "end");

			if (!e.Ignore)
			{
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, new IfNode { Condition = condition, Body = body, Else = elseNode });
			}
		}
	}
}
