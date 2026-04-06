using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest12
	{
		private static string FilePath = "Benchmarks/ExpressionTest12.txt";
		private static int r = 100 * (100 + 5) * (6 - 2);
		private static string s2 = File.ReadAllText(FilePath);

		[Benchmark]
		public void AScript1_read()
		{
			var script = new AScript.Script();
			script.Context.SetVar("n", 100);
			var result = (int)script.Eval(File.OpenRead(FilePath));
			//if (result != r)
			//{
			//	throw new Exception("result error");
			//}
		}

		[Benchmark]
		public void AScript2_string()
		{
			string s = File.ReadAllText(FilePath);
			var script = new AScript.Script();
			script.Context.SetVar("n", 100);
			var result = (int)script.Eval(s);
			//if (result != r)
			//{
			//	throw new Exception("result error");
			//}
		}

		[Benchmark]
		public void AScript3_string()
		{
			var script = new AScript.Script();
			script.Context.SetVar("n", 100);
			var result = (int)script.Eval(s2);
			//if (result != r)
			//{
			//	throw new Exception("result error");
			//}
		}
	}
}
