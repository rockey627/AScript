using AScript.Operators;
using AScript.TokenHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AScript.Lang.CSharp
{
    public class CSharpLang : ScriptLang
	{
		public static readonly CSharpLang Instance = new CSharpLang();

		protected CSharpLang()
		{
			// 类型
			AddType("void", typeof(void));    // 用于定义无返回值的函数
			AddType("var", typeof(object));     // 用于定义变量，类型取决于变量的值类型
			AddType("object", typeof(object));
			AddType("char", typeof(char));
			AddType("byte", typeof(byte));
			AddType("short", typeof(short));
			AddType("int", typeof(int));
			AddType("uint", typeof(uint));
			AddType("long", typeof(long));
			AddType("ulong", typeof(ulong));
			AddType("float", typeof(float));
			AddType("double", typeof(double));
			AddType("decimal", typeof(decimal));
			AddType("bool", typeof(bool));
			AddType("string", typeof(string));
			AddType("IList<>", typeof(IList<>));
			AddType("List<>", typeof(List<>));
			AddType("HashSet<>", typeof(HashSet<>));
			AddType("IDictionary<,>", typeof(IDictionary<,>));
			AddType("Dictionary<,>", typeof(Dictionary<,>));
			AddType(typeof(DateTime));
			AddType(typeof(TimeSpan));
			AddType(typeof(IList));
			AddType(typeof(Array));
			AddType(typeof(IDictionary));
			AddType(typeof(IEnumerable));
			AddType(typeof(Type));
			AddType(typeof(Assembly));
			AddType(typeof(MethodInfo));
			AddType(typeof(PropertyInfo));
			AddType(typeof(FieldInfo));
			AddType(typeof(Version));
			AddType(typeof(Delegate));
			AddType(typeof(Console));
			AddType(typeof(Math));
			AddType(typeof(File));
			AddType(typeof(Directory));
			AddType(typeof(FileInfo));
			AddType(typeof(DirectoryInfo));
			AddType(typeof(Path));
			AddType(typeof(Convert));
			AddType(typeof(Guid));

			AddFunc("=", AssignOperator.Instance);
			AddFunc("+=", PlusAssignOperator.Instance);
			AddFunc("-=", SubtractAssignOperator.Instance);
			AddFunc("*=", MultiplyAssignOperator.Instance);
			AddFunc("**=", PowerAssignOperator.Instance);
			AddFunc("/=", DivideAssignOperator.Instance);
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
			AddFunc("/", DivideOperator.Instance);
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
			AddFunc("[]", IndexOperator.Instance);
			AddFunc("[:]", IndexStartEndOperator.Instance);

			// 内置Convert方法，调用示例：'12'.ToInt32()
			AddFunc(typeof(Convert));

			AddFunc("Invoke", CustomFunctionEvaluator.Instance);

			// 内置eval函数
			AddFunc<ScriptContext, string, object>("eval", Eval);
			AddFunc<ScriptContext, string, int, object>("eval", Eval);
			AddFunc<ScriptContext, string, int, string, object>("eval", Eval);
			AddFunc<ScriptContext, string, int, string, string, object>("eval", Eval);

			AddTokenHandler("??", LazyTokenHandler.Instance);
			AddTokenHandler("?=", LazyTokenHandler.Instance);
			AddTokenHandler("?", QuestionIIFTokenHandler.Instance);
			AddTokenHandler("[", IndexTokenHandler.Instance);
			AddTokenHandler("null", NullTokenHandler.Instance);
			AddTokenHandler("new", NewTokenHandler.Instance);
			AddTokenHandler("if", IfTokenHandler.Instance);
			AddTokenHandler("else", IfTokenHandler.Instance);
			AddTokenHandler("for", ForTokenHandler.Instance);
			AddTokenHandler("while", WhileTokenHandler.Instance);
			AddTokenHandler("foreach", ForeachTokenHandler.Instance);
			AddTokenHandler("true", BoolTokenHandler.Instance);
			AddTokenHandler("false", BoolTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("break", BreakTokenHandler.Instance);
			AddTokenHandler("continue", ContinueTokenHandler.Instance);
			AddTokenHandler("$", StringInterpolationTokenHandler.Instance);
		}

		private static object Eval(ScriptContext context, string expression)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(context, expression);
		}

		private static object Eval(ScriptContext context, string expression, int cacheTime)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(context, expression, cacheTime);
		}

		private static object Eval(ScriptContext context, string expression, int cacheTime, string cacheKey)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(context, expression, cacheTime, cacheKey);
		}

		private static object Eval(ScriptContext context, string expression, int cacheTime, string cacheKey, string cacheVersion)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(context, expression, cacheTime, cacheKey, cacheVersion);
		}
	}
}
