using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptDateTest
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

		// Date creation with milliseconds
		[TestMethod]
		public void Test01_creationWithMilliseconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(DateTime));
		}

		[TestMethod]
		public void Test01_creationWithMilliseconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(0)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(DateTime));
		}

		// Date creation with string
		[TestMethod]
		public void Test02_creationWithString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date('2024-01-15')");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(DateTime));
		}

		[TestMethod]
		public void Test02_creationWithString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date('2024-01-15')");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(DateTime));
		}

		// Date creation with year, month, day
		[TestMethod]
		public void Test03_creationWithYearMonthDay()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(DateTime));
		}

		[TestMethod]
		public void Test03_creationWithYearMonthDay_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(DateTime));
		}

		// Date creation with full parameters
		[TestMethod]
		public void Test04_creationWithFullParams()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45, 123)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(DateTime));
		}

		[TestMethod]
		public void Test04_creationWithFullParams_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45, 123)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(DateTime));
		}

		// Date.now() static method
		[TestMethod]
		public void Test05_now()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Date.now()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(long));
			Assert.IsTrue((long)result > 0);
		}

		[TestMethod]
		public void Test05_now_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Date.now()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(long));
			Assert.IsTrue((long)result > 0);
		}

		// Date.parse() static method
		[TestMethod]
		public void Test06_parse()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Date.parse('2024-01-15')");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(long));
			Assert.AreNotEqual(-1L, result);
		}

		[TestMethod]
		public void Test06_parse_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Date.parse('2024-01-15')");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(long));
			Assert.AreNotEqual(-1L, result);
		}

		// Date.UTC() static method
		[TestMethod]
		public void Test07_utc()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Date.UTC(2024, 0, 15)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(long));
		}

		[TestMethod]
		public void Test07_utc_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("Date.UTC(2024, 0, 15)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(long));
		}

		// getDate
		[TestMethod]
		public void Test08_getDate()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(15L, script.Eval("new Date(2024, 0, 15).getDate()"));
		}

		[TestMethod]
		public void Test08_getDate_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(15L, script.Eval("new Date(2024, 0, 15).getDate()"));
		}

		// getDay
		[TestMethod]
		public void Test09_getDay()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).getDay()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test09_getDay_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).getDay()");
			Assert.IsNotNull(result);
		}

		// getFullYear
		[TestMethod]
		public void Test10_getFullYear()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(2024L, script.Eval("new Date(2024, 0, 15).getFullYear()"));
		}

		[TestMethod]
		public void Test10_getFullYear_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(2024L, script.Eval("new Date(2024, 0, 15).getFullYear()"));
		}

		// getHours
		[TestMethod]
		public void Test11_getHours()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval("new Date(2024, 0, 15, 10, 30, 45).getHours()"));
		}

		[TestMethod]
		public void Test11_getHours_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval("new Date(2024, 0, 15, 10, 30, 45).getHours()"));
		}

		// getMilliseconds
		[TestMethod]
		public void Test12_getMilliseconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(123L, script.Eval("new Date(2024, 0, 15, 10, 30, 45, 123).getMilliseconds()"));
		}

		[TestMethod]
		public void Test12_getMilliseconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(123L, script.Eval("new Date(2024, 0, 15, 10, 30, 45, 123).getMilliseconds()"));
		}

		// getMinutes
		[TestMethod]
		public void Test13_getMinutes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(30L, script.Eval("new Date(2024, 0, 15, 10, 30, 45).getMinutes()"));
		}

		[TestMethod]
		public void Test13_getMinutes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(30L, script.Eval("new Date(2024, 0, 15, 10, 30, 45).getMinutes()"));
		}

		// getMonth
		[TestMethod]
		public void Test14_getMonth()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Date(2024, 0, 15).getMonth()"));
			Assert.AreEqual(11L, script.Eval("new Date(2024, 11, 15).getMonth()"));
		}

		[TestMethod]
		public void Test14_getMonth_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Date(2024, 0, 15).getMonth()"));
			Assert.AreEqual(11L, script.Eval("new Date(2024, 11, 15).getMonth()"));
		}

		// getSeconds
		[TestMethod]
		public void Test15_getSeconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(45L, script.Eval("new Date(2024, 0, 15, 10, 30, 45).getSeconds()"));
		}

		[TestMethod]
		public void Test15_getSeconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(45L, script.Eval("new Date(2024, 0, 15, 10, 30, 45).getSeconds()"));
		}

		// getTime
		[TestMethod]
		public void Test16_getTime()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(0).getTime()");
			Assert.AreEqual(0L, result);
		}

		[TestMethod]
		public void Test16_getTime_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(0).getTime()");
			Assert.AreEqual(0L, result);
		}

		// getTimezoneOffset
		[TestMethod]
		public void Test17_getTimezoneOffset()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date().getTimezoneOffset()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test17_getTimezoneOffset_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date().getTimezoneOffset()");
			Assert.IsNotNull(result);
		}

		// getYear
		[TestMethod]
		public void Test18_getYear()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(124L, script.Eval("new Date(2024, 0, 15).getYear()"));
		}

		[TestMethod]
		public void Test18_getYear_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(124L, script.Eval("new Date(2024, 0, 15).getYear()"));
		}

		// UTC getters
		[TestMethod]
		public void Test19_getUTCDate()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).getUTCDate()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test19_getUTCDate_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).getUTCDate()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test20_getUTCDay()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).getUTCDay()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test20_getUTCDay_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).getUTCDay()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test21_getUTCFullYear()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(2024L, script.Eval("new Date(2024, 0, 15).getUTCFullYear()"));
		}

		[TestMethod]
		public void Test21_getUTCFullYear_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(2024L, script.Eval("new Date(2024, 0, 15).getUTCFullYear()"));
		}

		[TestMethod]
		public void Test22_getUTCHours()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45).getUTCHours()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test22_getUTCHours_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45).getUTCHours()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test23_getUTCMilliseconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(123L, script.Eval("new Date(2024, 0, 15, 10, 30, 45, 123).getUTCMilliseconds()"));
		}

		[TestMethod]
		public void Test23_getUTCMilliseconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(123L, script.Eval("new Date(2024, 0, 15, 10, 30, 45, 123).getUTCMilliseconds()"));
		}

		[TestMethod]
		public void Test24_getUTCMinutes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45).getUTCMinutes()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test24_getUTCMinutes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45).getUTCMinutes()");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test25_getUTCMonth()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Date(2024, 0, 15).getUTCMonth()"));
		}

		[TestMethod]
		public void Test25_getUTCMonth_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Date(2024, 0, 15).getUTCMonth()"));
		}

		[TestMethod]
		public void Test26_getUTCSeconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(45L, script.Eval("new Date(2024, 0, 15, 10, 30, 45).getUTCSeconds()"));
		}

		[TestMethod]
		public void Test26_getUTCSeconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(45L, script.Eval("new Date(2024, 0, 15, 10, 30, 45).getUTCSeconds()"));
		}

		// setDate
		[TestMethod]
		public void Test27_setDate()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setDate(20)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test27_setDate_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setDate(20)");
			Assert.IsNotNull(result);
		}

		// setFullYear
		[TestMethod]
		public void Test28_setFullYear()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setFullYear(2025)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test28_setFullYear_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setFullYear(2025)");
			Assert.IsNotNull(result);
		}

		// setHours
		[TestMethod]
		public void Test29_setHours()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setHours(15)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test29_setHours_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setHours(15)");
			Assert.IsNotNull(result);
		}

		// setMilliseconds
		[TestMethod]
		public void Test30_setMilliseconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45, 100); d.setMilliseconds(200)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test30_setMilliseconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45, 100); d.setMilliseconds(200)");
			Assert.IsNotNull(result);
		}

		// setMinutes
		[TestMethod]
		public void Test31_setMinutes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setMinutes(50)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test31_setMinutes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setMinutes(50)");
			Assert.IsNotNull(result);
		}

		// setMonth
		[TestMethod]
		public void Test32_setMonth()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setMonth(5)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test32_setMonth_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setMonth(5)");
			Assert.IsNotNull(result);
		}

		// setSeconds
		[TestMethod]
		public void Test33_setSeconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setSeconds(59)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test33_setSeconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setSeconds(59)");
			Assert.IsNotNull(result);
		}

		// setTime
		[TestMethod]
		public void Test34_setTime()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(); d.setTime(0)");
			Assert.AreEqual(0L, result);
		}

		[TestMethod]
		public void Test34_setTime_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(); d.setTime(0)");
			Assert.AreEqual(0L, result);
		}

		// setYear
		[TestMethod]
		public void Test35_setYear()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setYear(99)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test35_setYear_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setYear(99)");
			Assert.IsNotNull(result);
		}

		// UTC setters
		[TestMethod]
		public void Test36_setUTCDate()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setUTCDate(20)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test36_setUTCDate_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setUTCDate(20)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test37_setUTCFullYear()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setUTCFullYear(2025)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test37_setUTCFullYear_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setUTCFullYear(2025)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test38_setUTCHours()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setUTCHours(15)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test38_setUTCHours_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setUTCHours(15)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test39_setUTCMilliseconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45, 100); d.setUTCMilliseconds(200)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test39_setUTCMilliseconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45, 100); d.setUTCMilliseconds(200)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test40_setUTCMinutes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setUTCMinutes(50)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test40_setUTCMinutes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setUTCMinutes(50)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test41_setUTCMonth()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setUTCMonth(5)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test41_setUTCMonth_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15); d.setUTCMonth(5)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test42_setUTCSeconds()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setUTCSeconds(59)");
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Test42_setUTCSeconds_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = new Date(2024, 0, 15, 10, 30, 45); d.setUTCSeconds(59)");
			Assert.IsNotNull(result);
		}

		// Conversion methods
		[TestMethod]
		public void Test43_toDateString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toDateString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test43_toDateString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toDateString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test44_toISOString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45, 123).toISOString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test44_toISOString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45, 123).toISOString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test45_toJSON()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toJSON()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test45_toJSON_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toJSON()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test46_toLocaleDateString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toLocaleDateString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test46_toLocaleDateString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toLocaleDateString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test47_toLocaleTimeString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45).toLocaleTimeString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test47_toLocaleTimeString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45).toLocaleTimeString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test48_toLocaleString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toLocaleString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test48_toLocaleString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toLocaleString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test49_toTimeString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45).toTimeString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test49_toTimeString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15, 10, 30, 45).toTimeString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test50_toUTCString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toUTCString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test50_toUTCString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toUTCString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test51_valueOf()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Date(0).valueOf()"));
		}

		[TestMethod]
		public void Test51_valueOf_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Date(0).valueOf()"));
		}

		[TestMethod]
		public void Test52_toString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}

		[TestMethod]
		public void Test52_toString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Date(2024, 0, 15).toString()");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(string));
		}
	}
}