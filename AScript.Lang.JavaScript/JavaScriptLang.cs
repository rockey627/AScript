using AScript.Functions;
using AScript.Lang.JavaScript.TokenHandlers;
using AScript.Operators;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AScript.Lang.JavaScript
{
	/// <summary>
	/// JavaScript脚本语言
	/// </summary>
	public class JavaScriptLang : ScriptLang
	{
		public static readonly JavaScriptLang Instance = new JavaScriptLang();

		protected JavaScriptLang()
		{
			AddType("var", typeof(object));
			AddType("let", typeof(object));
			AddType("const", typeof(object));
			AddType("Array", typeof(Array));
			AddType("Set", typeof(HashSet<object>));
			AddType("Map", typeof(Dictionary<object, object>));

			AddFunc("=", AssignOperator.Instance);
			AddFunc("+=", PlusAssignOperator.Instance);
			AddFunc("-=", SubtractAssignOperator.Instance);
			AddFunc("*=", MultiplyAssignOperator.Instance);
			AddFunc("**=", PowerAssignOperator.Instance);
			AddFunc("/=", new DivideAssignOperator(true));
			AddFunc("%=", ModuloAssignOperator.Instance);
			AddFunc("^=", XOrAssignOperator.Instance);
			AddFunc("&=", AndAssignOperator.Instance);
			AddFunc("|=", OrAssignOperator.Instance);
			AddFunc("?=", QuestionAssignOperator.Instance);
			AddFunc(">>=", RightShiftAssignOperator.Instance);
			AddFunc("<<=", LeftShiftAssignOperator.Instance);
			AddFunc("??", QuestionQuestionOperator.Instance);
			AddFunc("?", QuestionIIFOperator.Instance);
			AddFunc("+", PlusOperator.Instance);
			AddFunc("-", SubtractOperator.Instance);
			AddFunc("*", MultiplyOperator.Instance);
			AddFunc("**", PowerOperator.Instance);
			AddFunc("/", new DivideOperator(true));
			AddFunc("%", ModuloOperator.Instance);
			AddFunc("&", AndOperator.Instance);
			AddFunc("|", OrOperator.Instance);
			AddFunc("^", XOrOperator.Instance);
			AddFunc("~", NotOperator.Instance);
			AddFunc("<<", LeftShiftOperator.Instance);
			AddFunc(">>", RightShiftOperator.Instance);
			AddFunc("++", IncrementAssignOperator.Instance);
			AddFunc("--", DecrementAssignOperator.Instance);
			AddFunc("!", BoolNotOperator.Instance);
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc("==", EqualOperator.Instance);
			AddFunc("!=", NotEqualOperator.Instance);
			AddFunc("&&", AndAlsoOperator.Instance);
			AddFunc("||", OrElseOperator.Instance);
			AddFunc(".", DotOperator.Instance);
			AddFunc("?.", new DotOperator(true));
			AddFunc("[]", IndexOperator.Instance);
			AddFunc("[:]", IndexStartEndOperator.Instance);

			AddFunc("eval", EvalFunction.Instance);
			AddFunc("concat", StringConcatFunction.Instance);

			AddLambda<Func<string, string, bool>>("startsWith", (s, p) => s.StartsWith(p));
			AddLambda<Func<string, string, bool>>("endsWith", (s, p) => s.EndsWith(p));
			AddLambda<Func<string, string, bool>>("includes", (s, p) => s.Contains(p));
			AddLambda<Func<string, string, long>>("indexOf", (s, p) => s.IndexOf(p));
			AddLambda<Func<string, string, long, long>>("indexOf", (s, p, start) => s.IndexOf(p, (int)start));
			AddLambda<Func<string, string, long>>("lastIndexOf", (s, p) => s.LastIndexOf(p));
			AddLambda<Func<string, string, long, long>>("lastIndexOf", (s, p, start) => s.LastIndexOf(p, (int)start));
			AddLambda<Func<string, string, long>>("search", (s, p) => s.IndexOf(p));
			AddFunc<string, JavaScriptRegexPattern, long>("search", String_search);
			AddLambda<Func<string, long, string>>("substr", (s, start) => s.Substring((int)(start < 0 ? s.Length + start : start)));
			AddLambda<Func<string, long, long, string>>("substr", (s, start, count) => s.Substring((int)(start < 0 ? s.Length + start : start), (int)count));
			AddLambda<Func<string, long, string>>("substring", (s, start) => s.Substring((int)start));
			AddLambda<Func<string, long, long, string>>("substring", (s, start, end) => s.Substring((int)start, (int)(end - start)));
			AddLambda<Func<string, long, string>>("slice", (s, start) => s.Substring((int)(start < 0 ? s.Length + start : start)));
			AddLambda<Func<string, long, long, string>>("slice", (s, start, end) => s.Substring((int)(start < 0 ? s.Length + start : start), (int)((end < 0 ? s.Length + end : end) - (start < 0 ? s.Length + start : start))));
			AddLambda<Func<string, string>>("toLowerCase", s => s.ToLower());
			AddLambda<Func<string, string>>("toUpperCase", s => s.ToUpper());
			AddLambda<Func<string, string>>("trim", s => s.Trim());
			AddLambda<Func<string, string>>("trimStart", s => s.TrimStart());
			AddLambda<Func<string, string>>("trimEnd", s => s.TrimEnd());
			AddFunc<string, string, List<object>>("match", String_match);
			AddFunc<string, JavaScriptRegexPattern, List<object>>("match", String_match);
			AddFunc<string, long, string>("charAt", (s, index) => index < 0 || index >= s.Length ? string.Empty : s[(int)index].ToString());
			AddFunc<string, long, long>("charCodeAt", (s, index) => index < 0 || index >= s.Length ? -1L : (long)(int)s[(int)index]);
			AddFunc<string, string, string, string>("replace", String_replace);
			AddFunc<string, JavaScriptRegexPattern, string, string>("replace", String_replace);
			AddFunc<string, string, List<object>>("split", String_split);
			//AddFunc<string, long, string>("repeat", (s, count) => count <= 0 || string.IsNullOrEmpty(s) ? "" : string.);

			AddTokenHandler("??", LazyTokenHandler.Instance);
			AddTokenHandler("?=", LazyTokenHandler.Instance);
			AddTokenHandler("?", QuestionIIFTokenHandler.Instance);
			AddTokenHandler("[", new BracketTokenHandler(typeof(List<object>)));
			AddTokenHandler("null", NullTokenHandler.Instance);
			AddTokenHandler("true", BoolTokenHandler.Instance);
			AddTokenHandler("false", BoolTokenHandler.Instance);
			AddTokenHandler("new", NewTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("break", BreakTokenHandler.Instance);
			AddTokenHandler("continue", ContinueTokenHandler.Instance);
			AddTokenHandler("if", IfTokenHandler.Instance);
			AddTokenHandler("else", IfTokenHandler.Instance);
			AddTokenHandler("while", WhileTokenHandler.Instance);
			AddTokenHandler("for", JavaScriptForTokenHandler.Instance);
			AddTokenHandler("function", JavaScriptFunctionTokenHandler.Instance);
			AddTokenHandler("/", JavaScriptRegexPatternTokenHandler.Instance);
		}

		public override bool IsDynamic()
		{
			return true;
		}

		public override bool IsObjectMethodEnabled(Type objType)
		{
			return false;
		}

		public override bool IsObjectPropertyEnabled(Type objType)
		{
			return objType == typeof(ExpandoObject);
		}

		public override ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return JavaScriptSyntaxAnalyzer.Instance;
		}

		private static List<object> String_match(string s, string p)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(p)) return null;
			var match = Regex.Match(s, p);
			if (!match.Success) return null;
			return new List<object> { match.Value };
		}

		private static List<object> String_match(string s, JavaScriptRegexPattern p)
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

		private static string String_replace(string s, string pattern, string value)
		{
			// 只替换第1个匹配项，string.Replace是替换所有匹配项
			if (string.IsNullOrEmpty(s)) return s;
			if (string.IsNullOrEmpty(pattern)) return s;
			int index = s.IndexOf(pattern);
			if (index < 0) return s;
			return s.Substring(0, index) + value + s.Substring(index + pattern.Length);
		}

		private static string String_replace(string s, JavaScriptRegexPattern p, string value)
		{
			if (string.IsNullOrEmpty(s)) return s;
			if (p.SearchAll)
			{
				return Regex.Replace(s, p.Pattern, value, p.Options);
			}
			return new Regex(p.Pattern, p.Options).Replace(s, value, 1);
		}

		private static long String_search(string s, JavaScriptRegexPattern p)
		{
			if (string.IsNullOrEmpty(s)) return -1L;
			var match = Regex.Match(s, p.Pattern, p.Options);
			if (!match.Success) return -1L;
			return match.Index;
		}

		private static List<object> String_split(string s, string pattern)
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
	}
}
