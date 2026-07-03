using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptConsoleModule : IScriptModule
	{
		public void Install(BaseContext context)
		{
			context.AddType("console", typeof(Console));

			context.AddFunc(typeof(JavaScriptConsoleExtensions));
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveType("console");
		}
	}
}
