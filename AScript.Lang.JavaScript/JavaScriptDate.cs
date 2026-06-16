using System;
using System.Globalization;

namespace AScript.Lang.JavaScript
{
	public class JavaScriptDate
	{
		private DateTime _dateTime;
		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public JavaScriptDate(long milliseconds)
		{
			_dateTime = _epoch.AddMilliseconds(milliseconds).ToLocalTime();
		}

		public JavaScriptDate(string dateString)
		{
			if (string.IsNullOrEmpty(dateString))
			{
				_dateTime = DateTime.Now;
				return;
			}
			if (DateTime.TryParse(dateString, out var result))
			{
				_dateTime = result.ToLocalTime();
			}
			else
			{
				throw new FormatException($"Invalid date string: {dateString}");
			}
		}

		public JavaScriptDate(long year, long month, long day)
		{
			_dateTime = new DateTime((int)year, (int)month + 1, (int)day, 0, 0, 0, DateTimeKind.Local);
		}

		public JavaScriptDate(long year, long month, long day, long hour, long minute, long second)
		{
			_dateTime = new DateTime((int)year, (int)month + 1, (int)day, (int)hour, (int)minute, (int)second, DateTimeKind.Local);
		}

		public JavaScriptDate(long year, long month, long day, long hour, long minute, long second, long millisecond)
		{
			_dateTime = new DateTime((int)year, (int)month + 1, (int)day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Local);
		}

		// 静态方法
		public static long Now()
		{
			return (long)(DateTime.Now - _epoch).TotalMilliseconds;
		}

		public static long Parse(string dateString)
		{
			if (DateTime.TryParse(dateString, out var result))
			{
				return (long)(result.ToUniversalTime() - _epoch).TotalMilliseconds;
			}
			return -1L;
		}

		public static long UTC(long year, long month, long day)
		{
			return UTC(year, month, day, 0, 0, 0, 0);
		}

		public static long UTC(long year, long month, long day, long hour, long minute, long second)
		{
			return UTC(year, month, day, hour, minute, second, 0);
		}

		public static long UTC(long year, long month, long day, long hour, long minute, long second, long millisecond)
		{
			var dt = new DateTime((int)year, (int)month + 1, (int)day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			return (long)(dt - _epoch).TotalMilliseconds;
		}

		// 实例方法 - getter
		public long GetDate()
		{
			return _dateTime.Day;
		}

		public long GetDay()
		{
			return (int)_dateTime.DayOfWeek;
		}

		public long GetFullYear()
		{
			return _dateTime.Year;
		}

		public long GetHours()
		{
			return _dateTime.Hour;
		}

		public long GetMilliseconds()
		{
			return _dateTime.Millisecond;
		}

		public long GetMinutes()
		{
			return _dateTime.Minute;
		}

		public long GetMonth()
		{
			return _dateTime.Month - 1;
		}

		public long GetSeconds()
		{
			return _dateTime.Second;
		}

		public long GetTime()
		{
			return (long)(_dateTime.ToUniversalTime() - _epoch).TotalMilliseconds;
		}

		public long GetTimezoneOffset()
		{
			return (long)(_dateTime - _dateTime.ToUniversalTime()).TotalMinutes;
		}

		public long GetYear()
		{
			return _dateTime.Year - 1900;
		}

		// UTC 版本
		public long GetUTCDate()
		{
			return _dateTime.ToUniversalTime().Day;
		}

		public long GetUTCDay()
		{
			return (int)_dateTime.ToUniversalTime().DayOfWeek;
		}

		public long GetUTCFullYear()
		{
			return _dateTime.ToUniversalTime().Year;
		}

		public long GetUTCHours()
		{
			return _dateTime.ToUniversalTime().Hour;
		}

		public long GetUTCMilliseconds()
		{
			return _dateTime.ToUniversalTime().Millisecond;
		}

		public long GetUTCMinutes()
		{
			return _dateTime.ToUniversalTime().Minute;
		}

		public long GetUTCMonth()
		{
			return _dateTime.ToUniversalTime().Month - 1;
		}

		public long GetUTCSeconds()
		{
			return _dateTime.ToUniversalTime().Second;
		}

		// setter 方法
		public long SetDate(long day)
		{
			_dateTime = _dateTime.AddDays(day - _dateTime.Day);
			return GetTime();
		}

		public long SetFullYear(long year)
		{
			_dateTime = new DateTime((int)year, _dateTime.Month, _dateTime.Day, _dateTime.Hour, _dateTime.Minute, _dateTime.Second, _dateTime.Millisecond, DateTimeKind.Local);
			return GetTime();
		}

		public long SetFullYear(long year, long month)
		{
			return SetFullYear(year, month, _dateTime.Day);
		}

		public long SetFullYear(long year, long month, long day)
		{
			_dateTime = new DateTime((int)year, (int)month + 1, (int)day, _dateTime.Hour, _dateTime.Minute, _dateTime.Second, _dateTime.Millisecond, DateTimeKind.Local);
			return GetTime();
		}

		public long SetHours(long hour)
		{
			return SetHours(hour, _dateTime.Minute, _dateTime.Second, _dateTime.Millisecond);
		}

		public long SetHours(long hour, long minute)
		{
			return SetHours(hour, minute, _dateTime.Second, _dateTime.Millisecond);
		}

		public long SetHours(long hour, long minute, long second)
		{
			return SetHours(hour, minute, second, _dateTime.Millisecond);
		}

		public long SetHours(long hour, long minute, long second, long millisecond)
		{
			_dateTime = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return GetTime();
		}

		public long SetMilliseconds(long millisecond)
		{
			_dateTime = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, _dateTime.Hour, _dateTime.Minute, _dateTime.Second, (int)millisecond, DateTimeKind.Local);
			return GetTime();
		}

		public long SetMinutes(long minute)
		{
			return SetMinutes(minute, _dateTime.Second, _dateTime.Millisecond);
		}

		public long SetMinutes(long minute, long second)
		{
			return SetMinutes(minute, second, _dateTime.Millisecond);
		}

		public long SetMinutes(long minute, long second, long millisecond)
		{
			_dateTime = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, _dateTime.Hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return GetTime();
		}

		public long SetMonth(long month)
		{
			return SetMonth(month, _dateTime.Day);
		}

		public long SetMonth(long month, long day)
		{
			_dateTime = new DateTime(_dateTime.Year, (int)month + 1, (int)day, _dateTime.Hour, _dateTime.Minute, _dateTime.Second, _dateTime.Millisecond, DateTimeKind.Local);
			return GetTime();
		}

		public long SetSeconds(long second)
		{
			return SetSeconds(second, _dateTime.Millisecond);
		}

		public long SetSeconds(long second, long millisecond)
		{
			_dateTime = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, _dateTime.Hour, _dateTime.Minute, (int)second, (int)millisecond, DateTimeKind.Local);
			return GetTime();
		}

		public long SetTime(long milliseconds)
		{
			_dateTime = _epoch.AddMilliseconds(milliseconds).ToLocalTime();
			return GetTime();
		}

		public long SetYear(long year)
		{
			long fullYear = year < 100L ? year + 1900L : year;
			return SetFullYear(fullYear);
		}

		// UTC setter 版本
		public long SetUTCDate(long day)
		{
			var utc = _dateTime.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			_dateTime = utc.ToLocalTime();
			return GetTime();
		}

		public long SetUTCFullYear(long year)
		{
			return SetUTCFullYear(year, _dateTime.Month - 1, _dateTime.Day);
		}

		public long SetUTCFullYear(long year, long month)
		{
			return SetUTCFullYear(year, month, _dateTime.Day);
		}

		public long SetUTCFullYear(long year, long month, long day)
		{
			var utc = _dateTime.ToUniversalTime();
			utc = new DateTime((int)year, (int)month + 1, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			_dateTime = utc.ToLocalTime();
			return GetTime();
		}

		public long SetUTCHours(long hour)
		{
			return SetUTCHours(hour, _dateTime.Minute, _dateTime.Second, _dateTime.Millisecond);
		}

		public long SetUTCHours(long hour, long minute)
		{
			return SetUTCHours(hour, minute, _dateTime.Second, _dateTime.Millisecond);
		}

		public long SetUTCHours(long hour, long minute, long second)
		{
			return SetUTCHours(hour, minute, second, _dateTime.Millisecond);
		}

		public long SetUTCHours(long hour, long minute, long second, long millisecond)
		{
			var utc = _dateTime.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, (int)hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			_dateTime = utc.ToLocalTime();
			return GetTime();
		}

		public long SetUTCMilliseconds(long millisecond)
		{
			var utc = _dateTime.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second, (int)millisecond, DateTimeKind.Utc);
			_dateTime = utc.ToLocalTime();
			return GetTime();
		}

		public long SetUTCMinutes(long minute)
		{
			return SetUTCMinutes(minute, _dateTime.Second, _dateTime.Millisecond);
		}

		public long SetUTCMinutes(long minute, long second)
		{
			return SetUTCMinutes(minute, second, _dateTime.Millisecond);
		}

		public long SetUTCMinutes(long minute, long second, long millisecond)
		{
			var utc = _dateTime.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, (int)minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			_dateTime = utc.ToLocalTime();
			return GetTime();
		}

		public long SetUTCMonth(long month)
		{
			return SetUTCMonth(month, _dateTime.Day);
		}

		public long SetUTCMonth(long month, long day)
		{
			var utc = _dateTime.ToUniversalTime();
			utc = new DateTime(utc.Year, (int)month + 1, (int)day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Utc);
			_dateTime = utc.ToLocalTime();
			return GetTime();
		}

		public long SetUTCSeconds(long second)
		{
			return SetUTCSeconds(second, _dateTime.Millisecond);
		}

		public long SetUTCSeconds(long second, long millisecond)
		{
			var utc = _dateTime.ToUniversalTime();
			utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, (int)second, (int)millisecond, DateTimeKind.Utc);
			_dateTime = utc.ToLocalTime();
			return GetTime();
		}

		// 转换方法
		public string ToDateString()
		{
			return _dateTime.ToString("ddd MMM dd yyyy", CultureInfo.InvariantCulture);
		}

		public string ToISOString()
		{
			return _dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
		}

		public string ToJSON()
		{
			return ToISOString();
		}

		public string ToLocaleDateString()
		{
			return _dateTime.ToLocalTime().ToString("D", CultureInfo.CurrentCulture);
		}

		public string ToLocaleTimeString()
		{
			return _dateTime.ToLocalTime().ToString("T", CultureInfo.CurrentCulture);
		}

		public string ToLocaleString()
		{
			return _dateTime.ToLocalTime().ToString("G", CultureInfo.CurrentCulture);
		}

		public string ToTimeString()
		{
			return _dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
		}

		public string ToUTCString()
		{
			return _dateTime.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture);
		}

		public long ValueOf()
		{
			return GetTime();
		}

		public override string ToString()
		{
			//return _dateTime.ToString("ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			return _dateTime.ToString();
		}

		// 隐式转换
		public static implicit operator DateTime(JavaScriptDate jsDate)
		{
			return jsDate._dateTime;
		}

		public static explicit operator JavaScriptDate(DateTime dt)
		{
			return new JavaScriptDate((long)(dt.ToLocalTime() - _epoch).TotalMilliseconds);
		}
	}
}