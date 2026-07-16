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
				if (arg0.Type == typeof(bool) && arg1.Type == typeof(bool))
				{
					e.Result = Expression.OrElse(arg0, arg1);
					return;
				}
				var expr0 = arg0;
				var expr1 = arg1;
				if (expr0.Type.IsValueType) expr0 = Expression.Convert(expr0, typeof(object));
				if (expr1.Type.IsValueType) expr1 = Expression.Convert(expr1, typeof(object));
				var isTrue0 = Expression.Call(e.BuildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_IsTrue, expr0);
				var isTrue1 = Expression.Call(e.BuildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_IsTrue, expr1);
				if (!ExpressionUtils.ConvertMaxType(ref arg0, ref arg1))
				//if (arg0.Type != arg1.Type)
				{
					if (arg0.Type != typeof(object))
					{
						arg0 = Expression.Convert(arg0, typeof(object));
					}
					if (arg1.Type != typeof(object))
					{
						arg1 = Expression.Convert(arg1, typeof(object));
					}
				}
				e.Result = Expression.Condition(
					isTrue0,
					arg0,
					Expression.Condition(
						isTrue1,
						arg1,
						Expression.Default(arg0.Type)
					)
				);
			}
			//if (e.Args.Count == 2)
			//{
			//	var arg0 = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			//	var arg1 = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			//	if (arg0.Type != typeof(bool))
			//	{
			//		arg0 = Expression.Convert(arg0, typeof(bool));
			//	}
			//	if (arg1.Type != typeof(bool))
			//	{
			//		arg1 = Expression.Convert(arg1, typeof(bool));
			//	}
			//	e.Result = Expression.OrElse(arg0, arg1);
			//}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				if (arg0 is bool b0)
				{
					if (b0)
					{
						e.SetResult(true);
						return;
					}
				}
				else if (e.Context.IsTrue(arg0))
				{
					e.SetResult(arg0);
					return;
				}
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				if (arg1 is bool b1)
				{
					e.SetResult(b1);
					return;
				}
				else if (e.Context.IsTrue(arg1))
				{
					e.SetResult(arg1);
					return;
				}
				e.SetResult(null);
			}
			//if (e.Args.Count == 2)
			//{
			//	var arg0 = (bool)e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
			//	if (arg0)
			//	{
			//		e.SetResult(arg0);
			//		return;
			//	}
			//	var arg1 = (bool)e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
			//	if (arg1)
			//	{
			//		e.SetResult(arg1);
			//		return;
			//	}
			//	e.SetResult(arg0);
			//}
		}
	}
}
