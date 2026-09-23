using System;

namespace AScript.Lang.Go.Extensions
{
	public class fmt
	{
		public static void Println(params object[] args)
		{
			if (args == null || args.Length == 0)
			{
				Console.WriteLine();
				return;
			}
			if (args.Length == 1)
			{
				Println(args[0]);
				return;
			}
			Console.WriteLine(string.Join(" ", args));
		}

		public static void Println()
		{
			Console.WriteLine();
		}

		public static void Println(object v1)
		{
			Console.WriteLine(v1);
		}

		public static void Println(object v1, object v2)
		{
			Console.WriteLine(v1?.ToString() + " " + v2?.ToString());
		}

		public static void Printf(string format, params object[] args)
		{
			Console.Write(Sprintf(format, args));
		}

		public static string Sprintf(string format, params object[] args)
		{
			int argIndex = 0;
			var netFormat = System.Text.RegularExpressions.Regex.Replace(format, @"%[sdfvq]", match => "{" + argIndex++ + "}");
			return string.Format(netFormat, args);
		}
	}
}
