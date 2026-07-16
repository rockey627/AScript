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
		public void Test01_Function_Basic()
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
		public void Test02_Function_NoArgs()
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
		public void Test03_Function_MultiArgs()
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
		public void Test04_Function_NestedCall()
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
		public void Test05_Function_Recursive()
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
		public void Test06_Function_Recursive2()
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
		public void Test07_Function_CompileMode()
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
		public void Test08_Function_Variable()
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
		public void Test09_Function_ReturnString()
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
		public void Test10_Function_ReturnBool()
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
		public void Test11_Arithmetic_Operators()
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
		public void Test12_Arithmetic_CompoundAssign()
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
		public void Test13_Logic_Operators()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(true, script.Eval("true && true"));
			Assert.AreEqual(false, script.Eval("true && false"));
			Assert.AreEqual(true, script.Eval("true || false"));
			Assert.AreEqual(false, script.Eval("false || false"));
			Assert.AreEqual(false, script.Eval("!true"));
			Assert.AreEqual(false, script.Eval("var a=false;var b=false; a||b"));
		}

		[TestMethod]
		public void Test13_Logic_Operators_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(true, script.Eval("true && true"));
			Assert.AreEqual(false, script.Eval("true && false"));
			Assert.AreEqual(true, script.Eval("true || false"));
			Assert.AreEqual(false, script.Eval("false || false"));
			Assert.AreEqual(false, script.Eval("!true"));
			Assert.AreEqual(false, script.Eval("var a=false;var b=false; a||b"));
		}

		[TestMethod]
		public void Test14_Comparison_Operators()
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
		public void Test15_String_Operations()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual("helloworld", script.Eval("'hello' + 'world'"));
			Assert.AreEqual(5L, script.Eval("'hello'.length"));
			Assert.AreEqual("ell", script.Eval("'hello'.substring(1, 4)"));
		}

		[TestMethod]
		public void Test16_Array_Basic()
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
		public void Test17_Array_Index()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(2L, script.Eval("[1, 2, 3][1]"));
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][2]"));
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][-1]"));
		}

		[TestMethod]
		public void Test18_Array_Length()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(3L, script.Eval("[1, 2, 3].length"));
			Assert.AreEqual(0L, script.Eval("[].length"));
		}

		[TestMethod]
		public void Test19_Null()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(null, script.Eval("null"));
		}

		[TestMethod]
		public void Test20_Bool()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(true, script.Eval("true"));
			Assert.AreEqual(false, script.Eval("false"));
		}

		[TestMethod]
		public void Test21_If_Condition()
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
		public void Test22_If_ElseIf()
		{
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
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("less than 20", script.Eval(code));
		}

		[TestMethod]
		public void Test22_If_ElseIf_CompileAll()
		{
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
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("less than 20", script.Eval(code));
		}

		[TestMethod]
		public void Test23_While_Loop()
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
		public void Test23_While_Loop_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
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
		public void Test23_While_Loop2()
		{
			string code = @"
var i = 0;
var sum = 0;
while (i < 5) {
	sum += i;
	i++;
}
sum;
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test23_While_Loop2_CompileAll()
		{
			string code = @"
var i = 0;
var sum = 0;
while (i < 5) {
	sum += i;
	i++;
}
sum;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test24_For_Loop()
		{
			string code = @"
var sum = 0;
for (var i = 0; i < 5; i = i + 1) {
	sum = sum + i;
}
sum;
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test24_For_Loop_CompileAll()
		{

			string code = @"
var sum = 0;
for (var i = 0; i < 5; i = i + 1) {
	sum = sum + i;
}
sum;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test24_For_Loop2()
		{
			string code = @"
var sum = 0;
for (var i = 0; i < 5; i++) {
	sum += i;
}
sum;
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test24_For_Loop2_CompileAll()
		{

			string code = @"
var sum = 0;
for (var i = 0; i < 5; i++) {
	sum += i;
}
sum;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test25_Return_Statement()
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
		public void Test25_Return_InFunction()
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
		public void Test26_Break_Statement()
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
		public void Test27_Continue_Statement()
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
		public void Test28_VariableDeclaration_Var()
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
		public void Test29_VariableDeclaration_Let()
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
		public void Test30_VariableDeclaration_Const()
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
		public void Test31_Increment_Decrement()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual(6L, script.Eval("i = 5; i++; i"));
			Assert.AreEqual(4L, script.Eval("i = 5; i--; i"));
		}

		[TestMethod]
		public void Test32_Ternary_Operator()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual("yes", script.Eval("true ? 'yes' : 'no'"));
			Assert.AreEqual("no", script.Eval("false ? 'yes' : 'no'"));
		}

		[TestMethod]
		public void Test33_NullCoalescing()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			Assert.AreEqual("default", script.Eval("null ?? 'default'"));
			Assert.AreEqual("value", script.Eval("'value' ?? 'default'"));
		}

		[TestMethod]
		public void Test34_Optional_Chaining()
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
		public void Test35_Function_MultipleStatements()
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
		public void Test36_Function_CallAfterDefine()
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
		public void Test37_Function_ReuseDefine()
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
		public void Test38_Function_ClosureLike()
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
		public void Test39_Function_CallAnotherFunction()
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
		public void Test40_MultipleFunction_Definitions()
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
		public void Test41_Function_ReDefine()
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
		public void Test42_Function_EmptyArgs()
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
		public void Test43_Function_ComplexExpression()
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
		public async Task Test44_Function_AsyncCompileMode()
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
		public void Test45_Function_AssignToVariable()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var fn = function (x) {
	return x * 2;
}
fn(5);
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test46_Function_AssignToVariable_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var fn = function (x) {
	return x * 2;
}
fn(5);
";
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test47_for_of_CompileAll()
		{
			string code = @"
var list = ['tom', 'jim', 'john'];
var s = '';
for(var item of list) {
	s += item + ',';
}
s
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("tom,jim,john,", script.Eval(code));
		}

		[TestMethod]
		public void Test47_for_of()
		{
			string code = @"
var list = ['tom', 'jim', 'john'];
var s = '';
for(var item of list) {
	s += item + ',';
}
s
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("tom,jim,john,", script.Eval(code));
		}

		[TestMethod]
		public void Test48_for_in_4()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var list = ['tom', 'jim', 'john'];
var s = '';
for(var i in list) {
	s += list[i] + ',';
}
s
";
			Assert.AreEqual("tom,jim,john,", script.Eval(code));
		}

		[TestMethod]
		public void Test48_for_in_3()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var list = ['tom', 'jim', 'john'];
var s = '';
for(var i in list) {
	s += list[i] + ',';
}
s
";
			Assert.AreEqual("tom,jim,john,", script.Eval(code));
		}

		[TestMethod]
		public void Test48_for_in_CompileAll()
		{
			string code = @"
var person = { 'name': 'Bob', age: 25 };
var s = '';
for(var key in person) {
	s += key + ':' + person[key] + ',';
}
s
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("name:Bob,age:25,", script.Eval(code));
		}

		[TestMethod]
		public void Test48_for_in()
		{
			string code = @"
var person = { 'name': 'Bob', age: 25 };
var s = '';
for(var key in person) {
	s += key + ':' + person[key] + ',';
}
s
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("name:Bob,age:25,", script.Eval(code));
		}

		[TestMethod]
		public void Test49_Object_Basic_8()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var person = { 'name 2': 'Bob', age: 25 };
person['name 2'] + person.age;
";
			Assert.AreEqual("Bob25", script.Eval(code));
		}

		[TestMethod]
		public void Test49_Object_Basic_7()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var person = { 'name 2': 'Bob', age: 25 };
person['name 2'] + person.age;
";
			Assert.AreEqual("Bob25", script.Eval(code));
		}

		[TestMethod]
		public void Test49_Object_Basic_6()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var person = { 'name+2': 'Bob', age: 25 };
person['name+2'] + person.age;
";
			Assert.AreEqual("Bob25", script.Eval(code));
		}

		[TestMethod]
		public void Test49_Object_Basic_5()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var person = { 'name+2': 'Bob', age: 25 };
person['name+2'] + person.age;
";
			Assert.AreEqual("Bob25", script.Eval(code));
		}

		[TestMethod]
		public void Test49_Object_Basic_4()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var person = { 'name': 'Bob', age: 25 };
person['name'] + person.age;
";
			Assert.AreEqual("Bob25", script.Eval(code));
		}

		[TestMethod]
		public void Test49_Object_Basic_3()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var person = { 'name': 'Bob', age: 25 };
person['name'] + person.age;
";
			Assert.AreEqual("Bob25", script.Eval(code));
		}

		[TestMethod]
		public void Test49_Object_Basic_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var person = { name: 'Bob', age: 25 };
person.name;
";
			Assert.AreEqual("Bob", script.Eval(code));
		}

		[TestMethod]
		public void Test49_Object_Basic()
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
		public void Test50_Object_PropertyAccess_8()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj.z=30;
obj.x + obj['y'] + obj['z'];
";
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test50_Object_PropertyAccess_7()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj.z=30;
obj.x + obj['y'] + obj['z'];
";
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test50_Object_PropertyAccess_6()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj['z']=30;
obj.x + obj['y'] + obj.z;
";
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test50_Object_PropertyAccess_5()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj['z']=30;
obj.x + obj['y'] + obj.z;
";
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void Test50_Object_PropertyAccess_4()
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
		public void Test50_Object_PropertyAccess_3()
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
		public void Test50_Object_PropertyAccess_2()
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
		public void Test50_Object_PropertyAccess()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var obj = { x: 10, y: 20 };
obj.x + obj.y;
";
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public async Task Test51_SetTimeout_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var result = 0;
function onTimeout() {
	result = 42;
}
var handle = setTimeout(onTimeout, 50);
handle;
";
			var handle = await script.EvalAsync<object>(code);
			Assert.IsNotNull(handle);

			// Wait for the timeout to fire
			await Task.Delay(100);
			Assert.AreEqual(42L, script.Eval("result"));
		}

		[TestMethod]
		public async Task Test51_SetTimeout_Basic_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
static var result = 0;
function onTimeout() {
	result = 42;
}
var handle = setTimeout(onTimeout, 50);
handle;
";
			var handle = await script.EvalAsync<object>(code);
			Assert.IsNotNull(handle);

			// Wait for the timeout to fire
			await Task.Delay(100);
			Assert.AreEqual(42L, script.Eval("result"));
		}

		[TestMethod]
		public async Task Test52_SetTimeout_WithArgs()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var result = 0;
function onTimeout(a, b) {
	result = a + b;
}
var handle = setTimeout(onTimeout, 50, 10, 20);
handle;
";
			await script.EvalAsync<object>(code);
			await Task.Delay(100);
			Assert.AreEqual(30L, script.Eval("result"));
		}

		[TestMethod]
		public async Task Test53_SetTimeout_ClearBeforeFire()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var result = 0;
function onTimeout() {
	result = 42;
}
var handle = setTimeout(onTimeout, 100);
clearTimeout(handle);
result;
";
			// clearTimeout should prevent the callback from firing
			var initialResult = await script.EvalAsync<object>(code);
			Assert.AreEqual(0L, initialResult);

			// Wait longer than the original timeout, result should still be 0
			await Task.Delay(200);
			Assert.AreEqual(0L, script.Eval("result"));
		}

		[TestMethod]
		public async Task Test54_SetTimeout_MultipleTimeouts()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var results = [];
function addResult(val) {
	results.push(val);
}
var h1 = setTimeout(function() { addResult(1); }, 30);
var h2 = setTimeout(function() { addResult(2); }, 60);
var h3 = setTimeout(function() { addResult(3); }, 90);
[h1, h2, h3];
";
			await script.EvalAsync<object>(code);

			await Task.Delay(50);
			var results30 = script.Eval<List<object>>("results");
			Assert.AreEqual(1L, results30[0]);

			await Task.Delay(70);
			var results90 = script.Eval<List<object>>("results");
			Assert.AreEqual(3L, results90[2]);
		}

		[TestMethod]
		public async Task Test54_SetTimeout_MultipleTimeouts2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var results = [];
function addResult(val) {
	results.push(val);
}
var h1 = setTimeout(function() { addResult(1); }, 30);
var h2 = setTimeout(function() { addResult(2); }, 60);
var h3 = setTimeout(function() { addResult(3); }, 90);
[h1, h2, h3];
";
			await script.EvalAsync<object>(code);

			await Task.Delay(45);
			var results30 = script.Eval<List<object>>("results");
			Assert.AreEqual(1L, results30[0]);

			await Task.Delay(70);
			var results90 = script.Eval<List<object>>("results");
			Assert.AreEqual(3L, results90[2]);
		}

		[TestMethod]
		public async Task Test55_SetInterval_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var count = 0;
function onInterval() {
	count = count + 1;
}
var handle = setInterval(onInterval, 30);
handle;
";
			var handle = await script.EvalAsync<object>(code);
			Assert.IsNotNull(handle);

			await Task.Delay(110);
			script.Eval("clearInterval(handle)");

			// Should have fired at least 3 times (30ms interval, waited 110ms)
			var count = script.Eval("count");
			Assert.IsTrue((long)count >= 3, $"Expected at least 3 fires, got {count}");
		}

		[TestMethod]
		public async Task Test56_SetInterval_WithArgs()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var result = 0;
function onInterval(a, b) {
	result = result + a + b;
}
var handle = setInterval(onInterval, 30, 1, 2);
handle;
";
			var handle = await script.EvalAsync<object>(code);
			Assert.IsNotNull(handle);

			await Task.Delay(100);
			script.Eval("clearInterval(handle)");

			// Each fire adds 1+2=3, at least 3 fires = at least 9
			var result = script.Eval("result");
			Assert.IsTrue((long)result >= 9, $"Expected at least 9, got {result}");
		}

		[TestMethod]
		public async Task Test56_SetInterval_WithArgs_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
static var result = 0;
function onInterval(a, b) {
	result = result + a + b;
}
var handle = setInterval(onInterval, 30, 1, 2);
handle;
";
			var handle = await script.EvalAsync<object>(code);
			Assert.IsNotNull(handle);

			await Task.Delay(100);
			script.Eval("clearInterval(handle)");

			// Each fire adds 1+2=3, at least 3 fires = at least 9
			var result = script.Eval("result");
			Assert.IsTrue((long)result >= 9, $"Expected at least 9, got {result}");
		}

		[TestMethod]
		public async Task Test57_SetInterval_ClearStopsFiring()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var count = 0;
function onInterval() {
	count = count + 1;
}
var handle = setInterval(onInterval, 30);
handle;
";
			var handle = await script.EvalAsync<object>(code);

			await Task.Delay(80);
			script.Eval("clearInterval(handle)");
			var countAfterClear = script.Eval("count");

			await Task.Delay(100);
			var countAfterWait = script.Eval("count");

			// Count should not increase after clearInterval
			Assert.AreEqual(countAfterClear, countAfterWait);
		}

		[TestMethod]
		public async Task Test58_SetInterval_ReturnsTimer()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var count = 0;
function handler() { count = 1; }
var timer = setInterval(handler, 40);
timer;
";
			var timerObj = await script.EvalAsync<object>(code);
			Assert.IsNotNull(timerObj);
			Assert.AreEqual("Timer", timerObj.GetType().Name);

			await Task.Delay(70);
			script.Eval("clearInterval(timer)");
			Assert.AreEqual(1L, script.Eval("count"));
		}

		[TestMethod]
		public async Task Test59_ClearTimeout_InvalidHandle()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			// clearTimeout with null or already cancelled source should not throw
			string code = @"
var result = 'ok';
try {
	clearTimeout(null);
} catch(e) {
	result = 'error';
}
result;
";
			var result = await script.EvalAsync<object>(code);
			Assert.AreEqual("ok", result);
		}

		[TestMethod]
		public async Task Test59_ClearTimeout_InvalidHandle_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			// clearTimeout with null or already cancelled source should not throw
			string code = @"
var result = 'ok';
try {
	clearTimeout(null);
} catch(e) {
	result = 'error';
}
result;
";
			var result = await script.EvalAsync<object>(code);
			Assert.AreEqual("ok", result);
		}

		[TestMethod]
		public async Task Test60_ClearInterval_InvalidHandle()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var result = 'ok';
try {
	clearInterval(null);
} catch(e) {
	result = 'error';
}
result;
";
			var result = await script.EvalAsync<object>(code);
			Assert.AreEqual("ok", result);
		}

		[TestMethod]
		public async Task Test60_ClearInterval_InvalidHandle_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var result = 'ok';
try {
	clearInterval(null);
} catch(e) {
	result = 'error';
}
result;
";
			var result = await script.EvalAsync<object>(code);
			Assert.AreEqual("ok", result);
		}

		[TestMethod]
		public async Task Test61_SetTimeout_ZeroDelay()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var result = 0;
function onTimeout() {
	result = 99;
}
var handle = setTimeout(onTimeout, 0);
handle;
";
			await script.EvalAsync<object>(code);
			// Even with 0 delay, the callback is scheduled asynchronously
			await Task.Delay(10);
			Assert.AreEqual(99L, script.Eval("result"));
		}

		[TestMethod]
		public async Task Test62_SetInterval_ZeroInterval()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var count = 0;
function onInterval() {
	count = count + 1;
}
var handle = setInterval(onInterval, 0);
handle;
";
			var handle = await script.EvalAsync<object>(code);
			Assert.IsNotNull(handle);

			// With 0 interval, it should fire rapidly - let it run briefly then clear
			await Task.Delay(1250);
			script.Eval("clearInterval(handle)");

			// Should have fired many times
			var count = script.Eval("count");
			Assert.IsTrue((long)count > 10, $"Expected many fires with 0 interval, got {count}");
		}

		[TestMethod]
		public void Test63_OrElse()
		{
			string code = @"
var a = '';
a || 'hello'
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval(code));
		}

		[TestMethod]
		public void Test63_OrElse_CompileAll()
		{
			string code = @"
var a = '';
a || 'hello'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval(code));
		}

		[TestMethod]
		public void Test63_OrElse2()
		{
			string code = @"
var a = 'abc';
var b = 'ok';
a || (b+='hello')
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("abc", script.Eval(code));
			Assert.AreEqual("ok", script.Eval("b"));
		}

		[TestMethod]
		public void Test63_OrElse2_CompileAll()
		{
			string code = @"
var a = 'abc';
var b = 'ok';
a || (b+='hello')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("abc", script.Eval(code));
			Assert.AreEqual("ok", script.Eval("b"));
		}

		[TestMethod]
		public void Test64_AndAlso()
		{
			string code = @"
var a = '';
a && 'hello'
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(false, script.Eval(code));
		}

		[TestMethod]
		public void Test64_AndAlso_CompileAll()
		{
			string code = @"
var a = '';
a && 'hello'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(false, script.Eval(code));
		}

		[TestMethod]
		public void Test64_AndAlso2()
		{
			string code = @"
var a = 'abc';
var b = 'ok';
a && (b+='hello')
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval(code));
			Assert.AreEqual("okhello", script.Eval("b"));
		}

		[TestMethod]
		public void Test64_AndAlso2_CompileAll()
		{
			string code = @"
var a = 'abc';
var b = 'ok';
a && (b+='hello')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval(code));
			Assert.AreEqual("okhello", script.Eval("b"));
		}

		[TestMethod]
		public void Test65_if_object()
		{
			var s = @"
var a = null;
var b = 0;
var c = 1;
if (a) return 1;
if (b) return 2;
if (c) return 3;
return 4;
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval(s));
		}

		[TestMethod]
		public void Test65_if_object_CompileAll()
		{
			var s = @"
var a = null;
var b = 0;
var c = 1;
if (a) return 1;
if (b) return 2;
if (c) return 3;
return 4;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval(s));
		}

		[TestMethod]
		public void Test65_if_object2()
		{
			var s = @"
var a = null;
var b = 0.0;
var c = 1.1;
if (a) return 1;
if (b) return 2;
if (c) return 3;
return 4;
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval(s));
		}

		[TestMethod]
		public void Test65_if_object2_CompileAll()
		{
			var s = @"
var a = null;
var b = 0.0;
var c = 1.1;
if (a) return 1;
if (b) return 2;
if (c) return 3;
return 4;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval(s));
		}

		[TestMethod]
		public void Test66()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1L, script.Eval("5?1:2"));
			Assert.AreEqual(2L, script.Eval("0?1:2"));
			Assert.AreEqual(2L, script.Eval("0.0?1:2"));
			Assert.AreEqual(1L, script.Eval("1.2?1:2"));
			Assert.AreEqual(1L, script.Eval("'hello'?1:2"));
			Assert.AreEqual(2L, script.Eval("''?1:2"));
			Assert.AreEqual(2L, script.Eval("null?1:2"));
			Assert.AreEqual(1L, script.Eval("true?1:2"));
			Assert.AreEqual(2L, script.Eval("false?1:2"));
		}

		[TestMethod]
		public void Test66_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1L, script.Eval("5?1:2"));
			Assert.AreEqual(2L, script.Eval("0?1:2"));
			Assert.AreEqual(2L, script.Eval("0.0?1:2"));
			Assert.AreEqual(1L, script.Eval("1.2?1:2"));
			Assert.AreEqual(1L, script.Eval("'hello'?1:2"));
			Assert.AreEqual(2L, script.Eval("''?1:2"));
			Assert.AreEqual(2L, script.Eval("null?1:2"));
			Assert.AreEqual(1L, script.Eval("true?1:2"));
			Assert.AreEqual(2L, script.Eval("false?1:2"));
		}
	}
}