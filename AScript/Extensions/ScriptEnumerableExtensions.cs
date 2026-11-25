using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Extensions
{
	public static class ScriptEnumerableExtensions
	{
		public static IEnumerable<T> WhereScript<T>(this IEnumerable<T> query, Expression<Func<T, string>> predicate)
		{
			return WhereScript(query, predicate, new ScriptContext());
		}

		/// <summary>
		/// <para>如果parameter是IDictionary字典并且键类型为字符串，则展开字典，参数名为键名称</para>
		/// <para>如果parameter是IList列表，则展开元素，参数名为@0 @1 @2 ...</para>
		/// <para>如果parameter是对象类型，则展开属性，参数名为属性名称</para>
		/// <para>其他基本类型，参数为@0</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <param name="predicate"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static IEnumerable<T> WhereScript<T>(this IEnumerable<T> query, Expression<Func<T, string>> predicate, object parameter)
		{
			return WhereScript(query, predicate, ScriptQuerableExtensions.GetScriptContext(parameter));
		}

		public static IEnumerable<T> WhereScript<T>(this IEnumerable<T> query, Expression<Func<T, string>> predicate, ScriptContext context)
		{
			string code = ScriptQuerableExtensions.GetString(predicate);
			string argName = predicate.Parameters[0].Name;
			var lambda = Script.Lambda<T, bool>(context, ScriptQuerableExtensions._options, code, argName);
			return query.Where(lambda.Compile());
		}

		public static IEnumerable<TResult> SelectScript<T, TResult>(this IEnumerable<T> query, Expression<Func<T, string>> selector)
		{
			return SelectScript<T, TResult>(query, selector, new ScriptContext());
		}

		/// <summary>
		/// <para>如果parameter是IDictionary字典并且键类型为字符串，则展开字典，参数名为键名称</para>
		/// <para>如果parameter是IList列表，则展开元素，参数名为@0 @1 @2 ...</para>
		/// <para>如果parameter是对象类型，则展开属性，参数名为属性名称</para>
		/// <para>其他基本类型，参数为@0</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="query"></param>
		/// <param name="selector"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static IEnumerable<TResult> SelectScript<T, TResult>(this IEnumerable<T> query, Expression<Func<T, string>> selector, object parameter)
		{
			return SelectScript<T, TResult>(query, selector, ScriptQuerableExtensions.GetScriptContext(parameter));
		}

		public static IEnumerable<TResult> SelectScript<T, TResult>(this IEnumerable<T> query, Expression<Func<T, string>> selector, ScriptContext context)
		{
			string code = ScriptQuerableExtensions.GetString(selector);
			string argName = selector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TResult>(context, ScriptQuerableExtensions._options, code, argName);
			return query.Select(lambda.Compile());
		}

		public static IOrderedEnumerable<T> OrderByScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector)
		{
			return OrderByScript<T, TKey>(query, keySelector, new ScriptContext());
		}

		/// <summary>
		/// <para>如果parameter是IDictionary字典并且键类型为字符串，则展开字典，参数名为键名称</para>
		/// <para>如果parameter是IList列表，则展开元素，参数名为@0 @1 @2 ...</para>
		/// <para>如果parameter是对象类型，则展开属性，参数名为属性名称</para>
		/// <para>其他基本类型，参数为@0</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="query"></param>
		/// <param name="keySelector"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static IOrderedEnumerable<T> OrderByScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector, object parameter)
		{
			return OrderByScript<T, TKey>(query, keySelector, ScriptQuerableExtensions.GetScriptContext(parameter));
		}

		public static IOrderedEnumerable<T> OrderByScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = ScriptQuerableExtensions.GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, ScriptQuerableExtensions._options, code, argName);
			return query.OrderBy(lambda.Compile());
		}

		public static IOrderedEnumerable<T> OrderByDescendingScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector)
		{
			return OrderByDescendingScript<T, TKey>(query, keySelector, new ScriptContext());
		}

		/// <summary>
		/// <para>如果parameter是IDictionary字典并且键类型为字符串，则展开字典，参数名为键名称</para>
		/// <para>如果parameter是IList列表，则展开元素，参数名为@0 @1 @2 ...</para>
		/// <para>如果parameter是对象类型，则展开属性，参数名为属性名称</para>
		/// <para>其他基本类型，参数为@0</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="query"></param>
		/// <param name="keySelector"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static IOrderedEnumerable<T> OrderByDescendingScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector, object parameter)
		{
			return OrderByDescendingScript<T, TKey>(query, keySelector, ScriptQuerableExtensions.GetScriptContext(parameter));
		}

		public static IOrderedEnumerable<T> OrderByDescendingScript<T, TKey>(this IEnumerable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = ScriptQuerableExtensions.GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, ScriptQuerableExtensions._options, code, argName);
			return query.OrderByDescending(lambda.Compile());
		}

		public static IOrderedEnumerable<T> ThenByScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector)
		{
			return ThenByScript<T, TKey>(query, keySelector, new ScriptContext());
		}

		/// <summary>
		/// <para>如果parameter是IDictionary字典并且键类型为字符串，则展开字典，参数名为键名称</para>
		/// <para>如果parameter是IList列表，则展开元素，参数名为@0 @1 @2 ...</para>
		/// <para>如果parameter是对象类型，则展开属性，参数名为属性名称</para>
		/// <para>其他基本类型，参数为@0</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="query"></param>
		/// <param name="keySelector"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static IOrderedEnumerable<T> ThenByScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector, object parameter)
		{
			return ThenByScript<T, TKey>(query, keySelector, ScriptQuerableExtensions.GetScriptContext(parameter));
		}

		public static IOrderedEnumerable<T> ThenByScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = ScriptQuerableExtensions.GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, ScriptQuerableExtensions._options, code, argName);
			return query.ThenBy(lambda.Compile());
		}

		public static IOrderedEnumerable<T> ThenByDescendingScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector)
		{
			return ThenByDescendingScript<T, TKey>(query, keySelector, new ScriptContext());
		}

		/// <summary>
		/// <para>如果parameter是IDictionary字典并且键类型为字符串，则展开字典，参数名为键名称</para>
		/// <para>如果parameter是IList列表，则展开元素，参数名为@0 @1 @2 ...</para>
		/// <para>如果parameter是对象类型，则展开属性，参数名为属性名称</para>
		/// <para>其他基本类型，参数为@0</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="query"></param>
		/// <param name="keySelector"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static IOrderedEnumerable<T> ThenByDescendingScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector, object parameter)
		{
			return ThenByDescendingScript<T, TKey>(query, keySelector, ScriptQuerableExtensions.GetScriptContext(parameter));
		}

		public static IOrderedEnumerable<T> ThenByDescendingScript<T, TKey>(this IOrderedEnumerable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = ScriptQuerableExtensions.GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, ScriptQuerableExtensions._options, code, argName);
			return query.ThenByDescending(lambda.Compile());
		}
	}
}
