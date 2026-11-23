using AScript.Nodes;
using AScript.Operators;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Go.Operators
{
	/// <summary>
	/// Go 赋值运算符：数组、结构体都是值类型，变量之间赋值需要拷贝
	/// </summary>
	public class GoAssignOperator : AssignOperator
	{
		public static readonly GoAssignOperator Instance = new GoAssignOperator();

		private static readonly MethodInfo Method_Array_Clone = typeof(Array).GetMethod("Clone");

		public override void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2)
			{
				base.Build(e);
				return;
			}
			// 只有变量之间赋值才需要拷贝
			if (e.Args[1] is VariableNode v
				&& e.BuildContext.TryGetVariableOrParameter(v.Name, out var parameter, out _, out _)
				&& parameter.Type.IsArray)
			{
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				var copied = Expression.Call(
					right,
					Method_Array_Clone);
				e.Args[1] = PoolManage.CreateExpressionNode(Expression.Convert(copied, parameter.Type));
			}
			base.Build(e);
		}

		public override void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			// 只有变量之间赋值才需要拷贝
			if (e.Args[1] is VariableNode variableNode)
			{
				var v1 = variableNode.Eval(e.Context, e.Options, e.Control, out _);
				if (v1 != null)
				{
					var type1 = v1.GetType();
					if (type1.IsArray)
					{
						var copied = ((Array)v1).Clone();
						e.Args[1] = PoolManage.CreateObjectNode(copied, type1);
					}
				}
			}
			base.Eval(e);
		}
	}
}
