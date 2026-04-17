using AScript.Lang.Sql.TokenHandlers;
using AScript.Operators;
using AScript.TokenHandlers;
using System;

namespace AScript.Lang.Sql
{
	/// <summary>
	/// sql脚本语言
	/// </summary>
	public class SqlLang : ScriptLang
	{
		public static readonly SqlLang Instance = new SqlLang();

		public SqlLang() : base(ignoreCase : true)
		{
			AddFunc(".", DotOperator.Instance);
			AddFunc("!", BoolNotOperator.Instance);
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc("=", EqualOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc("!=", NotEqualOperator.Instance);
			AddFunc("and", AndAlsoOperator.Instance);
			AddFunc("or", OrElseOperator.Instance);

			AddTokenHandler("and", AndAlsoTokenHandler.Instance);
			AddTokenHandler("or", OrElseTokenHandler.Instance);
			AddTokenHandler("=", EqualTokenHandler.Instance);
			AddTokenHandler("like", LikeTokenHandler.Instance);
			AddTokenHandler("from", FromTokenHandler.Instance);
		}
	}
}
