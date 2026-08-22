using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AScript.Operators
{
	/// <summary>
	/// 比较运算：小于&lt;
	/// </summary>
	public class LessThanOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly LessThanOperator Instance = new LessThanOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			e.Result = ExpressionUtils.LessThan(left, right);
			//if (left.Type == typeof(object) || right.Type == typeof(object)
			//	|| !ScriptUtils.ConvertMaxType(ref left, ref right))
			//{
			//	e.Result = Expression.Convert(Expression.Dynamic(ExpressionUtils.Binder_LessThan, typeof(object), left, right), typeof(bool));
			//}
			//else
			//{
			//	e.Result = Expression.LessThan(left, right);
			//}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(ExpressionUtils.LessThan(arg0, arg1));
			}
		}
	}
}
