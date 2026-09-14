using AScript.Nodes;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// 数组：[5]int/[5]int{}/[]int{}/[...]int{}
	/// 下标：arr[5]
	/// 切片：arr[2:5]
	/// </summary>
	public class GoBracketTokenHandler : BracketTokenHandler
	{
		public GoBracketTokenHandler() : base(typeof(Array))
		{
		}

		public override void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			if (e.TreeBuilder.Current == null || e.TreeBuilder.Current is OperatorNode opNode && !opNode.IsFull())
			{
				// 数组
				var token = analyzer.ValidateNextToken(e.TokenReader);
				// 长度
				ITreeNode count;
				Type collectionType = typeof(Array);
				if (token.Value.IsSymbol("]"))
				{
					// []
					count = null;
					collectionType = typeof(List<>);
				}
				else if (token.Value.IsSymbol("..."))
				{
					// [...]
					count = null;
					analyzer.ValidateNextToken(e.TokenReader, "]");
				}
				else
				{
					// [5]
					e.TokenReader.Push(token.Value);
					count = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					analyzer.ValidateNextToken(e.TokenReader, "]");
				}
				// 元素类型
				token = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				var itemType = e.ScriptContext.EvalType(token.Value.Value);
				if (itemType == null)
				{
					throw new Exceptions.ScriptRuntimeException($"unknown type '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				// 元素列表
				List<ITreeNode> items = null;
				token = e.TokenReader.Read();
				if (token.HasValue)
				{
					if (token.Value.IsSymbol("{"))
					{
						token = analyzer.ValidateNextToken(e.TokenReader);
						if (token.Value.IsSymbol("}")) { }
						else
						{
							items = e.Ignore ? null : new List<ITreeNode>();
							// { 5, 6, 7, 5: 10 }
							e.TokenReader.Push(token.Value);
							while (true)
							{
								var v1 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
								if (v1 == null)
								{
									analyzer.ValidateNextToken(e.TokenReader, "}");
									break;
								}
								token = analyzer.ValidateNextToken(e.TokenReader);
								if (token.Value.IsSymbol(":"))
								{
									var v2 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
									items?.Add(new TupleNode { Items = new[] { v1, v2 } });
								}
								else
								{
									items?.Add(v1);
									if (token.Value.IsSymbol(",")) continue;
									if (token.Value.IsSymbol("}")) break;
									throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
								}
							}
						}
					}
					else
					{
						e.TokenReader.Push(token.Value);
						if (!e.Ignore)
						{
							if (collectionType == typeof(List<>)) collectionType = collectionType.MakeGenericType(itemType);
							else collectionType = itemType.MakeArrayType();
							e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(collectionType));
						}
						return;
					}
				}
				if (!e.Ignore)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new CollectionNode { CollectionType = collectionType, ElementType = itemType, Items = items, Capacity = count });
				}
			}
			else
			{
				// 下标、切片
				base.Build(analyzer, e);
			}
		}
	}
}
