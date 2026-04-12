using AScript.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.Sql
{
	public class SqlLang : ScriptLang
	{
		public static readonly SqlLang Instance = new SqlLang();

		public SqlLang()
		{
			this.Compatible = false;

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

			AddTokenHandler("and", AndTokenHandler.Instance);
			AddTokenHandler("AND", AndTokenHandler.Instance);
			AddTokenHandler("or", OrTokenHandler.Instance);
			AddTokenHandler("OR", OrTokenHandler.Instance);
			AddTokenHandler("=", EqualTokenHandler.Instance);
			AddTokenHandler("like", LikeTokenHandler.Instance);
			AddTokenHandler("LIKE", LikeTokenHandler.Instance);
		}
	}
}
