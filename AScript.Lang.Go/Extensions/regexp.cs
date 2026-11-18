using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AScript.Lang.Go.Extensions
{
	/// <summary>
	/// Go语言regexp包的模拟实现
	/// <para>使用方式：</para>
	/// <para>import 'regexp'</para>
	/// <para>re := regexp.MustCompile('\d+')</para>
	/// <para>re.MatchString('abc123')</para>
	/// </summary>
	public class regexp
	{
		#region 包级函数

		/// <summary>
		/// 编译正则表达式，如果表达式非法则抛出异常（对应Go的regexp.MustCompile）
		/// </summary>
		/// <param name="pattern">正则表达式</param>
		/// <returns>编译后的正则对象</returns>
		public static Regexp MustCompile(string pattern)
		{
			Regexp re;
			string err;
			if (!TryCompile(pattern, out re, out err))
			{
				throw new Exceptions.ScriptRuntimeException(err);
			}
			return re;
		}

		/// <summary>
		/// 编译正则表达式（对应Go的regexp.Compile）
		/// </summary>
		/// <param name="pattern">正则表达式</param>
		/// <returns>第一个值为编译结果，第二个值为错误信息，编译成功时错误信息为null</returns>
#if NET45
		public static Tuple<Regexp, string> Compile(string pattern)
		{
			Regexp re;
			string err;
			if (!TryCompile(pattern, out re, out err))
			{
				return Tuple.Create<Regexp, string>(null, err);
			}
			return Tuple.Create<Regexp, string>(re, null);
		}
#else
		public static (Regexp, string) Compile(string pattern)
		{
			Regexp re;
			string err;
			if (!TryCompile(pattern, out re, out err))
			{
				return (null, err);
			}
			return (re, null);
		}
#endif

		/// <summary>
		/// 判断字符串是否匹配正则表达式（对应Go的regexp.MatchString）
		/// </summary>
		/// <param name="pattern">正则表达式</param>
		/// <param name="s">待匹配的字符串</param>
		/// <returns>是否匹配</returns>
		public static bool MatchString(string pattern, string s)
		{
			Regexp re;
			string err;
			if (!TryCompile(pattern, out re, out err))
			{
				throw new Exceptions.ScriptRuntimeException(err);
			}
			return re.MatchString(s);
		}

		/// <summary>
		/// 判断字符串是否匹配正则表达式（对应Go的regexp.Match）
		/// </summary>
		/// <param name="pattern">正则表达式</param>
		/// <param name="s">待匹配的字符串</param>
		/// <returns>第一个值为匹配结果，第二个值为错误信息，编译成功时错误信息为null</returns>
#if NET45
		public static Tuple<bool, string> Match(string pattern, string s)
		{
			Regexp re;
			string err;
			if (!TryCompile(pattern, out re, out err))
			{
				return Tuple.Create<bool, string>(false, err);
			}
			return Tuple.Create<bool, string>(re.MatchString(s), null);
		}
#else
		public static (bool, string) Match(string pattern, string s)
		{
			Regexp re;
			string err;
			if (!TryCompile(pattern, out re, out err))
			{
				return (false, err);
			}
			return (re.MatchString(s), null);
		}
#endif

		/// <summary>
		/// 转义正则表达式中的特殊字符，使其可以按字面量匹配（对应Go的regexp.QuoteMeta）
		/// </summary>
		/// <param name="s">待转义的字符串</param>
		/// <returns>转义后的字符串</returns>
		public static string QuoteMeta(string s)
		{
			if (string.IsNullOrEmpty(s)) return s;
			var specials = "\\.+*?()|[]{}^$";
			var sb = new StringBuilder(s.Length + 8);
			for (int i = 0; i < s.Length; i++)
			{
				var c = s[i];
				if (specials.IndexOf(c) >= 0) sb.Append('\\');
				sb.Append(c);
			}
			return sb.ToString();
		}

		/// <summary>
		/// 编译正则表达式（对应Go的regexp.CompilePOSIX）
		/// </summary>
		/// <param name="pattern">正则表达式</param>
		/// <returns>编译后的正则对象</returns>
		public static Regexp CompilePOSIX(string pattern)
		{
			return MustCompile(pattern);
		}

		#endregion

		private static bool TryCompile(string pattern, out Regexp re, out string err)
		{
			re = null;
			err = null;
			if (pattern == null)
			{
				err = "regexp: Compile(<nil>): invalid pattern";
				return false;
			}
			try
			{
				re = new Regexp(pattern);
				return true;
			}
			catch (ArgumentException ex)
			{
				err = $"regexp: Compile({QuoteMeta(pattern)}): error parsing regexp: {ex.Message}";
				return false;
			}
		}

		/// <summary>
		/// Go语言regexp.Regexp类型
		/// </summary>
		public class Regexp
		{
			private readonly Regex _regex;
			private readonly string _pattern;

			/// <summary>
			/// 编译正则表达式
			/// </summary>
			/// <param name="pattern">正则表达式</param>
			public Regexp(string pattern)
			{
				_pattern = pattern ?? string.Empty;
				_regex = new Regex(GoRegexPattern.Translate(_pattern));
			}

			/// <summary>
			/// 返回正则表达式的源码（对应Go的re.String()）
			/// </summary>
			/// <returns>正则表达式源码</returns>
			public string String()
			{
				return _pattern;
			}

			/// <summary>
			/// 判断字符串是否匹配（对应Go的re.MatchString）
			/// </summary>
			/// <param name="s">待匹配的字符串</param>
			/// <returns>是否匹配</returns>
			public bool MatchString(string s)
			{
				return _regex.IsMatch(s ?? string.Empty);
			}

			/// <summary>
			/// 查找第一个匹配项（对应Go的re.Match）
			/// </summary>
			/// <param name="s">待匹配的字符串</param>
			/// <returns>匹配结果，没有匹配时返回null</returns>
			public RegexpMatch Match(string s)
			{
				var m = _regex.Match(s ?? string.Empty);
				if (!m.Success) return null;
				return RegexpMatch.Create(m, _regex);
			}

			/// <summary>
			/// 查找第一个匹配的字符串（对应Go的re.FindString）
			/// </summary>
			/// <param name="s">待匹配的字符串</param>
			/// <returns>匹配的字符串，没有匹配时返回空字符串</returns>
			public string FindString(string s)
			{
				var m = _regex.Match(s ?? string.Empty);
				return m.Success ? m.Value : string.Empty;
			}

			/// <summary>
			/// 查找第一个匹配的索引（对应Go的re.FindStringIndex）
			/// </summary>
			/// <param name="s">待匹配的字符串</param>
			/// <returns>包含起始索引和结束索引的数组，没有匹配时返回null</returns>
			public int[] FindStringIndex(string s)
			{
				var m = _regex.Match(s ?? string.Empty);
				if (!m.Success) return null;
				return new int[] { m.Index, m.Index + m.Length };
			}

			/// <summary>
			/// 查找第一个匹配项及其子匹配项（对应Go的re.FindStringSubmatch）
			/// </summary>
			/// <param name="s">待匹配的字符串</param>
			/// <returns>第一个元素为整个匹配结果，其后为各个子匹配项，没有匹配时返回null</returns>
			public string[] FindStringSubmatch(string s)
			{
				var m = _regex.Match(s ?? string.Empty);
				if (!m.Success) return null;
				return RegexpMatch.GetSubmatches(m);
			}

			/// <summary>
			/// 查找所有匹配的字符串（对应Go的re.FindAllString）
			/// </summary>
			/// <param name="s">待匹配的字符串</param>
			/// <param name="n">最多返回的匹配数，小于0表示返回全部</param>
			/// <returns>所有匹配的字符串</returns>
			public string[] FindAllString(string s, int n)
			{
				var list = new List<string>();
				foreach (Match m in _regex.Matches(s ?? string.Empty))
				{
					if (n >= 0 && list.Count >= n) break;
					list.Add(m.Value);
				}
				return list.ToArray();
			}

			/// <summary>
			/// 查找所有匹配的索引（对应Go的re.FindAllStringIndex）
			/// </summary>
			/// <param name="s">待匹配的字符串</param>
			/// <param name="n">最多返回的匹配数，小于0表示返回全部</param>
			/// <returns>每个匹配项包含起始索引和结束索引</returns>
			public int[][] FindAllStringIndex(string s, int n)
			{
				var list = new List<int[]>();
				foreach (Match m in _regex.Matches(s ?? string.Empty))
				{
					if (n >= 0 && list.Count >= n) break;
					list.Add(new int[] { m.Index, m.Index + m.Length });
				}
				return list.ToArray();
			}

			/// <summary>
			/// 查找所有匹配项及其子匹配项（对应Go的re.FindAllStringSubmatch）
			/// </summary>
			/// <param name="s">待匹配的字符串</param>
			/// <param name="n">最多返回的匹配数，小于0表示返回全部</param>
			/// <returns>每个匹配项的第一个元素为整个匹配结果，其后为各个子匹配项</returns>
			public string[][] FindAllStringSubmatch(string s, int n)
			{
				var list = new List<string[]>();
				foreach (Match m in _regex.Matches(s ?? string.Empty))
				{
					if (n >= 0 && list.Count >= n) break;
					list.Add(RegexpMatch.GetSubmatches(m));
				}
				return list.ToArray();
			}

			/// <summary>
			/// 替换字符串中所有匹配的内容（对应Go的re.ReplaceAllString）
			/// </summary>
			/// <param name="src">源字符串</param>
			/// <param name="repl">替换模板，$1表示第一个子匹配项，${name}表示名称为name的子匹配项</param>
			/// <returns>替换后的字符串</returns>
			public string ReplaceAllString(string src, string repl)
			{
				return _regex.Replace(src ?? string.Empty, GoRegexPattern.ConvertTemplate(repl ?? string.Empty));
			}

			/// <summary>
			/// 使用函数替换字符串中所有匹配的内容（对应Go的re.ReplaceAllStringFunc）
			/// </summary>
			/// <param name="src">源字符串</param>
			/// <param name="fn">替换函数，参数为匹配到的文本，返回替换后的文本</param>
			/// <returns>替换后的字符串</returns>
			public string ReplaceAllStringFunc(string src, Delegate fn)
			{
				if (fn == null) throw new ArgumentNullException(nameof(fn));
				return _regex.Replace(src ?? string.Empty, m => fn.DynamicInvoke(new object[] { m.Value }) as string);
			}

			/// <summary>
			/// 以正则表达式为分隔符分割字符串（对应Go的re.Split）
			/// </summary>
			/// <param name="s">待分割的字符串</param>
			/// <param name="n">最多返回的子串数，小于0表示返回全部，等于0时返回null</param>
			/// <returns>分割后的子串数组</returns>
			public string[] Split(string s, int n)
			{
				if (n == 0) return null;
				s = s ?? string.Empty;
				if (_pattern.Length > 0 && s.Length == 0) return new string[] { string.Empty };
				var matches = FindAllStringIndex(s, n);
				var strs = new List<string>();
				int beg = 0;
				int end = 0;
				for (int i = 0; i < matches.Length; i++)
				{
					if (n > 0 && strs.Count == n - 1) break;
					end = matches[i][0];
					if (matches[i][1] != 0)
					{
						strs.Add(s.Substring(beg, end - beg));
					}
					beg = matches[i][1];
				}
				if (end != s.Length)
				{
					strs.Add(s.Substring(beg));
				}
				return strs.ToArray();
			}

			/// <summary>
			/// 返回捕获组的数量（对应Go的re.NumSubexp）
			/// </summary>
			/// <returns>捕获组的数量</returns>
			public int NumSubexp()
			{
				return _regex.GetGroupNumbers().Length - 1;
			}

			/// <summary>
			/// 返回各个捕获组的名称，没有名称的返回空字符串（对应Go的re.SubexpNames）
			/// </summary>
			/// <returns>捕获组名称数组</returns>
			public string[] SubexpNames()
			{
				var names = _regex.GetGroupNames();
				var result = new string[names.Length];
				for (int i = 0; i < names.Length; i++)
				{
					int number;
					result[i] = int.TryParse(names[i], out number) ? string.Empty : names[i];
				}
				return result;
			}
		}

		/// <summary>
		/// Go语言regexp.Match类型
		/// </summary>
		public class RegexpMatch
		{
			/// <summary>
			/// 匹配的起始索引和结束索引（对应Go的Match.Index）
			/// </summary>
			public int[] Index { get; set; }

			/// <summary>
			/// 各个子匹配项，第一个元素为整个匹配结果（对应Go的Match.Submatch）
			/// </summary>
			public string[] Submatches { get; set; }

			private string _text;
			private Dictionary<string, int> _subexpIndex;

			/// <summary>
			/// 返回匹配的文本（对应Go的Match.String()）
			/// </summary>
			/// <returns>匹配的文本</returns>
			public string String()
			{
				return _text;
			}

			/// <summary>
			/// 返回匹配的文本
			/// </summary>
			/// <returns>匹配的文本</returns>
			public override string ToString()
			{
				return _text;
			}

			/// <summary>
			/// 返回第i个子匹配项（对应Go的Match.Subexp）
			/// </summary>
			/// <param name="i">子匹配项索引，0表示整个匹配结果</param>
			/// <returns>子匹配项的文本</returns>
			public string Subexp(int i)
			{
				if (Submatches == null || i < 0 || i >= Submatches.Length) return string.Empty;
				return Submatches[i];
			}

			/// <summary>
			/// 返回名称为name的子匹配项的索引（对应Go的Match.SubexpIndex）
			/// </summary>
			/// <param name="name">捕获组名称</param>
			/// <returns>子匹配项的索引，不存在时返回-1</returns>
			public int SubexpIndex(string name)
			{
				if (_subexpIndex == null || name == null) return -1;
				return _subexpIndex.TryGetValue(name, out var index) ? index : -1;
			}

			/// <summary>
			/// 返回捕获组的数量（对应Go的Match.NumSubexp）
			/// </summary>
			/// <returns>捕获组的数量</returns>
			public int NumSubexp()
			{
				return Submatches == null ? 0 : Submatches.Length - 1;
			}

			internal static RegexpMatch Create(Match match, Regex regex)
			{
				var result = new RegexpMatch
				{
					Index = new int[] { match.Index, match.Index + match.Length },
					Submatches = GetSubmatches(match),
					_text = match.Value
				};
				var names = regex.GetGroupNames();
				var index = new Dictionary<string, int>();
				for (int i = 0; i < names.Length; i++)
				{
					int number;
					if (!int.TryParse(names[i], out number)) index[names[i]] = i;
				}
				result._subexpIndex = index;
				return result;
			}

			internal static string[] GetSubmatches(Match match)
			{
				var result = new string[match.Groups.Count];
				for (int i = 0; i < match.Groups.Count; i++)
				{
					result[i] = match.Groups[i].Value;
				}
				return result;
			}
		}

		/// <summary>
		/// Go语言正则表达式与.NET正则表达式之间的转换
		/// </summary>
		internal static class GoRegexPattern
		{
			/// <summary>
			/// 将Go的正则表达式转换为.NET的正则表达式
			/// <para>1.Go中的\d、\w、\s只匹配ASCII字符，.NET中默认匹配Unicode字符，这里做转换以保持一致</para>
			/// <para>2.Go中的命名捕获组写法为(?P&lt;name&gt;...)，.NET中为(?&lt;name&gt;...)</para>
			/// </summary>
			public static string Translate(string pattern)
			{
				if (string.IsNullOrEmpty(pattern)) return pattern ?? string.Empty;
				var sb = new StringBuilder(pattern.Length + 16);
				bool inClass = false;
				for (int i = 0; i < pattern.Length; i++)
				{
					var c = pattern[i];
					// 命名捕获组 (?P<name>...) 与 (?P=name)
					if (c == '(' && i + 3 < pattern.Length && pattern[i + 1] == '?' && pattern[i + 2] == 'P')
					{
						if (pattern[i + 3] == '<')
						{
							sb.Append("(?<");
							i += 3;
							continue;
						}
						if (pattern[i + 3] == '=')
						{
							var end = pattern.IndexOf(')', i + 4);
							if (end > 0)
							{
								sb.Append("\\k<");
								sb.Append(pattern, i + 4, end - i - 4);
								sb.Append('>');
								i = end;
								continue;
							}
						}
					}
					// 转义字符
					if (c == '\\' && i + 1 < pattern.Length)
					{
						var n = pattern[i + 1];
						switch (n)
						{
							case 'd':
								sb.Append(inClass ? "0-9" : "[0-9]");
								i++;
								continue;
							case 'w':
								sb.Append(inClass ? "0-9A-Za-z_" : "[0-9A-Za-z_]");
								i++;
								continue;
							case 's':
								sb.Append(inClass ? " \\t\\n\\r\\f\\v" : "[ \\t\\n\\r\\f\\v]");
								i++;
								continue;
							case 'D':
							case 'W':
							case 'S':
								// 字符类中无法直接表达取反的含义，保持原样
								if (!inClass)
								{
									sb.Append('[');
									sb.Append('^');
									switch (n)
									{
										case 'D':
											sb.Append("0-9");
											break;
										case 'W':
											sb.Append("0-9A-Za-z_");
											break;
										default:
											sb.Append(" \\t\\n\\r\\f\\v");
											break;
									}
									sb.Append(']');
									i++;
									continue;
								}
								break;
							default:
								break;
						}
						sb.Append(c);
						sb.Append(n);
						i++;
						continue;
					}
					if (c == '[')
					{
						inClass = true;
					}
					else if (c == ']')
					{
						inClass = false;
					}
					sb.Append(c);
				}
				return sb.ToString();
			}

			/// <summary>
			/// 将Go的替换模板转换为.NET的替换模板
			/// <para>Go中名称捕获组的引用写法为$name，.NET中为${name}</para>
			/// </summary>
			public static string ConvertTemplate(string template)
			{
				if (string.IsNullOrEmpty(template)) return template ?? string.Empty;
				var sb = new StringBuilder(template.Length + 8);
				for (int i = 0; i < template.Length; i++)
				{
					var c = template[i];
					if (c != '$' || i + 1 >= template.Length)
					{
						sb.Append(c);
						continue;
					}
					var n = template[i + 1];
					if (n == '$')
					{
						// $$ 表示字面量的$
						sb.Append("$$");
						i++;
						continue;
					}
					if (n == '{')
					{
						var end = template.IndexOf('}', i + 2);
						if (end > 0)
						{
							sb.Append(template, i, end - i + 1);
							i = end;
							continue;
						}
					}
					if (char.IsLetterOrDigit(n) || n == '_')
					{
						int j = i + 1;
						while (j < template.Length && (char.IsLetterOrDigit(template[j]) || template[j] == '_')) j++;
						sb.Append("${");
						sb.Append(template, i + 1, j - i - 1);
						sb.Append('}');
						i = j - 1;
						continue;
					}
					sb.Append(c);
				}
				return sb.ToString();
			}
		}
	}
}