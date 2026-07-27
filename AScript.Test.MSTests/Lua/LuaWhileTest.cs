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
	public class LuaWhileTest
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

		#region 基本while循环

		[TestMethod]
		public void Test01_While_Basic_Count()
		{
			string code = @"
local i = 0
local sum = 0
while i < 5 do
	i = i + 1
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test01_While_Basic_Count_CompileAll()
		{
			string code = @"
local i = 0
local sum = 0
while i < 5 do
	i = i + 1
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_While_Condition_False()
		{
			string code = @"
local x = 100
while false do
	x = 999
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_While_Condition_False_CompileAll()
		{
			string code = @"
local x = 100
while false do
	x = 999
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_While_Infinite_Loop_With_Break()
		{
			string code = @"
local i = 0
while true do
	i = i + 1
	if i >= 10 then
		break
	end
end
i
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_While_Infinite_Loop_With_Break_CompileAll()
		{
			string code = @"
local i = 0
while true do
	i = i + 1
	if i >= 10 then
		break
	end
end
i
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		#endregion

		#region break和continue

		[TestMethod]
		public void Test04_While_Break()
		{
			string code = @"
local sum = 0
local i = 1
while i <= 100 do
	if i > 5 then
		break
	end
	sum = sum + i
	i = i + 1
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test04_While_Break_CompileAll()
		{
			string code = @"
local sum = 0
local i = 1
while i <= 100 do
	if i > 5 then
		break
	end
	sum = sum + i
	i = i + 1
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test05_While_Continue()
		{
			string code = @"
local sum = 0
local i = 0
while i < 10 do
	i = i + 1
	if i % 2 == 0 then
		continue
	end
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(25L, script.Eval(code)); // 1+3+5+7+9 = 25
		}

		[TestMethod]
		public void Test05_While_Continue_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 10 do
	i = i + 1
	if i % 2 == 0 then
		continue
	end
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(25L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_While_Nested_Break_Inner()
		{
			string code = @"
local sum = 0
local i = 0
while i < 5 do
	i = i + 1
	local j = 0
	while j < 5 do
		j = j + 1
		if j > 2 then
			break
		end
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code)); // 5 iterations * 2 inner iterations = 10
		}

		[TestMethod]
		public void Test06_While_Nested_Break_Inner_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 5 do
	i = i + 1
	local j = 0
	while j < 5 do
		j = j + 1
		if j > 2 then
			break
		end
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_While_Break_Outer()
		{
			string code = @"
local sum = 0
local i = 0
while i < 5 do
	i = i + 1
	if i == 3 then
		break
	end
	local j = 0
	while j < 3 do
		j = j + 1
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code)); // i=1: 3, i=2: 3, i=3: break
		}

		[TestMethod]
		public void Test07_While_Break_Outer_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 5 do
	i = i + 1
	if i == 3 then
		break
	end
	local j = 0
	while j < 3 do
		j = j + 1
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code));
		}

		#endregion

		#region 空循环体和多语句

		[TestMethod]
		public void Test08_While_Empty_Body()
		{
			string code = @"
local x = 10
while x > 100 do
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// This will be an infinite loop in practice, but let's test a version that terminates
			// Actually, let's test with a condition that's immediately false
			script.Eval(code);
			// Since while false do end is tested separately, this test should use a terminating condition
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_While_Empty_Body_CompileAll()
		{
			string code = @"
local x = 10
while x > 100 do
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_While_Multi_Statement_Body()
		{
			string code = @"
local i = 0
local result = ''
while i < 3 do
	i = i + 1
	local doubled = i * 2
	local tripled = i * 3
	result = result .. doubled .. ',' .. tripled .. ';'
end
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("2,3;4,6;6,9;", script.Eval(code));
		}

		[TestMethod]
		public void Test09_While_Multi_Statement_Body_CompileAll()
		{
			string code = @"
local i = 0
local result = ''
while i < 3 do
	i = i + 1
	local doubled = i * 2
	local tripled = i * 3
	result = result .. doubled .. ',' .. tripled .. ';'
end
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("2,3;4,6;6,9;", script.Eval(code));
		}

		#endregion

		#region 字符串和混合类型

		[TestMethod]
		public void Test10_While_String_Concat()
		{
			string code = @"
local result = ''
local i = 0
while i < 3 do
	i = i + 1
	result = result .. 'a'
end
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("aaa", script.Eval(code));
		}

		[TestMethod]
		public void Test10_While_String_Concat_CompileAll()
		{
			string code = @"
local result = ''
local i = 0
while i < 3 do
	i = i + 1
	result = result .. 'a'
end
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("aaa", script.Eval(code));
		}

		[TestMethod]
		public void Test11_While_Mixed_Types()
		{
			string code = @"
local sum = 0
local i = 0
while i < 5 do
	i = i + 1
	if type(i) == 'number' then
		sum = sum + i
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test11_While_Mixed_Types_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 5 do
	i = i + 1
	if type(i) == 'number' then
		sum = sum + i
	end
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		#endregion

		#region 循环后变量值

		[TestMethod]
		public void Test12_While_Variable_After_Loop()
		{
			string code = @"
local last = 0
local i = 0
while i < 10 do
	i = i + 1
	last = i
end
last
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test12_While_Variable_After_Loop_CompileAll()
		{
			string code = @"
local last = 0
local i = 0
while i < 10 do
	i = i + 1
	last = i
end
last
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_While_Counter_After_Early_Break()
		{
			string code = @"
local counter = 0
local i = 0
while i < 100 do
	i = i + 1
	counter = counter + 1
	if i >= 5 then
		break
	end
end
counter
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_While_Counter_After_Early_Break_CompileAll()
		{
			string code = @"
local counter = 0
local i = 0
while i < 100 do
	i = i + 1
	counter = counter + 1
	if i >= 5 then
		break
	end
end
counter
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		#endregion

		#region 函数内使用while

		[TestMethod]
		public void Test14_While_Return_In_Function()
		{
			string code = @"
function findFifth(max)
	local i = 0
	while i < max do
		i = i + 1
		if i == 5 then
			return i
		end
	end
	return 0
end
findFifth(100)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test14_While_Return_In_Function_CompileAll()
		{
			string code = @"
function findFifth(max)
	local i = 0
	while i < max do
		i = i + 1
		if i == 5 then
			return i
		end
	end
	return 0
end
findFifth(100)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test15_While_Return_After_Loop()
		{
			string code = @"
function countTo(target)
	local i = 0
	local count = 0
	while i < target do
		i = i + 1
		count = count + i
	end
	return count
end
countTo(5)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test15_While_Return_After_Loop_CompileAll()
		{
			string code = @"
function countTo(target)
	local i = 0
	local count = 0
	while i < target do
		i = i + 1
		count = count + i
	end
	return count
end
countTo(5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		#endregion

		#region 嵌套while循环

		[TestMethod]
		public void Test16_While_Nested()
		{
			string code = @"
local sum = 0
local i = 0
while i < 3 do
	i = i + 1
	local j = 0
	while j < 2 do
		j = j + 1
		sum = sum + (i * j)
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// (1*1+1*2) + (2*1+2*2) + (3*1+3*2) = 3+6+9 = 18
			Assert.AreEqual(18L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_While_Nested_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 3 do
	i = i + 1
	local j = 0
	while j < 2 do
		j = j + 1
		sum = sum + (i * j)
	end
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(18L, script.Eval(code));
		}

		[TestMethod]
		public void Test17_While_Nested_Continue()
		{
			string code = @"
local sum = 0
local i = 0
while i < 3 do
	i = i + 1
	if i == 2 then
		continue
	end
	local j = 0
	while j < 2 do
		j = j + 1
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// i=1: 2, i=2: continue, i=3: 2 -> total = 4
			Assert.AreEqual(4L, script.Eval(code));
		}

		[TestMethod]
		public void Test17_While_Nested_Continue_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 3 do
	i = i + 1
	if i == 2 then
		continue
	end
	local j = 0
	while j < 2 do
		j = j + 1
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(4L, script.Eval(code));
		}

		#endregion

		#region 循环变量作用域

		[TestMethod]
		public void Test18_While_Local_Variable_Scope()
		{
			string code = @"
local sum = 0
local i = 0
while i < 3 do
	i = i + 1
	local temp = i * 2
	sum = sum + temp
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(12L, script.Eval(code)); // (1*2)+(2*2)+(3*2) = 12
		}

		[TestMethod]
		public void Test18_While_Local_Variable_Scope_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 3 do
	i = i + 1
	local temp = i * 2
	sum = sum + temp
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(12L, script.Eval(code));
		}

		[TestMethod]
		public void Test19_While_Count_Elements()
		{
			string code = @"
local count = 0
local i = 0
while i < 10 do
	i = i + 1
	count = count + 1
end
count
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test19_While_Count_Elements_CompileAll()
		{
			string code = @"
local count = 0
local i = 0
while i < 10 do
	i = i + 1
	count = count + 1
end
count
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		#endregion

		#region while与table结合

		[TestMethod]
		public void Test20_While_With_Table_Iteration()
		{
			string code = @"
local t = {10, 20, 30}
local sum = 0
local i = 1
while i <= 3 do
	sum = sum + t[i]
	i = i + 1
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_While_With_Table_Iteration_CompileAll()
		{
			string code = @"
local t = {10, 20, 30}
local sum = 0
local i = 1
while i <= 3 do
	sum = sum + t[i]
	i = i + 1
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test21_While_Table_Build()
		{
			string code = @"
local t = {}
local i = 1
while i <= 3 do
	t[i] = i * 10
	i = i + 1
end
t[1] + t[2] + t[3]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test21_While_Table_Build_CompileAll()
		{
			string code = @"
local t = {}
local i = 1
while i <= 3 do
	t[i] = i * 10
	i = i + 1
end
t[1] + t[2] + t[3]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		#endregion

		#region 复杂条件

		[TestMethod]
		public void Test22_While_Complex_Condition()
		{
			string code = @"
local a = 0
local b = 10
while a < 5 and b > 5 do
	a = a + 1
	b = b - 1
end
a * 100 + b
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(505L, script.Eval(code)); // a=5, b=5 -> 5*100+5 = 505? Wait: 0<5&10>5 T, 1<5&9>5 T, 2<5&8>5 T, 3<5&7>5 T, 4<5&6>5 T, 5<5&5>5 F => a=5, b=5
			// Actually: 5*100+5 = 505
		}

		[TestMethod]
		public void Test22_While_Complex_Condition_CompileAll()
		{
			string code = @"
local a = 0
local b = 10
while a < 5 and b > 5 do
	a = a + 1
	b = b - 1
end
a * 100 + b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(505L, script.Eval(code));
		}

		[TestMethod]
		public void Test23_While_Or_Condition()
		{
			string code = @"
local count = 0
local x = 0
while x < 2 or count < 10 do
	x = x + 1
	count = count + 1
	if x >= 3 then
		x = 0
	end
end
count
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// This loop will run until count >= 10, since x >= 3 resets x to 0 making the condition true again
			Assert.AreEqual(11L, script.Eval(code));
		}

		[TestMethod]
		public void Test23_While_Or_Condition_CompileAll()
		{
			string code = @"
local count = 0
local x = 0
while x < 2 or count < 10 do
	x = x + 1
	count = count + 1
	if x >= 3 then
		x = 0
	end
end
count
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(11L, script.Eval(code));
		}

		#endregion

		#region 递减循环

		[TestMethod]
		public void Test24_While_Countdown()
		{
			string code = @"
local result = ''
local i = 5
while i > 0 do
	result = result .. i
	i = i - 1
end
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("54321", script.Eval(code));
		}

		[TestMethod]
		public void Test24_While_Countdown_CompileAll()
		{
			string code = @"
local result = ''
local i = 5
while i > 0 do
	result = result .. i
	i = i - 1
end
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("54321", script.Eval(code));
		}

		[TestMethod]
		public void Test25_While_Step_Countdown()
		{
			string code = @"
local result = ''
local i = 10
while i >= 1 do
	result = result .. i
	i = i - 2
end
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("108642", script.Eval(code));
		}

		[TestMethod]
		public void Test25_While_Step_Countdown_CompileAll()
		{
			string code = @"
local result = ''
local i = 10
while i >= 1 do
	result = result .. i
	i = i - 2
end
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("108642", script.Eval(code));
		}

		#endregion

		#region 数学运算

		[TestMethod]
		public void Test26_While_Factorial()
		{
			string code = @"
function factorial(n)
	local result = 1
	local i = 1
	while i <= n do
		result = result * i
		i = i + 1
	end
	return result
end
factorial(5)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(120L, script.Eval(code));
		}

		[TestMethod]
		public void Test26_While_Factorial_CompileAll()
		{
			string code = @"
function factorial(n)
	local result = 1
	local i = 1
	while i <= n do
		result = result * i
		i = i + 1
	end
	return result
end
factorial(5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(120L, script.Eval(code));
		}

		[TestMethod]
		public void Test27_While_Fibonacci()
		{
			string code = @"
function fib(n)
	local a = 0
	local b = 1
	local i = 0
	while i < n do
		local temp = a
		a = b
		b = temp + b
		i = i + 1
	end
	return a
end
fib(10)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(55L, script.Eval(code));
		}

		[TestMethod]
		public void Test27_While_Fibonacci_CompileAll()
		{
			string code = @"
function fib(n)
	local a = 0
	local b = 1
	local i = 0
	while i < n do
		local temp = a
		a = b
		b = temp + b
		i = i + 1
	end
	return a
end
fib(10)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(55L, script.Eval(code));
		}

		#endregion

		#region 与其他控制结构结合

		[TestMethod]
		public void Test28_While_With_If_Inside()
		{
			string code = @"
local sum = 0
local i = 0
while i < 10 do
	i = i + 1
	if i % 2 == 1 then
		sum = sum + i
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(25L, script.Eval(code)); // 1+3+5+7+9 = 25
		}

		[TestMethod]
		public void Test28_While_With_If_Inside_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 10 do
	i = i + 1
	if i % 2 == 1 then
		sum = sum + i
	end
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(25L, script.Eval(code));
		}

		[TestMethod]
		public void Test29_While_Multiple_Continue()
		{
			string code = @"
local sum = 0
local i = 0
while i < 10 do
	i = i + 1
	if i == 3 then
		continue
	end
	if i == 7 then
		continue
	end
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(8L, script.Eval(code)); // Skips 3 and 7, so 10-2 = 8
		}

		[TestMethod]
		public void Test29_While_Multiple_Continue_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
while i < 10 do
	i = i + 1
	if i == 3 then
		continue
	end
	if i == 7 then
		continue
	end
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(8L, script.Eval(code));
		}

		#endregion
	}
}
