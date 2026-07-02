using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptJsonExtensions
	{
		public static object JSON_parse(string s)
		{
			var token = JToken.Parse(s);
			return ConvertToken(token);
		}

		private static object ConvertToken(JToken token)
		{
			switch (token.Type)
			{
				case JTokenType.Object:
					return ConvertObject((JObject)token);
				case JTokenType.Array:
					return ConvertArray((JArray)token);
				case JTokenType.Integer:
					return (long)token;
				case JTokenType.Float:
					return (double)token;
				case JTokenType.String:
					return (string)token;
				case JTokenType.Boolean:
					return (bool)token;
				case JTokenType.Null:
					return null;
				default:
					return null;
			}
		}

		private static ExpandoObject ConvertObject(JObject obj)
		{
			var expando = new ExpandoObject();
			var dict = (IDictionary<string, object>)expando;
			foreach (var property in obj.Properties())
			{
				dict[property.Name] = ConvertToken(property.Value);
			}
			return expando;
		}

		private static List<object> ConvertArray(JArray array)
		{
			var list = new List<object>();
			foreach (var item in array)
			{
				list.Add(ConvertToken(item));
			}
			return list;
		}

		public static string JSON_stringify(object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
	}
}
