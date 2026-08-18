using BenchmarkDotNet.Attributes;
using Iced.Intel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AScript.Test.Consoles.Benchmarks.FleeTest03_call;

namespace AScript.Test.Consoles.Benchmarks
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
			var script = new AScript.Script();
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
			}
			var d = _LeeContext2_2.CompileGeneric<int>(s);
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
			}
			var result = _Script3.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static readonly ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<int>> _LeeExpr3Dict = new ConcurrentDictionary<string, Flee.PublicTypes.IGenericExpression<int>>();

		[Benchmark]
		public void Lee3()
		{
			if (!_LeeExpr3Dict.TryGetValue(s, out var d))
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				d = context.CompileGeneric<int>(s);
				_LeeExpr3Dict[s] = d;
			}
			var result = d.Evaluate();
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
				_func4 = script.Compile<int>(s);
			}
			var result = _func4();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.IGenericExpression<int> _LeeExpr4;

		[Benchmark]
		public void Lee4()
		{
			if (_LeeExpr4 == null)
			{
				var context = new Flee.PublicTypes.ExpressionContext();
				_LeeExpr4 = context.CompileGeneric<int>(s);
			}
			var result = _LeeExpr4.Evaluate();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

	}
}
