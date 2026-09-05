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
	public class ZExpressionsTest06_recursive
	{
		private static readonly string s1 = @"
int fib(int n) {
	if (n <= 1) return n;
	return fib(n - 1) + fib(n - 2);
}
fib(10)
";
//		private static readonly string s2 = @"
//public int fib(int n) {
//	if (n <= 1) return n;
//	return fib(n - 1) + fib(n - 2);
//}
//fib(10)
//";
		private static readonly int r = fib(10); // 55

		private static int fib(int n)
		{
			if (n <= 1) return n;
			return fib(n - 1) + fib(n - 2);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<int>(s1);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			// Z.Expressions.Eval不回写临时变量到上下文，这里也关闭变量回写功能，保持功能一致
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			var result = script.Eval<int>(s1, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		//// 不支持递归
		//[Benchmark]
		//public void ZExpressions2()
		//{
		//	Z.Expressions.EvalManager.DefaultContext.UseCache = false;
		//	var result = Z.Expressions.Eval.Execute<int>(s2);
		//	if (result != r) throw new Exception("result error");
		//}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			// Z.Expressions.Eval不回写临时变量到上下文，这里也关闭变量回写功能，保持功能一致
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			var result = script.Eval<int>(s1, -1);
			if (result != r) throw new Exception("result error");
		}

		//// 不支持递归
		//[Benchmark]
		//public void ZExpressions3()
		//{
		//	Z.Expressions.EvalManager.DefaultContext.UseCache = true;
		//	var result = Z.Expressions.Eval.Execute<int>(s2);
		//	if (result != r) throw new Exception("result error");
		//}

	}
}
