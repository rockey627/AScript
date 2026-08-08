using AScript.Lang.Lua;
using AScript.Lang.Lua.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AScript.Test.MSTests.Lua
{
	[TestClass]
	public class LuaIOTest
	{
		private static string _testDir;
		private static string TestDir
		{
			get
			{
				if (_testDir == null)
				{
					_testDir = Path.Combine(Path.GetTempPath(), "LuaIOTest_" + Guid.NewGuid().ToString("N"));
					Directory.CreateDirectory(_testDir);
				}
				return _testDir;
			}
		}

		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["lua"] = LuaLang.Instance;
			LuaLang.Instance.Modules.Add("io", new LuaIOModule());
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("lua");
			if (_testDir != null && Directory.Exists(_testDir))
			{
				try
				{
					Directory.Delete(_testDir, true);
				}
				catch
				{
					// 忽略删除失败
				}
			}
		}

		[TestMethod]
		public void Test01_io_open_write_read()
		{
			string file = Path.Combine(TestDir, "test01.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('hello')
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello", script.Eval(code));
		}

		[TestMethod]
		public void Test01_io_open_write_read_CompileAll()
		{
			string file = Path.Combine(TestDir, "test01_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('hello')
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello", script.Eval(code));
		}

		[TestMethod]
		public void Test02_io_open_mode_w()
		{
			string file = Path.Combine(TestDir, "test02.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('first')
f:close()
local f = io.open('{file}', 'w')
f:write('second')
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("second", script.Eval(code));
		}

		[TestMethod]
		public void Test02_io_open_mode_w_CompileAll()
		{
			string file = Path.Combine(TestDir, "test02_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('first')
f:close()
local f = io.open('{file}', 'w')
f:write('second')
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("second", script.Eval(code));
		}

		[TestMethod]
		public void Test03_io_open_mode_a()
		{
			string file = Path.Combine(TestDir, "test03.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('first')
f:close()
local f = io.open('{file}', 'a')
f:write('-append')
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("first-append", script.Eval(code));
		}

		[TestMethod]
		public void Test03_io_open_mode_a_CompileAll()
		{
			string file = Path.Combine(TestDir, "test03_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('first')
f:close()
local f = io.open('{file}', 'a')
f:write('-append')
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("first-append", script.Eval(code));
		}

		[TestMethod]
		public void Test04_file_read_line()
		{
			string file = Path.Combine(TestDir, "test04.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\nline3\n");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local line1 = f:read('l')
local line2 = f:read('l')
f:close()
line1 .. '|' .. line2
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1|line2", script.Eval(code));
		}

		[TestMethod]
		public void Test04_file_read_line_CompileAll()
		{
			string file = Path.Combine(TestDir, "test04_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\nline3\n");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local line1 = f:read('l')
local line2 = f:read('l')
f:close()
line1 .. '|' .. line2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1|line2", script.Eval(code));
		}

		[TestMethod]
		public void Test05_file_read_n_chars()
		{
			string file = Path.Combine(TestDir, "test05.txt").Replace('\\', '/');
			File.WriteAllText(file, "abcdefghij");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local chars = f:read(5)
f:close()
chars
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abcde", script.Eval(code));
		}

		[TestMethod]
		public void Test05_file_read_n_chars_CompileAll()
		{
			string file = Path.Combine(TestDir, "test05_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "abcdefghij");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local chars = f:read(5)
f:close()
chars
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abcde", script.Eval(code));
		}

		[TestMethod]
		public void Test06_file_write_multiple_values()
		{
			string file = Path.Combine(TestDir, "test06.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('hello', ' ', 'world', 123)
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello world123", script.Eval(code));
		}

		[TestMethod]
		public void Test06_file_write_multiple_values_CompileAll()
		{
			string file = Path.Combine(TestDir, "test06_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('hello', ' ', 'world', 123)
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello world123", script.Eval(code));
		}

		[TestMethod]
		public void Test07_file_seek()
		{
			string file = Path.Combine(TestDir, "test07.txt").Replace('\\', '/');
			File.WriteAllText(file, "0123456789");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f:seek('set', 3)
local char = f:read(1)
f:close()
char
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("3", script.Eval(code));
		}

		[TestMethod]
		public void Test07_file_seek_CompileAll()
		{
			string file = Path.Combine(TestDir, "test07_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "0123456789");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f:seek('set', 3)
local char = f:read(1)
f:close()
char
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("3", script.Eval(code));
		}

		[TestMethod]
		public void Test08_file_seek_cur()
		{
			string file = Path.Combine(TestDir, "test08.txt").Replace('\\', '/');
			File.WriteAllText(file, "0123456789");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f:seek('set', 3)
f:seek('cur', 2)
local char = f:read(1)
f:close()
char
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("5", script.Eval(code));
		}

		[TestMethod]
		public void Test08_file_seek_cur_CompileAll()
		{
			string file = Path.Combine(TestDir, "test08_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "0123456789");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f:seek('set', 3)
f:seek('cur', 2)
local char = f:read(1)
f:close()
char
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("5", script.Eval(code));
		}

		[TestMethod]
		public void Test09_file_seek_end()
		{
			string file = Path.Combine(TestDir, "test09.txt").Replace('\\', '/');
			File.WriteAllText(file, "0123456789");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f:seek('end', -3)
local char = f:read(1)
f:close()
char
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("7", script.Eval(code));
		}

		[TestMethod]
		public void Test09_file_seek_end_CompileAll()
		{
			string file = Path.Combine(TestDir, "test09_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "0123456789");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f:seek('end', -3)
local char = f:read(1)
f:close()
char
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("7", script.Eval(code));
		}

		[TestMethod]
		public void Test10_file_rewind()
		{
			string file = Path.Combine(TestDir, "test10.txt").Replace('\\', '/');
			File.WriteAllText(file, "0123456789");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f:seek('end', -3)
f:rewind()
local char = f:read(1)
f:close()
char
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("0", script.Eval(code));
		}

		[TestMethod]
		public void Test10_file_rewind_CompileAll()
		{
			string file = Path.Combine(TestDir, "test10_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "0123456789");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f:seek('end', -3)
f:rewind()
local char = f:read(1)
f:close()
char
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("0", script.Eval(code));
		}

		[TestMethod]
		public void Test11_io_type()
		{
			string file = Path.Combine(TestDir, "test11.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
local t1 = io.type(f)
f:close()
local t2 = io.type(f)
t1 .. '|' .. t2
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("file|closed file", script.Eval(code));
		}

		[TestMethod]
		public void Test11_io_type_CompileAll()
		{
			string file = Path.Combine(TestDir, "test11_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
local t1 = io.type(f)
f:close()
local t2 = io.type(f)
t1 .. '|' .. t2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("file|closed file", script.Eval(code));
		}

		[TestMethod]
		public void Test12_io_tmpfile()
		{
			string code = @"
require 'io'
local f = io.tmpfile()
local t = io.type(f)
f:close()
t
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("file", script.Eval(code));
		}

		[TestMethod]
		public void Test12_io_tmpfile_CompileAll()
		{
			string code = @"
require 'io'
local f = io.tmpfile()
local t = io.type(f)
f:close()
t
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("file", script.Eval(code));
		}

		[TestMethod]
		public void Test13_file_lines()
		{
			string file = Path.Combine(TestDir, "test13.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\nline3\n");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local lines = {{}}
for line in f:lines() do
	table.insert(lines, line)
end
f:close()
lines[2] .. '|' .. lines[3] .. '|' .. lines[4]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1|line2|line3", script.Eval(code));
		}

		[TestMethod]
		public void Test13_file_lines_CompileAll()
		{
			string file = Path.Combine(TestDir, "test13_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\nline3\n");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local lines = {{}}
for line in f:lines() do
	table.insert(lines, line)
end
f:close()
lines[2] .. '|' .. lines[3] .. '|' .. lines[4]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1|line2|line3", script.Eval(code));
		}

		[TestMethod]
		public void Test14_io_lines()
		{
			string file = Path.Combine(TestDir, "test14.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\nline3\n");
			string code = $@"
require 'io'
local lines = {{}}
for line in io.lines('{file}') do
	table.insert(lines, line)
end
lines[2] .. '|' .. lines[3]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1|line2", script.Eval(code));
		}

		[TestMethod]
		public void Test14_io_lines_CompileAll()
		{
			string file = Path.Combine(TestDir, "test14_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\nline3\n");
			string code = $@"
require 'io'
local lines = {{}}
for line in io.lines('{file}') do
	table.insert(lines, line)
end
lines[2] .. '|' .. lines[3]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1|line2", script.Eval(code));
		}

		[TestMethod]
		public void Test15_file_read_all()
		{
			string file = Path.Combine(TestDir, "test15.txt").Replace('\\', '/');
			File.WriteAllText(file, "hello\nworld");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local content = f:read()
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello\nworld", script.Eval(code));
		}

		[TestMethod]
		public void Test15_file_read_all_CompileAll()
		{
			string file = Path.Combine(TestDir, "test15_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "hello\nworld");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local content = f:read()
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello\nworld", script.Eval(code));
		}

		[TestMethod]
		public void Test16_file_read_L_with_newline()
		{
			string file = Path.Combine(TestDir, "test16.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\n");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local line = f:read('L')
f:close()
line
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1\n", script.Eval(code));
		}

		[TestMethod]
		public void Test16_file_read_L_with_newline_CompileAll()
		{
			string file = Path.Combine(TestDir, "test16_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\n");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local line = f:read('L')
f:close()
line
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1\n", script.Eval(code));
		}

		[TestMethod]
		public void Test17_io_open_mode_r_plus()
		{
			string file = Path.Combine(TestDir, "test17.txt").Replace('\\', '/');
			File.WriteAllText(file, "abcdef");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r+')
f:seek('set', 0)
f:write('XY')
f:seek('set', 0)
local content = f:read(4)
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("XYcd", script.Eval(code));
		}

		[TestMethod]
		public void Test17_io_open_mode_r_plus_CompileAll()
		{
			string file = Path.Combine(TestDir, "test17_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "abcdef");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r+')
f:seek('set', 0)
f:write('XY')
f:seek('set', 0)
local content = f:read(4)
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("XYcd", script.Eval(code));
		}

		[TestMethod]
		public void Test18_io_open_mode_w_plus()
		{
			string file = Path.Combine(TestDir, "test18.txt").Replace('\\', '/');
			File.WriteAllText(file, "abcdef");
			string code = $@"
require 'io'
local f = io.open('{file}', 'w+')
f:write('XY')
f:seek('set', 0)
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("XY", script.Eval(code));
		}

		[TestMethod]
		public void Test18_io_open_mode_w_plus_CompileAll()
		{
			string file = Path.Combine(TestDir, "test18_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "abcdef");
			string code = $@"
require 'io'
local f = io.open('{file}', 'w+')
f:write('XY')
f:seek('set', 0)
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("XY", script.Eval(code));
		}

		[TestMethod]
		public void Test19_io_open_mode_a_plus()
		{
			string file = Path.Combine(TestDir, "test19.txt").Replace('\\', '/');
			File.WriteAllText(file, "abc");
			string code = $@"
require 'io'
local f = io.open('{file}', 'a+')
f:write('xyz')
f:seek('set', 0)
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abcxyz", script.Eval(code));
		}

		[TestMethod]
		public void Test19_io_open_mode_a_plus_CompileAll()
		{
			string file = Path.Combine(TestDir, "test19_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "abc");
			string code = $@"
require 'io'
local f = io.open('{file}', 'a+')
f:write('xyz')
f:seek('set', 0)
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abcxyz", script.Eval(code));
		}

		[TestMethod]
		public void Test20_file_close_twice()
		{
			string file = Path.Combine(TestDir, "test20.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('test')
f:close()
f:close()
'ok'
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("ok", script.Eval(code));
		}

		[TestMethod]
		public void Test20_file_close_twice_CompileAll()
		{
			string file = Path.Combine(TestDir, "test20_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('test')
f:close()
f:close()
'ok'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("ok", script.Eval(code));
		}

		[TestMethod]
		public void Test21_io_close_with_file()
		{
			string file = Path.Combine(TestDir, "test21.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('test')
io.close(f)
'ok'
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("ok", script.Eval(code));
		}

		[TestMethod]
		public void Test21_io_close_with_file_CompileAll()
		{
			string file = Path.Combine(TestDir, "test21_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('test')
io.close(f)
'ok'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("ok", script.Eval(code));
		}

		[TestMethod]
		public void Test22_file_read_multiple_formats()
		{
			string file = Path.Combine(TestDir, "test22.txt").Replace('\\', '/');
			File.WriteAllText(file, "abcdef\nghij");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local a, b, c = f:read(3, 'l', 2)
f:close()
a .. '|' .. b .. '|' .. c
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abc|def\n|gh", script.Eval(code));
		}

		[TestMethod]
		public void Test22_file_read_multiple_formats_CompileAll()
		{
			string file = Path.Combine(TestDir, "test22_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "abcdef\nghij");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local a, b, c = f:read(3, 'l', 2)
f:close()
a .. '|' .. b .. '|' .. c
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abc|def\n|gh", script.Eval(code));
		}

		[TestMethod]
		public void Test23_io_open_invalid_mode()
		{
			string file = Path.Combine(TestDir, "test23.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'invalid')
f
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.IsNull(script.Eval(code));
		}

		[TestMethod]
		public void Test23_io_open_invalid_mode_CompileAll()
		{
			string file = Path.Combine(TestDir, "test23_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'invalid')
f
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.IsNull(script.Eval(code));
		}

		[TestMethod]
		public void Test24_io_open_nonexistent_file()
		{
			string file = Path.Combine(TestDir, "nonexistent_12345.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.IsNull(script.Eval(code));
		}

		[TestMethod]
		public void Test24_io_open_nonexistent_file_CompileAll()
		{
			string file = Path.Combine(TestDir, "nonexistent_12345_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
f
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.IsNull(script.Eval(code));
		}

		[TestMethod]
		public void Test25_file_flush()
		{
			string file = Path.Combine(TestDir, "test25.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('test')
f:flush()
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("test", script.Eval(code));
		}

		[TestMethod]
		public void Test25_file_flush_CompileAll()
		{
			string file = Path.Combine(TestDir, "test25_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:write('test')
f:flush()
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("test", script.Eval(code));
		}

		[TestMethod]
		public void Test26_file_setvbuf()
		{
			string file = Path.Combine(TestDir, "test26.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:setvbuf('no')
f:write('test')
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("test", script.Eval(code));
		}

		[TestMethod]
		public void Test26_file_setvbuf_CompileAll()
		{
			string file = Path.Combine(TestDir, "test26_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
f:setvbuf('no')
f:write('test')
f:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("test", script.Eval(code));
		}

		[TestMethod]
		public void Test27_io_output_input()
		{
			string file = Path.Combine(TestDir, "test27.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
io.output(f)
io.write('hello')
io.output():close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello", script.Eval(code));
		}

		[TestMethod]
		public void Test27_io_output_input_CompileAll()
		{
			string file = Path.Combine(TestDir, "test27_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
io.output(f)
io.write('hello')
io.output():close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello", script.Eval(code));
		}

		[TestMethod]
		public void Test28_file_write_return_self()
		{
			string file = Path.Combine(TestDir, "test28.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
local result = f:write('hello')
result:write(' world')
result:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello world", script.Eval(code));
		}

		[TestMethod]
		public void Test28_file_write_return_self_CompileAll()
		{
			string file = Path.Combine(TestDir, "test28_ca.txt").Replace('\\', '/');
			string code = $@"
require 'io'
local f = io.open('{file}', 'w')
local result = f:write('hello')
result:write(' world')
result:close()
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello world", script.Eval(code));
		}

		[TestMethod]
		public void Test29_file_read_star_a()
		{
			string file = Path.Combine(TestDir, "test29.txt").Replace('\\', '/');
			File.WriteAllText(file, "hello\nworld");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello\nworld", script.Eval(code));
		}

		[TestMethod]
		public void Test29_file_read_star_a_CompileAll()
		{
			string file = Path.Combine(TestDir, "test29_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "hello\nworld");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local content = f:read('*a')
f:close()
content
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hello\nworld", script.Eval(code));
		}

		[TestMethod]
		public void Test30_file_read_star_l()
		{
			string file = Path.Combine(TestDir, "test30.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\nline3");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local line = f:read('*l')
f:close()
line
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1", script.Eval(code));
		}

		[TestMethod]
		public void Test30_file_read_star_l_CompileAll()
		{
			string file = Path.Combine(TestDir, "test30_ca.txt").Replace('\\', '/');
			File.WriteAllText(file, "line1\nline2\nline3");
			string code = $@"
require 'io'
local f = io.open('{file}', 'r')
local line = f:read('*l')
f:close()
line
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("line1", script.Eval(code));
		}
	}
}
