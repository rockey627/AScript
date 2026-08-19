using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.Scripting;
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
	public class DynamicExpressoTest02_var
	{
		private static readonly string s = "a * (b + 5) * (c-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Expresso2()
		{
			var interpreter = new DynamicExpresso.Interpreter();
			interpreter.SetVariable("a", 100);
			interpreter.SetVariable("b", 5);
			interpreter.SetVariable("c", 6);
			var result = interpreter.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

		//private static readonly ConcurrentDictionary<string, Delegate> _DynamicExpresso3Dict = new ConcurrentDictionary<string, Delegate>();

		//[Benchmark]
		//public void Expresso3()
		//{
		//	string key = s;
		//	if (!_DynamicExpresso3Dict.TryGetValue(key, out var func))
		//	{
		//		var interpreter = new DynamicExpresso.Interpreter();
		//		interpreter.SetVariable("a", 100);
		//		interpreter.SetVariable("b", 5);
		//		interpreter.SetVariable("c", 6);
		//		func = interpreter.ParseAsDelegate<Func<int>>(s);
		//		_DynamicExpresso3Dict[key] = func;
		//	}
		//	// interpreter.SetVariable("c", 8); 这里修改变量值，委托执行结果不变，些缓存对比无意义
		//	var result = ((Func<int>)func)();
		//	if (result != r) throw new Exception("result error");
		//}
	}
}
