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
	public class LuaForNumberTest
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

		#region 基本整数循环

		[TestMethod]
		public void Test01_For_Number_Basic_Positive_Step()
		{
			string code = @"
local sum = 0
for i = 1, 5 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test01_For_Number_Basic_Positive_Step_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 5 do
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
		public void Test02_For_Number_Basic_Single_Iteration()
		{
			string code = @"
local sum = 0
for i = 3, 3 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_For_Number_Basic_Single_Iteration_CompileAll()
		{
			string code = @"
local sum = 0
for i = 3, 3 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_For_Number_Zero_To_Negative()
		{
			string code = @"
local sum = 0
for i = 0, -5 do
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code)); // 0到-5，step默认1，不会执行
		}

		[TestMethod]
		public void Test03_For_Number_Zero_To_Negative_CompileAll()
		{
			string code = @"
local sum = 0
for i = 0, -5 do
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		#endregion

		#region 带 Step 的循环

		[TestMethod]
		public void Test04_For_Number_With_Positive_Step()
		{
			string code = @"
local sum = 0
for i = 1, 10, 2 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(25L, script.Eval(code)); // 1+3+5+7+9 = 25
		}

		[TestMethod]
		public void Test04_For_Number_With_Positive_Step_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 10, 2 do
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
		public void Test05_For_Number_With_Negative_Step()
		{
			string code = @"
local sum = 0
for i = 10, 1, -1 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(55L, script.Eval(code)); // 10+9+8+7+6+5+4+3+2+1 = 55
		}

		[TestMethod]
		public void Test05_For_Number_With_Negative_Step_CompileAll()
		{
			string code = @"
local sum = 0
for i = 10, 1, -1 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(55L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_For_Number_With_Negative_Step_Single_Iteration()
		{
			string code = @"
local sum = 0
for i = 5, 5, -1 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_For_Number_With_Negative_Step_Single_Iteration_CompileAll()
		{
			string code = @"
local sum = 0
for i = 5, 5, -1 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_For_Number_With_Negative_Step_No_Iteration()
		{
			string code = @"
local sum = 0
for i = 1, 5, -1 do
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code)); // 1到5，step为-1，不会执行
		}

		[TestMethod]
		public void Test07_For_Number_With_Negative_Step_No_Iteration_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 5, -1 do
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_For_Number_With_Step_Greater_Than_Range()
		{
			string code = @"
local sum = 0
for i = 1, 5, 10 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code)); // 只执行一次
		}

		[TestMethod]
		public void Test08_For_Number_With_Step_Greater_Than_Range_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 5, 10 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		#endregion

		#region 浮点数循环

		[TestMethod]
		public void Test09_For_Number_Float_Positive_Step()
		{
			string code = @"
local sum = 0.0
for i = 1.0, 3.0 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6.0, script.Eval(code));
		}

		[TestMethod]
		public void Test09_For_Number_Float_Positive_Step_CompileAll()
		{
			string code = @"
local sum = 0.0
for i = 1.0, 3.0 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6.0, script.Eval(code));
		}

		[TestMethod]
		public void Test10_For_Number_Float_With_Step()
		{
			string code = @"
local sum = 0.0
for i = 1.0, 2.5, 0.5 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(7.0, script.Eval(code)); // 1.0 + 1.5 + 2.0 + 2.5 = 7.0
		}

		[TestMethod]
		public void Test10_For_Number_Float_With_Step_CompileAll()
		{
			string code = @"
local sum = 0.0
for i = 1.0, 2.5, 0.5 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6.0, script.Eval(code));
		}

		[TestMethod]
		public void Test11_For_Number_Float_Negative_Step()
		{
			string code = @"
local sum = 0.0
for i = 3.0, 1.0, -0.5 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10.0, script.Eval(code)); // 3.0 + 2.5 + 2.0 + 1.5 + 1.0 = 10.0
		}

		[TestMethod]
		public void Test11_For_Number_Float_Negative_Step_CompileAll()
		{
			string code = @"
local sum = 0.0
for i = 3.0, 1.0, -0.5 do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(9.0, script.Eval(code));
		}

		[TestMethod]
		public void Test12_For_Number_Float_Decimal_Start_End()
		{
			string code = @"
local sum = 0
for i = 1.5, 4.5 do
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(4L, script.Eval(code)); // 1.5, 2.5, 3.5, 4.5
		}

		[TestMethod]
		public void Test12_For_Number_Float_Decimal_Start_End_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1.5, 4.5 do
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(4L, script.Eval(code));
		}

		#endregion

		#region 循环控制

		[TestMethod]
		public void Test13_For_Number_Break()
		{
			string code = @"
local sum = 0
for i = 1, 100 do
	if i > 5 then
		break
	end
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test13_For_Number_Break_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 100 do
	if i > 5 then
		break
	end
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
		public void Test14_For_Number_Nested()
		{
			string code = @"
local sum = 0
for i = 1, 3 do
	for j = 1, 2 do
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code)); // 3 * 2 = 6
		}

		[TestMethod]
		public void Test14_For_Number_Nested_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 3 do
	for j = 1, 2 do
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

		[TestMethod]
		public void Test15_For_Number_Nested_Break_Inner()
		{
			string code = @"
local sum = 0
for i = 1, 10 do
	for j = 1, 10 do
		if j > 3 then
			break
		end
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code)); // 10 * 3 = 30
		}

		[TestMethod]
		public void Test15_For_Number_Nested_Break_Inner_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 10 do
	for j = 1, 10 do
		if j > 3 then
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
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_For_Number_Return_In_Function()
		{
			string code = @"
function test()
	for i = 1, 100 do
		if i == 10 then
			return i
		end
	end
	return 0
end
test()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_For_Number_Return_In_Function_CompileAll()
		{
			string code = @"
function test()
	for i = 1, 100 do
		if i == 10 then
			return i
		end
	end
	return 0
end
test()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		#endregion

		#region 循环体操作

		[TestMethod]
		public void Test17_For_Number_Modify_Loop_Variable()
		{
			string code = @"
local sum = 0
for i = 1, 5 do
	i = i + 10
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// 循环变量在Lua中重新赋值不会影响循环的迭代次数
			Assert.AreEqual(65L, script.Eval(code)); // (11+12+13+14+15) = 65
		}

		[TestMethod]
		public void Test17_For_Number_Modify_Loop_Variable_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 5 do
	i = i + 10
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(65L, script.Eval(code));
		}

		[TestMethod]
		public void Test18_For_Number_Empty_Body()
		{
			string code = @"
local x = 10
for i = 1, 5 do
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test18_For_Number_Empty_Body_CompileAll()
		{
			string code = @"
local x = 10
for i = 1, 5 do
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test19_For_Number_Multi_Statement_Body()
		{
			string code = @"
local result = ''
for i = 1, 3 do
	local a = i * 2
	local b = i * 3
	result = result .. a .. ',' .. b .. ';'
end
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("2,3;4,6;6,9;", script.Eval(code));
		}

		[TestMethod]
		public void Test19_For_Number_Multi_Statement_Body_CompileAll()
		{
			string code = @"
local result = ''
for i = 1, 3 do
	local a = i * 2
	local b = i * 3
	result = result .. a .. ',' .. b .. ';'
end
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("2,3;4,6;6,9;", script.Eval(code));
		}

		#endregion

		#region 表达式作为循环边界

		[TestMethod]
		public void Test20_For_Number_Expression_Start_End()
		{
			string code = @"
local a = 1
local b = 5
local sum = 0
for i = a, b do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_For_Number_Expression_Start_End_CompileAll()
		{
			string code = @"
local a = 1
local b = 5
local sum = 0
for i = a, b do
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
		public void Test21_For_Number_Expression_Step()
		{
			string code = @"
local step = 2
local sum = 0
for i = 1, 10, step do
	sum = sum + i
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(25L, script.Eval(code));
		}

		[TestMethod]
		public void Test21_For_Number_Expression_Step_CompileAll()
		{
			string code = @"
local step = 2
local sum = 0
for i = 1, 10, step do
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
		public void Test22_For_Number_Call_Expression_In_Bound()
		{
			string code = @"
local count = 0
local function getEnd()
	count = count + 1
	return 5
end
local sum = 0
for i = 1, getEnd() do
	sum = sum + 1
end
count
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// getEnd() 只应该在循环开始前被调用一次
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test22_For_Number_Call_Expression_In_Bound_CompileAll()
		{
			string code = @"
local count = 0
local function getEnd()
	count = count + 1
	return 5
end
local sum = 0
for i = 1, getEnd() do
	sum = sum + 1
end
count
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		#endregion

		#region 边界情况

		[TestMethod]
		public void Test23_For_Number_Large_Range()
		{
			string code = @"
local count = 0
for i = 1, 10000 do
	count = count + 1
end
count
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10000L, script.Eval(code));
		}

		[TestMethod]
		public void Test23_For_Number_Large_Range_CompileAll()
		{
			string code = @"
local count = 0
for i = 1, 10000 do
	count = count + 1
end
count
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10000L, script.Eval(code));
		}

		[TestMethod]
		public void Test24_For_Number_Step_One()
		{
			string code = @"
local sum = 0
for i = 1, 1 do
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test24_For_Number_Step_One_CompileAll()
		{
			string code = @"
local sum = 0
for i = 1, 1 do
	sum = sum + 1
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test25_For_Number_Step_Zero()
		{
			string code = @"
local count = 0
for i = 1, 5, 0 do
	count = count + 1
end
count
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test25_For_Number_Step_Zero_CompileAll()
		{
			string code = @"
local count = 0
for i = 1, 5, 0 do
	count = count + 1
end
count
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test26_For_Number_Use_Last_Value()
		{
			string code = @"
local last = 0
for i = 1, 5 do
	last = i
end
last
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test26_For_Number_Use_Last_Value_CompileAll()
		{
			string code = @"
local last = 0
for i = 1, 5 do
	last = i
end
last
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		#endregion
	}
}
