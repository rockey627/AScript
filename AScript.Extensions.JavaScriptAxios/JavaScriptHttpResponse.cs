using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace AScript.Extensions.JavaScriptAxios
{
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
