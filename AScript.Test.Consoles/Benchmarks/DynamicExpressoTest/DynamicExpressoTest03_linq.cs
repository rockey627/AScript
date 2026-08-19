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
	public class DynamicExpressoTest03_linq
	{
		private static readonly List<int> _list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		private static readonly string s = "list.Where(a=>a%2==0).ToList().Count";
		private static readonly int r = 5;

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			script.Context.SetVar("list", _list);
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			script.Context.SetVar("list", _list);
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Expresso2()
		{
			var options = InterpreterOptions.Default | InterpreterOptions.LambdaExpressions;
			var interpreter = new DynamicExpresso.Interpreter(options);
			interpreter.SetVariable("list", _list);
			var result = interpreter.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			script.Context.SetVar("list", _list);
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

	}
}
