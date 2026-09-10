using System;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Operators
{
	/// <summary>
	/// Go语言的 &amp;^ 运算符（AND NOT）
	/// </summary>
	public class AndNotOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly AndNotOperator Instance = new AndNotOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			// left &amp; ~right
			var notRight = Expression.Not(right);
			e.Result = Expression.And(left, notRight);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				if (arg0 is long l && arg1 is long r)
				{
					e.SetResult(l & ~r);
				}
				else if (arg0 is int il && arg1 is int ir)
				{
					e.SetResult(il & ~ir);
				}
				else if (arg0 is ulong ul && arg1 is ulong ur)
				{
					e.SetResult(ul & ~ur);
				}
				else if (arg0 is uint iul && arg1 is uint iur)
				{
					e.SetResult(iul & ~iur);
				}
				else
				{
					e.SetResult(((long)Convert.ToInt64(arg0)) & ~Convert.ToInt64(arg1));
				}
			}
		}
	}
}
