using System;
using System.Globalization;

namespace AScript.Lang.JavaScript
{
	public static class JavaScriptDateExtensions
	{
		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime new_Date(long milliseconds)
		{
			return _epoch.AddMilliseconds(milliseconds).ToLocalTime();
		}

		public static DateTime new_Date(string dateString)
		{
			if (string.IsNullOrEmpty(dateString))
			{
				return DateTime.Now;
			}
			if (DateTime.TryParse(dateString, out var result))
			{
				return result.ToLocalTime();
			}
			else
			{
				throw new FormatException($"Invalid date string: {dateString}");
			}
		}

		public static DateTime new_Date(long year, long month, long day)
		{
			return new DateTime((int)year, (int)month + 1, (int)day, 0, 0, 0, DateTimeKind.Local);
		}

		public static DateTime new_Date(long year, long month, long day, long hour, long minute, long second)
		{
			return new DateTime((int)year, (int)month + 1, (int)day, (int)hour, (int)minute, (int)second, DateTimeKind.Local);
		}

		public static DateTime new_Date(long year, long month, long day, long hour, long minute, long second, long millisecond)
		{
			return new DateTime((int)year, (int)month + 1, (int)day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Local);
		}

		// 静态方法
		public static long Date_Now()
		{
			return (long)(DateTime.Now - _epoch).TotalMilliseconds;
		}

		public static long Date_Parse(string dateString)
		{
			if (DateTime.TryParse(dateString, out var result))
			{
				return (long)(result.ToUniversalTime() - _epoch).TotalMilliseconds;
			}
			return -1L;
		}

		public static long Date_UTC(long year, long month, long day)
		{
			return Date_UTC(year, month, day, 0, 0, 0, 0);
		}

		public static long Date_UTC(long year, long month, long day, long hour, long minute, long second)
		{
			return Date_UTC(year, month, day, hour, minute, second, 0);
		}

		public static long Date_UTC(long year, long month, long day, long hour, long minute, long second, long millisecond)
		{
			var dt = new DateTime((int)year, (int)month + 1, (int)day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			return (long)(dt - _epoch).TotalMilliseconds;
		}

		// 实例方法 - getter
		public static long getDate(DateTime time)
		{
			return time.Day;
		}

		public static long getDay(DateTime time)
		{
			return (int)time.DayOfWeek;
		}

		public static long GetFullYear(DateTime time)
		{
			return time.Year;
		}

		public static long GetHours(DateTime time)
		{
			return time.Hour;
		}

		public static long GetMilliseconds(DateTime time)
		{
			return time.Millisecond;
		}

		public static long GetMinutes(DateTime time)
		{
			return time.Minute;
		}

		public static long GetMonth(DateTime time)
		{
			return time.Month - 1;
		}

		public static long GetSeconds(DateTime time)
		{
			return time.Second;
		}

		public static long GetTime(DateTime time)
		{
			return (long)(time.ToUniversalTime() - _epoch).TotalMilliseconds;
		}

		public static long GetTimezoneOffset(DateTime time)
		{
			return (long)(time - time.ToUniversalTime()).TotalMinutes;
		}

		public static long GetYear(DateTime time)
		{
			return time.Year - 1900;
		}

		// UTC 版本
		public static long GetUTCDate(DateTime time)
		{
			return time.ToUniversalTime().Day;
		}

		public static long GetUTCDay(DateTime time)
		{
			return (int)time.ToUniversalTime().DayOfWeek;
		}

		public static long GetUTCFullYear(DateTime time)
		{
			return time.ToUniversalTime().Year;
		}

		public static long GetUTCHours(DateTime time)
		{
			return time.ToUniversalTime().Hour;
		}

		public static long GetUTCMilliseconds(DateTime time)
		{
			return time.ToUniversalTime().Millisecond;
		}

		public static long GetUTCMinutes(DateTime time)
		{
			return time.ToUniversalTime().Minute;
		}

		public static long GetUTCMonth(DateTime time)
		{
			return time.ToUniversalTime().Month - 1;
		}

		public static long GetUTCSeconds(DateTime time)
		{
			return time.ToUniversalTime().Second;
		}

		// setter 方法
		public static long SetDate(DateTime time, long day)
		{
			time = time.AddDays(day - time.Day);
			return GetTime(time);
		}

		public static long SetFullYear(DateTime time, long year)
		{
			time = new DateTime((int)year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
			return GetTime(time);
		}

		public static long SetFullYear(DateTime time, long year, long month)
		{
			return SetFullYear(time, year, month, time.Day);
		}

		public static long SetFullYear(DateTime time, long year, long month, long day)
		{
			time = new DateTime((int)year, (int)month + 1, (int)day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
			return GetTime(time);
		}

		public static long SetHours(DateTime time, long hour)
		{
			return SetHours(time, hour, time.Minute, time.Second, time.Millisecond);
		}

		public static long SetHours(DateTime time, long hour, long minute)
		{
			return SetHours(time, hour, minute, time.Second, time.Millisecond);
		}

		public static long SetHours(DateTime time, long hour, long minute, long second)
		{
			return SetHours(time, hour, minute, second, time.Millisecond);
		}

		public static long SetHours(DateTime time, long hour, long minute, long second, long millisecond)
		{
			time = new DateTime(time.Year, time.Month, time.Day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return GetTime(time);
		}

		public static long SetMilliseconds(DateTime time, long millisecond)
		{
			time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, (int)millisecond, DateTimeKind.Local);
			return GetTime(time);
		}

		public static long SetMinutes(DateTime time, long minute)
		{
			return SetMinutes(time, minute, time.Second, time.Millisecond);
		}

		public static long SetMinutes(DateTime time, long minute, long second)
		{
			return SetMinutes(time, minute, second, time.Millisecond);
		}

		public static long SetMinutes(DateTime time, long minute, long second, long millisecond)
		{
			time = new DateTime(time.Year, time.Month, time.Day, time.Hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return GetTime(time);
		}

		public static long SetMonth(DateTime time, long month)
		{
			return SetMonth(time, month, time.Day);
		}

		public static long SetMonth(DateTime time, long month, long day)
		{
			time = new DateTime(time.Year, (int)month + 1, (int)day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
			return GetTime(time);
		}

		public static long SetSeconds(DateTime time, long second)
		{
			return SetSeconds(time, second, time.Millisecond);
		}

		public static long SetSeconds(DateTime time, long second, long millisecond)
		{
			time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return GetTime(time);
		}

		public static long SetTime(DateTime time, long milliseconds)
		{
			time = _epoch.AddMilliseconds(milliseconds).ToLocalTime();
			return GetTime(time);
		}

		public static long SetYear(DateTime time, long year)
		{
			long fullYear = year < 100L ? year + 1900L : year;
			return SetFullYear(time, fullYear);
		}

		// UTC setter 版本
		public static long SetUTCDate(DateTime time, long day)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return GetTime(time);
		}

		public static long SetUTCFullYear(DateTime time, long year)
		{
			return SetUTCFullYear(time, year, time.Month - 1, time.Day);
		}

		public static long SetUTCFullYear(DateTime time, long year, long month)
		{
			return SetUTCFullYear(time, year, month, time.Day);
		}

		public static long SetUTCFullYear(DateTime time, long year, long month, long day)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime((int)year, (int)month + 1, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return GetTime(time);
		}

		public static long SetUTCHours(DateTime time, long hour)
		{
			return SetUTCHours(time, hour, time.Minute, time.Second, time.Millisecond);
		}

		public static long SetUTCHours(DateTime time, long hour, long minute)
		{
			return SetUTCHours(time, hour, minute, time.Second, time.Millisecond);
		}

		public static long SetUTCHours(DateTime time, long hour, long minute, long second)
		{
			return SetUTCHours(time, hour, minute, second, time.Millisecond);
		}

		public static long SetUTCHours(DateTime time, long hour, long minute, long second, long millisecond)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return GetTime(time);
		}

		public static long SetUTCMilliseconds(DateTime time, long millisecond)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second, (int)millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return GetTime(time);
		}

		public static long SetUTCMinutes(DateTime time, long minute)
		{
			return SetUTCMinutes(time, minute, time.Second, time.Millisecond);
		}

		public static long SetUTCMinutes(DateTime time, long minute, long second)
		{
			return SetUTCMinutes(time, minute, second, time.Millisecond);
		}

		public static long SetUTCMinutes(DateTime time, long minute, long second, long millisecond)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return GetTime(time);
		}

		public static long SetUTCMonth(DateTime time, long month)
		{
			return SetUTCMonth(time, month, time.Day);
		}

		public static long SetUTCMonth(DateTime time, long month, long day)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, (int)month + 1, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return GetTime(time);
		}

		public static long SetUTCSeconds(DateTime time, long second)
		{
			return SetUTCSeconds(time, second, time.Millisecond);
		}

		public static long SetUTCSeconds(DateTime time, long second, long millisecond)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return GetTime(time);
		}

		// 转换方法
		public static string ToDateString(DateTime time)
		{
			return time.ToString("ddd MMM dd yyyy", CultureInfo.InvariantCulture);
		}

		public static string ToISOString(DateTime time)
		{
			return time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
		}

		public static string ToJSON(DateTime time)
		{
			return ToISOString(time);
		}

		public static string ToLocaleDateString(DateTime time)
		{
			return time.ToLocalTime().ToString("D", CultureInfo.CurrentCulture);
		}

		public static string ToLocaleTimeString(DateTime time)
		{
			return time.ToLocalTime().ToString("T", CultureInfo.CurrentCulture);
		}

		public static string ToLocaleString(DateTime time)
		{
			return time.ToLocalTime().ToString("G", CultureInfo.CurrentCulture);
		}

		public static string ToTimeString(DateTime time)
		{
			return time.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
		}

		public static string ToUTCString(DateTime time)
		{
			return time.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture);
		}

		public static long ValueOf(DateTime time)
		{
			return GetTime(time);
		}

		public static string ToString(DateTime time)
		{
			return time.ToString("ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
		}
	}
}