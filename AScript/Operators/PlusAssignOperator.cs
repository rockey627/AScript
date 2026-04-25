using System;
using System.Linq.Expressions;
using AScript.Nodes;

namespace AScript.Operators
{
	public class PlusAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PlusAssignOperator Instance = new PlusAssignOperator();

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
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			Expression leftExpr = left;
			Expression rightExpr = right;
			if (left.Type == typeof(object) || right.Type == typeof(object)
				|| !ExpressionUtils.ConvertMaxType(ref leftExpr, ref rightExpr))
			{
				// dynamic方式作用+=无效
				//e.Result = Expression.Dynamic(ExpressionUtils.Binder_AddAssign, typeof(object), left, right);
				var addExpr = Expression.Dynamic(ExpressionUtils.Binder_Add, typeof(object), leftExpr, rightExpr);
				e.Result = Expression.Assign(left, addExpr);
			}
			else if (left.Type == typeof(string) && right.Type == typeof(string))
			{
				// 字符串相加使用string.Concat方法
				e.Result = Expression.Assign(left, Expression.Call(ExpressionUtils.Method_String_Concat2, left, right));
			}
			else
			{
				e.Result = Expression.AddAssign(left, right);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0Node = e.Args[0];
			if (arg0Node is VariableNode varNode)
			{
				dynamic arg0 = arg0Node.Eval(e.Context, e.Options, e.Control, out var type0);
				dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				arg0 += arg1;
				e.SetResult(arg0, type0);
				e.Context.SetTempVar(varNode.Name, e.Result, true);
			}
			else if (arg0Node is OperatorNode opNode && opNode.Name == "." && opNode.Right is VariableNode opRightNode)
			{
				// 属性赋值
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type1);
				var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
				var value = ScriptUtils.GetAndSetValue(opLeftValue, opRightNode.Name, out var type0, (t, v) => (dynamic)v + (dynamic)arg1);
				e.SetResult(value, type0 == typeof(object) ? type1 : type0);
			}
		}
	}
}
