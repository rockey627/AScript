using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class ExpressionNode : TreeNode
	{
		public Expression Expr { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return this.Expr;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			throw new NotImplementedException();
		}

		public override void Clear()
		{
			base.Clear();

			this.Expr = null;
		}
	}
}
