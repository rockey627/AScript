using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest02
	{
		private static readonly string s = "100 * (5 + 5) * (6-2)";
		private static readonly int r = 100 * (5 + 5) * (6 - 2);

		[Benchmark]
		public void AScript1()
		{
			//AScript.Script.UseCache = false;
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
		public void AScript2_UseCache()
		{
			var script = new AScript.Script();
			var result = script.Eval<int>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void Lambda()
		{
			// 100 * (5 + 5) * (6-2)
			// 5 + 5
			var v_5 = System.Linq.Expressions.Expression.Add(System.Linq.Expressions.Expression.Constant(5), System.Linq.Expressions.Expression.Constant(5));
			// 6 - 2
			var m62 = System.Linq.Expressions.Expression.Add(System.Linq.Expressions.Expression.Constant(6), System.Linq.Expressions.Expression.Constant(-2));
			// 100 * (5 + 5)
			var multi100_v_5 = System.Linq.Expressions.Expression.Multiply(System.Linq.Expressions.Expression.Constant(100), v_5);
			var c = System.Linq.Expressions.Expression.Multiply(multi100_v_5, m62);
			var func = System.Linq.Expressions.Expression.Lambda<Func<int>>(c).Compile();
			var result = func();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void Emit()
		{
			// 100 * (5 + 5) * (6-2)
			DynamicMethod dm = new DynamicMethod(string.Empty, typeof(int), new Type[0]);
			ILGenerator il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldc_I4, int.Parse("100"));
			il.Emit(OpCodes.Ldc_I4, int.Parse("5"));
			il.Emit(OpCodes.Ldc_I4, int.Parse("5"));
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Mul);
			il.Emit(OpCodes.Ldc_I4, int.Parse("6"));
			il.Emit(OpCodes.Ldc_I4, -int.Parse("2"));
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Mul);
			il.Emit(OpCodes.Ret);
			var f = (Func<int>)dm.CreateDelegate(typeof(Func<int>));
			var result = f();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		//[Benchmark]
		//public void DynamicExpresso()
		//{
		//	var interpreter = new DynamicExpresso.Interpreter();
		//	var result = interpreter.Eval<int>(s);
		//	if (result != r)
		//	{
		//		throw new Exception("result error");
		//	}
		//}
	}
}
