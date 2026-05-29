using AScript.Nodes;
using System;

namespace AScript.Lang.Sql.Nodes
{
	public abstract class SqlTreeNodeVisitor : TreeNodeVisitor
	{
		public override ITreeNode Visit(ITreeNode node)
		{
			if (node is SqlLikeNode likeNode)
			{
				return VisitLikeNode(likeNode);
			}
			return base.Visit(node);
		}

		public virtual ITreeNode VisitLikeNode(SqlLikeNode likeNode)
		{
			likeNode.Arg1 = Visit(likeNode.Arg1);
			return likeNode;
		}
	}
}
