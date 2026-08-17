using BenchmarkDotNet.Attributes;
using Iced.Intel;
using System;
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
		public void AScript2_NoCache()
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

		private static Flee.PublicTypes.ExpressionContext _LeeContext3;

		[Benchmark]
		public void Lee3()
		{
			if (_LeeContext3 == null)
			{
				_LeeContext3 = new Flee.PublicTypes.ExpressionContext();
			}
			var d = _LeeContext3.CompileDynamic(s);
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
				_func4 = script.Compile<int>(s);
			}
			var result = _func4();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		private static Flee.PublicTypes.IDynamicExpression _LeeExpr4 = _LeeContext3.CompileDynamic(s);

		[Benchmark]
		public void Lee4()
		{
			if (_LeeExpr4 == null)
			{
				var context = new Flee.PublicTypes.ExpressionContext();
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
