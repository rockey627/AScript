using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AScript.Lang.Lua.Extensions
{
	public static class LuaStringExtensions
	{
		// string.len(s) - 返回字符串长度
		public static long string_len(string s) => s.Length;

		// string.sub(s, i, j) - 截取字符串，Lua 使用 1-based 索引
		// i 为起始位置（1-based，可为负数），j 为结束位置（1-based，可为负数）
		public static string string_sub(string s, long i)
		{
			// sub(s, i) - 从 i 截取到末尾
			if (i > 0)
				return s.Substring((int)(i - 1));
			return s.Substring((int)(s.Length + i));
		}

		public static string string_sub(string s, long i, long j)
		{
			// sub(s, i, j) - 从 i 截取到 j
			var start = i > 0 ? i - 1 : s.Length + i;
			var end = j > 0 ? j : s.Length + j + 1;
			var length = end - start;
			return length > 0 ? s.Substring((int)start, (int)length) : string.Empty;
		}

		// string.find(s, pattern, init, plain) - 查找子串，Lua 使用 1-based 索引
		// 返回 (起始位置, 结束位置)，未找到返回 nil
#if NETFRAMEWORK
		public static Tuple<long, long> string_find(string s, string pattern)
		{
			// find(s, pattern) - 从头查找
			var index = s.IndexOf(pattern);
			return index >= 0 ? Tuple.Create(index + 1L, index + 1L + pattern.Length - 1) : null;
		}

		public static Tuple<long, long> string_find(string s, string pattern, long init)
		{
			// find(s, pattern, init) - 从 init 位置开始查找，init 为 1-based
			var startIndex = init > 0 ? (int)(init - 1) : Math.Max(0, s.Length + (int)init);
			var index = s.IndexOf(pattern, startIndex);
			return index >= 0 ? Tuple.Create(index + 1L, index + 1L + pattern.Length - 1) : null;
		}

		public static Tuple<long, long> string_find(string s, string pattern, long init, bool plain)
		{
			// find(s, pattern, init, plain) - plain 为 true 时禁用模式匹配
			var startIndex = init > 0 ? (int)(init - 1) : Math.Max(0, s.Length + (int)init);
			var index = s.IndexOf(pattern, startIndex);
			return index >= 0 ? Tuple.Create(index + 1L, index + 1L + pattern.Length - 1) : null;
		}
#else
		public static (long, long) string_find(string s, string pattern)
		{
			// find(s, pattern) - 从头查找
			var index = s.IndexOf(pattern);
			return index >= 0 ? (index + 1L, index + 1L + pattern.Length - 1) : (0L, 0L);
		}

		public static (long, long) string_find(string s, string pattern, long init)
		{
			// find(s, pattern, init) - 从 init 位置开始查找，init 为 1-based
			var startIndex = init > 0 ? (int)(init - 1) : Math.Max(0, s.Length + (int)init);
			var index = s.IndexOf(pattern, startIndex);
			return index >= 0 ? (index + 1L, index + 1L + pattern.Length - 1) : (0L, 0L);
		}

		public static (long, long) string_find(string s, string pattern, long init, bool plain)
		{
			// find(s, pattern, init, plain) - plain 为 true 时禁用模式匹配
			var startIndex = init > 0 ? (int)(init - 1) : Math.Max(0, s.Length + (int)init);
			var index = s.IndexOf(pattern, startIndex);
			return index >= 0 ? (index + 1L, index + 1L + pattern.Length - 1) : (0L, 0L);
		}
#endif

		// string.format(formatstring, ...) - 格式化字符串，支持 %s 和 %d 格式说明符
		public static string string_format(string format, params object[] args)
		{
			// 将 Lua 风格的 %s,%d,%q 格式说明符转换为 .NET 风格的 {0},{1} 等
			int argIndex = 0;
			var netFormat = System.Text.RegularExpressions.Regex.Replace(format, @"%[sdq]", match =>
			{
				return "{" + argIndex++ + "}";
			});
			return string.Format(netFormat, args);
		}

		// string.lower(s) / string.upper(s) - 大小写转换
		public static string string_lower(string s) => s.ToLower();
		public static string string_upper(string s) => s.ToUpper();

		// string.reverse(s) - 反转字符串
		public static string string_reverse(string s) => new string(s.Reverse().ToArray());

		// string.char(...) - 将数字转成字符
		public static string string_char(params long[] codes)
		{
			var sb = new StringBuilder(codes.Length);
			foreach (var code in codes)
			{
				sb.Append((char)Convert.ToInt32(code));
			}
			return sb.ToString();
		}

		// string.byte(s, i, j) - 获取字符的 ASCII 码，Lua 使用 1-based 索引
		public static long string_byte(string s)
		{
			// byte(s) - 返回第一个字符的 ASCII 码
			return s.Length > 0 ? s[0] : 0;
		}

		public static long string_byte(string s, long i)
		{
			// byte(s, i) - 返回第 i 个字符的 ASCII 码
			var index = i > 0 ? (int)(i - 1) : (int)(s.Length + i);
			return index >= 0 && index < s.Length ? s[index] : 0;
		}

		public static List<object> string_byte(string s, long i, long j)
		{
			// byte(s, i, j) - 返回 i 到 j 之间所有字符的 ASCII 码
			var start = i > 0 ? (int)(i - 1) : Math.Max(0, s.Length + (int)i);
			var end = j > 0 ? (int)j : s.Length + (int)j + 1;
			var result = new List<object>();
			for (int k = start; k < end && k < s.Length; k++)
			{
				if (k >= 0)
					result.Add((long)s[k]);
			}
			return result;
		}

		// string.gsub(s, pattern, replacement) - 全局替换，返回 (结果, 替换次数)
#if NETFRAMEWORK
		public static Tuple<string, long> string_gsub(string s, string pattern, string replacement)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
				return Tuple.Create(s, 0L);
			var count = 0L;
			var result = new StringBuilder();
			var remaining = s;
			while (true)
			{
				var index = remaining.IndexOf(pattern);
				if (index < 0) break;
				result.Append(remaining.Substring(0, index));
				result.Append(replacement);
				remaining = remaining.Substring(index + pattern.Length);
				count++;
			}
			result.Append(remaining);
			return Tuple.Create(result.ToString(), count);
		}

		public static Tuple<string, long> string_gsub(string s, string pattern, string replacement, long n)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
				return Tuple.Create(s, 0L);
			if (n <= 0) return Tuple.Create(s, 0L);

			var result = new StringBuilder();
			var remaining = s;
			var count = 0L;

			while (count < n)
			{
				var index = remaining.IndexOf(pattern);
				if (index < 0) break;

				result.Append(remaining.Substring(0, index));
				result.Append(replacement);
				remaining = remaining.Substring(index + pattern.Length);
				count++;
			}

			result.Append(remaining);
			return Tuple.Create(result.ToString(), count);
		}
#else
		public static (string result, long count) string_gsub(string s, string pattern, string replacement)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
				return (s, 0L);
			var count = 0L;
			var result = new StringBuilder();
			var remaining = s;
			while (true)
			{
				var index = remaining.IndexOf(pattern);
				if (index < 0) break;
				result.Append(remaining.Substring(0, index));
				result.Append(replacement);
				remaining = remaining.Substring(index + pattern.Length);
				count++;
			}
			result.Append(remaining);
			return (result.ToString(), count);
		}

		public static (string result, long count) string_gsub(string s, string pattern, string replacement, long n)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
				return (s, 0L);
			if (n <= 0) return (s, 0L);

			var result = new StringBuilder();
			var remaining = s;
			var count = 0L;

			while (count < n)
			{
				var index = remaining.IndexOf(pattern);
				if (index < 0) break;

				result.Append(remaining.Substring(0, index));
				result.Append(replacement);
				remaining = remaining.Substring(index + pattern.Length);
				count++;
			}

			result.Append(remaining);
			return (result.ToString(), count);
		}
#endif

		// 辅助方法：将 Lua 模式转换为 .NET 正则表达式模式
		private static string LuaPatternToNetRegex(string pattern)
		{
			if (string.IsNullOrEmpty(pattern)) return pattern;

			var sb = new StringBuilder();
			var i = 0;
			while (i < pattern.Length)
			{
				if (pattern[i] == '%' && i + 1 < pattern.Length)
				{
					var nextChar = pattern[i + 1];
					switch (nextChar)
					{
						// Lua 字符类转换为 .NET 正则表达式
						case 'a': sb.Append(@"[A-Za-z]"); break;          // 任何字母
						case 'c': sb.Append(@"[\x00-\x1F\x7F]"); break;  // 任何控制字符
						case 'd': sb.Append(@"[0-9]"); break;             // 任何数字
						case 'l': sb.Append(@"[a-z]"); break;             // 任何小写字母
						case 'p': sb.Append(@"[!""#$%&'()*+,\-./:;<=>?@[\\\]^_`{|}~]"); break; // 任何标点符号
						case 's': sb.Append(@"[ \t\n\r\f\v]"); break;      // 任何空白字符
						case 'u': sb.Append(@"[A-Z]"); break;             // 任何大写字母
						case 'w': sb.Append(@"[A-Za-z0-9]"); break;        // 任何字母数字
						case 'x': sb.Append(@"[0-9A-Fa-f]"); break;        // 任何十六进制数字
						case 'z': sb.Append(@"\x00"); break;              // 字符串结束符（null字符）
						// 转义字符
						case '%': sb.Append(@"%"); break;
						case '.': sb.Append(@"\."); break;
						case '^': sb.Append(@"\^"); break;
						case '$': sb.Append(@"\$"); break;
						case '(': sb.Append(@"\("); break;
						case ')': sb.Append(@"\)"); break;
						case '[': sb.Append(@"\["); break;
						case ']': sb.Append(@"\]"); break;
						case '*': sb.Append(@"\*"); break;
						case '+': sb.Append(@"\+"); break;
						case '?': sb.Append(@"\?"); break;
						case '#': sb.Append(@"\#"); break; // Lua 5.2+ 模式修饰符
						case '=': sb.Append(@"="); break;
						case '-': sb.Append(@"\-"); break;
						default: sb.Append('%').Append(nextChar); break;
					}
					i += 2;
				}
				else
				{
					// 普通字符，需要转义在正则表达式中有特殊意义的字符
					switch (pattern[i])
					{
						case '\\': sb.Append(@"\\"); break;
						case '/': sb.Append(@"/"); break;  // Lua pattern separator
						default: sb.Append(pattern[i]); break;
					}
					i++;
				}
			}
			return sb.ToString();
		}

		// string.match(s, pattern, init) - 匹配一次
		public static List<object> string_match(string s, string pattern)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern)) return null;
			var netPattern = LuaPatternToNetRegex(pattern);
			var match = Regex.Match(s, netPattern);
			if (!match.Success) return null;
			return new List<object> { match.Value };
		}

		public static List<object> string_match(string s, string pattern, long init)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern)) return null;
			var startIndex = init > 0 ? (int)(init - 1) : Math.Max(0, s.Length + (int)init);
			if (startIndex >= s.Length) return null;

			var remaining = s.Substring(startIndex);
			var netPattern = LuaPatternToNetRegex(pattern);
			var match = Regex.Match(remaining, netPattern);
			if (!match.Success) return null;
			return new List<object> { match.Value };
		}

		// string.gmatch(s, pattern) - 全局匹配，返回所有匹配结果
		public static List<object> string_gmatch(string s, string pattern)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern)) return new List<object>();
			var netPattern = LuaPatternToNetRegex(pattern);
			var matches = Regex.Matches(s, netPattern);
			var result = new List<object>(matches.Count);
			foreach (Match match in matches)
			{
				result.Add(match.Value);
			}
			return result;
		}

		// string.rep(s, n) - 重复字符串 n 次
		public static string string_rep(string s, long n) => string.Concat(Enumerable.Repeat(s, (int)n));

		// string.rep(s, sep, n) - 带分隔符重复字符串（Lua 5.3+ 支持）
		public static string string_rep(string s, string sep, long n)
		{
			if (string.IsNullOrEmpty(s)) return string.Empty;
			if (n <= 0) return string.Empty;
			if (n == 1) return s;
			if (string.IsNullOrEmpty(sep)) return string_rep(s, n);

			var parts = new string[n];
			for (int i = 0; i < n; i++)
			{
				parts[i] = s;
			}
			return string.Join(sep, parts);
		}

		// string.pack(fmt, ...) - 打包值（简化实现）
		public static string string_pack(string format, params object[] args)
		{
			var sb = new StringBuilder();
			foreach (var arg in args)
			{
				if (arg is long l)
				{
					sb.Append((char)l);
				}
				else if (arg is int i)
				{
					sb.Append((char)i);
				}
				else if (arg is double d)
				{
					sb.Append((char)(int)d);
				}
				else if (arg is string str)
				{
					sb.Append(str);
				}
			}
			return sb.ToString();
		}

		// string.unpack(fmt, s, i) - 解包值（简化实现）
		public static List<object> string_unpack(string format, string s, long pos = 1)
		{
			var result = new List<object>();
			var index = pos > 0 ? (int)(pos - 1) : Math.Max(0, s.Length + (int)pos);
			while (index < s.Length)
			{
				result.Add((long)s[index]);
				index++;
			}
			return result;
		}

	}
}
