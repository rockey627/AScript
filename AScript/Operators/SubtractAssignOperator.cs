using System;
using System.Linq.Expressions;
using AScript.Nodes;

namespace AScript.Operators
{
	public class SubtractAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
        public static readonly SubtractAssignOperator Instance = new SubtractAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (left.Type == typeof(object) || right.Type == typeof(object))
			{
				// dynamic方式作用-=无效
				//e.Result = Expression.Dynamic(ExpressionUtils.Binder_SubtractAssign, typeof(object), left, right);
				var expr = Expression.Dynamic(ExpressionUtils.Binder_Subtract, typeof(object), left, right);
				e.Result = Expression.Assign(left, expr);
			}
			else
			{
				e.Result = Expression.SubtractAssign(left, right);
			}
		}

		public void Eval(FunctionEvalArgs e)
        {
            if (e.Args.Count == 2 && e.Args[0] is VariableNode)
            {
                dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
                dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
                arg0 -= arg1;
                e.SetResult(arg0, type0);
                e.Context.SetTempVar(((VariableNode)e.Args[0]).Name, e.Result, true);
            }
        }
    }
}
