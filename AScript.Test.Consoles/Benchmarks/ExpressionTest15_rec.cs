using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	/// <summary>
	/// 递归
	/// </summary>
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest15_rec
	{
		private static readonly string s = @"
int exec(int a) {
	if (a < 1) return 0;
	a + exec(a-1);
}
exec(10)
";
		private static readonly int r = 55;

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
		public void AScript3_UseCache()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}
	}
}
