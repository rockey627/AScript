using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	public class NowFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly NowFunction Instance = new NowFunction();

		private static readonly PropertyInfo Property_DateTime_Now = typeof(DateTime).GetProperty("Now");

		public void Build(FunctionBuildArgs e)
		{
			e.Result = Expression.Property(null, Property_DateTime_Now);
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.SetResult(DateTime.Now);
		}
	}
}
