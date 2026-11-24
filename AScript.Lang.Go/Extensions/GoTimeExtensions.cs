using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace AScript.Lang.Go.Extensions
{
	public static class GoTimeExtensions
	{
		private static readonly int _Second = 1000;
		private static readonly int _Minute = 60000;
		private static readonly int _Hour = 60 * _Minute;
		private static readonly int _Day = 24 * _Hour;

		public static DateTime time_Now() => DateTime.Now;

		public static int time_get_Day() => _Day;
		public static int time_get_Hour() => _Hour;
		public static int time_get_Minute() => _Minute;
		public static int time_get_Second() => _Second;

#if NETFRAMEWORK
		public static Tuple<DateTime, string> time_Parse(string timeString)
#else
		public static (DateTime, string) time_Parse(string timeString)
#endif
		{
			return time_Parse(null, timeString);
		}

#if NETFRAMEWORK
		public static Tuple<DateTime, string> time_Parse(string format, string timeString)
		{
			if (string.IsNullOrEmpty(format))
			{
				if (DateTime.TryParse(timeString, out var t))
				{
					return Tuple.Create<DateTime, string>(t, null);
				}
				return Tuple.Create<DateTime, string>(default, "time format error");
			}
			string netFormat = GoTimeUtils.ToNetFormat(format);
			if (DateTime.TryParseExact(timeString, netFormat, System.Globalization.CultureInfo.CurrentCulture, DateTimeStyles.None, out var time))
			{
				return Tuple.Create<DateTime, string>(time, null);
			}
			return Tuple.Create<DateTime, string>(default, "time format error");
		}
#else
		public static (DateTime, string) time_Parse(string format, string timeString)
		{
			if (string.IsNullOrEmpty(format))
			{
				if (DateTime.TryParse(timeString, out var t))
				{
					return (t, null);
				}
				return (default, "time format error");
			}
			string netFormat = GoTimeUtils.ToNetFormat(format);
			if (DateTime.TryParseExact(timeString, netFormat, System.Globalization.CultureInfo.CurrentCulture, DateTimeStyles.None, out var time))
			{
				return (time, null);
			}
			return (default, "time format error");
		}
#endif

		public static void time_Sleep(int ms)
		{
			Thread.Sleep(ms);
		}

		public static int Year(DateTime dateTime) => dateTime.Year;
		public static int Month(DateTime dateTime) => dateTime.Month;
		public static int Day(DateTime dateTime) => dateTime.Day;
		public static int Hour(DateTime dateTime) => dateTime.Hour;
		public static int Minute(DateTime dateTime) => dateTime.Minute;
		public static int Second(DateTime dateTime) => dateTime.Second;

		public static DateTime Add(DateTime dateTime, int ms) => dateTime.AddMilliseconds(ms);
		public static TimeSpan Sub(DateTime t1, DateTime t2) => t1 - t2;

		public static bool Before(DateTime t1, DateTime t2) => t1 < t2;
		public static bool After(DateTime t1, DateTime t2) => t1 > t2;
		public static bool Equal(DateTime t1, DateTime t2) => t1 == t2;

		/// <summary>
		/// 按 Go 的时间格式格式化时间，例如 Format(t, "2006-01-02 15:04:05")
		/// </summary>
		public static string Format(DateTime dateTime, string format)
		{
			if (string.IsNullOrEmpty(format)) return string.Empty;
			return dateTime.ToString(GoTimeUtils.ToNetFormat(format, dateTime.Kind), CultureInfo.InvariantCulture);
		}
	}
}
