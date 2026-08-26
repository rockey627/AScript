using AScript.Lang.JavaScript;
using BenchmarkDotNet.Attributes;
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
	public class JavaScriptTest09_array_filter
	{
		private static readonly string s = "var arr = [1, 2, 3, 4, 5].filter(x => x % 2 == 0); arr.length";
		private static readonly int r = 2;

		static JavaScriptTest09_array_filter()
		{
			Script.Langs.Set("js", JavaScriptLang.Instance, setDefault: true);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Jint1()
		{
			var engine = new Jint.Engine();
			var result = engine.Evaluate(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		//[Benchmark]
		//public void Jurassic2()
		//{
		//	var engine = new Jurassic.ScriptEngine();
		//	var result = engine.Evaluate<int>(s);
		//	if (result != r) throw new Exception("result error");
		//}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

	}
}
