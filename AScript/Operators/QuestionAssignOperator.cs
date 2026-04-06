using System;
using System.Linq.Expressions;
using AScript.Nodes;

namespace AScript.Operators
{
	public class QuestionAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly QuestionAssignOperator Instance = new QuestionAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;

			var b = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var condition = Expression.Equal(b, Expression.Constant(null, b.Type));
			var ifTrue = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			e.Result = Expression.Assign(b, Expression.Condition(condition, ifTrue, b));
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2 && e.Args[0] is VariableNode)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				e.SetResult(arg0 ?? e.Args[1].Eval(e.Context, e.Options, e.Control, out _), type0);
				e.Context.SetTempVar(((VariableNode)e.Args[0]).Name, e.Result, true);
			}
		}
	}
}
