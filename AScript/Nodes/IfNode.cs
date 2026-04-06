using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class IfNode : TreeNode
	{
		public ITreeNode Condition { get; set; }
		public ITreeNode Body { get; set; }
		public ITreeNode Else { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (EvalCondition(context))
			{
				if (this.Body == null)
				{
					returnType = null;
					return null;
				}
				return this.Body.Eval(context, options, control, out returnType);
			}
			if (this.Else != null)
			{
				return this.Else.Eval(context, options, control, out returnType);
			}
			returnType = null;
			return null;
		}

		private bool EvalCondition(ScriptContext context)
		{
			if (this.Condition == null) return true;
			var conditionResult = this.Condition.Eval(context, null, null, out var conditionType);
			if (!(conditionResult is bool b))
			{
				throw new Exception($"invalid if condition type {conditionType}");
			}
			return b;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var testExpr = this.Condition.Build(buildContext, scriptContext, options);
			var ifTrueExpr = this.Body.Build(buildContext, scriptContext, options);
			if (this.Else == null)
			{
				return Expression.IfThen(testExpr, ifTrueExpr);
			}
			var elseExpr = this.Else.Build(buildContext, scriptContext, options);
			return Expression.IfThenElse(testExpr, ifTrueExpr, elseExpr);
		}

		public void Clear()
		{
			PoolManage.Return(Condition);
			PoolManage.Return(Body);
			if (this.Else != null)
			{
				this.Else.Clear();
				this.Else = null;
			}
			this.Condition = null;
			this.Body = null;
		}
	}
}
