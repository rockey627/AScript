using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Lang.Python3.Operators
{
	public class Python3DivideAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly Python3DivideAssignOperator Instance = new Python3DivideAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args[0] is VariableNode leftVar)
			{
				if (!e.BuildContext.TryGetVariableOrParameter(leftVar.Name, out var left, out _, out _, out var lastType))
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
				// 
				var r = Expression.Divide(leftExpr, rightExpr);
				e.Result = Expression.Assign(left, Expression.Convert(r, left.Type));
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2 && e.Args[0] is VariableNode varNode)
			{
				double arg0 = Convert.ToDouble(e.Args[0].Eval(e.Context, e.Options, e.Control, out _));
				double arg1 = Convert.ToDouble(e.Args[1].Eval(e.Context, e.Options, e.Control, out _));
				e.SetResult(arg0 / arg1);
				e.Context.SetTempVar(varNode.Name, e.Result, true);
			}
		}
	}
}
