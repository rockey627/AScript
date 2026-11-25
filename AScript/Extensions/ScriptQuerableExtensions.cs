using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Extensions
{
	public static class ScriptQuerableExtensions
	{
		internal static readonly BuildOptions _options = new BuildOptions(Script.DefaultOptions)
		{
			CompileMode = ECompileMode.All,
			RewriteFunctions = false,
			RewriteVariables = false,
			Standalone = true
		};

		public static IQueryable<T> WhereScript<T>(this IQueryable<T> query, Expression<Func<T, string>> predicate)
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
		public static IQueryable<T> WhereScript<T>(this IQueryable<T> query, Expression<Func<T, string>> predicate, object parameter)
		{
			return WhereScript(query, predicate, GetScriptContext(parameter));
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
		public static IQueryable<TResult> SelectScript<T, TResult>(this IQueryable<T> query, Expression<Func<T, string>> selector, object parameter)
		{
			return SelectScript<T, TResult>(query, selector, GetScriptContext(parameter));
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
		public static IOrderedQueryable<T> OrderByScript<T, TKey>(this IQueryable<T> query, Expression<Func<T, string>> keySelector, object parameter)
		{
			return OrderByScript<T, TKey>(query, keySelector, GetScriptContext(parameter));
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
		public static IOrderedQueryable<T> OrderByDescendingScript<T, TKey>(this IQueryable<T> query, Expression<Func<T, string>> keySelector, object parameter)
		{
			return OrderByDescendingScript<T, TKey>(query, keySelector, GetScriptContext(parameter));
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
		public static IOrderedQueryable<T> ThenByScript<T, TKey>(this IOrderedQueryable<T> query, Expression<Func<T, string>> keySelector, object parameter)
		{
			return ThenByScript<T, TKey>(query, keySelector, GetScriptContext(parameter));
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
		public static IOrderedQueryable<T> ThenByDescendingScript<T, TKey>(this IOrderedQueryable<T> query, Expression<Func<T, string>> keySelector, object parameter)
		{
			return ThenByDescendingScript<T, TKey>(query, keySelector, GetScriptContext(parameter));
		}

		public static IOrderedQueryable<T> ThenByDescendingScript<T, TKey>(this IOrderedQueryable<T> query, Expression<Func<T, string>> keySelector, ScriptContext context)
		{
			string code = GetString(keySelector);
			string argName = keySelector.Parameters[0].Name;
			var lambda = Script.Lambda<T, TKey>(context, _options, code, argName);
			return query.ThenByDescending(lambda);
		}

		internal static ScriptContext GetScriptContext(object arg0)
		{
			var context = new ScriptContext();
			context.SetVar("@0", arg0);
			if (arg0 == null) return context;
			if (arg0 is IDictionary dict)
			{
				foreach (var key in dict.Keys)
				{
					if (key is string s)
					{
						context.SetVar(s, dict[key]);
					}
				}
			}
			else if (arg0 is IList list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					context.SetVar($"@{i}", list[i]);
				}
			}
			else
			{
				var type0 = arg0.GetType();
				if (Type.GetTypeCode(type0) == TypeCode.Object)
				{
					foreach (var p in type0.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
					{
						if (!p.CanRead) continue;
						var v = p.GetValue(arg0);
						context.SetVar(p.Name, v, v == null ? p.PropertyType : null);
					}
				}
			}
			return context;
		}

		internal static string GetString<T>(Expression<Func<T, string>> lambda)
		{
			if (TryGetString(lambda.Body, out string code))
			{
				return code;
			}
			return lambda.Compile()(default);
		}

		internal static bool TryGetString(Expression expression, out string s)
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
