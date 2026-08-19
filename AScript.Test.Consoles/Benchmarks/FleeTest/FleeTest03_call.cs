using BenchmarkDotNet.Attributes;
using Iced.Intel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.FleeTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class FleeTest03_call
	{
		private static readonly string s = "a * Sum(b, 5) * (c-2)";
		private static readonly int r = 100 * MyFunctions.Sum(5, 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			script.Context.AddFunc(typeof(MyFunctions));
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
			var script = new Script();
			script.Context.AddFunc(typeof(MyFunctions));
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
			context.Imports.AddType(typeof(MyFunctions));
			context.Variables["a"] = 100;
			context.Variables["b"] = 5;
			context.Variables["c"] = 6;
			var d = context.CompileGeneric<int>(s);
			var result = d.Evaluate();
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
				_Script3 = new Script();
				_Script3.Context.AddFunc(typeof(MyFunctions));
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
				_FleeContext3.Imports.AddType(typeof(MyFunctions));
				_FleeContext3.Variables["a"] = 100;
				_FleeContext3.Variables["b"] = 5;
				_FleeContext3.Variables["c"] = 6;
			}
			var d = _FleeContext3.CompileGeneric<int>(s);
			var result = d.Evaluate();
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
				_Script4 = new Script();
				_Script4.Context.AddFunc(typeof(MyFunctions));
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

		private static readonly ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<int>> _FleeExpr4Dict = new ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<int>>();

		[Benchmark]
		public void Flee4()
		{
			string key = s;
			if (!_FleeExpr4Dict.TryGetValue(key, out var d))
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				context.Imports.AddType(typeof(MyFunctions));
				context.Variables["a"] = 100;
				context.Variables["b"] = 5;
				context.Variables["c"] = 6;
				d = context.CompileGeneric<int>(s);
				_FleeExpr4Dict[key] = d;
			}
			var result = d.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Func<int> _func5;

		[Benchmark]
		public void AScript5()
		{
			if (_func5 == null)
			{
				var script = new Script();
				script.Context.AddFunc(typeof(MyFunctions));
				script.Context.SetVar("a", 100);
				script.Context.SetVar("b", 5);
				script.Context.SetVar("c", 6);
				_func5 = script.Compile<int>(s);
			}
			var result = _func5();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.IGenericExpression<int> _FleeExpr5;

		[Benchmark]
		public void Flee5()
		{
			if (_FleeExpr5 == null)
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				context.Imports.AddType(typeof(MyFunctions));
				context.Variables["a"] = 100;
				context.Variables["b"] = 5;
				context.Variables["c"] = 6;
				_FleeExpr5 = context.CompileGeneric<int>(s);
			}
			var result = _FleeExpr5.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		public class MyFunctions
		{
			public static int Sum(int a, int b)
			{
				return a + b;
			}
		}
	}
}
