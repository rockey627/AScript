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
	public class LuaForGenericTest
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

		#region 基本遍历测试

		[TestMethod]
		public void Test01_For_Generic_Basic_Array()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3, 4, 5}) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test01_For_Generic_Basic_Array_CompileAll()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3, 4, 5}) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_For_Generic_Single_Variable()
		{
			string code = @"
local sum = 0
for v in ipairs({1, 2, 3}) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code)); // 1+2+3 = 6
		}

		[TestMethod]
		public void Test02_For_Generic_Single_Variable_CompileAll()
		{
			string code = @"
local sum = 0
for v in ipairs({1, 2, 3}) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_For_Generic_Index_Value()
		{
			string code = @"
local result = ''
for i, v in ipairs({10, 20, 30}) do
	result = result .. i .. ':' .. v .. ';'
end
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1:10;2:20;3:30;", script.Eval(code));
		}

		[TestMethod]
		public void Test03_For_Generic_Index_Value_CompileAll()
		{
			string code = @"
local result = ''
for i, v in ipairs({10, 20, 30}) do
	result = result .. i .. ':' .. v .. ';'
end
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1:10;2:20;3:30;", script.Eval(code));
		}

		#endregion

		#region 空集合和单元素

		[TestMethod]
		public void Test04_For_Generic_Empty_Table()
		{
			string code = @"
local sum = 0
for i, v in ipairs({}) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test04_For_Generic_Empty_Table_CompileAll()
		{
			string code = @"
local sum = 0
for i, v in ipairs({}) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test05_For_Generic_Single_Element()
		{
			string code = @"
local sum = 0
for i, v in ipairs({100}) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test05_For_Generic_Single_Element_CompileAll()
		{
			string code = @"
local sum = 0
for i, v in ipairs({100}) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		#endregion

		#region 循环控制

		[TestMethod]
		public void Test06_For_Generic_Break()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3, 4, 5, 6, 7, 8, 9, 10}) do
	if v > 5 then
		break
	end
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code)); // 1+2+3+4+5 = 15
		}

		[TestMethod]
		public void Test06_For_Generic_Break_CompileAll()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3, 4, 5, 6, 7, 8, 9, 10}) do
	if v > 5 then
		break
	end
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_For_Generic_Continue()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3, 4, 5}) do
	if v % 2 == 0 then
		continue
	end
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(9L, script.Eval(code)); // 1+3+5 = 9
		}

		[TestMethod]
		public void Test07_For_Generic_Continue_CompileAll()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3, 4, 5}) do
	if v % 2 == 0 then
		continue
	end
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(9L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_For_Generic_Nested()
		{
			string code = @"
local sum = 0
for i, outer in ipairs({1, 2, 3}) do
	for j, inner in ipairs({1, 2}) do
		sum = sum + (outer * inner)
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(18L, script.Eval(code)); // (1*1+1*2) + (2*1+2*2) + (3*1+3*2) = 3+6+9 = 18
		}

		[TestMethod]
		public void Test08_For_Generic_Nested_CompileAll()
		{
			string code = @"
local sum = 0
for i, outer in ipairs({1, 2, 3}) do
	for j, inner in ipairs({1, 2}) do
		sum = sum + (outer * inner)
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
		public void Test09_For_Generic_Return_In_Function()
		{
			string code = @"
function findThird(arr)
	for i, v in ipairs(arr) do
		if i == 3 then
			return v
		end
	end
	return 0
end
findThird({10, 20, 30, 40, 50})
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_For_Generic_Return_In_Function_CompileAll()
		{
			string code = @"
function findThird(arr)
	for i, v in ipairs(arr) do
		if i == 3 then
			return v
		end
	end
	return 0
end
findThird({10, 20, 30, 40, 50})
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code));
		}

		#endregion

		#region 循环体操作

		[TestMethod]
		public void Test10_For_Generic_Empty_Body()
		{
			string code = @"
local x = 10
for i, v in ipairs({1, 2, 3}) do
end
x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test10_For_Generic_Empty_Body_CompileAll()
		{
			string code = @"
local x = 10
for i, v in ipairs({1, 2, 3}) do
end
x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test11_For_Generic_Multi_Statement_Body()
		{
			string code = @"
local result = ''
for i, v in ipairs({1, 2, 3}) do
	local doubled = v * 2
	local tripled = v * 3
	result = result .. doubled .. ',' .. tripled .. ';'
end
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("2,3;4,6;6,9;", script.Eval(code));
		}

		[TestMethod]
		public void Test11_For_Generic_Multi_Statement_Body_CompileAll()
		{
			string code = @"
local result = ''
for i, v in ipairs({1, 2, 3}) do
	local doubled = v * 2
	local tripled = v * 3
	result = result .. doubled .. ',' .. tripled .. ';'
end
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("2,3;4,6;6,9;", script.Eval(code));
		}

		[TestMethod]
		public void Test12_For_Generic_Modify_Variable_Inside_Loop()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3, 4, 5}) do
	v = v + 10
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			// Modifying v inside the loop should not affect the iteration
			Assert.AreEqual(65L, script.Eval(code)); // Still sums original values
		}

		[TestMethod]
		public void Test12_For_Generic_Modify_Variable_Inside_Loop_CompileAll()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3, 4, 5}) do
	v = v + 10
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(65L, script.Eval(code));
		}

		#endregion

		#region 字符串遍历

		[TestMethod]
		public void Test13_For_Generic_String_Array()
		{
			string code = @"
local result = ''
for i, v in ipairs({'a', 'b', 'c'}) do
	result = result .. v
end
result
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abc", script.Eval(code));
		}

		[TestMethod]
		public void Test13_For_Generic_String_Array_CompileAll()
		{
			string code = @"
local result = ''
for i, v in ipairs({'a', 'b', 'c'}) do
	result = result .. v
end
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abc", script.Eval(code));
		}

		[TestMethod]
		public void Test14_For_Generic_Mixed_Types()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 'hello', 3.14, true, 5}) do
	if type(v) == 'number' then
		sum = sum + v
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(9.14, script.Eval(code));
		}

		[TestMethod]
		public void Test14_For_Generic_Mixed_Types_CompileAll()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 'hello', 3.14, true, 5}) do
	if type(v) == 'number' then
		sum = sum + v
	end
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(9.14, script.Eval(code));
		}

		#endregion

		#region 外部变量在循环后的值

		[TestMethod]
		public void Test15_For_Generic_Variable_After_Loop()
		{
			string code = @"
local last = 0
for i, v in ipairs({10, 20, 30}) do
	last = v
end
last
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void Test15_For_Generic_Variable_After_Loop_CompileAll()
		{
			string code = @"
local last = 0
for i, v in ipairs({10, 20, 30}) do
	last = v
end
last
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_For_Generic_Index_After_Loop()
		{
			string code = @"
local lastIndex = 0
for i, v in ipairs({10, 20, 30}) do
	lastIndex = i
end
lastIndex
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_For_Generic_Index_After_Loop_CompileAll()
		{
			string code = @"
local lastIndex = 0
for i, v in ipairs({10, 20, 30}) do
	lastIndex = i
end
lastIndex
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		#endregion

		#region 嵌套 break/continue

		[TestMethod]
		public void Test17_For_Generic_Nested_Break_Inner()
		{
			string code = @"
local sum = 0
for i, outer in ipairs({1, 2, 3, 4, 5}) do
	for j, inner in ipairs({1, 2, 3, 4, 5}) do
		if inner > 2 then
			break
		end
		sum = sum + 1
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test17_For_Generic_Nested_Break_Inner_CompileAll()
		{
			string code = @"
local sum = 0
for i, outer in ipairs({1, 2, 3, 4, 5}) do
	for j, inner in ipairs({1, 2, 3, 4, 5}) do
		if inner > 2 then
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
		public void Test18_For_Generic_Break_Outer()
		{
			string code = @"
local sum = 0
for i, outer in ipairs({1, 2, 3}) do
	if outer == 2 then
		break
	end
	for j, inner in ipairs({1, 2, 3}) do
		sum = sum + inner
	end
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code)); // Only first outer iteration runs (1+2+3)
		}

		[TestMethod]
		public void Test18_For_Generic_Break_Outer_CompileAll()
		{
			string code = @"
local sum = 0
for i, outer in ipairs({1, 2, 3}) do
	if outer == 2 then
		break
	end
	for j, inner in ipairs({1, 2, 3}) do
		sum = sum + inner
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

		#region 循环变量作用域

		[TestMethod]
		public void Test19_For_Generic_Local_Variable_Scope()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3}) do
	local temp = v * 2
	sum = sum + temp
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(12L, script.Eval(code)); // (1*2)+(2*2)+(3*2) = 12
		}

		[TestMethod]
		public void Test19_For_Generic_Local_Variable_Scope_CompileAll()
		{
			string code = @"
local sum = 0
for i, v in ipairs({1, 2, 3}) do
	local temp = v * 2
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
		public void Test20_For_Generic_Count_Elements()
		{
			string code = @"
local count = 0
for i, v in ipairs({1, 2, 3, 4, 5, 6, 7, 8, 9, 10}) do
	count = count + 1
end
count
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_For_Generic_Count_Elements_CompileAll()
		{
			string code = @"
local count = 0
for i, v in ipairs({1, 2, 3, 4, 5, 6, 7, 8, 9, 10}) do
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
		[TestMethod]
		public void Test21()
		{
			var code = @"
a = {""one"", a1=""two"", ""three""}
x1=a[1]
print(x1) -- 'one'
a['ok']='ok4'
x2=a['ok']
print(x2) --'ok4'
x3=a['a1']
print(x3) -- 'two'
--a[2]='2'
a[10]='hi10'
x4=a[10]
print(x4) -- 'hi10'
x5=''
for i, v in ipairs(a) do
	x5=x5..i..' '..v..' '
    print(i, v) -- 1 one \n 2 three
end 
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			script.Eval(code);
			Assert.AreEqual("one", script.Eval("x1"));
			Assert.AreEqual("ok4", script.Eval("x2"));
			Assert.AreEqual("two", script.Eval("x3"));
			Assert.AreEqual("hi10", script.Eval("x4"));
			Assert.AreEqual("1 one 2 three ", script.Eval("x5"));
		}
		
	}
}
