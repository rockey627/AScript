using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace AScript.Lang.Lua
{
	/// <summary>
	/// lua os
	/// </summary>
	public class LuaOS
	{
		private static readonly DateTime _baseTime = new DateTime(1970, 1, 1);

		/// <summary>
		/// 返回当前时间的 Unix 时间戳（秒）
		/// </summary>
		public long time() => (long)DateTime.UtcNow.Subtract(_baseTime).TotalSeconds;

		/// <summary>
		/// 把时间表转换为 Unix 时间戳（秒），缺省字段按 lua 规则处理
		/// </summary>
		/// <param name="time">year、month、day、hour、min、sec 字段，缺省时取当天、12 点</param>
		public long time(LuaTable time)
		{
			var now = DateTime.Now;
			int year = GetInt(time, "year", now.Year);
			int month = GetInt(time, "month", now.Month);
			int day = GetInt(time, "day", now.Day);
			int hour = GetInt(time, "hour", 12);
			int min = GetInt(time, "min", 0);
			int sec = GetInt(time, "sec", 0);
			// 时间表按本地时间处理，再换算成 UTC 时间戳
			var local = new DateTime(year, month, day, hour, min, sec, DateTimeKind.Local);
			var offset = TimeZoneInfo.Local.GetUtcOffset(local);
			return (long)(local - offset - _baseTime).TotalSeconds;
		}

		/// <summary>
		/// 返回程序运行所消耗的 CPU 时间（秒）
		/// </summary>
		public double clock()
		{
			try
			{
				using (var process = Process.GetCurrentProcess())
				{
					return process.TotalProcessorTime.TotalSeconds;
				}
			}
			catch (Exception)
			{
				return 0;
			}
		}

		/// <summary>
		/// 以默认格式（%c）返回当前时间
		/// </summary>
		public object date() => date("%c");

		/// <summary>
		/// 按格式返回当前时间，格式以 *t 开头时返回时间表
		/// </summary>
		/// <param name="format">strftime 格式，*t 表示返回时间表，! 前缀表示 UTC 时间</param>
		public object date(string format) => FormatDate(format, DateTime.Now);

		/// <summary>
		/// 按格式返回指定时间，格式以 *t 开头时返回时间表
		/// </summary>
		/// <param name="format">strftime 格式，*t 表示返回时间表，! 前缀表示 UTC 时间</param>
		/// <param name="time">Unix 时间戳（秒）</param>
		public object date(string format, object time) => FormatDate(format, ToLocalDateTime(ToLong(time)));

		/// <summary>
		/// 计算两个时间戳的差值（秒）
		/// </summary>
		public double difftime(object t2, object t1) => ToLong(t2) - ToLong(t1);

		/// <summary>
		/// 获取环境变量的值，不存在时返回 nil
		/// </summary>
		public string getenv(string name) => Environment.GetEnvironmentVariable(name);

		///// <summary>
		///// 删除文件或目录，成功返回 true，失败返回 nil 和错误信息
		///// </summary>
		//public object remove(string filename)
		//{
		//	try
		//	{
		//		if (Directory.Exists(filename))
		//		{
		//			Directory.Delete(filename);
		//			return true;
		//		}
		//		if (File.Exists(filename))
		//		{
		//			File.Delete(filename);
		//			return true;
		//		}
		//		return new object[] { null, $"{filename}: No such file or directory" };
		//	}
		//	catch (Exception ex)
		//	{
		//		return new object[] { null, ex.Message };
		//	}
		//}

		///// <summary>
		///// 重命名文件或目录，成功返回 true，失败返回 nil 和错误信息
		///// </summary>
		//public object rename(string oldname, string newname)
		//{
		//	try
		//	{
		//		if (Directory.Exists(oldname))
		//		{
		//			if (Directory.Exists(newname)) Directory.Delete(newname, true);
		//			Directory.Move(oldname, newname);
		//			return true;
		//		}
		//		if (File.Exists(oldname))
		//		{
		//			// 与 lua 保持一致，目标存在时覆盖
		//			if (File.Exists(newname)) File.Delete(newname);
		//			File.Move(oldname, newname);
		//			return true;
		//		}
		//		return new object[] { null, $"{oldname}: No such file or directory" };
		//	}
		//	catch (Exception ex)
		//	{
		//		return new object[] { null, ex.Message };
		//	}
		//}

		/// <summary>
		/// 返回一个临时文件名
		/// </summary>
		public string tmpname() => Path.Combine(Path.GetTempPath(), "lua_" + Guid.NewGuid().ToString("N"));

		///// <summary>
		///// 查询当前的区域设置
		///// </summary>
		//public string setlocale() => "C";

		///// <summary>
		///// 设置区域设置
		///// </summary>
		//public string setlocale(string locale) => "C";

		///// <summary>
		///// 退出程序。宿主环境不允许结束进程，抛出异常终止脚本执行
		///// </summary>
		//public void exit() => exit(0);

		///// <summary>
		///// 退出程序。宿主环境不允许结束进程，抛出异常终止脚本执行
		///// </summary>
		///// <param name="code">退出码</param>
		//public void exit(int code)
		//{
		//	throw new Exceptions.ScriptRuntimeException($"os.exit({code})");
		//}

		#region 内部方法

		private static object FormatDate(string format, DateTime dateTime)
		{
			if (string.IsNullOrEmpty(format)) format = "%c";
			bool utc = false;
			if (format[0] == '!')
			{
				utc = true;
				format = format.Substring(1);
				if (format.Length == 0) format = "%c";
			}
			var dt = utc ? dateTime.ToUniversalTime() : dateTime;
			if (format == "*t")
			{
				var table = new LuaTable();
				table["year"] = dt.Year;
				table["month"] = dt.Month;
				table["day"] = dt.Day;
				table["hour"] = dt.Hour;
				table["min"] = dt.Minute;
				table["sec"] = dt.Second;
				// lua 的 wday 从周日开始，为 1
				table["wday"] = (int)dt.DayOfWeek + 1;
				table["yday"] = dt.DayOfYear;
				table["isdst"] = false;
				return table;
			}
			return StrFTime(format, dt, utc);
		}

		/// <summary>
		/// 按 lua 的 strftime 规则格式化时间
		/// </summary>
		private static string StrFTime(string format, DateTime dt, bool utc)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c != '%')
				{
					sb.Append(c);
					continue;
				}
				i++;
				if (i >= format.Length)
				{
					sb.Append('%');
					break;
				}
				switch (format[i])
				{
					case 'a': sb.Append(Invariant.DateTimeFormat.GetAbbreviatedDayName(dt.DayOfWeek)); break;
					case 'A': sb.Append(Invariant.DateTimeFormat.GetDayName(dt.DayOfWeek)); break;
					case 'b':
					case 'h': sb.Append(Invariant.DateTimeFormat.GetAbbreviatedMonthName(dt.Month)); break;
					case 'B': sb.Append(Invariant.DateTimeFormat.GetMonthName(dt.Month)); break;
					case 'c': sb.Append(StrFTime("%a %b %e %H:%M:%S %Y", dt, utc)); break;
					case 'C': sb.Append((dt.Year / 100).ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'd': sb.Append(dt.Day.ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'D': sb.Append(StrFTime("%m/%d/%y", dt, utc)); break;
					case 'e': sb.Append(dt.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2)); break;
					case 'F': sb.Append(StrFTime("%Y-%m-%d", dt, utc)); break;
					case 'g': sb.Append((GetIsoYear(dt) % 100).ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'G': sb.Append(GetIsoYear(dt).ToString("D4", CultureInfo.InvariantCulture)); break;
					case 'H': sb.Append(dt.Hour.ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'I': sb.Append(GetHour12(dt).ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'j': sb.Append(dt.DayOfYear.ToString("D3", CultureInfo.InvariantCulture)); break;
					case 'k': sb.Append(dt.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2)); break;
					case 'l': sb.Append(GetHour12(dt).ToString(CultureInfo.InvariantCulture).PadLeft(2)); break;
					case 'm': sb.Append(dt.Month.ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'M': sb.Append(dt.Minute.ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'n': sb.Append('\n'); break;
					case 'p': sb.Append(dt.Hour < 12 ? "AM" : "PM"); break;
					case 'P': sb.Append(dt.Hour < 12 ? "am" : "pm"); break;
					case 'r': sb.Append(StrFTime("%I:%M:%S %p", dt, utc)); break;
					case 'R': sb.Append(StrFTime("%H:%M", dt, utc)); break;
					case 'S': sb.Append(dt.Second.ToString("D2", CultureInfo.InvariantCulture)); break;
					case 't': sb.Append('\t'); break;
					case 'T': sb.Append(StrFTime("%H:%M:%S", dt, utc)); break;
					// %u：周一为 1，周日为 7
					case 'u': sb.Append(((int)dt.DayOfWeek == 0 ? 7 : (int)dt.DayOfWeek).ToString(CultureInfo.InvariantCulture)); break;
					case 'U': sb.Append(GetWeekOfYear(dt, 0).ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'V': sb.Append(GetIsoWeek(dt).ToString("D2", CultureInfo.InvariantCulture)); break;
					// %w：周日为 0
					case 'w': sb.Append(((int)dt.DayOfWeek).ToString(CultureInfo.InvariantCulture)); break;
					case 'W': sb.Append(GetWeekOfYear(dt, 1).ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'x': sb.Append(StrFTime("%m/%d/%y", dt, utc)); break;
					case 'X': sb.Append(StrFTime("%H:%M:%S", dt, utc)); break;
					case 'y': sb.Append((dt.Year % 100).ToString("D2", CultureInfo.InvariantCulture)); break;
					case 'Y': sb.Append(dt.Year.ToString("D4", CultureInfo.InvariantCulture)); break;
					case 'z': sb.Append(GetOffsetText(dt, utc)); break;
					case 'Z': sb.Append(GetZoneName(dt, utc)); break;
					case '%': sb.Append('%'); break;
					default: sb.Append('%').Append(format[i]); break;
				}
			}
			return sb.ToString();
		}

		private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

		private static int GetHour12(DateTime dt)
		{
			int hour = dt.Hour % 12;
			return hour == 0 ? 12 : hour;
		}

		/// <summary>
		/// 一年中的第几周，firstDayOfWeek 为 0 表示周日，1 表示周一
		/// </summary>
		private static int GetWeekOfYear(DateTime dt, int firstDayOfWeek)
		{
			int yday = dt.DayOfYear;
			int wday = (int)dt.DayOfWeek;
			if (firstDayOfWeek == 1)
			{
				// 以周一为一周的第一天
				wday = (wday + 6) % 7;
			}
			return (yday + 7 - wday) / 7;
		}

		private static int GetIsoWeek(DateTime dt)
		{
			int year = dt.Year;
			int week = GetIsoWeekOfYear(dt, out year);
			return week;
		}

		private static int GetIsoYear(DateTime dt)
		{
			int year;
			GetIsoWeekOfYear(dt, out year);
			return year;
		}

		/// <summary>
		/// 计算 ISO 8601 的周数和周所属的年份
		/// </summary>
		private static int GetIsoWeekOfYear(DateTime dt, out int year)
		{
			// ISO 8601：周一为一周的第一天
			int dow = (int)dt.DayOfWeek;
			if (dow == 0) dow = 7;
			int week = (dt.DayOfYear - dow + 10) / 7;
			year = dt.Year;
			if (week < 1)
			{
				year--;
				week = GetWeeksInIsoYear(year);
			}
			else if (week > GetWeeksInIsoYear(year))
			{
				year++;
				week = 1;
			}
			return week;
		}

		/// <summary>
		/// ISO 8601 一年中的周数，52 或 53
		/// </summary>
		private static int GetWeeksInIsoYear(int year)
		{
			// 1 月 1 日是周四，或者闰年的 1 月 1 日是周三时，这一年有 53 周
			int dow = (int)new DateTime(year, 1, 1).DayOfWeek;
			if (dow == 0) dow = 7;
			if (dow == 4) return 53;
			if (dow == 3 && DateTime.IsLeapYear(year)) return 53;
			return 52;
		}

		private static string GetOffsetText(DateTime dt, bool utc)
		{
			TimeSpan offset;
			if (utc)
			{
				offset = TimeSpan.Zero;
			}
			else
			{
				offset = TimeZoneInfo.Local.GetUtcOffset(dt);
			}
			string sign = offset < TimeSpan.Zero ? "-" : "+";
			offset = offset.Duration();
			return string.Format(CultureInfo.InvariantCulture, "{0}{1:00}{2:00}", sign, offset.Hours, offset.Minutes);
		}

		private static string GetZoneName(DateTime dt, bool utc)
		{
			if (utc) return "UTC";
			try
			{
				var name = TimeZoneInfo.Local.StandardName;
				return string.IsNullOrEmpty(name) ? GetOffsetText(dt, false) : name;
			}
			catch (Exception)
			{
				return GetOffsetText(dt, false);
			}
		}

		/// <summary>
		/// Unix 时间戳转本地时间
		/// </summary>
		private static DateTime ToLocalDateTime(long seconds)
		{
			return _baseTime.AddSeconds(seconds).ToLocalTime();
		}

		private static long ToLong(object value)
		{
			if (value == null) return 0;
			if (value is long l) return l;
			if (value is int i) return i;
			if (value is double d) return (long)d;
			if (value is float f) return (long)f;
			if (value is decimal m) return (long)m;
			return Convert.ToInt64(value);
		}

		private static int GetInt(LuaTable table, string key, int defaultValue)
		{
			var value = table[key];
			if (value == null) return defaultValue;
			return Convert.ToInt32(value);
		}

		#endregion
	}
}
