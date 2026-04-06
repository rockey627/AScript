using System;
using System.Linq.Expressions;
using AScript.Nodes;

namespace AScript.Operators
{
	/// <summary>
	/// ++
	/// </summary>
	public class IncrementAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly IncrementAssignOperator Instance = new IncrementAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count == 1 && e.Args[0] is VariableNode)
			{
				var varExpr = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (e.IsPrefix)
				{
					e.Result = Expression.PreIncrementAssign(varExpr);
				}
				else
				{
					e.Result = Expression.PostIncrementAssign(varExpr);
				}
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 1 && e.Args[0] is VariableNode)
			{
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				if (ScriptUtils.IsNumberType(type0))
				{
					if (e.IsPrefix)
					{
						e.SetResult(++arg0);
					}
					else
					{
						e.SetResult(arg0++);
					}
					e.Context.SetTempVar(((VariableNode)e.Args[0]).Name, arg0, true);
				}
			}
		}
	}
}
