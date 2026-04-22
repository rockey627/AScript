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
			AddFunc("=", Python3AssignOperator.Instance);
			// 海象运算符：同时进行赋值和返回赋值的值
			AddFunc(":=", Python3AssignOperator.Instance);
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
			AddFunc("[]", IndexOperator.Instance);
			AddFunc("[:]", IndexStartEndOperator.Instance);

			AddFunc<ScriptContext, string, object>("exec", Exec);
			AddFunc<long, IReadOnlyList<long>>("range", Range);
			AddFunc<long, long, IReadOnlyList<long>>("range", Range);
			AddFunc<List<object>, List<object>, List<object>>("+", List_Plus);
			AddFunc<IList, long>("len", list => list == null ? 0L : (long)list.Count);
			AddAction<object>("print", Println);
			AddAction<IList, object>("append", (list, value) => list.Add(value));
			AddAction<IList, long, object>("insert", (list, index, value) => list.Insert((int)index, value));

			AddTokenHandler("?", QuestionIIFTokenHandler.Instance);
			AddTokenHandler("[", Python3BracketTokenHandler.Instance);
			AddTokenHandler("True", BoolTokenHandler.Instance);
			AddTokenHandler("False", BoolTokenHandler.Instance);
			AddTokenHandler("and", AndAlsoTokenHandler.Instance);
			AddTokenHandler("or", OrElseTokenHandler.Instance);
			AddTokenHandler("if", Python3IfTokenHandler.Instance);
			AddTokenHandler("def", Python3DefTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			// 字符串内插值：f'{m},{n}'
			AddTokenHandler("f", StringInterpolationTokenHandler.Instance);
			// python中不能使用#lang，用@lang代替
			AddTokenHandler("@lang", new LangTokenHandler("@end"));
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

		private static object Exec(ScriptContext context, string expression)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(context, expression);
		}

		private static void Println(object obj)
		{
			Print(obj);
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

		private static List<object> List_Plus(List<object> list1, List<object> list2)
		{
			var list = new List<object>((list1 == null ? 0 : list1.Count) + (list2 == null ? 0 : list2.Count));
			if (list1 != null) list.AddRange(list1);
			if (list2 != null) list.AddRange(list2);
			return list;
		}

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
