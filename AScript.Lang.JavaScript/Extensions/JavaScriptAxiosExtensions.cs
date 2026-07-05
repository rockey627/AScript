using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptAxiosExtensions
	{
		public static Task<JavaScriptHttpResponse[]> all(HttpClient client, IList<object> list)
		{
			return Task.WhenAll(list.Select(a => (Task<JavaScriptHttpResponse>)a));
		}

		public static HttpClient create(HttpClient client)
		{
			return create(client, null);
		}

		public static HttpClient create(HttpClient client, dynamic config)
		{
			var instance = new HttpClient();
			return instance;
		}

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
		private bool _disposed;
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
						if (item.Value == null) continue;
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

		~JavaScriptHttpResponse()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// 释放托管资源
				}
				// 释放非托管资源
				try { _Response?.Dispose(); } catch { }
				_disposed = true;
			}
		}
	}
}