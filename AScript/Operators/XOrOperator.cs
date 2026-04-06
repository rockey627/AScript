using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	/// <summary>
	/// 位运算：异或
	/// </summary>
	public class XOrOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly XOrOperator Instance = new XOrOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (left.Type == typeof(object) || right.Type == typeof(object)
					|| !ExpressionUtils.ConvertMaxType(ref left, ref right))
			{
				e.Result = Expression.Dynamic(ExpressionUtils.Binder_XOr, typeof(object), left, right);
			}
			else
			{
				e.Result = Expression.ExclusiveOr(left, right);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(arg0 ^ arg1);
			}
		}
	}
}
