using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class PowerAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PowerAssignOperator Instance = new PowerAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			var v1 = Expression.Convert(left, typeof(double));
			var v2 = Expression.Convert(right, typeof(double));
			var v = Expression.Call(ExpressionUtils.Method_Math_Power, v1, v2);
			e.Result = Expression.Assign(left, Expression.Convert(v, left.Type));
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2 && e.Args[0] is VariableNode varNode)
			{
				var v1 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				var v2 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				var r = Math.Pow(Convert.ToDouble(v1), Convert.ToDouble(v2));
				e.SetResult(ScriptUtils.Convert(r, type0));
				e.Context.SetTempVar(varNode.Name, e.Result, true);
			}
		}
	}
}
