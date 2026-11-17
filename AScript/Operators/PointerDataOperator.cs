using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class PointerDataOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PointerDataOperator Instance = new PointerDataOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 1) return;

			var p0 = e.BuildArgs(0);
			var addr0 = Expression.Property(p0, "Address");
			var addr = Expression.Convert(addr0, typeof(Pointer.VariableAddress));
			var name = Expression.Property(addr, "Name");
			var data = Expression.Call(addr, ScriptUtils.Method_Pointer_Address_GetValue, e.BuildContext.GetScriptContextParameter(true));
			e.Result = data;
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 1) return;

			var a0 = (Pointer)e.EvalArgs(0, out _);
			e.SetResult(a0.Address.GetValue(e.Context));
		}
	}
}
