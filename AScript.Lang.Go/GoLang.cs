using AScript.Functions;
using AScript.Lang.Go.Operators;
using AScript.Lang.Go.TokenHandlers;
using AScript.Lang.Go.Types;
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
			//AddType<char>("rune", false);
			AddType<object>("any", false);

			// 集合类型
			//AddType<Dictionary<object, object>>("map");
			//AddType<List<object>>("slice");
			//AddType<List<object>>("array");
			//AddType<object[]>("[...]");

			// 赋值运算符
			AddFunc("=", AssignOperator.Instance);
			AddFunc(":=", AssignOperator.Instance);
			AddFunc("+=", PlusAssignOperator.Instance);
			AddFunc("-=", SubtractAssignOperator.Instance);
			AddFunc("*=", MultiplyAssignOperator.Instance);
			AddFunc("/=", DivideAssignOperator.Instance);
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

			AddFunc("++", IncrementAssignOperator.Instance);
			AddFunc("--", DecrementAssignOperator.Instance);

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
			AddFunc("[:]", IndexStartEndOperator.Instance);

			// 内置函数
			AddFunc(typeof(Extensions.GoCommonExtensions));

			// Token处理器 - 核心语句
			AddTokenHandler("var", GoVarTokenHandler.Instance);
			AddTokenHandler("const", new GoVarTokenHandler(Modifiers.CONST));
			AddTokenHandler("func", GoFuncTokenHandler.Instance);
			AddTokenHandler("if", IfTokenHandler.Instance);
			AddTokenHandler("for", GoForTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("break", BreakTokenHandler.Instance);
			AddTokenHandler("continue", ContinueTokenHandler.Instance);
			AddTokenHandler("switch", new CaseWhenTokenHandler("case", true));

			// 关键字
			AddTokenHandler("nil", NullTokenHandler.Instance);
			AddTokenHandler("true", BoolTokenHandler.Instance);
			AddTokenHandler("false", BoolTokenHandler.Instance);

			// 操作符处理
			AddTokenHandler("&&", LazyTokenHandler.Instance);
			AddTokenHandler("||", LazyTokenHandler.Instance);
			AddTokenHandler("...", IgnoreTokenHandler.Instance);

			// 索引和切片
			AddTokenHandler("[", new GoBracketTokenHandler());
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
				case ":=":
					return DefaultSyntaxAnalyzer.OperatorPriorities["="];
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

		/// <summary>
		/// <para>解析变量定义列表</para>
		/// <para>a</para>
		/// <para>a int</para>
		/// <para>a, b</para>
		/// <para>a, b int</para>
		/// <para>a int, b string</para>
		/// </summary>
		/// <returns></returns>
		public static List<DefineVarNode> ParseDefineVars(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, bool ignore)
		{
			var defines = ignore ? null : new List<DefineVarNode>();
			Token? nextToken;
			while (true)
			{
				var nameToken = analyzer.ValidateNextToken(tokenReader, ETokenType.Word);
				string varName = nameToken.Value.Value;

				if (TryParseType(analyzer, buildContext, scriptContext, options, tokenReader, ignore, out var type))
				{
					if (defines != null)
					{
						for (int i = 0; i < defines.Count; i++)
						{
							var defineVar = defines[i];
							if (defineVar.SystemType == null)
							{
								defineVar.Type = type.Name;
								defineVar.SystemType = type.RealType;
							}
						}
					}
				}

				defines?.Add(PoolManage.CreateDefineVarNode(varName, type?.Name, type?.RealType));

				nextToken = tokenReader.Read();
				if (!nextToken.HasValue) break;
				if (nextToken.Value.IsSymbol(",")) continue;
				break;
			}
			if (nextToken.HasValue)
			{
				tokenReader.Push(nextToken.Value);
			}
			return defines;
		}

		public static bool TryParseType(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, bool ignore, out GoType type)
		{
			var nextToken = tokenReader.Read();

			if (!nextToken.HasValue)
			{
				type = null;
				return false;
			}

			if (nextToken.Value.IsSymbol("func"))
			{
				type = ParseFuncType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
				return true;
			}

			if (nextToken.Value.IsSymbol("map"))
			{
				type = ParseMapType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
				return true;
			}

			if (nextToken.Value.IsSymbol("["))
			{
				// 数组、切片类型
				type = ParseArrayType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
				return true;
			}

			if (nextToken.Value.Type == ETokenType.Word)
			{
				var typeName = nextToken.Value.Value;
				var runtimeType = scriptContext.EvalType(typeName);
				if (runtimeType != null)
				{
					type = new GoRuntimeType(typeName, runtimeType);
					return true;
				}
			}

			tokenReader.Push(nextToken.Value);

			type = null;
			return false;
		}

		public static GoType ParseType(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, bool ignore)
		{
			var token = analyzer.ValidateNextToken(tokenReader);

			if (token.Value.IsSymbol("func"))
			{
				// 函数类型
				return ParseFuncType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
			}

			if (token.Value.IsSymbol("map"))
			{
				return ParseMapType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
			}

			if (token.Value.IsSymbol("["))
			{
				// 数组、切片类型
				return ParseArrayType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
			}

			if (token.Value.Type == ETokenType.Word)
			{
				var typeName = token.Value.Value;
				var type = scriptContext.EvalType(typeName);
				if (type == null) throw new Exceptions.ScriptAnalyzingException($"unknown type '{typeName}' at ({token.Value.Line},{token.Value.Column})");
				return new GoRuntimeType(typeName, type);
			}

			throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column}), expect type");
		}

		/// <summary>
		/// map[keyType]valueType
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="tokenReader"></param>
		/// <param name="ignore"></param>
		/// <returns></returns>
		public static GoType ParseMapType(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, bool ignore)
		{
			analyzer.ValidateNextToken(tokenReader, "[");
			var keyTypeToken = analyzer.ValidateNextToken(tokenReader, ETokenType.Word);
			return null;
		}

		/// <summary>
		/// func(argType1, argType2) returnType
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="tokenReader"></param>
		/// <param name="ignore"></param>
		/// <returns></returns>
		/// <exception cref="Exceptions.ScriptAnalyzingException"></exception>
		public static GoType ParseFuncType(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, bool ignore)
		{
			analyzer.ValidateNextToken(tokenReader, "(");
			var token = analyzer.ValidateNextToken(tokenReader);
			var funcArgTypes = new List<Type>();
			var funcArgNames = new List<string>();
			if (!token.Value.IsSymbol(")"))
			{
				tokenReader.Push(token.Value);
				while (true)
				{
					var argType = ParseType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
					funcArgTypes.Add(argType.RealType);
					funcArgNames.Add(argType.Name);
					token = analyzer.ValidateNextToken(tokenReader);
					if (token.Value.IsSymbol(",")) continue;
					if (token.Value.IsSymbol(")")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' near func at ({token.Value.Line},{token.Value.Column}), expect type");
				}
			}
			// 返回类型
			TryParseType(analyzer, buildContext, scriptContext, options, tokenReader, ignore, out var funcReturnType);
			var delegateType = ScriptUtils.GetDelegateType(funcArgTypes, funcReturnType?.RealType ?? typeof(void));
			return new GoRuntimeType($"func({string.Join(",", funcArgNames)}){funcReturnType?.Name}", delegateType);
		}

		public static GoArrayType ParseArrayType(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, bool ignore)
		{
			var token = analyzer.ValidateNextToken(tokenReader);
			bool isArray;
			ITreeNode count;
			if (token.Value.IsSymbol("]"))
			{
				// []
				isArray = false;
				count = null;
			}
			else if (token.Value.IsSymbol("..."))
			{
				// [...]
				isArray = true;
				count = null;
				analyzer.ValidateNextToken(tokenReader, "]");
			}
			else
			{
				tokenReader.Push(token.Value);
				isArray = true;
				count = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, null, ignore);
				analyzer.ValidateNextToken(tokenReader, "]");
			}
			// 元素类型
			var itemType = ParseType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
			return new GoArrayType(isArray, itemType, count, null);
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
