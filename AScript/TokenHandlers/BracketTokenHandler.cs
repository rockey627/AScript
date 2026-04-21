using System;
using System.Collections.Generic;
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
			// 创建数组：[1,2,3,4]

			return true;
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
