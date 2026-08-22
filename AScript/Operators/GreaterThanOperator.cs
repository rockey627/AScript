using System;

namespace AScript.Operators
{
	public class GreaterThanOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly GreaterThanOperator Instance = new GreaterThanOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			e.Result = ExpressionUtils.GreaterThan(left, right);
			//if (left.Type == typeof(object) || right.Type == typeof(object)
			//	|| !ScriptUtils.ConvertMaxType(ref left, ref right))
			//{
			//	e.Result = Expression.Convert(Expression.Dynamic(ExpressionUtils.Binder_GreaterThan, typeof(object), left, right), typeof(bool));
			//}
			//else
			//{
			//	e.Result = Expression.GreaterThan(left, right);
			//}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(ExpressionUtils.GreaterThan(arg0, arg1));
				//e.SetResult(arg0 > arg1);
			}
		}
	}
}
