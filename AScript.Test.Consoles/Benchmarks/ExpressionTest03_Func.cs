using System;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest03_Func
	{
		private static readonly string s = "100 * test(5, 5) * (6-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			script.Context.AddFunc("test", Functions.test);
			var result = (int)script.Eval(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache()
		{
			var script = new AScript.Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddFunc("test", Functions.test);
			var result = (int)script.Eval(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache()
		{
			var script = new AScript.Script();
			script.Context.AddFunc("test", Functions.test);
			var result = (int)script.Eval(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		public class Functions
		{
			public static int test(int a, int b) => a + b;
		}
	}
}
