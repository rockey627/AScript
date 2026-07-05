#if NET45
using System;

namespace System.Net.Http
{
	public interface IHttpClientFactory
	{
		HttpClient CreateClient(string name);
	}
}
#endif