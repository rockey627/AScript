using System;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Lang.Lua.Extensions
{
	public static class LuaTableExtensions
	{
		public static object table_remove(Dictionary<object, object> table, object key)
		{
			if (table.TryGetValue(key, out var value))
			{
				table.Remove(key);
				return value;
			}
			return null;
		}

		public static
#if NET45
			IEnumerable<Tuple<long, object>>
#else
			IEnumerable<(long, object)>
#endif
			ipairs(object tableObj)
		{
			var table = (IDictionary<object, object>)tableObj;
			long i = 1L;
			var keys = table.Keys
				.Where(a => a is int || a is long)
				.Select(a => new { Key = a, Value = Convert.ToInt64(a) })
				.OrderBy(a => a.Value)
				.ToList();
			foreach (var key in keys)
			{
				if (key.Value == i)
				{
#if NET45
					yield return Tuple.Create(i++, table[key.Key]);
#else
					yield return (i++, table[key.Key]);
#endif
				}
				//break;
			}
		}
	}
}
