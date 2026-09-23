using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoIfTest
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
		public void Test01_IfBasic()
		{
			var s = @"
var result = 0
if true {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test01_IfBasic_CompileAll()
		{
			var s = @"
var result = 0
if true {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test02_IfFalseCondition()
		{
			var s = @"
var result = 0
if false {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(0, script.Eval(s));
		}

		[TestMethod]
		public void Test02_IfFalseCondition_CompileAll()
		{
			var s = @"
var result = 0
if false {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(0, script.Eval(s));
		}

		[TestMethod]
		public void Test03_IfElse()
		{
			var s = @"
var result = 0
if true {
    result = 1
} else {
    result = 2
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test03_IfElse_CompileAll()
		{
			var s = @"
var result = 0
if true {
    result = 1
} else {
    result = 2
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test04_IfElseFalse()
		{
			var s = @"
var result = 0
if false {
    result = 1
} else {
    result = 2
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test04_IfElseFalse_CompileAll()
		{
			var s = @"
var result = 0
if false {
    result = 1
} else {
    result = 2
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test05_IfElseIf()
		{
			var s = @"
var result = 0
if false {
    result = 1
} else if true {
    result = 2
} else {
    result = 3
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test05_IfElseIf_CompileAll()
		{
			var s = @"
var result = 0
if false {
    result = 1
} else if true {
    result = 2
} else {
    result = 3
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test06_IfElseIfElseIf()
		{
			var s = @"
var result = 0
if false {
    result = 1
} else if false {
    result = 2
} else if true {
    result = 3
} else {
    result = 4
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(3, script.Eval(s));
		}

		[TestMethod]
		public void Test06_IfElseIfElseIf_CompileAll()
		{
			var s = @"
var result = 0
if false {
    result = 1
} else if false {
    result = 2
} else if true {
    result = 3
} else {
    result = 4
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(3, script.Eval(s));
		}

		[TestMethod]
		public void Test07_IfWithComparison()
		{
			var s = @"
var result = 0
var a = 10
if a > 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test07_IfWithComparison_CompileAll()
		{
			var s = @"
var result = 0
var a = 10
if a > 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test08_IfWithInitStatement()
		{
			var s = @"
var result = 0
if a := 10; a > 5 {
    result = a
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test08_IfWithInitStatement_CompileAll()
		{
			var s = @"
var result = 0
if a := 10; a > 5 {
    result = a
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test09_IfWithInitStatementElse()
		{
			var s = @"
var result = 0
if a := 3; a > 5 {
    result = 1
} else {
    result = 2
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test09_IfWithInitStatementElse_CompileAll()
		{
			var s = @"
var result = 0
if a := 3; a > 5 {
    result = 1
} else {
    result = 2
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test10_IfWithAndOperator()
		{
			var s = @"
var result = 0
if true && true {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test10_IfWithAndOperator_CompileAll()
		{
			var s = @"
var result = 0
if true && true {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test11_IfWithOrOperator()
		{
			var s = @"
var result = 0
if false || true {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test11_IfWithOrOperator_CompileAll()
		{
			var s = @"
var result = 0
if false || true {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test12_IfWithNotOperator()
		{
			var s = @"
var result = 0
if !false {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test12_IfWithNotOperator_CompileAll()
		{
			var s = @"
var result = 0
if !false {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test13_IfWithEquals()
		{
			var s = @"
var result = 0
var a = 5
if a == 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test13_IfWithEquals_CompileAll()
		{
			var s = @"
var result = 0
var a = 5
if a == 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test14_IfWithNotEquals()
		{
			var s = @"
var result = 0
var a = 5
if a != 3 {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test14_IfWithNotEquals_CompileAll()
		{
			var s = @"
var result = 0
var a = 5
if a != 3 {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test15_IfWithLessThan()
		{
			var s = @"
var result = 0
var a = 3
if a < 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test15_IfWithLessThan_CompileAll()
		{
			var s = @"
var result = 0
var a = 3
if a < 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test16_IfWithGreaterThanOrEqual()
		{
			var s = @"
var result = 0
var a = 5
if a >= 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test16_IfWithGreaterThanOrEqual_CompileAll()
		{
			var s = @"
var result = 0
var a = 5
if a >= 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test17_IfWithLessThanOrEqual()
		{
			var s = @"
var result = 0
var a = 5
if a <= 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test17_IfWithLessThanOrEqual_CompileAll()
		{
			var s = @"
var result = 0
var a = 5
if a <= 5 {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test18_IfNested()
		{
			var s = @"
var result = 0
var a = 5
if a > 3 {
    if a < 10 {
        result = 1
    }
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test18_IfNested_CompileAll()
		{
			var s = @"
var result = 0
var a = 5
if a > 3 {
    if a < 10 {
        result = 1
    }
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test19_IfInFunction()
		{
			var s = @"
func max(a int, b int) int {
    if a > b {
        return a
    }
    return b
}
max(3, 5)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test19_IfInFunction_CompileAll()
		{
			var s = @"
func max(a int, b int) int {
    if a > b {
        return a
    }
    return b
}
max(3, 5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test20_IfWithMapAccess()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 10
var result = 0
if v, ok := m[""a""]; ok {
    result = v
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test20_IfWithMapAccess_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
m[""a""] = 10
var result = 0
if v, ok := m[""a""]; ok {
    result = v
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test21_IfWithMapAccessNotExist()
		{
			var s = @"
var m = make(map[string]int)
var result = 0
if v, ok := m[""notexist""]; ok {
    result = v
} else {
    result = -1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(-1, script.Eval(s));
		}

		[TestMethod]
		public void Test21_IfWithMapAccessNotExist_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
var result = 0
if v, ok := m[""notexist""]; ok {
    result = v
} else {
    result = -1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(-1, script.Eval(s));
		}

		[TestMethod]
		public void Test22_IfWithStringComparison()
		{
			var s = @"
var result = 0
var s = ""hello""
if s == ""hello"" {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test22_IfWithStringComparison_CompileAll()
		{
			var s = @"
var result = 0
var s = ""hello""
if s == ""hello"" {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test23_IfShortDeclaration()
		{
			var s = @"
var result = 0
if n := 10; n > 5 {
    result = n
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test23_IfShortDeclaration_CompileAll()
		{
			var s = @"
var result = 0
if n := 10; n > 5 {
    result = n
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test24_IfWithComplexCondition()
		{
			var s = @"
var result = 0
var a = 5
var b = 10
if a > 0 && b > 5 || a == b {
    result = 1
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test24_IfWithComplexCondition_CompileAll()
		{
			var s = @"
var result = 0
var a = 5
var b = 10
if a > 0 && b > 5 || a == b {
    result = 1
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test25_IfElseIfChain()
		{
			var s = @"
var result = 0
var score = 85
if score >= 90 {
    result = 1
} else if score >= 80 {
    result = 2
} else if score >= 70 {
    result = 3
} else {
    result = 4
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test25_IfElseIfChain_CompileAll()
		{
			var s = @"
var result = 0
var score = 85
if score >= 90 {
    result = 1
} else if score >= 80 {
    result = 2
} else if score >= 70 {
    result = 3
} else {
    result = 4
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test26_IfElseIfChain()
		{
			var s = @"
func check(score int) int {
	if score >= 90 {
		return 1
	} else if score >= 80 {
		return 2
	} else if score >= 70 {
		return 3
	} else {
		return 4
	}
}
[]int{ check(100), check(85), check(75), check(60) }
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var list = script.Eval<List<int>>(s);
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4 }, list);
		}

		[TestMethod]
		public void Test26_IfElseIfChain_CompileAll()
		{
			var s = @"
func check(score int) int {
	if score >= 90 {
		return 1
	} else if score >= 80 {
		return 2
	} else if score >= 70 {
		return 3
	} else {
		return 4
	}
}
[]int{ check(100), check(85), check(75), check(60) }
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var list = script.Eval<List<int>>(s);
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4 }, list);
		}
	}
}