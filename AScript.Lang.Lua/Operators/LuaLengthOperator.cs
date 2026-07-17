using AScript.Functions;
using AScript.Operators;
using System;
using System.Collections;
using System.Linq.Expressions;

namespace AScript.Lang.Lua.Operators
{
	/// <summary>
	/// Lua长度运算符 #
	/// </summary>
	public class LuaLengthOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly LuaLengthOperator Instance = new LuaLengthOperator();

		public void Build(FunctionBuildArgs e)
		{
			var arg = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (arg.Type == typeof(string))
			{
				var lengthProperty = typeof(string).GetProperty("Length");
				e.Result = Expression.Property(arg, lengthProperty);
			}
			else if (typeof(ICollection).IsAssignableFrom(arg.Type))
			{
				var countProperty = arg.Type.GetProperty("Count");
				if (countProperty != null)
				{
					e.Result = Expression.Property(arg, countProperty);
				}
				else
				{
					e.Result = Expression.Constant(0L);
				}
			}
			else
			{
				e.Result = Expression.Constant(0L);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 1)
			{
				var arg = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				if (arg == null)
				{
					e.SetResult(0L);
					return;
				}
				if (arg is string s)
				{
					e.SetResult((long)s.Length);
					return;
				}
				if (arg is ICollection coll)
				{
					e.SetResult((long)coll.Count);
					return;
				}
				e.SetResult(0L);
			}
		}
	}
}
