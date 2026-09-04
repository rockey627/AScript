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
	public class ZExpressionsTest01_const
	{
		private static readonly string s = "100 * (5 + 5) * (6-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void ZExpressions2()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = false;
			var result = Z.Expressions.Eval.Execute<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void ZExpressions3()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = true;
			var result = Z.Expressions.Eval.Execute<int>(s);
			if (result != r) throw new Exception("result error");
		}

	}
}
