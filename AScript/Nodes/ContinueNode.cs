using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class ContinueNode : TreeNode
	{
		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (control == null)
			{
				throw new Exception("invalid continue statement");
			}
			control.Continue = true;
			returnType = null;
			return null;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			//if (buildContext.ContinueLabel == null)
			//{
			//	buildContext.ContinueLabel = Expression.Label();
			//}
			return Expression.Continue(buildContext.ContinueLabel);
		}
	}
}
