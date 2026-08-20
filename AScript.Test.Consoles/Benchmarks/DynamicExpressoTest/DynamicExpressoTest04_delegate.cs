using BenchmarkDotNet.Attributes;
using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.DynamicExpressoTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class DynamicExpressoTest04_delegate
	{
		[Benchmark]
		public void AScript1_0()
		{
			var script = new Script();
			var func = script.Compile<int, int, int>("a + b", "a", "b");
			var result = func(4, 6);
			if (result != 10) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			// 编译结果不依赖上下文，与DynamicExpresso逻辑保持一致
			script.Options.Standalone = true;
			var func = script.Compile<int, int, int>("a + b", "a", "b");
			var result = func(4, 6);
			if (result != 10) throw new Exception("result error");
		}

		[Benchmark]
		public void Expresso1()
		{
			var interpreter = new DynamicExpresso.Interpreter();
			var func = interpreter.ParseAsDelegate<Func<int, int, int>>("a + b", "a", "b");
			var result = func(4, 6);
			if (result != 10) throw new Exception("result error");
		}

	}
}
