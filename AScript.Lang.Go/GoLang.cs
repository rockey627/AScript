using AScript.Functions;
using AScript.Lang.Go.Operators;
using AScript.Lang.Go.TokenHandlers;
using AScript.Nodes;
using AScript.Operators;
using AScript.Readers;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go
{
	/// <summary>
	/// Go脚本语言
	/// </summary>
	public class GoLang : ScriptLang
	{
		public static readonly GoLang Instance = new GoLang();

		internal static readonly HashSet<string> EndTokens_brace = new HashSet<string> { "}" };
		internal static readonly HashSet<string> EndTokens_semi = new HashSet<string> { ";" };

		protected GoLang()
		{
			// 基本类型
			AddType<int>("int", false);
			AddType<byte>("int8", false);
			AddType<short>("int16", false);
			AddType<int>("int32", false);
			AddType<long>("int64", false);
			AddType<uint>("uint", false);
			AddType<sbyte>("uint8", false);
			AddType<ushort>("uint16", false);
			AddType<uint>("uint32", false);
			AddType<ulong>("uint64", false);
			AddType<float>("float32", false);
			AddType<double>("float64", false);
			AddType<bool>("bool", false);
			AddType<string>("string", false);
			AddType<byte>("byte", false);
			AddType<char>("rune", false);
			AddType<object>("any", false);

			// 集合类型
			AddType<Dictionary<object, object>>("map");
			AddType<List<object>>("slice");
			AddType<List<object>>("array");
			//AddType<object[]>("[...]");

			// 赋值运算符
			AddFunc("=", AssignOperator.Instance);
			AddFunc("+=", PlusAssignOperator.Instance);
			AddFunc("-=", SubtractAssignOperator.Instance);
			AddFunc("*=", MultiplyAssignOperator.Instance);
			AddFunc("/=", DivideOperator.Instance);
			AddFunc("%=", ModuloAssignOperator.Instance);
			AddFunc("&=", AndAssignOperator.Instance);
			AddFunc("|=", OrAssignOperator.Instance);
			AddFunc("^=", XOrAssignOperator.Instance);
			AddFunc("<<=", LeftShiftAssignOperator.Instance);
			AddFunc(">>=", RightShiftAssignOperator.Instance);

			// 算术运算符
			AddFunc("+", PlusOperator.Instance);
			AddFunc("-", SubtractOperator.Instance);
			AddFunc("*", MultiplyOperator.Instance);
			AddFunc("/", DivideOperator.Instance);
			AddFunc("%", ModuloOperator.Instance);
			AddFunc("&", AndOperator.Instance);
			AddFunc("|", OrOperator.Instance);
			AddFunc("^", XOrOperator.Instance);
			AddFunc("<<", LeftShiftOperator.Instance);
			AddFunc(">>", RightShiftOperator.Instance);
			AddFunc("&^", new AndNotOperator());

			// 关系运算符
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("==", EqualOperator.Instance);
			AddFunc("!=", NotEqualOperator.Instance);

			// 逻辑运算符
			AddFunc("&&", AndAlsoOperator.Instance);
			AddFunc("||", OrElseOperator.Instance);
			AddFunc("!", BoolNotOperator.Instance);

			// 其他运算符
			AddFunc(".", DotOperator.Instance);
			AddFunc("[]", IndexOperator.Instance);

			// 内置函数
			AddFunc(typeof(Extensions.GoCommonExtensions));

			// Token处理器 - 核心语句
			AddTokenHandler("var", GoVarTokenHandler.Instance);
			AddTokenHandler("func", GoFunctionTokenHandler.Instance);
			AddTokenHandler("if", GoIfTokenHandler.Instance);
			AddTokenHandler("else", GoIfTokenHandler.Instance);
			AddTokenHandler("for", GoForTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("break", BreakTokenHandler.Instance);
			AddTokenHandler("continue", ContinueTokenHandler.Instance);

			// 关键字
			AddTokenHandler("nil", NullTokenHandler.Instance);
			AddTokenHandler("true", BoolTokenHandler.Instance);
			AddTokenHandler("false", BoolTokenHandler.Instance);

			// 操作符处理
			AddTokenHandler("&&", LazyTokenHandler.Instance);
			AddTokenHandler("||", LazyTokenHandler.Instance);

			// 索引和切片
			AddTokenHandler("[", new BracketTokenHandler(typeof(List<object>)));
		}

		public override ITokenStream GetTokenStream(CharReader charReader)
		{
			return new GoTokenStream(charReader);
		}

		public override ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return GoSyntaxAnalyzer.Instance;
		}

		public override int? GetOperatorPriority(string op)
		{
			switch (op)
			{
				case "&^":
					return DefaultSyntaxAnalyzer.OperatorPriorities["&"];
				default:
					break;
			}
			return base.GetOperatorPriority(op);
		}

		public override bool IsTrue(object obj)
		{
			if (obj == null) return false;
			if (obj is bool b) return b;
			if (obj is string s) return !string.IsNullOrEmpty(s);
			if (obj is long l) return l != 0L;
			if (obj is ulong ul) return ul != 0L;
			if (obj is int i) return i != 0;
			if (obj is uint ui) return ui != 0;
			if (obj is float f) return f != 0F;
			if (obj is double d) return d != 0D;
			if (obj is decimal dec) return dec != 0M;
			if (obj is byte b2) return b2 != 0;
			if (obj is short s2) return s2 != 0;
			if (obj is ushort us) return us != 0;
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

		//public static ITreeNode BuildBlock(int parentColumn, DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, HashSet<string> endTokens = null)
		//{
		//	var token = tokenReader.Read();
		//	if (!token.HasValue) return null;
		//	if (token.Value.Column <= parentColumn)
		//	{
		//		tokenReader.Push(token.Value);
		//		return null;
		//	}

		//	var builder = ignore ? null : new TreeBuilder();
		//	int column = token.Value.Column;
		//	while (token.HasValue && token.Value.Column == column)
		//	{
		//		tokenReader.Push(token.Value);
		//		var statement = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens: endTokens);
		//		if (!ignore)
		//		{
		//			builder.Add(buildContext, scriptContext, options, control, statement);
		//		}
		//		token = tokenReader.Read();
		//	}
		//	if (token.HasValue)
		//	{
		//		tokenReader.Push(token.Value);
		//	}

		//	return builder;
		//}
	}
}
