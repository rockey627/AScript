using AScript.Lang.Lua.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// <![CDATA[
	/// function name(args) body end
	/// function ClassName:name(args) body end
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
			var token = analyzer.ValidateNextToken(e.TokenReader);
			string className = null;
			ITreeNode classNode = null;
			string funcName;
			if (token.Value.IsSymbol("("))
			{
				funcName = null;
			}
			else
			{
				funcName = token.Value.Value;
				var lastName = funcName;

				// function table1.table2:name(args) body end
				var nextToken = analyzer.ValidateNextToken(e.TokenReader);
				if (nextToken.Value.IsSymbol("."))
				{
					// 支持多个.操作符: table1.table2.table3:name(args)
					// 必须有:号，className为完整路径如table1.table2
					classNode = PoolManage.CreateVariableNode(funcName);
					var classNameBuilder = funcName;
					while (nextToken.Value.IsSymbol("."))
					{
						lastName = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word).Value.Value;
						classNameBuilder = classNameBuilder + "." + lastName;
						var opNode = PoolManage.CreateOperatorNode(".", 2, 19);
						opNode.Left = classNode;
						opNode.Right = PoolManage.CreateVariableNode(lastName);
						classNode = opNode;
						nextToken = analyzer.ValidateNextToken(e.TokenReader);
					}
					// 必须有:号
					if (!nextToken.Value.IsSymbol(":"))
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid function syntax at ({e.CurrentToken.Line},{e.CurrentToken.Column}), expect ':' after '.'");
					}
					className = classNameBuilder;
					funcName = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word).Value.Value;
				}
				else if (nextToken.Value.IsSymbol(":"))
				{
					className = funcName;
					classNode = PoolManage.CreateVariableNode(className);
					funcName = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word).Value.Value;
				}
				else
				{
					e.TokenReader.Push(nextToken.Value);
				}

				// 参数列表
				analyzer.ValidateNextToken(e.TokenReader, "(");
			}
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
			var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens_end);

			analyzer.ValidateNextToken(e.TokenReader, "end");

			if (!e.Ignore)
			{
				// 当使用 function ClassName:name(args) 语法时，需要自动添加 self 参数
				DefineVarNode[] args;
				if (!string.IsNullOrEmpty(className))
				{
					args = new DefineVarNode[argNames.Count + 1];
					args[0] = new DefineVarNode { Name = "self", SystemType = typeof(LuaTable) };
					for (int i = 0; i < argNames.Count; i++)
					{
						args[i + 1] = new DefineVarNode { Name = argNames[i], SystemType = typeof(object) };
					}
				}
				else
				{
					args = new DefineVarNode[argNames.Count];
					for (int i = 0; i < argNames.Count; i++)
					{
						args[i] = new DefineVarNode { Name = argNames[i], SystemType = typeof(object) };
					}
				}
				var defineNode = new LuaDefineFuncNode { ClassNode = classNode, ClassName = className, Name = funcName, Args = args, Body = body };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineNode);
			}
		}
	}
}
