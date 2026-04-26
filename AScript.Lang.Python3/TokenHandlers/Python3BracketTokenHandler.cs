using AScript.Nodes;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Python3.TokenHandlers
{
	public class Python3BracketTokenHandler : BracketTokenHandler
	{
		public static readonly Python3BracketTokenHandler Instance = new Python3BracketTokenHandler();

		protected override CollectionNode CreateCollection(IList<ITreeNode> items, ForeachNode foreachNode)
		{
			return new CollectionNode { Items = items, ForeachNode = foreachNode, CollectionType = typeof(List<object>)};
		}
	}
}
