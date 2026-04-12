using System;
using System.IO;
using System.Linq.Expressions;

namespace AScript
{
	public interface IScriptProvider
	{
		object Eval(ScriptContext context, BuildOptions options, string expression, out Type returnType);
		object Eval(ScriptContext context, BuildOptions options, Stream expression, out Type returnType);
		LambdaExpression Lambda(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression);
		LambdaExpression Lambda(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression);
		Delegate Compile(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression);
		Delegate Compile(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression);
	}
}
