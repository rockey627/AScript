using System;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Lang.Lua.Extensions
{
	public static class LuaCommonExtensions
	{
		public static void print(params object[] args)
		{
			if (args != null && args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (i > 0) Console.Write(' ');
					Console.Write(args[i] ?? "nil");
				}
			}
			Console.WriteLine();
		}

		public static string type(object obj)
		{
			if (obj == null) return "nil";
			if (obj is bool) return "boolean";
			if (obj is int || obj is long || obj is double) return "number";
			if (obj is string) return "string";
			else if (obj is IDictionary<object, object>) return "table";
			else if (obj is Delegate) return "function";
			return "userdata";
		}

		public static
#if NET45
			IEnumerable<Tuple<long, object>>
#else
			IEnumerable<(long, object)>
#endif
			ipairs(object tableObj)
		{
			var table = (LuaTable)tableObj;
			for (int i = 0; i < table.Array.Count; i++)
			{
				if (table.Array[i] == null) break;
#if NET45
				yield return Tuple.Create((long)(i + 1), table.Array[i]);
#else
				yield return ((long)(i + 1), table.Array[i]);
#endif
			}
		}
	}
}
