using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	public class NotEqualOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly NotEqualOperator Instance = new NotEqualOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var leftNode = e.Args[0];
			var rightNode = e.Args[1];
			Expression left = null, right = null;
			if (leftNode is ObjectNode objNode0 && objNode0.Data == null)
			{
				if (rightNode is ObjectNode objNode1 && objNode1.Data == null)
				{
					e.Result = Expression.Constant(false);
					return;
				}
				right = rightNode.Build(e.BuildContext, e.ScriptContext, e.Options);
				left = Expression.Constant(null, right.Type);
			}
			else if (rightNode is ObjectNode objNode1 && objNode1.Data == null)
			{
				left = leftNode.Build(e.BuildContext, e.ScriptContext, e.Options);
				right = Expression.Constant(null, left.Type);
			}
			if (left == null) left = leftNode.Build(e.BuildContext, e.ScriptContext, e.Options);
			if (right == null) right = rightNode.Build(e.BuildContext, e.ScriptContext, e.Options);
			if (left.Type == typeof(object) || right.Type == typeof(object)
				|| left.Type != right.Type && !ExpressionUtils.ConvertMaxType(ref left, ref right))
			{
				e.Result = Expression.Convert(Expression.Dynamic(ExpressionUtils.Binder_NotEqual, typeof(object), left, right), typeof(bool));
			}
			else
			{
#if NETSTANDARD
				if (left.Type.Name.StartsWith("ValueTuple`") && left.Type == right.Type)
				{
					e.Result = Expression.Not(Expression.Call(left, left.Type.GetMethod("Equals", new[] { left.Type }), right));
					return;
				}
#endif
				e.Result = Expression.NotEqual(left, right);
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
					e.SetResult(!arg0.Equals(arg1));
					return;
				}
#endif
				e.SetResult((dynamic)arg0 != (dynamic)arg1);
			}
		}
	}
}
