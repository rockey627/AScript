using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class DynamicTest
	{
		private static readonly TestClass _instanct = new TestClass();

		//[Benchmark]
		//public void Dynamic_GetProperty()
		//{
		//	dynamic t = _instanct;
		//	object name = t.Name;
		//}

		//[Benchmark]
		//public void Reflect_GetProperty()
		//{
		//	var propertyInfo = typeof(TestClass).GetProperty("Name");
		//	object name = propertyInfo.GetValue(_instanct);
		//}

		//[Benchmark]
		//public void Dynamic_SetProperty()
		//{
		//	dynamic t = _instanct;
		//	t.Name = "hi";
		//}

		//[Benchmark]
		//public void Reflect_SetProperty()
		//{
		//	var propertyInfo = typeof(TestClass).GetProperty("Name");
		//	propertyInfo.SetValue(_instanct, "hi");
		//}

		[Benchmark]
		public void Dynamic_Method()
		{
			dynamic t = _instanct;
			t.Hello("jim");
		}

		[Benchmark]
		public void Dynamic_Method2()
		{
			var m = _instanct.Hello;
			dynamic d = m;
			d("jim");
		}

		[Benchmark]
		public void Reflect_Method()
		{
			var methodInfo = typeof(TestClass).GetMethod("Hello", new Type[] { typeof(string) });
			methodInfo.Invoke(_instanct, new object[] { "jim" });
		}

		[Benchmark]
		public void Delegate_Method()
		{
			Delegate del = _instanct.Hello;
			del.DynamicInvoke("jim");
		}

		public class TestClass
		{
			public string Name { get; set; }

			public void Hello(string name)
			{
				this.Name = name;
			}
		}
	}
}
