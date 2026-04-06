using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class LeftShiftAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly LeftShiftAssignOperator Instance = new LeftShiftAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (left.Type == typeof(object) || right.Type == typeof(object)
					|| !ExpressionUtils.ConvertMaxType(ref left, ref right))
			{
				var expr = Expression.Dynamic(ExpressionUtils.Binder_LeftShift, typeof(object), left, right);
				e.Result = Expression.Assign(left, expr);
			}
			else
			{
				e.Result = Expression.LeftShiftAssign(left, right);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(arg0 >> arg1);
			}
		}
	}
}
