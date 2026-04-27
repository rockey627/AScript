using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Lang.Python3.TokenHandlers
{
	/// <summary>
	/// <para>定义函数：</para>
	/// <para>def 函数名(参数1[:类型],参数2[:类型],...) [->类型]:</para>
	/// <para>    函数体</para>
	/// </summary>
	public class Python3DefTokenHandler : ITokenHandler
	{
		public static readonly Python3DefTokenHandler Instance = new Python3DefTokenHandler();

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
				throw new Exception($"invalid def at ({e.CurrentToken.Line},{e.CurrentToken.Column}), expect function name");
			}
			if (token.Value.Type != ETokenType.Word)
			{
				throw new Exception($"invalid def name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column}), expect function name");
			}
			string funcName = token.Value.Value;
			// 参数
			analyzer.ValidateNextToken(e.TokenReader, "(");
			var argNames = e.Ignore ? null : new List<string>();
			var argTypes = e.Ignore ? null : new List<string>();
			token = e.TokenReader.Read();
			while (true)
			{
				if (!token.HasValue)
				{
					throw new Exception($"invalid def {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
				if (token.Value.Type == ETokenType.String)
				{
					throw new Exception($"invalid argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				if (token.Value.Value == ")") break;
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exception($"invalid argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				string argName = token.Value.Value;
				string argType = null;
				// 检查参数类型注解
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exception($"invalid def {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
				if (token.Value.Value == ":")
				{
					// 解析类型
					var typeToken = e.TokenReader.Read();
					if (!typeToken.HasValue || typeToken.Value.Type != ETokenType.Word)
					{
						throw new Exception($"invalid parameter type at ({token.Value.Line},{token.Value.Column})");
					}
					argType = typeToken.Value.Value;
					token = e.TokenReader.Read();
					if (!token.HasValue)
					{
						throw new Exception($"invalid def {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
					}
				}
				if (!e.Ignore)
				{
					argNames.Add(argName);
					argTypes.Add(argType);
				}
				// 逗号分割下一个参数名
				if (token.Value.Type == ETokenType.String)
				{
					throw new Exception($"invalid argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				if (token.Value.Value == ",")
				{
					token = e.TokenReader.Read();
					continue;
				}
				if (token.Value.Value == ")") break;
				throw new Exception($"invalid argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
			}
			// 返回类型注解 -> Type
			string returnTypeName = null;
			token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exception($"invalid def {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			if (token.Value.Value == "->")
			{
				// 解析返回类型
				var returnTypeToken = e.TokenReader.Read();
				if (!returnTypeToken.HasValue || returnTypeToken.Value.Type != ETokenType.Word)
				{
					throw new Exception($"invalid return type at ({token.Value.Line},{token.Value.Column})");
				}
				returnTypeName = returnTypeToken.Value.Value;
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exception($"invalid def {funcName} at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}), expect ':'");
				}
			}
			if (token.Value.Type == ETokenType.String || token.Value.Value != ":")
			{
				throw new Exception($"invalid def {funcName} at ({token.Value.Line},{token.Value.Column}), expect ':'");
			}
			// 函数体
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = Python3Lang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, Python3Lang.EndTokens);
			//
			if (!e.Ignore)
			{
				var args = new DefineVarNode[argNames.Count];
				for (int i = 0; i < argNames.Count; i++)
				{
					string typeName = argTypes[i];
					Type systemType = null;
					if (!string.IsNullOrEmpty(typeName))
					{
						systemType = e.ScriptContext.EvalType(typeName);
						if (systemType == null)
						{
							throw new Exception($"unknown parameter type '{typeName}' in function {funcName}");
						}
					}
					else
					{
						systemType = typeof(object);
					}
					args[i] = new DefineVarNode { Name = argNames[i], SystemType = systemType, Type = typeName };
				}
				Type returnSystemType = null;
				if (!string.IsNullOrEmpty(returnTypeName))
				{
					returnSystemType = e.ScriptContext.EvalType(returnTypeName);
					if (returnSystemType == null)
					{
						throw new Exception($"unknown return type '{returnTypeName}' in function {funcName}");
					}
				}
				else
				{
					returnSystemType = typeof(object);
				}
				var defineNode = new DefineFuncNode { Name = funcName, Args = args, Body = body, ReturnSystemType = returnSystemType, ReturnType = returnTypeName };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineNode);
			}
		}
	}
}
