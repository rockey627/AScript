using System;
using System.Linq.Expressions;
using System.Reflection;
using AScript.Nodes;

namespace AScript.Operators
{
	public class SubtractAssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly SubtractAssignOperator Instance = new SubtractAssignOperator();

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
			if (left.Type == typeof(object) || right.Type == typeof(object))
			{
				// dynamic方式作用-=无效
				//e.Result = Expression.Dynamic(ExpressionUtils.Binder_SubtractAssign, typeof(object), left, right);
				var expr = Expression.Dynamic(ExpressionUtils.Binder_Subtract, typeof(object), left, right);
				e.Result = Expression.Assign(left, expr);
			}
			else
			{
				e.Result = Expression.SubtractAssign(left, right);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0Node = e.Args[0];
			if (arg0Node is VariableNode varNode)
			{
				dynamic arg0 = varNode.Eval(e.Context, e.Options, e.Control, out var type0);
				dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				arg0 -= arg1;
				e.SetResult(arg0, type0);
				e.Context.SetTempVar(varNode.Name, e.Result, true);
			}
			else if (arg0Node is OperatorNode opNode && opNode.Name == "." && opNode.Right is VariableNode opRightNode)
			{
				Type type1 = null;
				var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
				var value = ScriptUtils.GetAndSetValue(opLeftValue, opRightNode.Name, out var type0, (m, t, v) =>
				{
					if (m is EventInfo eventInfo)
					{
						// 事件
						var arg1Node = e.Args[1];
						Delegate d;
						if (arg1Node is VariableNode var1)
						{
							d = e.Context.GetEvent(var1.Name, t);
							if (d != null)
							{
								eventInfo.RemoveEventHandler(opLeftValue is TypeWrapper ? null : opLeftValue, d);
							}
							return d;
						}
						throw new Exception("invalid expression near -= event");
					}
					else
					{
						// 属性赋值
						var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out type1);
						return (dynamic)v - (dynamic)arg1;
					}
				});
				e.SetResult(value, type0 == typeof(object) ? type1 : type0);
			}
		}
	}
}
