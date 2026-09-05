using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.ZExpressionsTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ZExpressionsTest09_for
	{
		private static readonly string s = @"
var n = 0;
for (var i = 0; i < 10000; i++) {
	n += i;
}
n
";
		private static readonly int r = 9999 * 5000;

		// 解释执行
		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		// 只编译循环语句
		[Benchmark]
		public void AScript1_Loop()
		{
			var script = new Script();
			// Z.Expressions.Eval不回写临时变量到上下文，这里也关闭变量回写功能，保持功能一致
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			var result = script.Eval<int>(s, ECompileMode.Loop);
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
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void ZExpressions2()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = false;
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
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void ZExpressions3()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = true;
			var result = Z.Expressions.Eval.Execute<int>(s);
			if (result != r) throw new Exception("result error");
		}

	}
}
