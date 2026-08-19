using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.DynamicExpressoTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class DynamicExpressoTest01_const
	{
		private static readonly string s = "100 * (5 + 5) * (6-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Expresso2()
		{
			var interpreter = new DynamicExpresso.Interpreter();
			var result = interpreter.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		private static Script _Script3;

		[Benchmark]
		public void AScript3_Cache()
		{
			if (_Script3 == null)
			{
				_Script3 = new Script();
			}
			var result = _Script3.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

		private static readonly ConcurrentDictionary<string, Delegate> _DynamicExpresso3Dict = new ConcurrentDictionary<string, Delegate>();

		[Benchmark]
		public void Expresso3()
		{
			string key = s;
			if (!_DynamicExpresso3Dict.TryGetValue(key, out var func))
			{
				var interpreter = new DynamicExpresso.Interpreter();
				func = interpreter.ParseAsDelegate<Func<int>>(s);
				_DynamicExpresso3Dict[key] = func;
			}
			var result = ((Func<int>)func)();
			if (result != r) throw new Exception("result error");
		}
	}
}
