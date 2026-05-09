using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class EqualOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly EqualOperator Instance = new EqualOperator();

		public void Build(FunctionBuildArgs e)
		{
			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			if (left.Type == typeof(object) || right.Type == typeof(object)
				|| left.Type != right.Type && !ExpressionUtils.ConvertMaxType(ref left, ref right))
			{
				e.Result = Expression.Dynamic(ExpressionUtils.Binder_Equal, typeof(object), left, right);
			}
			else
			{
#if NETSTANDARD
				if (left.Type.Name.StartsWith("ValueTuple`") && left.Type == right.Type)
				{
					e.Result = Expression.Call(left, left.Type.GetMethod("Equals", new[] { left.Type }), right);
					return;
				}
#endif
				e.Result = Expression.Equal(left, right);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
#if NETSTANDARD
				if (type0.Name.StartsWith("ValueTuple`"))
				{
					e.SetResult(arg0.Equals(arg1));
					return;
				}
#endif
				e.SetResult((dynamic)arg0 == (dynamic)arg1);
			}
		}
	}
}
