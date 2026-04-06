using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class DynamicTest2
	{
		private static int Test(string s) => s.Length;

		[Benchmark]
		public void DynamicInvoke()
		{
			Delegate d = Test;
			d.DynamicInvoke("hello");
		}

		[Benchmark]
		public void Func()
		{
			Delegate d = Test;
			((Func<string, int>)d)("hello");
		}

	}
}
