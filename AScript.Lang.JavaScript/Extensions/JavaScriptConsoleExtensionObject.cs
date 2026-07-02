using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptConsoleExtensionObject : IScriptExtensionObject
	{
		public void Init(BaseContext context)
		{
			context.AddType("console", typeof(Console));

			context.AddFunc(typeof(JavaScriptConsoleExtensions));
		}
	}
}
