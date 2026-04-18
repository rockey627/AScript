using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class PowerOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PowerOperator Instance = new PowerOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			var v1 = Expression.Convert(left, typeof(double));
			var v2 = Expression.Convert(right, typeof(double));
			var v = Expression.Call(ExpressionUtils.Method_Math_Power, v1, v2);
			var maxType = ScriptUtils.GetMaxType(left.Type, right.Type);
			e.Result = Expression.Convert(v, maxType);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var v1 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var v2 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				var r = Math.Pow(Convert.ToDouble(v1), Convert.ToDouble(v2));
				var maxType = ScriptUtils.GetMaxType(v1.GetType(), v2.GetType());
				e.SetResult(ScriptUtils.Convert(r, maxType), maxType);
			}
		}
	}
}
