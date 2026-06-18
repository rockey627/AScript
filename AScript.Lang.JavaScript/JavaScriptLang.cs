using AScript.Functions;
using AScript.Lang.JavaScript.Extensions;
using AScript.Lang.JavaScript.TokenHandlers;
using AScript.Operators;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;
using System.Linq;

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
			AddType("String", typeof(string));
			AddType("Array", typeof(List<object>));
			AddType("Set", typeof(HashSet<object>));
			AddType("Map", typeof(Dictionary<object, object>));
			AddType("Date", typeof(DateTime));
			AddType("Math", typeof(Math));

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
			AddFunc("concat", ConcatFunction.Instance);
			AddFunc("includes", new ContainsFunction());
			AddFunc("get_length", new LengthFunction(typeof(long)));

			AddFunc(typeof(JavaScriptDateExtensions));
			AddFunc(typeof(JavaScriptMathExtensions));
			AddFunc(typeof(JavaScriptStringExtensions));
			AddFunc(typeof(JavaScriptArrayExtensions));

			AddLambda<Func<List<object>, string>>("join", list => string.Join("", list));
			AddLambda<Func<List<object>, string, string>>("join", (list,separator) => string.Join(separator, list));
			AddLambda<Func<List<object>, object, long>>("indexOf", (list, obj) => (long)list.IndexOf(obj));
			AddFunc<List<object>, List<object>>("reverse", list => { list.Reverse(); return list; });
			AddFunc<List<object>, object, List<object>>("fill", List_fill);
			AddFunc<List<object>, Func<object, bool>, List<object>>("filter", List_filter);
			AddFunc<List<object>, Func<object, bool>, long>("findIndex", List_findIndex);
			AddFunc<List<object>, Func<object, object>, List<object>>("map", List_map);
			AddFunc<List<object>, Func<object, object, object>, object>("reduce", List_reduce1);
			AddFunc<List<object>, Func<object, object, object>, object, object>("reduce", List_reduce2);
			AddAction<List<object>, Action<object>>("forEach", List_forEach);
			AddFunc<List<object>, object>("pop", List_pop);
			AddFunc<List<object>, object>("shift", List_shift);
			AddFunc<List<object>, object, long>("push", List_push);
			AddFunc<List<object>, object[], long>("push", List_push);
			AddFunc<List<object>, object, long>("unshift", List_unshift);
			AddFunc<List<object>, object[], long>("unshift", List_unshift);
			AddFunc("slice", IndexStartEndOperator.Instance);
			AddFunc<List<object>, long, long, object[], List<object>>("splice", List_splice);
			AddFunc<List<object>, long, List<object>>("splice", List_splice);
			AddFunc<List<object>, long, long, List<object>>("splice", List_splice);
			AddFunc(typeof(Enumerable), method =>
			{
				if (method.Name == "All") return "every";
				if (method.Name == "Any") return "some";
				if (method.Name == "FirstOrDefault") return "find";
				return null;
			});

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
			AddTokenHandler("`", JavaScriptStringInterpolationTokenHandler.Instance);
		}

		public override bool IsDynamic()
		{
			return true;
		}

		public override bool IsObjectMemberEnabled(Type objType)
		{
			if (this.ObjectMemberEnabledDict.TryGetValue(objType, out var enable))
			{
				return enable;
			}
			return false;
		}

		public override ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return JavaScriptSyntaxAnalyzer.Instance;
		}

		private static List<object> List_filter(List<object> list, Func<object, bool> func)
		{
			return list.Where(func).ToList();
		}

		private static List<object> List_map(List<object> list, Func<object, object> func)
		{
			return list.Select(func).ToList();
		}

		private static object List_reduce1(List<object> list, Func<object, object, object> func)
		{
			if (list.Count == 0) return null;
			if (list.Count == 1) return list[0];
			object r = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				r = func(r, list[i]);
			}
			return r;
		}

		private static object List_reduce2(List<object> list, Func<object, object, object> func, object init)
		{
			if (list.Count == 0) return init;
			object r = init;
			for (int i = 0; i < list.Count; i++)
			{
				r = func(r, list[i]);
			}
			return r;
		}

		private static long List_findIndex(List<object> list, Func<object, bool> func)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (func(list[i])) return i;
			}
			return -1L;
		}

		private static List<object> List_fill(List<object> list, object v)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = v;
			}
			return list;
		}

		private static void List_forEach(List<object> list, Action<object> action)
		{
			list.ForEach(action);
		}

		private static object List_pop(List<object> list)
		{
			if (list == null || list.Count == 0) return null;
			int index = list.Count - 1;
			object v = list[index];
			list.RemoveAt(index);
			return v;
		}

		private static object List_shift(List<object> list)
		{
			if (list == null || list.Count == 0) return null;
			object v = list[0];
			list.RemoveAt(0);
			return v;
		}

		private static long List_push(List<object> list, object v)
		{
			list.Add(v);
			return list.Count;
		}

		private static long List_push(List<object> list, params object[] vs)
		{
			if (vs == null || vs.Length == 0) return list.Count;
			if (vs.Length == 1) list.Add(vs[0]);
			else list.AddRange(vs);
			return list.Count;
		}

		private static long List_unshift(List<object> list, object v)
		{
			list.Insert(0, v);
			return list.Count;
		}

		private static long List_unshift(List<object> list, params object[] vs)
		{
			if (vs == null || vs.Length == 0) return list.Count;
			if (vs.Length == 1) list.Insert(0, vs[0]);
			else list.InsertRange(0, vs);
			return list.Count;
		}

		private static List<object> List_splice(List<object> list, long index)
		{
			int count = list.Count - (int)index;
			return List_splice(list, index, count);
		}

		private static List<object> List_splice(List<object> list, long index, long count)
		{
			if (count == 0) return new List<object>();
			var result = new List<object>((int)count);
			for (int i = 0; i < count; i++)
			{
				result.Add(list[i + (int)index]);
			}
			list.RemoveRange((int)index, (int)count);
			return result;
		}

		private static List<object> List_splice(List<object> list, long index, long count, params object[] addingList)
		{
			var result = List_splice(list, index, count);
			if (addingList != null && addingList.Length > 0)
			{
				list.InsertRange((int)index, addingList);
			}
			return result;
		}
	}
}
