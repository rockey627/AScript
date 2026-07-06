using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.JavaScript.Functions
{
	public class JavaScriptRequireFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly JavaScriptRequireFunction Instance = new JavaScriptRequireFunction();

		private static readonly MethodInfo Method_BaseContext_InstallModule_string = typeof(BaseContext).GetMethod("InstallModule", new[] { typeof(string) });

		public void Build(FunctionBuildArgs e)
		{
			e.BuildArgs();
			var nameExpr = e.ArgExprs[0];
			Type moduleType = null;
			if (nameExpr is ConstantExpression constantExpression)
			{
				var module = e.ScriptContext.GetModule((string)constantExpression.Value);
				moduleType = (module as IScriptModuleType)?.ModuleType;
			}
			Expression result = Expression.Call(e.BuildContext.GetScriptContextParameter(), Method_BaseContext_InstallModule_string, nameExpr);
			if (moduleType != null)
			{
				result = Expression.Convert(result, moduleType);
			}
			e.Result = result;
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.EvalArgs();
			var module = e.Context.InstallModule((string)e.ArgValues[0]);
			e.SetResult(module);
		}
	}
}
