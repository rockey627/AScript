using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Lang.Python3.Operators
{
	public class Python3Divide2AssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly Python3Divide2AssignOperator Instance = new Python3Divide2AssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args[0] is VariableNode leftVar)
			{
				var left = leftVar.BuildForAssign(e.BuildContext, e.ScriptContext, e.Options, out _, out var lastType);
				if (left == null)
				{
					throw new Exception($"invalid expression: {leftVar.Name} is not exists");
				}
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);

				var leftType = lastType ?? left.Type;

				Expression leftExpr, rightExpr;
				// 
				if (leftType == typeof(object)) leftExpr = Expression.Call(ExpressionUtils.Method_ScriptUtils_Convert, left, ExpressionUtils.Constant_typeof_double);
				else if (left.Type == typeof(double)) leftExpr = left;
				else if (lastType == null || left.Type == lastType) leftExpr = Expression.Convert(left, typeof(double));
				else leftExpr = Expression.Convert(Expression.Convert(left, lastType), typeof(double));
				// 
				if (right.Type == typeof(object)) rightExpr = Expression.Call(ExpressionUtils.Method_ScriptUtils_Convert, right, ExpressionUtils.Constant_typeof_double);
				else if (right.Type == typeof(double)) rightExpr = right;
				else rightExpr = Expression.Convert(right, typeof(double));

				//var d1 = Expression.Convert(left, typeof(double));
				//var d2 = Expression.Convert(right, typeof(double));
				var d = Expression.Divide(leftExpr, rightExpr);
				var r = Expression.Call(ExpressionUtils.Method_Math_Floor, d);
				if (ScriptUtils.IsIntegerType(leftType) && ScriptUtils.IsIntegerType(right.Type))
				{
					var maxType = ScriptUtils.GetMaxType(leftType, right.Type);
					var r2 = Expression.Convert(r, maxType);
					e.Result = Expression.Assign(left, Expression.Convert(r2, left.Type));
				}
				else
				{
					e.Result = Expression.Assign(left, Expression.Convert(r, left.Type));
				}
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2 && e.Args[0] is VariableNode varNode)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type1);
				double r = Math.Floor(Convert.ToDouble(arg0) / Convert.ToDouble(arg1));
				if (ScriptUtils.IsIntegerType(type0) && ScriptUtils.IsIntegerType(type1))
				{
					var maxType = ScriptUtils.GetMaxType(type0, type1);
					var r2 = ScriptUtils.Convert(r, maxType);
					e.SetResult(ScriptUtils.Convert(r2, type0), type0);
				}
				else
				{
					e.SetResult(r);
				}
				e.Context.SetTempVar(varNode.Name, e.Result, true);
			}
		}
	}
}
