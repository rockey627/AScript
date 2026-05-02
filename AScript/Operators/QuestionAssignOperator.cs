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

			var arg0 = e.Args[0];
			Expression left;
			if (arg0 is VariableNode leftVar)
			{
				left = leftVar.BuildForAssign(e.BuildContext, e.ScriptContext, e.Options, out _, out var lastType);
				if (left == null)
				{
					throw new Exception($"invalid expression: {leftVar.Name} is not exists");
				}
			}
			else
			{
				left = arg0.Build(e.BuildContext, e.ScriptContext, e.Options);
			}
			var condition = Expression.Equal(left, Expression.Constant(null, left.Type));
			var ifTrue = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			e.Result = Expression.Assign(left, Expression.Condition(condition, ifTrue, left));
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0Node = e.Args[0];
			if (arg0Node is VariableNode varNode)
			{
				var arg0 = varNode.Eval(e.Context, e.Options, e.Control, out var type0);
				e.SetResult(arg0 ?? e.Args[1].Eval(e.Context, e.Options, e.Control, out _), type0);
				e.Context.SetTempVar(varNode.Name, e.Result, true);
			}
			else if (arg0Node is OperatorNode opNode && opNode.Name == "." && opNode.Right is VariableNode opRightNode)
			{
				// 属性赋值
				//var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type1);
				Type type1 = null;
				var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
				var value = ScriptUtils.GetAndSetValue(opLeftValue, opRightNode.Name, out var type0, (t, v) => v ?? e.Args[1].Eval(e.Context, e.Options, e.Control, out type1));
				e.SetResult(value, type0 == typeof(object) ? type1 : type0);
			}
		}
	}
}
