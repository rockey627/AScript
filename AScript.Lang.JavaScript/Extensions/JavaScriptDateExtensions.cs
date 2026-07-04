using System;
using System.Globalization;
using System.Linq.Expressions;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptDateExtensions
	{
		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static Expression<Func<DateTime>> new_Date()
		{
			return () => DateTime.Now;
		}

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month">0 ~ 11</param>
		/// <param name="day"></param>
		/// <returns></returns>
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
		public static long Date_now()
		{
			return (long)(DateTime.Now - _epoch).TotalMilliseconds;
		}

		public static long Date_parse(string dateString)
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

		public static long getFullYear(DateTime time)
		{
			return time.Year;
		}

		public static long getHours(DateTime time)
		{
			return time.Hour;
		}

		public static long getMilliseconds(DateTime time)
		{
			return time.Millisecond;
		}

		public static long getMinutes(DateTime time)
		{
			return time.Minute;
		}

		public static long getMonth(DateTime time)
		{
			return time.Month - 1;
		}

		public static long getSeconds(DateTime time)
		{
			return time.Second;
		}

		public static long getTime(DateTime time)
		{
			return (long)(time.ToUniversalTime() - _epoch).TotalMilliseconds;
		}

		public static long getTimezoneOffset(DateTime time)
		{
			return (long)(time - time.ToUniversalTime()).TotalMinutes;
		}

		public static long getYear(DateTime time)
		{
			return time.Year - 1900;
		}

		// UTC 版本
		public static long getUTCDate(DateTime time)
		{
			return time.ToUniversalTime().Day;
		}

		public static long getUTCDay(DateTime time)
		{
			return (int)time.ToUniversalTime().DayOfWeek;
		}

		public static long getUTCFullYear(DateTime time)
		{
			return time.ToUniversalTime().Year;
		}

		public static long getUTCHours(DateTime time)
		{
			return time.ToUniversalTime().Hour;
		}

		public static long getUTCMilliseconds(DateTime time)
		{
			return time.ToUniversalTime().Millisecond;
		}

		public static long getUTCMinutes(DateTime time)
		{
			return time.ToUniversalTime().Minute;
		}

		public static long getUTCMonth(DateTime time)
		{
			return time.ToUniversalTime().Month - 1;
		}

		public static long getUTCSeconds(DateTime time)
		{
			return time.ToUniversalTime().Second;
		}

		// setter 方法
		public static long setDate(DateTime time, long day)
		{
			time = time.AddDays(day - time.Day);
			return getTime(time);
		}

		public static long setFullYear(DateTime time, long year)
		{
			time = new DateTime((int)year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
			return getTime(time);
		}

		public static long setFullYear(DateTime time, long year, long month)
		{
			return setFullYear(time, year, month, time.Day);
		}

		public static long setFullYear(DateTime time, long year, long month, long day)
		{
			time = new DateTime((int)year, (int)month + 1, (int)day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
			return getTime(time);
		}

		public static long setHours(DateTime time, long hour)
		{
			return setHours(time, hour, time.Minute, time.Second, time.Millisecond);
		}

		public static long setHours(DateTime time, long hour, long minute)
		{
			return setHours(time, hour, minute, time.Second, time.Millisecond);
		}

		public static long setHours(DateTime time, long hour, long minute, long second)
		{
			return setHours(time, hour, minute, second, time.Millisecond);
		}

		public static long setHours(DateTime time, long hour, long minute, long second, long millisecond)
		{
			time = new DateTime(time.Year, time.Month, time.Day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return getTime(time);
		}

		public static long setMilliseconds(DateTime time, long millisecond)
		{
			time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, (int)millisecond, DateTimeKind.Local);
			return getTime(time);
		}

		public static long setMinutes(DateTime time, long minute)
		{
			return setMinutes(time, minute, time.Second, time.Millisecond);
		}

		public static long setMinutes(DateTime time, long minute, long second)
		{
			return setMinutes(time, minute, second, time.Millisecond);
		}

		public static long setMinutes(DateTime time, long minute, long second, long millisecond)
		{
			time = new DateTime(time.Year, time.Month, time.Day, time.Hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return getTime(time);
		}

		public static long setMonth(DateTime time, long month)
		{
			return setMonth(time, month, time.Day);
		}

		public static long setMonth(DateTime time, long month, long day)
		{
			time = new DateTime(time.Year, (int)month + 1, (int)day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
			return getTime(time);
		}

		public static long setSeconds(DateTime time, long second)
		{
			return setSeconds(time, second, time.Millisecond);
		}

		public static long setSeconds(DateTime time, long second, long millisecond)
		{
			time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return getTime(time);
		}

		public static long setTime(DateTime time, long milliseconds)
		{
			time = _epoch.AddMilliseconds(milliseconds).ToLocalTime();
			return getTime(time);
		}

		public static long setYear(DateTime time, long year)
		{
			long fullYear = year < 100L ? year + 1900L : year;
			return setFullYear(time, fullYear);
		}

		// UTC setter 版本
		public static long setUTCDate(DateTime time, long day)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return getTime(time);
		}

		public static long setUTCFullYear(DateTime time, long year)
		{
			return setUTCFullYear(time, year, time.Month - 1, time.Day);
		}

		public static long setUTCFullYear(DateTime time, long year, long month)
		{
			return setUTCFullYear(time, year, month, time.Day);
		}

		public static long setUTCFullYear(DateTime time, long year, long month, long day)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime((int)year, (int)month + 1, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return getTime(time);
		}

		public static long setUTCHours(DateTime time, long hour)
		{
			return setUTCHours(time, hour, time.Minute, time.Second, time.Millisecond);
		}

		public static long setUTCHours(DateTime time, long hour, long minute)
		{
			return setUTCHours(time, hour, minute, time.Second, time.Millisecond);
		}

		public static long setUTCHours(DateTime time, long hour, long minute, long second)
		{
			return setUTCHours(time, hour, minute, second, time.Millisecond);
		}

		public static long setUTCHours(DateTime time, long hour, long minute, long second, long millisecond)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return getTime(time);
		}

		public static long setUTCMilliseconds(DateTime time, long millisecond)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second, (int)millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return getTime(time);
		}

		public static long setUTCMinutes(DateTime time, long minute)
		{
			return setUTCMinutes(time, minute, time.Second, time.Millisecond);
		}

		public static long setUTCMinutes(DateTime time, long minute, long second)
		{
			return setUTCMinutes(time, minute, second, time.Millisecond);
		}

		public static long setUTCMinutes(DateTime time, long minute, long second, long millisecond)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return getTime(time);
		}

		public static long setUTCMonth(DateTime time, long month)
		{
			return setUTCMonth(time, month, time.Day);
		}

		public static long setUTCMonth(DateTime time, long month, long day)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, (int)month + 1, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return getTime(time);
		}

		public static long setUTCSeconds(DateTime time, long second)
		{
			return setUTCSeconds(time, second, time.Millisecond);
		}

		public static long setUTCSeconds(DateTime time, long second, long millisecond)
		{
			var utc = time.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			time = utc.ToLocalTime();
			return getTime(time);
		}

		// 转换方法
		public static string toDateString(DateTime time)
		{
			return time.ToString("ddd MMM dd yyyy", CultureInfo.InvariantCulture);
		}

		public static string toISOString(DateTime time)
		{
			return time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
		}

		public static string toJSON(DateTime time)
		{
			return toISOString(time);
		}

		public static string toLocaleDateString(DateTime time)
		{
			return time.ToLocalTime().ToString("D", CultureInfo.CurrentCulture);
		}

		public static string toLocaleTimeString(DateTime time)
		{
			return time.ToLocalTime().ToString("T", CultureInfo.CurrentCulture);
		}

		public static string toLocaleString(DateTime time)
		{
			return time.ToLocalTime().ToString("G", CultureInfo.CurrentCulture);
		}

		public static string toTimeString(DateTime time)
		{
			return time.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
		}

		public static string toUTCString(DateTime time)
		{
			return time.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture);
		}

		public static long valueOf(DateTime time)
		{
			return getTime(time);
		}

		public static string toString(DateTime time)
		{
			return time.ToString("ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
		}

		public static string toString(DateTime time, string format)
		{
			return time.ToString(format);
		}
	}
}