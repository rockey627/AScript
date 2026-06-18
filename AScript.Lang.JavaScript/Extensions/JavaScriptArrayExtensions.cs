using System;
using System.Collections.Generic;

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
	}
}
