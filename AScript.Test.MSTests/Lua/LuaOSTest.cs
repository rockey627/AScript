using AScript.Lang.Lua;
using AScript.Lang.Lua.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AScript.Test.MSTests.Lua
{
	[TestClass]
	public class LuaOSTest
	{
		private static string _testDir;

		private static string TestDir
		{
			get
			{
				if (_testDir == null)
				{
					_testDir = Path.Combine(Path.GetTempPath(), "LuaOSTest_" + Guid.NewGuid().ToString("N"));
					Directory.CreateDirectory(_testDir);
				}
				return _testDir;
			}
		}

		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["lua"] = LuaLang.Instance;
			//LuaLang.Instance.Modules.Add("io", new LuaIOModule());
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("lua");
			//if (_testDir != null && Directory.Exists(_testDir))
			//{
			//	try
			//	{
			//		Directory.Delete(_testDir, true);
			//	}
			//	catch
			//	{
			//		// 忽略删除失败
			//	}
			//}
		}

		private static Script NewScript(bool compileAll = false)
		{
			var script = new Script();
			if (compileAll) script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			return script;
		}

		/// <summary>
		/// net45 没有 DateTimeOffset.ToUnixTimeSeconds
		/// </summary>
		private static long ToUnixSeconds(DateTimeOffset value)
		{
			return (long)(value.UtcDateTime - new DateTime(1970, 1, 1)).TotalSeconds;
		}

		[TestMethod]
		public void Test01_Time()
		{
			var script = NewScript();
			var now = ToUnixSeconds(DateTimeOffset.UtcNow);
			var result = script.Eval<long>("os.time()");
			Console.WriteLine(result);
			Assert.IsTrue(Math.Abs(result - now) < 10, $"实际值 {result}，期望值约 {now}");
		}

		[TestMethod]
		public void Test01_Time_CompileAll()
		{
			var script = NewScript(true);
			var now = ToUnixSeconds(DateTimeOffset.UtcNow);
			var result = script.Eval<long>("os.time()");
			Assert.IsTrue(Math.Abs(result - now) < 10, $"实际值 {result}，期望值约 {now}");
		}

		[TestMethod]
		public void Test02_TimeWithTable()
		{
			var script = NewScript();
			var result = script.Eval<long>(@"
local t = {year = 2024, month = 1, day = 2, hour = 3, min = 4, sec = 5}
os.time(t)
");
			Console.WriteLine(result);
			var expect = ToUnixSeconds(new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeZoneInfo.Local.GetUtcOffset(new DateTime(2024, 1, 2, 3, 4, 5))));
			Assert.AreEqual(expect, result);
		}

		[TestMethod]
		public void Test02_TimeWithTable_CompileAll()
		{
			var script = NewScript(true);
			var result = script.Eval<long>(@"
local t = {year = 2024, month = 1, day = 2, hour = 3, min = 4, sec = 5}
os.time(t)
");
			var expect = ToUnixSeconds(new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeZoneInfo.Local.GetUtcOffset(new DateTime(2024, 1, 2, 3, 4, 5))));
			Assert.AreEqual(expect, result);
		}

		[TestMethod]
		public void Test03_Date()
		{
			var script = NewScript();
			var result = script.Eval<string>(@"
local t = os.time({year = 2024, month = 1, day = 2, hour = 3, min = 4, sec = 5})
os.date(""%Y-%m-%d %H:%M:%S"", t)
");
			Assert.AreEqual("2024-01-02 03:04:05", result);
		}

		[TestMethod]
		public void Test03_Date_CompileAll()
		{
			var script = NewScript(true);
			var result = script.Eval<string>(@"
local t = os.time({year = 2024, month = 1, day = 2, hour = 3, min = 4, sec = 5})
os.date(""%Y-%m-%d %H:%M:%S"", t)
");
			Assert.AreEqual("2024-01-02 03:04:05", result);
		}

		[TestMethod]
		public void Test04_DateTable()
		{
			var script = NewScript();
			var result = script.Eval<string>(@"
local t = os.time({year = 2024, month = 1, day = 2, hour = 3, min = 4, sec = 5})
local d = os.date(""*t"", t)
d.year .. ""-"" .. d.month .. ""-"" .. d.day .. "" "" .. d.hour .. "":"" .. d.min .. "":"" .. d.sec
");
			Assert.AreEqual("2024-1-2 3:4:5", result);
		}

		[TestMethod]
		public void Test04_DateTable_CompileAll()
		{
			var script = NewScript(true);
			var result = script.Eval<string>(@"
local t = os.time({year = 2024, month = 1, day = 2, hour = 3, min = 4, sec = 5})
local d = os.date(""*t"", t)
d.year .. ""-"" .. d.month .. ""-"" .. d.day .. "" "" .. d.hour .. "":"" .. d.min .. "":"" .. d.sec
");
			Assert.AreEqual("2024-1-2 3:4:5", result);
		}

		[TestMethod]
		public void Test05_DateTable_WDayAndYDay()
		{
			var script = NewScript();
			var result = script.Eval<string>(@"
local t = os.time({year = 2024, month = 1, day = 2, hour = 3, min = 4, sec = 5})
local d = os.date(""*t"", t)
d.wday .. "","" .. d.yday
");
			// 2024-01-02 是星期二，lua 的 wday 从周日开始，所以是 3；yday 为 2
			Assert.AreEqual("3,2", result);
		}

		[TestMethod]
		public void Test06_DateFormats()
		{
			var script = NewScript();
			var result = script.Eval<string>(@"
local t = os.time({year = 2024, month = 1, day = 2, hour = 15, min = 4, sec = 5})
os.date(""%Y|%m|%d|%H|%M|%S|%p|%A|%B|%a|%b"", t)
");
			Assert.AreEqual("2024|01|02|15|04|05|PM|Tuesday|January|Tue|Jan", result);
		}

		[TestMethod]
		public void Test07_DateDefault()
		{
			var script = NewScript();
			var result = script.Eval<string>("os.date()");
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Length > 0);
		}

		[TestMethod]
		public void Test08_Clock()
		{
			var script = NewScript();
			var result = script.Eval<double>("os.clock()");
			Assert.IsTrue(result >= 0);
		}

		[TestMethod]
		public void Test09_Difftime()
		{
			var script = NewScript();
			var result = script.Eval<double>("os.difftime(100, 40)");
			Assert.AreEqual(60d, result);
		}

		[TestMethod]
		public void Test10_Getenv()
		{
			var script = NewScript();
			var result = script.Eval<string>("os.getenv(\"PATH\")");
			Assert.IsNotNull(result);
			var none = script.Eval("os.getenv(\"__ASCRIPT_NOT_EXIST_ENV__\")");
			Assert.IsNull(none);
		}

//		[TestMethod]
//		public void Test11_RemoveAndRename()
//		{
//			string file = Path.Combine(TestDir, "test01.txt").Replace('\\', '/');
//			var script = NewScript();
//			script.Context.SetVar("file", file);
//			var result = script.Eval<string>(@"
//require 'io'
//local f = io.open(file, 'w')
//f:write('hello')
//f:close()
//local ok = os.rename(file, file .. '.bak')
//if not ok then return 'rename failed' end
//local ok2 = os.remove(file .. '.bak')
//if not ok2 then return 'remove failed' end
//'done'
//");
//			Assert.AreEqual("done", result);
//			Assert.IsFalse(File.Exists(file));
//			Assert.IsFalse(File.Exists(file + ".bak"));
//		}

//		[TestMethod]
//		public void Test11_RemoveAndRename_CompileAll()
//		{
//			string file = Path.Combine(TestDir, "test02.txt").Replace('\\', '/');
//			var script = NewScript(true);
//			script.Context.SetVar("file", file);
//			var result = script.Eval<string>(@"
//require 'io'
//local f = io.open(file, 'w')
//f:write('hello')
//f:close()
//local ok = os.rename(file, file .. '.bak')
//if not ok then return 'rename failed' end
//local ok2 = os.remove(file .. '.bak')
//if not ok2 then return 'remove failed' end
//'done'
//");
//			Assert.AreEqual("done", result);
//		}

//		[TestMethod]
//		public void Test12_RemoveNotExists()
//		{
//			var script = NewScript();
//			var result = script.Eval<string>(@"
//local ok, err = os.remove('/__ascript__/not_exist_file__')
//if ok == nil then return 'nil' end
//'not nil'
//");
//			Assert.AreEqual("nil", result);
//		}

		[TestMethod]
		public void Test13_Tmpname()
		{
			var script = NewScript();
			var result = script.Eval<string>("os.tmpname()");
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Length > 0);
		}

		//		[TestMethod]
		//		public void Test14_Setlocale()
		//		{
		//			var script = NewScript();
		//			Assert.AreEqual("C", script.Eval<string>("os.setlocale()"));
		//		}

		//		[TestMethod]
		//		public void Test15_Exit()
		//		{
		//			var script = NewScript();
		//			var result = script.Eval<string>(@"
		//local ok, err = pcall(function() os.exit(1) end)
		//if ok then return 'no exit' end
		//'exit'
		//");
		//			Assert.AreEqual("exit", result);
		//		}

		[TestMethod]
		public void Test15_time_CompileAll()
		{
			var s = @"
@lang lua
local now = os.time();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Console.WriteLine(script.Eval(s));
		}
	}
}
