using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
		private static readonly HashSet<string> _EndTokens2 = new HashSet<string> { "for", "foreach" };

		public virtual void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.Current == null || e.TreeBuilder.Current is OperatorNode opNode && !opNode.IsFull())
			{
				// 创建数组：[1,2,3,4]
				if (BuildCollection(analyzer, e)) return;
				throw new Exception($"invalid expression '[' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			// 索引器、集合切片
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
			List<ITreeNode> args = null;
			ForeachNode foreachNode = null;
			var tokenReader = e.TokenReader;

			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exception($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect ']'");
			}
			if (nextToken.Value.Type != ETokenType.String && nextToken.Value.Value == ",")
			{
				throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect value or ']'");
			}
			if (nextToken.Value.Type == ETokenType.String || nextToken.Value.Value != "]")
			{
				tokenReader.Push(nextToken.Value);
				var createFullNodeOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
				var s0 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullNodeOptions, tokenReader, e.Control, e.Ignore, endTokens: _EndTokens2);
				nextToken = tokenReader.Read();
				if (!nextToken.HasValue)
				{
					throw new Exception($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect ']'");
				}
				if (nextToken.Value.Type == ETokenType.Word && _EndTokens2.Contains(nextToken.Value.Value))
				{
					// 推导式：x+2 for x in [1,2,3]
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exception($"invalid {e.CurrentToken.Value} expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
					}
					if (nextToken.Value.Type != ETokenType.Word)
					{
						throw new Exception($"invalid '{nextToken.Value.Value}' of {e.CurrentToken.Value} expression at ({nextToken.Value.Line},{nextToken.Value.Column})");
					}
					var varName = nextToken.Value.Value;
					List<DefineVarNode> varDefines = null;

					// 检查是否是变量名列表 (for x, y, z in ...)
					nextToken = e.TokenReader.Read();

					if (nextToken.HasValue && nextToken.Value.Type != ETokenType.String && nextToken.Value.Value == ",")
					{
						if (!e.Ignore)
						{
							varDefines = new List<DefineVarNode>();
							varDefines.Add(PoolManage.CreateDefineVarNode(varName, null, typeof(object)));
						}
						while (nextToken.HasValue && nextToken.Value.Type != ETokenType.String && nextToken.Value.Value == ",")
						{
							var varToken = e.TokenReader.Read();
							if (!varToken.HasValue || varToken.Value.Type != ETokenType.Word || varToken.Value.Value == "in")
							{
								throw new Exception($"invalid variable name at ({varToken.Value.Line},{varToken.Value.Column})");
							}
							if (!e.Ignore)
							{
								varName = varToken.Value.Value;
								varDefines.Add(PoolManage.CreateDefineVarNode(varName, null, typeof(object)));
							}
							nextToken = e.TokenReader.Read();
						}
					}

					if (!nextToken.HasValue)
					{
						throw new Exception($"invalid {e.CurrentToken.Value} expression at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
					}
					if (nextToken.Value.Type == ETokenType.String || nextToken.Value.Value != "in")
					{
						throw new Exception($"invalid {nextToken.Value.Value} of {e.CurrentToken.Value} expression at ({nextToken.Value.Line},{nextToken.Value.Column})");
					}
					//analyzer.ValidateNextToken(e.TokenReader, "in");
					// 
					var listBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					
					if (!e.Ignore)
					{
						foreachNode = new ForeachNode
						{
							VarDefine = varDefines == null ? PoolManage.CreateDefineVarNode(varName, null, typeof(object)) : null,
							VarDefines = varDefines,
							Collection = listBuilder,
							Body = s0
						};
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
					}
				}
				else if (nextToken.Value.Type == ETokenType.String)
				{
					throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ',' or ']'");
				}
				else if (nextToken.Value.Value == "]")
				{
					if (!e.Ignore)
					{
						args = new List<ITreeNode>();
						args.Add(s0);
					}
				}
				else if (nextToken.Value.Value != ",")
				{
					throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ',' or ']'");
				}
				else
				{
					if (!e.Ignore)
					{
						args = new List<ITreeNode>();
						args.Add(s0);
					}
					while (true)
					{
						var element = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, tokenReader, e.Control, e.Ignore);

						if (!e.Ignore)
						{
							args.Add(element);
						}

						nextToken = tokenReader.Read();
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
				}
			}

			if (e.Ignore) return true;

			var collectionNode = CreateCollection(args, foreachNode);
			if (e.Options.CreateFullTreeNode ?? false)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, collectionNode);
			}
			else if ((e.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var expr = collectionNode.Build(e.BuildContext, e.ScriptContext, e.Options);
				PoolManage.Return(collectionNode);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateExpressionNode(expr));
			}
			else
			{
				var arr = collectionNode.Eval(e.ScriptContext, e.Options, out _);
				PoolManage.Return(collectionNode);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(arr));
			}
			return true;
		}

		/// <summary>
		/// items与foreachNode设置一个值
		/// </summary>
		/// <param name="items">集合列表</param>
		/// <param name="foreachNode">推导式</param>
		/// <returns></returns>
		protected virtual CollectionNode CreateCollection(IList<ITreeNode> items, ForeachNode foreachNode)
		{
			return new CollectionNode { Items = items, ForeachNode = foreachNode, CollectionType = typeof(Array) };
		}

		protected virtual bool TryBuildNext(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, ITreeNode statement0, Token nextToken)
		{
			if (nextToken.Value == ":")
			{
				// [1:3]
				var end = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				analyzer.ValidateNextToken(e.TokenReader, "]");
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
