using AScript.Nodes;
using System;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlQueryNodeVisitor : SqlTreeNodeVisitor
	{
		private readonly BuildContext _BuildContext;
		private readonly ScriptContext _ScriptContext;
		private readonly QueryNode _QueryNode;

		public SqlQueryNodeVisitor(BuildContext buildContext, ScriptContext scriptContext, QueryNode queryNode)
		{
			_BuildContext = buildContext;
			_ScriptContext = scriptContext;
			_QueryNode = queryNode;
		}

		public override ITreeNode VisitVariableNode(VariableNode variableNode)
		{
			if (ScriptUtils.IsVariableExists(_BuildContext, _ScriptContext, variableNode.Name))
			{
				return variableNode;
			}
			if (_QueryNode.Source is CallFuncNode callFuncNode && callFuncNode.Name == "GroupBy")
			{
				var keyOpNode = new OperatorNode(".", 0, 2)
				{
					Left = new VariableNode(_QueryNode.CurrentVarName),
					Right = new VariableNode("Key")
				};
				if (variableNode.Parent != null)
				{
					return keyOpNode;
				}
				return new OperatorNode(".", 0, 2)
				{
					Left = variableNode,
					Right = keyOpNode
				};
			}
			return new OperatorNode(".", 0, 2)
			{
				Left = new VariableNode(_QueryNode.CurrentVarName),
				Right = variableNode
			};
		}

		public override ITreeNode VisitOperatorNode(OperatorNode operatorNode)
		{
			if (operatorNode.Name == "." || operatorNode.Name == "?.")
			{
				if (_QueryNode.Source is CallFuncNode sourceCallFuncNode && sourceCallFuncNode.Name == "GroupBy")
				{
					var keyOpNode = new OperatorNode(".", 0, 2)
					{
						Left = new VariableNode(_QueryNode.CurrentVarName),
						Right = new VariableNode("Key")
					};
					if (operatorNode.Parent != null)
					{
						return keyOpNode;
					}
					operatorNode = new OperatorNode("=", 0, 2)
					{
						Left = operatorNode.Right,
						Right = keyOpNode
					};
				}
				return operatorNode;
			}
			if (operatorNode.Name == "=")
			{
				operatorNode.Right = Visit(operatorNode.Right);
				return operatorNode;
			}
			return base.VisitOperatorNode(operatorNode);
		}

		public override ITreeNode VisitCallFuncNode(CallFuncNode callFuncNode)
		{
			if ("count".Equals(callFuncNode.Name, StringComparison.OrdinalIgnoreCase))
			{
				callFuncNode.Name = "Count";
				callFuncNode.Args = new ITreeNode[]
				{
						new VariableNode(_QueryNode.CurrentVarName)
				};
				return callFuncNode;
			}
			return base.VisitCallFuncNode(callFuncNode);
		}
	}
}
