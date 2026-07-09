using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	public class InstallModuleFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly InstallModuleFunction Instance = new InstallModuleFunction();

		//private static readonly MethodInfo Method_BaseContext_InstallModule_string = typeof(BaseContext).GetMethod("InstallModule", new[] { typeof(string) });

		public void Build(FunctionBuildArgs e)
		{
			e.BuildArgs();
			var nameExpr = e.ArgExprs[0];
			//Type moduleType = null;
			if (nameExpr is ConstantExpression constantExpression)
			{
				var moduleName = (string)constantExpression.Value;
				//var module = e.ScriptContext.GetModule(moduleName);
				//moduleType = (module as IScriptModuleType)?.ModuleType;
				var instance = e.ScriptContext.InstallModule(moduleName);
				e.Result = Expression.Constant(instance, instance?.GetType() ?? typeof(object));
				return;
			}
			throw new Exceptions.ScriptRuntimeException($"{e.Name} need const string arg");
			//Expression result = Expression.Call(e.BuildContext.GetScriptContextParameter(), Method_BaseContext_InstallModule_string, nameExpr);
			//if (moduleType != null)
			//{
			//	result = Expression.Convert(result, moduleType);
			//}
			//e.Result = result;
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.EvalArgs();
			var module = e.Context.InstallModule((string)e.ArgValues[0]);
			e.SetResult(module);
		}
	}
}
