using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptMathTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["js"] = JavaScriptLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("js");
		}

		// Math.abs
		[TestMethod]
		public void Test01_abs()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(5.0, script.Eval("Math.abs(-5)"));
			Assert.AreEqual(5.0, script.Eval("Math.abs(5)"));
			Assert.AreEqual(0.0, script.Eval("Math.abs(0)"));
		}

		[TestMethod]
		public void Test01_abs_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(5.0, script.Eval("Math.abs(-5)"));
			Assert.AreEqual(5.0, script.Eval("Math.abs(5)"));
			Assert.AreEqual(0.0, script.Eval("Math.abs(0)"));
		}

		// Math.acos
		[TestMethod]
		public void Test02_acos()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.acos(0.5)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test02_acos_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.acos(0.5)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.acosh
		[TestMethod]
		public void Test03_acosh()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.acosh(2)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test03_acosh_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.acosh(2)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.asin
		[TestMethod]
		public void Test04_asin()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.asin(0.5)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test04_asin_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.asin(0.5)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.asinh
		[TestMethod]
		public void Test05_asinh()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.asinh(2)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test05_asinh_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.asinh(2)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.atan
		[TestMethod]
		public void Test06_atan()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.atan(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test06_atan_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.atan(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.atan2
		[TestMethod]
		public void Test07_atan2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.atan2(1, 1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test07_atan2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.atan2(1, 1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.atanh
		[TestMethod]
		public void Test08_atanh()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.atanh(0.5)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test08_atanh_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.atanh(0.5)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.cbrt
		[TestMethod]
		public void Test09_cbrt()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.cbrt(27)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test09_cbrt_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.cbrt(27)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.ceil
		[TestMethod]
		public void Test10_ceil()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(6.0, script.Eval("Math.ceil(5.1)"));
			Assert.AreEqual(5.0, script.Eval("Math.ceil(5)"));
		}

		[TestMethod]
		public void Test10_ceil_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(6.0, script.Eval("Math.ceil(5.1)"));
			Assert.AreEqual(5.0, script.Eval("Math.ceil(5)"));
		}

		// Math.clz32
		[TestMethod]
		public void Test11_clz32()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.clz32(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test11_clz32_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.clz32(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.cos
		[TestMethod]
		public void Test12_cos()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.cos(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test12_cos_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.cos(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.cosh
		[TestMethod]
		public void Test13_cosh()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.cosh(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test13_cosh_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.cosh(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.exp
		[TestMethod]
		public void Test14_exp()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.exp(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test14_exp_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.exp(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.expm1
		[TestMethod]
		public void Test15_expm1()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.expm1(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test15_expm1_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.expm1(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.floor
		[TestMethod]
		public void Test16_floor()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(5.0, script.Eval("Math.floor(5.9)"));
			Assert.AreEqual(5.0, script.Eval("Math.floor(5)"));
		}

		[TestMethod]
		public void Test16_floor_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(5.0, script.Eval("Math.floor(5.9)"));
			Assert.AreEqual(5.0, script.Eval("Math.floor(5)"));
		}

		// Math.fround
		[TestMethod]
		public void Test17_fround()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.fround(5.5)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test17_fround_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.fround(5.5)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.hypot
		[TestMethod]
		public void Test18_hypot()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.hypot(3, 4)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test18_hypot_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.hypot(3, 4)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.imul
		[TestMethod]
		public void Test19_imul()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.imul(2, 3)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(long));
		}

		[TestMethod]
		public void Test19_imul_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.imul(2, 3)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(long));
		}

		// Math.log
		[TestMethod]
		public void Test20_log()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.log(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test20_log_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.log(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.log10
		[TestMethod]
		public void Test21_log10()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.log10(100)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test21_log10_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.log10(100)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.log1p
		[TestMethod]
		public void Test22_log1p()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.log1p(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test22_log1p_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.log1p(1)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.log2
		[TestMethod]
		public void Test23_log2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.log2(8)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test23_log2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.log2(8)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.max
		[TestMethod]
		public void Test24_max()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10.0, script.Eval("Math.max(1, 10, 5)"));
			Assert.AreEqual(-1.0, script.Eval("Math.max(-5, -10, -1)"));
			Assert.AreEqual(5.0, script.Eval("Math.max(-5, -10, -1, 1, 2, 3, 4, 5)"));
		}

		[TestMethod]
		public void Test24_max_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10.0, script.Eval("Math.max(1, 10, 5)"));
			Assert.AreEqual(-1.0, script.Eval("Math.max(-5, -10, -1)"));
			Assert.AreEqual(5.0, script.Eval("Math.max(-5, -10, -1, 1, 2, 3, 4, 5)"));
		}

		// Math.min
		[TestMethod]
		public void Test25_min()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1.0, script.Eval("Math.min(1, 10, 5)"));
			Assert.AreEqual(-10.0, script.Eval("Math.min(-5, -10, -1)"));
			Assert.AreEqual(-10.0, script.Eval("Math.min(-5, -10, -1, 1, 2, 3, 4, 5)"));
		}

		[TestMethod]
		public void Test25_min_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1.0, script.Eval("Math.min(1, 10, 5)"));
			Assert.AreEqual(-10.0, script.Eval("Math.min(-5, -10, -1)"));
			Assert.AreEqual(-10.0, script.Eval("Math.min(-5, -10, -1, 1, 2, 3, 4, 5)"));
		}

		// Math.pow
		[TestMethod]
		public void Test26_pow()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(8.0, script.Eval("Math.pow(2, 3)"));
			Assert.AreEqual(1.0, script.Eval("Math.pow(5, 0)"));
		}

		[TestMethod]
		public void Test26_pow_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(8.0, script.Eval("Math.pow(2, 3)"));
			Assert.AreEqual(1.0, script.Eval("Math.pow(5, 0)"));
		}

		// Math.random
		[TestMethod]
		public void Test27_random()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.random()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
			var value = (double)result;
			Assert.IsTrue(value >= 0 && value < 1);
		}

		[TestMethod]
		public void Test27_random_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.random()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
			var value = (double)result;
			Assert.IsTrue(value >= 0 && value < 1);
		}

		// Math.round
		[TestMethod]
		public void Test28_round()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(6.0, script.Eval("Math.round(5.5)"));
			Assert.AreEqual(5.0, script.Eval("Math.round(5.4)"));
		}

		[TestMethod]
		public void Test28_round_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(6.0, script.Eval("Math.round(5.5)"));
			Assert.AreEqual(5.0, script.Eval("Math.round(5.4)"));
		}

		// Math.sign
		[TestMethod]
		public void Test29_sign()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1.0, script.Eval("Math.sign(5)"));
			Assert.AreEqual(-1.0, script.Eval("Math.sign(-5)"));
			Assert.AreEqual(0.0, script.Eval("Math.sign(0)"));
		}

		[TestMethod]
		public void Test29_sign_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1.0, script.Eval("Math.sign(5)"));
			Assert.AreEqual(-1.0, script.Eval("Math.sign(-5)"));
			Assert.AreEqual(0.0, script.Eval("Math.sign(0)"));
		}

		// Math.sin
		[TestMethod]
		public void Test30_sin()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.sin(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test30_sin_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.sin(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.sinh
		[TestMethod]
		public void Test31_sinh()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.sinh(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test31_sinh_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.sinh(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.sqrt
		[TestMethod]
		public void Test32_sqrt()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(2.0, script.Eval("Math.sqrt(4)"));
			Assert.AreEqual(3.0, script.Eval("Math.sqrt(9)"));
		}

		[TestMethod]
		public void Test32_sqrt_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(2.0, script.Eval("Math.sqrt(4)"));
			Assert.AreEqual(3.0, script.Eval("Math.sqrt(9)"));
		}

		// Math.tan
		[TestMethod]
		public void Test33_tan()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.tan(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test33_tan_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.tan(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.tanh
		[TestMethod]
		public void Test34_tanh()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.tanh(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test34_tanh_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.tanh(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.trunc
		[TestMethod]
		public void Test35_trunc()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(5.0, script.Eval("Math.trunc(5.9)"));
			Assert.AreEqual(-5.0, script.Eval("Math.trunc(-5.9)"));
		}

		[TestMethod]
		public void Test35_trunc_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(5.0, script.Eval("Math.trunc(5.9)"));
			Assert.AreEqual(-5.0, script.Eval("Math.trunc(-5.9)"));
		}

		// Math.E constant
		[TestMethod]
		public void Test36_E()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.E");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test36_E_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.E");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.PI constant
		[TestMethod]
		public void Test37_PI()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.PI");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test37_PI_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.PI");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.SQRT2 constant
		[TestMethod]
		public void Test38_SQRT2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.SQRT2");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test38_SQRT2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.SQRT2");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.SQRT1_2 constant
		[TestMethod]
		public void Test39_SQRT1_2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.SQRT1_2");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test39_SQRT1_2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.SQRT1_2");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.LN2 constant
		[TestMethod]
		public void Test40_LN2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.LN2");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test40_LN2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.LN2");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.LN10 constant
		[TestMethod]
		public void Test41_LN10()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.LN10");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test41_LN10_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.LN10");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.LOG2E constant
		[TestMethod]
		public void Test42_LOG2E()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.LOG2E");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test42_LOG2E_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.LOG2E");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Math.LOG10E constant
		[TestMethod]
		public void Test43_LOG10E()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.LOG10E");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		[TestMethod]
		public void Test43_LOG10E_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.LOG10E");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(double));
		}

		// Complex expression test
		[TestMethod]
		public void Test45_complexExpression()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.sqrt(Math.pow(3, 2) + Math.pow(4, 2))");
			Assert.AreEqual(5.0, result);
		}

		[TestMethod]
		public void Test45_complexExpression_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Math.sqrt(Math.pow(3, 2) + Math.pow(4, 2))");
			Assert.AreEqual(5.0, result);
		}

		// Edge cases
		[TestMethod]
		public void Test46_edgeCases()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			// NaN
			Assert.AreEqual(double.NaN, script.Eval("Math.sqrt(-1)"));
			// Infinity
			Assert.AreEqual(double.PositiveInfinity, script.Eval("Math.pow(10, 1000)"));
			Assert.AreEqual(double.NegativeInfinity, script.Eval("Math.log(0)"));
		}

		[TestMethod]
		public void Test46_edgeCases_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			// NaN
			Assert.AreEqual(double.NaN, script.Eval("Math.sqrt(-1)"));
			// Infinity
			Assert.AreEqual(double.PositiveInfinity, script.Eval("Math.pow(10, 1000)"));
			Assert.AreEqual(double.NegativeInfinity, script.Eval("Math.log(0)"));
		}
	}
}
