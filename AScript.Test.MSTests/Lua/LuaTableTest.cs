using AScript.Lang.Lua;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AScript.Test.MSTests.Lua
{
	[TestClass]
	public class LuaTableTest
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
		public void Test00()
		{
			var table = new LuaTable();
			table["Age"] = 18L;
			dynamic dynamicTable = table;
			dynamicTable.Name = "Test";
			Assert.AreEqual("Test", dynamicTable.Name);
			Assert.AreEqual("Test", table["Name"]);
			Assert.AreEqual(18L, dynamicTable.Age);
		}

		#region Table Creation and Access

		[TestMethod]
		public void Test01_Table_Array_Create()
		{
			string code = @"
local arr = {1, 2, 3}
arr
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			var result = script.Eval<LuaTable>(code);
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Array.Count);
			Assert.AreEqual(1L, result.Array[0]);
			Assert.AreEqual(2L, result.Array[1]);
			Assert.AreEqual(3L, result.Array[2]);
		}

		[TestMethod]
		public void Test01_Table_Array_Create_CompileAll()
		{
			string code = @"
local arr = {1, 2, 3}
arr
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			var result = script.Eval<LuaTable>(code);
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Array.Count);
		}

		[TestMethod]
		public void Test02_Table_Array_Access()
		{
			string code = @"
local arr = {10, 20, 30}
arr[1] + arr[2] + arr[3]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_Table_Array_Access_CompileAll()
		{
			string code = @"
local arr = {10, 20, 30}
arr[1] + arr[2] + arr[3]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_Table_KeyValue_Create()
		{
			string code = @"
local t = {a = 1, b = 2}
t
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			var result = script.Eval<LuaTable>(code);
			Assert.IsNotNull(result);
			Assert.AreEqual(1L, result["a"]);
			Assert.AreEqual(2L, result["b"]);
		}

		[TestMethod]
		public void Test03_Table_KeyValue_Create_CompileAll()
		{
			string code = @"
local t = {a = 1, b = 2}
t
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			var result = script.Eval<LuaTable>(code);
			Assert.IsNotNull(result);
			Assert.AreEqual(1L, result["a"]);
		}

		[TestMethod]
		public void Test04_Table_KeyValue_Access()
		{
			string code = @"
local t = {x = 5, y = 10}
t.x + t.y
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test04_Table_KeyValue_Access_CompileAll()
		{
			string code = @"
local t = {x = 5, y = 10}
t.x + t.y
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test05_Table_Mixed()
		{
			string code = @"
local t = {'h1', a=5, 'h2'}
t
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			var result = script.Eval<LuaTable>(code);
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Array.Count);
			Assert.AreEqual("h1", result.Array[0]);
			Assert.AreEqual("h2", result.Array[1]);
			Assert.AreEqual(5L, result["a"]);
		}

		[TestMethod]
		public void Test05_Table_Mixed_CompileAll()
		{
			string code = @"
local t = {'h1', a=5, 'h2'}
t
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			var result = script.Eval<LuaTable>(code);
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Array.Count);
		}

		[TestMethod]
		public void Test06_Table_Empty()
		{
			string code = @"
local t = {}
t
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			var result = script.Eval<LuaTable>(code);
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Array.Count);
		}

		[TestMethod]
		public void Test06_Table_Empty_CompileAll()
		{
			string code = @"
local t = {}
t
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			var result = script.Eval<LuaTable>(code);
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Array.Count);
		}

		#endregion

		#region Table Update

		[TestMethod]
		public void Test07_Table_Update_Array()
		{
			string code = @"
local t = {1, 2, 3}
t[2] = 20
t[2]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(20L, script.Eval(code));
		}

		[TestMethod]
		public void Test07_Table_Update_Array_CompileAll()
		{
			string code = @"
local t = {1, 2, 3}
t[2] = 20
t[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(20L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_Table_Update_KeyValue()
		{
			string code = @"
local t = {a = 1, b = 2}
t.a = 100
t.b = 200
t.a + t.b
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(300L, script.Eval(code));
		}

		[TestMethod]
		public void Test08_Table_Update_KeyValue_CompileAll()
		{
			string code = @"
local t = {a = 1, b = 2}
t.a = 100
t.b = 200
t.a + t.b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(300L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_Table_Add_New_Key()
		{
			string code = @"
local t = {a = 1}
t.b = 2
t.a + t.b
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_Table_Add_New_Key_CompileAll()
		{
			string code = @"
local t = {a = 1}
t.b = 2
t.a + t.b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		#endregion

		#region Nested Table

		[TestMethod]
		public void Test10_Table_Nested()
		{
			string code = @"
local t = {{1, 2}, {3, 4}}
t[1][1]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test10_Table_Nested_CompileAll()
		{
			string code = @"
local t = {{1, 2}, {3, 4}}
t[1][1]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test11_Table_Nested_KeyValue()
		{
			string code = @"
local t = {inner = {x = 10, y = 20}}
t.inner.x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test11_Table_Nested_KeyValue_CompileAll()
		{
			string code = @"
local t = {inner = {x = 10, y = 20}}
t.inner.x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		#endregion

		#region Table as Function Parameter and Return Value

		[TestMethod]
		public void Test12_Table_As_Function_Parameter()
		{
			string code = @"
function sum(t)
	return t[1] + t[2] + t[3]
end
sum({10, 20, 30})
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test12_Table_As_Function_Parameter_CompileAll()
		{
			string code = @"
function sum(t)
	return t[1] + t[2] + t[3]
end
sum({10, 20, 30})
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_Table_As_Return_Value()
		{
			string code = @"
function createPoint(x, y)
	return {x = x, y = y}
end
local p = createPoint(3, 4)
p.x + p.y
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(7L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_Table_As_Return_Value_CompileAll()
		{
			string code = @"
function createPoint(x, y)
	return {x = x, y = y}
end
local p = createPoint(3, 4)
p.x + p.y
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(7L, script.Eval(code));
		}

		#endregion

		#region Table Remove (table.remove)

		[TestMethod]
		public void Test14_Table_Remove_Last()
		{
			string code = @"
local t = {1, 2, 3}
table.remove(t, 3)
t[3]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(null, script.Eval(code));
		}

		[TestMethod]
		public void Test14_Table_Remove_Last_CompileAll()
		{
			string code = @"
local t = {1, 2, 3}
table.remove(t, 3)
t[3]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(null, script.Eval(code));
		}

		[TestMethod]
		public void Test15_Table_Remove_Middle()
		{
			string code = @"
local t = {10, 20, 30}
table.remove(t, 2)
t[1] + t[2]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(40L, script.Eval(code));
		}

		[TestMethod]
		public void Test15_Table_Remove_Middle_CompileAll()
		{
			string code = @"
local t = {10, 20, 30}
table.remove(t, 2)
t[1] + t[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(40L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_Table_Remove_Return_Value()
		{
			string code = @"
local t = {5, 10, 15}
table.remove(t, 2)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test16_Table_Remove_Return_Value_CompileAll()
		{
			string code = @"
local t = {5, 10, 15}
table.remove(t, 2)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test17_Table_Remove_Non_Existent_Key()
		{
			string code = @"
local t = {1, 2, 3}
table.remove(t, 100)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(null, script.Eval(code));
		}

		[TestMethod]
		public void Test17_Table_Remove_Non_Existent_Key_CompileAll()
		{
			string code = @"
local t = {1, 2, 3}
table.remove(t, 100)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(null, script.Eval(code));
		}

		#endregion

		#region Table with Different Value Types

		[TestMethod]
		public void Test18_Table_Mixed_Types()
		{
			string code = @"
local t = {1, 'hello', true, nil}
t[1] + 10
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(11L, script.Eval(code));
		}

		[TestMethod]
		public void Test18_Table_Mixed_Types_CompileAll()
		{
			string code = @"
local t = {1, 'hello', true, nil}
t[1] + 10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(11L, script.Eval(code));
		}

		[TestMethod]
		public void Test19_Table_String_Key()
		{
			string code = @"
local t = {['key with space'] = 100}
t['key with space']
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test19_Table_String_Key_CompileAll()
		{
			string code = @"
local t = {['key with space'] = 100}
t['key with space']
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		#endregion

		#region Table Length with ipairs

		[TestMethod]
		public void Test20_Table_Ipairs()
		{
			string code = @"
local t = {10, 20, 30}
local sum = 0
for i, v in ipairs(t) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_Table_Ipairs_CompileAll()
		{
			string code = @"
local t = {10, 20, 30}
local sum = 0
for i, v in ipairs(t) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test21_Table_Ipairs_With_Remove()
		{
			string code = @"
local t = {5, 10, 15}
table.remove(t, 2)
local sum = 0
for i, v in ipairs(t) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(20L, script.Eval(code));
		}

		[TestMethod]
		public void Test21_Table_Ipairs_With_Remove_CompileAll()
		{
			string code = @"
local t = {5, 10, 15}
table.remove(t, 2)
local sum = 0
for i, v in ipairs(t) do
	sum = sum + v
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(20L, script.Eval(code));
		}

		#endregion

		#region Table Iteration

		[TestMethod]
		public void Test22_Table_While_Loop()
		{
			string code = @"
local t = {1, 2, 3, 4, 5}
local i = 1
local sum = 0
while t[i] do
	sum = sum + t[i]
	i = i + 1
end
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test22_Table_While_Loop_CompileAll()
		{
			string code = @"
local t = {1, 2, 3, 4, 5}
local i = 1
local sum = 0
while t[i] do
	sum = sum + t[i]
	i = i + 1
end
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		#endregion

		#region Complex Table Operations

		[TestMethod]
		public void Test23_Table_Complex_Access()
		{
			string code = @"
local t = {
	a = {b = {c = 123}}
}
t.a.b.c
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(123L, script.Eval(code));
		}

		[TestMethod]
		public void Test23_Table_Complex_Access_CompileAll()
		{
			string code = @"
local t = {
	a = {b = {c = 123}}
}
t.a.b.c
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(123L, script.Eval(code));
		}

		[TestMethod]
		public void Test24_Table_For_Loop()
		{
			string code = @"
local t = {2, 4, 6, 8, 10}
local product = 1
for i = 1, 5 do
	product = product * t[i]
end
product
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3840L, script.Eval(code));
		}

		[TestMethod]
		public void Test24_Table_For_Loop_CompileAll()
		{
			string code = @"
local t = {2, 4, 6, 8, 10}
local product = 1
for i = 1, 5 do
	product = product * t[i]
end
product
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3840L, script.Eval(code));
		}

		[TestMethod]
		public void Test25_Table_If_Condition()
		{
			string code = @"
local t = {value = 10}
local n=-1
if t.value > 5 then
	n=1
else
	n=0
end
n
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test25_Table_If_Condition_CompileAll()
		{
			string code = @"
local t = {value = 10}
local n=-1
if t.value > 5 then
	n=1
else
	n=0
end
n
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test26_Table_Operator_Expression()
		{
			string code = @"
local t = {a = 3, b = 5}
(t.a + t.b) * 2
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(16L, script.Eval(code));
		}

		[TestMethod]
		public void Test26_Table_Operator_Expression_CompileAll()
		{
			string code = @"
local t = {a = 3, b = 5}
(t.a + t.b) * 2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(16L, script.Eval(code));
		}

		#endregion

		#region Table Global

		[TestMethod]
		public void Test27_Table_Global()
		{
			string code = @"
globalData = {x = 100}
local t = globalData
t.x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test27_Table_Global_CompileAll()
		{
			string code = @"
globalData = {x = 100}
local t = globalData
t.x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		#endregion

		#region Table Concat (table.concat)

		[TestMethod]
		public void Test28_Table_Concat_Basic()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abc", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test28_Table_Concat_Basic_CompileAll()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abc", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test29_Table_Concat_With_Separator()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("a,b,c", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test29_Table_Concat_With_Separator_CompileAll()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("a,b,c", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test30_Table_Concat_With_Start_Index()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t, ',', 2)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("b,c", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test30_Table_Concat_With_Start_Index_CompileAll()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t, ',', 2)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("b,c", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test31_Table_Concat_With_Start_And_End_Index()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t, '-', 2, 2)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("b", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test31_Table_Concat_With_Start_And_End_Index_CompileAll()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t, '-', 2, 2)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("b", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test32_Table_Concat_Negative_Index()
		{
			string code = @"
local t = {'a', 'b', 'c', 'd', 'e'}
table.concat(t, ':', -2)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("d:e", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test32_Table_Concat_Negative_Index_CompileAll()
		{
			string code = @"
local t = {'a', 'b', 'c', 'd', 'e'}
table.concat(t, ':', -2)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("d:e", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test33_Table_Concat_Negative_Start_And_End()
		{
			string code = @"
local t = {'a', 'b', 'c', 'd', 'e'}
table.concat(t, '-', -3, -1)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("c-d-e", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test33_Table_Concat_Negative_Start_And_End_CompileAll()
		{
			string code = @"
local t = {'a', 'b', 'c', 'd', 'e'}
table.concat(t, '-', -3, -1)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("c-d-e", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test34_Table_Concat_Empty_Table()
		{
			string code = @"
local t = {}
table.concat(t)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test34_Table_Concat_Empty_Table_CompileAll()
		{
			string code = @"
local t = {}
table.concat(t)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test35_Table_Concat_Start_Greater_Than_End()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t, ',', 3, 1)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test35_Table_Concat_Start_Greater_Than_End_CompileAll()
		{
			string code = @"
local t = {'a', 'b', 'c'}
table.concat(t, ',', 3, 1)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test36_Table_Concat_Single_Element()
		{
			string code = @"
local t = {'only'}
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("only", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test36_Table_Concat_Single_Element_CompileAll()
		{
			string code = @"
local t = {'only'}
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("only", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test37_Table_Concat_Numbers()
		{
			string code = @"
local t = {1, 2, 3, 4, 5}
table.concat(t, '-')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1-2-3-4-5", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test37_Table_Concat_Numbers_CompileAll()
		{
			string code = @"
local t = {1, 2, 3, 4, 5}
table.concat(t, '-')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1-2-3-4-5", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test38_Table_Concat_LuaTable_Direct()
		{
			var table = new LuaTable();
			table[1] = "hello";
			table[2] = "world";
			table[3] = "lua";
			var result = LuaTable.concat(table, " ");
			Assert.AreEqual("hello world lua", result);
		}

		[TestMethod]
		public void Test39_Table_Concat_LuaTable_No_Separator()
		{
			var table = new LuaTable();
			table[1] = "x";
			table[2] = "y";
			table[3] = "z";
			var result = LuaTable.concat(table);
			Assert.AreEqual("xyz", result);
		}

		[TestMethod]
		public void Test40_Table_Concat_LuaTable_With_Range()
		{
			var table = new LuaTable();
			table[1] = "a";
			table[2] = "b";
			table[3] = "c";
			table[4] = "d";
			var result = LuaTable.concat(table, ":", 2, 3);
			Assert.AreEqual("b:c", result);
		}

		[TestMethod]
		public void Test41_Table_Concat_LuaTable_Negative_Indices()
		{
			var table = new LuaTable();
			table[1] = "a";
			table[2] = "b";
			table[3] = "c";
			var result = LuaTable.concat(table, "-", -2, -1);
			Assert.AreEqual("b-c", result);
		}

		[TestMethod]
		public void Test42_Table_Concat_With_String_Number_Mix()
		{
			string code = @"
local t = {'a', 1, 'b', 2, 'c'}
table.concat(t, '')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("a1b2c", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test42_Table_Concat_With_String_Number_Mix_CompileAll()
		{
			string code = @"
local t = {'a', 1, 'b', 2, 'c'}
table.concat(t, '')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("a1b2c", script.Eval<string>(code));
		}

		#endregion

		#region Table Insert (table.insert)

		[TestMethod]
		public void Test43_Table_Insert_End()
		{
			string code = @"
local t = {1, 2, 3}
table.insert(t, 4)
t[4]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(4L, script.Eval(code));
		}

		[TestMethod]
		public void Test43_Table_Insert_End_CompileAll()
		{
			string code = @"
local t = {1, 2, 3}
table.insert(t, 4)
t[4]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(4L, script.Eval(code));
		}

		[TestMethod]
		public void Test44_Table_Insert_Position()
		{
			string code = @"
local t = {1, 3, 4}
table.insert(t, 2, 2)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3,4", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test44_Table_Insert_Position_CompileAll()
		{
			string code = @"
local t = {1, 3, 4}
table.insert(t, 2, 2)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3,4", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test45_Table_Insert_First()
		{
			string code = @"
local t = {2, 3, 4}
table.insert(t, 1, 1)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3,4", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test45_Table_Insert_First_CompileAll()
		{
			string code = @"
local t = {2, 3, 4}
table.insert(t, 1, 1)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3,4", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test46_Table_Insert_String()
		{
			string code = @"
local t = {'a', 'c'}
table.insert(t, 2, 'b')
table.concat(t, '')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abc", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test46_Table_Insert_String_CompileAll()
		{
			string code = @"
local t = {'a', 'c'}
table.insert(t, 2, 'b')
table.concat(t, '')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("abc", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test47_Table_Insert_Multiple()
		{
			string code = @"
local t = {}
table.insert(t, 1)
table.insert(t, 2)
table.insert(t, 3)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test47_Table_Insert_Multiple_CompileAll()
		{
			string code = @"
local t = {}
table.insert(t, 1)
table.insert(t, 2)
table.insert(t, 3)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test48_Table_Insert_LuaTable_Direct()
		{
			var table = new LuaTable();
			table[1] = "a";
			table[2] = "c";
			LuaTable.insert(table, 2, "b");
			Assert.AreEqual("a", table[1]);
			Assert.AreEqual("b", table[2]);
			Assert.AreEqual("c", table[3]);
		}

		[TestMethod]
		public void Test49_Table_Insert_LuaTable_End()
		{
			var table = new LuaTable();
			table[1] = "x";
			table[2] = "y";
			LuaTable.insert(table, "z");
			Assert.AreEqual("x", table[1]);
			Assert.AreEqual("y", table[2]);
			Assert.AreEqual("z", table[3]);
			Assert.AreEqual(3, table.Array.Count);
		}

		[TestMethod]
		public void Test50_Table_Insert_Mixed_Types()
		{
			string code = @"
local t = {1, 'two', true}
table.insert(t, 4, nil)
t[4] = 'four'
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,two,true,four", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test50_Table_Insert_Mixed_Types_CompileAll()
		{
			string code = @"
local t = {1, 'two', true}
table.insert(t, 4, nil)
t[4] = 'four'
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,two,true,four", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test51_Table_Insert_Position_At_End()
		{
			string code = @"
local t = {'a', 'b'}
table.insert(t, 3, 'c')
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("a,b,c", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test51_Table_Insert_Position_At_End_CompileAll()
		{
			string code = @"
local t = {'a', 'b'}
table.insert(t, 3, 'c')
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("a,b,c", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test52_Table_Insert_Return_Value()
		{
			string code = @"
local t = {1, 2}
table.insert(t, 2, 3)
t[2]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test52_Table_Insert_Return_Value_CompileAll()
		{
			string code = @"
local t = {1, 2}
table.insert(t, 2, 3)
t[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		#endregion
	}
}
