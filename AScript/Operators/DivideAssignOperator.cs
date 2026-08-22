using System;
using System.Linq.Expressions;
using AScript.Exceptions;
using AScript.Nodes;

namespace AScript.Operators
{
	public class DivideAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly DivideAssignOperator Instance = new DivideAssignOperator();

		/// <summary>
		/// 是否转浮点型
		/// </summary>
		private readonly bool _Double;

		public DivideAssignOperator() { }
		public DivideAssignOperator(bool isDouble)
		{
			_Double = isDouble;
		}

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0 = e.Args[0];
			Expression left;
			if (arg0 is VariableNode leftVar)
			{
				left = leftVar.BuildForAssign(e.BuildContext, e.ScriptContext, e.Options, out _, out var lastType);
				if (left == null)
				{
					throw new ScriptAnalyzingException($"invalid expression: {leftVar.Name} is not exists");
				}
			}
			else
			{
				left = arg0.Build(e.BuildContext, e.ScriptContext, e.Options);
			}
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			var left2 = left;
			if (_Double)
			{
				if (left2.Type != typeof(double)) left2 = Expression.Convert(left2, typeof(double));
				if (right.Type != typeof(double)) right = Expression.Convert(right, typeof(double));
			}
			if (left2.Type == typeof(object) || right.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref left2, ref right))
			{
				// dynamic方式作用/=无效
				//e.Result = Expression.Dynamic(ExpressionUtils.Binder_AddAssign, typeof(object), left, right);
				//var expr = Expression.Dynamic(ExpressionUtils.Binder_Divide, typeof(object), left2, right);
				//var expr = ExpressionUtils.Divide(left2, right, _Double);
				if (left2.Type.IsValueType) left2 = Expression.Convert(left2, typeof(object));
				if (right.Type.IsValueType) right = Expression.Convert(right, typeof(object));
				var expr = Expression.Call(ExpressionUtils.Method_Divide, left2, right, Expression.Constant(_Double));
				//e.Result = Expression.Assign(left, expr);
				if (expr.Type != left.Type)
				{
					e.Result = Expression.Assign(left, Expression.Convert(expr, left.Type));
				}
				else
				{
					e.Result = Expression.Assign(left, expr);
				}
			}
			else
			{
				if (_Double && left.Type != typeof(object) && left.Type != typeof(double))
				{
					var result = Expression.Divide(left2, right);
					var convert = Expression.Convert(result, left.Type);
					e.Result = Expression.Assign(left, convert);
				}
				else
				{
					e.Result = Expression.DivideAssign(left, right);
				}
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0Node = e.Args[0];
			if (arg0Node is VariableNode varNode)
			{
				if (_Double)
				{
					double arg0 = Convert.ToDouble(varNode.Eval(e.Context, e.Options, e.Control, out _));
					double arg1 = Convert.ToDouble(e.Args[1].Eval(e.Context, e.Options, e.Control, out _));
					arg0 /= arg1;
					e.SetResult(arg0);
				}
				else
				{
					dynamic arg0 = varNode.Eval(e.Context, e.Options, e.Control, out var type0);
					dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
					arg0 /= arg1;
					e.SetResult(arg0, type0);
				}
				e.Context.SetTempVar(varNode.Name, e.Result, true);
			}
			else if (arg0Node is OperatorNode opNode)
			{
				if (opNode.Name == "." && opNode.Right is VariableNode opRightNode)
				{
					// 属性赋值
					var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type1);
					var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					var value = ScriptUtils.GetAndSetValue(opLeftValue, opRightNode.Name, out var type0, (m, t, v) =>
					{
						if (_Double) return Convert.ToDouble(v) / Convert.ToDouble(arg1);
						return (dynamic)v / (dynamic)arg1;
					});
					e.SetResult(value, type0 == typeof(object) ? type1 : type0);
					return;
				}

				if (opNode.Name == "[]")
				{
					// 设置索引值
					var obj = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					var idx = opNode.Right.Eval(e.Context, e.Options, e.Control, out _);
					var value = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);

					//// 根据obj类型处理索引器赋值
					var v = ScriptUtils.GetAndSetValue(obj, idx, v1 =>
					{
						if (_Double) return Convert.ToDouble(v1) / Convert.ToDouble(value);
						return (dynamic)v1 / (dynamic)value;
					});

					e.SetResult(v);
					return;
				}
			}
		}
	}
}
