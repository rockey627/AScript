#if NETSTANDARD
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
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

		public static async Task<JavaScriptHttpResponse> delete(HttpClient client, string url)
		{
			var response = await client.DeleteAsync(url).ConfigureAwait(false);
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

		public static async Task<JavaScriptHttpResponse> put(HttpClient client, string url)
		{
			var content = new StringContent(null, Encoding.UTF8, "application/json");
			var response = await client.PutAsync(url, content).ConfigureAwait(false);
			return new JavaScriptHttpResponse(response);
		}

		public static async Task<JavaScriptHttpResponse> put(HttpClient client, string url, object data)
		{
			var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
			var response = await client.PutAsync(url, content).ConfigureAwait(false);
			return new JavaScriptHttpResponse(response);
		}

#if NETSTANDARD2_1_OR_GREATER
		public static async Task<JavaScriptHttpResponse> patch(HttpClient client, string url)
		{
			var content = new StringContent(null, Encoding.UTF8, "application/json");
			var response = await client.PatchAsync(url, content).ConfigureAwait(false);
			return new JavaScriptHttpResponse(response);
		}

		public static async Task<JavaScriptHttpResponse> patch(HttpClient client, string url, object data)
		{
			var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
			var response = await client.PatchAsync(url, content).ConfigureAwait(false);
			return new JavaScriptHttpResponse(response);
		}
#endif
	}

	public class JavaScriptHttpResponse : IDisposable
	{
		private readonly HttpResponseMessage _Response;

		private bool _IsDataParsed;
		private bool _IsHeadersParsed;
		private object _Data;
		private ExpandoObject _Headers;

		public object data
		{
			get
			{
				if (!_IsDataParsed)
				{
					_IsDataParsed = true;
					var s = _Response.Content.ReadAsStringAsync().Result;
					_Data = JavaScriptJsonExtensions.JSON_parse(s);
				}
				return _Data;
			}
		}

		public long status => (int)_Response.StatusCode;

		public string statusText => _Response.ReasonPhrase;

		public ExpandoObject headers
		{
			get
			{
				if (!_IsHeadersParsed)
				{
					_IsHeadersParsed = true;
					_Headers = new ExpandoObject();
					var dict = (IDictionary<string, object>)headers;
					foreach (var item in _Response.Headers)
					{
						dict[item.Key] = string.Join(",", item.Value);
					}
				}
				return _Headers;
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