#if NETSTANDARD
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptAxiosExtensions
	{
		public static async Task<JavaScriptHttpResponse> get(HttpClient client, string url)
		{
			var response = await client.GetAsync(url).ConfigureAwait(false);
			return new JavaScriptHttpResponse(response);
		}

		public static async Task<JavaScriptHttpResponse> post(HttpClient client, string url)
		{
			var content = new StringContent(null, Encoding.UTF8, "application/json");
			var response = await client.PostAsync(url, content).ConfigureAwait(false);
			return new JavaScriptHttpResponse(response);
		}

		public static async Task<JavaScriptHttpResponse> post(HttpClient client, string url, object data)
		{
			var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(url, content).ConfigureAwait(false);
			return new JavaScriptHttpResponse(response);
		}
	}

	public class JavaScriptHttpResponse : IDisposable
	{
		private readonly HttpResponseMessage _Response;

		private bool _IsParsed;
		private object _Data;

		public object data
		{
			get
			{
				if (!_IsParsed)
				{
					_IsParsed = true;
					var s = _Response.Content.ReadAsStringAsync().Result;
					_Data = JavaScriptJsonExtensions.JSON_parse(s);
				}
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