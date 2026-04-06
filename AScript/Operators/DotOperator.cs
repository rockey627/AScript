using System;
using AScript.Nodes;

namespace AScript.Operators
{
	public class DotOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly DotOperator Instance = new DotOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is VariableNode)) return;

			var arg0 = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var fieldName = ((VariableNode)e.Args[1]).Name;
			e.Result = ExpressionUtils.GetValue(arg0, fieldName);
			//if (arg0.Type == typeof(TypeWrapper))
			//{
			//	// 调用静态类属性或字段
			//	var type = ((TypeWrapper)((ConstantExpression)arg0).Value).Type;
			//	var property = type.GetProperty(fieldName, BindingFlags.Static | BindingFlags.Public);
			//	if (property != null)
			//	{
			//		e.Result = Expression.Property(null, property.GetMethod);
			//	}
			//	else
			//	{
			//		var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
			//		e.Result = Expression.Field(null, field);
			//	}
			//}
			//else
			//{
			//	// 变量的属性或字段
			//	e.Result = Expression.PropertyOrField(arg0, fieldName);
			//}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is VariableNode)) return;

			var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
			var fieldName = ((VariableNode)e.Args[1]).Name;
			var value = ScriptUtils.GetValue(arg0, fieldName, out var type);
			e.SetResult(value, type);
		}
	}
}
