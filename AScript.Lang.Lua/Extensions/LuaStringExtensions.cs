using System;
using System.Collections.Generic;
using System.IO;
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

		// 辅助方法：将小端字节数组转换为指定字节序的值
		private static short ReadInt16(byte[] bytes, int index, bool bigEndian)
		{
			if (bigEndian)
			{
				var b = new byte[2];
				Array.Copy(bytes, index, b, 0, 2);
				Array.Reverse(b);
				return BitConverter.ToInt16(b, 0);
			}
			return BitConverter.ToInt16(bytes, index);
		}

		private static ushort ReadUInt16(byte[] bytes, int index, bool bigEndian)
		{
			if (bigEndian)
			{
				var b = new byte[2];
				Array.Copy(bytes, index, b, 0, 2);
				Array.Reverse(b);
				return BitConverter.ToUInt16(b, 0);
			}
			return BitConverter.ToUInt16(bytes, index);
		}

		private static int ReadInt32(byte[] bytes, int index, bool bigEndian)
		{
			if (bigEndian)
			{
				var b = new byte[4];
				Array.Copy(bytes, index, b, 0, 4);
				Array.Reverse(b);
				return BitConverter.ToInt32(b, 0);
			}
			return BitConverter.ToInt32(bytes, index);
		}

		private static uint ReadUInt32(byte[] bytes, int index, bool bigEndian)
		{
			if (bigEndian)
			{
				var b = new byte[4];
				Array.Copy(bytes, index, b, 0, 4);
				Array.Reverse(b);
				return BitConverter.ToUInt32(b, 0);
			}
			return BitConverter.ToUInt32(bytes, index);
		}

		private static long ReadInt64(byte[] bytes, int index, bool bigEndian)
		{
			if (bigEndian)
			{
				var b = new byte[8];
				Array.Copy(bytes, index, b, 0, 8);
				Array.Reverse(b);
				return BitConverter.ToInt64(b, 0);
			}
			return BitConverter.ToInt64(bytes, index);
		}

		private static ulong ReadUInt64(byte[] bytes, int index, bool bigEndian)
		{
			if (bigEndian)
			{
				var b = new byte[8];
				Array.Copy(bytes, index, b, 0, 8);
				Array.Reverse(b);
				return BitConverter.ToUInt64(b, 0);
			}
			return BitConverter.ToUInt64(bytes, index);
		}

		private static float ReadSingle(byte[] bytes, int index, bool bigEndian)
		{
			if (bigEndian)
			{
				var b = new byte[4];
				Array.Copy(bytes, index, b, 0, 4);
				Array.Reverse(b);
				return BitConverter.ToSingle(b, 0);
			}
			return BitConverter.ToSingle(bytes, index);
		}

		private static double ReadDouble(byte[] bytes, int index, bool bigEndian)
		{
			if (bigEndian)
			{
				var b = new byte[8];
				Array.Copy(bytes, index, b, 0, 8);
				Array.Reverse(b);
				return BitConverter.ToDouble(b, 0);
			}
			return BitConverter.ToDouble(bytes, index);
		}

		// 辅助方法：将值写入指定字节序的字节数组
		private static byte[] WriteInt16(short value, bool bigEndian)
		{
			var bytes = BitConverter.GetBytes(value);
			if (bigEndian) Array.Reverse(bytes);
			return bytes;
		}

		private static byte[] WriteUInt16(ushort value, bool bigEndian)
		{
			var bytes = BitConverter.GetBytes(value);
			if (bigEndian) Array.Reverse(bytes);
			return bytes;
		}

		private static byte[] WriteInt32(int value, bool bigEndian)
		{
			var bytes = BitConverter.GetBytes(value);
			if (bigEndian) Array.Reverse(bytes);
			return bytes;
		}

		private static byte[] WriteUInt32(uint value, bool bigEndian)
		{
			var bytes = BitConverter.GetBytes(value);
			if (bigEndian) Array.Reverse(bytes);
			return bytes;
		}

		private static byte[] WriteInt64(long value, bool bigEndian)
		{
			var bytes = BitConverter.GetBytes(value);
			if (bigEndian) Array.Reverse(bytes);
			return bytes;
		}

		private static byte[] WriteUInt64(ulong value, bool bigEndian)
		{
			var bytes = BitConverter.GetBytes(value);
			if (bigEndian) Array.Reverse(bytes);
			return bytes;
		}

		private static byte[] WriteSingle(float value, bool bigEndian)
		{
			var bytes = BitConverter.GetBytes(value);
			if (bigEndian) Array.Reverse(bytes);
			return bytes;
		}

		private static byte[] WriteDouble(double value, bool bigEndian)
		{
			var bytes = BitConverter.GetBytes(value);
			if (bigEndian) Array.Reverse(bytes);
			return bytes;
		}

		// 解析格式字符串开头的字节序指示符
		private static bool ParseEndianness(string format, out int skipChars)
		{
			skipChars = 0;
			if (string.IsNullOrEmpty(format)) return false;

			if (format[0] == '>')
			{
				skipChars = 1;
				return true; // 大端
			}
			if (format[0] == '<')
			{
				skipChars = 1;
				return false; // 小端
			}
			return false; // 默认小端
		}

		// string.pack(fmt, ...) - 打包值（Lua 5.3 风格）
		// 格式字符: b(signed byte), B(unsigned byte), h(signed short), H(unsigned short),
		//           l(signed long), L(unsigned long), i<I>(signed int with size), f(float), d(double),
		//           s(string with length prefix), c<sz>(fixed string), x(padding byte)
		// 字节序: > 表示大端，< 表示小端（默认）
		public static string string_pack(string format, params object[] args)
		{
			// 空格式字符串：将所有参数作为字节打包
			if (string.IsNullOrEmpty(format))
			{
				var sb = new StringBuilder();
				foreach (var arg in args)
				{
					if (arg is long l)
						sb.Append((char)l);
					else if (arg is int i)
						sb.Append((char)i);
					else if (arg is double d)
						sb.Append((char)(int)d);
					else if (arg is string str)
						sb.Append(str);
				}
				return sb.ToString();
			}

			// 解析字节序
			var bigEndian = ParseEndianness(format, out var skipChars);
			format = format.Substring(skipChars);
			using (var ms = new MemoryStream())
			{
				using (var bw = new BinaryWriter(ms))
				{
					int argIndex = 0;
					int i = 0;
					while (i < format.Length)
					{
						var fmt = format[i];
						switch (fmt)
						{
							case ' ':
								i++;
								break;
							case 'b': // signed byte (1 byte)
								if (argIndex < args.Length)
									bw.Write(Convert.ToSByte(args[argIndex++]));
								i++;
								break;
							case 'B': // unsigned byte (1 byte)
								if (argIndex < args.Length)
									bw.Write(Convert.ToByte(args[argIndex++]));
								i++;
								break;
							case 'h': // signed short (2 bytes)
								if (argIndex < args.Length)
									bw.Write(WriteInt16(Convert.ToInt16(args[argIndex++]), bigEndian));
								i++;
								break;
							case 'H': // unsigned short (2 bytes)
								if (argIndex < args.Length)
									bw.Write(WriteUInt16(Convert.ToUInt16(args[argIndex++]), bigEndian));
								i++;
								break;
							case 'l': // signed long (8 bytes)
								if (argIndex < args.Length)
									bw.Write(WriteInt64(Convert.ToInt64(args[argIndex++]), bigEndian));
								i++;
								break;
							case 'L': // unsigned long (8 bytes)
								if (argIndex < args.Length)
									bw.Write(WriteUInt64(Convert.ToUInt64(args[argIndex++]), bigEndian));
								i++;
								break;
							case 'i': // signed int with optional size modifier
							case 'I': // unsigned int with optional size modifier
								{
									var size = 4;
									if (i + 1 < format.Length && char.IsDigit(format[i + 1]))
									{
										size = format[i + 1] - '0';
										i++;
									}
									var isUnsigned = fmt == 'I';
									if (argIndex < args.Length)
									{
										var val = Convert.ToInt64(args[argIndex++]);
										switch (size)
										{
											case 1:
												if (isUnsigned) bw.Write(Convert.ToByte(val));
												else bw.Write(Convert.ToSByte(val));
												break;
											case 2:
												bw.Write(isUnsigned ? WriteUInt16(Convert.ToUInt16(val), bigEndian) : WriteInt16(Convert.ToInt16(val), bigEndian));
												break;
											case 4:
												bw.Write(isUnsigned ? WriteUInt32(Convert.ToUInt32(val), bigEndian) : WriteInt32(Convert.ToInt32(val), bigEndian));
												break;
											case 8:
												bw.Write(isUnsigned ? WriteUInt64(Convert.ToUInt64(val), bigEndian) : WriteInt64(val, bigEndian));
												break;
										}
									}
									i++;
								}
								break;
							case 'f': // float (4 bytes)
								if (argIndex < args.Length)
									bw.Write(WriteSingle(Convert.ToSingle(args[argIndex++]), bigEndian));
								i++;
								break;
							case 'd': // double (8 bytes)
								if (argIndex < args.Length)
									bw.Write(WriteDouble(Convert.ToDouble(args[argIndex++]), bigEndian));
								i++;
								break;
							case 's': // string with length prefix (4-byte length)
								if (argIndex < args.Length)
								{
									var str = args[argIndex++]?.ToString() ?? "";
									var lenBytes = WriteInt32(str.Length, bigEndian);
									bw.Write(lenBytes);
									var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
									bw.Write(strBytes);
								}
								i++;
								break;
							case 'c': // fixed-size string
								{
									var sizeStr = new StringBuilder();
									i++;
									while (i < format.Length && char.IsDigit(format[i]))
									{
										sizeStr.Append(format[i]);
										i++;
									}
									var size = sizeStr.Length > 0 ? int.Parse(sizeStr.ToString()) : 1;
									if (argIndex < args.Length)
									{
										var str = args[argIndex++]?.ToString() ?? "";
										var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
										if (strBytes.Length >= size)
										{
											for (int k = 0; k < size; k++)
												bw.Write(strBytes[k]);
										}
										else
										{
											bw.Write(strBytes);
											for (int k = strBytes.Length; k < size; k++)
												bw.Write((byte)0);
										}
									}
								}
								break;
							case 'x': // padding byte
								bw.Write((byte)0);
								i++;
								break;
							default:
								i++;
								break;
						}
					}

					bw.Flush();
					var result = ms.ToArray();
					// 将字节数组转换为字符串（每个字节对应一个字符）
					return System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(result);
				}
			}
		}

		public static List<object> string_unpack(string format, string s)
		{
			return string_unpack(format, s, 1);
		}

		// string.unpack(fmt, s, i) - 解包值（Lua 5.3 风格）
		// 返回 List<object>: 包含解包的值和下一个读取位置（非空格式时）
		public static List<object> string_unpack(string format, string s, long pos)
		{
			var result = new List<object>();
			var index = pos > 0 ? (int)(pos - 1) : Math.Max(0, s.Length + (int)pos);

			// 空格式字符串：将从pos开始的所有字节作为值返回（保持原有行为）
			if (string.IsNullOrEmpty(format))
			{
				while (index < s.Length)
				{
					result.Add((long)s[index]);
					index++;
				}
				return result;
			}

			// 解析字节序
			var bigEndian = ParseEndianness(format, out var skipChars);
			format = format.Substring(skipChars);

			// 将字符串转换为字节数组
			var bytes = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(s);

			int i = 0;
			while (i < format.Length && index < bytes.Length)
			{
				var fmt = format[i];
				switch (fmt)
				{
					case ' ':
						i++;
						break;
					case 'b': // signed byte
						result.Add((sbyte)bytes[index]);
						index++;
						i++;
						break;
					case 'B': // unsigned byte
						result.Add((long)bytes[index]);
						index++;
						i++;
						break;
					case 'h': // signed short (2 bytes)
						if (index + 2 <= bytes.Length)
						{
							result.Add(ReadInt16(bytes, index, bigEndian));
							index += 2;
						}
						i++;
						break;
					case 'H': // unsigned short (2 bytes)
						if (index + 2 <= bytes.Length)
						{
							result.Add((long)ReadUInt16(bytes, index, bigEndian));
							index += 2;
						}
						i++;
						break;
					case 'l': // signed long (8 bytes)
						if (index + 8 <= bytes.Length)
						{
							result.Add(ReadInt64(bytes, index, bigEndian));
							index += 8;
						}
						i++;
						break;
					case 'L': // unsigned long (8 bytes)
						if (index + 8 <= bytes.Length)
						{
							result.Add((long)ReadUInt64(bytes, index, bigEndian));
							index += 8;
						}
						i++;
						break;
					case 'i': // signed int with optional size modifier
					case 'I': // unsigned int with optional size modifier
						{
							var size = 4;
							if (i + 1 < format.Length && char.IsDigit(format[i + 1]))
							{
								size = format[i + 1] - '0';
								i++;
							}
							if (index + size <= bytes.Length)
							{
								switch (size)
								{
									case 1:
										result.Add(fmt == 'I' ? (object)(long)bytes[index] : (sbyte)bytes[index]);
										break;
									case 2:
										result.Add(fmt == 'I' ? (object)(long)ReadUInt16(bytes, index, bigEndian) : ReadInt16(bytes, index, bigEndian));
										break;
									case 4:
										result.Add(fmt == 'I' ? (object)(long)ReadUInt32(bytes, index, bigEndian) : ReadInt32(bytes, index, bigEndian));
										break;
									case 8:
										result.Add(fmt == 'I' ? (object)(long)ReadUInt64(bytes, index, bigEndian) : ReadInt64(bytes, index, bigEndian));
										break;
								}
								index += size;
							}
							i++;
						}
						break;
					case 'f': // float (4 bytes)
						if (index + 4 <= bytes.Length)
						{
							result.Add((double)ReadSingle(bytes, index, bigEndian));
							index += 4;
						}
						i++;
						break;
					case 'd': // double (8 bytes)
						if (index + 8 <= bytes.Length)
						{
							result.Add(ReadDouble(bytes, index, bigEndian));
							index += 8;
						}
						i++;
						break;
					case 's': // string with length prefix (4-byte length)
						if (index + 4 <= bytes.Length)
						{
							var len = ReadInt32(bytes, index, bigEndian);
							index += 4;
							if (len >= 0 && index + len <= bytes.Length)
							{
								var strBytes = new byte[len];
								Array.Copy(bytes, index, strBytes, 0, len);
								result.Add(System.Text.Encoding.UTF8.GetString(strBytes));
								index += len;
							}
						}
						i++;
						break;
					case 'c': // fixed-size string
						{
							var sizeStr = new StringBuilder();
							i++;
							while (i < format.Length && char.IsDigit(format[i]))
							{
								sizeStr.Append(format[i]);
								i++;
							}
							var size = sizeStr.Length > 0 ? int.Parse(sizeStr.ToString()) : 1;
							if (index + size <= bytes.Length)
							{
								var strBytes = new byte[size];
								Array.Copy(bytes, index, strBytes, 0, size);
								// 移除末尾的 null 字符
								var str = System.Text.Encoding.UTF8.GetString(strBytes);
								str = str.TrimEnd('\0');
								result.Add(str);
								index += size;
							}
						}
						break;
					case 'x': // padding byte
						index++;
						i++;
						break;
					case '=': // alignment
						i++;
						break;
					default:
						i++;
						break;
				}
			}

			// 返回解包的值列表和下一个读取位置
			result.Add((long)(index + 1));
			return result;
		}

	}
}
