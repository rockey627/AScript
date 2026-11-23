using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace AScript.Lang.Go.Extensions
{
	public static class GoTimeExtensions
	{
		public static DateTime time_Now() => DateTime.Now;

		public static void time_Sleep(int ms)
		{
			Thread.Sleep(ms);
		}

		/// <summary>
		/// 按 Go 的时间格式格式化时间，例如 Format(t, "2006-01-02 15:04:05")
		/// </summary>
		public static string Format(DateTime dateTime, string format)
		{
			if (string.IsNullOrEmpty(format)) return string.Empty;
			return dateTime.ToString(ToNetFormat(format, dateTime.Kind), CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Go 的时间格式转成 C# 的时间格式
		/// </summary>
		/// <param name="layout">Go 的时间格式，如 "2006-01-02 15:04:05"</param>
		/// <param name="kind">时间类型，决定 ISO8601 时区（Z07:00）在 UTC 时是否输出 Z</param>
		public static string ToNetFormat(string layout, DateTimeKind kind = DateTimeKind.Unspecified)
		{
			if (string.IsNullOrEmpty(layout)) return string.Empty;
			var sb = new StringBuilder();
			var literal = new StringBuilder();
			int index = 0;
			while (index < layout.Length)
			{
				// 小数秒：.000、.000000、.999
				if (layout[index] == '.' && index + 1 < layout.Length && (layout[index + 1] == '0' || layout[index + 1] == '9'))
				{
					int count = 0;
					while (index + 1 + count < layout.Length && (layout[index + 1 + count] == '0' || layout[index + 1 + count] == '9')) count++;
					bool zeros = true;
					bool nines = true;
					for (int i = 0; i < count; i++)
					{
						char c = layout[index + 1 + i];
						if (c != '0') zeros = false;
						if (c != '9') nines = false;
					}
					if (zeros || nines)
					{
						// 小数秒前面的点需要转义
						FlushLiteral(sb, literal);
						sb.Append("'.'");
						// f 保留尾随零，F 去掉尾随零
						sb.Append(new string(zeros ? 'f' : 'F', count));
						index += count + 1;
						continue;
					}
				}
				// 标准格式
				int length = MatchStdChunk(layout, index);
				if (length > 0)
				{
					FlushLiteral(sb, literal);
					sb.Append(GetNetFormat(layout.Substring(index, length), kind));
					index += length;
					continue;
				}
				// 普通字符
				literal.Append(layout[index]);
				index++;
			}
			FlushLiteral(sb, literal);
			return sb.ToString();
		}

		/// <summary>
		/// Go 的标准时间格式
		/// </summary>
		private static readonly HashSet<string> StdChunks = new HashSet<string>
		{
			"January", "Jan", "01", "1",
			"Monday", "Mon",
			"02", "_2", "2",
			"__2", "002",
			"03", "3", "15",
			"04", "4",
			"05", "5",
			"PM", "pm",
			"MST",
			"Z07:00", "Z0700", "Z07",
			"-07:00", "-0700", "-07",
			"2006", "06"
		};

		/// <summary>
		/// 匹配 Go 的标准时间格式，优先匹配最长的
		/// </summary>
		/// <returns>匹配到的长度，没有匹配返回 0</returns>
		private static int MatchStdChunk(string layout, int index)
		{
			int max = Math.Min(7, layout.Length - index);
			for (int length = max; length > 0; length--)
			{
				if (StdChunks.Contains(layout.Substring(index, length)))
				{
					return length;
				}
			}
			return 0;
		}

		/// <summary>
		/// Go 的标准时间格式对应的 C# 时间格式
		/// </summary>
		private static string GetNetFormat(string chunk, DateTimeKind kind)
		{
			switch (chunk)
			{
				case "January": return "MMMM";
				case "Jan": return "MMM";
				case "Monday": return "dddd";
				case "Mon": return "ddd";
				case "01": return "MM";
				case "1": return "M";
				case "02": return "dd";
				// Go 用空格补齐，C# 没有空格补齐的写法
				case "_2": return "d";
				case "2": return "d";
				// 一年中的第几天
				case "002": return "DD";
				case "__2": return "D";
				case "03": return "hh";
				case "3": return "h";
				case "15": return "HH";
				case "04": return "mm";
				case "4": return "m";
				case "05": return "ss";
				case "5": return "s";
				case "PM": return "tt";
				case "pm": return "tt";
				// 时区
				case "MST": return "K";
				case "Z07:00":
				case "Z0700":
					// UTC 时 Go 输出 Z
					return kind == DateTimeKind.Utc ? "'Z'" : "zzz";
				case "Z07":
					return kind == DateTimeKind.Utc ? "'Z'" : "zz";
				case "-07:00":
				case "-0700":
					return "zzz";
				case "-07":
					return "zz";
				case "2006": return "yyyy";
				case "06": return "yy";
				default: return string.Empty;
			}
		}

		/// <summary>
		/// 输出普通字符，需要用单引号转义
		/// </summary>
		private static void FlushLiteral(StringBuilder sb, StringBuilder literal)
		{
			if (literal.Length == 0) return;
			sb.Append('\'');
			for (int i = 0; i < literal.Length; i++)
			{
				char c = literal[i];
				// 单引号需要转义成两个
				if (c == '\'') sb.Append('\'');
				sb.Append(c);
			}
			sb.Append('\'');
			literal.Clear();
		}
	}
}
