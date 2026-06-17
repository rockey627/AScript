using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace AScript.Lang.JavaScript
{
	public static class JavaScriptStringExtensions
	{
		public static string String_fromCharCode(params long[] codes)
		{
			var sb = new StringBuilder(codes.Length);
			for (int i = 0; i < codes.Length; i++)
			{
				sb.Append((char)Convert.ToInt32(codes[i]));
			}
			return sb.ToString();
		}

		public static Expression<Func<string, string, bool>> startsWith() => (s, p) => s.StartsWith(p);
		public static Expression<Func<string, string, bool>> endsWith() => (s, p) => s.EndsWith(p);
		public static Expression<Func<string, string, bool>> includes() => (s, p) => s.Contains(p);
		// indexOf扩展方法有多个重载方法
		public static LambdaExpression[] indexOf()
		{
			Expression<Func<string, string, long>> expr1 = (s, p) => s.IndexOf(p);
			Expression<Func<string, string, long, long>> expr2 = (s, p, start) => s.IndexOf(p, (int)start);
			return new LambdaExpression[] { expr1, expr2 };
		}
		public static LambdaExpression[] lastIndexOf()
		{
			Expression<Func<string, string, long>> expr1 = (s, p) => s.LastIndexOf(p);
			Expression<Func<string, string, long, long>> expr2 = (s, p, start) => s.LastIndexOf(p, (int)start);
			return new LambdaExpression[] { expr1, expr2 };
		}
		public static Expression<Func<string, string, long>> search()=> (s, p) => s.LastIndexOf(p);

		public static List<object> match(string s, string p)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(p)) return null;
			var match = Regex.Match(s, p);
			if (!match.Success) return null;
			return new List<object> { match.Value };
		}

		public static List<object> match(string s, JavaScriptRegexPattern p)
		{
			if (string.IsNullOrEmpty(s)) return null;
			if (p.SearchAll)
			{
				var matches = Regex.Matches(s, p.Pattern, p.Options);
				if (matches.Count == 0) return null;
				var list = new List<object>(matches.Count);
				for (int i = 0; i < matches.Count; i++)
				{
					var match = matches[i];
					list.Add(match.Value);
				}
				return list;
			}
			else
			{
				var match = Regex.Match(s, p.Pattern, p.Options);
				if (!match.Success) return null;
				return new List<object> { match.Value };
			}
		}

		public static string replace(string s, string pattern, string value)
		{
			// 只替换第1个匹配项，string.Replace是替换所有匹配项
			if (string.IsNullOrEmpty(s)) return s;
			if (string.IsNullOrEmpty(pattern)) return s;
			int index = s.IndexOf(pattern);
			if (index < 0) return s;
			return s.Substring(0, index) + value + s.Substring(index + pattern.Length);
		}

		public static string replace(string s, JavaScriptRegexPattern p, string value)
		{
			if (string.IsNullOrEmpty(s)) return s;
			if (p.SearchAll)
			{
				return Regex.Replace(s, p.Pattern, value, p.Options);
			}
			return new Regex(p.Pattern, p.Options).Replace(s, value, 1);
		}

		public static long search(string s, JavaScriptRegexPattern p)
		{
			if (string.IsNullOrEmpty(s)) return -1L;
			var match = Regex.Match(s, p.Pattern, p.Options);
			if (!match.Success) return -1L;
			return match.Index;
		}

		public static List<object> split(string s, string pattern)
		{
			if (string.IsNullOrEmpty(s)) return new List<object>();
			if (string.IsNullOrEmpty(pattern))
			{
				return s.Select(a => (object)a.ToString()).ToList();
			}
#if NETSTANDARD2_1_OR_GREATER
			return s.Split(pattern).Select(a => (object)a).ToList();
#else
			return s.Split(new[] { pattern }, StringSplitOptions.None).Select(a => (object)a).ToList();
#endif
		}

		public static string repeat(string s, long count)
		{
			if (string.IsNullOrEmpty(s)) return string.Empty;
			if (count <= 0) return string.Empty;
			if (count == 1) return s;
			if (count == 2) return s + s;
			if (count == 3) return s + s + s;
			if (count == 4) return s + s + s + s;
			var arr = new string[count];
			for (int i = 0; i < count; i++)
			{
				arr[i] = s;
			}
			return string.Concat(arr);
		}

	}
}
