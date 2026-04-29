using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	public class ContainsFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly ContainsFunction Instance = new ContainsFunction();

		private readonly bool _reverse;

		public ContainsFunction() { }
		public ContainsFunction(bool reverse)
		{
			_reverse = reverse;
		}

		public void Build(FunctionBuildArgs e)
		{
			if ((e.Args == null || e.Args.Count < 2) && (e.ArgExprs == null || e.ArgExprs.Count < 2)) return;

			Expression arg0Expr, arg1Expr;
			if (e.ArgExprs != null)
			{
				arg0Expr = e.ArgExprs[0];
				arg1Expr = e.ArgExprs[1];
			}
			else
			{
				arg0Expr = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				arg1Expr = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			}
			if (_reverse)
			{
				var tmpExpr = arg0Expr;
				arg0Expr = arg1Expr;
				arg1Expr = tmpExpr;
			}
			var type0 = arg0Expr.Type;
			MethodInfo containsMethod;

			// Dictionary<,> 类型，调用 ContainsKey 方法
			if (type0.IsGenericType && type0.GetGenericTypeDefinition() == typeof(Dictionary<,>))
			{
				containsMethod = type0.GetMethod("ContainsKey", BindingFlags.Public | BindingFlags.Instance);
				if (containsMethod != null)
				{
					e.Result = Expression.Call(arg0Expr, containsMethod, arg1Expr);
					return;
				}
			}

			// HashSet<> 类型，调用 Contains 方法
			if (type0.IsGenericType && type0.GetGenericTypeDefinition() == typeof(HashSet<>))
			{
				containsMethod = type0.GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance);
				if (containsMethod != null)
				{
					e.Result = Expression.Call(arg0Expr, containsMethod, arg1Expr);
					return;
				}
			}

			// List<> 类型，调用 Contains 方法
			if (type0.IsGenericType && type0.GetGenericTypeDefinition() == typeof(List<>))
			{
				containsMethod = type0.GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance);
				if (containsMethod != null)
				{
					e.Result = Expression.Call(arg0Expr, containsMethod, arg1Expr);
					return;
				}
			}

			// 其他类型（如非泛型 IDictionary），调用 IDictionary.Contains
			if (typeof(IDictionary).IsAssignableFrom(type0))
			{
				var idictContainsMethod = typeof(IDictionary).GetMethod("Contains");
				e.Result = Expression.Call(arg0Expr, idictContainsMethod, arg1Expr);
				return;
			}

			var elementType = type0.HasElementType ? type0.GetElementType() : type0.GetGenericArguments()[0];
			var containsMethodGeneric = typeof(Enumerable).GetMethod("Contains");
			var containsMethodSpecific = containsMethodGeneric.MakeGenericMethod(elementType);
			e.Result = Expression.Call(containsMethodSpecific, arg0Expr, arg1Expr);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args == null || e.Args.Count < 2) return;

			var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
			var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type1);

			if (_reverse)
			{
				var tmp = arg0;
				arg0 = arg1;
				arg1 = tmp;
			}

			bool result = Contains(arg0, arg1);
			e.SetResult(result);
		}

		private static bool Contains(object collection, object item)
		{
			if (collection is IDictionary dict)
			{
				return dict.Contains(item);
			}

			var type = collection.GetType();
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
			{
				var containsMethod = type.GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance);
				return (bool)containsMethod.Invoke(collection, new[] { item });
			}

			if (collection is IEnumerable enumerable)
			{
				foreach (var i in enumerable)
				{
					if (object.Equals(i, item))
						return true;
				}
			}

			return false;
		}
	}
}
