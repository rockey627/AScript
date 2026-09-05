using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.ZExpressionsTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ZExpressionsTest04_call
	{
		private static readonly string s = "sum(5, 6)";
		private static readonly int r = 11;

		// 解释执行
		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			//script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			script.Context.AddFunc(typeof(MyMethod));
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		// 编译执行
		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			// Z.Expressions.Eval不回写临时变量到上下文，这里也关闭变量回写功能，保持功能一致
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			//script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			script.Context.AddFunc(typeof(MyMethod));
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void ZExpressions2()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = false;
			Z.Expressions.EvalManager.DefaultContext.RegisterStaticMethod(typeof(MyMethod));
			var result = Z.Expressions.Eval.Execute<int>(s);
			if (result != r) throw new Exception("result error");
		}

		// 编译缓存
		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			// Z.Expressions.Eval不回写临时变量到上下文，这里也关闭变量回写功能，保持功能一致
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			//script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			script.Context.AddFunc(typeof(MyMethod));
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void ZExpressions3()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = true;
			Z.Expressions.EvalManager.DefaultContext.RegisterStaticMethod(typeof(MyMethod));
			var result = Z.Expressions.Eval.Execute<int>(s);
			if (result != r) throw new Exception("result error");
		}

		private class MyMethod
		{
			public static int sum(int a, int b) => a + b;

			public string Hello(string name)
			{
				return $"hello {name}";
			}
		}

	}
}
