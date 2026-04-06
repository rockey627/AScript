using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AScript.Operators
{
	/// <summary>
	/// ? :
	/// </summary>
	public class QuestionIIFOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly QuestionIIFOperator Instance = new QuestionIIFOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;

			var arg1 = e.Args[1];
			if (arg1 is TreeBuilder treeBuilder)
			{
				arg1 = treeBuilder.Root;
			}
			if (arg1 is OperatorNode opNode && opNode.Name == ":")
			{
				var b = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var ifTrue = opNode.Left.Build(e.BuildContext, e.ScriptContext, e.Options);
				var ifFalse = opNode.Right.Build(e.BuildContext, e.ScriptContext, e.Options);
				e.Result = Expression.Condition(b, ifTrue, ifFalse);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;

			var arg1 = e.Args[1];
			if (arg1 is TreeBuilder treeBuilder)
			{
				arg1 = treeBuilder.Root;
			}
			if (arg1 is OperatorNode opNode && opNode.Name == ":")
			{
				bool b = (bool)e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				if (b)
				{
					e.SetResult(opNode.Left.Eval(e.Context, e.Options, e.Control, out var type), type);
				}
				else
				{
					e.SetResult(opNode.Right.Eval(e.Context, e.Options, e.Control, out var type), type);
				}
			}
		}
	}
}
