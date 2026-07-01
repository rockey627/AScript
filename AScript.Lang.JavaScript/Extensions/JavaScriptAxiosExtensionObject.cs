#if NETSTANDARD
using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptAxiosExtensionObject : IScriptExtensionObject
	{
		public void Init(BaseContext context)
		{
			context.SetVar("axios", new System.Net.Http.HttpClient());
			context.AddFunc(typeof(JavaScriptAxiosExtensions));
		}
	}
}
#endif