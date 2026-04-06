using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class QuestionQuestionOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly QuestionQuestionOperator Instance = new QuestionQuestionOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;

			var b = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var condition = Expression.Equal(b, Expression.Constant(null, b.Type));
			var ifTrue = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			e.Result = Expression.Condition(condition, ifTrue, b);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				e.SetResult(arg0 ?? e.Args[1].Eval(e.Context, e.Options, e.Control, out _), type0);
			}
		}
	}
}
