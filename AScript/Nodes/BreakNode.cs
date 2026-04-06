using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class BreakNode : TreeNode
	{
		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (control == null)
			{
				throw new Exception("invalid break statement");
			}
			control.Break = true;
			returnType = null;
			return null;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return Expression.Break(buildContext.BreakLabel);
		}
	}
}
