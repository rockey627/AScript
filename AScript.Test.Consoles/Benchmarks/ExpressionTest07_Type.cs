using System;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest07_Type
	{
		private static readonly string s = "var user=new User('张三',10);100 * user.Age * (6-2)";
		private static readonly int r = 100 * 10 * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			script.Context.AddType<User>();
			var result = (int)script.Eval(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache()
		{
			var script = new AScript.Script();
			script.Context.AddType<User>();
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache()
		{
			var script = new AScript.Script();
			script.Context.AddType<User>();
			var result = script.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		public class User
		{
			public string Name { get; set; }
			public int Age { get; set; }

			public User(string name, int age)
			{
				this.Name = name;
				this.Age = age;
			}
		}
	}
}
