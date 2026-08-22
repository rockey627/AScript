using System;

namespace AScript.Operators
{
	public class PlusOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PlusOperator Instance = new PlusOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count == 1)
			{
				var arg = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				e.Result = arg;
			}
			else if (e.Args.Count == 2)
			{
				var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				e.Result = ExpressionUtils.Add(left, right);
				//if (!ExpressionUtils.ConvertMaxType(ref left, ref right))
				//{
				//	//e.Result = Expression.Dynamic(ExpressionUtils.Binder_Add, typeof(object), left, right);
				//	e.Result = Expression.Call(ScriptUtils.Method_ScriptUtils_Add, Expression.Convert(left, typeof(object)), Expression.Convert(right, typeof(object)));
				//}
				//else if (left.Type == typeof(string) && right.Type == typeof(string))
				//{
				//	// 字符串相加使用string.Concat方法
				//	e.Result = Expression.Call(null, ExpressionUtils.Method_String_Concat2, left, right);
				//}
				//else
				//{
				//	e.Result = Expression.Add(left, right);
				//}
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type1);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type2);
				e.SetResult(ExpressionUtils.Add(arg0, arg1));
				//if (arg0 == null)
				//{
				//	arg0 = ScriptUtils.GetDefaultValue(type1 ?? type2);
				//}
				//if (arg1 == null)
				//{
				//	arg1 = ScriptUtils.GetDefaultValue(type2 ?? type1);
				//}
				//e.SetResult(arg0 + arg1);
			}
			else if (e.Args.Count == 1)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				e.SetResult(+arg0);
			}
		}
	}
}
