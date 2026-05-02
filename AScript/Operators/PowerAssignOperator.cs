using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	/// <summary>
	/// 幂运算：a**=3
	/// </summary>
	public class PowerAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PowerAssignOperator Instance = new PowerAssignOperator();

		public void Build(FunctionBuildArgs e)
		{
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
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			var v1 = Expression.Convert(left, typeof(double));
			var v2 = Expression.Convert(right, typeof(double));
			var v = Expression.Call(ExpressionUtils.Method_Math_Power, v1, v2);
			e.Result = Expression.Assign(left, Expression.Convert(v, left.Type));
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0Node = e.Args[0];
			if (arg0Node is VariableNode varNode)
			{
				var v1 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				var v2 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				var r = Math.Pow(Convert.ToDouble(v1), Convert.ToDouble(v2));
				e.SetResult(ScriptUtils.Convert(r, type0));
				e.Context.SetTempVar(varNode.Name, e.Result, true);
			}
			else if (arg0Node is OperatorNode opNode && opNode.Name == "." && opNode.Right is VariableNode opRightNode)
			{
				// 属性赋值
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type1);
				var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
				var value = ScriptUtils.GetAndSetValue(opLeftValue, opRightNode.Name, out var type0, (t, v) => Math.Pow(Convert.ToDouble(v), Convert.ToDouble(arg1)));
				e.SetResult(value, type0 == typeof(object) ? type1 : type0);
			}
		}
	}
}
