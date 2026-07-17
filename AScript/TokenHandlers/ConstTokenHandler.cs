using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	public class ConstTokenHandler : ITokenHandler
	{
		public static readonly ConstTokenHandler Instance = new ConstTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			if (!e.Ignore)
			{
				string errMsg = $"invalid expression near '{e.CurrentToken.Value}' at ({e.CurrentToken.Line},{e.CurrentToken.Column})";
				SetConst(statement, errMsg);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, statement);
			}
		}

		private void SetConst(ITreeNode node, string errMsg)
		{
			if (node is TreeBuilder treeBuilder)
			{
				node = treeBuilder.Root;
			}
			// 
			if (node is DefineVarNode defineVarNode)
			{
				defineVarNode.Modifier = Modifiers.CONST;
				return;
			}
			if (node is PropertyMapNode propertyMapNode)
			{
				SetConst(propertyMapNode.MapNode, errMsg);
				return;
			}
			if (node is OperatorNode operatorNode && operatorNode.Name == "=")
			{
				SetConst(operatorNode.Left, errMsg);
				return;
			}
			if (node is MultiNode multiNode)
			{
				foreach (var item in multiNode.Nodes)
				{
					SetConst(item, errMsg);
				}
				return;
			}
			if (node is TupleNode tupleNode)
			{
				foreach (var item in tupleNode.Items)
				{
					SetConst(item, errMsg);
				}
				return;
			}
			if (node is CollectionNode collectionNode)
			{
				foreach (var item in collectionNode.Items)
				{
					SetConst(item, errMsg);
				}
				return;
			}
			throw new Exceptions.ScriptAnalyzingException(errMsg);
		}
	}
}
