using System;
using System.Linq.Expressions;
using AScript.Nodes;

namespace AScript.Operators
{
	/// <summary>
	/// 异或^
	/// </summary>
	public class XOrAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly XOrAssignOperator Instance = new XOrAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (left.Type == typeof(object) || right.Type == typeof(object))
			{
				// dynamic方式作用+=无效
				//e.Result = Expression.Dynamic(ExpressionUtils.Binder_AndAssign, typeof(object), left, right);
				var addExpr = Expression.Dynamic(ExpressionUtils.Binder_XOr, typeof(object), left, right);
				e.Result = Expression.Assign(left, addExpr);
			}
			else
			{
				e.Result = Expression.ExclusiveOrAssign(left, right);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2 && e.Args[0] is VariableNode)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				arg0 ^= arg1;
				e.SetResult(arg0, type0);
				e.Context.SetTempVar(((VariableNode)e.Args[0]).Name, e.Result, true);
			}
		}
	}
}
