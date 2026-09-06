using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言for循环处理器
	/// 格式1：for i := 0; i < n; i++ { }   // 传统for循环
	/// 格式2：for condition { }              // while循环
	/// 格式3：for { }                        // 无限循环
	/// 格式4：for k, v := range m { }        // range循环
	/// </summary>
	public class GoForTokenHandler : ITokenHandler
	{
		public static readonly GoForTokenHandler Instance = new GoForTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };

			// 查看下一个token确定for循环类型
			var token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid for expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}

			// 无限循环：for { }
			if (token.Value.IsSymbol("{"))
			{
				e.TokenReader.Push(token.Value);
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
				if (!e.Ignore)
				{
					var forNode = new ForNode { Body = body };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forNode);
				}
				return;
			}

			// 检查是否有分号，判断是否是传统for循环
			e.TokenReader.Push(token.Value);

			// 读取更多token来判断
			token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid for expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}

			// range循环：for k, v := range m { }
			if (token.Value.Value == "range")
			{
				// 简化处理，range m
				var collection = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
				if (!e.Ignore)
				{
					var foreachNode = new ForeachNode { Collection = collection, Body = body };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
				}
				return;
			}

			// 传统for循环或while型for
			e.TokenReader.Push(token.Value);

			// 尝试解析第一个语句
			var first = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);

			token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid for expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}

			if (token.Value.IsSymbol(";"))
			{
				// 传统for循环：for init; condition; post { }
				var init = first;
				var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				token = analyzer.ValidateNextToken(e.TokenReader, ";");
				var post = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

				if (!e.Ignore)
				{
					var forNode = new ForNode { Init = init, Condition = condition, Post = post, Body = body };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forNode);
				}
			}
			else
			{
				// while型for循环：for condition { }
				e.TokenReader.Push(token.Value);
				var condition = first;
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

				if (!e.Ignore)
				{
					var forNode = new ForNode { Condition = condition, Body = body };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forNode);
				}
			}
		}
	}
}
