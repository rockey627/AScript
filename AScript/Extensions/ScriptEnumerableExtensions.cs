using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Extensions
{
	public static class ScriptEnumerableExtensions
	{
		private static readonly BuildOptions _options = new BuildOptions(Script.DefaultOptions)
		{
			CompileMode = ECompileMode.All,
			RewriteFunctions = false,
			RewriteVariables = false,
			Standalone = true
		};

		public static IEnumerable<T> WhereScript<T>(this IEnumerable<T> query, Expression<Func<T, string>> predicate)
		{
			return WhereScript(query, predicate, ScriptContext.Root);
		}

		public static IEnumerable<T> WhereScript<T>(this IEnumerable<T> query, Expression<Func<T, string>> predicate, ScriptContext context)
		{
			string code = GetString(predicate);
			string argName = predicate.Parameters[0].Name;
			var lambda = Script.Lambda<T, bool>(context, _options, code, argName);
			return query.Where(lambda.Compile());
		}

		public static IEnumerable<TResult> SelectScript<T, TResult>(this IEnumerable<T> query, Expression<Func<T, string>> selector)
		{
			return SelectScript<T, TResult>(query, selector, ScriptContext.Root);
		}

		public static IEnumerable<TResult> SelectScript<T, TResult>(this IEnumerable<T> query, Expression<Func<T, string>> selector, ScriptContext context)
		{
			string code = GetString(selector);
			string argName = selector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TResult>(context, _options, code, argName);
			return query.Select(lambda.Compile());
		}

		public static IOrderedEnumerable<T> OrderByScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector)
		{
			return OrderByScript<T, TKey>(query, keySelector, ScriptContext.Root);
		}

		public static IOrderedEnumerable<T> OrderByScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.OrderBy(lambda.Compile());
		}

		public static IOrderedEnumerable<T> OrderByDescendingScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector)
		{
			return OrderByDescendingScript<T, TKey>(query, keySelector, ScriptContext.Root);
		}

		public static IOrderedEnumerable<T> OrderByDescendingScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.OrderByDescending(lambda.Compile());
		}

		public static IOrderedEnumerable<T> ThenByScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector)
		{
			return ThenByScript<T, TKey>(query, keySelector, ScriptContext.Root);
		}

		public static IOrderedEnumerable<T> ThenByScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.ThenBy(lambda.Compile());
		}

		public static IOrderedEnumerable<T> ThenByDescendingScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector)
		{
			return ThenByDescendingScript<T, TKey>(query, keySelector, ScriptContext.Root);
		}

		public static IOrderedEnumerable<T> ThenByDescendingScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.ThenByDescending(lambda.Compile());
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
