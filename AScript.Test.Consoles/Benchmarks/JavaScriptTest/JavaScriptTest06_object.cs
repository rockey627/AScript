using BenchmarkDotNet.Attributes;
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
	public class JavaScriptTest06_object
	{
		private static readonly string s = @"
var obj = { name: 'Alice', address: { city: 'Beijing' } };
obj.address.city
";
		private static readonly string r = "Beijing";

		static JavaScriptTest06_object()
		{
			Script.Langs.Set("js", AScript.Lang.JavaScript.JavaScriptLang.Instance, true);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<string>(s);
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
			var result = script.Eval<string>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Jurassic2()
		{
			var engine = new Jurassic.ScriptEngine();
			var result = engine.Evaluate<string>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			var result = script.Eval<string>(s, -1);
			if (result != r) throw new Exception("result error");
		}

	}
}
