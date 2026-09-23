using AScript.Functions;
using AScript.Lang.Go.Nodes;
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
			AddType<object>("any");

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
			AddFunc(typeof(Extensions.GoConvertExtensions));

			AddFunc("make", Functions.GoMakeFunction.Instance);

			AddFunc("import", InstallModuleFunction.Instance);

			AddModule("strconv", new SingleTypeScriptModule<Extensions.strconv>(2));
			AddModule("fmt", new SingleTypeScriptModule<Extensions.fmt>(2));
			AddModule("sync", new SingleTypeScriptModule<Extensions.sync>(2));
			AddModule("time", new Extensions.TimeScriptModule());

			// Token处理器 - 核心语句
			AddTokenHandler("var", GoVarTokenHandler.Instance);
			AddTokenHandler("const", new GoVarTokenHandler(Modifiers.CONST));
			AddTokenHandler("func", GoFuncTokenHandler.Instance);
			AddTokenHandler("if", GoIfTokenHandler.Instance);
			AddTokenHandler("for", GoForTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("break", BreakTokenHandler.Instance);
			AddTokenHandler("continue", ContinueTokenHandler.Instance);
			AddTokenHandler("switch", new CaseWhenTokenHandler("case", true));
			AddTokenHandler("go", GoGoTokenHandler.Instance);
			AddTokenHandler("import", GoImportTokenHandler.Instance);
			AddTokenHandler("defer", GoDeferTokenHandler.Instance);

			// 常量
			AddTokenHandler("nil", NullTokenHandler.Instance);
			AddTokenHandler("true", BoolTokenHandler.Instance);
			AddTokenHandler("false", BoolTokenHandler.Instance);

			// 操作符处理
			AddTokenHandler("&&", LazyTokenHandler.Instance);
			AddTokenHandler("||", LazyTokenHandler.Instance);
			AddTokenHandler("...", IgnoreTokenHandler.Instance);

			// 索引和切片
			AddTokenHandler("[", GoBracketTokenHandler.Instance);
			// 集合
			AddTokenHandler("map", GoMapTokenHandler.Instance);
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
		public static List<GoDefineVarNode> ParseDefineVars(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, bool ignore)
		{
			var defines = ignore ? null : new List<GoDefineVarNode>();
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
							if (defineVar.GoType == null)
							{
								defineVar.GoType = type;
							}
							//if (defineVar.SystemType == null)
							//{
							//	defineVar.Type = type.Name;
							//	defineVar.SystemType = type.RealType;
							//}
						}
					}
				}

				defines?.Add(new GoDefineVarNode(varName, type));
				//defines?.Add(PoolManage.CreateDefineVarNode(varName, type?.Name, type?.RealType));

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
				//var typeName = nextToken.Value.Value;
				//var runtimeType = scriptContext.EvalType(typeName);
				//if (runtimeType != null)
				//{
				//	type = new GoRuntimeType(typeName, runtimeType);
				//	return true;
				//}
				if (TryParseWordType(analyzer, buildContext, scriptContext, options, tokenReader, nextToken.Value, out type))
				{
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
				//var typeName = token.Value.Value;
				//var type = scriptContext.EvalType(typeName);
				//if (type == null) throw new Exceptions.ScriptAnalyzingException($"unknown type '{typeName}' at ({token.Value.Line},{token.Value.Column})");
				//return new GoRuntimeType(typeName, type);
				return ParseWordType(analyzer, buildContext, scriptContext, options, tokenReader, token.Value);
			}

			throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column}), expect type");
		}

		public static bool TryParseWordType(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, Token word, out GoType type)
		{
			var typeName = word.Value;
			var nextToken = tokenReader.Read();
			if (nextToken.HasValue)
			{
				if (nextToken.Value.IsSymbol("."))
				{
					var token2 = tokenReader.Read();
					if (token2.HasValue)
					{
						if (token2.Value.Type == ETokenType.Word)
						{
							// sync.Mutex
							var instance = scriptContext.EvalVar(word.Value);
							if (instance is TypeWrapper wrapper)
							{
								var runtimeType2 = wrapper.Type.GetNestedType(token2.Value.Value);
								if (runtimeType2 != null)
								{
									typeName = $"{word.Value}.{token2.Value.Value}";
									type = new GoRuntimeType(typeName, runtimeType2);
									return true;
								}
							}
						}
						tokenReader.Push(token2.Value);
					}
				}
				tokenReader.Push(nextToken.Value);
			}
			var runtimeType = scriptContext.EvalType(typeName);
			if (runtimeType != null)
			{
				type = new GoRuntimeType(typeName, runtimeType);
				return true;
			}
			type = null;
			return false;
		}

		public static GoType ParseWordType(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, Token word)
		{
			if (!TryParseWordType(analyzer, buildContext, scriptContext, options, tokenReader, word, out var type))
			{
				throw new Exceptions.ScriptAnalyzingException($"unknown type '{word.Value}' at ({word.Line},{word.Column})");
			}
			return type;
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
		public static GoMapType ParseMapType(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, bool ignore)
		{
			analyzer.ValidateNextToken(tokenReader, "[");
			var keyTypeToken = analyzer.ValidateNextToken(tokenReader, ETokenType.Word);
			var keyType = scriptContext.EvalType(keyTypeToken.Value.Value);
			if (keyType == null) throw new Exceptions.ScriptAnalyzingException($"unknown type '{keyTypeToken.Value.Value}' at ({keyTypeToken.Value.Line},{keyTypeToken.Value.Column})");
			analyzer.ValidateNextToken(tokenReader, "]");
			var valueType = ParseType(analyzer, buildContext, scriptContext, options, tokenReader, ignore);
			return new GoMapType(keyTypeToken.Value.Value, keyType, valueType);
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

		public static ITreeNode ParseValue(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore, GoType valueType)
		{
			if (valueType is GoArrayType goArrayType)
			{
				// 数组
				return ParseArrayValue(analyzer, buildContext, scriptContext, options, tokenReader, control, ignore, goArrayType);
			}
			else if (valueType is GoMapType goMapType)
			{
				return ParseMapValue(analyzer, buildContext, scriptContext, options, tokenReader, control, ignore, goMapType);
			}
			else
			{
				var v1 = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
				if (v1 == null)
				{
					return null;
				}
				var token = analyzer.ValidateNextToken(tokenReader);
				if (token.Value.IsSymbol(":"))
				{
					var v2 = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
					if (ignore) return null;
					// [v1] = v2
					var op = PoolManage.CreateOperatorNode("[]", 2, 0);
					op.Left = v1;
					op.Right = v2;
					return op;
				}
				else
				{
					tokenReader.Push(token.Value);
					return v1;
				}
			}
		}

		public static ITreeNode ParseArrayValue(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore, GoArrayType goArrayType)
		{
			var token = tokenReader.Read();
			if (token.HasValue)
			{
				if (token.Value.IsSymbol("{"))
				{
					List<ITreeNode> items = null;
					token = analyzer.ValidateNextToken(tokenReader);
					if (token.Value.IsSymbol("}")) { }
					else
					{
						items = ignore ? null : new List<ITreeNode>();
						// { 5, 6, 7, 5: 10 }
						tokenReader.Push(token.Value);
						while (true)
						{
							var item = ParseValue(analyzer, buildContext, scriptContext, options, tokenReader, control, ignore, goArrayType.ItemType);
							if (item != null) items?.Add(item);
							token = analyzer.ValidateNextToken(tokenReader);
							if (token.Value.IsSymbol(",")) continue;
							if (token.Value.IsSymbol("}")) break;
							throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
						}
					}
					if (ignore) return null;
					return new CollectionNode { CollectionType = goArrayType.RealType, ElementType = goArrayType.ItemType.RealType, Items = items, Length = goArrayType.Length };
				}
				tokenReader.Push(token.Value);
			}
			return ignore ? null : PoolManage.CreateObjectNode(goArrayType.RealType);
		}

		public static ITreeNode ParseMapValue(DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore, GoMapType goMapType)
		{
			var token = tokenReader.Read();
			if (token.HasValue)
			{
				if (token.Value.IsSymbol("{"))
				{
					List<ITreeNode> items = null;
					token = analyzer.ValidateNextToken(tokenReader);
					if (token.Value.IsSymbol("}")) { }
					else
					{
						items = ignore ? null : new List<ITreeNode>();
						tokenReader.Push(token.Value);
						while (true)
						{
							var key = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
							if (key == null)
							{
								analyzer.ValidateNextToken(tokenReader, "}");
								break;
							}
							analyzer.ValidateNextToken(tokenReader, ":");
							var value = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
							if (items != null)
							{
								// [key] = value
								var op = PoolManage.CreateOperatorNode("[]", 2, 0);
								op.Left = key;
								op.Right = value;
								items.Add(op);
							}
							token = analyzer.ValidateNextToken(tokenReader);
							if (token.Value.IsSymbol(",")) continue;
							if (token.Value.IsSymbol("}")) break;
							throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
						}
					}
					if (ignore) return null;
					return new NewNode
					{
						SystemType = goMapType.RealType,
						InitProperties = items
					};
				}
				tokenReader.Push(token.Value);
			}
			return ignore ? null : PoolManage.CreateObjectNode(goMapType.RealType);
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
