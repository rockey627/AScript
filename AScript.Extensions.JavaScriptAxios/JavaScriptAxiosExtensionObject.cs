using System;

namespace AScript.Extensions.JavaScriptAxios
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
