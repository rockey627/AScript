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
	public class LuaRepeatTest
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

		#region 基本repeat-until循环

		[TestMethod]
		public void Test01_Repeat_Basic_Count()
		{
			string code = @"
local i = 0
repeat
	i = i + 1
until i >= 5
i
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test01_Repeat_Basic_Count_CompileAll()
		{
			string code = @"
local i = 0
repeat
	i = i + 1
until i >= 5
i
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_Repeat_Condition_True_Immediately()
		{
			string code = @"
local x = 100
repeat
	x = 999
until true
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// repeat至少执行一次，然后until true时退出
			Assert.AreEqual(999L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_Repeat_Condition_True_Immediately_CompileAll()
		{
			string code = @"
local x = 100
repeat
	x = 999
until true
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(999L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_Repeat_Infinite_Loop_With_Break()
		{
			string code = @"
local i = 0
repeat
	i = i + 1
	if i >= 10 then
		break
	end
until false
i
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_Repeat_Infinite_Loop_With_Break_CompileAll()
		{
			string code = @"
local i = 0
repeat
	i = i + 1
	if i >= 10 then
		break
	end
until false
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
		public void Test04_Repeat_Break()
		{
			string code = @"
local sum = 0
local i = 1
repeat
	if i > 5 then
		break
	end
	sum = sum + i
	i = i + 1
until false
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test04_Repeat_Break_CompileAll()
		{
			string code = @"
local sum = 0
local i = 1
repeat
	if i > 5 then
		break
	end
	sum = sum + i
	i = i + 1
until false
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test05_Repeat_Continue()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i % 2 == 0 then
		continue
	end
	sum = sum + i
until i >= 10
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(36L, script.Eval(code)); // 1+3+5+7+9+11 = 36
		}

		[TestMethod]
		public void Test05_Repeat_Continue_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i % 2 == 0 then
		continue
	end
	sum = sum + i
until i >= 10
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(36L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_Repeat_Nested_Break_Inner()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	local j = 0
	repeat
		j = j + 1
		if j > 2 then
			break
		end
		sum = sum + 1
	until false
until i >= 5
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code)); // 5 iterations * 2 inner iterations = 10
		}

		[TestMethod]
		public void Test06_Repeat_Nested_Break_Inner_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	local j = 0
	repeat
		j = j + 1
		if j > 2 then
			break
		end
		sum = sum + 1
	until false
until i >= 5
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_Repeat_Break_Outer()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i == 3 then
		break
	end
	local j = 0
	repeat
		j = j + 1
		sum = sum + 1
	until true
until false
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(2L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_Repeat_Break_Outer_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i == 3 then
		break
	end
	local j = 0
	repeat
		j = j + 1
		sum = sum + 1
	until true
until false
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(2L, script.Eval(code));
		}

		#endregion

		#region 至少执行一次（与while的区别）

		[TestMethod]
		public void Test08_Repeat_Executes_At_Least_Once()
		{
			string code = @"
local x = 10
repeat
	x = x + 1
until x < 100
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(11L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_Repeat_Executes_At_Least_Once_CompileAll()
		{
			string code = @"
local x = 10
repeat
	x = x + 1
until x < 100
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(11L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_While_Might_Not_Execute()
		{
			string code = @"
local x = 10
while x > 100 do
	x = x + 1
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// while条件初始为false，循环体不执行，x保持为10
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_While_Might_Not_Execute_CompileAll()
		{
			string code = @"
local x = 10
while x > 100 do
	x = x + 1
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		#endregion

		#region 空循环体

		[TestMethod]
		public void Test10_Repeat_Empty_Body()
		{
			string code = @"
local x = 10
repeat
until x < 100
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// 空循环体，x不变，最终x=10
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test10_Repeat_Empty_Body_CompileAll()
		{
			string code = @"
local x = 10
repeat
until x < 100
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test11_Repeat_Empty_Body_True_Condition()
		{
			string code = @"
local x = 10
repeat
until true
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// 空循环体，但until true立即退出，x保持为10
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test11_Repeat_Empty_Body_True_Condition_CompileAll()
		{
			string code = @"
local x = 10
repeat
until true
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		#endregion

		#region 嵌套repeat循环

		[TestMethod]
		public void Test12_Repeat_Nested()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	local j = 0
	repeat
		j = j + 1
		sum = sum + (i * j)
	until true
until i >= 3
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test12_Repeat_Nested_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	local j = 0
	repeat
		j = j + 1
		sum = sum + (i * j)
	until true
until i >= 3
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_Repeat_Nested_Continue()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i == 2 then
		continue
	end
	local j = 0
	repeat
		j = j + 1
		sum = sum + 1
	until true
until i >= 3
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(2L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_Repeat_Nested_Continue_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i == 2 then
		continue
	end
	local j = 0
	repeat
		j = j + 1
		sum = sum + 1
	until true
until i >= 3
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(2L, script.Eval(code));
		}

		#endregion

		#region 字符串和混合类型

		[TestMethod]
		public void Test14_Repeat_String_Concat()
		{
			string code = @"
local result = ''
local i = 0
repeat
	i = i + 1
	result = result .. 'a'
until i >= 3
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("aaa", script.Eval(code));
		}

		[TestMethod]
		public void Test14_Repeat_String_Concat_CompileAll()
		{
			string code = @"
local result = ''
local i = 0
repeat
	i = i + 1
	result = result .. 'a'
until i >= 3
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("aaa", script.Eval(code));
		}

		[TestMethod]
		public void Test15_Repeat_Mixed_Types()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if type(i) == 'number' then
		sum = sum + i
	end
until i >= 5
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test15_Repeat_Mixed_Types_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if type(i) == 'number' then
		sum = sum + i
	end
until i >= 5
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
		public void Test16_Repeat_Variable_After_Loop()
		{
			string code = @"
local last = 0
local i = 0
repeat
	i = i + 1
	last = i
until i >= 10
last
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_Repeat_Variable_After_Loop_CompileAll()
		{
			string code = @"
local last = 0
local i = 0
repeat
	i = i + 1
	last = i
until i >= 10
last
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test17_Repeat_Counter_After_Early_Break()
		{
			string code = @"
local counter = 0
local i = 0
repeat
	i = i + 1
	counter = counter + 1
	if i >= 5 then
		break
	end
until false
counter
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test17_Repeat_Counter_After_Early_Break_CompileAll()
		{
			string code = @"
local counter = 0
local i = 0
repeat
	i = i + 1
	counter = counter + 1
	if i >= 5 then
		break
	end
until false
counter
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		#endregion

		#region 函数内使用repeat

		[TestMethod]
		public void Test18_Repeat_Return_In_Function()
		{
			string code = @"
function findFifth(max)
	local i = 0
	repeat
		i = i + 1
		if i == 5 then
			return i
		end
	until i >= max
	return 0
end
findFifth(100)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test18_Repeat_Return_In_Function_CompileAll()
		{
			string code = @"
function findFifth(max)
	local i = 0
	repeat
		i = i + 1
		if i == 5 then
			return i
		end
	until i >= max
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
		public void Test19_Repeat_Return_After_Loop()
		{
			string code = @"
function countTo(target)
	local i = 0
	local count = 0
	repeat
		i = i + 1
		count = count + i
	until i >= target
	return count
end
countTo(5)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test19_Repeat_Return_After_Loop_CompileAll()
		{
			string code = @"
function countTo(target)
	local i = 0
	local count = 0
	repeat
		i = i + 1
		count = count + i
	until i >= target
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

		#region repeat与table结合

		[TestMethod]
		public void Test20_Repeat_With_Table_Iteration()
		{
			string code = @"
local t = {10, 20, 30}
local sum = 0
local i = 1
repeat
	sum = sum + t[i]
	i = i + 1
until i > 3
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_Repeat_With_Table_Iteration_CompileAll()
		{
			string code = @"
local t = {10, 20, 30}
local sum = 0
local i = 1
repeat
	sum = sum + t[i]
	i = i + 1
until i > 3
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test21_Repeat_Table_Build()
		{
			string code = @"
local t = {}
local i = 1
repeat
	t[i] = i * 10
	i = i + 1
until i > 3
t[1] + t[2] + t[3]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test21_Repeat_Table_Build_CompileAll()
		{
			string code = @"
local t = {}
local i = 1
repeat
	t[i] = i * 10
	i = i + 1
until i > 3
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
		public void Test22_Repeat_Complex_Condition()
		{
			string code = @"
local a = 0
local b = 10
repeat
	a = a + 1
	b = b - 1
until a >= 5 or b <= 5
a * 100 + b
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// a=5, b=5 -> 5*100+5 = 505
			Assert.AreEqual(505L, script.Eval(code));
		}

		[TestMethod]
		public void Test22_Repeat_Complex_Condition_CompileAll()
		{
			string code = @"
local a = 0
local b = 10
repeat
	a = a + 1
	b = b - 1
until a >= 5 or b <= 5
a * 100 + b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(505L, script.Eval(code));
		}

		[TestMethod]
		public void Test23_Repeat_And_Condition()
		{
			string code = @"
local count = 0
local x = 0
repeat
	x = x + 1
	count = count + 1
	if x >= 3 then
		x = 0
	end
until count >= 10
count
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// This loop will run until count >= 10
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test23_Repeat_And_Condition_CompileAll()
		{
			string code = @"
local count = 0
local x = 0
repeat
	x = x + 1
	count = count + 1
	if x >= 3 then
		x = 0
	end
until count >= 10
count
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		#endregion

		#region 递减循环

		[TestMethod]
		public void Test24_Repeat_Countdown()
		{
			string code = @"
local result = ''
local i = 5
repeat
	result = result .. i
	i = i - 1
until i <= 0
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("54321", script.Eval(code));
		}

		[TestMethod]
		public void Test24_Repeat_Countdown_CompileAll()
		{
			string code = @"
local result = ''
local i = 5
repeat
	result = result .. i
	i = i - 1
until i <= 0
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("54321", script.Eval(code));
		}

		[TestMethod]
		public void Test25_Repeat_Step_Countdown()
		{
			string code = @"
local result = ''
local i = 10
repeat
	result = result .. i
	i = i - 2
until i <= 0
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("108642", script.Eval(code));
		}

		[TestMethod]
		public void Test25_Repeat_Step_Countdown_CompileAll()
		{
			string code = @"
local result = ''
local i = 10
repeat
	result = result .. i
	i = i - 2
until i <= 0
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
		public void Test26_Repeat_Factorial()
		{
			string code = @"
function factorial(n)
	local result = 1
	local i = 1
	repeat
		result = result * i
		i = i + 1
	until i > n
	return result
end
factorial(5)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(120L, script.Eval(code));
		}

		[TestMethod]
		public void Test26_Repeat_Factorial_CompileAll()
		{
			string code = @"
function factorial(n)
	local result = 1
	local i = 1
	repeat
		result = result * i
		i = i + 1
	until i > n
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
		public void Test27_Repeat_Fibonacci()
		{
			string code = @"
function fib(n)
	local a = 0
	local b = 1
	local i = 0
	repeat
		local temp = a
		a = b
		b = temp + b
		i = i + 1
	until i >= n
	return a
end
fib(10)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(55L, script.Eval(code));
		}

		[TestMethod]
		public void Test27_Repeat_Fibonacci_CompileAll()
		{
			string code = @"
function fib(n)
	local a = 0
	local b = 1
	local i = 0
	repeat
		local temp = a
		a = b
		b = temp + b
		i = i + 1
	until i >= n
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
		public void Test28_Repeat_With_If_Inside()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i % 2 == 1 then
		sum = sum + i
	end
until i >= 10
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(25L, script.Eval(code)); // 1+3+5+7+9 = 25
		}

		[TestMethod]
		public void Test28_Repeat_With_If_Inside_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i % 2 == 1 then
		sum = sum + i
	end
until i >= 10
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(25L, script.Eval(code));
		}

		[TestMethod]
		public void Test29_Repeat_Multiple_Continue()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i == 3 then
		continue
	end
	if i == 7 then
		continue
	end
	sum = sum + 1
until i >= 10
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(8L, script.Eval(code)); // Skips 3 and 7, so 10-2 = 8
		}

		[TestMethod]
		public void Test29_Repeat_Multiple_Continue_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	if i == 3 then
		continue
	end
	if i == 7 then
		continue
	end
	sum = sum + 1
until i >= 10
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(8L, script.Eval(code));
		}

		#endregion

		#region 循环变量作用域

		[TestMethod]
		public void Test30_Repeat_Local_Variable_Scope()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	local temp = i * 2
	sum = sum + temp
until i >= 3
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(12L, script.Eval(code)); // (1*2)+(2*2)+(3*2) = 12
		}

		[TestMethod]
		public void Test30_Repeat_Local_Variable_Scope_CompileAll()
		{
			string code = @"
local sum = 0
local i = 0
repeat
	i = i + 1
	local temp = i * 2
	sum = sum + temp
until i >= 3
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(12L, script.Eval(code));
		}

		[TestMethod]
		public void Test31_Repeat_Count_Elements()
		{
			string code = @"
local count = 0
local i = 0
repeat
	i = i + 1
	count = count + 1
until i >= 10
count
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test31_Repeat_Count_Elements_CompileAll()
		{
			string code = @"
local count = 0
local i = 0
repeat
	i = i + 1
	count = count + 1
until i >= 10
count
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		#endregion
	}
}
