using AScript.Lang.JavaScript.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Module.JavaScriptAxios
{
	public static class JavaScriptAxiosExtensions
	{
		public static Task<JavaScriptHttpResponse[]> all(IHttpClientFactory factory, IList<object> list)
		{
			return Task.WhenAll(list.Select(a => (Task<JavaScriptHttpResponse>)a));
		}

		public static HttpClient create(IHttpClientFactory factory)
		{
			return factory.CreateClient(JavaScriptAxiosModule.ClientName);
		}

		public static HttpClient create(IHttpClientFactory factory, IDictionary<string, object> config)
		{
			var client = factory.CreateClient(JavaScriptAxiosModule.ClientName);
			return client;
		}

		public static HttpClient create(IHttpClientFactory factory, HttpMessageHandler messageHandler)
		{
			return new HttpClient(messageHandler);
		}

		public static HttpClient createMock(IHttpClientFactory factory, object responseBody)
		{
			return new HttpClient(new MockHttpMessageHandler(responseBody, 200));
		}

		public static HttpClient createMock(IHttpClientFactory factory, object responseBody, int statusCode)
		{
			return new HttpClient(new MockHttpMessageHandler(responseBody, statusCode));
		}

		public static Task<JavaScriptHttpResponse> get(IHttpClientFactory factory, string url)
		{
			var client = create(factory);
			return get(client, url);
		}

		public static Task<JavaScriptHttpResponse> delete(IHttpClientFactory factory, string url)
		{
			var client = create(factory);
			return delete(client, url);
		}

		public static Task<JavaScriptHttpResponse> post(IHttpClientFactory factory, string url)
		{
			var client = create(factory);
			return post(client, url);
		}

		public static Task<JavaScriptHttpResponse> post(IHttpClientFactory factory, string url, object data)
		{
			var client = create(factory);
			return post(client, url, data);
		}

		public static Task<JavaScriptHttpResponse> put(IHttpClientFactory factory, string url)
		{
			var client = create(factory);
			return put(client, url);
		}

		public static Task<JavaScriptHttpResponse> put(IHttpClientFactory factory, string url, object data)
		{
			var client = create(factory);
			return put(client, url, data);
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
		public static Task<JavaScriptHttpResponse> patch(IHttpClientFactory factory, string url)
		{
			var client = create(factory);
			return patch(client, url);
		}

		public static Task<JavaScriptHttpResponse> patch(IHttpClientFactory factory, string url, object data)
		{
			var client = create(factory);
			return patch(client, url, data);
		}

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
		private class MockHttpMessageHandler : HttpMessageHandler
		{
			private readonly string _responseBody;
			private readonly System.Net.HttpStatusCode _statusCode;

			public MockHttpMessageHandler(object responseBody, int statusCode)
			{
				if (responseBody is string s)
				{
					_responseBody = s;
				}
				else if (responseBody != null)
				{
					_responseBody = JsonConvert.SerializeObject(responseBody);
				}
				_statusCode = (System.Net.HttpStatusCode)statusCode;
			}

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				var response = new HttpResponseMessage(_statusCode)
				{
					Content = new StringContent(_responseBody, Encoding.UTF8, "application/json")
				};
				return Task.FromResult(response);
			}
		}
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