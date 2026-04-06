using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class BoolNotOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly BoolNotOperator Instance = new BoolNotOperator();

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
				e.SetResult(!arg0);
			}
		}
	}
}
