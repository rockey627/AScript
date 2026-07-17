using AScript.Functions;
using AScript.Operators;
using System;
using System.Linq.Expressions;

namespace AScript.Lang.Lua.Operators
{
	/// <summary>
	/// Lua一元取负运算符 -
	/// </summary>
	public class LuaUnaryMinusOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly LuaUnaryMinusOperator Instance = new LuaUnaryMinusOperator();

		public void Build(FunctionBuildArgs e)
		{
			var arg = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (arg.Type == typeof(long))
			{
				e.Result = Expression.Negate(arg);
			}
			else
			{
				if (arg.Type != typeof(double)) arg = Expression.Convert(arg, typeof(double));
				e.Result = Expression.Negate(arg);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 1)
			{
				var arg = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				if (arg is long l) e.SetResult(-l);
				else if (arg is double d) e.SetResult(-d);
				else if (arg is int i) e.SetResult(-i);
				else if (arg is float f) e.SetResult(-f);
				else if (arg is decimal dec) e.SetResult(-dec);
				else e.SetResult(0);
			}
		}
	}
}
