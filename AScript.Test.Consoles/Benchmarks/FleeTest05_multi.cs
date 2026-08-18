using BenchmarkDotNet.Attributes;
using Iced.Intel;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class FleeTest05_multi
	{
		private static readonly string s = "int m = a * (b + 5) * (c-2); int n = b + a / 10 -c; m + n";
		private static readonly int r = 100 * (5 + 5) * (6 - 2) + 5 + 100 / 10 - 6;

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = (int)script.Eval(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new AScript.Script();
			// 不回写脚本中的临时变量
			script.Options.RewriteVariables = false;
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = script.Eval<int>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void Flee2()
		{
			var context = new Flee.PublicTypes.ExpressionContext();
			context.Variables["a"] = 100;
			context.Variables["b"] = 5;
			context.Variables["c"] = 6;
			var engine = new Flee.CalcEngine.PublicTypes.CalculationEngine();
			engine.Add("m", "a * (b + 5) * (c-2)", context);
			engine.Add("n", "b + a / 10 -c", context);
			engine.Add("result", "m + n", context);
			int result = engine.GetResult<int>("result");
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Script _Script3;

		[Benchmark]
		public void AScript3_Context()
		{
			if (_Script3 == null)
			{
				_Script3 = new AScript.Script();
				// 不回写脚本中的临时变量
				_Script3.Options.RewriteVariables = false;
				_Script3.Context.SetVar("a", 100);
				_Script3.Context.SetVar("b", 5);
				_Script3.Context.SetVar("c", 6);
			}
			var result = _Script3.Eval<int>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.ExpressionContext _FleeContext3;

		[Benchmark]
		public void Flee3()
		{
			if (_FleeContext3 == null)
			{
				_FleeContext3 = new Flee.PublicTypes.ExpressionContext();
				_FleeContext3.Variables["a"] = 100;
				_FleeContext3.Variables["b"] = 5;
				_FleeContext3.Variables["c"] = 6;
			}
			var engine = new Flee.CalcEngine.PublicTypes.CalculationEngine();
			engine.Add("m", "a * (b + 5) * (c-2)", _FleeContext3);
			engine.Add("n", "b + a / 10 -c", _FleeContext3);
			engine.Add("result", "m + n", _FleeContext3);
			int result = engine.GetResult<int>("result");
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Script _Script4;

		[Benchmark]
		public void AScript4_Cache()
		{
			if (_Script4 == null)
			{
				_Script4 = new AScript.Script();
				_Script4.Context.SetVar("a", 100);
				_Script4.Context.SetVar("b", 5);
				_Script4.Context.SetVar("c", 6);
			}
			var result = _Script4.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

	}
}
