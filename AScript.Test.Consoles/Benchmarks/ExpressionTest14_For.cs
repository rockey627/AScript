using System;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest14_For
	{
		private static readonly string s = @"
int m=0;
for(int n=0;n<10;n++) {
	m+=2;
	if(n%2==0) {
		m+=1;
		continue;
	}
	m+=3;
}
m+=3;
m+15";
		private static readonly int r;

		static ExpressionTest14_For()
		{
			int m = 0;
			for (int n=0; n < 10; n++)
			{
				m += 2;
				if (n % 2 == 0)
				{
					m += 1;
					continue;
				}
				m += 3;
			}
			m += 3;
			r = m + 15;
		}

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
