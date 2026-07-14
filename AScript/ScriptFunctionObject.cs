using System;
using System.Linq;

namespace AScript
{
	public class ScriptFunctionObject : IFunctionObject
	{
		private readonly ScriptContext _scriptContext;
		private readonly string _functionName;

		public ScriptFunctionObject(ScriptContext context, string functionName)
		{
			_scriptContext = context;
			_functionName = functionName;
		}

		public object DynamicInvoke(params object[] args)
		{
			return DynamicInvoke(_scriptContext, args);
		}

		public object DynamicInvoke(ScriptContext context, params object[] args)
		{
			return context.EvalFunc(_functionName, args, null);
		}

		public Delegate Compile(Type delegateType, BuildOptions options)
		{
			var argTypes = delegateType.GetMethod("Invoke").GetParameters().Select(a => a.ParameterType).ToArray();
			return _scriptContext.GetFunc(_functionName, argTypes);
		}
	}
}
