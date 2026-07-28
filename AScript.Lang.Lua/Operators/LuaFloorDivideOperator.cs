using AScript.Functions;
using AScript.Operators;
using System;
using System.Linq.Expressions;

namespace AScript.Lang.Lua.Operators
{
	/// <summary>
	/// Lua整数除法运算符 //
	/// </summary>
	public class LuaFloorDivideOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly LuaFloorDivideOperator Instance = new LuaFloorDivideOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (left.Type != typeof(double)) left = Expression.Convert(left, typeof(double));
			if (right.Type != typeof(double)) right = Expression.Convert(right, typeof(double));
			var floorMethod = ExpressionUtils.Method_Math_Floor;// typeof(Math).GetMethod("Floor", new[] { typeof(double) });
			var floor = Expression.Call(floorMethod, Expression.Divide(left, right));
			e.Result = Expression.Convert(floor, typeof(long));
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				double arg0 = Convert.ToDouble(e.Args[0].Eval(e.Context, e.Options, e.Control, out _));
				double arg1 = Convert.ToDouble(e.Args[1].Eval(e.Context, e.Options, e.Control, out _));
				e.SetResult((long)Math.Floor(arg0 / arg1));
			}
		}
	}
}
