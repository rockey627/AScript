using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AScript.Operators
{
	/// <summary>
	/// 示例：s="01234"; s[1:3]为"12"
	/// </summary>
	public class IndexStartEndOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly IndexStartEndOperator Instance = new IndexStartEndOperator();

		public void Build(FunctionBuildArgs e)
		{
			throw new NotImplementedException();
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

			if (target is string str)
			{
				// Python风格：负数索引从末尾计算
				int len = str.Length;
				int startIdx = start < 0 ? len + start : start;
				if (!end.HasValue)
				{
					e.SetResult(str.Substring(startIdx));
					return;
				}

				int endIdx = end.Value < 0 ? len + end.Value : end.Value;

				// 边界约束
				startIdx = Math.Max(0, Math.Min(startIdx, len));
				endIdx = Math.Max(0, Math.Min(endIdx, len));

				if (startIdx == 0 && endIdx >= len)
				{
					e.SetResult(str);
				}
				else
				{
					e.SetResult(str.Substring(startIdx, endIdx - startIdx));
				}
				return;
			}
			if (target is IList list)
			{
				int len = list.Count;
				int startIdx = start < 0 ? len + start : start;
				int endIdx = end.HasValue ? (end.Value < 0 ? len + end.Value : end.Value) : len;

				// 边界约束
				startIdx = Math.Max(0, Math.Min(startIdx, len));
				endIdx = Math.Max(0, Math.Min(endIdx, len));

				var result = new List<object>();
				for (int i = startIdx; i < endIdx; i++)
				{
					result.Add(list[i]);
				}
				e.SetResult(result);
				return;
			}
			else if (target is IEnumerable enumerable)
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

				var result = new List<object>();
				int idx = 0;
				foreach (var item in enumerable)
				{
					if (idx >= endIdx)
						break;
					if (idx >= startIdx)
						result.Add(item);
					idx++;
				}
				e.SetResult(result);
			}
		}
	}
}
