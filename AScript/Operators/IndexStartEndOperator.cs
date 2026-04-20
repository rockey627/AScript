using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Operators
{
	/// <summary>
	/// 示例：s="01234"; s[1:3]为"12"
	/// </summary>
	public class IndexStartEndOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly IndexStartEndOperator Instance = new IndexStartEndOperator();

		private static readonly MethodInfo Method_Execute = typeof(IndexStartEndOperator).GetMethod("Execute");

		public void Build(FunctionBuildArgs e)
		{
			var target = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var start = e.Args[1]?.Build(e.BuildContext, e.ScriptContext, e.Options) ?? Expression.Constant(0);
			var end = e.Args[2]?.Build(e.BuildContext, e.ScriptContext, e.Options) ?? ExpressionUtils.Constant_null;
			var result = Expression.Call(Method_Execute, target, start, Expression.Convert(end, typeof(int?)));
			if (target.Type == typeof(string))
			{
				e.Result = Expression.Convert(result, typeof(string));
			}
			else
			{
				var elementType = target.Type.HasElementType ? target.Type.GetElementType() : target.Type.GetGenericArguments()[0];
				var listType = typeof(List<>).MakeGenericType(elementType);
				e.Result = Expression.Convert(result, listType);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args == null || e.Args.Count != 3) return;

			var targetNode = e.Args[0];
			var startNode = e.Args[1];
			var endNode = e.Args[2];
			var target = targetNode.Eval(e.Context, e.Options, e.Control, out _);
			int start = startNode == null ? 0 : (int)startNode.Eval(e.Context, e.Options, e.Control, out _);
			int? end = endNode == null ? (int?)null : (int)endNode.Eval(e.Context, e.Options, e.Control, out _);

			var result = Execute(target, start, end);
			e.SetResult(result);

			//if (target is string str)
			//{
			//	// Python风格：负数索引从末尾计算
			//	int len = str.Length;
			//	int startIdx = start < 0 ? len + start : start;
			//	if (!end.HasValue)
			//	{
			//		e.SetResult(str.Substring(startIdx));
			//		return;
			//	}

			//	int endIdx = end.Value < 0 ? len + end.Value : end.Value;

			//	// 边界约束
			//	startIdx = Math.Max(0, Math.Min(startIdx, len));
			//	endIdx = Math.Max(0, Math.Min(endIdx, len));

			//	if (startIdx == 0 && endIdx >= len)
			//	{
			//		e.SetResult(str);
			//	}
			//	else
			//	{
			//		e.SetResult(str.Substring(startIdx, endIdx - startIdx));
			//	}
			//	return;
			//}
			//if (target is IList list)
			//{
			//	int len = list.Count;
			//	int startIdx = start < 0 ? len + start : start;
			//	int endIdx = end.HasValue ? (end.Value < 0 ? len + end.Value : end.Value) : len;

			//	// 边界约束
			//	startIdx = Math.Max(0, Math.Min(startIdx, len));
			//	endIdx = Math.Max(0, Math.Min(endIdx, len));

			//	var result = new List<object>();
			//	for (int i = startIdx; i < endIdx; i++)
			//	{
			//		result.Add(list[i]);
			//	}
			//	e.SetResult(result);
			//	return;
			//}
			//else if (target is IEnumerable enumerable)
			//{
			//	// 处理通用 IEnumerable
			//	int len = 0;
			//	foreach (var _ in enumerable)
			//	{
			//		len++;
			//	}

			//	int startIdx = start < 0 ? len + start : start;
			//	int endIdx = end.HasValue ? (end.Value < 0 ? len + end.Value : end.Value) : len;

			//	startIdx = Math.Max(0, Math.Min(startIdx, len));
			//	endIdx = Math.Max(0, Math.Min(endIdx, len));

			//	var result = new List<object>();
			//	int idx = 0;
			//	foreach (var item in enumerable)
			//	{
			//		if (idx >= endIdx)
			//			break;
			//		if (idx >= startIdx)
			//			result.Add(item);
			//		idx++;
			//	}
			//	e.SetResult(result);
			//}
		}

		public static object Execute(object target, int start, int? end)
		{
			if (target == null) return null;
			if (target is string str)
			{
				// Python风格：负数索引从末尾计算
				int len = str.Length;
				int startIdx = start < 0 ? len + start : start;
				if (startIdx == 0 && !end.HasValue) return str;
				if (!end.HasValue)
				{
					return str.Substring(startIdx);
				}

				int endIdx = end.Value < 0 ? len + end.Value : end.Value;

				// 边界约束
				startIdx = Math.Max(0, Math.Min(startIdx, len));
				endIdx = Math.Max(0, Math.Min(endIdx, len));

				if (startIdx == 0 && endIdx >= len)
				{
					return str;
				}
				return str.Substring(startIdx, endIdx - startIdx);
			}

			var targetType = target.GetType();
			var elementType = targetType.HasElementType ? targetType.GetElementType() : targetType.GetGenericArguments()[0];
			var listType = typeof(List<>).MakeGenericType(elementType);

			if (target is IList list)
			{
				int len = list.Count;
				int startIdx = start < 0 ? len + start : start;
				int endIdx = end.HasValue ? (end.Value < 0 ? len + end.Value : end.Value) : len;

				// 边界约束
				startIdx = Math.Max(0, Math.Min(startIdx, len));
				endIdx = Math.Max(0, Math.Min(endIdx, len));

				var result = (IList)Activator.CreateInstance(listType, endIdx - startIdx);
				for (int i = startIdx; i < endIdx; i++)
				{
					result.Add(list[i]);
				}
				return result;
			}
			if (target is IEnumerable enumerable)
			{
				// 处理通用 IEnumerable
				int len = 0;
				foreach (var _ in enumerable)
				{
					len++;
				}

				int startIdx = start < 0 ? len + start : start;
				int endIdx = end.HasValue ? (end.Value < 0 ? len + end.Value : end.Value) : len;

				startIdx = Math.Max(0, Math.Min(startIdx, len));
				endIdx = Math.Max(0, Math.Min(endIdx, len));

				var result = (IList)Activator.CreateInstance(listType, endIdx - startIdx);
				int idx = 0;
				foreach (var item in enumerable)
				{
					if (idx >= endIdx)
						break;
					if (idx >= startIdx)
						result.Add(item);
					idx++;
				}
				return result;
			}
			return null;
		}
	}
}
