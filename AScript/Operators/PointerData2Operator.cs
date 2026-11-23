using System;
using System.Linq.Expressions;
using AScript.Nodes;

namespace AScript.Operators
{
	/// <summary>
	/// **p
	/// </summary>
	public class PointerData2Operator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PointerData2Operator Instance = new PointerData2Operator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 1 || !e.IsPrefix) return;

			var args1 = new FunctionBuildArgs(e.BuildContext, e.ScriptContext, e.Options, e.Control, "*", true, e.Args, e.ArgExprs);
			PointerDataOperator.Instance.Build(args1);

			var args2 = new FunctionBuildArgs(e.BuildContext, e.ScriptContext, e.Options, e.Control, "*", true, null, new[] { args1.Result });
			PointerDataOperator.Instance.Build(args2);

			e.Result = args2.Result;
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 1 || !e.IsPrefix) return;

			var a1 = (Pointer)e.EvalArgs(0, out _);
			var p1 = a1.Address.GetValue(e.Context);
			var a2 = (Pointer)p1;
			var p2 = a2.Address.GetValue(e.Context);
			e.SetResult(p2);
		}
	}
}
