using System;
using System.Collections;
using System.Collections.Generic;
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

		public static readonly IndexOperator Instance = new IndexOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;

			var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);

			// 如果right是object类型，转换为实际需要的类型
			if (right.Type == typeof(object))
			{
				var elementType = GetIndexType(left.Type);
				if (elementType != null)
				{
					right = Expression.Convert(right, elementType);
				}
			}

			// 判断是否为数组类型
			if (left.Type.IsArray)
			{
				e.Result = Expression.ArrayIndex(left, right);
			}
			else
			{
				// 使用索引器（Item属性）访问
				var indexer = left.Type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
				if (indexer != null)
				{
					e.Result = Expression.Property(left, indexer, right);
				}
				else
				{
					// 如果没有索引器，尝试使用泛型Dictionary接口
					if (left.Type.IsGenericType)
					{
						var genericTypeDef = left.Type.GetGenericTypeDefinition();
						if (genericTypeDef == typeof(Dictionary<,>)
							|| genericTypeDef == typeof(IDictionary<,>))
						{
							// Dictionary访问
							var getItemMethod = left.Type.GetMethod("get_Item");
							if (getItemMethod != null)
							{
								e.Result = Expression.Call(left, getItemMethod, right);
								return;
							}
						}
					}

					// 使用动态表达式进行动态访问
					e.Result = Expression.Dynamic(
						IndexBinder,
						typeof(object),
						left,
						right);
				}
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
				dynamic arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
				dynamic arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				if (arg0 is IList list)
				{
					if (arg1 is int index)
					{
						if (index < 0)
						{
							index = list.Count + index;
						}
						e.SetResult(list[index]);
						return;
					}
				}
				else if (arg0 is string s)
				{
					if (arg1 is int index)
					{
						if (index < 0)
						{
							index = s.Length + index;
						}
						e.SetResult(s[index]);
						return;
					}
				}
				e.SetResult(arg0[arg1]);
			}
		}
	}
}
