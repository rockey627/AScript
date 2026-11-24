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
			var format = GoTimeUtils.ToNetFormat("2006-01-02 15:04:05");
			Assert.AreEqual("yyyy'-'MM'-'dd' 'HH':'mm':'ss", format);
		}

		[TestMethod]
		public void Test03_ToNetFormat_Literal()
		{
			var format = GoTimeUtils.ToNetFormat("2006年01月02日");
			Assert.AreEqual("yyyy'年'MM'月'dd'日'", format);
		}

		[TestMethod]
		public void Test04_ToNetFormat_QuarterDay()
		{
			// Go: 01 月, 02 日, 15 时, 04 分, 05 秒, 03 12小时制
			Assert.AreEqual("MM", GoTimeUtils.ToNetFormat("01"));
			Assert.AreEqual("dd", GoTimeUtils.ToNetFormat("02"));
			Assert.AreEqual("HH", GoTimeUtils.ToNetFormat("15"));
			Assert.AreEqual("mm", GoTimeUtils.ToNetFormat("04"));
			Assert.AreEqual("ss", GoTimeUtils.ToNetFormat("05"));
			Assert.AreEqual("hh", GoTimeUtils.ToNetFormat("03"));
			Assert.AreEqual("yy", GoTimeUtils.ToNetFormat("06"));
			Assert.AreEqual("yyyy", GoTimeUtils.ToNetFormat("2006"));
		}

		[TestMethod]
		public void Test05_ToNetFormat_WeekdayAndMonth()
		{
			Assert.AreEqual("MMMM", GoTimeUtils.ToNetFormat("January"));
			Assert.AreEqual("MMM", GoTimeUtils.ToNetFormat("Jan"));
			Assert.AreEqual("dddd", GoTimeUtils.ToNetFormat("Monday"));
			Assert.AreEqual("ddd", GoTimeUtils.ToNetFormat("Mon"));
		}

		[TestMethod]
		public void Test06_Format_Result()
		{
			var t = new DateTime(2024, 1, 2, 3, 4, 5);
			Assert.AreEqual("2024-01-02 03:04:05", GoTimeExtensions.Format(t, "2006-01-02 15:04:05"));
			Assert.AreEqual("2024-01-02", GoTimeExtensions.Format(t, "2006-01-02"));
			Assert.AreEqual("03:04:05", GoTimeExtensions.Format(t, "15:04:05"));
		}

		#region Go time 包格式规范用例

		/// <summary>
		/// Go 的参考时间：2006-01-02 15:04:05 (Mon Jan  2 03:04:05 MST 2006)
		/// 这里取等价的一个真实时间点：2024-01-02 03:04:05 UTC（星期二）
		/// </summary>
		private static readonly DateTime _goRefTime = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);

		private static string F(string layout)
		{
			return GoTimeExtensions.Format(_goRefTime, layout);
		}

		[TestMethod]
		public void Test10_GoFormat_Year()
		{
			// Year: "2006" "06"
			Assert.AreEqual("2024", F("2006"));
			Assert.AreEqual("24", F("06"));
		}

		[TestMethod]
		public void Test11_GoFormat_Month()
		{
			Console.WriteLine(_goRefTime.ToString(""));
			// Month: "Jan" "January" "01" "1"
			Assert.AreEqual("Jan", F("Jan"));
			Assert.AreEqual("January", F("January"));
			Assert.AreEqual("01", F("01"));
			//Assert.AreEqual("1", F("1"));
		}

		[TestMethod]
		public void Test12_GoFormat_Day()
		{
			// Day: "2" "_2" "02"      "_2" 表示用空格补齐
			//Assert.AreEqual("2", F("2"));
			Assert.AreEqual("02", F("02"));
			//Assert.AreEqual(" 2", F("_2"));
		}

		[TestMethod]
		public void Test13_GoFormat_Hour()
		{
			// Hour: "15" "3" "03"    "15" 是 24 小时制，"03"/"3" 是 12 小时制
			Assert.AreEqual("03", F("15"));
			Assert.AreEqual("03", F("03"));
			//Assert.AreEqual("3", F("3"));
		}

		[TestMethod]
		public void Test14_GoFormat_Minute()
		{
			// Minute: "4" "04"
			Assert.AreEqual("04", F("04"));
			//Assert.AreEqual("4", F("4"));
		}

		[TestMethod]
		public void Test15_GoFormat_Second()
		{
			// Second: "5" "05"
			Assert.AreEqual("05", F("05"));
			//Assert.AreEqual("5", F("5"));
		}

		[TestMethod]
		public void Test16_GoFormat_AMPM()
		{
			// AM/PM: "PM" "pm"
			Assert.AreEqual("AM", F("PM"));
			//Assert.AreEqual("am", F("pm"));
		}

		//[TestMethod]
		//public void Test17_GoFormat_DayOfYear()
		//{
		//	// Day of year: "__2" "002"
		//	//Assert.AreEqual("002", F("002"));
		//	//Assert.AreEqual(" 2", F("__2"));
		//}

		[TestMethod]
		public void Test18_GoFormat_Weekday()
		{
			// Weekday: "Mon" "Monday"
			Assert.AreEqual("Tue", F("Mon"));
			Assert.AreEqual("Tuesday", F("Monday"));
		}

		[TestMethod]
		public void Test19_GoFormat_Kitchen()
		{
			// Kitchen: "3:04PM"
			Assert.AreEqual("3:04AM", F("3:04PM"));
		}

		[TestMethod]
		public void Test20_GoFormat_Stamp()
		{
			// Stamp: "Jan _2 15:04:05"
			Assert.AreEqual("Jan 2 03:04:05", F("Jan _2 15:04:05"));
		}

		[TestMethod]
		public void Test21_GoFormat_StampMilli()
		{
			// StampMilli: "Jan _2 15:04:05.000"
			Assert.AreEqual("Jan 2 03:04:05.000", F("Jan _2 15:04:05.000"));
		}

		[TestMethod]
		public void Test22_GoFormat_StampMicro()
		{
			// StampMicro: "Jan _2 15:04:05.000000"
			Assert.AreEqual("Jan 2 03:04:05.000000", F("Jan _2 15:04:05.000000"));
		}

		//[TestMethod]
		//public void Test23_GoFormat_StampNano()
		//{
		//	// StampNano: "Jan _2 15:04:05.000000000"
		//	Assert.AreEqual("Jan 2 03:04:05.000000000", F("Jan _2 15:04:05.000000000"));
		//}

		[TestMethod]
		public void Test24_GoFormat_RFC3339()
		{
			// RFC3339: "2006-01-02T15:04:05Z07:00"
			Assert.AreEqual("2024-01-02T03:04:05Z", F("2006-01-02T15:04:05Z07:00"));
		}

		//[TestMethod]
		//public void Test25_GoFormat_RFC3339Nano()
		//{
		//	// RFC3339Nano: "2006-01-02T15:04:05.999999999Z07:00"
		//	Assert.AreEqual("2024-01-02T03:04:05Z", F("2006-01-02T15:04:05.999999999Z07:00"));
		//}

		//[TestMethod]
		//public void Test26_GoFormat_RFC1123()
		//{
		//	// RFC1123: "Mon, 02 Jan 2006 15:04:05 MST"
		//	Assert.AreEqual("Tue, 02 Jan 2024 03:04:05 UTC", F("Mon, 02 Jan 2006 15:04:05 MST"));
		//}

		//[TestMethod]
		//public void Test27_GoFormat_RFC1123Z()
		//{
		//	// RFC1123Z: "Mon, 02 Jan 2006 15:04:05 -0700"
		//	Assert.AreEqual("Tue, 02 Jan 2024 03:04:05 +0000", F("Mon, 02 Jan 2006 15:04:05 -0700"));
		//}

		//[TestMethod]
		//public void Test28_GoFormat_RFC822()
		//{
		//	// RFC822: "02 Jan 06 15:04 MST"
		//	Assert.AreEqual("02 Jan 24 03:04 UTC", F("02 Jan 06 15:04 MST"));
		//}

		//[TestMethod]
		//public void Test29_GoFormat_RFC850()
		//{
		//	// RFC850: "Monday, 02-Jan-06 15:04:05 MST"
		//	Assert.AreEqual("Tuesday, 02-Jan-24 03:04:05 UTC", F("Monday, 02-Jan-06 15:04:05 MST"));
		//}

		//[TestMethod]
		//public void Test30_GoFormat_UnixDate()
		//{
		//	// UnixDate: "Mon Jan _2 15:04:05 MST 2006"
		//	Assert.AreEqual("Tue Jan  2 03:04:05 UTC 2024", F("Mon Jan _2 15:04:05 MST 2006"));
		//}

		//[TestMethod]
		//public void Test31_GoFormat_RubyDate()
		//{
		//	// RubyDate: "Mon Jan 02 15:04:05 -0700 2006"
		//	Assert.AreEqual("Tue Jan 02 03:04:05 +0000 2024", F("Mon Jan 02 15:04:05 -0700 2006"));
		//}

		//[TestMethod]
		//public void Test32_GoFormat_ANSIC()
		//{
		//	// ANSIC: "Mon Jan _2 15:04:05 2006"
		//	Assert.AreEqual("Tue Jan  2 03:04:05 2024", F("Mon Jan _2 15:04:05 2006"));
		//}

		[TestMethod]
		public void Test33_GoFormat_Answer()
		{
			// Answer: "Jan 2 15:04:05"
			Assert.AreEqual("Jan 2 03:04:05", F("Jan 2 15:04:05"));
		}

		[TestMethod]
		public void Test34_GoFormat_HTTPDate()
		{
			// HTTPDate: "Mon, 02 Jan 2006 15:04:05 GMT"
			Assert.AreEqual("Tue, 02 Jan 2024 03:04:05 GMT", F("Mon, 02 Jan 2006 15:04:05 GMT"));
		}

		[TestMethod]
		public void Test35_GoFormat_DateTime()
		{
			// DateTime: "2006-01-02 15:04:05"
			Assert.AreEqual("2024-01-02 03:04:05", F("2006-01-02 15:04:05"));
		}

		[TestMethod]
		public void Test36_GoFormat_DateOnly()
		{
			// DateOnly: "2006-01-02"
			Assert.AreEqual("2024-01-02", F("2006-01-02"));
		}

		[TestMethod]
		public void Test37_GoFormat_TimeOnly()
		{
			// TimeOnly: "15:04:05"
			Assert.AreEqual("03:04:05", F("15:04:05"));
		}

		//[TestMethod]
		//public void Test38_GoFormat_TimeZone()
		//{
		//	// Z07:00 / Z0700 / Z07 / -07:00 / -0700 / -07
		//	Assert.AreEqual("Z", F("Z07:00"));
		//	Assert.AreEqual("Z", F("Z0700"));
		//	Assert.AreEqual("Z", F("Z07"));
		//	Assert.AreEqual("+00:00", F("-07:00"));
		//	Assert.AreEqual("+0000", F("-0700"));
		//	Assert.AreEqual("+00", F("-07"));
		//}

		[TestMethod]
		public void Test39_GoFormat_LocalTimeZone()
		{
			// 本地时间：Z07:00 输出 +08:00 形式
			var t = DateTime.Now;
			Assert.AreEqual(t.ToString("zzz"), F2(t, "Z07:00"));
			Assert.AreEqual(t.ToString("zzz"), F2(t, "-07:00"));
		}

		private static string F2(DateTime t, string layout)
		{
			return GoTimeExtensions.Format(t, layout);
		}

		[TestMethod]
		public void Test40_GoFormat_EmptyLayout()
		{
			Assert.AreEqual("", GoTimeExtensions.Format(_goRefTime, ""));
			Assert.AreEqual("", GoTimeExtensions.Format(_goRefTime, null));
		}

		#endregion

		#region 脚本级用例

		[TestMethod]
		public void Test50_Script_Format()
		{
			var s = @"
import ""time""
t.Format(""2006-01-02 15:04:05"")
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual("2024-01-02 03:04:05", script.Eval<string>(s));
		}

		[TestMethod]
		public void Test50_Script_Format_CompileAll()
		{
			var s = @"
import ""time""
t.Format(""2006-01-02 15:04:05"")
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual("2024-01-02 03:04:05", script.Eval<string>(s));
		}

		[TestMethod]
		public void Test51_Script_Format_MultiLayout()
		{
			var s = @"
import ""time""
var a = t.Format(""2006-01-02"")
var b = t.Format(""15:04:05"")
var c = t.Format(""Jan _2 15:04:05"")
a + ""|"" + b + ""|"" + c
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual("2024-01-02|03:04:05|Jan 2 03:04:05", script.Eval<string>(s));
		}

		[TestMethod]
		public void Test51_Script_Format_MultiLayout_CompileAll()
		{
			var s = @"
import ""time""
var a = t.Format(""2006-01-02"")
var b = t.Format(""15:04:05"")
var c = t.Format(""Jan _2 15:04:05"")
a + ""|"" + b + ""|"" + c
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual("2024-01-02|03:04:05|Jan 2 03:04:05", script.Eval<string>(s));
		}

		[TestMethod]
		public void Test52_Script_Now()
		{
			var s = @"
import ""time""
time.Now()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var before = DateTime.Now.AddSeconds(-5);
			var result = script.Eval(s);
			Assert.IsInstanceOfType(result, typeof(DateTime));
			var t = (DateTime)result;
			Assert.IsTrue(t >= before && t <= DateTime.Now.AddSeconds(5));
		}

		[TestMethod]
		public void Test52_Script_Now_CompileAll()
		{
			var s = @"
import ""time""
time.Now()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var before = DateTime.Now.AddSeconds(-5);
			var result = script.Eval(s);
			Assert.IsInstanceOfType(result, typeof(DateTime));
			var t = (DateTime)result;
			Assert.IsTrue(t >= before && t <= DateTime.Now.AddSeconds(5));
		}

		[TestMethod]
		public void Test53_Script_Sleep()
		{
			var s = @"
import ""time""
time.Sleep(50)
""done""
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var sw = System.Diagnostics.Stopwatch.StartNew();
			Assert.AreEqual("done", script.Eval<string>(s));
			Assert.IsTrue(sw.ElapsedMilliseconds >= 50);
		}

		[TestMethod]
		public void Test53_Script_Sleep_CompileAll()
		{
			var s = @"
import ""time""
time.Sleep(50)
""done""
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var sw = System.Diagnostics.Stopwatch.StartNew();
			Assert.AreEqual("done", script.Eval<string>(s));
			Assert.IsTrue(sw.ElapsedMilliseconds >= 50);
		}

		[TestMethod]
		public void Test54_Script_Parse()
		{
			var s = @"
import ""time""
t, err := time.Parse(""2006-01-02 15:04:05"", ""2024-01-02 03:04:05"")
t.Format(""2006-01-02 15:04:05"")
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("2024-01-02 03:04:05", script.Eval<string>(s));
		}

		[TestMethod]
		public void Test54_Script_Parse_CompileAll()
		{
			var s = @"
import ""time""
t, err := time.Parse(""2006-01-02 15:04:05"", ""2024-01-02 03:04:05"")
t.Format(""2006-01-02 15:04:05"")
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("2024-01-02 03:04:05", script.Eval<string>(s));
		}

		[TestMethod]
		public void Test55_Script_Parse_Error()
		{
			var s = @"
import ""time""
t, err := time.Parse(""2006-01-02"", ""not a time"")
err
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.IsNotNull(script.Eval(s));
		}

		[TestMethod]
		public void Test56_Script_TimeComponents()
		{
			var s = @"
import ""time""
var y = t.Year()
var m = t.Month()
var d = t.Day()
var h = t.Hour()
var mi = t.Minute()
var sec = t.Second()
"""" + y + ""-"" + m + ""-"" + d + "" "" + h + "":"" + mi + "":"" + sec
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual("2024-1-2 3:4:5", script.Eval<string>(s));
		}

		[TestMethod]
		public void Test56_Script_TimeComponents_CompileAll()
		{
			var s = @"
import ""time""
var y = t.Year()
var m = t.Month()
var d = t.Day()
var h = t.Hour()
var mi = t.Minute()
var sec = t.Second()
"""" + y + ""-"" + m + ""-"" + d + "" "" + h + "":"" + mi + "":"" + sec
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual("2024-1-2 3:4:5", script.Eval<string>(s));
		}

		[TestMethod]
		public void Test57_Script_Add_Sub()
		{
			var s = @"
import ""time""
var t2 = t.Add(1000)
var d = t.Sub(t2)
d.TotalMilliseconds
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual(-1000d, script.Eval(s));
		}

		[TestMethod]
		public void Test57_Script_Add_Sub_CompileAll()
		{
			var s = @"
import ""time""
var t2 = t.Add(1000)
var d = t.Sub(t2)
d.TotalMilliseconds
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual(-1000d, script.Eval(s));
		}

		[TestMethod]
		public void Test58_Script_Before_After_Equal()
		{
			var s = @"
import ""time""
var r1 = t.Before(t.Add(1000))
var r2 = t.After(t.Add(-1000))
var r3 = t.Equal(t)
r1 && r2 && r3
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test58_Script_Before_After_Equal_CompileAll()
		{
			var s = @"
import ""time""
var r1 = t.Before(t.Add(1000))
var r2 = t.After(t.Add(-1000))
var r3 = t.Equal(t)
r1 && r2 && r3
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Context.SetVar("t", new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
			Assert.AreEqual(true, script.Eval(s));
		}

		#endregion
	}
}
