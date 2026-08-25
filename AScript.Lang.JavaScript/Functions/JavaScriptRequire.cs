using System;
using System.Linq.Expressions;

namespace AScript.Lang.JavaScript.Functions
{
	public class JavaScriptRequire : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly JavaScriptRequire Instance = new JavaScriptRequire();

		public void Build(FunctionBuildArgs e)
		{
			e.BuildArgs();
			var nameExpr = e.ArgExprs[0];
			//Type moduleType = null;
			if (nameExpr is ConstantExpression constantExpression)
			{
				var moduleName = (string)constantExpression.Value;
				//var instance = e.ScriptContext.InstallModule(moduleName);
				//e.Result = Expression.Constant(instance, instance?.GetType() ?? typeof(object));
				var module = JavaScriptExportModule.InstallModule(e.ScriptContext, moduleName);
				e.Result = Expression.Constant(module.exports);
				return;
			}
			throw new Exceptions.ScriptRuntimeException($"{e.Name} need const string arg");
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.EvalArgs();
			var module = JavaScriptExportModule.InstallModule(e.Context, (string)e.ArgValues[0]);
			e.SetResult(module.exports);
		}
	}
}
