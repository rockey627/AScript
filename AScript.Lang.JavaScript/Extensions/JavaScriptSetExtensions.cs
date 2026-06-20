using System;
using System.Collections.Generic;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptSetExtensions
	{
		public static HashSet<object> new_Set()
		{
			return new HashSet<object>();
		}

		public static HashSet<object> new_Set(List<object> list)
		{
			return new HashSet<object>(list);
		}

		public static long get_size(HashSet<object> set)
		{
			if (set == null) return 0L;
			return set.Count;
		}

		public static HashSet<object> add(HashSet<object> set, object value)
		{
			set.Add(value);
			return set;
		}

		public static bool delete(HashSet<object> set, object value)
		{
			return set.Remove(value);
		}

		public static bool has(HashSet<object> set, object value)
		{
			return set.Contains(value);
		}

		public static void forEach(HashSet<object> set, Action<object> action)
		{
			foreach (var item in set)
			{
				action(item);
			}
		}

		public static void clear(HashSet<object> set)
		{
			set.Clear();
		}
	}
}
