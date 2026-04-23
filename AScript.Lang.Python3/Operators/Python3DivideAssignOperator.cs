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
				if (!e.BuildContext.TryGetVariableOrParameter(leftVar.Name, out var left))
				{
					throw new Exception($"invalid expression: {leftVar.Name} is not exists");
				}
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				var r = Expression.Divide(Expression.Convert(left, typeof(double)), Expression.Convert(right, typeof(double)));
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
