using AScript.Values;
using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class AValueTest01
	{
		private static object Add(object v1, object v2)
		{
			return (dynamic)v1 + (dynamic)v2;
		}

		private static object Add2(object v1, object v2)
		{
			// is 影响性能
			if (v1 is AValue aValue1 && v2 is AValue aValue2)
			{
				return aValue1 + aValue2;
			}
			return (dynamic)v1 + (dynamic)v2;
		}

		// 性能第3：10.172ns
		[Benchmark]
		public void BoxTest1()
		{
			var result = Add(1, 3);
		}

		// 性能第5：11.943ns
		[Benchmark]
		public void AValueTest1()
		{
			var result = Add(AValue.Create(1), AValue.Create(3));
		}

		// 性能第4：11.492ns
		[Benchmark]
		public void BoxTest2()
		{
			var result = Add2(1, 3);
		}

		// 性能第7：19.861ns
		[Benchmark]
		public void AValueTest2()
		{
			var result = Add2(AValue.Create(1), AValue.Create(3));
		}

		// 性能第1：7.84ns
		[Benchmark]
		public void AValueTest3()
		{
			var result = AValue.Create(1) + AValue.Create(3);
		}

		// 性能第2：8.125ns
		[Benchmark]
		public void AValueTest4()
		{
			var v1 = AValue.Create(1);
			var v2 = AValue.Create(3);
			var result = v1 + v2;
		}

		// 性能第6：15.825ns
		[Benchmark]
		public void AValueTest5()
		{
			AValue v1 = AValue.Create(1);
			AValue v2 = AValue.Create(3);
			var result = v1 + v2;
		}
	}
}
