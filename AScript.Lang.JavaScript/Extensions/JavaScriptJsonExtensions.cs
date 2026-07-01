using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptJsonExtensions
	{
		public static JToken JSON_parse(string s)
		{
			return (JToken)JsonConvert.DeserializeObject(s);
		}

		public static string JSON_stringify(object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
	}
}
