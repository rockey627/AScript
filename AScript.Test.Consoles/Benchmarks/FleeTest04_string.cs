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
	public class FleeTest04_string
	{
		private static readonly string s = "a + b + c";
		private static readonly string r = "helloeveryone";

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			script.Context.SetVar("a", "hello");
			script.Context.SetVar("b", "every");
			script.Context.SetVar("c", "one");
			var result = script.Eval<string>(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new AScript.Script();
			script.Context.SetVar("a", "hello");
			script.Context.SetVar("b", "every");
			script.Context.SetVar("c", "one");
			var result = script.Eval<string>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void Lee2()
		{
			var context = new Flee.PublicTypes.ExpressionContext();
			context.Variables["a"] = "hello";
			context.Variables["b"] = "every";
			context.Variables["c"] = "one";
			var d = context.CompileGeneric<string>(s);
			var result = d.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Script _Script2;

		[Benchmark]
		public void AScript2_Compile2()
		{
			if (_Script2 == null)
			{
				_Script2 = new AScript.Script();
				_Script2.Context.SetVar("a", "hello");
				_Script2.Context.SetVar("b", "every");
				_Script2.Context.SetVar("c", "one");
			}
			var result = _Script2.Eval<string>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.ExpressionContext _LeeContext2_2;

		[Benchmark]
		public void Lee2_2()
		{
			if (_LeeContext2_2 == null)
			{
				_LeeContext2_2 = new Flee.PublicTypes.ExpressionContext();
				_LeeContext2_2.Variables["a"] = "hello";
				_LeeContext2_2.Variables["b"] = "every";
				_LeeContext2_2.Variables["c"] = "one";
			}
			var d = _LeeContext2_2.CompileGeneric<string>(s);
			var result = d.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		//[Benchmark]
		//public void AScript3_UseCache()
		//{
		//	var script = new AScript.Script();
		//	script.Context.SetVar("a", 100);
		//	script.Context.SetVar("b", 5);
		//	script.Context.SetVar("c", 6);
		//	var result = script.Eval<int>(s, -1);
		//	if (result != r)
		//	{
		//		throw new Exception("result error");
		//	}
		//}

		private static Script _Script3;

		[Benchmark]
		public void AScript3_UseCache()
		{
			if (_Script3 == null)
			{
				_Script3 = new AScript.Script();
				_Script3.Context.SetVar("a", "hello");
				_Script3.Context.SetVar("b", "every");
				_Script3.Context.SetVar("c", "one");
			}
			var result = _Script3.Eval<string>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static readonly ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<string>> _LeeExpr3Dict = new ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<string>>();

		[Benchmark]
		public void Lee3()
		{
			if (!_LeeExpr3Dict.TryGetValue(s, out var d))
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				context.Variables["a"] = "hello";
				context.Variables["b"] = "every";
				context.Variables["c"] = "one";
				d = context.CompileGeneric<string>(s);
				_LeeExpr3Dict[s] = d;
			}
			var result = d.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Func<string> _func4;

		[Benchmark]
		public void AScript4()
		{
			if (_func4 == null)
			{
				var script = new AScript.Script();
				script.Context.SetVar("a", "hello");
				script.Context.SetVar("b", "every");
				script.Context.SetVar("c", "one");
				_func4 = script.Compile<string>(s);
			}
			var result = _func4();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.IGenericExpression<string> _LeeExpr4;

		[Benchmark]
		public void Lee4()
		{
			if (_LeeExpr4 == null)
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				context.Variables["a"] = "hello";
				context.Variables["b"] = "every";
				context.Variables["c"] = "one";
				_LeeExpr4 = context.CompileGeneric<string>(s);
			}
			var result = _LeeExpr4.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

	}
}
