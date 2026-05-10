using System;
using System.Linq.Expressions;

namespace AScript.Functions
{
	/// <summary>
	/// 类型转换函数
	/// </summary>
	public class ConvertFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly ConvertFunction Instance = new ConvertFunction();

		public void Build(FunctionBuildArgs e)
		{
			var v = e.BuildArgs(0);
			var t = (TypeWrapper)e.Args[1].Eval(e.ScriptContext, e.Options, e.Control, out _);
			e.Result = Expression.Convert(v, t.Type);
		}

		public void Eval(FunctionEvalArgs e)
		{
			var v = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
			var t = (TypeWrapper)e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
			e.SetResult(v, t.Type);
		}
	}
}
