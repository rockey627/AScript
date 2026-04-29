using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace AScript.Operators
{
	public class IndexOperator : IFunctionEvaluator, IFunctionBuilder
	{
		/// <summary>
		/// 动态索引访问Binder
		/// </summary>
		private static readonly CallSiteBinder IndexBinder = Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(
			CSharpBinderFlags.None,
			typeof(object),
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

		private static readonly MethodInfo Method_GetItem = typeof(IndexOperator).GetMethod("GetItem");

		public static readonly IndexOperator Instance = new IndexOperator();

		private readonly bool _Char2String;

		public IndexOperator() { }
		/// <summary>
		/// 字符串索引的字符是否转换为字符串返回
		/// </summary>
		/// <param name="char2String"></param>
		public IndexOperator(bool char2String)
		{
			_Char2String = char2String;
		}

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;

			var target = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var index = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);

			// 如果right是object类型，转换为实际需要的类型
			if (index.Type == typeof(object))
			{
				var elementType = GetIndexType(target.Type);
				if (elementType != null)
				{
					index = Expression.Convert(index, elementType);
				}
			}

			// 处理 IList、string 和数组的负索引
			if (target.Type == typeof(string) || target.Type.GetInterfaces().Contains(typeof(IList)))
			{
				//var indexer = target.Type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
				var indexer = target.Type.GetProperty("Item", new Type[] { typeof(int) });
				if (indexer != null)
				{
					if (index.Type != typeof(int))
					{
						index = Expression.Convert(index, typeof(int));
					}
					var adjustedIndex = Expression.Condition(
						Expression.LessThan(index, Expression.Constant(0)),
						Expression.Add(
							target.Type == typeof(string)
								? Expression.Property(target, "Length")
								: Expression.Property(target, "Count"),
							index),
						index);
					e.Result = Expression.Property(target, indexer, adjustedIndex);
				}
				return;
			}

			// 数组类型
			if (target.Type.IsArray)
			{
				if (index.Type != typeof(int))
				{
					index = Expression.Convert(index, typeof(int));
				}
				var adjustedIndex = Expression.Condition(
					Expression.LessThan(index, Expression.Constant(0)),
					Expression.Add(Expression.ArrayLength(target), index),
					index);
				e.Result = Expression.ArrayIndex(target, adjustedIndex);
				return;
			}

			// 使用索引器（Item属性）访问
			var indexerProp = target.Type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
			if (indexerProp != null)
			{
				e.Result = Expression.Property(target, indexerProp, index);
			}
			else
			{
				// 如果没有索引器，尝试使用泛型Dictionary接口
				if (target.Type.IsGenericType)
				{
					var genericTypeDef = target.Type.GetGenericTypeDefinition();
					if (genericTypeDef == typeof(Dictionary<,>)
						|| genericTypeDef == typeof(IDictionary<,>))
					{
						// Dictionary访问
						var getItemMethod = target.Type.GetMethod("get_Item");
						if (getItemMethod != null)
						{
							e.Result = Expression.Call(target, getItemMethod, index);
							return;
						}
					}
				}

				//// 使用动态表达式进行动态访问
				//e.Result = Expression.Dynamic(
				//	IndexBinder,
				//	typeof(object),
				//	target,
				//	index);
				e.Result = Expression.Call(Method_GetItem, target, Expression.Convert(index, typeof(object)));
			}
		}

		/// <summary>
		/// 获取索引类型
		/// </summary>
		private static Type GetIndexType(Type containerType)
		{
			// 数组类型
			if (containerType.IsArray)
			{
				return typeof(int);
			}

			// Dictionary类型
			if (containerType.IsGenericType)
			{
				var args = containerType.GetGenericArguments();
				if (containerType.GetGenericTypeDefinition() == typeof(Dictionary<,>)
					|| containerType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
				{
					return args[0];
				}
				// 其他泛型集合尝试获取索引器类型
				var indexer = containerType.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
				if (indexer != null)
				{
					var indexParams = indexer.GetIndexParameters();
					if (indexParams.Length > 0)
					{
						return indexParams[0].ParameterType;
					}
				}
			}

			return null;
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count == 2)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				var result = GetItem(arg0, arg1);
				if (_Char2String && arg0 is string)
				{
					result = result.ToString();
				}
				e.SetResult(result);
			}
		}

		public static object GetItem(object arg0, object arg1)
		{
			if (arg0 is IList list)
			{
				int index = Convert.ToInt32(arg1);
				if (index < 0)
				{
					index = list.Count + index;
				}
				return list[index];
			}
			if (arg0 is string s)
			{
				int index = Convert.ToInt32(arg1);
				if (index < 0)
				{
					index = s.Length + index;
				}
				return s[index];
			}
			dynamic obj = arg0;
			dynamic key = arg1;
			return obj[key];
		}
	}
}
