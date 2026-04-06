using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest09_if
	{
		private static readonly string s_z = @"
if (n==0) { n=n+1 }
else if (n==1) {n=n+2}
else if (n==2) {n=n+3}
else{n=n+4}
100 * (n + 5) * (6-2)
";

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			script.Context.SetVar("n", 2);
			var result = (int)script.Eval(s_z);
			if (result != 4000)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache()
		{
			var script = new AScript.Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("n", 2);
			var result = (int)script.Eval(s_z);
			if (result != 4000)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_UseCache()
		{
			var script = new AScript.Script();
			script.Context.SetVar("n", 2);
			var result = (int)script.Eval(s_z, -1);
			if (result != 4000)
			{
				throw new Exception("result error");
			}
		}
	}
}
