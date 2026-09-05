using AScript.Extensions;
using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.ZExpressionsTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ZExpressionsTest10_linqdynamic
	{
		private static readonly List<int> _list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		private static readonly IQueryable<int> _query = _list.AsQueryable();
		private static readonly int r = 5;

		[Benchmark]
		public void Queryable_AScript()
		{
			// 此场景一般是拼接查询条件字符串，AScript不提供编译缓存方案
			var result = _query.WhereScript(a => "a % 2 == 0").Count();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Queryable_ZExpressions()
		{
			// 此场景一般是拼接查询条件字符串，不使用缓存
			Z.Expressions.EvalManager.DefaultContext.UseCache = false;
			var result = _query.WhereDynamic(a => "a % 2 == 0").Count();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Queryable_ZExpressions_Cache()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = true;
			var result = _query.WhereDynamic(a => "a % 2 == 0").Count();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Enumerable_AScript()
		{
			// 此场景一般是拼接查询条件字符串，AScript不提供编译缓存方案
			var result = _list.WhereScript(a => "a % 2 == 0").Count();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Enumerable_ZExpressions()
		{
			// 此场景一般是拼接查询条件字符串，不使用缓存
			Z.Expressions.EvalManager.DefaultContext.UseCache = false;
			var result = _list.WhereDynamic(a => "a % 2 == 0").Count();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Enumerable_ZExpressions_Cache()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = true;
			var result = _list.WhereDynamic(a => "a % 2 == 0").Count();
			if (result != r) throw new Exception("result error");
		}
	}
}
