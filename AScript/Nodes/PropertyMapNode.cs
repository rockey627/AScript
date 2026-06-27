using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class PropertyMapNode : TreeNode
	{
		public string PropertyName { get; set; }
		public ITreeNode MapNode { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			throw new NotImplementedException();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			throw new NotImplementedException();
		}
	}
}
