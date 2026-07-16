using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class AndAlsoOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly AndAlsoOperator Instance = new AndAlsoOperator();

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				if (!e.Context.IsTrue(arg0))
				{
					e.SetResult(false);
					return;
				}
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(e.Context.IsTrue(arg1));
			}
		}

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var arg1 = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (arg0.Type == typeof(bool) && arg1.Type == typeof(bool))
				{
					e.Result = Expression.AndAlso(arg0, arg1);
					return;
				}
				if (arg0.Type.IsValueType) arg0 = Expression.Convert(arg0, typeof(object));
				if (arg1.Type.IsValueType) arg1 = Expression.Convert(arg1, typeof(object));
				var isTrue0 = Expression.Call(e.BuildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_IsTrue, arg0);
				var isTrue1 = Expression.Call(e.BuildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_IsTrue, arg1);
				e.Result = Expression.AndAlso(isTrue0, isTrue1);
			}
		}
	}
}
