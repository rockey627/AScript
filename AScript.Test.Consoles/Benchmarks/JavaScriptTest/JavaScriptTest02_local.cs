using AScript.Lang.JavaScript;
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
	public class JavaScriptTest02_local
	{
		private static readonly string s = "var a = 100; var b = 5; var c = 6; a * (b + 5) * (c-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		static JavaScriptTest02_local()
		{
			Script.Langs.Set("js", AScript.Lang.JavaScript.JavaScriptLang.Instance, true);
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
		//public void AScript2_Compile2()
		//{
		//	var script = new Script();
		//	// 关闭变量/函数回写上下文功能（脚本中定义的变量和函数不用回写到上下文）
		//	script.Options.RewriteVariables = false;
		//	script.Options.RewriteFunctions = false;
		//	var result = script.Eval<int>(s, ECompileMode.All);
		//	if (result != r) throw new Exception("result error");
		//}

		[Benchmark]
		public void Jurassic2()
		{
			var engine = new Jurassic.ScriptEngine();
			var result = engine.Evaluate<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

	}
}
