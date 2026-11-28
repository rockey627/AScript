using System;
using System.Linq.Expressions;
using AScript.Exceptions;
using AScript.Nodes;

namespace AScript.Operators
{
	/// <summary>
	/// --
	/// </summary>
	public class DecrementAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly DecrementAssignOperator Instance = new DecrementAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 1) return;

			var arg0 = e.Args[0];
			Expression left;
			Type lastType = null;
			if (arg0 is VariableNode leftVar)
			{
				left = leftVar.BuildForAssign(e.BuildContext, e.ScriptContext, e.Options, out _, out lastType);
				if (left == null)
				{
					throw new ScriptAnalyzingException($"invalid expression: {leftVar.Name} is not exists");
				}
			}
			else
			{
				left = arg0.Build(e.BuildContext, e.ScriptContext, e.Options);
			}

			if (e.IsPrefix)
			{
				if (left.Type == typeof(object))
				{
					e.Result = ExpressionUtils.SubtractAssign(left, Expression.Constant(1), lastType);
				}
				else
				{
					e.Result = Expression.PreDecrementAssign(left);
				}
			}
			else
			{
				if (left.Type == typeof(object))
				{
					var tmp = Expression.Variable(left.Type);
					var tmpAssign = Expression.Assign(tmp, left);
					var expr = ExpressionUtils.SubtractAssign(left, Expression.Constant(1), lastType);
					e.Result = Expression.Block(new[] { tmp }, tmpAssign, expr, tmp);
				}
				else
				{
					e.Result = Expression.PostDecrementAssign(left);
				}
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 1) return;
			var arg0Node = e.Args[0];
			if (arg0Node is VariableNode varNode)
			{
				var arg0 = varNode.Eval(e.Context, e.Options, e.Control, out var type0);
				if (ScriptUtils.IsNumberType(type0))
				{
					if (ExpressionUtils.TryStep(arg0, false, e.IsPrefix, out var newValue, out var result))
					{
						e.SetResult(result);
						e.Context.SetTempVar(varNode.Name, newValue, true);
					}
					else
					{
						dynamic d = arg0;
						if (e.IsPrefix)
						{
							e.SetResult(--d);
						}
						else
						{
							e.SetResult(d--);
						}
						e.Context.SetTempVar(varNode.Name, d, true);
					}
				}
			}
			else if (arg0Node is OperatorNode opNode)
			{
				if (opNode.Name == "." && opNode.Right is VariableNode opRightNode)
				{
					// 属性赋值
					var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					var value = ScriptUtils.GetAndSetValue(opLeftValue, opRightNode.Name, out var type0, (m, t, v) =>
					{
						if (ExpressionUtils.TryStep(v, false, e.IsPrefix, out var newValue, out var result))
						{
							e.SetResult(result);
							return newValue;
						}
						dynamic d = v;
						if (e.IsPrefix)
						{
							e.SetResult(--d);
						}
						else
						{
							e.SetResult(d--);
						}
						return d;
					});
					return;
				}

				if (opNode.Name == "[]")
				{
					// 设置索引值
					var obj = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					var idx = opNode.Right.Eval(e.Context, e.Options, e.Control, out _);

					//// 根据obj类型处理索引器赋值
					object v2 = null;
					var v = ScriptUtils.GetAndSetValue(obj, idx, v1 => { v2 = v1; return ExpressionUtils.Subtract(v1, 1); });

					e.SetResult(e.IsPrefix ? v : v2);
					return;
				}
			}
		}
	}
}
