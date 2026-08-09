using AScript.Lang.Lua.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// <![CDATA[
	/// 数值for：for i = start, end, step do body end
	/// 泛型for：for i,v in ipairs(table) do body end
	///          for v in ipairs(table) do body end
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

			var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			string varName = varToken.Value.Value;

			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.IsSymbol("="))
			{
				// 数值for循环：for i=start,end[,step] do body end
				BuildNumberFor(analyzer, e, varName);
				return;
			}
			if (nextToken.Value.IsSymbol("in"))
			{
				// 泛型for循环：for v in ipairs(table) do body end
				e.TokenReader.Push(nextToken.Value);
				e.TokenReader.Push(varToken.Value);
				BuildGenericFor(analyzer, e, null);
				return;
			}
			if (nextToken.Value.IsSymbol(","))
			{
				// 泛型for循环：for i,v in ipairs(table) do body end
				BuildGenericFor(analyzer, e, varName);
				return;
			}

			throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
		}

		private void BuildNumberFor(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, string varName)
		{
			var start = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ",");
			var end = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens_do);
			ITreeNode step = null;
			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.IsSymbol(","))
			{
				step = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens_do);
			}
			else
			{
				e.TokenReader.Push(nextToken.Value);
			}
			analyzer.ValidateNextToken(e.TokenReader, "do");
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens_end);
			analyzer.ValidateNextToken(e.TokenReader, "end");

			if (!e.Ignore)
			{
				var forNode = new LuaForNumberNode { VarNode = new VariableNode(varName), StartNode = start, EndNode = end, StepNode = step, Body = body };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forNode);
			}
		}

		//private void BuildGeneric1For(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, string vVarName)
		//{
		//	// 解析迭代器表达式
		//	var iterator = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens_do);
		//	analyzer.ValidateNextToken(e.TokenReader, "do");

		//	// body
		//	var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
		//	var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens_end);
		//	analyzer.ValidateNextToken(e.TokenReader, "end");

		//	if (!e.Ignore)
		//	{
		//		var foreachNode = new ForeachNode();
		//		foreachNode.VarDefine = new DefineVarNode(vVarName, null, typeof(object));
		//		foreachNode.Collection = iterator;
		//		foreachNode.Body = body;
		//		e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
		//	}
		//}

		private void BuildGenericFor(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, string iVarName)
		{
			// 解析变量列表：for i,v in ...  
			var nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			var vVarName = nextToken.Value.Value;
			analyzer.ValidateNextToken(e.TokenReader, "in");

			// 解析迭代器表达式
			var iterator = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens_do);
			analyzer.ValidateNextToken(e.TokenReader, "do");

			// body
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens_end);
			analyzer.ValidateNextToken(e.TokenReader, "end");

			if (!e.Ignore)
			{
				var foreachNode = new ForeachNode();
				foreachNode.VarDefines = new List<DefineVarNode>
				{
					string.IsNullOrEmpty(iVarName) ? null : new DefineVarNode(iVarName, null, typeof(long)),
					new DefineVarNode(vVarName, null, typeof(object))
				};
				foreachNode.Collection = iterator;
				foreachNode.Body = body;
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
			}
		}
	}
}
