using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest13
	{
		private static string FilePath1 = "Benchmarks/ExpressionTest10.txt";
		private static string FilePath2 = "Benchmarks/ExpressionTest12.txt";
		private static int r = 100 * (100 + 5) * (6 - 2);
		private static string s1 = File.ReadAllText(FilePath1);
		private static string s2 = File.ReadAllText(FilePath2);


		[Benchmark]
		public void AScript()
		{
			var script = new AScript.Script();
			script.Context.SetVar("n", 100);
			script.Eval(s1);
			script.Eval(s2);
			//if (result != r)
			//{
			//	throw new Exception("result error");
			//}
		}
	}
}
