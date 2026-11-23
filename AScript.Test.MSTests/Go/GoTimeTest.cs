using AScript.Functions;
using AScript.Lang.Go;
using AScript.Lang.Go.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoTimeTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["go"] = GoLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("go");
		}

		[TestMethod]
		public void Test01_Format()
		{
			var s = @"
import ""time""
var t = time.Now()
t.Format(""2006-01-02 15:04:05"")
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s) as string;
			Console.WriteLine(result);
			Assert.IsNotNull(result);
			// 形如 2024-01-02 03:04:05
			Assert.AreEqual(19, result.Length);
			Assert.AreEqual('-', result[4]);
			Assert.AreEqual('-', result[7]);
			Assert.AreEqual(' ', result[10]);
			Assert.AreEqual(':', result[13]);
			Assert.AreEqual(':', result[16]);
		}

		[TestMethod]
		public void Test01_Format_CompileAll()
		{
			var s = @"
import ""time""
var t = time.Now()
t.Format(""2006-01-02 15:04:05"")
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s) as string;
			Console.WriteLine(result);
			Assert.IsNotNull(result);
			Assert.AreEqual(19, result.Length);
		}

		[TestMethod]
		public void Test01_Format2()
		{
			var s = @"
import ""time""
now.Format(""2006-01-02 15:04:05"")
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var now = DateTime.Now;
			script.Context.SetVar("now", now);
			var result = script.Eval<string>(s);
			Assert.AreEqual(now.ToString("yyyy-MM-dd HH:mm:ss"), result);
		}

		[TestMethod]
		public void Test01_Format2_CompileAll()
		{
			var s = @"
import ""time""
now.Format(""2006-01-02 15:04:05"")
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var now = DateTime.Now;
			script.Context.SetVar("now", now);
			var result = script.Eval<string>(s);
			Assert.AreEqual(now.ToString("yyyy-MM-dd HH:mm:ss"), result);
		}

		[TestMethod]
		public void Test02_ToNetFormat()
		{
			var format = GoTimeExtensions.ToNetFormat("2006-01-02 15:04:05");
			Assert.AreEqual("yyyy'-'MM'-'dd' 'HH':'mm':'ss", format);
		}

		[TestMethod]
		public void Test03_ToNetFormat_Literal()
		{
			var format = GoTimeExtensions.ToNetFormat("2006年01月02日");
			Assert.AreEqual("yyyy'年'MM'月'dd'日'", format);
		}

		[TestMethod]
		public void Test04_ToNetFormat_QuarterDay()
		{
			// Go: 01 月, 02 日, 15 时, 04 分, 05 秒, 03 12小时制
			Assert.AreEqual("MM", GoTimeExtensions.ToNetFormat("01"));
			Assert.AreEqual("dd", GoTimeExtensions.ToNetFormat("02"));
			Assert.AreEqual("HH", GoTimeExtensions.ToNetFormat("15"));
			Assert.AreEqual("mm", GoTimeExtensions.ToNetFormat("04"));
			Assert.AreEqual("ss", GoTimeExtensions.ToNetFormat("05"));
			Assert.AreEqual("hh", GoTimeExtensions.ToNetFormat("03"));
			Assert.AreEqual("yy", GoTimeExtensions.ToNetFormat("06"));
			Assert.AreEqual("yyyy", GoTimeExtensions.ToNetFormat("2006"));
		}

		[TestMethod]
		public void Test05_ToNetFormat_WeekdayAndMonth()
		{
			Assert.AreEqual("MMMM", GoTimeExtensions.ToNetFormat("January"));
			Assert.AreEqual("MMM", GoTimeExtensions.ToNetFormat("Jan"));
			Assert.AreEqual("dddd", GoTimeExtensions.ToNetFormat("Monday"));
			Assert.AreEqual("ddd", GoTimeExtensions.ToNetFormat("Mon"));
		}

		[TestMethod]
		public void Test06_Format_Result()
		{
			var t = new DateTime(2024, 1, 2, 3, 4, 5);
			Assert.AreEqual("2024-01-02 03:04:05", GoTimeExtensions.Format(t, "2006-01-02 15:04:05"));
			Assert.AreEqual("2024-01-02", GoTimeExtensions.Format(t, "2006-01-02"));
			Assert.AreEqual("03:04:05", GoTimeExtensions.Format(t, "15:04:05"));
		}
	}
}
