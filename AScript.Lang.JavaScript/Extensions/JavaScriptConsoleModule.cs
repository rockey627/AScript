using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptConsoleModule : IScriptModule
	{
		public object Install(BaseContext context)
		{
			if (context.EvalType("console") != null) return null;
			context.AddType("console", typeof(Console));
			context.AddFunc(typeof(JavaScriptConsoleExtensions));
			return null;
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveType("console");
		}
	}
}
