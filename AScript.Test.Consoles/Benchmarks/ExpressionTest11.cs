using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest11
	{
		private static string FilePath = "Benchmarks/ExpressionTest10.txt";
		private static int r = 100 * (100 + 5) * (6 - 2);
		public static string s = File.ReadAllText(FilePath);

		[Benchmark]
		public void AScript()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}
	}
}
