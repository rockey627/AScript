using System;
using System.Net.Http;

namespace AScript.Lang.JavaScript.axios
{
	public class JavaScriptAxiosModule : IScriptModule
	{
		public static readonly string ClientName = "AScript.Lang.JavaScript.axios";

		public static IHttpClientFactory HttpClientFactory { get; set; } = new DefaultHttpClientFactory();

		public object Install(BaseContext context)
		{
			//context.SetObjectMemberEnabled(typeof(JavaScriptHttpResponse), true);
			context.SetObjectMemberEnabled(typeof(IHttpClientFactory), false);
			context.SetObjectMemberEnabled(typeof(HttpClient), false);
			context.AddFunc(typeof(JavaScriptAxiosExtensions));
			return HttpClientFactory;
		}

		public void Uninstall(BaseContext context)
		{
			//context.SetObjectMemberEnabled(typeof(JavaScriptHttpResponse), null);
			context.SetObjectMemberEnabled(typeof(IHttpClientFactory), null);
			context.SetObjectMemberEnabled(typeof(HttpClient), null);
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