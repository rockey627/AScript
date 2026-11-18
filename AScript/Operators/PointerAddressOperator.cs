using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	/// <summary>
	/// 取址操作：&amp;a
	/// </summary>
	public class PointerAddressOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PointerAddressOperator Instance = new PointerAddressOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 1) return;

			var a0 = e.Args[0];
			if (a0 is VariableNode v0)
			{
				var addr = Pointer.CreateVariableAddress(v0.Name);
				var expr = v0.Build(e.BuildContext, e.ScriptContext, e.Options);
				var pointerType = typeof(Pointer<>).MakeGenericType(expr.Type);
				e.Result = Expression.Constant(Activator.CreateInstance(pointerType, addr), pointerType);
				//e.Result = Expression.New(ScriptUtils.Constructor_Pointer, Expression.Constant(expr.Type), Expression.Constant(addr));
				return;
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 1) return;

			var a0 = e.Args[0];
			if (a0 is VariableNode v0)
			{
				var addr = Pointer.CreateVariableAddress(v0.Name);
				var type = e.Context.GetVarType(v0.Name) ?? typeof(object);
				var pointerType = typeof(Pointer<>).MakeGenericType(type);
				e.SetResult(Activator.CreateInstance(pointerType, addr));
				return;
			}
		}
	}
}
