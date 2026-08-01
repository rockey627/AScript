using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Operators
{
	public class DivideOperator : IFunctionEvaluator, IFunctionBuilder
	{
		private static readonly MethodInfo Method_Div = typeof(DivideOperator).GetMethod("Div", BindingFlags.Static | BindingFlags.NonPublic);

		public static readonly DivideOperator Instance = new DivideOperator();

		/// <summary>
		/// 是否转浮点型
		/// </summary>
		private readonly bool _Double;

		public DivideOperator() { }
		public DivideOperator(bool isDouble)
		{
			_Double = isDouble;
		}

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (ScriptUtils.IsNumberType(left.Type) && ScriptUtils.IsNumberType(right.Type))
			{
				if (_Double)
				{
					if (left.Type != typeof(double)) left = Expression.Convert(left, typeof(double));
					if (right.Type != typeof(double)) right = Expression.Convert(right, typeof(double));
				}
				if (left.Type == typeof(object) || right.Type == typeof(object)
						|| !ExpressionUtils.ConvertMaxType(ref left, ref right))
				{
					e.Result = Expression.Dynamic(ExpressionUtils.Binder_Divide, typeof(object), left, right);
				}
				else
				{
					e.Result = Expression.Divide(left, right);
				}
			}
			else
			{
				e.Result = Expression.Call(Method_Div, left, right, Expression.Constant(_Double));
			}
		}

		private static object Div(object v1, object v2, bool isDouble)
		{
			if (isDouble)
			{
				if (v1 != null && ScriptUtils.IsNumberType(v1.GetType())
						&& v2 != null && ScriptUtils.IsNumberType(v2.GetType()))
				{
					double d1 = Convert.ToDouble(v1);
					double d2 = Convert.ToDouble(v2);
					return d1 / d2;
				}
			}
			dynamic arg1 = v1;
			dynamic arg2 = v2;
			return arg1 / arg2;
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0Obj = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var arg1Obj = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(Div(arg0Obj, arg1Obj, _Double));
				//if (_Double)
				//{
				//	var arg0Obj = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				//	var arg1Obj = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				//	if (arg0Obj != null && ScriptUtils.IsNumberType(arg0Obj.GetType())
				//		&& arg1Obj != null && ScriptUtils.IsNumberType(arg1Obj.GetType()))
				//	{
				//		double arg0 = Convert.ToDouble(arg0Obj);
				//		double arg1 = Convert.ToDouble(arg1Obj);
				//		e.SetResult(arg0 / arg1);
				//	}
				//	else
				//	{
				//		// 其他对象可能有运算符重载
				//		dynamic arg0 = arg0Obj;
				//		dynamic arg1 = arg1Obj;
				//		e.SetResult(arg0 / arg1);
				//	}
				//}
				//else
				//{
				//	dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				//	dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				//	e.SetResult(arg0 / arg1);
				//}
			}
		}
	}
}
