using System;
using System.Net.Http;

namespace AScript.Module.JavaScriptAxios
{
	public class JavaScriptAxiosModule : IScriptModule, IScriptModuleType
	{
		public static readonly string ClientName = "AScript.JavaScript.Axios";

		public static IHttpClientFactory HttpClientFactory = new DefaultHttpClientFactory();

		public Type ModuleType => typeof(IHttpClientFactory);

		public object Install(BaseContext context)
		{
			context.SetObjectMemberEnabled(typeof(JavaScriptHttpResponse), true);
			context.AddFunc(typeof(JavaScriptAxiosExtensions));
			return HttpClientFactory;
		}

		public void Uninstall(BaseContext context)
		{
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