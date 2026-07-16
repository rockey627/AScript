using AScript.Exceptions;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
    public class IfNode : TreeNode
	{
		public bool ReturnValue { get; set; }
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

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			if (await EvalConditionAsync(context, cancellationToken).ConfigureAwait(false))
			{
				if (this.Body == null)
				{
					return default;
				}
				return await this.Body.EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
			}
			if (this.Else != null)
			{
				return await this.Else.EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
			}
			return default;
		}

		private bool EvalCondition(ScriptContext context)
		{
			if (this.Condition == null) return true;
			var conditionResult = this.Condition.Eval(context, null, null, out var conditionType);
			if (conditionResult is bool b) return b;
			return context.IsTrue(conditionResult);
		}

		private async Task<bool> EvalConditionAsync(ScriptContext context, CancellationToken cancellationToken)
		{
			if (this.Condition == null) return true;
			var result = await this.Condition.EvalAsync(context, null, null, cancellationToken).ConfigureAwait(false);
			var conditionResult = result.Value;
			if (conditionResult is bool b) return b;
			return context.IsTrue(conditionResult);
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var testExpr = this.Condition.Build(buildContext, scriptContext, options);
			if (testExpr.Type != typeof(bool))
			{
				if (testExpr.Type.IsValueType) testExpr = Expression.Convert(testExpr, typeof(object));
				var isTrue0 = Expression.Call(buildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_IsTrue, testExpr);
				testExpr = isTrue0;
			}
			var ifTrueExpr = this.Body.Build(buildContext, scriptContext, options);
			if (this.Else == null)
			{
				if (this.ReturnValue)
				{
					//var v = Expression.Variable(ifTrueExpr.Type);
					//var assign = Expression.Assign(v, ifTrueExpr);
					//var ifExpr = Expression.IfThen(testExpr, assign);
					//return Expression.Block(new[] { v }, ifExpr, v);
					var elseValue = Expression.Constant(ScriptUtils.GetDefaultValue(ifTrueExpr.Type));
					return Expression.Condition(testExpr, ifTrueExpr, elseValue);
				}
				return Expression.IfThen(testExpr, ifTrueExpr);
			}
			var elseExpr = this.Else.Build(buildContext, scriptContext, options);
			if (this.ReturnValue)
			{
				//var v = Expression.Variable(ifTrueExpr.Type);
				//var ifTrueAssign = Expression.Assign(v, ifTrueExpr);
				//var elseAssign = Expression.Assign(v, elseExpr);
				//var ifExpr = Expression.IfThenElse(testExpr, ifTrueAssign, elseAssign);
				//return Expression.Block(new[] { v }, ifExpr, v);
				return Expression.Condition(testExpr, ifTrueExpr, elseExpr);
			}
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
