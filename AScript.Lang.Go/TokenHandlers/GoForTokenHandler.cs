using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.ObjectModel;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言for循环处理器
	/// 格式1：for i := 0; i &lt; n; i++ { }   // 传统for循环
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
			var token = analyzer.ValidateNextToken(e.TokenReader);

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

			if (TryParseRange(analyzer, e, createFullOptions, token))
			{
				return;
			}

			// 传统for循环或while型for
			e.TokenReader.Push(token.Value);

			// 尝试解析第一个语句
			var first = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);

			token = analyzer.ValidateNextToken(e.TokenReader);
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

		// for k, v := range m { }
		private bool TryParseRange(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions, Token? token)
		{
			if (token.Value.Type != ETokenType.Word) return false;

			var nextToken = analyzer.ValidateNextToken(e.TokenReader);

			if (nextToken.Value.IsSymbol(","))
			{
				var token2 = analyzer.ValidateNextToken(e.TokenReader);
				if (token2.Value.Type != ETokenType.Word)
				{
					e.TokenReader.Push(token2.Value);
					e.TokenReader.Push(nextToken.Value);
					return false;
				}
				// =或者:=
				var nextToken2 = analyzer.ValidateNextToken(e.TokenReader);
				if (!nextToken2.Value.IsSymbol("=") && !nextToken2.Value.IsSymbol(":="))
				{
					e.TokenReader.Push(nextToken2.Value);
					e.TokenReader.Push(token2.Value);
					e.TokenReader.Push(nextToken.Value);
					return false;
				}
				// range
				var rangeToken = analyzer.ValidateNextToken(e.TokenReader);
				if (!rangeToken.Value.IsSymbol("range"))
				{
					e.TokenReader.Push(rangeToken.Value);
					e.TokenReader.Push(nextToken2.Value);
					e.TokenReader.Push(token2.Value);
					e.TokenReader.Push(nextToken.Value);
					return false;
				}
				// list
				var list = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
				if (!e.Ignore)
				{
					var foreachNode = new ForeachNode
					{
						Collection = new CallFuncNode { Name = "range", Args = new[] { list } },
						Body = body,
						VarDefines = new[] 
						{
							new DefineVarNode(token.Value.Value),
							new DefineVarNode(token2.Value.Value)
						}
					};
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
				}
				return true;
			}

			if (nextToken.Value.IsSymbol("=") || nextToken.Value.IsSymbol(":="))
			{
				var rangeToken = analyzer.ValidateNextToken(e.TokenReader);
				if (!rangeToken.Value.IsSymbol("range"))
				{
					e.TokenReader.Push(rangeToken.Value);
					e.TokenReader.Push(nextToken.Value);
					return false;
				}
				var list = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
				if (!e.Ignore)
				{
					var foreachNode = new ForeachNode
					{
						Collection = new CallFuncNode { Name = "range", Args = new[] { list } },
						Body = body,
						VarDefines = new[]
						{
							new DefineVarNode(token.Value.Value)
						}
					};
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
				}
				return true;
			}

			// 
			e.TokenReader.Push(nextToken.Value);
			return false;
		}
	}
}
