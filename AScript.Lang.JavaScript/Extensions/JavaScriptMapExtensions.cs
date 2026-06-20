using System;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptMapExtensions
	{
		public static Dictionary<object, object> new_Map()
		{
			return new Dictionary<object, object>();
		}

		public static Dictionary<object, object> new_Map(List<object> list)
		{
			var dict = new Dictionary<object, object>();
			if (list != null)
			{
				foreach(List<object> item in list)
				{
					dict[item[0]] = item[1];
				}
			}
			return dict;
		}

		//public static void Map_groupBy(Dictionary<object, object> dict, Func<object, object> groupBy)
		//{

		//}

		public static Dictionary<object, object> set(Dictionary<object, object> dict, object key, object value)
		{
			dict[key] = value;
			return dict;
		}

		public static object get(Dictionary<object, object> dict, object key)
		{
			if (dict.TryGetValue(key, out var value))
			{
				return value;
			}
			return JavaScriptUndefined.Instance;
		}

		public static bool delete(Dictionary<object, object> dict, object key)
		{
			return dict.Remove(key);
		}

		public static void clear(Dictionary<object, object> dict)
		{
			dict.Clear();
		}

		public static bool has(Dictionary<object, object> dict, object key)
		{
			return dict.ContainsKey(key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dict"></param>
		/// <param name="action">(value, key) => {}</param>
		public static void forEach(Dictionary<object, object> dict, Action<object, object> action)
		{
			foreach (var item in dict)
			{
				action(item.Value, item.Key);
			}
		}

		public static List<object> keys(Dictionary<object, object> dict)
		{
			return dict.Keys.ToList();
		}

		public static List<object> values(Dictionary<object, object> dict)
		{
			return dict.Values.ToList();
		}

		public static List<List<object>> entries(Dictionary<object, object> dict)
		{
			return dict.Select(a => new List<object> { a.Key, a.Value }).ToList();
		}

		public static long get_size(Dictionary<object, object> dict)
		{
			if (dict == null) return 0L;
			return dict.Count;
		}
	}
}
