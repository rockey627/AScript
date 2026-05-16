using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class ConvertOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public int TypeIndex { get; set; }

		public ConvertOperator() { }
		public ConvertOperator(int typeIndex)
		{
			this.TypeIndex = typeIndex;
		}

		public void Build(FunctionBuildArgs e)
		{
			int valueIndex = this.TypeIndex == 0 ? 1 : 0;
			var type = (Type)((ConstantExpression)e.Args[this.TypeIndex].Build(e.BuildContext, e.ScriptContext, e.Options)).Value;
			var v = e.Args[valueIndex].Build(e.BuildContext, e.ScriptContext, e.Options);
			e.Result = v.Type == type ? v : Expression.Convert(v, type);
		}

		public void Eval(FunctionEvalArgs e)
		{
			int valueIndex = this.TypeIndex == 0 ? 1 : 0;
			var type = (Type)e.Args[this.TypeIndex].Eval(e.Context, e.Options, e.Control, out _);
			var v = e.Args[valueIndex].Eval(e.Context, e.Options, e.Control, out var t);
			if (t == type || v == null)
			{
				e.SetResult(v, type);
			}
			else
			{
				e.SetResult(Convert.ChangeType(v, type), type);
			}
		}
	}
}
