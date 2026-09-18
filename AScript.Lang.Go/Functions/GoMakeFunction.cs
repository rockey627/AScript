using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Go.Functions
{
	/// <summary>
	/// make 函数 - 创建slice/map/chan
	/// </summary>
	public class GoMakeFunction : IFunctionEvaluator, IFunctionBuilder
	{
		private static readonly MethodInfo Method_makeList_1 = typeof(GoMakeFunction).GetMethod("makeList", new[] { typeof(Type), typeof(int) });
		private static readonly MethodInfo Method_makeList_2 = typeof(GoMakeFunction).GetMethod("makeList", new[] { typeof(Type), typeof(int), typeof(int) });

		public static readonly GoMakeFunction Instance = new GoMakeFunction();

		public void Build(FunctionBuildArgs e)
		{
			var typeExpr = e.BuildArgs(0);
			if (!(typeExpr is ConstantExpression constantExpression)) return;
			var type = (Type)constantExpression.Value;
			if (type.IsGenericType)
			{
				var definition = type.GetGenericTypeDefinition();
				if (definition == typeof(List<>))
				{
					if (e.Args.Count == 1)
					{
						e.Result = Expression.New(type);
						return;
					}
					if (e.Args.Count == 2)
					{
						var length = e.BuildArgs(1);
						e.Result = Expression.Convert(Expression.Call(Method_makeList_1, typeExpr, length), type);
						return;
					}
					if (e.Args.Count == 3)
					{
						var length = e.BuildArgs(1);
						var capacity = e.BuildArgs(2);
						e.Result = Expression.Convert(Expression.Call(Method_makeList_2, typeExpr, length, capacity), type);
						return;
					}
					return;
				}
				if (definition == typeof(Dictionary<,>))
				{
					if (e.Args.Count == 1)
					{
						e.Result = Expression.New(type);
						return;
					}
					if (e.Args.Count == 2)
					{
						var capacity = e.BuildArgs(1);
						e.Result = Expression.New(type.GetConstructor(new[] { typeof(int) }), capacity);
						return;
					}
					return;
				}
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			var type = (Type)e.EvalArgs(0, out _);
			if (type.IsGenericType)
			{
				var definition = type.GetGenericTypeDefinition();
				if (definition == typeof(List<>))
				{
					if (e.Args.Count == 1)
					{
						e.SetResult(makeList(type), type);
						return;
					}
					if (e.Args.Count == 2)
					{
						int length = (int)e.EvalArgs(1, out _);
						e.SetResult(makeList(type, length), type);
						return;
					}
					if (e.Args.Count == 3)
					{
						int length = (int)e.EvalArgs(1, out _);
						int capacity = (int)e.EvalArgs(2, out _);
						e.SetResult(makeList(type, length, capacity), type);
						return;
					}
					return;
				}
				if (definition == typeof(Dictionary<,>))
				{
					if (e.Args.Count == 1)
					{
						e.SetResult(makeMap(type), type);
						return;
					}
					if (e.Args.Count == 2)
					{
						int capacity = (int)e.EvalArgs(1, out _);
						e.SetResult(makeMap(type, capacity), type);
						return;
					}
					return;
				}
			}
		}

		public static object makeList(Type type)
		{
			return Activator.CreateInstance(type);
		}

		public static object makeList(Type type, int length)
		{
			var list = (IList)Activator.CreateInstance(type);
			var itemType = type.GenericTypeArguments[0];
			object defaultValue;
			if (itemType.IsValueType) defaultValue = Activator.CreateInstance(itemType);
			else defaultValue = null;
			for (int i = 0; i < length; i++)
			{
				list.Add(defaultValue);
			}
			return list;
		}

		public static object makeList(Type type, int length, int capacity)
		{
			var list = (IList)Activator.CreateInstance(type, new object[] { capacity });
			var itemType = type.GenericTypeArguments[0];
			object defaultValue;
			if (itemType.IsValueType) defaultValue = Activator.CreateInstance(itemType);
			else defaultValue = null;
			for (int i = 0; i < length; i++)
			{
				list.Add(defaultValue);
			}
			return list;
		}

		public static object makeMap(Type type)
		{
			return Activator.CreateInstance(type);
		}

		public static object makeMap(Type type, int capacity)
		{
			return Activator.CreateInstance(type, capacity);
		}

	}
}
