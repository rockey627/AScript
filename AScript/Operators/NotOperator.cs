using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Operators
{
	/// <summary>
	/// 位运算：非~
	/// </summary>
	public class NotOperator : IFunctionEvaluator, IFunctionBuilder
	{
		private static readonly MethodInfo Method_Not = typeof(NotOperator).GetMethod("Not", BindingFlags.Static | BindingFlags.NonPublic);

		public static readonly NotOperator Instance = new NotOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count == 1)
			{
				var arg = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (arg.Type == typeof(object))
				{
					//e.Result = Expression.Dynamic(ExpressionUtils.Binder_Not, typeof(object), arg);
					e.Result = Expression.Call(Method_Not, arg);
				}
				else
				{
					e.Result = Expression.Not(arg);
				}
			}
		}

		private static object Not(object obj)
		{
			dynamic d = obj;
			return ~d;
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
