using AScript.Lang.Lua;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.Lua
{
	[TestClass]
	public class LuaIfTest
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
		public void Test00_01()
		{
			string code = @"
local x = 10
if x >= 10 and x < 50 then
	local a = 5
	local b = 15
	x = a + b + 80
elseif x >= 50 and x < 100 then
	x = 200
else
	x = 300
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test00_01_CompileAll()
		{
			string code = @"
local x = 10
if x >= 10 and x < 50 then
	local a = 5
	local b = 15
	x = a + b + 80
elseif x >= 50 and x < 100 then
	x = 200
else
	x = 300
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test01_If_Basic_True()
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
		public void Test01_If_Basic_True_CompileAll()
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
		public void Test02_If_Basic_False()
		{
			string code = @"
local x = 3
if x > 5 then
	x = 100
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_If_Basic_False_CompileAll()
		{
			string code = @"
local x = 3
if x > 5 then
	x = 100
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_If_Else_True()
		{
			string code = @"
local x = 10
if x > 5 then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_If_Else_True_CompileAll()
		{
			string code = @"
local x = 10
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
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test04_If_Else_False()
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
		public void Test04_If_Else_False_CompileAll()
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
		public void Test05_If_ElseIf_Else()
		{
			string code = @"
local x = 5
if x > 10 then
	x = 100
elseif x > 5 then
	x = 50
else
	x = 0
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test05_If_ElseIf_Else_CompileAll()
		{
			string code = @"
local x = 5
if x > 10 then
	x = 100
elseif x > 5 then
	x = 50
else
	x = 0
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_If_ElseIf_Else_ElseIf()
		{
			string code = @"
local x = 7
if x > 10 then
	x = 100
elseif x > 8 then
	x = 90
elseif x > 5 then
	x = 50
else
	x = 0
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(50L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_If_ElseIf_Else_ElseIf_CompileAll()
		{
			string code = @"
local x = 7
if x > 10 then
	x = 100
elseif x > 8 then
	x = 90
elseif x > 5 then
	x = 50
else
	x = 0
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(50L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_If_ElseIf_Only()
		{
			string code = @"
local x = 7
if x > 10 then
	x = 100
elseif x > 5 then
	x = 50
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(50L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_If_ElseIf_Only_CompileAll()
		{
			string code = @"
local x = 7
if x > 10 then
	x = 100
elseif x > 5 then
	x = 50
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(50L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_If_ElseIf_None_Match()
		{
			string code = @"
local x = 3
if x > 10 then
	x = 100
elseif x > 8 then
	x = 90
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_If_ElseIf_None_Match_CompileAll()
		{
			string code = @"
local x = 3
if x > 10 then
	x = 100
elseif x > 8 then
	x = 90
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_If_And_Condition()
		{
			string code = @"
local x = 10
if x > 5 and x < 15 then
	x = 100
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_If_And_Condition_CompileAll()
		{
			string code = @"
local x = 10
if x > 5 and x < 15 then
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
		public void Test10_If_Or_Condition()
		{
			string code = @"
local x = 3
if x > 10 or x < 5 then
	x = 100
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test10_If_Or_Condition_CompileAll()
		{
			string code = @"
local x = 3
if x > 10 or x < 5 then
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
		public void Test11_If_Not_Condition()
		{
			string code = @"
local x = false
if not x then
	x = true
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(true, script.Eval(code));
		}

		[TestMethod]
		public void Test11_If_Not_Condition_CompileAll()
		{
			string code = @"
local x = false
if not x then
	x = true
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(true, script.Eval(code));
		}

		[TestMethod]
		public void Test12_If_Nested()
		{
			string code = @"
local x = 10
if x > 5 then
	if x > 8 then
		x = 100
	else
		x = 50
	end
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test12_If_Nested_CompileAll()
		{
			string code = @"
local x = 10
if x > 5 then
	if x > 8 then
		x = 100
	else
		x = 50
	end
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_If_Nil_False()
		{
			string code = @"
local x = nil
if x then
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
		public void Test13_If_Nil_False_CompileAll()
		{
			string code = @"
local x = nil
if x then
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
		public void Test14_If_Zero_True()
		{
			string code = @"
local x = 0
if x then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test14_If_Zero_True_CompileAll()
		{
			string code = @"
local x = 0
if x then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test15_If_Empty_String_True()
		{
			string code = @"
local x = ''
if x then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test15_If_Empty_String_True_CompileAll()
		{
			string code = @"
local x = ''
if x then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_If_Equals_String()
		{
			string code = @"
local x = 'hello'
if x == 'hello' then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_If_Equals_String_CompileAll()
		{
			string code = @"
local x = 'hello'
if x == 'hello' then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test17_If_Not_Equals()
		{
			string code = @"
local x = 5
if x ~= 10 then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test17_If_Not_Equals_CompileAll()
		{
			string code = @"
local x = 5
if x ~= 10 then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test18_If_Multi_Statement_Body()
		{
			string code = @"
local x = 10
if x > 5 then
	local a = 1
	local b = 2
	x = a + b + 100
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(103L, script.Eval(code));
		}

		[TestMethod]
		public void Test18_If_Multi_Statement_Body_CompileAll()
		{
			string code = @"
local x = 10
if x > 5 then
	local a = 1
	local b = 2
	x = a + b + 100
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(103L, script.Eval(code));
		}

		[TestMethod]
		public void Test19_If_Return_Function()
		{
			string code = @"
function test(x)
	if x > 10 then
		return 100
	elseif x > 5 then
		return 50
	else
		return 0
	end
end
test(7)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(50L, script.Eval(code));
		}

		[TestMethod]
		public void Test19_If_Return_Function_CompileAll()
		{
			string code = @"
function test(x)
	if x > 10 then
		return 100
	elseif x > 5 then
		return 50
	else
		return 0
	end
end
test(7)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(50L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_If_Comparison_Operators()
		{
			string code = @"
local x = 10
if x >= 10 and x <= 10 then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_If_Comparison_Operators_CompileAll()
		{
			string code = @"
local x = 10
if x >= 10 and x <= 10 then
	x = 100
else
	x = 200
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}
	}
}
