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
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			string code = @"
local x = 10
x
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_Function_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			string code = @"
function add(a, b)
	return a + b
end
add(3, 5)
";
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
		public void Test06_If_Statement()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			string code = @"
local x = 10
if x > 5 then
	x = 100
end
x
";
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_If_Else()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			string code = @"
local x = 3
if x > 5 then
	x = 100
else
	x = 200
end
x
";
			Assert.AreEqual(200L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_String_Concat()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("helloworld", script.Eval("\"hello\" .. \"world\""));
		}

		[TestMethod]
		public void Test09_String_Length()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(5L, script.Eval("#\"hello\""));
		}

		[TestMethod]
		public void Test10_Nil()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			string code = @"
local x = nil
x
";
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
		public void Test12_Table_Array()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			string code = @"
local arr = {1, 2, 3}
arr[1]
";
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_Function_NoArgs()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			string code = @"
function getValue()
	return 100
end
getValue()
";
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test14_Function_Recursive()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			string code = @"
function factorial(n)
	if n <= 1 then
		return 1
	end
	return n * factorial(n - 1)
end
factorial(5)
";
			Assert.AreEqual(120L, script.Eval(code));
		}
	}
}
