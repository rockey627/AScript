using AScript.Functions;
using AScript.Operators;
using System;
using System.Linq.Expressions;

namespace AScript.Lang.Lua.Operators
{
	/// <summary>
	/// Lua字符串连接运算符 ..
	/// </summary>
	public class LuaConcatOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly LuaConcatOperator Instance = new LuaConcatOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			var concatMethod = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });
			e.Result = Expression.Call(concatMethod, left, right);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(string.Concat(ToString(arg0), ToString(arg1)));
			}
		}

		private static string ToString(object obj)
		{
			return obj?.ToString() ?? "";
		}
	}
}
