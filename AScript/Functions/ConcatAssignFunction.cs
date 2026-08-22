using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Functions
{
	public class ConcatAssignFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly ConcatAssignFunction Instance = new ConcatAssignFunction();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0 = e.Args[0];
			Expression left;
			Type lastType = null;
			if (arg0 is VariableNode leftVar)
			{
				left = leftVar.BuildForAssign(e.BuildContext, e.ScriptContext, e.Options, out _, out lastType);
				if (left == null)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression: {leftVar.Name} is not exists");
				}
			}
			else
			{
				left = arg0.Build(e.BuildContext, e.ScriptContext, e.Options);
			}
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			Expression rightExpr = right;
			Expression result;
			if (left.Type == typeof(string))
			{
				if (rightExpr.Type == typeof(string))
				{
					result = Expression.Call(ScriptUtils.Method_String_Concat2, left, rightExpr);
				}
				else
				{
					if (rightExpr.Type.IsValueType)
					{
						rightExpr = Expression.Convert(rightExpr, typeof(object));
					}
					result = Expression.Call(ScriptUtils.Method_String_Concat2_object, left, rightExpr);
				}
			}
			else
			{
				var leftExpr = left;
				if (leftExpr.Type.IsValueType)
				{
					leftExpr = Expression.Convert(leftExpr, typeof(object));
				}
				if (rightExpr.Type.IsValueType)
				{
					rightExpr = Expression.Convert(rightExpr, typeof(object));
				}
				result = Expression.Call(ScriptUtils.Method_String_Concat2_object, leftExpr, rightExpr);
			}
			e.Result = Expression.Assign(left, result);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0Node = e.Args[0];

			if (arg0Node is VariableNode varNode)
			{
				object arg0 = arg0Node.Eval(e.Context, e.Options, e.Control, out _);
				object arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				string result = string.Concat(arg0, arg1);
				e.SetResult(result, typeof(string));
				e.Context.SetTempVar(varNode.Name, e.Result, true);
				return;
			}

			if (arg0Node is OperatorNode opNode)
			{
				if (opNode.Name == "." && opNode.Right is VariableNode opRightNode)
				{
					var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					var value = ScriptUtils.GetAndSetValue(opLeftValue, opRightNode.Name, out var type0, (m, t, v) =>
					{
						var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
						return string.Concat(v, arg1);
					});
					e.SetResult(value, typeof(string));
					return;
				}

				if (opNode.Name == "[]")
				{
					var obj = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					var idx = opNode.Right.Eval(e.Context, e.Options, e.Control, out _);
					var value = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);

					var v = ScriptUtils.GetAndSetValue(obj, idx, v1 => string.Concat(v1, value));

					e.SetResult(v, typeof(string));
					return;
				}
			}
		}
	}
}
