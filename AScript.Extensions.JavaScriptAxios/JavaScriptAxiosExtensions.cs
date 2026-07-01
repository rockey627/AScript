using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AScript.Extensions.JavaScriptAxios
{
	public static class JavaScriptAxiosExtensions
	{
		public static Task<HttpResponseMessage> get(HttpClient client, string url)
		{
			return client.GetAsync(url);
		}
	}
}