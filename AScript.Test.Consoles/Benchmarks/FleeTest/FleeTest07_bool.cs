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
	public class FleeTest07_bool
	{
		private static readonly string s_ascript = "100 > 50 || 5 < 10 && 6 != 6";
		private static readonly string s_flee = "100 > 50 or 5 < 10 and 6 <> 6";
		private static readonly bool r = 100 > 50 || 5 < 10 && 6 != 6;

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<bool>(s_ascript);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			var result = script.Eval<bool>(s_ascript, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void Flee2()
		{
			var context = new Flee.PublicTypes.ExpressionContext();
			var d = context.CompileGeneric<bool>(s_flee);
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
			var result = _Script3.Eval<bool>(s_ascript, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.ExpressionContext _LeeContext3;

		[Benchmark]
		public void Flee3()
		{
			if (_LeeContext3 == null)
			{
				_LeeContext3 = new Flee.PublicTypes.ExpressionContext();
			}
			var d = _LeeContext3.CompileGeneric<bool>(s_flee);
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
			var result = _Script4.Eval<bool>(s_ascript, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static readonly ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<bool>> _LeeExpr4Dict = new ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<bool>>();

		[Benchmark]
		public void Flee4()
		{
			string key = s_flee;
			if (!_LeeExpr4Dict.TryGetValue(key, out var d))
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				d = context.CompileGeneric<bool>(s_flee);
				_LeeExpr4Dict[key] = d;
			}
			var result = d.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Func<bool> _func5;

		[Benchmark]
		public void AScript5()
		{
			if (_func5 == null)
			{
				var script = new Script();
				_func5 = script.Compile<bool>(s_ascript);
			}
			var result = _func5();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.IGenericExpression<bool> _LeeExpr5;

		[Benchmark]
		public void Flee5()
		{
			if (_LeeExpr5 == null)
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				_LeeExpr5 = context.CompileGeneric<bool>(s_flee);
			}
			var result = _LeeExpr5.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

	}
}
