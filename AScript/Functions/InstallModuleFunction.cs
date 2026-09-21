using System;
using System.Linq.Expressions;

namespace AScript.Functions
{
	public class InstallModuleFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly InstallModuleFunction Instance = new InstallModuleFunction();

		public void Build(FunctionBuildArgs e)
		{
			e.BuildArgs();
			object module = null;
			for (int i = 0; i < e.ArgExprs.Count; i++)
			{
				var nameExpr = e.ArgExprs[i];
				if (nameExpr is ConstantExpression constantExpression)
				{
					var moduleName = (string)constantExpression.Value;
					module = e.ScriptContext.InstallModule(moduleName);
					continue;
				}
				throw new Exceptions.ScriptRuntimeException($"{e.Name} need const string arg");
			}
			e.Result = Expression.Constant(module, module?.GetType() ?? typeof(object));
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.EvalArgs();
			object module = null;
			for (int i = 0; i < e.ArgValues.Length; i++)
			{
				var moduleName = (string)e.ArgValues[i];
				module = e.Context.InstallModule(moduleName);
			}
			e.SetResult(module);
		}
	}
}
