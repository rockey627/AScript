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
	public class ZExpressionsTest07_linq
	{
		private static readonly List<int> _list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		private static readonly string s = "list.Where(a => a % 2 == 0).ToList().Count";
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
			// Z.Expressions.Eval不回写临时变量到上下文，这里也关闭变量回写功能，保持功能一致
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Context.SetVar("list", _list);
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void ZExpressions2()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = false;
			var result = Z.Expressions.Eval.Execute<int>(s, new { list = _list });
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			// Z.Expressions.Eval不回写临时变量到上下文，这里也关闭变量回写功能，保持功能一致
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Context.SetVar("list", _list);
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void ZExpressions3()
		{
			Z.Expressions.EvalManager.DefaultContext.UseCache = true;
			var result = Z.Expressions.Eval.Execute<int>(s, new { list = _list });
			if (result != r) throw new Exception("result error");
		}

	}
}
