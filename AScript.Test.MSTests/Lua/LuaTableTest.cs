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

		[TestMethod]
		public void Test00_01()
		{
			string code = @"
local t = {}
t[3]='a'
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			script.Eval(code);
			Assert.AreEqual(0L, script.Eval("#t"));
			Assert.AreEqual("a", script.Eval("t[3]"));
			Assert.AreEqual(null, script.Eval("t[1]"));
		}

		[TestMethod]
		public void Test00_01_CompileAll()
		{
			string code = @"
local t = {}
t[3]='a'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			script.Eval(code);
			Assert.AreEqual(0L, script.Eval("#t"));
			Assert.AreEqual("a", script.Eval("t[3]"));
			Assert.AreEqual(null, script.Eval("t[1]"));
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

		#region Table Sort (table.sort)

		[TestMethod]
		public void Test53_Table_Sort_Numbers()
		{
			string code = @"
local t = {3, 1, 4, 1, 5, 9, 2, 6}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,1,2,3,4,5,6,9", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test53_Table_Sort_Numbers_CompileAll()
		{
			string code = @"
local t = {3, 1, 4, 1, 5, 9, 2, 6}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,1,2,3,4,5,6,9", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test54_Table_Sort_Strings()
		{
			string code = @"
local t = {'dog', 'cat', 'bird', 'apple'}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("apple,bird,cat,dog", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test54_Table_Sort_Strings_CompileAll()
		{
			string code = @"
local t = {'dog', 'cat', 'bird', 'apple'}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("apple,bird,cat,dog", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test55_Table_Sort_Already_Sorted()
		{
			string code = @"
local t = {1, 2, 3, 4, 5}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3,4,5", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test55_Table_Sort_Already_Sorted_CompileAll()
		{
			string code = @"
local t = {1, 2, 3, 4, 5}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3,4,5", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test56_Table_Sort_Reverse_Order()
		{
			string code = @"
local t = {5, 4, 3, 2, 1}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3,4,5", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test56_Table_Sort_Reverse_Order_CompileAll()
		{
			string code = @"
local t = {5, 4, 3, 2, 1}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,2,3,4,5", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test57_Table_Sort_Single_Element()
		{
			string code = @"
local t = {42}
table.sort(t)
t[1]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(42L, script.Eval(code));
		}

		[TestMethod]
		public void Test57_Table_Sort_Single_Element_CompileAll()
		{
			string code = @"
local t = {42}
table.sort(t)
t[1]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(42L, script.Eval(code));
		}

		[TestMethod]
		public void Test58_Table_Sort_Empty_Table()
		{
			string code = @"
local t = {}
table.sort(t)
#t
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test58_Table_Sort_Empty_Table_CompileAll()
		{
			string code = @"
local t = {}
table.sort(t)
#t
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(0L, script.Eval(code));
		}

		[TestMethod]
		public void Test59_Table_Sort_LuaTable_Direct()
		{
			var table = new LuaTable();
			table[1] = 5;
			table[2] = 2;
			table[3] = 8;
			table[4] = 1;
			LuaTable.sort(table);
			Assert.AreEqual(1, table[1]);
			Assert.AreEqual(2, table[2]);
			Assert.AreEqual(5, table[3]);
			Assert.AreEqual(8, table[4]);
		}

		[TestMethod]
		public void Test60_Table_Sort_Duplicates()
		{
			string code = @"
local t = {3, 3, 3, 1, 1, 2, 2, 2}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,1,2,2,2,3,3,3", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test60_Table_Sort_Duplicates_CompileAll()
		{
			string code = @"
local t = {3, 3, 3, 1, 1, 2, 2, 2}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,1,2,2,2,3,3,3", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test61_Table_Sort_Negative_Numbers()
		{
			string code = @"
local t = {5, -3, 2, -1, 0, 4, -10}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("-10,-3,-1,0,2,4,5", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test61_Table_Sort_Negative_Numbers_CompileAll()
		{
			string code = @"
local t = {5, -3, 2, -1, 0, 4, -10}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("-10,-3,-1,0,2,4,5", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test62_Table_Sort_Float_Numbers()
		{
			string code = @"
local t = {3.14, 2.71, 1.41, 0.5}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("0.5,1.41,2.71,3.14", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test62_Table_Sort_Float_Numbers_CompileAll()
		{
			string code = @"
local t = {3.14, 2.71, 1.41, 0.5}
table.sort(t)
table.concat(t, ',')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("0.5,1.41,2.71,3.14", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test63_Table_Sort_Does_Not_Affect_Dictionary_Part()
		{
			string code = @"
local t = {3, 1, 4}
t.name = 'test'
t.other = 123
table.sort(t)
t[1] .. ',' .. t[2] .. ',' .. t[3] .. ',' .. t.name
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,3,4,test", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test63_Table_Sort_Does_Not_Affect_Dictionary_Part_CompileAll()
		{
			string code = @"
local t = {3, 1, 4}
t.name = 'test'
t.other = 123
table.sort(t)
t[1] .. ',' .. t[2] .. ',' .. t[3] .. ',' .. t.name
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("1,3,4,test", script.Eval<string>(code));
		}

		#endregion

		#region Metatable

		[TestMethod]
		public void Test64_Metatable_Set_Get()
		{
			string code = @"
local t = {}
setmetatable(t, {__index={key='value'}})
t.key
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("value", script.Eval(code));
		}

		[TestMethod]
		public void Test64_Metatable_Set_Get_CompileAll()
		{
			string code = @"
local t = {}
setmetatable(t, {__index={key='value'}})
t.key
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("value", script.Eval(code));
		}

		[TestMethod]
		public void Test65_Metatable_Index_Table_Delegate()
		{
			string code = @"
mytable = setmetatable({key1 = 'value1'}, {
  __index = function(mytable, key)
    if key == 'key2' then
      return 'metatablevalue'
    else
      return nil
    end
  end
})
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			script.Eval(code);
			Assert.AreEqual("value1", script.Eval("mytable.key1"));
			Assert.AreEqual("metatablevalue", script.Eval("mytable.key2"));
		}

		[TestMethod]
		public void Test65_Metatable_Index_Table_Delegate_CompileAll()
		{
			string code = @"
mytable = setmetatable({key1 = 'value1'}, {
  __index = function(mytable, key)
    if key == 'key2' then
      return 'metatablevalue'
    else
      return nil
    end
  end
})
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			script.Eval(code);
			Assert.AreEqual("value1", script.Eval("mytable.key1"));
			Assert.AreEqual("metatablevalue", script.Eval("mytable.key2"));
		}

		[TestMethod]
		public void Test66_Metatable_Index_Table_Delegate_Array()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto[1] = 'first'
proto[2] = 'second'
mt.__index = proto
setmetatable(t, mt)
t[1] .. ',' .. t[2]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("first,second", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test66_Metatable_Index_Table_Delegate_Array_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto[1] = 'first'
proto[2] = 'second'
mt.__index = proto
setmetatable(t, mt)
t[1] .. ',' .. t[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("first,second", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test66_Metatable_Index_Table_Delegate_Array2()
		{
			string code = @"
local t = {'a','b','c'}
t[2]=null
local mt = {}
local proto = {}
proto[1] = 'first'
proto[2] = 'second'
mt.__index = proto
setmetatable(t, mt)
t[1] .. ',' .. t[2]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("a,second", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test66_Metatable_Index_Table_Delegate_Array2_CompileAll()
		{
			string code = @"
local t = {'a','b','c'}
t[2]=null
local mt = {}
local proto = {}
proto[1] = 'first'
proto[2] = 'second'
mt.__index = proto
setmetatable(t, mt)
t[1] .. ',' .. t[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("a,second", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test67_Metatable_Index_Not_Found()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto.existing = 'value'
mt.__index = proto
setmetatable(t, mt)
t.nonexistent
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(null, script.Eval(code));
		}

		[TestMethod]
		public void Test67_Metatable_Index_Not_Found_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto.existing = 'value'
mt.__index = proto
setmetatable(t, mt)
t.nonexistent
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(null, script.Eval(code));
		}

		[TestMethod]
		public void Test68_Metatable_Index_Override_Existing()
		{
			string code = @"
local t = {name = 'instance'}
local mt = {}
local proto = {}
proto.name = 'prototype'
mt.__index = proto
setmetatable(t, mt)
t.name
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("instance", script.Eval(code));
		}

		[TestMethod]
		public void Test68_Metatable_Index_Override_Existing_CompileAll()
		{
			string code = @"
local t = {name = 'instance'}
local mt = {}
local proto = {}
proto.name = 'prototype'
mt.__index = proto
setmetatable(t, mt)
t.name
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("instance", script.Eval(code));
		}

		[TestMethod]
		public void Test69_Metatable_NewIndex_Table()
		{
			string code = @"
local t = {}
local mt = {}
local storage = {}
mt.__newindex = storage
setmetatable(t, mt)
t.newkey = 'newvalue'
storage.newkey
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("newvalue", script.Eval(code));
			Assert.AreEqual(null, script.Eval("t.newkey"));
		}

		[TestMethod]
		public void Test69_Metatable_NewIndex_Table_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
local storage = {}
mt.__newindex = storage
setmetatable(t, mt)
t.newkey = 'newvalue'
storage.newkey
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("newvalue", script.Eval(code));
			Assert.AreEqual(null, script.Eval("t.newkey"));
		}

		[TestMethod]
		public void Test70_Metatable_Chained_Index()
		{
			string code = @"
local t = {}
local mt = {}
local proto1 = {}
local proto2 = {}
proto2.deep = 'found'
proto1.parent = proto2
mt.__index = proto1
setmetatable(t, mt)
t.parent.deep
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("found", script.Eval(code));
		}

		[TestMethod]
		public void Test70_Metatable_Chained_Index_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
local proto1 = {}
local proto2 = {}
proto2.deep = 'found'
proto1.parent = proto2
mt.__index = proto1
setmetatable(t, mt)
t.parent.deep
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("found", script.Eval(code));
		}

		[TestMethod]
		public void Test71_Metatable_Index_With_Array_And_Dict()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto[1] = 'array_value'
proto.dict = 'dict_value'
mt.__index = proto
setmetatable(t, mt)
t[1] .. ',' .. t.dict
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("array_value,dict_value", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test71_Metatable_Index_With_Array_And_Dict_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto[1] = 'array_value'
proto.dict = 'dict_value'
mt.__index = proto
setmetatable(t, mt)
t[1] .. ',' .. t.dict
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("array_value,dict_value", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test72_Metatable_NewIndex_Overwrite()
		{
			string code = @"
local t = {key = 'original'}
local mt = {}
local storage = {}
mt.__newindex = storage
setmetatable(t, mt)
t.key = 'modified'
storage.key .. ',' .. t.key
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(",modified", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test72_Metatable_NewIndex_Overwrite_CompileAll()
		{
			string code = @"
local t = {key = 'original'}
local mt = {}
local storage = {}
mt.__newindex = storage
setmetatable(t, mt)
t.key = 'modified'
storage.key .. ',' .. t.key
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(",modified", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test73_Metatable_Index_Numeric_Key_In_Prototype()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto[100] = 'hundred'
mt.__index = proto
setmetatable(t, mt)
t[100]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("hundred", script.Eval(code));
		}

		[TestMethod]
		public void Test74_Metatable_NewIndex_Numeric_Key()
		{
			string code = @"
local t = {}
local mt = {}
local storage = {}
mt.__newindex = storage
setmetatable(t, mt)
t[5] = 'five'
storage[5]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("five", script.Eval(code));
		}

		[TestMethod]
		public void Test74_Metatable_NewIndex_Numeric_Key_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
local storage = {}
mt.__newindex = storage
setmetatable(t, mt)
t[5] = 'five'
storage[5]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("five", script.Eval(code));
		}

		[TestMethod]
		public void Test75_Metatable_Index_Own_And_Prototype()
		{
			string code = @"
local t = {own = 'mine'}
local mt = {}
local proto = {}
proto.proto_key = 'proto_value'
mt.__index = proto
setmetatable(t, mt)
t.own .. ',' .. t.proto_key .. ',' .. tostring(t.missing)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("mine,proto_value,nil", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test75_Metatable_Index_Own_And_Prototype_CompileAll()
		{
			string code = @"
local t = {own = 'mine'}
local mt = {}
local proto = {}
proto.proto_key = 'proto_value'
mt.__index = proto
setmetatable(t, mt)
t.own .. ',' .. t.proto_key .. ',' .. tostring(t.missing)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("mine,proto_value,nil", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test76_Metatable_Multiple_Keys()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto.a = 1
proto.b = 2
proto.c = 3
mt.__index = proto
setmetatable(t, mt)
t.a + t.b + t.c
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test76_Metatable_Multiple_Keys_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto.a = 1
proto.b = 2
proto.c = 3
mt.__index = proto
setmetatable(t, mt)
t.a + t.b + t.c
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test77_Metatable_Mixed_Read_Write()
		{
			string code = @"
local t = {direct = 'direct_value'}
local mt = {}
local storage = {}
local index_tbl = {}
index_tbl.from_index = 'index_value'
mt.__newindex = storage
mt.__index = index_tbl
setmetatable(t, mt)
t.direct .. ',' .. t.from_index .. ',' .. tostring(t.new_key)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("direct_value,index_value,nil", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test77_Metatable_Mixed_Read_Write_CompileAll()
		{
			string code = @"
local t = {direct = 'direct_value'}
local mt = {}
local storage = {}
local index_tbl = {}
index_tbl.from_index = 'index_value'
mt.__newindex = storage
mt.__index = index_tbl
setmetatable(t, mt)
t.direct .. ',' .. t.from_index .. ',' .. tostring(t.new_key)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("direct_value,index_value,nil", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test78_Metatable_Prototype_Inheritance()
		{
			string code = @"
local child = {child_key = 'child_value'}
local mt = {}
local parent = {}
parent.parent_key = 'parent_value'
parent.another = 'another_value'
mt.__index = parent
setmetatable(child, mt)
child.child_key .. ',' .. child.parent_key .. ',' .. child.another
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("child_value,parent_value,another_value", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test78_Metatable_Prototype_Inheritance_CompileAll()
		{
			string code = @"
local child = {child_key = 'child_value'}
local mt = {}
local parent = {}
parent.parent_key = 'parent_value'
parent.another = 'another_value'
mt.__index = parent
setmetatable(child, mt)
child.child_key .. ',' .. child.parent_key .. ',' .. child.another
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("child_value,parent_value,another_value", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test79_Metatable_Array_In_Prototype()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto[1] = 100
proto[2] = 200
proto[3] = 300
mt.__index = proto
setmetatable(t, mt)
t[1] + t[2] + t[3]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(600L, script.Eval(code));
		}

		[TestMethod]
		public void Test79_Metatable_Array_In_Prototype_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
local proto = {}
proto[1] = 100
proto[2] = 200
proto[3] = 300
mt.__index = proto
setmetatable(t, mt)
t[1] + t[2] + t[3]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(600L, script.Eval(code));
		}

		[TestMethod]
		public void Test80_Metatable_Add_Operator()
		{
			string code = @"
local t1 = {value = 10}
local t2 = {value = 20}
local mt = {}
mt.__add = function(a, b)
	local r = {}
	r.value = a.value + b.value
	return r
end
setmetatable(t1, mt)
setmetatable(t2, mt)
local r = t1 + t2
r.value
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void Test80_Metatable_Add_Operator_CompileAll()
		{
			string code = @"
local t1 = {value = 10}
local t2 = {value = 20}
local mt = {}
mt.__add = function(a, b)
	local r = {}
	r.value = a.value + b.value
	return r
end
setmetatable(t1, mt)
setmetatable(t2, mt)
local r = t1 + t2
r.value
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void Test81_Metatable_Tostring()
		{
			string code = @"
local t = {1, 2, 3}
local mt = {}
mt.__tostring = function(tbl)
	return '[' .. tbl[1] .. ',' .. tbl[2] .. ',' .. tbl[3] .. ']'
end
setmetatable(t, mt)
tostring(t)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("[1,2,3]", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test81_Metatable_Tostring_CompileAll()
		{
			string code = @"
local t = {1, 2, 3}
local mt = {}
mt.__tostring = function(tbl)
	return '[' .. tbl[1] .. ',' .. tbl[2] .. ',' .. tbl[3] .. ']'
end
setmetatable(t, mt)
tostring(t)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("[1,2,3]", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test82_Metatable_Tostring_Custom_Format()
		{
			string code = @"
local t = {name = 'test', value = 42}
local mt = {}
mt.__tostring = function(tbl)
	return 'Table:{name=' .. tbl.name .. ',value=' .. tbl.value .. '}'
end
setmetatable(t, mt)
tostring(t)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Table:{name=test,value=42}", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test82_Metatable_Tostring_Custom_Format_CompileAll()
		{
			string code = @"
local t = {name = 'test', value = 42}
local mt = {}
mt.__tostring = function(tbl)
	return 'Table:{name=' .. tbl.name .. ',value=' .. tbl.value .. '}'
end
setmetatable(t, mt)
tostring(t)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Table:{name=test,value=42}", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test83_Metatable_No_Add_Operator()
		{
			string code = @"
local t1 = {value = 10}
local t2 = {value = 20}
setmetatable(t1, {})
setmetatable(t2, {})
local ok, err = pcall(function() return t1 + t2 end)
ok
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(false, script.Eval<bool>(code));
		}

		[TestMethod]
		public void Test83_Metatable_No_Add_Operator_CompileAll()
		{
			string code = @"
local t1 = {value = 10}
local t2 = {value = 20}
setmetatable(t1, {})
setmetatable(t2, {})
local ok, err = pcall(function() return t1 + t2 end)
ok
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(false, script.Eval<bool>(code));
		}

		[TestMethod]
		public void Test84_Metatable_Add_Operator_Array_Concat()
		{
			string code = @"
local t1 = {'a', 'b'}
local t2 = {'c', 'd'}
local mt = {}
mt.__add = function(a, b)
	local r = {}
	r[1] = a[1] .. b[1]
	r[2] = a[2] .. b[2]
	return r
end
setmetatable(t1, mt)
setmetatable(t2, mt)
local r = t1 + t2
r[1] .. r[2]
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("acbd", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test84_Metatable_Add_Operator_Array_Concat_CompileAll()
		{
			string code = @"
local t1 = {'a', 'b'}
local t2 = {'c', 'd'}
local mt = {}
mt.__add = function(a, b)
	local r = {}
	r[1] = a[1] .. b[1]
	r[2] = a[2] .. b[2]
	return r
end
setmetatable(t1, mt)
setmetatable(t2, mt)
local r = t1 + t2
r[1] .. r[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("acbd", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test85_Metatable_Newindex_Function()
		{
			string code = @"
local t = {}
local captured = {}
local mt = {}
mt.__newindex = function(tbl, k, v)
	captured[k] = v
end
setmetatable(t, mt)
t.key1 = 'value1'
t.key2 = 'value2'
captured.key1 .. ',' .. captured.key2
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("value1,value2", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test85_Metatable_Newindex_Function_CompileAll()
		{
			string code = @"
local t = {}
local captured = {}
local mt = {}
mt.__newindex = function(tbl, k, v)
	captured[k] = v
end
setmetatable(t, mt)
t.key1 = 'value1'
t.key2 = 'value2'
captured.key1 .. ',' .. captured.key2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("value1,value2", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test86_Metatable_Index_Function()
		{
			string code = @"
local t = {}
local mt = {}
mt.__index = function(tbl, k)
	return 'custom_' .. k
end
setmetatable(t, mt)
t.hello .. ',' .. t.world
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("custom_hello,custom_world", script.Eval<string>(code));
		}

		[TestMethod]
		public void Test86_Metatable_Index_Function_CompileAll()
		{
			string code = @"
local t = {}
local mt = {}
mt.__index = function(tbl, k)
	return 'custom_' .. k
end
setmetatable(t, mt)
t.hello .. ',' .. t.world
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("custom_hello,custom_world", script.Eval<string>(code));
		}

		#endregion
	}
}
