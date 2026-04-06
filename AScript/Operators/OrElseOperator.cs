using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class OrElseOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly OrElseOperator Instance = new OrElseOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var arg1 = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				e.Result = Expression.OrElse(arg0, arg1);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = (bool)e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var arg1 = (bool)e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(arg0 || arg1);
			}
		}
	}
}
