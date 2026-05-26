using System;
using System.Collections.Generic;

namespace AScript.Nodes
{
	public class TreeNodeVisitor
	{
		public virtual void Visit(IList<ITreeNode> nodes)
		{
			if (nodes == null || nodes.Count == 0) return;
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i] = Visit(nodes[i]);
			}
		}

		public virtual ITreeNode Visit(ITreeNode node)
		{
			if (node == null) return null;
			if (node is DefineVarNode defineVarNode)
			{
				return VisitDefineVarNode(defineVarNode);
			}
			if (node is VariableNode variableNode)
			{
				return VisitVariableNode(variableNode);
			}
			if (node is OperatorNode operatorNode)
			{
				return VisitOperatorNode(operatorNode);
			}
			if (node is NewNode newNode)
			{
				return VisitNewNode(newNode);
			}
			if (node is CallFuncNode callFuncNode)
			{
				return VisitCallFuncNode(callFuncNode);
			}
			return node;
		}

		public virtual ITreeNode VisitDefineVarNode(DefineVarNode defineVarNode)
		{
			return defineVarNode;
		}

		public virtual ITreeNode VisitVariableNode(VariableNode variableNode)
		{
			return variableNode;
		}

		public virtual ITreeNode VisitOperatorNode(OperatorNode operatorNode)
		{
			operatorNode.Left = Visit(operatorNode.Left);
			operatorNode.Right = Visit(operatorNode.Right);
			return operatorNode;
		}

		public virtual ITreeNode VisitNewNode(NewNode newNode)
		{
			if (newNode.Args != null)
			{
				for (int i = 0; i < newNode.Args.Count; i++)
				{
					newNode.Args[i] = Visit(newNode.Args[i]);
				}
			}
			if (newNode.InitProperties != null)
			{
				for (int i = 0; i < newNode.InitProperties.Count; i++)
				{
					newNode.InitProperties[i] = Visit(newNode.InitProperties[i]);
				}
			}
			return newNode;
		}

		public virtual ITreeNode VisitCallFuncNode(CallFuncNode callFuncNode)
		{
			if (callFuncNode.Args != null)
			{
				for (int i = 0; i < callFuncNode.Args.Length; i++)
				{
					callFuncNode.Args[i] = Visit(callFuncNode.Args[i]);
				}
			}
			if (callFuncNode.Target is ITreeNode targetNode)
			{
				callFuncNode.Target = Visit(targetNode);
			}
			return callFuncNode;
		}
	}
}
