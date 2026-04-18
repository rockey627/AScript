using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Lang.Python3.TokenHandlers
{
	/// <summary>
	/// <para>定义函数：</para>
	/// <para>def 函数名（参数1,参数2,...）:</para>
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
				if (!e.Ignore)
				{
					argNames.Add(token.Value.Value);
				}
				// 逗号分割下一个参数名
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exception($"invalid def {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
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
			// :
			analyzer.ValidateNextToken(e.TokenReader, ":", token.Value.Line, token.Value.Column);
			// 函数体
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = Python3Lang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, Python3Lang.EndTokens);
			// 
			if (!e.Ignore)
			{
				var args = argNames.Select(a => new DefineVarNode { Name = a, SystemType = typeof(object) }).ToArray();
				var defineNode = new DefineFuncNode { Name = funcName, Args = args, Body = body, ReturnSystemType = typeof(object) };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineNode);
			}
		}
	}
}
