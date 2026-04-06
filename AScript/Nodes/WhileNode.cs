using System;
using System.CodeDom;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class WhileNode : TreeNode
	{
		public ITreeNode Condition { get; set; }
		public ITreeNode Body { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var tempContext = ScriptContext.Create(context);
			var tempControl = new EvalControl(control, true);
			object bodyResult = null;
			Type bodyType = null;
			while (true)
			{
				if (!EvalCondition(tempContext, options))
				{
					break;
				}
				if (this.Body != null)
				{
					bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out bodyType);
					if (tempControl.Terminal || tempControl.Break) break;
					tempControl.Continue = false;
				}
			}
			returnType = bodyType;
			return bodyResult;
		}

		private bool EvalCondition(ScriptContext context, BuildOptions options)
		{
			if (this.Condition == null) return true;
			var conditionResult = this.Condition.Eval(context, options, null, out var conditionType);
			if (!(conditionResult is bool b))
			{
				throw new Exception($"invalid if condition type {conditionType}");
			}
			return b;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tempBuildContext = new BuildContext(buildContext);
			// 条件
			Expression conditionExpression = this.Condition.Build(tempBuildContext, scriptContext, options);
			// 循环体
			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();
			var bodyBuildContext = new BuildContext(tempBuildContext)
			{
				ContinueLabel = continueLabel,
				BreakLabel = breakLabel
			};
			Expression bodyExpression;
			if (this.Body == null)
			{
				bodyExpression = Expression.Empty();
			}
			else
			{
				bodyExpression = this.Body.Build(bodyBuildContext, scriptContext, options);
				bodyExpression = bodyBuildContext.BuildBlock(scriptContext, options, bodyExpression);
			}
			// 
			Expression loopBlockExpression = Expression.Block(bodyExpression, Expression.Label(continueLabel));
			var loop = Expression.Loop(
				Expression.IfThenElse(conditionExpression, loopBlockExpression, Expression.Break(breakLabel)), 
				breakLabel);
			return loop;
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Condition);
			PoolManage.Return(this.Body);
			this.Condition = null;
			this.Body = null;
		}
	}
}
