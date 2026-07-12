using AScript.Functions;
using AScript.Lang.JavaScript.Extensions;
using AScript.Lang.JavaScript.TokenHandlers;
using AScript.Operators;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
			AddType("String", typeof(string));
			AddType("Array", typeof(List<object>));
			AddType("Set", typeof(HashSet<object>));
			AddType("Map", typeof(Dictionary<object, object>));
			AddType("Date", typeof(DateTime));
			AddType("Math", typeof(Math));
			AddType("Promise", typeof(Task<object>));

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

			AddFunc("await", new AwaitFunction());
			AddFunc("eval", EvalFunction.Instance);
			AddFunc("concat", ConcatFunction.Instance);
			AddFunc("includes", new ContainsFunction());
			AddFunc("get_length", new LengthFunction(typeof(long)));
			AddFunc("slice", IndexStartEndOperator.Instance);
			AddFunc("require", InstallModuleFunction.Instance);

			AddFunc<long, Task>("delay", ms => Task.Delay((int)ms));
			AddAction<long>("sleep", ms => Thread.Sleep((int)ms));

			AddFunc(typeof(CommonFunctions));

			AddFunc(typeof(JavaScriptDateExtensions));
			AddFunc(typeof(JavaScriptMathExtensions));
			AddFunc(typeof(JavaScriptStringExtensions));
			AddFunc(typeof(JavaScriptArrayExtensions));
			AddFunc(typeof(JavaScriptSetExtensions));
			AddFunc(typeof(JavaScriptMapExtensions));
			AddFunc(typeof(JavaScriptPromiseExtensions));

			InstallModule(new JavaScriptConsoleModule());
			InstallModule(new JavaScriptJsonModule());

			AddTokenHandler("var", VarTokenHandler.Instance);
			AddTokenHandler("let", VarTokenHandler.Instance);
			AddTokenHandler("const", VarTokenHandler.Instance);
			AddTokenHandler("??", LazyTokenHandler.Instance);
			AddTokenHandler("?=", LazyTokenHandler.Instance);
			AddTokenHandler("?", QuestionIIFTokenHandler.Instance);
			AddTokenHandler("[", new BracketTokenHandler(typeof(List<object>)));
			AddTokenHandler("null", NullTokenHandler.Instance);
			AddTokenHandler("true", BoolTokenHandler.Instance);
			AddTokenHandler("false", BoolTokenHandler.Instance);
			AddTokenHandler("await", AwaitTokenHandler.Instance);
			AddTokenHandler("throw", ThrowTokenHandler.Instance);
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
			AddTokenHandler("undefined", JavaScriptUndefinedTokenHandler.Instance);
		}

		public override bool IsDynamic()
		{
			return true;
		}

		public override bool? IsObjectMemberEnabled(Type objType)
		{
			var enabled = base.IsObjectMemberEnabled(objType);
			return enabled ?? false;
		}

		public override ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return JavaScriptSyntaxAnalyzer.Instance;
		}

		private static class CommonFunctions
		{
			public static CancellationTokenSource setTimeout(Delegate del, long ms, params object[] args)
			{
				var source = new CancellationTokenSource();
				var token = source.Token;
				Task.Delay((int)ms, token).ContinueWith(t => del.DynamicInvoke(args), token);
				return source;
			}

			public static void clearTimeout(CancellationTokenSource source)
			{
				source.Cancel();
			}

			public static Timer setInterval(Delegate del, long ms, params object[] args)
			{
				var timer = new Timer(new TimerCallback(obj =>
				{
					del.DynamicInvoke(args);
				}), null, 0, ms);
				return timer;
			}

			public static void clearInterval(Timer timer)
			{
				try { timer.Change(0, 0); } catch { }
				try { timer.Dispose(); } catch { }
			}
		}
	}
}
