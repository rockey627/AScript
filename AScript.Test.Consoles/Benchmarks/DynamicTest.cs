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
		[Benchmark]
		public void Dynamic_GetProperty()
		{
			dynamic t = new TestClass();
			object name = t.Name;
		}

		[Benchmark]
		public void Reflect_GetProperty()
		{
			object t = new TestClass();
			var propertyInfo = typeof(TestClass).GetProperty("Name");
			object name = propertyInfo.GetValue(t);
		}

		[Benchmark]
		public void Dynamic_SetProperty()
		{
			dynamic t = new TestClass();
			t.Name = "hi";
		}

		[Benchmark]
		public void Reflect_SetProperty()
		{
			object t = new TestClass();
			var propertyInfo = typeof(TestClass).GetProperty("Name");
			propertyInfo.SetValue(t, "hi");
		}

		[Benchmark]
		public void Dynamic_Method()
		{
			dynamic t = new TestClass();
			t.Hello("jim");
		}

		[Benchmark]
		public void Reflect_Method()
		{
			object t = new TestClass();
			var methodInfo = typeof(TestClass).GetMethod("Hello", new Type[] { typeof(string) });
			methodInfo.Invoke(t, new object[] { "jim" });
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
