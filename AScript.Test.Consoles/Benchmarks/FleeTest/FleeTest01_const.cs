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
	public class FleeTest01_const
	{
		private static readonly string s = "100 * (5 + 5) * (6-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<int>(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
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
			}
			var result = _Script3.Eval<int>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.ExpressionContext _LeeContext3;

		[Benchmark]
		public void Flee3_Context()
		{
			if (_LeeContext3 == null)
			{
				_LeeContext3 = new Flee.PublicTypes.ExpressionContext();
			}
			var d = _LeeContext3.CompileGeneric<int>(s);
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
			}
			var result = _Script4.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static readonly ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<int>> _LeeExpr4Dict = new ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<int>>();

		[Benchmark]
		public void Flee4()
		{
			string key = s;
			if (!_LeeExpr4Dict.TryGetValue(key, out var d))
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				d = context.CompileGeneric<int>(s);
				_LeeExpr4Dict[key] = d;
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
				_func5 = script.Compile<int>(s);
			}
			var result = _func5();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.IGenericExpression<int> _LeeExpr5;

		[Benchmark]
		public void Flee5()
		{
			if (_LeeExpr5 == null)
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				_LeeExpr5 = context.CompileGeneric<int>(s);
			}
			var result = _LeeExpr5.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

	}
}
