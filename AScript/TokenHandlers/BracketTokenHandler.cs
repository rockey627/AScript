using System;
using System.Collections.Generic;
using System.Linq;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// 中括号token解析
	/// </summary>
	public class BracketTokenHandler : ITokenHandler
	{
		public static readonly BracketTokenHandler Instance = new BracketTokenHandler();

		private static readonly HashSet<string> _EndTokens = new HashSet<string> { ":" };

		public virtual void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.Current == null)
			{
				// 创建数组：[1,2,3,4]
				if (BuildCollection(analyzer, e)) return;
				throw new Exception($"invalid expression '[' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			var statement0 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _EndTokens);
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exception($"invalid expression at {e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}, expect ']'");
			}
			if (nextToken.Value.Type == ETokenType.String)
			{
				throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ']'");
			}
			if (nextToken.Value.Value == "]")
			{
				if (!e.Ignore)
				{
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("[]", 2, DefaultSyntaxAnalyzer.OperatorPriorities["["]));
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, statement0);
				}
				return;
			}
			if (TryBuildNext(analyzer, e, statement0, nextToken.Value))
			{
				return;
			}
			throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ']'");
		}

		protected virtual bool BuildCollection(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			// 解析 [1,2,3,4] 数组
			var args = e.Ignore ? null : new List<ITreeNode>();
			var tokenReader = e.TokenReader;

			while (true)
			{
				var element = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, tokenReader, e.Control, e.Ignore);
				if (!e.Ignore)
				{
					args.Add(element);
				}

				var nextToken = tokenReader.Read();
				if (!nextToken.HasValue)
				{
					throw new Exception($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect ']'");
				}
				if (nextToken.Value.Type == ETokenType.String)
				{
					throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ',' or ']'");
				}
				if (nextToken.Value.Value == "]")
				{
					break;
				}

				if (nextToken.Value.Value != ",")
				{
					throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ',' or ']'");
				}
			}

			if (!e.Ignore)
			{
				var arr = CreateCollection(args.Select(a => a.Eval(e.ScriptContext, e.Options, e.Control, out _)).ToList());
				var arrNode = PoolManage.CreateObjectData(arr);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, arrNode);
			}

			return true;
		}

		protected virtual object CreateCollection(List<object> list)
		{
			Type elementType = null;
			foreach (var item in list)
			{
				if (item == null) continue;
				var itemType = item.GetType();
				if (elementType == null)
				{
					elementType = itemType;
					continue;
				}
				if (itemType != elementType)
				{
					elementType = null;
					break;
				}
			}
			if (elementType == null) elementType = typeof(object);
			var arr = Array.CreateInstance(elementType, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				arr.SetValue(list[i], i);
			}
			return arr;
		}

		protected virtual bool TryBuildNext(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, ITreeNode statement0, Token nextToken)
		{
			if (nextToken.Value == ":")
			{
				// [1:3]
				var end = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				analyzer.ValidateNextToken(e.TokenReader, "]", e.CurrentToken.Line, e.CurrentToken.Column);
				if (!e.Ignore)
				{
					var target = e.TreeBuilder.Pop();
					var funcNode = new CallFuncNode { Name = "[:]", Args = new[] { target, statement0, end } };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, funcNode);
				}
				return true;
			}
			return false;
		}
	}
}
