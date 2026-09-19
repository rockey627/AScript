using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoMapTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["go"] = GoLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("go");
		}

		[TestMethod]
		public void Test01_MapMakeDeclaration()
		{
			var s = @"
var m = make(map[string]int)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(m)"));
		}

		[TestMethod]
		public void Test01_MapMakeDeclaration_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(m)"));
		}

		[TestMethod]
		public void Test02_MapShortDeclaration()
		{
			var s = @"
m := make(map[string]int)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(m)"));
		}

		[TestMethod]
		public void Test02_MapShortDeclaration_CompileAll()
		{
			var s = @"
m := make(map[string]int)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(m)"));
		}

		[TestMethod]
		public void Test03_MapAssignmentAndAccess()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 1
m[""b""] = 2
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("m[\"a\"]"));
			Assert.AreEqual(2, script.Eval("m[\"b\"]"));
		}

		[TestMethod]
		public void Test03_MapAssignmentAndAccess_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 1
m[""b""] = 2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("m[\"a\"]"));
			Assert.AreEqual(2, script.Eval("m[\"b\"]"));
		}

		[TestMethod]
		public void Test04_MapLen()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 1
m[""b""] = 2
m[""c""] = 3
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(m)"));
		}

		[TestMethod]
		public void Test04_MapLen_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 1
m[""b""] = 2
m[""c""] = 3
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(m)"));
		}

		[TestMethod]
		public void Test05_MapStringToString()
		{
			var s = @"
var m = make(map[string]string)
m[""name""] = ""hello""
m[""world""] = ""go""
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("hello", script.Eval("m[\"name\"]"));
			Assert.AreEqual("go", script.Eval("m[\"world\"]"));
		}

		[TestMethod]
		public void Test05_MapStringToString_CompileAll()
		{
			var s = @"
var m = make(map[string]string)
m[""name""] = ""hello""
m[""world""] = ""go""
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("hello", script.Eval("m[\"name\"]"));
			Assert.AreEqual("go", script.Eval("m[\"world\"]"));
		}

		[TestMethod]
		public void Test06_MapIntToInt()
		{
			var s = @"
var m = make(map[int]int)
m[1] = 100
m[2] = 200
m[3] = 300
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("m[1]"));
			Assert.AreEqual(200, script.Eval("m[2]"));
			Assert.AreEqual(300, script.Eval("m[3]"));
		}

		[TestMethod]
		public void Test06_MapIntToInt_CompileAll()
		{
			var s = @"
var m = make(map[int]int)
m[1] = 100
m[2] = 200
m[3] = 300
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("m[1]"));
			Assert.AreEqual(200, script.Eval("m[2]"));
			Assert.AreEqual(300, script.Eval("m[3]"));
		}

		[TestMethod]
		public void Test07_MapUpdateValue()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 1
m[""a""] = m[""a""] + 10
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(11, script.Eval("m[\"a\"]"));
		}

		[TestMethod]
		public void Test07_MapUpdateValue_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 1
m[""a""] = m[""a""] + 10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(11, script.Eval("m[\"a\"]"));
		}

		[TestMethod]
		public void Test08_MapAsFunctionParameter()
		{
			var s = @"
func get(m map[string]int) int {
    return m[""a""]
}
var m = make(map[string]int)
m[""a""] = 42
get(m)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(42, script.Eval(s));
		}

		[TestMethod]
		public void Test08_MapAsFunctionParameter_CompileAll()
		{
			var s = @"
func get(m map[string]int) int {
    return m[""a""]
}
var m = make(map[string]int)
m[""a""] = 42
get(m)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(42, script.Eval(s));
		}

		[TestMethod]
		public void Test09_MapReturnFromFunction()
		{
			var s = @"
func createMap() map[string]int {
    var m = make(map[string]int)
    m[""x""] = 99
    return m
}
var m = createMap()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(99, script.Eval("m[\"x\"]"));
		}

		[TestMethod]
		public void Test09_MapReturnFromFunction_CompileAll()
		{
			var s = @"
func createMap() map[string]int {
    var m = make(map[string]int)
    m[""x""] = 99
    return m
}
var m = createMap()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(99, script.Eval("m[\"x\"]"));
		}

		[TestMethod]
		public void Test10_MapFloatValues()
		{
			var s = @"
var m = make(map[string]float64)
m[""pi""] = 3.14
m[""e""] = 2.718
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3.14, script.Eval("m[\"pi\"]"));
			Assert.AreEqual(2.718, script.Eval("m[\"e\"]"));
		}

		[TestMethod]
		public void Test10_MapFloatValues_CompileAll()
		{
			var s = @"
var m = make(map[string]float64)
m[""pi""] = 3.14
m[""e""] = 2.718
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3.14, script.Eval("m[\"pi\"]"));
			Assert.AreEqual(2.718, script.Eval("m[\"e\"]"));
		}

		[TestMethod]
		public void Test11_MapBooleanValues()
		{
			var s = @"
var m = make(map[string]bool)
m[""enabled""] = true
m[""disabled""] = false
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(true, script.Eval("m[\"enabled\"]"));
			Assert.AreEqual(false, script.Eval("m[\"disabled\"]"));
		}

		[TestMethod]
		public void Test11_MapBooleanValues_CompileAll()
		{
			var s = @"
var m = make(map[string]bool)
m[""enabled""] = true
m[""disabled""] = false
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(true, script.Eval("m[\"enabled\"]"));
			Assert.AreEqual(false, script.Eval("m[\"disabled\"]"));
		}

		[TestMethod]
		public void Test12_MapSliceValue()
		{
			var s = @"
var m = make(map[string][]int)
m[""numbers""] = []int{1, 2, 3}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(m[\"numbers\"])"));
			Assert.AreEqual(1, script.Eval("m[\"numbers\"][0]"));
			Assert.AreEqual(3, script.Eval("m[\"numbers\"][2]"));
		}

		[TestMethod]
		public void Test12_MapSliceValue_CompileAll()
		{
			var s = @"
var m = make(map[string][]int)
m[""numbers""] = []int{1, 2, 3}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(m[\"numbers\"])"));
			Assert.AreEqual(1, script.Eval("m[\"numbers\"][0]"));
			Assert.AreEqual(3, script.Eval("m[\"numbers\"][2]"));
		}

		[TestMethod]
		public void Test13_MapModifySliceInMap()
		{
			var s = @"
var m = make(map[string][]int)
m[""numbers""] = []int{1, 2, 3}
m[""numbers""] = append(m[""numbers""], 4, 5)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(m[\"numbers\"])"));
			Assert.AreEqual(4, script.Eval("m[\"numbers\"][3]"));
			Assert.AreEqual(5, script.Eval("m[\"numbers\"][4]"));
		}

		[TestMethod]
		public void Test13_MapModifySliceInMap_CompileAll()
		{
			var s = @"
var m = make(map[string][]int)
m[""numbers""] = []int{1, 2, 3}
m[""numbers""] = append(m[""numbers""], 4, 5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(m[\"numbers\"])"));
			Assert.AreEqual(4, script.Eval("m[\"numbers\"][3]"));
			Assert.AreEqual(5, script.Eval("m[\"numbers\"][4]"));
		}

		[TestMethod]
		public void Test14_MapArrayValue()
		{
			var s = @"
var m = make(map[int][]int)
m[0] = []int{1, 2, 3}
m[1] = []int{4, 5, 6}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(m[0])"));
			Assert.AreEqual(1, script.Eval("m[0][0]"));
			Assert.AreEqual(3, script.Eval("len(m[1])"));
			Assert.AreEqual(6, script.Eval("m[1][2]"));
		}

		[TestMethod]
		public void Test14_MapArrayValue_CompileAll()
		{
			var s = @"
var m = make(map[int][]int)
m[0] = []int{1, 2, 3}
m[1] = []int{4, 5, 6}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(m[0])"));
			Assert.AreEqual(1, script.Eval("m[0][0]"));
			Assert.AreEqual(3, script.Eval("len(m[1])"));
			Assert.AreEqual(6, script.Eval("m[1][2]"));
		}

		[TestMethod]
		public void Test15_MapMultipleAssignments()
		{
			var s = @"
var m = make(map[string]int)
m[""a""], m[""b""] = 1, 2
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("m[\"a\"]"));
			Assert.AreEqual(2, script.Eval("m[\"b\"]"));
		}

		[TestMethod]
		public void Test15_MapMultipleAssignments_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
m[""a""], m[""b""] = 1, 2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("m[\"a\"]"));
			Assert.AreEqual(2, script.Eval("m[\"b\"]"));
		}

		[TestMethod]
		public void Test16_MapIteration()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 1
m[""b""] = 2
m[""c""] = 3
var sum = 0
for k := 0; k < len(m); k++ {
    sum += m[""a""]
}
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(3, script.Eval(s));
		}

		[TestMethod]
		public void Test16_MapIteration_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 1
m[""b""] = 2
m[""c""] = 3
var sum = 0
for k := 0; k < len(m); k++ {
    sum += m[""a""]
}
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(3, script.Eval(s));
		}

		[TestMethod]
		public void Test17_MapMapValue()
		{
			var s = @"
var m = make(map[string]map[string]int)
m[""outer""] = make(map[string]int)
m[""outer""][""inner""] = 100
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("m[\"outer\"][\"inner\"]"));
		}

		[TestMethod]
		public void Test17_MapMapValue_CompileAll()
		{
			var s = @"
var m = make(map[string]map[string]int)
m[""outer""] = make(map[string]int)
m[""outer""][""inner""] = 100
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("m[\"outer\"][\"inner\"]"));
		}

		[TestMethod]
		public void Test18_MapEmptyMap()
		{
			var s = @"
var m = make(map[string]int)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(m)"));
		}

		[TestMethod]
		public void Test18_MapEmptyMap_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(m)"));
		}

		[TestMethod]
		public void Test19_MapVariableAsKey()
		{
			var s = @"
var m = make(map[string]int)
var key = ""name""
m[key] = 123
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(123, script.Eval("m[key]"));
		}

		[TestMethod]
		public void Test19_MapVariableAsKey_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
var key = ""name""
m[key] = 123
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(123, script.Eval("m[key]"));
		}

		[TestMethod]
		public void Test20_MapVariableAsIndex()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 10
m[""b""] = 20
var idx = ""a""
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("m[idx]"));
		}

		[TestMethod]
		public void Test20_MapVariableAsIndex_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 10
m[""b""] = 20
var idx = ""a""
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("m[idx]"));
		}

		[TestMethod]
		public void Test21_MapFunctionReturnAsKey()
		{
			var s = @"
func getKey() string {
    return ""key""
}
var m = make(map[string]int)
m[getKey()] = 999
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(999, script.Eval("m[getKey()]"));
		}

		[TestMethod]
		public void Test21_MapFunctionReturnAsKey_CompileAll()
		{
			var s = @"
func getKey() string {
    return ""key""
}
var m = make(map[string]int)
m[getKey()] = 999
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(999, script.Eval("m[getKey()]"));
		}
	}
}