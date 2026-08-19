using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.DynamicExpressoTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class DynamicExpressoTest05_lambda
	{
		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			Expression<Func<int, int, int>> lambda = script.Lambda<int, int, int>("a + b", "a", "b");
			if (lambda == null) throw new Exception("result error");
		}

		[Benchmark]
		public void Expresso1()
		{
			var interpreter = new DynamicExpresso.Interpreter();
			Expression<Func<int, int, int>> lambda = interpreter.ParseAsExpression<Func<int, int, int>>("a + b", "a", "b");
			if (lambda == null) throw new Exception("result error");
		}
	}
}
