using AScript.Functions;
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

		protected SqlLang() : base(ignoreCase: true)
		{
			AddType("tinyint", typeof(byte));
			AddType("smallint", typeof(short));
			AddType("int", typeof(int));
			AddType("bigint", typeof(long));
			AddType("decimal", typeof(decimal));
			AddType("float", typeof(float));
			AddType("real", typeof(double));
			AddType("double", typeof(double));
			AddType("bit", typeof(bool));
			AddType("char", typeof(string));
			AddType("nchar", typeof(string));
			AddType("varchar", typeof(string));
			AddType("nvarchar", typeof(string));
			AddType("text", typeof(string));
			AddType("datetime", typeof(DateTime));
			AddType("datetime2", typeof(DateTime));

			AddFunc("+", PlusOperator.Instance);
			AddFunc("-", SubtractOperator.Instance);
			AddFunc("*", MultiplyOperator.Instance);
			//AddFunc("**", PowerOperator.Instance);
			AddFunc("/", DivideOperator.Instance);
			AddFunc("%", ModuloOperator.Instance);
			AddFunc(".", DotOperator.Instance);
			AddFunc("?.", new DotOperator(true));
			AddFunc("!", BoolNotOperator.Instance);
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc("=", AssignOperator.Instance);
			AddFunc("==", EqualOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc("!=", NotEqualOperator.Instance);
			AddFunc("and", AndAlsoOperator.Instance);
			AddFunc("or", OrElseOperator.Instance);
			AddFunc("in", new ContainsFunction(reverse: true));

			// MySql获取当前时间
			AddFunc("now", NowFunction.Instance);
			// SqlServer获取当前时间
			AddFunc("getdate", NowFunction.Instance);

			// IEnumerable<T>扩展方法
			AddFunc(typeof(System.Linq.Enumerable));
			// IQueryable<T>扩展方法
			AddFunc(typeof(System.Linq.Queryable), method => !method.IsGenericMethod && method.Name == "AsQueryable" ? null : method.Name);
			// CONCAT函数
			AddFunc(typeof(string), method =>
			{
				if (method.Name == "Concat") return method.Name;
				return null;
			});

			AddTokenHandler("return", ReturnTokenHandler.Instance);
			AddTokenHandler("=", new OperatorTokenHandler("==", "=="));
			AddTokenHandler("<>", new OperatorTokenHandler("!=", "!="));
			AddTokenHandler("and", AndAlsoTokenHandler.Instance);
			AddTokenHandler("or", OrElseTokenHandler.Instance);
			AddTokenHandler("null", NullTokenHandler.Instance);
			AddTokenHandler("not", new OperatorTokenHandler("!", "!") { DataCount = 1, Prefix = true });
			AddTokenHandler("is", SqlIsNullTokenHandler.Instance);
			AddTokenHandler("in", SqlInTokenHandler.Instance);
			AddTokenHandler("like", SqlLikeTokenHandler.Instance);
			AddTokenHandler("set", SqlSetTokenHandler.Instance);
			AddTokenHandler("select", SqlSelectTokenHandler.Instance);
			AddTokenHandler("from", SqlFromTokenHandler.Instance);
			AddTokenHandler("insert", SqlInsertTokenHandler.Instance);
			AddTokenHandler("update", SqlUpdateTokenHandler.Instance);
			AddTokenHandler("delete", SqlDeleteTokenHandler.Instance);
			AddTokenHandler("create", SqlCreateTokenHandler.Instance);
			AddTokenHandler("call", SqlCallTokenHandler.Instance);
			AddTokenHandler("exec", SqlExecTokenHandler.Instance);
			AddTokenHandler("execute", SqlExecTokenHandler.Instance);
			AddTokenHandler("declare", SqlDeclareTokenHandler.Instance);
		}
	}
}
