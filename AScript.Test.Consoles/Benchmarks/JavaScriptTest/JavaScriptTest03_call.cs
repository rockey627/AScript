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
	public class JavaScriptTest03_call
	{
		private static readonly string s = "sum(5, 6)";
		private static readonly int r = 11;

		static JavaScriptTest03_call()
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

		private static Script _Script3;

		[Benchmark]
		public void AScript3_Context()
		{
			if (_Script3 == null)
			{
				_Script3 = new Script();
				_Script3.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			}
			var result = _Script3.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		private static Jurassic.ScriptEngine _JurassicEngine3;

		[Benchmark]
		public void Jurassic3()
		{
			if (_JurassicEngine3 == null)
			{
				_JurassicEngine3 = new Jurassic.ScriptEngine();
				_JurassicEngine3.SetGlobalFunction("sum", new Func<int, int, int>((a, b) => a + b));
			}
			var result = _JurassicEngine3.Evaluate<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript4_Cache()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

	}
}
