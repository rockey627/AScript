using AScript.Functions;
using AScript.Lang.JavaScript.TokenHandlers;
using AScript.Operators;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;
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

			// 内置eval函数
			AddFunc("eval", EvalFunction.Instance);

			AddLambda<Func<string, string, bool>>("startsWith", (s, p) => s.StartsWith(p));
			AddLambda<Func<string, string, bool>>("endsWith", (s, p) => s.EndsWith(p));
			AddLambda<Func<string, string, bool>>("includes", (s, p) => s.Contains(p));
			//AddLambda < Func<string, string, List<object>>("match", (s, p) => p[0] == '/' ? (p.SubString(p.LastIndexOf('/') + 1).Contains('g')) : new List<object> { Regex.Match(p).Value });
			AddLambda<Func<string, string, long>>("indexOf", (s, p) => s.IndexOf(p));
			AddLambda<Func<string, string, long, long>>("indexOf", (s, p, start) => s.IndexOf(p, (int)start));
			AddLambda<Func<string, string, long>>("lastIndexOf", (s, p) => s.LastIndexOf(p));
			AddLambda<Func<string, string, long, long>>("lastIndexOf", (s, p, start) => s.LastIndexOf(p, (int)start));
			AddLambda<Func<string, long, string>>("substr", (s, start) => s.Substring((int)(start < 0 ? s.Length + start : start)));
			AddLambda<Func<string, long, long, string>>("substr", (s, start, count) => s.Substring((int)(start < 0 ? s.Length + start : start), (int)count));
			AddLambda<Func<string, long, string>>("substring", (s, start) => s.Substring((int)start));
			AddLambda<Func<string, long, long, string>>("substring", (s, start, end) => s.Substring((int)start, (int)(end - start)));
			AddLambda<Func<string, long, string>>("slice", (s, start) => s.Substring((int)(start < 0 ? s.Length + start : start)));
			AddLambda<Func<string, long, long, string>>("slice", (s, start, end) => s.Substring((int)(start < 0 ? s.Length + start : start), (int)((end < 0 ? s.Length + end : end) - (start < 0 ? s.Length + start : start))));
			AddFunc<string, string, List<object>>("match", StringMatch);
			AddFunc<string, JavaScriptRegexPattern, List<object>>("match", StringMatch);

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

		public override ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return JavaScriptSyntaxAnalyzer.Instance;
		}

		private static List<object> StringMatch(string s, string p)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(p)) return new List<object>();
			var match = Regex.Match(s, p);
			if (match == null) return new List<object>();
			return new List<object> { match.Value };
		}

		private static List<object> StringMatch(string s, JavaScriptRegexPattern p)
		{
			if (string.IsNullOrEmpty(s)) return new List<object>();
			int lastIndex = p.Value.LastIndexOf('/');
			var p0 = p.Value.Substring(1, lastIndex - 1);
			var p1 = p.Value.Substring(lastIndex + 1);
			var options = p1.Contains('i') ? RegexOptions.IgnoreCase : RegexOptions.None;
			if (p1.Contains('g'))
			{
				var matches = Regex.Matches(s, p0, options);
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
				var match = Regex.Match(s, p0, options);
				if (match == null) return new List<object>();
				return new List<object> { match.Value };
			}
		}
	}
}
