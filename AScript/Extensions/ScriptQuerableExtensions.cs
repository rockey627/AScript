using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Extensions
{
	public static class ScriptQuerableExtensions
	{
		private static readonly BuildOptions _options = new BuildOptions(Script.DefaultOptions)
		{
			CompileMode = ECompileMode.All,
			RewriteFunctions = false,
			RewriteVariables = false,
			Standalone = true
		};

		public static IQueryable<T> WhereScript<T>(this IQueryable<T> query, Expression<Func<T, string>> predicate)
		{
			return WhereScript(query, predicate, ScriptContext.Root);
		}

		public static IQueryable<T> WhereScript<T>(this IQueryable<T> query, Expression<Func<T, string>> predicate, ScriptContext context)
		{
			string code = GetString(predicate);
			string argName = predicate.Parameters[0].Name;
			var lambda = Script.Lambda<T, bool>(context, _options, code, argName);
			return query.Where(lambda);
		}

		public static IQueryable<TResult> SelectScript<T, TResult>(this IQueryable<T> query, Expression<Func<T, string>> selector)
		{
			return SelectScript<T, TResult>(query, selector, ScriptContext.Root);
		}

		public static IQueryable<TResult> SelectScript<T, TResult>(this IQueryable<T> query, Expression<Func<T, string>> selector, ScriptContext context)
		{
			string code = GetString(selector);
			string argName = selector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TResult>(context, _options, code, argName);
			return query.Select(lambda);
		}

		public static IOrderedQueryable<T> OrderByScript<T, TKey>(this IQueryable<T> query, Expression<Func<T, string>> keySelector)
		{
			return OrderByScript<T, TKey>(query, keySelector, ScriptContext.Root);
		}

		public static IOrderedQueryable<T> OrderByScript<T, TKey>(this IQueryable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.OrderBy(lambda);
		}

		public static IOrderedQueryable<T> OrderByDescendingScript<T, TKey>(this IQueryable<T> query, Expression<Func<T, string>> keySelector)
		{
			return OrderByDescendingScript<T, TKey>(query, keySelector, ScriptContext.Root);
		}

		public static IOrderedQueryable<T> OrderByDescendingScript<T, TKey>(this IQueryable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.OrderByDescending(lambda);
		}

		public static IOrderedQueryable<T> ThenByScript<T, TKey>(this IOrderedQueryable<T> query, Expression<Func<T, string>> keySelector)
		{
			return ThenByScript<T, TKey>(query, keySelector, ScriptContext.Root);
		}

		public static IOrderedQueryable<T> ThenByScript<T, TKey>(this IOrderedQueryable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.ThenBy(lambda);
		}

		public static IOrderedQueryable<T> ThenByDescendingScript<T, TKey>(this IOrderedQueryable<T> query, Expression<Func<T, string>> keySelector)
		{
			return ThenByDescendingScript<T, TKey>(query, keySelector, ScriptContext.Root);
		}

		public static IOrderedQueryable<T> ThenByDescendingScript<T, TKey>(this IOrderedQueryable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.ThenByDescending(lambda);
		}

		private static string GetString<T>(Expression<Func<T, string>> lambda)
		{
			if (TryGetString(lambda.Body, out string code))
			{
				return code;
			}
			return lambda.Compile()(default);
		}

		private static bool TryGetString(Expression expression, out string s)
		{
			if (expression is ConstantExpression constant)
			{
				s = constant.Value?.ToString();
				return true;
			}
			s = null;
			return false;
		}
	}
}
