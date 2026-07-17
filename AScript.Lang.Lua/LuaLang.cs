using AScript.Functions;
using AScript.Lang.Lua.Operators;
using AScript.Lang.Lua.TokenHandlers;
using AScript.Nodes;
using AScript.Operators;
using AScript.Readers;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Lua
{
	/// <summary>
	/// Lua脚本语言
	/// </summary>
	public class LuaLang : ScriptLang
	{
		public static readonly LuaLang Instance = new LuaLang();

		internal static readonly HashSet<string> EndTokens = new HashSet<string>() { "\n", "end", "else", "elseif", "until" };

		protected LuaLang()
		{
			AddType<long>("integer");
			AddType<double>("number");
			AddType<string>("string");
			AddType<bool>("boolean");
			AddType<Dictionary<object, object>>("table");
			AddType<object>("nil");

			// 赋值运算符
			AddFunc("=", AssignOperator.Instance);
			AddFunc("+=", PlusAssignOperator.Instance);
			AddFunc("-=", SubtractAssignOperator.Instance);
			AddFunc("*=", MultiplyAssignOperator.Instance);
			AddFunc("/=", DivideAssignOperator.Instance);
			AddFunc("%=", ModuloAssignOperator.Instance);
			AddFunc("^=", PowerAssignOperator.Instance);

			// 算术运算符
			AddFunc("+", PlusOperator.Instance);
			AddFunc("-", SubtractOperator.Instance);
			AddFunc("*", MultiplyOperator.Instance);
			AddFunc("/", DivideOperator.Instance);
			AddFunc("%", ModuloOperator.Instance);
			AddFunc("^", PowerOperator.Instance);
			AddFunc("//", LuaFloorDivideOperator.Instance);
			AddFunc("-", LuaUnaryMinusOperator.Instance);

			// 关系运算符
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("==", EqualOperator.Instance);
			AddFunc("~=", NotEqualOperator.Instance);

			// 逻辑运算符
			AddFunc("and", AndAlsoOperator.Instance);
			AddFunc("or", OrElseOperator.Instance);
			AddFunc("not", BoolNotOperator.Instance);

			// 其他运算符
			AddFunc(".", DotOperator.Instance);
			AddFunc("[]", IndexOperator.Instance);
			AddFunc("..", LuaConcatOperator.Instance);
			AddFunc("#", LuaLengthOperator.Instance);

			// 内置函数
			AddFunc("print", new PrintFunction());
			AddFunc("type", new TypeFunction());

			// Token处理器
			AddTokenHandler("local", LuaLocalTokenHandler.Instance);
			AddTokenHandler("nil", LuaNilTokenHandler.Instance);
			AddTokenHandler("true", BoolTokenHandler.Instance);
			AddTokenHandler("false", BoolTokenHandler.Instance);
			AddTokenHandler("if", LuaIfTokenHandler.Instance);
			AddTokenHandler("while", LuaWhileTokenHandler.Instance);
			AddTokenHandler("repeat", LuaRepeatTokenHandler.Instance);
			AddTokenHandler("for", LuaForTokenHandler.Instance);
			AddTokenHandler("in", AScript.TokenHandlers.InTokenHandler.Instance);
			AddTokenHandler("function", LuaFunctionTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("break", BreakTokenHandler.Instance);
			AddTokenHandler("continue", ContinueTokenHandler.Instance);
			AddTokenHandler("[", new BracketTokenHandler(typeof(List<object>)));
		}

		public override ITokenStream GetTokenStream(CharReader charReader)
		{
			return new LuaTokenStream(charReader);
		}

		public override ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return LuaSyntaxAnalyzer.Instance;
		}

		public override bool IsDynamic()
		{
			return true;
		}

		public override bool IsTrue(object obj)
		{
			if (obj == null) return false;
			if (obj is bool b) return b;
			var type = obj.GetType();
			if (type.IsValueType)
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return ((dynamic)obj).HasValue;
				}
			}
			return true;
		}

		public override int? GetOperatorPriority(string op)
		{
			switch (op)
			{
				case "//":
					return DefaultSyntaxAnalyzer.OperatorPriorities["/"];
				case "..":
					return DefaultSyntaxAnalyzer.OperatorPriorities["+"];
				case "~=":
					return DefaultSyntaxAnalyzer.OperatorPriorities["!="];
				case "~":
					return DefaultSyntaxAnalyzer.OperatorPriorities["!"];
				default:
					break;
			}
			return base.GetOperatorPriority(op);
		}

		public static ITreeNode BuildSubBlock(int parentColumn, DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null)
		{
			if (endTokens == null) endTokens = EndTokens;
			var token = tokenReader.Read();
			if (!token.HasValue) return null;

			var builder = ignore ? null : new TreeBuilder();
			int column = token.Value.Column;
			while (token.HasValue && token.Value.Column > parentColumn)
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

		// 内置函数类
		private class PrintFunction : IFunctionEvaluator
		{
			public void Eval(FunctionEvalArgs e)
			{
				if (e.Args.Count >= 1)
				{
					var obj = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
					Console.WriteLine(obj);
				}
			}
		}

		private class TypeFunction : IFunctionEvaluator
		{
			public void Eval(FunctionEvalArgs e)
			{
				if (e.Args.Count >= 1)
				{
					var obj = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
					string typeName;
					if (obj == null) typeName = "nil";
					else if (obj is bool) typeName = "boolean";
					else if (obj is long || obj is double) typeName = "number";
					else if (obj is string) typeName = "string";
					else if (obj is List<object>) typeName = "table";
					else if (obj is Dictionary<object, object>) typeName = "table";
					else if (obj is Delegate) typeName = "function";
					else typeName = "userdata";
					e.SetResult(typeName);
				}
			}
		}
	}
}
