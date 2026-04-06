using System;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class ExpressionTest08_For
	{
		private static readonly string s_z = "int a=0; for(int i=0;i<1000;i=i+1){a=a+i;};a";
		private static readonly int r;

		static ExpressionTest08_For()
		{
			int a = 0; for (int i = 0; i < 1000; i = i + 1) { a = a + i; }
			r = a;
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(s_z);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(s_z, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache2()
		{
			var script = new AScript.Script();
			script.Options.RewriteVariables = false;
			var result = (int)script.Eval(s_z, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache()
		{
			var script = new AScript.Script();
			var result = (int)script.Eval(s_z, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache2()
		{
			var script = new AScript.Script();
			script.Options.RewriteVariables = false;
			var result = (int)script.Eval(s_z, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		//[Benchmark]
		//public void Emit()
		//{
		//	// 100 * (5 + 5) * (6-2)
		//	DynamicMethod dm = new DynamicMethod(string.Empty, typeof(int), new Type[0]);
		//	ILGenerator il = dm.GetILGenerator();
		//	il.Emit(OpCodes.Ldc_I4, int.Parse("100"));
		//	il.Emit(OpCodes.Ldc_I4, int.Parse("5"));
		//	il.Emit(OpCodes.Ldc_I4, int.Parse("5"));
		//	il.Emit(OpCodes.Add);
		//	il.Emit(OpCodes.Mul);
		//	il.Emit(OpCodes.Ldc_I4, int.Parse("6"));
		//	il.Emit(OpCodes.Ldc_I4, -int.Parse("2"));
		//	il.Emit(OpCodes.Add);
		//	il.Emit(OpCodes.Mul);
		//	il.Emit(OpCodes.Ret);
		//	var f = (Func<int>)dm.CreateDelegate(typeof(Func<int>));
		//	var result = f();
		//	if (result != r)
		//	{
		//		throw new Exception("result error");
		//	}
		//}

		//public Func<AScript.ScriptContext, object> Compile()
		//{
		//	DynamicMethod dm = new DynamicMethod(string.Empty, typeof(object), new Type[0]);
		//	ILGenerator il = dm.GetILGenerator();
		//	il.Emit(OpCodes.Ldc_I4, int.Parse("100"));
		//	il.Emit(OpCodes.Ldc_I4, int.Parse("5"));
		//	il.Emit(OpCodes.Ldc_I4, int.Parse("5"));
		//	il.Emit(OpCodes.Add);
		//	il.Emit(OpCodes.Mul);
		//	il.Emit(OpCodes.Ldc_I4, int.Parse("6"));
		//	il.Emit(OpCodes.Ldc_I4, -int.Parse("2"));
		//	il.Emit(OpCodes.Add);
		//	il.Emit(OpCodes.Mul);
		//	il.Emit(OpCodes.Ret);
		//	return (Func<AScript.ScriptContext, object>)dm.CreateDelegate(typeof(Func<AScript.ScriptContext, object>));
		//}

		// 不支持多语句
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
