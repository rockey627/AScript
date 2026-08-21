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
	public class JavaScriptTest04_file
	{
		private static readonly string filePath = "./Benchmarks/JavaScriptTest/utils.js";
		private static readonly string s = "sum(factorial(5), 10)";
		private static readonly int r = 5 * 4 * 3 * 2 * 1 + 10;

		static JavaScriptTest04_file()
		{
			Script.Langs.Set("js", AScript.Lang.JavaScript.JavaScriptLang.Instance, true);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			script.EvalFile(filePath);
			var result = script.Eval<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			script.EvalFile(filePath);
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile2()
		{
			var script = new Script();
			// 关闭变量/函数回写上下文功能（脚本中定义的变量和函数不用回写到上下文）
			script.Options.RewriteVariables = false;
			script.Options.RewriteFunctions = false;
			script.EvalFile(filePath);
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void Jurassic2()
		{
			var engine = new Jurassic.ScriptEngine();
			engine.ExecuteFile(filePath);
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
			}
			_Script3.EvalFile(filePath);
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
			}
			_JurassicEngine3.ExecuteFile(filePath);
			var result = _JurassicEngine3.Evaluate<int>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript4_Cache()
		{
			var script = new Script();
			script.EvalFile(filePath, -1);
			var result = script.Eval<int>(s, -1);
			if (result != r) throw new Exception("result error");
		}

	}
}
