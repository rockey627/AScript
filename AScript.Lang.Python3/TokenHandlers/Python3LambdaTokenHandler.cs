using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Python3.TokenHandlers
{
	/// <summary>
	/// lambda a,b:a+b
	/// </summary>
	public class Python3LambdaTokenHandler : ITokenHandler
	{
		public static readonly Python3LambdaTokenHandler Instance = new Python3LambdaTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			// lambda 参数列表以冒号结束，参数没有类型注解
			var argNames = e.Ignore ? null : new List<string>();
			var token = e.TokenReader.Read();
			while (true)
			{
				if (!token.HasValue)
				{
					throw new Exception($"invalid lambda at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
				if (token.Value.Type == ETokenType.String)
				{
					throw new Exception($"invalid lambda argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				if (token.Value.Value == ":") break;
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exception($"invalid lambda argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				string argName = token.Value.Value;
				if (!e.Ignore)
				{
					argNames.Add(argName);
				}
				// 逗号分割下一个参数名
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exception($"invalid lambda at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
				if (token.Value.Value == ",")
				{
					token = e.TokenReader.Read();
					continue;
				}
				if (token.Value.Value == ":") break;
				throw new Exception($"invalid lambda argument name '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
			}
			// 解析 lambda 表达式体
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			//
			if (!e.Ignore)
			{
				if (body is TreeBuilder bodyTreeBuilder)
				{
					body = bodyTreeBuilder.EvalRoot(e.BuildContext, e.ScriptContext, createFullOptions, e.Control);
					//PoolManage.Return(bodyTreeBuilder);
				}
				var args = new DefineVarNode[argNames.Count];
				for (int i = 0; i < argNames.Count; i++)
				{
					args[i] = new DefineVarNode { Name = argNames[i], SystemType = typeof(object), Type = null };
				}
				// lambda 函数名使用 "_" 表示匿名函数
				var defineNode = new DefineFuncNode { Name = "_", Args = args, Body = body, ReturnSystemType = typeof(object), ReturnType = null };
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineNode);
			}
		}
	}
}
