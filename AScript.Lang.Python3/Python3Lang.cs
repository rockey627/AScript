using AScript.Functions;
using AScript.Lang.Python3.Operators;
using AScript.Lang.Python3.TokenHandlers;
using AScript.Nodes;
using AScript.Operators;
using AScript.Readers;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AScript.Lang.Python3
{
	public class Python3Lang : ScriptLang
	{
		public static readonly Python3Lang Instance = new Python3Lang();

		public static readonly HashSet<string> EndTokens = new HashSet<string>() { ":", "\n" };

		public Python3Lang()
		{
			AddType<long>("int");
			AddType<double>("float");
			AddType<string>("str");
			AddType<bool>("bool");

			//AddFunc("=", Python3AssignOperator.Instance);
			//AddFunc(":=", Python3AssignOperator.Instance);
			AddFunc("=", AssignOperator.Instance);
			// 海象运算符：同时进行赋值和返回赋值的值
			AddFunc(":=", AssignOperator.Instance);
			AddFunc("+=", PlusAssignOperator.Instance);
			AddFunc("-=", SubtractAssignOperator.Instance);
			AddFunc("*=", MultiplyAssignOperator.Instance);
			AddFunc("**=", PowerAssignOperator.Instance);
			AddFunc("/=", Python3DivideAssignOperator.Instance);
			AddFunc("//=", Python3Divide2AssignOperator.Instance);
			AddFunc("%=", ModuloAssignOperator.Instance);
			AddFunc("^=", XOrAssignOperator.Instance);
			AddFunc("&=", AndAssignOperator.Instance);
			AddFunc("|=", OrAssignOperator.Instance);
			AddFunc("?=", QuestionAssignOperator.Instance);
			AddFunc(">>=", RightShiftAssignOperator.Instance);
			AddFunc("<<=", LeftShiftAssignOperator.Instance);
			AddFunc("+", PlusOperator.Instance);
			AddFunc("-", SubtractOperator.Instance);
			AddFunc("*", MultiplyOperator.Instance);
			AddFunc("**", PowerOperator.Instance);
			AddFunc("/", Python3DivideOperator.Instance);
			AddFunc("//", Python3Divide2Operator.Instance);
			AddFunc("%", ModuloOperator.Instance);
			AddFunc("&", AndOperator.Instance);
			AddFunc("|", OrOperator.Instance);
			AddFunc("^", XOrOperator.Instance);
			AddFunc("~", NotOperator.Instance);
			AddFunc("<<", LeftShiftOperator.Instance);
			AddFunc(">>", RightShiftOperator.Instance);
			AddFunc("++", IncrementAssignOperator.Instance);
			AddFunc("--", DecrementAssignOperator.Instance);
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc("==", EqualOperator.Instance);
			AddFunc("!=", NotEqualOperator.Instance);
			AddFunc("and", AndAlsoOperator.Instance);
			AddFunc("or", OrElseOperator.Instance);
			AddFunc(".", DotOperator.Instance);
			AddFunc("[]", IndexOperator.Instance);
			AddFunc("[:]", IndexStartEndOperator.Instance);

			//AddFunc<ScriptContext, string, object>("exec", Exec);
			AddFunc("exec", EvalFunction.Instance);

			AddFunc<object, Python3Type>("type", a => new Python3Type(a?.GetType()));
			AddFunc<object, string>("str", a => a?.ToString());
			AddFunc<object, long>("int", a => Convert.ToInt64(a));
			AddFunc<object, double>("float", a => Convert.ToDouble(a));
			AddFunc<object, bool>("bool", a => Convert.ToBoolean(a));
			AddFunc<long, IReadOnlyList<long>>("range", Range);
			AddFunc<long, long, IReadOnlyList<long>>("range", Range);
			AddFunc<IList, IList, bool>("==", List_Equal);
			AddFunc<List<object>, List<object>, List<object>>("+", List_plus);
			AddFunc<IList, long>("len", list => list == null ? 0L : (long)list.Count);
			AddFunc<List<object>, object>("pop", List_pop);
			AddFunc<List<object>, long, object>("pop", List_pop);
			AddAction<object>("print", Println);
			AddAction<object, object>("print", Println);
			AddAction<object, object, object>("print", Println);
			AddAction<object, object, object, object>("print", Println);
			AddAction<object, object, object, object, object>("print", Println);
			AddAction<List<object>, object>("remove", List_remove);
			AddAction<List<object>, object>("append", (list, value) => list.Add(value));
			AddAction<List<object>, long, object>("insert", (list, index, value) => list.Insert((int)index, value));
#if NETFRAMEWORK
			AddFunc<IEnumerable, IEnumerable<Tuple<long, object>>>("enumerate", enumerate);
#else
			AddFunc<IEnumerable, IEnumerable<(long, object)>>("enumerate", enumerate);
#endif

			AddTokenHandler("?", QuestionIIFTokenHandler.Instance);
			AddTokenHandler("[", Python3BracketTokenHandler.Instance);
			AddTokenHandler("True", BoolTokenHandler.Instance);
			AddTokenHandler("False", BoolTokenHandler.Instance);
			AddTokenHandler("None", NullTokenHandler.Instance);
			AddTokenHandler("and", AndAlsoTokenHandler.Instance);
			AddTokenHandler("or", OrElseTokenHandler.Instance);
			AddTokenHandler("if", Python3IfTokenHandler.Instance);
			AddTokenHandler("for", Python3ForTokenHandler.Instance);
			AddTokenHandler("def", Python3DefTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("break", BreakTokenHandler.Instance);
			AddTokenHandler("continue", ContinueTokenHandler.Instance);
			// 字符串内插值：f'{m},{n}'
			AddTokenHandler("f", StringInterpolationTokenHandler.Instance);
		}

		public override ITokenStream GetTokenStream(CharReader charReader)
		{
			return new Python3TokenStream(charReader);
		}

		public override ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return Python3SyntaxAnalyzer.Instance;
		}

		public override int? GetOperatorPriority(string op)
		{
			switch (op)
			{
				case ":=":
					return DefaultSyntaxAnalyzer.OperatorPriorities["="];
				case "//":
					return DefaultSyntaxAnalyzer.OperatorPriorities["/"];
				case "//=":
					return DefaultSyntaxAnalyzer.OperatorPriorities["/="];
				default:
					break;
			}
			return base.GetOperatorPriority(op);
		}

		public override bool IsDynamic()
		{
			return true;
		}

		//private static object Exec(ScriptContext context, string expression)
		//{
		//	var engine = ScriptEngine.GetCurrent(context);
		//	if (engine == null) throw new Exception("unkown inner ScriptEngine");
		//	return engine.Eval(context, expression);
		//}

		private static bool List_Equal(IList list1, IList list2)
		{
			if (list1 == null) return list2 == null;
			if (list1.Count != list2.Count) return false;
			for (int i = 0; i < list1.Count; i++)
			{
				if (!Equal2(list1[i], list2[i])) return false;
			}
			return true;
		}

		private static bool Equal2(object a, object b)
		{
			if (a == null) return b == null;
			if (a is IList list1)
			{
				if (b is IList list2)
				{
					if (list1.Count != list2.Count) return false;
					for (int i = 0; i < list1.Count; i++)
					{
						if (!Equal2(list1[i], list2[i])) return false;
					}
					return true;
				}
				return false;
			}
			return a.Equals(b);
		}

		private static void Println(object obj)
		{
			Print(obj);
			Console.WriteLine();
		}

		private static void Println(object obj1, object obj2)
		{
			Print(obj1);
			Print(obj2);
			Console.WriteLine();
		}

		private static void Println(object obj1, object obj2, object obj3)
		{
			Print(obj1);
			Print(obj2);
			Print(obj3);
			Console.WriteLine();
		}

		private static void Println(object obj1, object obj2, object obj3, object obj4)
		{
			Print(obj1);
			Print(obj2);
			Print(obj3);
			Print(obj4);
			Console.WriteLine();
		}

		private static void Println(object obj1, object obj2, object obj3, object obj4, object obj5)
		{
			Print(obj1);
			Print(obj2);
			Print(obj3);
			Print(obj4);
			Print(obj5);
			Console.WriteLine();
		}

		private static void Print(object obj)
		{
			if (obj == null) return;
			if (obj is string s)
			{
				Console.Write(s);
				return;
			}
			if (obj is IList list)
			{
				Console.Write('[');
				for (int i = 0; i < list.Count; i++)
				{
					Print(list[i]);
					if (i < list.Count - 1)
					{
						Console.Write(", ");
					}
				}
				Console.Write(']');
				return;
			}
			Console.Write(obj.ToString());
		}

		private static IReadOnlyList<long> Range(long stop)
		{
			var arr = new long[stop];
			for (int i = 0; i < stop; i++)
			{
				arr[i] = i;
			}
			return new ReadOnlyCollection<long>(arr);
		}

		private static IReadOnlyList<long> Range(long start, long stop)
		{
			var arr = new long[stop - start];
			for (long i = start; i < stop; i++)
			{
				arr[i - start] = i;
			}
			return new ReadOnlyCollection<long>(arr);
		}

		/// <summary>
		/// 2个列表相加
		/// </summary>
		/// <param name="list1"></param>
		/// <param name="list2"></param>
		/// <returns></returns>
		private static List<object> List_plus(List<object> list1, List<object> list2)
		{
			var list = new List<object>((list1 == null ? 0 : list1.Count) + (list2 == null ? 0 : list2.Count));
			if (list1 != null) list.AddRange(list1);
			if (list2 != null) list.AddRange(list2);
			return list;
		}

		/// <summary>
		/// 列表移除最后1个元素
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private static object List_pop(List<object> list)
		{
			return List_pop(list, -1L);
		}

		/// <summary>
		/// 列表移除1个元素
		/// </summary>
		/// <param name="list"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private static object List_pop(List<object> list, long index)
		{
			if (index < 0) index += list.Count;
			int intIndex = (int)index;
			var v = list[intIndex];
			list.RemoveAt(intIndex);
			return v;
		}

		private static void List_remove(List<object> list, object v)
		{
			list.Remove(v);
		}

#if NETFRAMEWORK
		private static IEnumerable<Tuple<long, object>> enumerate(IEnumerable list)
		{
			if (list is IList a)
			{
				for (int i = 0; i < a.Count; i++)
				{
					yield return Tuple.Create((long)i, a[i]);
				}
			}
			else
			{
				int i = 0;
				foreach (var item in list)
				{
					yield return Tuple.Create((long)(i++), item);
				}
			}
		}
#else
		private static IEnumerable<(long, object)> enumerate(IEnumerable list)
		{
			if (list is IList a)
			{
				for (int i = 0; i < a.Count; i++)
				{
					yield return ((long)i, a[i]);
				}
			}
			else
			{
				int i = 0;
				foreach (var item in list)
				{
					yield return ((long)(i++), item);
				}
			}
		}
#endif

		public static TreeBuilder BuildSubBlock(int parentColumn, DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null)
		{
			var token = tokenReader.Read();
			if (!token.HasValue) return null;
			if (token.Value.Column <= parentColumn)
			{
				tokenReader.Push(token.Value);
				return null;
			}

			var builder = ignore ? null : new TreeBuilder();
			int column = token.Value.Column;
			while (token.HasValue && token.Value.Column == column)
			{
				tokenReader.Push(token.Value);
				var statement = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens: endTokens);
				if (!ignore)
				{
					builder.Add(buildContext, scriptContext, options, control, statement);
				}
				token = tokenReader.Read();
			}
			if (token.HasValue)
			{
				tokenReader.Push(token.Value);
			}

			return builder;
		}
	}
}
