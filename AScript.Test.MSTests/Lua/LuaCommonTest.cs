using AScript.Lang.Lua;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AScript.Test.MSTests.Lua
{
	[TestClass]
	public class LuaCommonTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["lua"] = LuaLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("lua");
		}

		[TestMethod]
		public void Test01_Variable_Local()
		{
			string code = @"
local x = 10
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test01_Variable_Local_CompileAll()
		{
			string code = @"
local x = 10
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_Function_Basic()
		{
			string code = @"
function add(a, b)
--[[ab
a=a+10
b=b+20
]]
	return a--[[hello]]+ b
end
add(3, 5)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(8L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_Function_Basic_CompileAll()
		{
			string code = @"
function add(a, b)
--[[ab
a=a+10
b=b+20
]]
	return a--[[hello]]+ b
end
add(3, 5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(8L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_Arithmetic_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(15L, script.Eval("10 + 5"));
			Assert.AreEqual(5L, script.Eval("10 - 5"));
			Assert.AreEqual(50L, script.Eval("10 * 5"));
			Assert.AreEqual(2.5, script.Eval("10 / 4"));
			Assert.AreEqual(1L, script.Eval("10 % 3"));
			Assert.AreEqual(8L, script.Eval("2 ^ 3"));
			Assert.AreEqual(3L, script.Eval("10 // 3"));
			Assert.AreEqual(2.5d, script.Eval("10.0 / 4"));
			Assert.AreEqual(8.0d, script.Eval("2.0 ^ 3"));
			Assert.AreEqual(3L, script.Eval("10.9 // 3"));
			Assert.AreEqual(27.0d, script.Eval("3 ^ 3.0"));
		}

		[TestMethod]
		public void Test03_Arithmetic_Basic_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(15L, script.Eval("10 + 5"));
			Assert.AreEqual(5L, script.Eval("10 - 5"));
			Assert.AreEqual(50L, script.Eval("10 * 5"));
			Assert.AreEqual(2.5, script.Eval("10 / 4"));
			Assert.AreEqual(1L, script.Eval("10 % 3"));
			Assert.AreEqual(8L, script.Eval("2 ^ 3"));
			Assert.AreEqual(3L, script.Eval("10 // 3"));
			Assert.AreEqual(2.5d, script.Eval("10.0 / 4"));
			Assert.AreEqual(8.0d, script.Eval("2.0 ^ 3"));
			Assert.AreEqual(3L, script.Eval("10.9 // 3"));
			Assert.AreEqual(27.0d, script.Eval("3 ^ 3.0"));
		}

		[TestMethod]
		public void Test04_Comparison_Operators()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(true, script.Eval("10 == 10"));
			Assert.AreEqual(false, script.Eval("10 == 5"));
			Assert.AreEqual(true, script.Eval("5 ~= 10"));
			Assert.AreEqual(true, script.Eval("10 > 5"));
			Assert.AreEqual(true, script.Eval("5 < 10"));
		}

		[TestMethod]
		public void Test04_Comparison_Operators_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(true, script.Eval("10 == 10"));
			Assert.AreEqual(false, script.Eval("10 == 5"));
			Assert.AreEqual(true, script.Eval("5 ~= 10"));
			Assert.AreEqual(true, script.Eval("10 > 5"));
			Assert.AreEqual(true, script.Eval("5 < 10"));
		}

		[TestMethod]
		public void Test05_Logic_Operators()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(true, script.Eval("true and true"));
			Assert.AreEqual(false, script.Eval("true and false"));
			Assert.AreEqual(true, script.Eval("true or false"));
			Assert.AreEqual(false, script.Eval("not true"));
		}

		[TestMethod]
		public void Test05_Logic_Operators_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(true, script.Eval("true and true"));
			Assert.AreEqual(false, script.Eval("true and false"));
			Assert.AreEqual(true, script.Eval("true or false"));
			Assert.AreEqual(false, script.Eval("not true"));
		}

		[TestMethod]
		public void Test06_If_Statement()
		{
			string code = @"
local x = 10
if x > 5 then
	x = 100
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_If_Statement_CompileAll()
		{
			string code = @"
local x = 10
if x > 5 then
	x = 100
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_If_Else()
		{
			string code = @"
local x = 3
if x > 5 then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(200L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_If_Else_CompileAll()
		{
			string code = @"
local x = 3
if x > 5 then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(200L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_String_Concat()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("helloworld", script.Eval("'hello' .. 'world'"));
		}

		[TestMethod]
		public void Test08_String_Concat_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("helloworld", script.Eval("'hello' .. 'world'"));
		}

		[TestMethod]
		public void Test09_String_Length()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(5L, script.Eval("#\"hello\""));
			Assert.AreEqual(0L, script.Eval("#''"));
		}

		[TestMethod]
		public void Test09_String_Length_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(5L, script.Eval("#\"hello\""));
			Assert.AreEqual(0L, script.Eval("#''"));
		}

		[TestMethod]
		public void Test10_Nil()
		{
			string code = @"
local x = nil
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(null, script.Eval(code));
		}

		[TestMethod]
		public void Test10_Nil_CompileAll()
		{
			string code = @"
local x = nil
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(null, script.Eval(code));
		}

		[TestMethod]
		public void Test11_Unary_Minus()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(-10L, script.Eval("-10"));
			Assert.AreEqual(-5L, script.Eval("5 + -10"));
		}

		[TestMethod]
		public void Test11_Unary_Minus_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(-10L, script.Eval("-10"));
			Assert.AreEqual(-5L, script.Eval("5 + -10"));
		}

		[TestMethod]
		public void Test12_Table_Array()
		{
			string code = @"
local arr = {1, 2, 3}
arr[1]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test12_Table_Array_CompileAll()
		{
			string code = @"
local arr = {1, 2, 3}
arr[1]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_Function_NoArgs()
		{
			string code = @"
function getValue()
	return 100
end
getValue()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_Function_NoArgs_CompileAll()
		{
			string code = @"
function getValue()
	return 100
end
getValue()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test14_Function_Recursive()
		{
			string code = @"
function factorial(n)
	if n <= 1 then
		return 1
	end
	return n * factorial(n - 1)
end
factorial(5)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(120L, script.Eval(code));
		}

		[TestMethod]
		public void Test14_Function_Recursive_CompileAll()
		{
			string code = @"
function factorial(n)
	if n <= 1 then
		return 1
	end
	return n * factorial(n - 1)
end
factorial(5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(120L, script.Eval(code));
		}

		[TestMethod]
		public void Test15_String_LongBracket()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello", script.Eval("[[hello]]"));
			Assert.AreEqual("hello123", script.Eval("[[hello]]..'123'"));
			Assert.AreEqual("hello world", script.Eval("[[hello world]]"));
			Assert.AreEqual("line1\nline2", script.Eval("[[line1\nline2]]"));
			Assert.AreEqual("a]b", script.Eval("[[a]b]]"));
			Assert.AreEqual("hello\nworld", script.Eval("[[hello\\nworld]]"));
			Assert.AreEqual("hello\tworld", script.Eval("[[hello\\tworld]]"));
			Assert.AreEqual("hello\rworld", script.Eval("[[hello\\rworld]]"));
			Assert.AreEqual("hello\\world", script.Eval("[[hello\\\\world]]"));
			Assert.AreEqual("line1line2", script.Eval("[[line1\\z\nline2]]"));
		}

		[TestMethod]
		public void Test15_String_LongBracket_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello", script.Eval("[[hello]]"));
			Assert.AreEqual("hello123", script.Eval("[[hello]]..'123'"));
			Assert.AreEqual("hello world", script.Eval("[[hello world]]"));
			Assert.AreEqual("line1\nline2", script.Eval("[[line1\nline2]]"));
			Assert.AreEqual("a]b", script.Eval("[[a]b]]"));
			Assert.AreEqual("hello\nworld", script.Eval("[[hello\\nworld]]"));
			Assert.AreEqual("hello\tworld", script.Eval("[[hello\\tworld]]"));
			Assert.AreEqual("hello\rworld", script.Eval("[[hello\\rworld]]"));
			Assert.AreEqual("hello\\world", script.Eval("[[hello\\\\world]]"));
			Assert.AreEqual("line1line2", script.Eval("[[line1\\z\nline2]]"));
		}

		[TestMethod]
		public void Test16_Tuple()
		{
			var s = @"
local a,b = 1, 2
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			script.Eval(s);
			Assert.AreEqual(1L, script.Eval("a"));
			Assert.AreEqual(2L, script.Eval("b"));
		}

		[TestMethod]
		public void Test16_Tuple_CompileAll()
		{
			var s = @"
local a,b = 1, 2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			script.Eval(s);
			Assert.AreEqual(1L, script.Eval("a"));
			Assert.AreEqual(2L, script.Eval("b"));
		}

		[TestMethod]
		public void Test16_Tuple2()
		{
			var s = @"
local a,b = 1
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			script.Eval(s);
			Assert.AreEqual(1L, script.Eval("a"));
			Assert.IsNull(script.Eval("b"));
		}

		[TestMethod]
		public void Test16_Tuple2_CompileAll()
		{
			var s = @"
local a,b = 1
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			script.Eval(s);
			Assert.AreEqual(1L, script.Eval("a"));
			Assert.IsNull(script.Eval("b"));
		}

		[TestMethod]
		public void Test17_Tuple()
		{
			var s = @"
local a,b = 1, 2
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(s));
		}

		[TestMethod]
		public void Test17_Tuple_CompileAll()
		{
			var s = @"
local a,b = 1, 2
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(s));
		}
	}
}
