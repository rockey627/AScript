using AScript.Functions;
using AScript.Lang.Lua.Operators;
using AScript.Lang.Lua.TokenHandlers;
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

		//internal static readonly HashSet<string> EndTokens = new HashSet<string>() { "end", "else", "elseif", "until" };

		internal static readonly HashSet<string> EndTokens_do = new HashSet<string> { "do" };
		internal static readonly HashSet<string> EndTokens_end = new HashSet<string> { "end" };
		internal static readonly HashSet<string> EndTokens_until = new HashSet<string> { "until" };
		internal static readonly HashSet<string> EndTokens_then = new HashSet<string> { "then" };
		internal static readonly HashSet<string> EndTokens_else = new HashSet<string> { "else", "elseif", "end" };

		protected LuaLang()
		{
			AddType<long>("integer", false);
			AddType<double>("number", false);
			AddType<string>("string", false);
			AddType<bool>("boolean", false);
			AddType<LuaTable>("table");

			SetObjectMemberEnabled(typeof(List<object>), false);

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
			AddFunc("/", new DivideOperator(isDouble: true));
			AddFunc("%", ModuloOperator.Instance);
			AddFunc("^", PowerOperator.Instance);
			AddFunc("//", LuaFloorDivideOperator.Instance);
			AddFunc("~", NotOperator.Instance);

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
			AddFunc("[]", new IndexOperator(false, true));
			AddFunc("..", ConcatFunction.Instance);
			AddFunc("#", new LengthFunction(typeof(long)));
			AddFunc<LuaTable, LuaTable, string>("..", (table1, table2) => LuaTable.concat(table1, table2));
			AddFunc<LuaTable, long>("#", table => table.ArrayLength);

			AddFunc(typeof(Extensions.LuaCommonExtensions));
			AddFunc(typeof(Extensions.LuaStringExtensions));

			// Token处理器
			AddTokenHandler("local", LuaLocalTokenHandler.Instance);
			AddTokenHandler("null", NullTokenHandler.Instance);
			AddTokenHandler("nil", NullTokenHandler.Instance);
			AddTokenHandler("and", new OperatorTokenHandler("&&"));
			AddTokenHandler("or", new OperatorTokenHandler("||"));
			AddTokenHandler("not", new OperatorTokenHandler("!") { DataCount = 1, Prefix = true });
			AddTokenHandler("true", BoolTokenHandler.Instance);
			AddTokenHandler("false", BoolTokenHandler.Instance);
			AddTokenHandler("if", LuaIfTokenHandler.Instance);
			AddTokenHandler("while", LuaWhileTokenHandler.Instance);
			AddTokenHandler("repeat", LuaRepeatTokenHandler.Instance);
			AddTokenHandler("for", LuaForTokenHandler.Instance);
			AddTokenHandler("in", InTokenHandler.Instance);
			AddTokenHandler("function", LuaFunctionTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("break", BreakTokenHandler.Instance);
			AddTokenHandler("continue", ContinueTokenHandler.Instance);
			AddTokenHandler("[", new BracketTokenHandler(typeof(List<object>)));
			//AddTokenHandler("#", new OperatorTokenHandler(".") { DataCount = 1, Prefix = true });
			AddTokenHandler("#", LuaLenTokenHandler.Instance);
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
				case "#":
					return DefaultSyntaxAnalyzer.OperatorPriorities["."];
				default:
					break;
			}
			return base.GetOperatorPriority(op);
		}

	}
}
