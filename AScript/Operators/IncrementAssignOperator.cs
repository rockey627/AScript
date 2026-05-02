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
			if (e.Args.Count != 1) return;

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
			if (e.IsPrefix)
			{
				e.Result = Expression.PreIncrementAssign(left);
			}
			else
			{
				e.Result = Expression.PostIncrementAssign(left);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 1) return;
			var arg0Node = e.Args[0];
			if (arg0Node is VariableNode varNode)
			{
				dynamic arg0 = varNode.Eval(e.Context, e.Options, e.Control, out var type0);
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
					e.Context.SetTempVar(varNode.Name, arg0, true);
				}
			}
			else if (arg0Node is OperatorNode opNode && opNode.Name == "." && opNode.Right is VariableNode opRightNode)
			{
				// 属性赋值
				var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
				var value = ScriptUtils.GetAndSetValue(opLeftValue, opRightNode.Name, out var type0, (t, v) =>
				{
					dynamic d = v;
					if (e.IsPrefix)
					{
						e.SetResult(++d);
					}
					else
					{
						e.SetResult(d++);
					}
					return d;
				});
			}
		}
	}
}
