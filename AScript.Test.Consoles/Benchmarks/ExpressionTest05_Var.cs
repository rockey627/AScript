using System;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest05_Var
	{
		private static readonly string s = "int v=5;100 * (v + 5) * (6-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
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
			var result = (int)script.Eval(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache2()
		{
			var script = new AScript.Script();
			script.Options.RewriteVariables = false;
			var result = (int)script.Eval(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache2()
		{
			var script = new AScript.Script();
			script.Options.RewriteVariables = false;
			var result = (int)script.Eval(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}
	}
}
