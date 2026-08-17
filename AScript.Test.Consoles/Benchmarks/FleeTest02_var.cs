using BenchmarkDotNet.Attributes;
using Iced.Intel;
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
	public class FleeTest02_var
	{
		private static readonly string s = "a * (b + 5) * (c-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

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
		public void Lee2()
		{
			var context = new Flee.PublicTypes.ExpressionContext();
			context.Variables["a"] = 100;
			context.Variables["b"] = 5;
			context.Variables["c"] = 6;
			var d = context.CompileDynamic(s);
			var result = (int)d.Evaluate();
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
				_Script2.Context.SetVar("a", 100);
				_Script2.Context.SetVar("b", 5);
				_Script2.Context.SetVar("c", 6);
			}
			var result = _Script2.Eval<int>(s, ECompileMode.All);
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
				_LeeContext2_2.Variables["a"] = 100;
				_LeeContext2_2.Variables["b"] = 5;
				_LeeContext2_2.Variables["c"] = 6;
			}
			var d = _LeeContext2_2.CompileDynamic(s);
			var result = (int)d.Evaluate();
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
				_Script3.Context.SetVar("a", 100);
				_Script3.Context.SetVar("b", 5);
				_Script3.Context.SetVar("c", 6);
			}
			var result = _Script3.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static readonly ConcurrentDictionary<string, Flee.PublicTypes.IDynamicExpression> _LeeExpr3Dict = new ConcurrentDictionary<string, Flee.PublicTypes.IDynamicExpression>();

		[Benchmark]
		public void Lee3()
		{
			if (!_LeeExpr3Dict.TryGetValue(s, out var d))
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				context.Variables["a"] = 100;
				context.Variables["b"] = 5;
				context.Variables["c"] = 6;
				d = context.CompileDynamic(s);
				_LeeExpr3Dict[s] = d;
			}
			var result = (int)d.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Func<int> _func4;

		[Benchmark]
		public void AScript4()
		{
			if (_func4 == null)
			{
				var script = new AScript.Script();
				script.Context.SetVar("a", 100);
				script.Context.SetVar("b", 5);
				script.Context.SetVar("c", 6);
				_func4 = script.Compile<int>(s);
			}
			var result = _func4();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.IDynamicExpression _LeeExpr4;

		[Benchmark]
		public void Lee4()
		{
			if (_LeeExpr4 == null)
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				context.Variables["a"] = 100;
				context.Variables["b"] = 5;
				context.Variables["c"] = 6;
				_LeeExpr4 = context.CompileDynamic(s);
			}
			var result = (int)_LeeExpr4.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

	}
}
