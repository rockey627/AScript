using System;

namespace AScript
{
	public class CustomFunctionEvaluator : IFunctionEvaluator
	{
		public static readonly CustomFunctionEvaluator Instance = new CustomFunctionEvaluator();

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args == null || e.Args.Count == 0) return;
			var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
			if (!(arg0 is CustomFunctionObject customFunctionObj)) return;
			if (e.Args.Count > 1)
			{
				for (int i = 1; i < e.Args.Count; i++)
				{
					e.Context.SetVar(customFunctionObj.Function.ArgNames[i - 1], e.Args[i].Eval(e.Context, e.Options, e.Control, out var type), type);
				}
			}
			if (customFunctionObj.Function.Body == null)
			{
				e.SetResult(null, null);
			}
			else
			{
				var result = customFunctionObj.Function.Body.Eval(e.Context, e.Options, new EvalControl(), out var returnType);
				e.SetResult(result, returnType);
			}
		}
	}
}
