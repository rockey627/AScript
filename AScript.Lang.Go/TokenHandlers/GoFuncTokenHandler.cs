using AScript.Lang.Go.Nodes;
using AScript.Lang.Go.Types;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq;

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

			List<GoDefineVarNode> args;
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
				args = GoLang.ParseDefineVars(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Ignore);
				analyzer.ValidateNextToken(e.TokenReader, ")");
			}
			// 检查返回值类型（可选）
			string returnType = null;
			Type returnSystemType = null;
			token = analyzer.ValidateNextToken(e.TokenReader);
			if (token.Value.IsSymbol("("))
			{
				// 多返回值
				var returnTypes = new List<GoType>();
				while (true)
				{
					var type = GoLang.ParseType(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Ignore);
					returnTypes.Add(type);
					token = analyzer.ValidateNextToken(e.TokenReader);
					if (token.Value.IsSymbol(",")) continue;
					if (token.Value.IsSymbol(")")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' near func {funcName} at ({token.Value.Line},{token.Value.Column})");
				}
				returnType = $"({string.Join(",", returnTypes.Select(a => a.Name))})";
				returnSystemType = ScriptUtils.GetTupleType(returnTypes.Select(a => a.RealType).ToArray());
			}
			else
			{
				e.TokenReader.Push(token.Value);
				if (GoLang.TryParseType(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Ignore, out var goType))
				{
					returnType = goType.Name;
					returnSystemType = goType.RealType;
				}
				else if (token.Value.Type == ETokenType.Word)
				{
					throw new Exceptions.ScriptRuntimeException($"unknown return type {token.Value.Value} of func {funcName} at ({token.Value.Line},{token.Value.Column})");
				}
				else
				{
					returnSystemType = typeof(void);
				}
			}

			// 解析函数体
			analyzer.ValidateNextToken(e.TokenReader, "{");
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, "}");

			// 匿名方法判断是否立即调用
			if (string.IsNullOrEmpty(funcName))
			{
				token = e.TokenReader.Read();
				if (token.HasValue)
				{
					if (token.Value.IsSymbol("("))
					{
						// 解析参数列表
						var callArgs = analyzer.BuildFuncParams(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
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
							var callNode = new CallFuncNode
							{
								Method = defineNode,
								Args = callArgs?.ToArray()
							};
							e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, callNode);
						}
						return;
					}
					e.TokenReader.Push(token.Value);
				}
			}

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
