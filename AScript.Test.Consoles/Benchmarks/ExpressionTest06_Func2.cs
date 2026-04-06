using System;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest06_Func2
	{
		private static readonly string s = "int test(int a,int b){ return a+b;} int test2(int a){return a*2;}100 * test(5,5)*test2(10) * (6-2)";
		private static readonly int r = 100 * (5 + 5) * (10 * 2) * (6 - 2);

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
		public void AScript3_UseCache()
		{
			var script = new AScript.Script();
			var result = script.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}
	}
}
