using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Python3.Operators
{
	public class Python3Divide2Operator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly Python3Divide2Operator Instance = new Python3Divide2Operator();

		private static readonly MethodInfo Method_Math_Floor = typeof(Math).GetMethod("Floor", new[] { typeof(double) });

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			var d1 = Expression.Convert(left, typeof(double));
			var d2 = Expression.Convert(right, typeof(double));
			var d = Expression.Divide(d1, d2);
			var r = Expression.Call(Method_Math_Floor, d);
			if (ScriptUtils.IsIntegerType(left.Type) && ScriptUtils.IsIntegerType(right.Type))
			{
				var maxType = ScriptUtils.GetMaxType(left.Type, right.Type);
				e.Result = Expression.Convert(r, maxType);
			}
			else
			{
				e.Result = r;
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				var type0 = arg0.GetType();
				var type1 = arg1.GetType();
				double r = Math.Floor(Convert.ToDouble(arg0) / Convert.ToDouble(arg1));
				if (ScriptUtils.IsIntegerType(type0) && ScriptUtils.IsIntegerType(type1))
				{
					var maxType = ScriptUtils.GetMaxType(type0, type1);
					if (maxType == typeof(long))
					{
						e.SetResult(Convert.ToInt64(r));
					}
					else if (maxType == typeof(ulong))
					{
						e.SetResult(Convert.ToUInt64(r));
					}
					else
					{
						e.SetResult(Convert.ToInt32(r));
					}
				}
				else
				{
					e.SetResult(r);
				}
			}
		}
	}
}
