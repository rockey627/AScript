using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	public class ReadOnlyTokenHandler : ITokenHandler
	{
		public static readonly ReadOnlyTokenHandler Instance = new ReadOnlyTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.Type == ETokenType.Word)
			{
				string varName;
				Type varType = null;
				var nextToken2 = analyzer.ValidateNextToken(e.TokenReader);
				if (nextToken2.Value.Type == ETokenType.Word)
				{
					// const int n=10;
					varType = e.ScriptContext.EvalType(nextToken.Value.Value);
					if (varType == null)
					{
						throw new Exceptions.ScriptAnalyzingException($"unknown type '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
					}
					varName = nextToken2.Value.Value;
					nextToken = e.TokenReader.Read();
				}
				else
				{
					// const n=10
					varName = nextToken.Value.Value;
					nextToken = nextToken2;
				}

				if (!nextToken.HasValue)
				{
					if (!e.Ignore)
					{
						var defineVarNode = PoolManage.CreateDefineVarNode(varName, null, systemType: varType ?? typeof(object), Modifiers.READONLY);
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineVarNode);
					}
					return;
				}
				if (nextToken.Value.IsSymbol("="))
				{
					var value = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					if (!e.Ignore)
					{
						var defineVarNode = PoolManage.CreateDefineVarNode(varName, null, systemType: varType ?? typeof(object), Modifiers.READONLY);
						var opNode = PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]);
						opNode.Left = defineVarNode;
						opNode.Right = value;
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, opNode);
					}
					return;
				}

				e.TokenReader.Push(nextToken.Value);
				return;
			}

			if (nextToken.Value.IsSymbol("("))
			{
				var list = e.Ignore ? null : new List<ITreeNode>();
				while (true)
				{
					nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					list?.Add(PoolManage.CreateDefineVarNode(nextToken.Value.Value, null, systemType: typeof(object), Modifiers.READONLY));
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol(")")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
				if (!e.Ignore)
				{
					var node = new TupleNode { Items = list };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, node);
				}
				analyzer.ValidateNextToken(e.TokenReader, "=");
				if (!e.Ignore)
				{
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]));
				}
				return;
			}

			if (nextToken.Value.IsSymbol("{"))
			{
				var list = BuildList(analyzer, e, "}");
				if (!e.Ignore)
				{
					var node = new CollectionNode { Items = list, CollectionType = typeof(object) };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, node);
				}
				analyzer.ValidateNextToken(e.TokenReader, "=");
				if (!e.Ignore)
				{
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]));
				}
				return;
			}

			if (nextToken.Value.IsSymbol("["))
			{
				var list = BuildList(analyzer, e, "]");
				if (!e.Ignore)
				{
					var node = new CollectionNode { Items = list, CollectionType = typeof(Array) };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, node);
				}
				analyzer.ValidateNextToken(e.TokenReader, "=");
				if (!e.Ignore)
				{
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]));
				}
				return;
			}

			throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
		}

		private List<ITreeNode> BuildList(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, string endToken)
		{
			var list = e.Ignore ? null : new List<ITreeNode>();
			while (true)
			{
				var nextToken = analyzer.ValidateNextToken(e.TokenReader);
				if (nextToken.Value.IsSymbol(","))
				{
					list?.Add(null);
					continue;
				}
				if (nextToken.Value.IsSymbol("{"))
				{
					var list2 = BuildList(analyzer, e, "}");
					if (list != null)
					{
						var node = new CollectionNode { Items = list2, CollectionType = typeof(object) };
						list.Add(node);
					}
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
				}
				else if (nextToken.Value.IsSymbol("["))
				{
					var list2 = BuildList(analyzer, e, "]");
					if (list != null)
					{
						var node = new CollectionNode { Items = list2, CollectionType = typeof(Array) };
						list.Add(node);
					}
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
				}
				else
				{
					if (nextToken.Value.Type != ETokenType.Word)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({nextToken.Value.Line},{nextToken.Value.Column}), expect Word");
					}
					string varName = nextToken.Value.Value;
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
					if (nextToken.Value.IsSymbol("="))
					{
						var value = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
						if (list != null)
						{
							var opNode = PoolManage.CreateOperatorNode("=", 2, 0);
							opNode.Left = PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object), Modifiers.READONLY);
							opNode.Right = value;
							list.Add(opNode);
						}
						nextToken = analyzer.ValidateNextToken(e.TokenReader);
					}
					else if (nextToken.Value.IsSymbol(":"))
					{
						nextToken = analyzer.ValidateNextToken(e.TokenReader);
						if (nextToken.Value.Type == ETokenType.Word)
						{
							if (list != null)
							{
								var node = new PropertyMapNode { PropertyName = varName, MapNode = new DefineVarNode(nextToken.Value.Value, null, typeof(object)) };
								list.Add(node);
							}
						}
						else if (nextToken.Value.IsSymbol("{"))
						{
							var list2 = BuildList(analyzer, e, "}");
							if (list != null)
							{
								var node = new CollectionNode { Items = list2, CollectionType = typeof(object) };
								list.Add(new PropertyMapNode { PropertyName = varName, MapNode = node });
							}
						}
						else if (nextToken.Value.IsSymbol("["))
						{
							var list2 = BuildList(analyzer, e, "]");
							if (list != null)
							{
								var node = new CollectionNode { Items = list2, CollectionType = typeof(Array) };
								list.Add(new PropertyMapNode { PropertyName = varName, MapNode = node });
							}
						}
						else
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
						}
						nextToken = analyzer.ValidateNextToken(e.TokenReader);
					}
					else
					{
						list?.Add(PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object), Modifiers.READONLY));
					}
				}
				if (nextToken.Value.IsSymbol(",")) continue;
				if (nextToken.Value.IsSymbol(endToken)) break;
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			return list;
		}
	}
}
