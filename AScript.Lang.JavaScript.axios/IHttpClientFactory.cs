using System;
using System.Net.Http;

namespace AScript.Lang.JavaScript.axios
{
	public interface IHttpClientFactory
	{
		HttpClient CreateClient(string name);
	}
}