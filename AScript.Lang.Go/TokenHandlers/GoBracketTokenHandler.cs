using AScript.Nodes;
using AScript.Readers;
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
				var arrType = GoLang.ParseArrayType(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Ignore, out var arrTypeName);
				var value = ParseValue(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, arrType, e.Ignore);
				if (!e.Ignore)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, value);
				}
				//var itemType = arrType.ItemType;
				//// 元素列表
				//List<ITreeNode> items = null;
				//var token = e.TokenReader.Read();
				//if (token.HasValue)
				//{
				//	if (token.Value.IsSymbol("{"))
				//	{
				//		token = analyzer.ValidateNextToken(e.TokenReader);
				//		if (token.Value.IsSymbol("}")) { }
				//		else
				//		{
				//			items = e.Ignore ? null : new List<ITreeNode>();
				//			// { 5, 6, 7, 5: 10 }
				//			e.TokenReader.Push(token.Value);
				//			while (true)
				//			{
				//				var v1 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				//				if (v1 == null)
				//				{
				//					analyzer.ValidateNextToken(e.TokenReader, "}");
				//					break;
				//				}
				//				token = analyzer.ValidateNextToken(e.TokenReader);
				//				if (token.Value.IsSymbol(":"))
				//				{
				//					var v2 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				//					items?.Add(new TupleNode { Items = new[] { v1, v2 } });
				//				}
				//				else
				//				{
				//					items?.Add(v1);
				//					if (token.Value.IsSymbol(",")) continue;
				//					if (token.Value.IsSymbol("}")) break;
				//					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				//				}
				//			}
				//		}
				//	}
				//	else
				//	{
				//		e.TokenReader.Push(token.Value);
				//		if (!e.Ignore)
				//		{
				//			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(arrType.RealType));
				//		}
				//		return;
				//	}
				//}
				//if (!e.Ignore)
				//{
				//	e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new CollectionNode { CollectionType = arrType.RealType, ElementType = itemType, Items = items, Length = arrType.Length });
				//}
			}
			else
			{
				// 下标、切片
				base.Build(analyzer, e);
			}
		}

		private static ITreeNode ParseValue(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, Type valueType, bool ignore)
		{
			if (valueType is GoArrayType goArrayType)
			{
				// 数组
				List<ITreeNode> items = null;
				var token = tokenReader.Read();
				if (token.HasValue)
				{
					if (token.Value.IsSymbol("{"))
					{
						token = analyzer.ValidateNextToken(tokenReader);
						if (token.Value.IsSymbol("}")) { }
						else
						{
							items = ignore ? null : new List<ITreeNode>();
							// { 5, 6, 7, 5: 10 }
							tokenReader.Push(token.Value);
							while (true)
							{
								var item = ParseValue(analyzer, buildContext, scriptContext, options, tokenReader, control, goArrayType.ItemType, ignore);
								if (item != null) items?.Add(item);
								token = analyzer.ValidateNextToken(tokenReader);
								if (token.Value.IsSymbol(",")) continue;
								if (token.Value.IsSymbol("}")) break;
								throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
								//var v1 = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
								//if (v1 == null)
								//{
								//	analyzer.ValidateNextToken(tokenReader, "}");
								//	break;
								//}
								//token = analyzer.ValidateNextToken(tokenReader);
								//if (token.Value.IsSymbol(":"))
								//{
								//	var v2 = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
								//	items?.Add(new TupleNode { Items = new[] { v1, v2 } });
								//}
								//else
								//{
								//	items?.Add(v1);
								//	if (token.Value.IsSymbol(",")) continue;
								//	if (token.Value.IsSymbol("}")) break;
								//	throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
								//}
							}
						}
					}
					else
					{
						tokenReader.Push(token.Value);
						return ignore ? null : PoolManage.CreateObjectNode(goArrayType.RealType);
					}
				}
				if (ignore) return null;
				var itemRealType = goArrayType.ItemType is GoArrayType itemGoArrayType ? itemGoArrayType.RealType : goArrayType.ItemType;
				return new CollectionNode { CollectionType = goArrayType.RealType, ElementType = itemRealType, Items = items, Length = goArrayType.Length };
			}
			else
			{
				var v1 = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
				if (v1 == null)
				{
					return null;
				}
				var token = analyzer.ValidateNextToken(tokenReader);
				if (token.Value.IsSymbol(":"))
				{
					var v2 = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
					return ignore ? null : new TupleNode { Items = new[] { v1, v2 } };
				}
				else
				{
					tokenReader.Push(token.Value);
					return v1;
					//items?.Add(v1);
					//if (token.Value.IsSymbol(",")) continue;
					//if (token.Value.IsSymbol("}")) break;
					//throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
			}
		}
	}
}
