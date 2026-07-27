using System;
using System.Collections.Generic;

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
					Console.Write(args[i]);
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
	}
}
