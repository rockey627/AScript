using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptAxiosModule : IScriptModule
	{
		public object Install(BaseContext context)
		{
			var axios = (System.Net.Http.HttpClient)context.EvalVar("axios");
			if (axios != null) return axios;
			axios = new System.Net.Http.HttpClient();
			context.SetVar("axios", axios);
			context.SetObjectMemberEnabled(typeof(JavaScriptHttpResponse), true);
			context.AddFunc(typeof(JavaScriptAxiosExtensions));
			return axios;
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveVar("axios");
			context.SetObjectMemberEnabled(typeof(JavaScriptHttpResponse), null);
		}
	}
}