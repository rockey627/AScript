using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoCommonTest
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
		public void Test01()
		{
			var s = @"
var a int = 10
var b int = 20
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test01_CompileAll()
		{
			var s = @"
var a int = 10
var b int = 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test02()
		{
			var s = @"
var a = 10
var b = 20
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test02_CompileAll()
		{
			var s = @"
var a = 10
var b = 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test03()
		{
			var s = @"
var a, b = 10, 20
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test03_CompileAll()
		{
			var s = @"
var a, b = 10, 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test04()
		{
			var s = @"
var a, b int = 10, 20
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test04_CompileAll()
		{
			var s = @"
var a, b int = 10, 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test05()
		{
			var s = @"
var a string, b int = 'hello', 20
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("hello20", script.Eval(s));
			Assert.AreEqual(typeof(string), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test05_CompileAll()
		{
			var s = @"
var a string, b int = 'hello', 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("hello20", script.Eval(s));
			Assert.AreEqual(typeof(string), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test06()
		{
			var s = @"
var a, b = 'hello', 20
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("hello20", script.Eval(s));
			Assert.AreEqual(typeof(string), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test06_CompileAll()
		{
			var s = @"
var a, b = 'hello', 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("hello20", script.Eval(s));
			Assert.AreEqual(typeof(string), script.Context.GetVarType("a"));
			Assert.AreEqual(typeof(int), script.Context.GetVarType("b"));
		}

		[TestMethod]
		public void Test07_ArithmeticOperators()
		{
			var s = @"
var a = 10
var b = 3
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(13, script.Eval("a+b"));
			Assert.AreEqual(7, script.Eval("a-b"));
			Assert.AreEqual(30, script.Eval("a*b"));
			Assert.AreEqual(3, script.Eval("a/b"));
			Assert.AreEqual(1, script.Eval("a%b"));
		}

		[TestMethod]
		public void Test07_ArithmeticOperators_CompileAll()
		{
			var s = @"
var a = 10
var b = 3
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(13, script.Eval("a+b"));
			Assert.AreEqual(7, script.Eval("a-b"));
			Assert.AreEqual(30, script.Eval("a*b"));
			Assert.AreEqual(3, script.Eval("a/b"));
			Assert.AreEqual(1, script.Eval("a%b"));
		}

		[TestMethod]
		public void Test08_ComparisonOperators()
		{
			var s = @"
var a = 10
var b = 20
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(false, script.Eval("a == b"));
			Assert.AreEqual(true, script.Eval("a != b"));
			Assert.AreEqual(true, script.Eval("a < b"));
			Assert.AreEqual(false, script.Eval("a > b"));
			Assert.AreEqual(true, script.Eval("a <= b"));
			Assert.AreEqual(false, script.Eval("a >= b"));
		}

		[TestMethod]
		public void Test08_ComparisonOperators_CompileAll()
		{
			var s = @"
var a = 10
var b = 20
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(false, script.Eval("a == b"));
			Assert.AreEqual(true, script.Eval("a != b"));
			Assert.AreEqual(true, script.Eval("a < b"));
			Assert.AreEqual(false, script.Eval("a > b"));
			Assert.AreEqual(true, script.Eval("a <= b"));
			Assert.AreEqual(false, script.Eval("a >= b"));
		}

		[TestMethod]
		public void Test09_LogicalOperators()
		{
			var s = @"
var a = true
var b = false
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(false, script.Eval("a && b"));
			Assert.AreEqual(true, script.Eval("a || b"));
			Assert.AreEqual(false, script.Eval("!a"));
			Assert.AreEqual(true, script.Eval("!b"));
		}

		[TestMethod]
		public void Test09_LogicalOperators_CompileAll()
		{
			var s = @"
var a = true
var b = false
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(false, script.Eval("a && b"));
			Assert.AreEqual(true, script.Eval("a || b"));
			Assert.AreEqual(false, script.Eval("!a"));
			Assert.AreEqual(true, script.Eval("!b"));
		}

		[TestMethod]
		public void Test10_BitwiseOperators()
		{
			var s = @"
var a = 5
var b = 3
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("a & b"));
			Assert.AreEqual(7, script.Eval("a | b"));
			Assert.AreEqual(6, script.Eval("a ^ b"));
			Assert.AreEqual(20, script.Eval("a << 2"));
			Assert.AreEqual(6, script.Eval("b << 1"));
			Assert.AreEqual(1, script.Eval("a >> 2"));
		}

		[TestMethod]
		public void Test10_BitwiseOperators_CompileAll()
		{
			var s = @"
var a = 5
var b = 3
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("a & b"));
			Assert.AreEqual(7, script.Eval("a | b"));
			Assert.AreEqual(6, script.Eval("a ^ b"));
			Assert.AreEqual(20, script.Eval("a << 2"));
			Assert.AreEqual(6, script.Eval("b << 1"));
			Assert.AreEqual(1, script.Eval("a >> 2"));
		}

		[TestMethod]
		public void Test11_AssignmentOperators()
		{
			var s = @"
var a = 10
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(15, script.Eval("a += 5"));
			Assert.AreEqual(12, script.Eval("a -= 3"));
			Assert.AreEqual(24, script.Eval("a *= 2"));
			Assert.AreEqual(6, script.Eval("a /= 4"));
			Assert.AreEqual(1, script.Eval("a %= 5"));
		}

		[TestMethod]
		public void Test11_AssignmentOperators_CompileAll()
		{
			var s = @"
var a = 10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(15, script.Eval("a += 5"));
			Assert.AreEqual(12, script.Eval("a -= 3"));
			Assert.AreEqual(24, script.Eval("a *= 2"));
			Assert.AreEqual(6, script.Eval("a /= 4"));
			Assert.AreEqual(1, script.Eval("a %= 5"));
		}

		[TestMethod]
		public void Test12_CompoundAssignment()
		{
			var s = @"
var a = 100
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(4, script.Eval("a &= 15"));
			Assert.AreEqual(7, script.Eval("a |= 3"));
			Assert.AreEqual(4, script.Eval("a ^= 3"));
		}

		[TestMethod]
		public void Test12_CompoundAssignment_CompileAll()
		{
			var s = @"
var a = 100
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(4, script.Eval("a &= 15"));
			Assert.AreEqual(7, script.Eval("a |= 3"));
			Assert.AreEqual(4, script.Eval("a ^= 3"));
		}

		[TestMethod]
		public void Test13_ShiftAssignment()
		{
			var s = @"
var a = 1
var b = 8
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(8, script.Eval("a <<= 3"));
			Assert.AreEqual(2, script.Eval("b >>= 2"));
		}

		[TestMethod]
		public void Test13_ShiftAssignment_CompileAll()
		{
			var s = @"
var a = 1
var b = 8
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(8, script.Eval("a <<= 3"));
			Assert.AreEqual(2, script.Eval("b >>= 2"));
		}

		[TestMethod]
		public void Test14_ShortVariableDeclaration()
		{
			var s = @"
a := 10
b := 20
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(30, script.Eval("a + b"));
		}

		[TestMethod]
		public void Test14_ShortVariableDeclaration_CompileAll()
		{
			var s = @"
a := 10
b := 20
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(30, script.Eval("a + b"));
		}

		[TestMethod]
		public void Test15_IncrementDecrement()
		{
			var s = @"
var a = 10
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("a++"));
			Assert.AreEqual(12, script.Eval("++a"));
			Assert.AreEqual(12, script.Eval("a--"));
			Assert.AreEqual(10, script.Eval("--a"));
			Assert.AreEqual(10, script.Eval("a"));
		}

		[TestMethod]
		public void Test15_IncrementDecrement_CompileAll()
		{
			var s = @"
var a = 10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("a++"));
			Assert.AreEqual(12, script.Eval("++a"));
			Assert.AreEqual(12, script.Eval("a--"));
			Assert.AreEqual(10, script.Eval("--a"));
			Assert.AreEqual(10, script.Eval("a"));
		}

		[TestMethod]
		public void Test16_Negation()
		{
			var s = @"
var a = -10
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("-a"));
		}

		[TestMethod]
		public void Test16_Negation_CompileAll()
		{
			var s = @"
var a = -10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("-a"));
		}

		[TestMethod]
		public void Test17_ConstDeclaration()
		{
			var s = @"
const a int = 100
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("a"));
		}

		[TestMethod]
		public void Test17_ConstDeclaration_CompileAll()
		{
			var s = @"
const a int = 100
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("a"));
		}

		[TestMethod]
		public void Test18_ConstMultiple()
		{
			var s = @"
const a, b = 100, 200
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(300, script.Eval(s));
		}

		[TestMethod]
		public void Test18_ConstMultiple_CompileAll()
		{
			var s = @"
const a, b = 100, 200
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(300, script.Eval(s));
		}

		[TestMethod]
		public void Test19_ComplexExpression()
		{
			var s = @"
var a = 10
var b = 20
var c = 30
(a + b) * c / 10 - 5
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(85, script.Eval(s));
		}

		[TestMethod]
		public void Test19_ComplexExpression_CompileAll()
		{
			var s = @"
var a = 10
var b = 20
var c = 30
(a + b) * c / 10 - 5
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(85, script.Eval(s));
		}

		[TestMethod]
		public void Test20_IfElse()
		{
			var s = @"
var a = 10
var b = 20
var c1, c2
if a < b { c1 = 100 } else { c1 = 200 }
if a > b { c2 = 100 } else { c2 = 200 }
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("c1"));
			Assert.AreEqual(200, script.Eval("c2"));
		}

		[TestMethod]
		public void Test20_IfElse_CompileAll()
		{
			var s = @"
var a = 10
var b = 20
var c1, c2
if a < b { c1 = 100 } else { c1 = 200 }
if a > b { c2 = 100 } else { c2 = 200 }
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("c1"));
			Assert.AreEqual(200, script.Eval("c2"));
		}

		[TestMethod]
		public void Test21_IfElseIf()
		{
			var s = @"
var a = 10
var b1, b2, b3
// b1
if a > 20 { b1 = 1 } 
else if a > 15 { b1 = 2 } 
else if a > 5 { b1 = 3 } 
else { b1 = 4 }
// b2
if a > 20 { b2 = 1 } 
else if a > 12 { b2 = 2 } 
else { b2 = 3 }
// b3
if a < 5 { b3 = 1 } 
else if a < 12 { b3 = 2 } 
else { b3 = 3 }
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("b1"));
			Assert.AreEqual(3, script.Eval("b2"));
			Assert.AreEqual(2, script.Eval("b3"));
		}

		[TestMethod]
		public void Test21_IfElseIf_CompileAll()
		{
			var s = @"
var a = 10
var b1, b2, b3
// b1
if a > 20 { b1 = 1 } 
else if a > 15 { b1 = 2 } 
else if a > 5 { b1 = 3 } 
else { b1 = 4 }
// b2
if a > 20 { b2 = 1 } 
else if a > 12 { b2 = 2 } 
else { b2 = 3 }
// b3
if a < 5 { b3 = 1 } 
else if a < 12 { b3 = 2 } 
else { b3 = 3 }
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("b1"));
			Assert.AreEqual(3, script.Eval("b2"));
			Assert.AreEqual(2, script.Eval("b3"));
		}

		[TestMethod]
		public void Test22_NestedIf()
		{
			var s = @"
var a = 10
var b = 20
var c1, c2
// c1
if a < b { 
	if a > 5 { c1 = 100 } else { c1 = 200 } 
} else { c1 = 300 }
// c2
if a < b { 
	if a > 15 { c2 = 100 } else { c2 = 200 } 
} else { c2 = 300 }
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("c1"));
			Assert.AreEqual(200, script.Eval("c2"));
		}

		[TestMethod]
		public void Test22_NestedIf_CompileAll()
		{
			var s = @"
var a = 10
var b = 20
var c1, c2
// c1
if a < b { 
	if a > 5 { c1 = 100 } else { c1 = 200 } 
} else { c1 = 300 }
// c2
if a < b { 
	if a > 15 { c2 = 100 } else { c2 = 200 } 
} else { c2 = 300 }
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("c1"));
			Assert.AreEqual(200, script.Eval("c2"));
		}

		[TestMethod]
		public void Test23_ForLoop()
		{
			var s = @"
var sum = 0
for i := 0; i < 5; i++ { sum += i }
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test23_ForLoop_CompileAll()
		{
			var s = @"
var sum = 0
for i := 0; i < 5; i++ { sum += i }
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test24_ForWhileStyle()
		{
			var s = @"
var sum = 0
var i = 0
for i < 5 {
    sum += i
    i++
}
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test24_ForWhileStyle_CompileAll()
		{
			var s = @"
var sum = 0
var i = 0
for i < 5 {
    sum += i
    i++
}
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test25_ForInfinite()
		{
			var s = @"
var sum = 0
var i = 0
for {
    sum += i
    i++
    if i >= 5 {
        break
    }
}
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test25_ForInfinite_CompileAll()
		{
			var s = @"
var sum = 0
var i = 0
for {
    sum += i
    i++
    if i >= 5 {
        break
    }
}
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test26_ForWithContinue()
		{
			var s = @"
var sum = 0
for i := 0; i < 5; i++ {
    if i == 2 {
        continue
    }
    sum += i
}
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(8, script.Eval(s));
		}

		[TestMethod]
		public void Test26_ForWithContinue_CompileAll()
		{
			var s = @"
var sum = 0
for i := 0; i < 5; i++ {
    if i == 2 {
        continue
    }
    sum += i
}
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(8, script.Eval(s));
		}

		[TestMethod]
		public void Test27_ForWithBreak()
		{
			var s = @"
var sum = 0
for i := 0; i < 10; i++ {
    if i == 5 {
        break
    }
    sum += i
}
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test27_ForWithBreak_CompileAll()
		{
			var s = @"
var sum = 0
for i := 0; i < 10; i++ {
    if i == 5 {
        break
    }
    sum += i
}
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test28_Switch()
		{
			var s = @"
var a = 2
var result = 0
switch a {
case 1:
    result = 100
case 2:
    result = 200
	var ss = 'hello'
case 3:
    result = 300
default:
    result = 0
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(200, script.Eval(s));
		}

		[TestMethod]
		public void Test28_Switch_CompileAll()
		{
			var s = @"
var a = 2
var result = 0
switch a {
case 1:
    result = 100
case 2:
    result = 200
	var ss = 'hello'
case 3:
    result = 300
default:
    result = 0
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(200, script.Eval(s));
		}

		[TestMethod]
		public void Test29_SwitchMultipleCases()
		{
			var s = @"
var a = 3
var result = 0
switch a {
case 1, 2:
    result = 100
case 3, 4:
    result = 200
default:
    result = 0
}
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(200, script.Eval(s));
		}

		[TestMethod]
		public void Test29_SwitchMultipleCases_CompileAll()
		{
			var s = @"
var a = 3
var result = 0
switch a {
case 1, 2:
    result = 100
case 3, 4:
    result = 200
default:
    result = 0
}
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(200, script.Eval(s));
		}

//		[TestMethod]
//		public void Test30_SwitchFallthrough()
//		{
//			var s = @"
//var a = 1
//var result = 0
//switch a {
//case 1:
//    result = 100
//    fallthrough
//case 2:
//    result = result + 50
//default:
//    result = 0
//}
//result
//";
//			var script = new Script();
//			script.Context.Langs = new[] { "go" };
//			Assert.AreEqual(150, script.Eval(s));
//		}

//		[TestMethod]
//		public void Test30_SwitchFallthrough_CompileAll()
//		{
//			var s = @"
//var a = 1
//var result = 0
//switch a {
//case 1:
//    result = 100
//    fallthrough
//case 2:
//    result = result + 50
//default:
//    result = 0
//}
//result
//";
//			var script = new Script();
//			script.Options.CompileMode = ECompileMode.All;
//			script.Context.Langs = new[] { "go" };
//			Assert.AreEqual(150, script.Eval(s));
//		}

		[TestMethod]
		public void Test31_FunctionDeclaration()
		{
			var s = @"
func add(a int, b int) int {
    return a + b
}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(30, script.Eval("add(10, 20)"));
			Assert.AreEqual(50, script.Eval("add(20, 30)"));
		}

		[TestMethod]
		public void Test31_FunctionDeclaration_CompileAll()
		{
			var s = @"
func add(a int, b int) int {
    return a + b
}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(30, script.Eval("add(10, 20)"));
			Assert.AreEqual(50, script.Eval("add(20, 30)"));
		}

		[TestMethod]
		public void Test32_FunctionMultipleReturn()
		{
			var s = @"
func swap(a int, b int) (int, int) {
    return b, a
}
var x, y = swap(10, 20)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(20, script.Eval("x"));
			Assert.AreEqual(10, script.Eval("y"));
		}

		[TestMethod]
		public void Test32_FunctionMultipleReturn_CompileAll()
		{
			var s = @"
func swap(a int, b int) (int, int) {
    return b, a
}
var x, y = swap(10, 20)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(20, script.Eval("x"));
			Assert.AreEqual(10, script.Eval("y"));
		}

		[TestMethod]
		public void Test33_FunctionNoParams()
		{
			var s = @"
func getValue() int {
    return 42
}
getValue()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(42, script.Eval(s));
		}

		[TestMethod]
		public void Test33_FunctionNoParams_CompileAll()
		{
			var s = @"
func getValue() int {
    return 42
}
getValue()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(42, script.Eval(s));
		}

		[TestMethod]
		public void Test34_FunctionExpression()
		{
			var s = @"
var fn = func(a int, b int) int {
    return a * b
}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(12, script.Eval("fn(3, 4)"));
			Assert.AreEqual(20, script.Eval("fn(4, 5)"));
		}

		[TestMethod]
		public void Test34_FunctionExpression_CompileAll()
		{
			var s = @"
var fn = func(a int, b int) int {
    return a * b
}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(12, script.Eval("fn(3, 4)"));
			Assert.AreEqual(20, script.Eval("fn(4, 5)"));
		}

		[TestMethod]
		public void Test35_Closure()
		{
			var s = @"
var add = func(a int) func(int) int {
    return func(b int) int {
        return a + b
    }
}
var add10 = add(10)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(15, script.Eval("add10(5)"));
			Assert.AreEqual(25, script.Eval("add10(15)"));
		}

		[TestMethod]
		public void Test35_Closure_CompileAll()
		{
			var s = @"
var add = func(a int) func(int) int {
    return func(b int) int {
        return a + b
    }
}
var add10 = add(10)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(15, script.Eval("add10(5)"));
			Assert.AreEqual(25, script.Eval("add10(15)"));
		}

		[TestMethod]
		public void Test36_StringLen()
		{
			var s = @"
var s = 'hello'
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(s)"));
		}

		[TestMethod]
		public void Test36_StringLen_CompileAll()
		{
			var s = @"
var s = 'hello'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(s)"));
		}

		[TestMethod]
		public void Test37_StringIndex()
		{
			var s = @"
var s = 'hello'
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual('h', script.Eval("s[0]"));
			Assert.AreEqual('o', script.Eval("s[4]"));
		}

		[TestMethod]
		public void Test37_StringIndex_CompileAll()
		{
			var s = @"
var s = 'hello'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual('h', script.Eval("s[0]"));
			Assert.AreEqual('o', script.Eval("s[4]"));
		}

		[TestMethod]
		public void Test38_ArrayDeclaration()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(5, script.Eval("arr[4]"));
			Assert.AreEqual(6, script.Eval("arr[0] + arr[4]"));
		}

		[TestMethod]
		public void Test38_ArrayDeclaration_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(5, script.Eval("arr[4]"));
			Assert.AreEqual(6, script.Eval("arr[0] + arr[4]"));
		}

		[TestMethod]
		public void Test39_ArrayIndex()
		{
			var s = @"
var arr = [3]int{10, 20, 30}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(20, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test39_ArrayIndex_CompileAll()
		{
			var s = @"
var arr = [3]int{10, 20, 30}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(20, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test40_EmptyArray()
		{
			var s = @"
var arr = [0]int{}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test40_EmptyArray_CompileAll()
		{
			var s = @"
var arr = [0]int{}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test41_MapDeclaration()
		{
			var s = @"
var m = make(map[string]int)
m['a'] = 1
m['b'] = 2
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("m['a']"));
			Assert.AreEqual(2, script.Eval("m['b']"));
			Assert.AreEqual(3, script.Eval("m['a'] + m['b']"));
		}

		[TestMethod]
		public void Test41_MapDeclaration_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
m['a'] = 1
m['b'] = 2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("m['a']"));
			Assert.AreEqual(2, script.Eval("m['b']"));
			Assert.AreEqual(3, script.Eval("m['a'] + m['b']"));
		}

		[TestMethod]
		public void Test42_EmptyMap()
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
		public void Test42_EmptyMap_CompileAll()
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
		public void Test43_SliceDeclaration()
		{
			var s = @"
var s = []int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(s)"));
		}

		[TestMethod]
		public void Test43_SliceDeclaration_CompileAll()
		{
			var s = @"
var s = []int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(s)"));
		}

		[TestMethod]
		public void Test44_PointerBasic()
		{
			var s = @"
var a = 10
var p = &a
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("*p"));
		}

		[TestMethod]
		public void Test44_PointerBasic_CompileAll()
		{
			var s = @"
var a = 10
var p = &a
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("*p"));
		}
	}
}
