using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.ZExpressionsTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ZExpressionsTest08_compile
	{
		private static readonly string s = @"
var list = new List<int>() { 1, 2, 3, 4 };
return list.Where(x => x > 2).ToList();";

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Compile<List<int>>(s);
		}

		[Benchmark]
		public void ZExpressions1()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = false;
			var result = Z.Expressions.Eval.Compile<Func<List<int>>>(s);
		}

		[Benchmark]
		public void AScript2_Cache()
		{
			var script = new Script();
			var result = script.Compile<List<int>>(s, -1);
		}

		[Benchmark]
		public void ZExpressions2()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = true;
			var result = Z.Expressions.Eval.Compile<Func<List<int>>>(s);
		}

	}
}
