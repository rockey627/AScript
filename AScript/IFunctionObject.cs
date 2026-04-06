using System;

namespace AScript
{
	public interface IFunctionObject
	{
		object DynamicInvoke(params object[] args);
		object DynamicInvoke(ScriptContext context, params object[] args);
	}
}
