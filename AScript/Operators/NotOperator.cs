using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	/// <summary>
	/// 位运算：非~
	/// </summary>
	public class NotOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly NotOperator Instance = new NotOperator();

		public void Build(FunctionBuildArgs e)
		{
			var arg = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (arg.Type == typeof(object))
			{
				e.Result = Expression.Dynamic(ExpressionUtils.Binder_Not, typeof(object), arg);
			}
			else
			{
				e.Result = Expression.Not(arg);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 1)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(~arg0);
			}
		}
	}
}
