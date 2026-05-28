using System;
using AScript.Nodes;

namespace AScript.Operators
{
	public class DotOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly DotOperator Instance = new DotOperator();

		public bool Nullable { get; set; }

		public DotOperator() { }
		public DotOperator(bool nullable)
		{
			this.Nullable = nullable;
		}

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is VariableNode)) return;

			var arg0 = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var fieldName = ((VariableNode)e.Args[1]).Name;
			e.Result = ExpressionUtils.GetValue(arg0, fieldName, this.Nullable);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is VariableNode)) return;

			var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
			if (this.Nullable && arg0 == null)
			{
				e.SetResult(null);
				return;
			}
			var fieldName = ((VariableNode)e.Args[1]).Name;
			var value = ScriptUtils.GetValue(arg0, fieldName, out var type);
			e.SetResult(value, type);
		}
	}
}
