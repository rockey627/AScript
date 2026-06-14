using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	public class ContainsFunction : IFunctionEvaluator, IFunctionBuilder
	{
		private readonly bool _reverse;

		public ContainsFunction() { }
		public ContainsFunction(bool reverse)
		{
			_reverse = reverse;
		}

		public void Build(FunctionBuildArgs e)
		{
			if ((e.Args == null || e.Args.Count < 2) && (e.ArgExprs == null || e.ArgExprs.Count < 2)) return;

			Expression listExpr, itemExpr;
			if (e.ArgExprs != null)
			{
				listExpr = e.ArgExprs[0];
				itemExpr = e.ArgExprs[1];
			}
			else
			{
				listExpr = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				itemExpr = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			}
			if (_reverse)
			{
				var tmpExpr = listExpr;
				listExpr = itemExpr;
				itemExpr = tmpExpr;
			}
			var type0 = listExpr.Type;
			MethodInfo containsMethod;

			// Dictionary<,> 类型，调用 ContainsKey 方法
			if (type0.IsGenericType && type0.GetGenericTypeDefinition() == typeof(Dictionary<,>))
			{
				containsMethod = type0.GetMethod("ContainsKey", BindingFlags.Public | BindingFlags.Instance);
				if (containsMethod != null)
				{
					var definedType = containsMethod.GetParameters()[0].ParameterType;
					if (itemExpr.Type != definedType)
					{
						itemExpr = Expression.Convert(itemExpr, definedType);
					}
					e.Result = Expression.Call(listExpr, containsMethod, itemExpr);
					return;
				}
			}

			// HashSet<> 类型，调用 Contains 方法
			if (type0.IsGenericType && type0.GetGenericTypeDefinition() == typeof(HashSet<>))
			{
				containsMethod = type0.GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance);
				if (containsMethod != null)
				{
					var definedType = containsMethod.GetParameters()[0].ParameterType;
					if (itemExpr.Type != definedType)
					{
						itemExpr = Expression.Convert(itemExpr, definedType);
					}
					e.Result = Expression.Call(listExpr, containsMethod, itemExpr);
					return;
				}
			}

			// List<> 类型，调用 Contains 方法
			if (type0.IsGenericType && type0.GetGenericTypeDefinition() == typeof(List<>))
			{
				containsMethod = type0.GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance);
				if (containsMethod != null)
				{
					var definedType = containsMethod.GetParameters()[0].ParameterType;
					if (itemExpr.Type != definedType)
					{
						itemExpr = Expression.Convert(itemExpr, definedType);
					}
					e.Result = Expression.Call(listExpr, containsMethod, itemExpr);
					return;
				}
			}

			// 其他类型（如非泛型 IDictionary），调用 IDictionary.Contains
			if (typeof(IDictionary).IsAssignableFrom(type0))
			{
				var idictContainsMethod = typeof(IDictionary).GetMethod("Contains");
				var definedType = idictContainsMethod.GetParameters()[0].ParameterType;
				if (itemExpr.Type != definedType)
				{
					itemExpr = Expression.Convert(itemExpr, definedType);
				}
				e.Result = Expression.Call(listExpr, idictContainsMethod, itemExpr);
				return;
			}

			//var elementType = type0.HasElementType ? type0.GetElementType() : type0.GetGenericArguments()[0];
			//var containsMethodGeneric = typeof(Enumerable).GetMethod("Contains", new Type[] { type0, elementType });
			//var containsMethodSpecific = containsMethodGeneric.MakeGenericMethod(elementType);
			//e.Result = Expression.Call(containsMethodSpecific, arg0Expr, arg1Expr);
			e.Result = e.ScriptContext.BuildFunc(e.BuildContext, e.Options, e.Control, "Contains", false, new ITreeNode[] { new ExpressionNode(listExpr), new ExpressionNode(itemExpr) });
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

			bool result = Contains(e, arg0, arg1);
			e.SetResult(result);
		}

		private static bool Contains(FunctionEvalArgs e, object collection, object item)
		{
			if (collection is IDictionary dict)
			{
				return dict.Contains(item);
			}
			if (collection is IList list)
			{
				return list.Contains(item);
			}

			var type = collection.GetType();
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
			{
				var containsMethod = type.GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance);
				return (bool)containsMethod.Invoke(collection, new[] { item });
			}

			return (bool)e.Context.EvalFunc(e.Options, e.Control, "Contains", new ITreeNode[] { new ObjectNode(collection), new ObjectNode(item) });
		}
	}
}
