using System;
using System.Linq.Expressions;
using AScript.Nodes;

namespace AScript.Operators
{
	public class AndAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly AndAssignOperator Instance = new AndAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args[0] is VariableNode leftVar)
			{
				if (!e.BuildContext.TryGetVariableOrParameter(leftVar.Name, out var left))
				{
					throw new Exception($"invalid expression: {leftVar.Name} is not exists");
				}
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (left.Type == typeof(object) || right.Type == typeof(object))
				{
					// dynamic方式作用+=无效
					//e.Result = Expression.Dynamic(ExpressionUtils.Binder_AndAssign, typeof(object), left, right);
					var addExpr = Expression.Dynamic(ExpressionUtils.Binder_And, typeof(object), left, right);
					e.Result = Expression.Assign(left, addExpr);
				}
				else
				{
					e.Result = Expression.AndAssign(left, right);
				}
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2 && e.Args[0] is VariableNode)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				arg0 &= arg1;
				e.SetResult(arg0, type0);
				e.Context.SetTempVar(((VariableNode)e.Args[0]).Name, e.Result, true);
			}
		}
	}
}
