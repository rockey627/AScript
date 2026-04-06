using System;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest06_Func
	{
		private static readonly string s = "int test(int a,int b){ return a+b;}100 * test(5,5) * (6-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			var result = script.Eval<int>(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache()
		{
			var script = new AScript.Script();
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache2()
		{
			var script = new AScript.Script();
			script.Options.RewriteFunctions = false;
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache()
		{
			var script = new AScript.Script();
			var result = script.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache2()
		{
			var script = new AScript.Script();
			script.Options.RewriteFunctions = false;
			var result = script.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}
	}
}
