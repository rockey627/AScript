using System;
using System.Threading;

namespace AScript.Lang.Go.Extensions
{
	public static class TimeExtensions
	{
		public static DateTime time_Now() => DateTime.Now;

		public static void time_Sleep(int ms)
		{
			Thread.Sleep(ms);
		}
	}
}
