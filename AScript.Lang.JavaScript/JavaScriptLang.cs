using AScript.Functions;
using AScript.Lang.JavaScript.TokenHandlers;
using AScript.Operators;
using AScript.TokenHandlers;
using System;

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
			AddType("Array", typeof(Array));

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
			AddFunc("?.", new DotOperator(true));
			AddFunc("[]", IndexOperator.Instance);
			AddFunc("[:]", IndexStartEndOperator.Instance);

			// 内置eval函数
			AddFunc("eval", EvalFunction.Instance);

			AddTokenHandler("??", LazyTokenHandler.Instance);
			AddTokenHandler("?=", LazyTokenHandler.Instance);
			AddTokenHandler("?", QuestionIIFTokenHandler.Instance);
			AddTokenHandler("[", BracketTokenHandler.Instance);
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
			AddTokenHandler("function", JavaScriptFunctionTokenHandler.Instance);
		}

		public override bool IsDynamic()
		{
			return true;
		}
	}
}
