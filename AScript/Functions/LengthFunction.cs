using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	public class LengthFunction : IFunctionEvaluator, IFunctionBuilder
	{
		private static readonly MethodInfo Method_Queryable_Count1 = typeof(Queryable).GetMethods().FirstOrDefault(a => a.Name == "Count" && a.GetParameters().Length == 1);
		private static readonly MethodInfo Method_Enumerable_Count1 = typeof(Enumerable).GetMethods().FirstOrDefault(a => a.Name == "Count" && a.GetParameters().Length == 1);

		public static readonly LengthFunction Instance = new LengthFunction();

		private readonly Type _lengthType;

		public LengthFunction() { }
		public LengthFunction(Type lengthType)
		{
			_lengthType = lengthType;
		}

		public void Build(FunctionBuildArgs e)
		{
			var a = e.BuildArgs(0);
			if (a.Type == typeof(string) || a.Type.IsArray)
			{
				e.Result = TryChangeType(Expression.Property(a, "Length"));
				return;
			}
			if (typeof(IList).IsAssignableFrom(a.Type) || typeof(IDictionary).IsAssignableFrom(a.Type))
			{
				e.Result = TryChangeType(Expression.Property(a, "Count"));
				return;
			}
			if (typeof(IQueryable).IsAssignableFrom(a.Type))
			{
				if (a.Type.IsGenericType)
				{
					var countMethod = Method_Queryable_Count1.MakeGenericMethod(a.Type.GetGenericArguments()[0]);
					e.Result = TryChangeType(Expression.Call(countMethod, a));
					return;
				}
			}
			if (typeof(IEnumerable).IsAssignableFrom(a.Type))
			{
				if (a.Type.IsGenericType)
				{
					var countMethod = Method_Enumerable_Count1.MakeGenericMethod(a.Type.GetGenericArguments()[0]);
					e.Result = TryChangeType(Expression.Call(countMethod, a));
					return;
				}
			}
		}

		private Expression TryChangeType(Expression expr)
		{
			if (_lengthType == null || _lengthType == typeof(int)) return expr;
			return Expression.Convert(expr, _lengthType);
		}

		private object TryChangeType(object length)
		{
			if (_lengthType == null || _lengthType == typeof(int)) return length;
			return Convert.ChangeType(length, _lengthType);
		}

		public void Eval(FunctionEvalArgs e)
		{
			var a = e.EvalArgs(0, out _);
			if (a == null)
			{
				e.SetResult(TryChangeType(0));
				return;
			}
			if (a is string s)
			{
				e.SetResult(TryChangeType(s.Length));
				return;
			}
			if (a is IList list)
			{
				e.SetResult(TryChangeType(list.Count));
				return;
			}
			if (a is IDictionary dict)
			{
				e.SetResult(TryChangeType(dict.Count));
				return;
			}
			if (a is IQueryable)
			{
				var type = a.GetType();
				if (type.IsGenericType)
				{
					var countMethod = Method_Queryable_Count1.MakeGenericMethod(type.GetGenericArguments()[0]);
					e.SetResult(TryChangeType(countMethod.Invoke(null, new[] { a })));
					return;
				}
			}
			if (a is IEnumerable)
			{
				var type = a.GetType();
				if (type.IsGenericType)
				{
					var countMethod = Method_Enumerable_Count1.MakeGenericMethod(type.GetGenericArguments()[0]);
					e.SetResult(TryChangeType(countMethod.Invoke(null, new[] { a })));
					return;
				}
				//int n = 0;
				//var it = en.GetEnumerator();
				//while (it.MoveNext())
				//{
				//	n++;
				//}
				//e.SetResult(n);
				//return;
			}
		}
	}
}
