using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.JavaScriptTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class JavaScriptTest04_call
	{
		private static readonly string s = "sum(5, 6)";
		private static readonly int r = 11;

		static JavaScriptTest04_call()
		{
			Script.Langs.Set("js", AScript.Lang.JavaScript.JavaScriptLang.Instance, true);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Jint1()
		{
			var engine = new Jint.Engine();
			engine.SetValue("sum", new Func<int, int, int>((a, b) => a + b));
			var result = engine.Evaluate(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Jurassic2()
		{
			var engine = new Jurassic.ScriptEngine();
			engine.SetGlobalFunction("sum", new Func<int, int, int>((a, b) => a + b));
			var result = engine.Evaluate<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

	}
}
