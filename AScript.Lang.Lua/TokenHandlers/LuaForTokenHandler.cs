using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// <![CDATA[
	/// 数值for：for i = start, end, step do body end
	/// ]]>
	/// </summary>
	public class LuaForTokenHandler : ITokenHandler
	{
		public static readonly LuaForTokenHandler Instance = new LuaForTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue || nextToken.Value.Type != ETokenType.Word)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid for expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			string varName = nextToken.Value.Value;

			nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue || nextToken.Value.Value != "=")
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid for expression at ({e.CurrentToken.Line},{e.CurrentToken.Column}), expect '='");
			}

			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var startExpr = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);
			analyzer.ValidateNextToken(e.TokenReader, ",");
			var endExpr = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);
			ITreeNode stepExpr = null;
			var token2 = e.TokenReader.Read();
			if (token2.HasValue && token2.Value.Value == ",")
			{
				stepExpr = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);
			}
			else
			{
				e.TokenReader.Push(token2.Value);
			}
			analyzer.ValidateNextToken(e.TokenReader, "do");
			var body = LuaLang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);

			if (!e.Ignore)
			{
				// 创建变量定义
				var defNode = PoolManage.CreateDefineVarNode(varName, null, typeof(object));
				var forNode = new ForNode
				{
					Init = startExpr,
					Condition = endExpr,
					Post = stepExpr,
					Body = body
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forNode);
			}
		}
	}
}
