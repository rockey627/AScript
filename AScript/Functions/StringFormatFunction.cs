using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	/// <summary>
	/// "my name is {0}, I'm {1} years old.".format('tom', 20)
	/// </summary>
	public class StringFormatFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly StringFormatFunction Instance = new StringFormatFunction();

#if NETSTANDARD
		private static readonly Expression Constant_Array_Object_Empty = Expression.Constant(Array.Empty<object>());
#else
		private static readonly Expression Constant_Array_Object_Empty = Expression.Constant(new object[0]);
#endif
		private static readonly MethodInfo Method_String_Format_Array = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object[]) });
		private static readonly MethodInfo Method_String_Format_Object1 = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object) });
		private static readonly MethodInfo Method_String_Format_Object2 = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object), typeof(object) });
		private static readonly MethodInfo Method_String_Format_Object3 = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object), typeof(object), typeof(object) });

		public void Build(FunctionBuildArgs e)
		{
			int argsCount = e.GetArgsCount();
			var s = e.BuildArgs(0);
			if (argsCount <= 1)
			{
				e.Result = Expression.Call(Method_String_Format_Array, s, Constant_Array_Object_Empty);
				return;
			}
			var v1 = e.BuildArgs(1);
			if (argsCount <= 2)
			{
				e.Result = Expression.Call(Method_String_Format_Object1, s, v1);
				return;
			}
			var v2 = e.BuildArgs(2);
			if (argsCount <= 3)
			{
				e.Result = Expression.Call(Method_String_Format_Object2, s, v1, v2);
				return;
			}
			var v3 = e.BuildArgs(3);
			if (argsCount <= 4)
			{
				e.Result = Expression.Call(Method_String_Format_Object3, s, v1, v2, v3);
				return;
			}
			var args = new Expression[argsCount - 1];
			args[0] = v1;
			args[1] = v2;
			args[2] = v3;
			for (int i = 4; i < argsCount; i++)
			{
				args[i - 1] = e.BuildArgs(i);
			}
			e.Result = Expression.Call(Method_String_Format_Array, s, Expression.NewArrayInit(typeof(object), args));
		}

		public void Eval(FunctionEvalArgs e)
		{
			int argsCount = e.Args.Count;
			var s = e.Args[0].Eval(e.Context, e.Options, e.Control, out _)?.ToString();
			if (string.IsNullOrEmpty(s) || argsCount <= 1)
			{
				e.SetResult(s);
				return;
			}
			var v1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
			if (argsCount <= 2)
			{
				e.SetResult(string.Format(s, v1));
				return;
			}
			var v2 = e.Args[2].Eval(e.Context, e.Options, e.Control, out _);
			if (argsCount <= 3)
			{
				e.SetResult(string.Format(s, v1, v2));
				return;
			}
			var v3 = e.Args[3].Eval(e.Context, e.Options, e.Control, out _);
			if (argsCount <= 4)
			{
				e.SetResult(string.Format(s, v1, v2, v3));
				return;
			}
			var args = new object[argsCount - 1];
			args[0] = v1;
			args[1] = v2;
			args[2] = v3;
			for (int i = 4; i < argsCount; i++)
			{
				args[i - 1] = e.Args[i].Eval(e.Context, e.Options, e.Control, out _);
			}
			e.SetResult(string.Format(s, args));
		}
	}
}
