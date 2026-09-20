using System;

namespace AScript.Lang.Go.Extensions
{
	public class strconv
	{
#if NET45
		public static Tuple<int, string> Atoi(string s)
		{
			if (int.TryParse(s, out var v))
			{
				return Tuple.Create<int, string>(v, null);
			}
			return Tuple.Create(0, $"strconv.Atoi: parsing \"{s}\": invalid syntax");
		}
#else
		public static (int, string) Atoi(string s)
		{
			if (int.TryParse(s, out var v))
			{
				return (v, null);
			}
			return (0, $"strconv.Atoi: parsing \"{s}\": invalid syntax");
		}
#endif
	}
}
