using System;
using System.Net.Http;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptAxiosModule : IScriptModule, IScriptModuleType
	{
		public const string ClientName = "AScript.JavaScript.Axios";

		public static IHttpClientFactory HttpClientFactory = new DefaultHttpClientFactory();

		public Type ModuleType => typeof(IHttpClientFactory);

		public object Install(BaseContext context)
		{
			var axios = (IHttpClientFactory)context.EvalVar("axios");
			if (axios != null) return axios;
			axios = HttpClientFactory;
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

		private class DefaultHttpClientFactory : IHttpClientFactory
		{
			private readonly HttpClientHandler _Handler = new HttpClientHandler();

			public HttpClient CreateClient(string name)
			{
				return new HttpClient(_Handler, false);
			}
		}
	}
}