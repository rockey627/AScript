using System;
using System.Collections;
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
					if (i > 0) Console.Write("\t");
					var v = args[i];
					if (v == null)
					{
						Console.Write("nil");
						continue;
					}
					if (v is IList list)
					{
						for (int j = 0; j < list.Count; j++)
						{
							if (j > 0) Console.Write("\t");
							Console.Write(tostring(list[j]));
						}
						continue;
					}
					var type = v.GetType();
					string typeName = type.Name;
					if (typeName.StartsWith("Tuple`"))
					{
						var properties = type.GetProperties();
						for (int j = 0; j < properties.Length; j++)
						{
							if (j > 0) Console.Write("\t");
							Console.Write(tostring(properties[j].GetValue(v)));
						}
					}
					else if (typeName.StartsWith("ValueTuple`"))
					{
						var fields = type.GetFields();
						for (int j = 0; j < fields.Length; j++)
						{
							if (j > 0) Console.Write("\t");
							Console.Write(tostring(fields[j].GetValue(v)));
						}
					}
					else
					{
						Console.Write(tostring(v));
					}
				}
			}
			Console.WriteLine();
		}

		public static string tostring(object obj)
		{
			if (obj == null) return "nil";
			if (obj is bool b) return b ? "true" : "false";
			return obj.ToString();
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

		public static LuaTable setmetatable(LuaTable table, LuaTable metatable)
		{
			table.Metatable = metatable;
			return table;
		}

		public static LuaTable getmetatable(LuaTable table)
		{
			return table.Metatable;
		}

		public static object[] pcall(Delegate func, params object[] args)
		{
			try
			{
				var result = func.DynamicInvoke(args);
				return new object[] { true, result };
			}
			catch (Exception ex)
			{
				return new object[] { false, ex.Message };
			}
		}
	}
}
