using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptCommonTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["js"] = JavaScriptLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("js");
		}

		[TestMethod]
		public void TestFunction_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			
			// 基本函数定义和调用
			string code = @"
function add(a, b) {
	return a + b;
}
add(3, 5);
";
			Assert.AreEqual(8L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_NoArgs()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// 无参数函数
			string code = @"
function getValue() {
	return 100;
}
getValue();
";
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_MultiArgs()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// 多参数函数
			string code = @"
function sum(a, b, c, d, e) {
	return a + b + c + d + e;
}
sum(1, 2, 3, 4, 5);
";
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_NestedCall()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// 函数嵌套调用
			string code = @"
function double(x) {
	return x * 2;
}
function addFive(x) {
	return x + 5;
}
double(addFive(3));
";
			Assert.AreEqual(16L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_Recursive()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// 递归函数 - 阶乘
			string code = @"
function factorial(n) {
	if (n <= 1) {
		return 1;
	}
	return n * factorial(n - 1);
}
factorial(5);
";
			Assert.AreEqual(120L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_Recursive2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// 递归函数 - 斐波那契
			string code = @"
function fib(n) {
	if (n <= 1) {
		return n;
	}
	return fib(n - 1) + fib(n - 2);
}
fib(10);
";
			Assert.AreEqual(55L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_CompileMode()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
function multiply(a, b) {
	return a * b;
}
multiply(7, 8);
";
			Assert.AreEqual(56L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_Variable()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// 函数作为变量
			string code = @"
function greet(name) {
	return 'Hello, ' + name;
}
var message = greet('World');
message;
";
			Assert.AreEqual("Hello, World", script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_ReturnString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function getName() {
	return 'Alice';
}
getName();
";
			Assert.AreEqual("Alice", script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_ReturnBool()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function isAdult(age) {
	return age >= 18;
}
isAdult(20);
";
			Assert.AreEqual(true, script.Eval(code));
		}

		[TestMethod]
		public void TestArithmetic_Operators()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(15L, script.Eval("10 + 5"));
			Assert.AreEqual(5L, script.Eval("10 - 5"));
			Assert.AreEqual(50L, script.Eval("10 * 5"));
			Assert.AreEqual(2.0, script.Eval("10 / 5"));
			Assert.AreEqual(2L, script.Eval("17 % 5"));
			Assert.AreEqual(1024L, script.Eval("2 ** 10"));
		}

		[TestMethod]
		public void TestArithmetic_CompoundAssign()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(15L, script.Eval("n = 10; n += 5; n"));
			Assert.AreEqual(5L, script.Eval("n = 10; n -= 5; n"));
			Assert.AreEqual(50L, script.Eval("n = 10; n *= 5; n"));
			Assert.AreEqual(2.0, script.Eval("n = 10; n /= 5; n"));
			Assert.AreEqual(2L, script.Eval("n = 17; n %= 5; n"));
		}

		[TestMethod]
		public void TestLogic_Operators()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(true, script.Eval("true && true"));
			Assert.AreEqual(false, script.Eval("true && false"));
			Assert.AreEqual(true, script.Eval("true || false"));
			Assert.AreEqual(false, script.Eval("false || false"));
			Assert.AreEqual(false, script.Eval("!true"));
		}

		[TestMethod]
		public void TestComparison_Operators()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(true, script.Eval("10 == 10"));
			Assert.AreEqual(false, script.Eval("10 == 5"));
			Assert.AreEqual(true, script.Eval("10 != 5"));
			Assert.AreEqual(true, script.Eval("10 > 5"));
			Assert.AreEqual(false, script.Eval("10 < 5"));
			Assert.AreEqual(true, script.Eval("10 >= 10"));
			Assert.AreEqual(true, script.Eval("10 <= 10"));
		}

		[TestMethod]
		public void TestString_Operations()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual("helloworld", script.Eval("'hello' + 'world'"));
			Assert.AreEqual(5, script.Eval("'hello'.length"));
			Assert.AreEqual("ell", script.Eval("'hello'.substring(1, 4)"));
		}

		[TestMethod]
		public void TestArray_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			var result = script.Eval("[1, 2, 3]");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(3L, arr[2]);
		}

		[TestMethod]
		public void TestArray_Index()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(2L, script.Eval("[1, 2, 3][1]"));
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][2]"));
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][-1]"));
		}

		[TestMethod]
		public void TestArray_Length()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(3L, script.Eval("[1, 2, 3].length"));
			Assert.AreEqual(0L, script.Eval("[].length"));
		}

		[TestMethod]
		public void TestNull()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(null, script.Eval("null"));
		}

		[TestMethod]
		public void TestBool()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(true, script.Eval("true"));
			Assert.AreEqual(false, script.Eval("false"));
		}

		[TestMethod]
		public void TestIf_Condition()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var result = '';
if (true) {
	result = 'true branch';
} else {
	result = 'false branch';
}
result;
";
			Assert.AreEqual("true branch", script.Eval(code));
		}

		[TestMethod]
		public void TestIf_ElseIf()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var n = 15;
var result = '';
if (n < 10) {
	result = 'less than 10';
} else if (n < 20) {
	result = 'less than 20';
} else {
	result = '20 or more';
}
result;
";
			Assert.AreEqual("less than 20", script.Eval(code));
		}

		[TestMethod]
		public void TestWhile_Loop()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var i = 0;
var sum = 0;
while (i < 5) {
	sum = sum + i;
	i = i + 1;
}
sum;
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void TestFor_Loop()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var sum = 0;
for (var i = 0; i < 5; i = i + 1) {
	sum = sum + i;
}
sum;
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void TestReturn_Statement()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function test() {
	return 42;
}
test();
";
			Assert.AreEqual(42L, script.Eval(code));
		}

		[TestMethod]
		public void TestReturn_InFunction()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function calculate(x) {
	if (x > 0) {
		return x * 2;
	}
	return x;
}
calculate(5);
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void TestBreak_Statement()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var i = 0;
var sum = 0;
while (i < 10) {
	if (i >= 5) {
		break;
	}
	sum = sum + i;
	i = i + 1;
}
sum;
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void TestContinue_Statement()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var sum = 0;
for (var i = 0; i < 5; i = i + 1) {
	if (i == 2) {
		continue;
	}
	sum = sum + i;
}
sum;
";
			// 0 + 1 + 3 + 4 = 8
			Assert.AreEqual(8L, script.Eval(code));
		}

		[TestMethod]
		public void TestVariableDeclaration_Var()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var x = 10;
var y = 20;
x + y;
";
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void TestVariableDeclaration_Let()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
let a = 100;
let b = 200;
a + b;
";
			Assert.AreEqual(300L, script.Eval(code));
		}

		[TestMethod]
		public void TestVariableDeclaration_Const()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
const PI = 3.14159;
const RADIUS = 5;
PI * RADIUS * RADIUS;
";
			Assert.AreEqual(78.53975, (double)script.Eval(code), 0.0001);
		}

		[TestMethod]
		public void TestIncrement_Decrement()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(6L, script.Eval("i = 5; i++; i"));
			Assert.AreEqual(4L, script.Eval("i = 5; i--; i"));
		}

		[TestMethod]
		public void TestTernary_Operator()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual("yes", script.Eval("true ? 'yes' : 'no'"));
			Assert.AreEqual("no", script.Eval("false ? 'yes' : 'no'"));
		}

		[TestMethod]
		public void TestNullCoalescing()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual("default", script.Eval("null ?? 'default'"));
			Assert.AreEqual("value", script.Eval("'value' ?? 'default'"));
		}

		[TestMethod]
		public void TestOptional_Chaining()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { name: 'Alice', address: { city: 'Beijing' } };
obj?.name;
";
			Assert.AreEqual("Alice", script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_MultipleStatements()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function calculate(x) {
	var a = x + 1;
	var b = a * 2;
	var c = b - 3;
	return c;
}
calculate(10);
";
			// (10 + 1) * 2 - 3 = 19
			Assert.AreEqual(19L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_CallAfterDefine()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// 先定义函数，后调用
			string code = @"
function square(n) {
	return n * n;
}
var result = square(7);
result;
";
			Assert.AreEqual(49L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_ReuseDefine()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function add(a, b) {
	return a + b;
}
add(1, 2) + add(3, 4);
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_ClosureLike()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// 函数内部定义函数（嵌套函数）
			string code = @"
function outer(x) {
	function inner(y) {
		return y * 2;
	}
	return inner(x) + 10;
}
outer(5);
";
			// inner(5) * 2 + 10 = 20
			Assert.AreEqual(20L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_CallAnotherFunction()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function first() {
	return 10;
}
function second() {
	return first() + 5;
}
second();
";
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void TestMultipleFunction_Definitions()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function add(a, b) {
	return a + b;
}
function multiply(a, b) {
	return a * b;
}
function subtract(a, b) {
	return a - b;
}
add(5, 3) + multiply(2, 4) + subtract(10, 3);
";
			// 8 + 8 + 7 = 23
			Assert.AreEqual(23L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_ReDefine()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function test() {
	return 1;
}
function test() {
	return 2;
}
test();
";
			// 后定义的覆盖先定义的
			Assert.AreEqual(2L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_EmptyArgs()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function identity() {
	return 99;
}
identity();
";
			Assert.AreEqual(99L, script.Eval(code));
		}

		[TestMethod]
		public void TestFunction_ComplexExpression()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function abs(n) {
	if (n < 0) {
		return -n;
	}
	return n;
}
abs(-5) + abs(3) + abs(0);
";
			// 5 + 3 + 0 = 8
			Assert.AreEqual(8L, script.Eval(code));
		}

		[TestMethod]
		public async Task TestFunction_AsyncCompileMode()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
function asyncAdd(a, b) {
	return a + b;
}
asyncAdd(100, 200);
";
			var result = await script.EvalAsync<object>(code);
			Assert.AreEqual(300L, result);
		}

		[TestMethod]
		public void TestFunction_AssignToVariable()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
function double(x) {
	return x * 2;
}
var fn = double;
fn(5);
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void TestObject_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var person = { name: 'Bob', age: 25 };
person.name;
";
			Assert.AreEqual("Bob", script.Eval(code));
		}

		[TestMethod]
		public void TestObject_PropertyAccess_4()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj.x + obj['y'];
";
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void TestObject_PropertyAccess_3()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj.x + obj['y'];
";
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void TestObject_PropertyAccess_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj.x + obj.y;
";
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void TestObject_PropertyAccess()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj.x + obj.y;
";
			Assert.AreEqual(30L, script.Eval(code));
		}
	}
}