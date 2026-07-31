using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Operators
{
	public class SubtractOperator : IFunctionEvaluator, IFunctionBuilder
	{
		private static readonly MethodInfo Method_Negate = typeof(SubtractOperator).GetMethod("Negate", BindingFlags.Static | BindingFlags.NonPublic);

		public static readonly SubtractOperator Instance = new SubtractOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count == 1)
			{
				var arg = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (arg.Type == typeof(object))
				{
					e.Result = Expression.Call(Method_Negate, arg);
				}
				else
				{
					e.Result = Expression.Negate(arg);
				}
			}
			else
			{
				var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (left.Type == typeof(object) || right.Type == typeof(object)
					|| !ExpressionUtils.ConvertMaxType(ref left, ref right))
				{
					e.Result = Expression.Dynamic(ExpressionUtils.Binder_Subtract, typeof(object), left, right);
				}
				else
				{
					e.Result = Expression.Subtract(left, right);
				}
			}
		}

		private static object Negate(object obj)
		{
			dynamic d = obj;
			return -d;
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(arg0 - arg1);
			}
			else if (e.Args.Count == 1)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(-arg0);
			}
		}
	}
}
