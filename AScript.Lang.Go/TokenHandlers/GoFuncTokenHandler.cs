using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go函数声明处理器
	/// func name(params) returnType { body }
	/// func name(params) { body }
	/// func(params) { body }
	/// </summary>
	public class GoFuncTokenHandler : ITokenHandler
	{
		public static readonly GoFuncTokenHandler Instance = new GoFuncTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var token = analyzer.ValidateNextToken(e.TokenReader);

			// 函数名（可选，匿名函数）
			string funcName;
			// 检查是否是匿名函数 func()
			if (token.Value.IsSymbol("("))
			{
				// 匿名函数
				funcName = null;
			}
			else if (token.Value.Type == ETokenType.Word)
			{
				funcName = token.Value.Value;
				analyzer.ValidateNextToken(e.TokenReader, "(");
			}
			else
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid function token '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
			}

			List<DefineVarNode> args;
			token = analyzer.ValidateNextToken(e.TokenReader);
			if (token.Value.IsSymbol(")"))
			{
				// 无参数
				args = null;
			}
			else
			{
				// 解析参数列表
				e.TokenReader.Push(token.Value);
				args = GoLang.ParseDefineVars(analyzer, e.ScriptContext, e.TokenReader, e.Ignore);
				analyzer.ValidateNextToken(e.TokenReader, ")");
			}
			// 检查返回值类型（可选）
			string returnType = null;
			Type returnSystemType = null;
			token = analyzer.ValidateNextToken(e.TokenReader);
			if (token.Value.Type == ETokenType.Word)
			{
				returnType = token.Value.Value;
				returnSystemType = e.ScriptContext.EvalType(returnType);
				if (returnSystemType == null)
				{
					throw new Exceptions.ScriptRuntimeException($"unknown return type {returnType} of func {funcName} at ({token.Value.Line},{token.Value.Column})");
				}
			}
			else if (token.Value.IsSymbol("("))
			{
				// 多返回值
				var returnTypes = e.Ignore ? null : new List<Type>();
				while (true)
				{
					var typeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					var type = e.ScriptContext.EvalType(typeToken.Value.Value);
					if (type == null)
					{
						throw new Exceptions.ScriptRuntimeException($"unknown return type {returnType} of func {funcName} at ({token.Value.Line},{token.Value.Column})");
					}
					returnTypes?.Add(type);
					token = analyzer.ValidateNextToken(e.TokenReader);
					if (token.Value.IsSymbol(",")) continue;
					if (token.Value.IsSymbol(")")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' near func {funcName} at ({token.Value.Line},{token.Value.Column})");
				}
				returnType = null;
				returnSystemType = ScriptUtils.GetTupleType(returnTypes.ToArray());
			}
			else
			{
				e.TokenReader.Push(token.Value);
			}

			// 解析函数体
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

			if (!e.Ignore)
			{
				var defineNode = new DefineFuncNode
				{
					Name = funcName,
					Args = args?.ToArray(),
					Body = body,
					ReturnSystemType = returnSystemType,
					ReturnType = returnType
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineNode);
			}
		}
	}
}
