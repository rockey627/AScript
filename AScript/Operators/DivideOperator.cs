using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class DivideOperator : IFunctionEvaluator, IFunctionBuilder
	{
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
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
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

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				if (_Double)
				{
					double arg0 = Convert.ToDouble(e.Args[0].Eval(e.Context, e.Options, e.Control, out _));
					double arg1 = Convert.ToDouble(e.Args[1].Eval(e.Context, e.Options, e.Control, out _));
					e.SetResult(arg0 / arg1);
				}
				else
				{
					dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
					dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
					e.SetResult(arg0 / arg1);
				}
			}
		}
	}
}
