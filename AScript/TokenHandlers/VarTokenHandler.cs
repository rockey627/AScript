using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	public class VarTokenHandler : ITokenHandler
	{
		public static readonly VarTokenHandler Instance = new VarTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.Type == ETokenType.Word)
			{
				if (!e.Ignore)
				{
					var defineVarNode = PoolManage.CreateDefineVarNode(nextToken.Value.Value, null, systemType: typeof(object));
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineVarNode);
				}
				return;
			}

			if (nextToken.Value.IsSymbol("("))
			{
				var list = e.Ignore ? null : new List<ITreeNode>();
				while (true)
				{
					nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					list?.Add(PoolManage.CreateDefineVarNode(nextToken.Value.Value, null, systemType: typeof(object)));
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
				var list = e.Ignore ? null : new List<ITreeNode>();
				while (true)
				{
					nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					string varName = nextToken.Value.Value;
					//list?.Add(PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object)));
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
					if (nextToken.Value.IsSymbol("="))
					{
						var value = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
						if (list != null)
						{
							var opNode = PoolManage.CreateOperatorNode("=", 2, 0);
							opNode.Left = PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object));
							opNode.Right = value;
							list.Add(opNode);
						}
						nextToken = analyzer.ValidateNextToken(e.TokenReader);
					}
					else
					{
						list?.Add(PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object)));
					}
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol("}")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
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
				var list = e.Ignore ? null : new List<ITreeNode>();
				while (true)
				{
					//nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
					if (nextToken.Value.IsSymbol(","))
					{
						list?.Add(null);
						continue;
					}
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
							opNode.Left = PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object));
							opNode.Right = value;
							list.Add(opNode);
						}
						nextToken = analyzer.ValidateNextToken(e.TokenReader);
					}
					else
					{
						list?.Add(PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object)));
					}
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol("]")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
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
	}
}
