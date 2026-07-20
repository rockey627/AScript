using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

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
			var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);

			ITreeNode elseNode = null;
			IfNode currentElseIfNode = null;
			var token = analyzer.ValidateNextToken(e.TokenReader);
			while (token.Value.IsSymbol("elseif"))
			{
				var elseifCondition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);
				analyzer.ValidateNextToken(e.TokenReader, "then");
				var elseifBody = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);
				if (!e.Ignore)
				{
					var elseifNode = new IfNode { Condition = elseifCondition, Body = elseifBody };
					if (currentElseIfNode != null)
					{
						currentElseIfNode.Else = elseifNode;
					}
					currentElseIfNode = elseifNode;
					if (elseNode == null) elseNode = elseifNode;
				}
				token = analyzer.ValidateNextToken(e.TokenReader);
			}
			if (token.Value.IsSymbol("else"))
			{
				var elseNode1 = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);
				if (elseNode == null) elseNode = elseNode1;
				else currentElseIfNode.Else = elseNode1;
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
