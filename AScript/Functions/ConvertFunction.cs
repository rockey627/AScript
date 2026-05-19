using System;
using System.Linq.Expressions;

namespace AScript.Functions
{
	public class ConvertFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public const string FORCE_NAME = "<>__convert__";

		public int TypeIndex { get; set; }

		public ConvertFunction() { }
		public ConvertFunction(int typeIndex)
		{
			TypeIndex = typeIndex;
		}

		public void Build(FunctionBuildArgs e)
		{
			int valueIndex = TypeIndex == 0 ? 1 : 0;
			var t = ((ConstantExpression)e.Args[TypeIndex].Build(e.BuildContext, e.ScriptContext, e.Options)).Value;
			Type type;
			if (t is Type tt) type = tt;
			else if (t is TypeWrapper wrapper) type = wrapper.Type;
			else throw new Exceptions.ScriptRuntimeException($"invalid expression new type convert");
			var v = e.Args[valueIndex].Build(e.BuildContext, e.ScriptContext, e.Options);
			e.Result = v.Type == type ? v : Expression.Convert(v, type);
		}

		public void Eval(FunctionEvalArgs e)
		{
			int valueIndex = TypeIndex == 0 ? 1 : 0;
			var t = e.Args[TypeIndex].Eval(e.Context, e.Options, e.Control, out _);
			Type type;
			if (t is Type tt) type = tt;
			else if (t is TypeWrapper wrapper) type = wrapper.Type;
			else throw new Exceptions.ScriptRuntimeException($"invalid expression new type convert");
			var v = e.Args[valueIndex].Eval(e.Context, e.Options, e.Control, out var t2);
			if (t2 == type || v == null)
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
