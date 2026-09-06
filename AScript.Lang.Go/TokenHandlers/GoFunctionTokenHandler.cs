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
	public class GoFunctionTokenHandler : ITokenHandler
	{
		public static readonly GoFunctionTokenHandler Instance = new GoFunctionTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// 函数名（可选，匿名函数）
			string funcName = null;
			var token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid function at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}

			// 检查是否是匿名函数 func()
			if (token.Value.IsSymbol("("))
			{
				// 匿名函数
				funcName = null;
			}
			else if (token.Value.Type == ETokenType.Word)
			{
				funcName = token.Value.Value;
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid function {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}

				// 如果是方法 receiver
				if (token.Value.IsSymbol("("))
				{
					// 方法声明：func (receiver) name(params) returnType { body }
					token = e.TokenReader.Read();
					// receiver参数
					while (!token.HasValue || !token.Value.IsSymbol(")"))
					{
						if (!token.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid receiver for function {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
						}
						token = e.TokenReader.Read();
					}
					token = e.TokenReader.Read();
					if (!token.HasValue || token.Value.Type != ETokenType.Word)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid method name at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
					}
					funcName = token.Value.Value;
					token = e.TokenReader.Read();
				}

				// 期望左括号
				if (!token.HasValue || !token.Value.IsSymbol("("))
				{
					throw new Exceptions.ScriptAnalyzingException($"expect '(' after function name {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
			}
			else
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid function token '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
			}

			// 解析参数列表
			var argNames = e.Ignore ? null : new List<string>();
			var argTypes = e.Ignore ? null : new List<string>();
			token = e.TokenReader.Read();
			while (true)
			{
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid function {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
				if (token.Value.IsSymbol(")")) break;

				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid parameter name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}

				string argName = token.Value.Value;
				string argType = null;

				// 检查参数名后是否有类型
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid function {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}

				// 如果遇到 , 说明没有类型注解
				if (token.Value.IsSymbol(","))
				{
					if (!e.Ignore)
					{
						argNames.Add(argName);
						argTypes.Add(null); // 无类型注解默认为any
					}
					token = e.TokenReader.Read();
					continue;
				}

				// 如果遇到 ) 说明参数列表结束
				if (token.Value.IsSymbol(")"))
				{
					if (!e.Ignore)
					{
						argNames.Add(argName);
						argTypes.Add(null);
					}
					break;
				}

				// 解析类型
				argType = token.Value.Value;
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid function {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}

				if (!e.Ignore)
				{
					argNames.Add(argName);
					argTypes.Add(argType);
				}

				if (token.Value.IsSymbol(","))
				{
					token = e.TokenReader.Read();
					continue;
				}
				if (token.Value.IsSymbol(")"))
				{
					break;
				}
			}

			// 检查返回值类型（可选）
			string returnType = null;
			token = e.TokenReader.Read();
			if (token.HasValue && token.Value.Type == ETokenType.Word && !IsBlockStart(token.Value.Value))
			{
				returnType = token.Value.Value;
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
				var args = new DefineVarNode[argNames.Count];
				for (int i = 0; i < argNames.Count; i++)
				{
					Type systemType = typeof(object);
					if (!string.IsNullOrEmpty(argTypes[i]))
					{
						systemType = e.ScriptContext.EvalType(argTypes[i]) ?? typeof(object);
					}
					args[i] = new DefineVarNode { Name = argNames[i], SystemType = systemType, Type = argTypes[i] };
				}

				Type returnSystemType = typeof(object);
				if (!string.IsNullOrEmpty(returnType))
				{
					returnSystemType = e.ScriptContext.EvalType(returnType) ?? typeof(object);
				}

				var defineNode = new DefineFuncNode
				{
					Name = funcName,
					Args = args,
					Body = body,
					ReturnSystemType = returnSystemType,
					ReturnType = returnType
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineNode);
			}
		}

		private bool IsBlockStart(string value)
		{
			return value == "{" || value == "do" || value == "then" || value == "begin";
		}
	}
}
