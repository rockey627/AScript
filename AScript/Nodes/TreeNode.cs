using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public abstract class TreeNode : ITreeNode
	{
		public OperatorNode Parent { get; set; }

		public object Eval(ScriptContext context, BuildOptions options, out Type returnType)
		{
			return Eval(context, options, null, out returnType);
		}

		public abstract object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType);
		public abstract Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options);

		public virtual void Clear()
		{
			this.Parent = null;
		}
	}
}
