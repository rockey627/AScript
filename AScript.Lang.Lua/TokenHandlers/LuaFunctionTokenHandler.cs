using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// <![CDATA[
	/// function name(args) body end
	/// function name.modname(args) body end  (方法调用)
	/// ]]>
	/// </summary>
	public class LuaFunctionTokenHandler : ITokenHandler
	{
		public static readonly LuaFunctionTokenHandler Instance = new LuaFunctionTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// 函数名
			var token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid function at ({e.CurrentToken.Line},{e.CurrentToken.Column}), expect function name");
			}
			if (token.Value.Type != ETokenType.Word)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid function name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column}), expect function name");
			}
			string funcName = token.Value.Value;

			// 检查是否是方法调用 (name:name())
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue && nextToken.Value.Value == ":")
			{
				// 方法调用，函数名变为 name:name
				funcName = funcName + ":";
				token = e.TokenReader.Read();
				if (!token.HasValue || token.Value.Type != ETokenType.Word)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid method name at ({token.Value.Line},{token.Value.Column})");
				}
				funcName = funcName + token.Value.Value;
			}
			else if (nextToken.HasValue)
			{
				e.TokenReader.Push(nextToken.Value);
			}

			// 参数列表
			analyzer.ValidateNextToken(e.TokenReader, "(");
			var argNames = e.Ignore ? null : new List<string>();
			token = e.TokenReader.Read();
			while (true)
			{
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid function {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
				if (token.Value.Type == ETokenType.String)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				if (token.Value.Value == ")") break;
				if (token.Value.Value == ",")
				{
					token = e.TokenReader.Read();
					continue;
				}
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				if (!e.Ignore)
				{
					argNames.Add(token.Value.Value);
				}
				token = e.TokenReader.Read();
				if (!token.HasValue) break;
				if (token.Value.Value == ",") continue;
				if (token.Value.Value == ")") break;
			}

			// 函数体
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = LuaLang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens);

			analyzer.ValidateNextToken(e.TokenReader, "end");

			if (!e.Ignore)
			{
				var args = new DefineVarNode[argNames.Count];
				for (int i = 0; i < argNames.Count; i++)
				{
					args[i] = new DefineVarNode { Name = argNames[i], SystemType = typeof(object) };
				}
				var defineNode = new DefineFuncNode { Name = funcName, Args = args, Body = body, ReturnSystemType = typeof(object) };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineNode);
			}
		}
	}
}
