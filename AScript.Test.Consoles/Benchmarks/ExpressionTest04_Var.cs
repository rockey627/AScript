using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest04_Var
	{
		private static readonly string s = "100 * (v + 5) * (6-2)";
		private static readonly int r = 4000;

		private static AScript.Script script2 = new AScript.Script();
		//private static DynamicMethod dm3 = new DynamicMethod(string.Empty, typeof(int), new Type[0]);

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			script.Context.SetVar("v", 5);
			var result = (int)script.Eval(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2()
		{
			script2.Context.SetVar("v", 5);
			var result = (int)script2.Eval(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void Lambda()
		{
			// 100 * (v + 5) * (6-2)
			// v
			var v = System.Linq.Expressions.Expression.Parameter(typeof(int), "v");
			// v = 5
			var v5 = System.Linq.Expressions.Expression.Assign(v, System.Linq.Expressions.Expression.Constant(5));
			// v + 5
			var v_5 = System.Linq.Expressions.Expression.Add(v, System.Linq.Expressions.Expression.Constant(5));
			// 6 - 2
			var m62 = System.Linq.Expressions.Expression.Add(System.Linq.Expressions.Expression.Constant(6), System.Linq.Expressions.Expression.Constant(-2));
			// 100 * (v + 5)
			var multi100_v_5 = System.Linq.Expressions.Expression.Multiply(System.Linq.Expressions.Expression.Constant(100), v_5);
			var c = System.Linq.Expressions.Expression.Multiply(multi100_v_5, m62);
			var block = System.Linq.Expressions.Expression.Block(typeof(int), new[] { v }, v5, c);
			var func = System.Linq.Expressions.Expression.Lambda<Func<int>>(block).Compile();
			var result = func();
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void Emit1()
		{
			// 100 * (v + 5) * (6-2)
			DynamicMethod dm = new DynamicMethod(string.Empty, typeof(int), new Type[0]);
			ILGenerator il = dm.GetILGenerator();
			LocalBuilder v = il.DeclareLocal(typeof(int));
			il.Emit(OpCodes.Ldc_I4, int.Parse("100"));
			il.Emit(OpCodes.Ldc_I4, 5);
			il.Emit(OpCodes.Stloc, v);
			il.Emit(OpCodes.Ldloc, v);
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

		[Benchmark]
		public void Emit2()
		{
			// 100 * (v + 5) * (6-2)
			DynamicMethod dm = new DynamicMethod(string.Empty, typeof(int), new Type[0]);
			ILGenerator il = dm.GetILGenerator();
			LocalBuilder v = il.DeclareLocal(typeof(int));
			il.Emit(OpCodes.Ldc_I4, 100);
			il.Emit(OpCodes.Ldc_I4, 5);
			il.Emit(OpCodes.Stloc, v);
			il.Emit(OpCodes.Ldloc, v);
			il.Emit(OpCodes.Ldc_I4, 5);
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Mul);
			il.Emit(OpCodes.Ldc_I4, 6);
			il.Emit(OpCodes.Ldc_I4, -2);
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
		//public void Emit3()
		//{
		//	// 100 * (v + 5) * (6-2)
		//	ILGenerator il = dm3.GetILGenerator();
		//	LocalBuilder v = il.DeclareLocal(typeof(int));
		//	il.Emit(OpCodes.Ldc_I4, 100);
		//	il.Emit(OpCodes.Ldc_I4, 5);
		//	il.Emit(OpCodes.Stloc, v);
		//	il.Emit(OpCodes.Ldloc, v);
		//	il.Emit(OpCodes.Ldc_I4, 5);
		//	il.Emit(OpCodes.Add);
		//	il.Emit(OpCodes.Mul);
		//	il.Emit(OpCodes.Ldc_I4, 6);
		//	il.Emit(OpCodes.Ldc_I4, -2);
		//	il.Emit(OpCodes.Add);
		//	il.Emit(OpCodes.Mul);
		//	il.Emit(OpCodes.Ret);
		//	var f = (Func<int>)dm3.CreateDelegate(typeof(Func<int>));
		//	var result = f();
		//	if (result != r)
		//	{
		//		throw new Exception("result error");
		//	}
		//}

		//[Benchmark]
		//public void DynamicExpresso()
		//{
		//	var interpreter = new DynamicExpresso.Interpreter();
		//	interpreter.SetVariable("v", 5);
		//	var result = interpreter.Eval<int>(s);
		//	if (result != r)
		//	{
		//		throw new Exception("result error");
		//	}
		//}

	}
}
