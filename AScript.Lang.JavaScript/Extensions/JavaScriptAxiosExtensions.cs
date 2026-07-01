#if NETSTANDARD
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptAxiosExtensions
	{
		public static Task<HttpResponseMessage> get(HttpClient client, string url)
		{
			return client.GetAsync(url);
		}
	}

	public class JavaScriptHttpResponse : IDisposable
	{
		private readonly HttpResponseMessage _Response;

		private JToken _Data;

		public JToken data
		{
			get
			{
				return _Data;
			}
		}

		public JavaScriptHttpResponse(HttpResponseMessage response)
		{
			_Response = response;
		}

		public void Dispose()
		{
			_Response.Dispose();
		}
	}
}
#endif