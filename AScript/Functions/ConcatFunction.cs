using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Functions
{
	public class ConcatFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly ConcatFunction Instance = new ConcatFunction();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args == null || e.Args.Count == 0) return;
			var args = e.BuildArgs();
			var t0 = args[0].Type;
			if (t0 == typeof(object) || t0 == typeof(string))
			{
				e.Result = ConcatString(args);
			}
			else if (t0.IsArray)
			{

			}
			else if (typeof(ICollection).IsAssignableFrom(t0))
			{

			}
		}

		private Expression ConcatString(IList<Expression> args)
		{
			if (args.Count == 1)
			{
				var arg0 = args[0];
				if (arg0.Type == typeof(string))
				{
					return arg0;
				}
				else
				{
					if (arg0.Type != typeof(object)) arg0 = Expression.Convert(arg0, typeof(object));
					return Expression.Call(ExpressionUtils.Method_String_Concat_object, arg0);
				}
			}
			else if (args.Count == 2)
			{
				var arg0 = args[0];
				var arg1 = args[1];
				if (arg0.Type == typeof(string) && arg1.Type == typeof(string))
				{
					return Expression.Call(ExpressionUtils.Method_String_Concat2, arg0, arg1);
				}
				else
				{
					if (arg0.Type != typeof(object)) arg0 = Expression.Convert(arg0, typeof(object));
					if (arg1.Type != typeof(object)) arg1 = Expression.Convert(arg1, typeof(object));
					return Expression.Call(ExpressionUtils.Method_String_Concat2_object, arg0, arg1);
				}
			}
			else if (args.Count == 3)
			{
				var arg0 = args[0];
				var arg1 = args[1];
				var arg2 = args[2];
				if (arg0.Type == typeof(string) && arg1.Type == typeof(string) && arg2.Type == typeof(string))
				{
					return Expression.Call(ExpressionUtils.Method_String_Concat3, arg0, arg1, arg2);
				}
				else
				{
					if (arg0.Type != typeof(object)) arg0 = Expression.Convert(arg0, typeof(object));
					if (arg1.Type != typeof(object)) arg1 = Expression.Convert(arg1, typeof(object));
					if (arg2.Type != typeof(object)) arg2 = Expression.Convert(arg2, typeof(object));
					return Expression.Call(ExpressionUtils.Method_String_Concat3_object, arg0, arg1, arg2);
				}
			}
			else if (args.Count == 4)
			{
				var arg0 = args[0];
				var arg1 = args[1];
				var arg2 = args[2];
				var arg3 = args[3];
				if (arg0.Type == typeof(string) && arg1.Type == typeof(string) && arg2.Type == typeof(string) && arg3.Type == typeof(string))
				{
					return Expression.Call(ExpressionUtils.Method_String_Concat4, arg0, arg1, arg2, arg3);
				}
				else
				{
					if (arg0.Type != typeof(object)) arg0 = Expression.Convert(arg0, typeof(object));
					if (arg1.Type != typeof(object)) arg1 = Expression.Convert(arg1, typeof(object));
					if (arg2.Type != typeof(object)) arg2 = Expression.Convert(arg2, typeof(object));
					if (arg3.Type != typeof(object)) arg3 = Expression.Convert(arg3, typeof(object));
					return Expression.Call(ExpressionUtils.Method_String_Concat4_object, arg0, arg1, arg2, arg3);
				}
			}
			else if (args.All(a => a.Type == typeof(string)))
			{
				var arr = args is Expression[] argsArr ? Expression.NewArrayInit(typeof(string), argsArr) : Expression.NewArrayInit(typeof(string), args);
				return Expression.Call(ExpressionUtils.Method_String_Concat_array, arr);
			}
			else
			{
				var argsArr = new Expression[args.Count];
				for (int i = 0; i < args.Count; i++)
				{
					var arg = args[i];
					if (arg.Type != typeof(object)) arg = Expression.Convert(arg, typeof(object));
					argsArr[i] = arg;
				}
				var arr = Expression.NewArrayInit(typeof(object), argsArr);
				return Expression.Call(ExpressionUtils.Method_String_Concat_array_object, arr);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args == null || e.Args.Count == 0) return;
			e.EvalArgs();
			var t0 = e.ArgTypes[0];
			if (t0 == typeof(string) || t0 == typeof(object))
			{
				e.SetResult(string.Concat(e.ArgValues));
			}
			else if (t0.IsArray)
			{

			}
			else if (typeof(ICollection).IsAssignableFrom(t0))
			{
				
			}
		}
	}
}
