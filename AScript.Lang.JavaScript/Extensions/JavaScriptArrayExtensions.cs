using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptArrayExtensions
	{
		public static List<object> new_Array(long length)
		{
			var list = new List<object>((int)length);
			for (int i = 0; i < length; i++)
			{
				list.Add(null);
			}
			return list;
		}

		public static List<object> new_Array(params object[] values)
		{
			return new List<object>(values);
		}

		public static LambdaExpression[] join()
		{
			Expression<Func<List<object>, string>> join1 = list => string.Join("", list);
			Expression<Func<List<object>, string, string>> join2 = (list, separator) => string.Join(separator, list);
			return new LambdaExpression[] { join1, join2 };
		}

		public static Expression<Func<List<object>, object, long>> indexOf() => (list, obj) => (long)list.IndexOf(obj);

		public static List<object> reverse(List<object> list)
		{
			list.Reverse();
			return list;
		}

		public static List<object> fill(List<object> list, object v)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = v;
			}
			return list;
		}

		public static List<object> filter(List<object> list, Func<object, bool> func)
		{
			return list.Where(func).ToList();
		}

		public static long findIndex(List<object> list, Func<object, bool> func)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (func(list[i])) return i;
			}
			return -1L;
		}

		public static List<object> map(List<object> list, Func<object, object> func)
		{
			return list.Select(func).ToList();
		}

		public static object reduce(List<object> list, Func<object, object, object> func)
		{
			if (list.Count == 0) return null;
			if (list.Count == 1) return list[0];
			object r = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				r = func(r, list[i]);
			}
			return r;
		}

		public static object reduce(List<object> list, Func<object, object, object> func, object init)
		{
			if (list.Count == 0) return init;
			object r = init;
			for (int i = 0; i < list.Count; i++)
			{
				r = func(r, list[i]);
			}
			return r;
		}

		public static void forEach(List<object> list, Action<object> action)
		{
			list.ForEach(action);
		}

		public static object pop(List<object> list)
		{
			if (list == null || list.Count == 0) return null;
			int index = list.Count - 1;
			object v = list[index];
			list.RemoveAt(index);
			return v;
		}

		public static object shift(List<object> list)
		{
			if (list == null || list.Count == 0) return null;
			object v = list[0];
			list.RemoveAt(0);
			return v;
		}

		public static long push(List<object> list, object v)
		{
			list.Add(v);
			return list.Count;
		}

		public static long push(List<object> list, params object[] vs)
		{
			if (vs == null || vs.Length == 0) return list.Count;
			if (vs.Length == 1) list.Add(vs[0]);
			else list.AddRange(vs);
			return list.Count;
		}

		public static long unshift(List<object> list, object v)
		{
			list.Insert(0, v);
			return list.Count;
		}

		public static long unshift(List<object> list, params object[] vs)
		{
			if (vs == null || vs.Length == 0) return list.Count;
			if (vs.Length == 1) list.Insert(0, vs[0]);
			else list.InsertRange(0, vs);
			return list.Count;
		}

		public static List<object> splice(List<object> list, long index)
		{
			int count = list.Count - (int)index;
			return splice(list, index, count);
		}

		public static List<object> splice(List<object> list, long index, long count)
		{
			if (count == 0) return new List<object>();
			var result = new List<object>((int)count);
			for (int i = 0; i < count; i++)
			{
				result.Add(list[i + (int)index]);
			}
			list.RemoveRange((int)index, (int)count);
			return result;
		}

		public static List<object> splice(List<object> list, long index, long count, params object[] addingList)
		{
			var result = splice(list, index, count);
			if (addingList != null && addingList.Length > 0)
			{
				list.InsertRange((int)index, addingList);
			}
			return result;
		}

		public static bool every(List<object> list, Func<object, bool> predicate)
		{
			return list.All(predicate);
		}

		public static bool some(List<object> list, Func<object, bool> predicate)
		{
			return list.Any(predicate);
		}

		public static object find(List<object> list, Func<object, bool> predicate)
		{
			return list.FirstOrDefault(predicate);
		}
	}
}
