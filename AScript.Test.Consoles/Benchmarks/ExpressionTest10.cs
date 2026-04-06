using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest10
	{
		private static string FilePath = "Benchmarks/ExpressionTest10.txt";
		private static int r = 100 * (100 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1_Stream()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(File.OpenRead(FilePath));
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript1_NoCache()
		{
			var script = new AScript.Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = (int)script.Eval(File.OpenRead(FilePath));
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript1_UseCache1()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(File.OpenRead(FilePath), -1, FilePath);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript1_UseCache2()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(() => File.OpenRead(FilePath), -1, FilePath);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_String()
		{
			string s = File.ReadAllText(FilePath);
			var script = new AScript.Script();
			var result = (int)script.Eval(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}
	}
}
