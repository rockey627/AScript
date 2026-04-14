using AScript.Operators;
using System;

namespace AScript.Test.MSTests.Python
{
	public class PythonLang : ScriptLang
	{
		public static readonly PythonLang Instance = new PythonLang();

		public static readonly HashSet<string> EndTokens = new HashSet<string>() { ":", "\n" };

		public PythonLang()
		{
			this.Compatible = false;

			AddFunc("=", AssignOperator.Instance);
			AddFunc("+", PlusOperator.Instance);
			AddFunc("-", SubtractOperator.Instance);
			AddFunc("*", MultiplyOperator.Instance);
			AddFunc("/", DivideOperator.Instance);
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc("==", EqualOperator.Instance);
			AddFunc("!=", NotEqualOperator.Instance);
			AddFunc("and", AndAlsoOperator.Instance);
			AddFunc("or", OrElseOperator.Instance);

			AddTokenHandler(NewLineHandler.Instance);

			AddTokenHandler("#", AScript.TokenHandlers.AnnotationLineTokenHandler.Instance);
			AddTokenHandler("and", AndTokenHandler.Instance);
			AddTokenHandler("or", OrTokenHandler.Instance);
			AddTokenHandler("if", IfTokenHandler.Instance);
		}
	}
}
