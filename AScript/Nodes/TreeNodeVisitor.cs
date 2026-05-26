using System;
using System.Collections.Generic;

namespace AScript.Nodes
{
	public abstract class TreeNodeVisitor
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
			if (node is BlockNode blockNode)
			{
				return VisitBlockNode(blockNode);
			}
			if (node is IfNode ifNode)
			{
				return VisitIfNode(ifNode);
			}
			if (node is ForNode forNode)
			{
				return VisitForNode(forNode);
			}
			if (node is ForeachNode foreachNode)
			{
				return VisitForeachNode(foreachNode);
			}
			if (node is WhileNode whileNode)
			{
				return VisitWhileNode(whileNode);
			}
			if (node is ReturnNode returnNode)
			{
				return VisitReturnNode(returnNode);
			}
			if (node is BreakNode breakNode)
			{
				return VisitBreakNode(breakNode);
			}
			if (node is ContinueNode continueNode)
			{
				return VisitContinueNode(continueNode);
			}
			if (node is DefineFuncNode defineFuncNode)
			{
				return VisitDefineFuncNode(defineFuncNode);
			}
			if (node is TupleNode tupleNode)
			{
				return VisitTupleNode(tupleNode);
			}
			if (node is CollectionNode collectionNode)
			{
				return VisitCollectionNode(collectionNode);
			}
			if (node is QueryNode queryNode)
			{
				return VisitQueryNode(queryNode);
			}
			if (node is ExpressionNode expressionNode)
			{
				return VisitExpressionNode(expressionNode);
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

		public virtual ITreeNode VisitBlockNode(BlockNode blockNode)
		{
			blockNode.Block = Visit(blockNode.Block);
			return blockNode;
		}

		public virtual ITreeNode VisitIfNode(IfNode ifNode)
		{
			ifNode.Condition = Visit(ifNode.Condition);
			ifNode.Body = Visit(ifNode.Body);
			ifNode.Else = Visit(ifNode.Else);
			return ifNode;
		}

		public virtual ITreeNode VisitForNode(ForNode forNode)
		{
			forNode.Init = Visit(forNode.Init);
			forNode.Condition = Visit(forNode.Condition);
			forNode.Body = Visit(forNode.Body);
			forNode.Post = Visit(forNode.Post);
			return forNode;
		}

		public virtual ITreeNode VisitForeachNode(ForeachNode foreachNode)
		{
			if (foreachNode.VarDefine != null)
			{
				foreachNode.VarDefine = (DefineVarNode)Visit(foreachNode.VarDefine);
			}
			if (foreachNode.VarDefines != null)
			{
				for (int i = 0; i < foreachNode.VarDefines.Count; i++)
				{
					foreachNode.VarDefines[i] = (DefineVarNode)Visit(foreachNode.VarDefines[i]);
				}
			}
			foreachNode.Collection = Visit(foreachNode.Collection);
			foreachNode.Body = Visit(foreachNode.Body);
			return foreachNode;
		}

		public virtual ITreeNode VisitWhileNode(WhileNode whileNode)
		{
			whileNode.Condition = Visit(whileNode.Condition);
			whileNode.Body = Visit(whileNode.Body);
			return whileNode;
		}

		public virtual ITreeNode VisitReturnNode(ReturnNode returnNode)
		{
			returnNode.Body = Visit(returnNode.Body);
			return returnNode;
		}

		public virtual ITreeNode VisitBreakNode(BreakNode breakNode)
		{
			return breakNode;
		}

		public virtual ITreeNode VisitContinueNode(ContinueNode continueNode)
		{
			return continueNode;
		}

		public virtual ITreeNode VisitDefineFuncNode(DefineFuncNode defineFuncNode)
		{
			if (defineFuncNode.Args != null)
			{
				for (int i = 0; i < defineFuncNode.Args.Length; i++)
				{
					defineFuncNode.Args[i] = (DefineVarNode)Visit(defineFuncNode.Args[i]);
				}
			}
			defineFuncNode.Body = Visit(defineFuncNode.Body);
			return defineFuncNode;
		}

		public virtual ITreeNode VisitTupleNode(TupleNode tupleNode)
		{
			Visit(tupleNode.Items);
			return tupleNode;
		}

		public virtual ITreeNode VisitCollectionNode(CollectionNode collectionNode)
		{
			Visit(collectionNode.Items);
			if (collectionNode.ForeachNode != null)
			{
				collectionNode.ForeachNode = (ForeachNode)VisitForeachNode(collectionNode.ForeachNode);
			}
			return collectionNode;
		}

		public virtual ITreeNode VisitQueryNode(QueryNode queryNode)
		{
			// QueryNode 不需要遍历其内部结构
			return queryNode;
		}

		public virtual ITreeNode VisitExpressionNode(ExpressionNode expressionNode)
		{
			return expressionNode;
		}
	}
}
