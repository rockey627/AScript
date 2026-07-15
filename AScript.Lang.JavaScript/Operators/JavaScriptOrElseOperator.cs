using System;
using System.Linq.Expressions;

namespace AScript.Lang.JavaScript.Operators
{
	public class JavaScriptOrElseOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly JavaScriptOrElseOperator Instance = new JavaScriptOrElseOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var arg1 = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (arg0.Type == typeof(bool) && arg1.Type == typeof(bool))
				{
					e.Result = Expression.OrElse(arg0, arg1);
					return;
				}
				// 调用avaScriptLang.IsTrue方法判断是否为真，返回为真的项

			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				if (JavaScriptLang.IsTrue(arg0))
				{
					e.SetResult(arg0);
					return;
				}
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				if (JavaScriptLang.IsTrue(arg1))
				{
					e.SetResult(arg1);
					return;
				}
				if (arg0 is bool || arg1 is bool)
				{
					e.SetResult(false);
				}
				else
				{
					e.SetResult(null);
				}
			}
		}
	}
}
