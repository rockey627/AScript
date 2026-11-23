using System;
using System.Linq.Expressions;
using AScript.Nodes;

namespace AScript.Operators
{
	public class PointerDataOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PointerDataOperator Instance = new PointerDataOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.GetArgsCount() != 1 || !e.IsPrefix) return;

			var p0 = e.BuildArgs(0);
			if (p0 == null) return;

			// 编译期能确定地址类型时，直接按变量的方式取数据
			var pointer = TryGetPointer(e, p0);
			if (pointer?.Address is Pointer.VariableAddress variableAddress)
			{
				var expr = new VariableNode(variableAddress.Name).Build(e.BuildContext, e.ScriptContext, e.Options);
				if (expr != null)
				{
					e.Result = expr;
					return;
				}
			}

			if (!typeof(Pointer).IsAssignableFrom(p0.Type))
			{
				p0 = Expression.Convert(p0, typeof(Pointer));
			}
			var addr0 = Expression.Property(p0, "Address");
			var data = Expression.Call(addr0, ScriptUtils.Method_Pointer_Address_GetValue, e.BuildContext.GetScriptContextParameter(true));
			e.Result = data;
		}

		/// <summary>
		/// 尝试在编译期确定指针
		/// </summary>
		private static Pointer TryGetPointer(FunctionBuildArgs e, Expression p0)
		{
			// 编译期常量指针
			if (ScriptUtils.TryGetPointerConstant(p0, out var pointer)) return pointer;
			// 编译期已知指针值的变量
			if (e.Args != null && e.Args[0] is VariableNode variableNode
				&& e.BuildContext.TryGetPointerVariable(variableNode.Name, out var known)) return known;
			return null;
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.GetArgsCount() != 1 || !e.IsPrefix) return;

			var a0 = (Pointer)e.EvalArgs(0, out _);
			e.SetResult(a0.Address.GetValue(e.Context));
		}
	}
}
