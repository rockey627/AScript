using System;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Functions
{
	public class LambdaFunction : IFunctionEvaluator, IFunctionBuilder
	{
		private readonly LambdaExpression _lambda;
		private Delegate _del;

		public LambdaFunction(LambdaExpression lambda)
		{
			_lambda = lambda;
		}

		public void Build(FunctionBuildArgs e)
		{
			var args = e.BuildArgs();
			var argTypes = args?.Select(a => a.Type).ToList();
			if (ScriptUtils.IsMatchArgTypes(argTypes, _lambda, out _, out _))
			{
				e.Result = _lambda;
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.EvalArgs();
			if (ScriptUtils.IsMatchArgTypes(e.ArgTypes, _lambda, out _, out _))
			{
				if (_del == null)
				{
					lock (this)
					{
						if (_del == null) _del = _lambda.Compile();
					}
				}
				e.SetResult(_del.DynamicInvoke(e.ArgValues), _lambda.Type);
			}
		}
	}
}
