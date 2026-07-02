using Newtonsoft.Json;
using System;
using System.Text;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptConsoleExtensions
	{
		public static void console_log(params object[] logs)
		{
			if (logs == null || logs.Length == 0)
			{
				Console.WriteLine();
				return;
			}

			var sb = new StringBuilder();
			for (int i = 0; i < logs.Length; i++)
			{
				if (i > 0)
					sb.Append(',');
				sb.Append(FormatValue(logs[i]));
			}
			Console.WriteLine(sb.ToString());
		}

		private static string FormatValue(object value)
		{
			if (value == null)
				return "null";

			if (value is bool b)
				return b ? "true" : "false";

			if (value is string s)
				return s;

			if (Type.GetTypeCode(value.GetType()) == TypeCode.Object)
			{
				return JsonConvert.SerializeObject(value);
			}

			return value.ToString();
		}
	}
}
