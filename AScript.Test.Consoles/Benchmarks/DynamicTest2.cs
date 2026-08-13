using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
			int n = (int)d.DynamicInvoke("hello");
			if (n != 5)
			{
				throw new Exception();
			}
		}

		[Benchmark]
		public void Func()
		{
			Delegate d = Test;
			int n = ((Func<string, int>)d)("hello");
			if (n != 5)
			{
				throw new Exception();
			}
		}

		private static Func<object, int> newFunc;

		[Benchmark]
		public void Expr()
		{
			if (newFunc == null)
			{
				Delegate d = Test;
				newFunc = ScriptUtils.ConvertDelegate<Func<object, int>>(d);
			}
			int n = newFunc("hello");
			//Console.WriteLine(n);
			if (n != 5)
			{
				throw new Exception();
			}
		}

		[Benchmark]
		public void Expr2()
		{
			Delegate d = Test;
			var f = ScriptUtils.ConvertDelegate<Func<object, int>>(d);
			int n = f("hello");
			//Console.WriteLine(n);
			if (n != 5)
			{
				throw new Exception();
			}
		}
	}
}
