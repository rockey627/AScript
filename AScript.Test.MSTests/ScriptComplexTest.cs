using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptComplexTest
	{
		[TestMethod]
		public void TestFile()
		{
			string filePath = @"./ScriptComplexTest_TestFile.txt";

			try
			{
				File.WriteAllText(filePath, "5+6*10");
				int cacheTime = -1;
				string cacheKey = filePath;
				var script = new Script();

				{
					string cacheVersion = File.GetLastWriteTime(filePath).ToFileTimeUtc().ToString();
					var result = script.Eval(() => File.OpenRead(filePath), cacheTime, cacheKey, cacheVersion);
					Assert.AreEqual(65, result);
				}

				{
					string cacheVersion = File.GetLastWriteTime(filePath).ToFileTimeUtc().ToString();
					var result = script.Eval(() => File.OpenRead(filePath), cacheTime, cacheKey, cacheVersion);
					Assert.AreEqual(65, result);
				}

				Thread.Sleep(100);
				File.WriteAllText(filePath, "8+6*10");

				{
					string cacheVersion = File.GetLastWriteTime(filePath).ToFileTimeUtc().ToString();
					var result = script.Eval(() => File.OpenRead(filePath), cacheTime, cacheKey, cacheVersion);
					Assert.AreEqual(68, result);
				}

				{
					string cacheVersion = File.GetLastWriteTime(filePath).ToFileTimeUtc().ToString();
					var result = script.Eval(() => File.OpenRead(filePath), cacheTime, cacheKey, cacheVersion);
					Assert.AreEqual(68, result);
				}
			}
			finally
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}
		}

		/// <summary>
		/// 测试复杂表达式计算
		/// </summary>
		[TestMethod]
		public void TestComplexExpression()
		{
			string s = @"
int a = 10;
int b = 20;
int c = 30;
int result = (a + b) * c - (a - b) * (c / 2) + (a % b);
result
";
			int expected = (10 + 20) * 30 - (10 - 20) * (30 / 2) + (10 % 20);

			var script = new Script();
			Assert.AreEqual(expected, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(expected, script.Eval(s));
		}

		/// <summary>
		/// 测试多层三元运算符
		/// </summary>
		[TestMethod]
		public void TestNestedTernary()
		{
			string s = @"int n = 5;string result = n > 10 ? ""大"" : (n > 5 ? ""中"" : ""小"");result";
			var script = new Script();
			Assert.AreEqual("小", script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("小", script.Eval(s));

			s = @"int n = 7;string result = n > 10 ? ""大"" : (n > 5 ? ""中"" : ""小"");result";
			script = new Script();
			Assert.AreEqual("中", script.Eval(s));

			s = @"int n = 15;string result = n > 10 ? ""大"" : (n > 5 ? ""中"" : ""小"");result";
			script = new Script();
			Assert.AreEqual("大", script.Eval(s));
		}

		/// <summary>
		/// 测试函数调用链
		/// </summary>
		[TestMethod]
		public void TestFunctionCallChain()
		{
			string s = @"
int add(int a, int b) { return a + b; }
int square(int a) { return a * a; }
int result = add(square(2), square(3));
result
";
			// 2^2 + 3^2 = 4 + 9 = 13
			var script = new Script();
			Assert.AreEqual(13, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(13, script.Eval(s));
		}

		/// <summary>
		/// 测试foreach循环处理集合
		/// </summary>
		[TestMethod]
		public void TestComplexForeach()
		{
			var list = new List<int> { 1, 2, 3, 4, 5 };
			string s = @"
int sum = 0;
foreach(var item in items) {
	if (item % 2 == 0) {
		sum += item * 2;
	} else {
		sum += item;
	}
}
sum
";
			int expected = 0;
			foreach (var item in list)
			{
				if (item % 2 == 0)
				{
					expected += item * 2;
				}
				else
				{
					expected += item;
				}
			}

			var script = new Script();
			script.Context.SetVar("items", list);
			Assert.AreEqual(expected, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("items", list);
			Assert.AreEqual(expected, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂if-else链
		/// </summary>
		[TestMethod]
		public void TestComplexIfElse()
		{
			string s = @"int score = 85;string grade;if (score >= 90) { grade = ""A""; } else if (score >= 80) { grade = ""B""; } else if (score >= 70) { grade = ""C""; } else if (score >= 60) { grade = ""D""; } else { grade = ""F""; } grade";
			var script = new Script();
			Assert.AreEqual("B", script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("B", script.Eval(s));

			// 测试其他分支
			s = @"int score = 95;string grade;if (score >= 90) { grade = ""A""; } else if (score >= 80) { grade = ""B""; } else if (score >= 70) { grade = ""C""; } else if (score >= 60) { grade = ""D""; } else { grade = ""F""; } grade";
			script = new Script();
			Assert.AreEqual("A", script.Eval(s));

			s = @"int score = 55;string grade;if (score >= 90) { grade = ""A""; } else if (score >= 80) { grade = ""B""; } else if (score >= 70) { grade = ""C""; } else if (score >= 60) { grade = ""D""; } else { grade = ""F""; } grade";
			script = new Script();
			Assert.AreEqual("F", script.Eval(s));
		}

		/// <summary>
		/// 测试复杂赋值表达式
		/// </summary>
		[TestMethod]
		public void TestComplexAssignment()
		{
			string s = @"
int a = 10;
int b = 20;
int c = 30;
a += b;
b *= 2;
c /= 5;
a + b + c
";
			int a = 10;
			int b = 20;
			int c = 30;
			a += b;
			b *= 2;
			c /= 5;
			int expected = a + b + c;

			var script = new Script();
			Assert.AreEqual(expected, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(expected, script.Eval(s));
		}

		/// <summary>
		/// 测试字符串连接操作
		/// </summary>
		[TestMethod]
		public void TestComplexString()
		{
			string s = @"
string name = ""World"";
int count = 5;
string result = ""Hello, "" + name + ""! You have "" + count + "" messages."";
result
";
			var script = new Script();
			Assert.AreEqual("Hello, World! You have 5 messages.", script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("Hello, World! You have 5 messages.", script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的null合并运算符
		/// </summary>
		[TestMethod]
		public void TestComplexNullCoalescing_2()
		{
			string s = @"string a = null;string b = null;string c = ""default"";string result = a ?? b ?? c ?? ""fallback"";result";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("default", script.Eval(s));

			s = @"string a = null;string b = ""value"";string c = null;string result = a ?? b ?? c ?? ""fallback"";result";
			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("value", script.Eval(s));

			s = @"string a = ""first"";string b = null;string c = null;string result = a ?? b ?? c ?? ""fallback"";result";
			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("first", script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的null合并运算符
		/// </summary>
		[TestMethod]
		public void TestComplexNullCoalescing()
		{
			string s = @"string a = null;string b = null;string c = ""default"";string result = a ?? b ?? c ?? ""fallback"";result";
			var script = new Script();
			Assert.AreEqual("default", script.Eval(s));

			s = @"string a = null;string b = ""value"";string c = null;string result = a ?? b ?? c ?? ""fallback"";result";
			script = new Script();
			Assert.AreEqual("value", script.Eval(s));

			s = @"string a = ""first"";string b = null;string c = null;string result = a ?? b ?? c ?? ""fallback"";result";
			script = new Script();
			Assert.AreEqual("first", script.Eval(s));
		}

		/// <summary>
		/// 测试复杂逻辑运算符组合
		/// </summary>
		[TestMethod]
		public void TestComplexLogicalOperators()
		{
			string s = @"
bool a = true;
bool b = false;
bool c = true;
bool result = (a && b) || (c && !b) || (a || b && c);
result
";
			// (true && false) || (true && !false) || (true || false && true)
			// = false || true || true = true
			var script = new Script();
			Assert.AreEqual(true, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(true, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂位运算符
		/// </summary>
		[TestMethod]
		public void TestComplexBitwiseOperators()
		{
			string s = @"
int a = 5;  // 101
int b = 3;  // 011
int result = (a & b) | (a ^ b) << 1;
result
";
			// (5 & 3) | (5 ^ 3) << 1 = (1) | (6 << 1) = 1 | 12 = 13
			var script = new Script();
			Assert.AreEqual(13, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(13, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的一元运算符
		/// </summary>
		[TestMethod]
		public void TestComplexUnaryOperators()
		{
			string s = @"
int a = 5;
int b = ++a + a++ + --a + a--;
a + b
";
			int a = 5;
			int b = ++a + a++ + --a + a--;
			int expected = a + b;

			var script = new Script();
			Assert.AreEqual(expected, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(expected, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的多语句脚本
		/// </summary>
		[TestMethod]
		public void TestComplexMultiStatement_2()
		{
			string s = @"
int sum = 0;

// 初始化数组
var numbers = new List<int>{1, 2, 3, 4, 5};

// 计算偶数的和
foreach(var n in numbers) {
	if (n % 2 == 0) {
		sum += n;
	}
}

// 添加统计信息
sum = sum * 2 + count;
sum
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int count = 0;
			int sum = 0;
			foreach (var n in list)
			{
				if (n % 2 == 0)
				{
					sum += n;
				}
			}
			sum = sum * 2 + count;

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("count", 0);
			Assert.AreEqual(sum, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的多语句脚本
		/// </summary>
		[TestMethod]
		public void TestComplexMultiStatement()
		{
			string s = @"
int sum = 0;

// 初始化数组
var numbers = new List<int>{1, 2, 3, 4, 5};

// 计算偶数的和
foreach(var n in numbers) {
	if (n % 2 == 0) {
		sum += n;
	}
}

// 添加统计信息
sum = sum * 2 + count;
sum
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int count = 0;
			int sum = 0;
			foreach (var n in list)
			{
				if (n % 2 == 0)
				{
					sum += n;
				}
			}
			sum = sum * 2 + count;

			var script = new Script();
			script.Context.SetVar("count", 0);
			Assert.AreEqual(sum, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的比较运算符链
		/// </summary>
		[TestMethod]
		public void TestComplexComparisonChain()
		{
			string s = @"int a = 5;int b = 10;int c = 15;bool result = a < b && b < c && c > a && a != c;result";
			var script = new Script();
			Assert.AreEqual(true, script.Eval(s));

			s = @"int a = 5;int b = 10;int c = 15;bool result = a > b || b > c || c < a;result";
			script = new Script();
			Assert.AreEqual(false, script.Eval(s));
		}

		/// <summary>
		/// 测试通过脚本调用外部函数
		/// </summary>
		[TestMethod]
		public void TestExternalFunction()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("max", Math.Max);
			script.Context.AddFunc<int, int, int>("min", Math.Min);
			script.Context.AddFunc<int, int>("abs", Math.Abs);

			string s = @"max(min(abs(-10), 20), 15)";
			Assert.AreEqual(15, script.Eval(s));

			s = @"max(min(abs(-10), 20), 15)";
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(15, script.Eval(s));
		}

		/// <summary>
		/// 测试类型转换表达式
		/// </summary>
		[TestMethod]
		public void TestComplexCast()
		{
			var script = new Script();
			script.Context.AddType(typeof(Math));

			//			string s = @"
			//double d = 10.5;
			//int i = (int)(d + 0.5);
			//i
			//";
			string s = @"
double d = 10.5;
int i = d + 0.5;
i
";
			Assert.AreEqual(11, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(11, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的混合类型运算
		/// </summary>
		[TestMethod]
		public void TestMixedTypeOperation()
		{
			var script = new Script();
			script.Context.SetVar("d", 10.5);

			string s = @"d + 5";
			Assert.AreEqual(15.5, script.Eval(s));

			s = @"d + 5";
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(15.5, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂数学运算
		/// </summary>
		[TestMethod]
		public void TestComplexMath()
		{
			var script = new Script();
			script.Context.AddType(typeof(Math));

			//			string s = @"
			//int result = (int)(Math.Pow(2, 3) + Math.Sqrt(16) + Math.Abs(-5) + Math.Max(3, 7));
			//result
			//";
			string s = @"
int result = Math.Pow(2, 3) + Math.Sqrt(16) + Math.Abs(-5) + Math.Max(3, 7);
result
";
			// 8 + 4 + 5 + 7 = 24
			Assert.AreEqual(24, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(24, script.Eval(s));
		}

		/// <summary>
		/// 测试foreach中的嵌套逻辑
		/// </summary>
		[TestMethod]
		public void TestForeachNestedLogic()
		{
			var items = new List<Dictionary<string, int>>
			{
				new Dictionary<string, int> { { "count", 10 }, { "price", 5 } },
				new Dictionary<string, int> { { "count", 20 }, { "price", 3 } },
				new Dictionary<string, int> { { "count", 15 }, { "price", 8 } }
			};

			string s = @"
int total = 0;
foreach(var item in items) {
	var count = item[""count""];
	var price = item[""price""];
	total += count * price;
}
total
";

			int expected = 10 * 5 + 20 * 3 + 15 * 8;

			var script = new Script();
			script.Context.SetVar("items", items);
			Assert.AreEqual(expected, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("items", items);
			Assert.AreEqual(expected, script.Eval(s));
		}

		/// <summary>
		/// 测试使用var声明复杂类型
		/// </summary>
		[TestMethod]
		public void TestVarComplexType()
		{
			var script = new Script();
			script.Context.AddType<Person>();

			string s = @"
var person = new Person(""Alice"", 25);
var greeting = person.SayHello();
greeting
";

			var person = new Person("Alice", 25);
			Assert.AreEqual(person.SayHello(), script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(person.SayHello(), script.Eval(s));
		}

		/// <summary>
		/// 测试for循环计算
		/// </summary>
		[TestMethod]
		public void TestComplexForLoop()
		{
			string s = @"
int sum = 0;
for (int i = 1; i <= 10; i++) {
	sum += i;
}
sum
";
			// 1+2+3+...+10 = 55
			var script = new Script();
			Assert.AreEqual(55, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(55, script.Eval(s));
		}

		/// <summary>
		/// 测试嵌套for循环
		/// </summary>
		[TestMethod]
		public void TestNestedForLoop()
		{
			string s = @"
int sum = 0;
for (int i = 1; i <= 3; i++) {
	for (int j = 1; j <= 3; j++) {
		sum += i * j;
	}
}
sum
";
			int expected = 0;
			for (int i = 1; i <= 3; i++)
			{
				for (int j = 1; j <= 3; j++)
				{
					expected += i * j;
				}
			}

			var script = new Script();
			Assert.AreEqual(expected, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(expected, script.Eval(s));
		}

		/// <summary>
		/// 测试while循环
		/// </summary>
		[TestMethod]
		public void TestComplexWhileLoop()
		{
			string s = @"
int count = 0;
int sum = 0;
while (count < 10) {
	count++;
	sum += count;
}
sum
";
			int expected = 0;
			int count = 0;
			while (count < 10)
			{
				count++;
				expected += count;
			}

			var script = new Script();
			Assert.AreEqual(expected, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(expected, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的作用域嵌套
		/// </summary>
		[TestMethod]
		public void TestComplexScope_5()
		{
			// Test using single line format similar to working Test13
			string s = "int outer() { int x = 10; { int y = 20; { int z = 30; return x + y + z; } } };outer()";
			var script = new Script();
			Assert.AreEqual(60, script.Eval<int>(s));
		}

		/// <summary>
		/// 测试复杂的作用域嵌套
		/// </summary>
		[TestMethod]
		public void TestComplexScope_4()
		{
			// Test using single line format similar to working Test13
			string s = "int outer() { int x = 10; { int y = 20; { int z = 30; return x + y + z } } };outer()";
			var script = new Script();
			Assert.AreEqual(60, script.Eval<int>(s));
		}

		/// <summary>
		/// 测试复杂的作用域嵌套
		/// </summary>
		[TestMethod]
		public void TestComplexScope_3()
		{
			string s = @"
int outer() {
	int x = 10;
		int y = 20;
			int z = 30;
			return x + y + z;
}
outer()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(60, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的作用域嵌套
		/// </summary>
		[TestMethod]
		public void TestComplexScope_2()
		{
			string s = @"
int outer() {
	int x = 10;
		int y = 20;
			int z = 30;
			return x + y + z;
}
outer()
";
			var script = new Script();
			Assert.AreEqual(60, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的作用域嵌套
		/// </summary>
		[TestMethod]
		public void TestComplexScope_1()
		{
			string s = @"
int outer() {
	int x = 10;
	{
		int y = 20;
		{
			int z = 30;
			return x + y + z;
		}
	}
}
outer()
";
			var script = new Script();
			Assert.AreEqual(60, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂的作用域嵌套
		/// </summary>
		[TestMethod]
		public void TestComplexScope()
		{
			string s = @"
int outer() {
	int x = 10;
	{
		int y = 20;
		{
			int z = 30;
			return x + y + z;
		}
	}
}
outer()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(60, script.Eval(s));
		}

		/// <summary>
		/// 测试函数嵌套调用
		/// </summary>
		[TestMethod]
		public void TestNestedFunction()
		{
			string s = @"
int add(int a, int b) { return a + b; }
int square(int a) { return a * a; }
int triple(int a) { return a * 3; }
int result = triple(add(square(2), square(3)));
result
";
			// square(2) = 4, square(3) = 9, add(4, 9) = 13, triple(13) = 39
			var script = new Script();
			Assert.AreEqual(39, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(39, script.Eval(s));
		}

		/// <summary>
		/// 测试递归函数
		/// </summary>
		[TestMethod]
		public void TestRecursiveFunction()
		{
			// 递归计算阶乘
			string s = @"
int factorial(int n) {
	if (n <= 1) {
		return 1;
	}
	return n * factorial(n - 1);
}
factorial(5)
";
			// 5! = 120
			var script = new Script();
			Assert.AreEqual(120, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(120, script.Eval(s));

			// 递归计算斐波那契数列
			s = @"
int fibonacci(int n) {
	if (n <= 1) {
		return n;
	}
	return fibonacci(n - 1) + fibonacci(n - 2);
}
fibonacci(10)
";
			// fibonacci(10) = 55
			script = new Script();
			Assert.AreEqual(55, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(55, script.Eval(s));
		}

		/// <summary>
		/// 测试互相调用函数
		/// </summary>
		[TestMethod]
		public void TestMutualFunctionCall()
		{
			string s = @"
int a(int n) {
	if (n <= 1) { return 1; }
	return b(n - 1) + 1;
}
int b(int n) {
	if (n <= 1) { return 1; }
	return a(n - 1) + 2;
}
int result = a(5) + b(5);
result
";
			// a(5) -> b(4) -> a(3) -> b(2) -> a(1)+1 +2+1 = ...
			// a(1)=1, b(1)=1
			// a(2) = b(1)+1 = 1+1 = 2
			// b(2) = a(1)+2 = 1+2 = 3
			// a(3) = b(2)+1 = 3+1 = 4
			// b(3) = a(2)+2 = 2+2 = 4
			// a(4) = b(3)+1 = 4+1 = 5
			// b(4) = a(3)+2 = 4+2 = 6
			// a(5) = b(4)+1 = 6+1 = 7
			// b(5) = a(4)+2 = 5+2 = 7
			// result = 7 + 7 = 14
			var script = new Script();
			Assert.AreEqual(14, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(14, script.Eval(s));
		}

		/// <summary>
		/// 测试引用外部变量
		/// </summary>
		[TestMethod]
		public void TestExternalVariableReference()
		{
			string s = @"
int outerVar = 100;
int addOuter(int a) {
	return a + outerVar;
}
int multiplyOuter(int a) {
	return a * outerVar;
}
int result = addOuter(multiplyOuter(2));
result
";
			// multiplyOuter(2) = 2 * 100 = 200
			// addOuter(200) = 200 + 100 = 300
			var script = new Script();
			Assert.AreEqual(300, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(300, script.Eval(s));
		}

		/// <summary>
		/// 测试嵌套语句块中的变量生命周期
		/// </summary>
		[TestMethod]
		public void TestNestedStatementBlocks()
		{
			string s = @"
int outer() {
	int x = 10;
	{
		int y = x * 2;
		{
			int z = y + x;
			{
				int w = z * 2;
				return w;
			}
		}
	}
}
outer()
";
			// x=10, y=20, z=30, w=60
			var script = new Script();
			Assert.AreEqual(60, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(60, script.Eval(s));
		}

		/// <summary>
		/// 测试嵌套循环中的break和continue
		/// </summary>
		[TestMethod]
		public void TestNestedLoopsWithBreakContinue()
		{
			string s = @"
int sum = 0;
for (int i = 1; i <= 5; i++) {
	if (i == 3) {
		continue;
	}
	for (int j = 1; j <= 5; j++) {
		if (j == 2) {
			continue;
		}
		if (i + j >= 7) {
			break;
		}
		sum += i * j;
	}
}
sum
";
			int expected = 0;
			for (int i = 1; i <= 5; i++)
			{
				if (i == 3) continue;
				for (int j = 1; j <= 5; j++)
				{
					if (j == 2) continue;
					if (i + j >= 7) break;
					expected += i * j;
				}
			}

			var script = new Script();
			Assert.AreEqual(expected, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(expected, script.Eval(s));
		}

		/// <summary>
		/// 测试循环中的break语句
		/// </summary>
		[TestMethod]
		public void TestBreakStatement()
		{
			string s = @"
int sum = 0;
for (int i = 0; i < 10; i++) {
	if (i >= 5) {
		break;
	}
	sum += i;
}
sum
";
			// 0+1+2+3+4 = 10
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));

			// 测试while循环中的break
			s = @"
int count = 0;
int sum = 0;
while (count < 10) {
	count++;
	if (count > 7) {
		break;
	}
	sum += count;
}
sum
";
			// 1+2+3+4+5+6+7 = 28
			script = new Script();
			Assert.AreEqual(28, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(28, script.Eval(s));
		}

		/// <summary>
		/// 测试循环中的continue语句
		/// </summary>
		[TestMethod]
		public void TestContinueStatement()
		{
			string s = @"
int sum = 0;
for (int i = 0; i < 10; i++) {
	if (i % 2 == 0) {
		continue;
	}
	sum += i;
}
sum
";
			// 1+3+5+7+9 = 25
			var script = new Script();
			Assert.AreEqual(25, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(25, script.Eval(s));

			// 测试while循环中的continue
			s = @"
int count = 0;
int sum = 0;
while (count < 10) {
	count++;
	if (count % 3 == 0) {
		continue;
	}
	sum += count;
}
sum
";
			// 1+2+4+5+7+8+10 = 37
			script = new Script();
			Assert.AreEqual(37, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(37, script.Eval(s));
		}

		/// <summary>
		/// 测试函数中的return语句
		/// </summary>
		[TestMethod]
		public void TestReturnStatement()
		{
			string s = @"
int findFirstEven(int[] arr) {
	foreach(var item in arr) {
		if (item % 2 == 0) {
			return item;
		}
	}
	return -1;
}
int result = findFirstEven(new int[]{1, 3, 5, 6, 8});
result
";
			var script = new Script();
			Assert.AreEqual(6, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(6, script.Eval(s));

			// 测试没有找到目标的情况
			s = @"
int findFirstEven(int[] arr) {
	foreach(var item in arr) {
		if (item % 2 == 0) {
			return item;
		}
	}
	return -1;
}
int result = findFirstEven(new int[]{1, 3, 5, 7, 9});
result
";
			script = new Script();
			Assert.AreEqual(-1, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(-1, script.Eval(s));
		}

		/// <summary>
		/// 测试变量的生命周期和作用域
		/// </summary>
		[TestMethod]
		public void TestVariableLifetime_2()
		{
			// 测试内层作用域变量屏蔽外层
			var s = @"
int outer() {
	int a = 5;
	{
		int a = 10;
		return a;
	}
}
outer()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		/// <summary>
		/// 测试变量的生命周期和作用域
		/// </summary>
		[TestMethod]
		public void TestVariableLifetime()
		{
			string s = @"
int outer() {
	int a = 1;
	{
		int b = a + 10;
		{
			int c = b + 100;
			{
				int d = c + 1000;
				return d;
			}
		}
	}
}
outer()
";
			// a=1, b=11, c=111, d=1111
			var script = new Script();
			Assert.AreEqual(1111, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(1111, script.Eval(s));
		}

		/// <summary>
		/// 测试嵌套if语句和复合条件
		/// </summary>
		[TestMethod]
		public void TestNestedIfStatements()
		{
			string s = @"
int classify(int age) {
	if (age < 0) {
		return -1;
	}
	if (age < 18) {
		if (age < 13) {
			if (age < 3) {
				return 0;
			}
			return 1;
		}
		return 2;
	}
	if (age < 65) {
		return 3;
	}
	return 4;
}
int result = classify(10) + classify(50) + classify(70);
result
";
			// 10: age<18 && age <13 age>3 -> 1
			// 50: age>=18 && age<65 -> 3
			// 70: age>=65 -> 4
			// result = 1 + 3 + 4 = 8
			var script = new Script();
			Assert.AreEqual(8, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(8, script.Eval(s));
		}

		/// <summary>
		/// 测试复杂嵌套循环计算
		/// </summary>
		[TestMethod]
		public void TestComplexNestedLoops()
		{
			string s = @"
int sum = 0;
for (int i = 1; i <= 3; i++) {
	for (int j = 1; j <= 3; j++) {
		for (int k = 1; k <= 3; k++) {
			sum += i * j * k;
		}
	}
}
sum
";
			int expected = 0;
			for (int i = 1; i <= 3; i++)
			{
				for (int j = 1; j <= 3; j++)
				{
					for (int k = 1; k <= 3; k++)
					{
						expected += i * j * k;
					}
				}
			}
			// (1+2+3)^3 = 6^3 = 216
			var script = new Script();
			Assert.AreEqual(expected, script.Eval(s));
			Assert.AreEqual(216, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(expected, script.Eval(s));
			Assert.AreEqual(216, script.Eval(s));
		}

		/// <summary>
		/// 测试多层递归嵌套
		/// </summary>
		[TestMethod]
		public void TestDeepRecursiveFunction()
		{
			string s = @"
int power(int base, int exp) {
	if (exp <= 0) {
		return 1;
	}
	return base * power(base, exp - 1);
}
int result = power(2, 10);
result
";
			// 2^10 = 1024
			var script = new Script();
			Assert.AreEqual(1024, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(1024, script.Eval(s));
		}

		/// <summary>
		/// 测试函数参数与外部变量同名时的作用域
		/// </summary>
		[TestMethod]
		public void TestParameterHidesExternalVariable()
		{
			string s = @"
int x = 100;
int add(int x, int y) {
	return x + y;
}
int result = add(5, 10);
result
";
			// 参数x=5屏蔽了外部变量x=100
			// add(5, 10) = 5 + 10 = 15
			var script = new Script();
			Assert.AreEqual(15, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(15, script.Eval(s));
		}

		/// <summary>
		/// 测试循环变量与外部变量重名
		/// </summary>
		[TestMethod]
		public void TestLoopVariableHidesExternalVariable_2()
		{
			string s = @"
int sum = 0;
int i = 100;
for (int i = 1; i <= 5; i++) {
	sum += i;
}
int result = sum + i;
result
";
			// 循环结束后sum=15, i=6(最后一次i++变成6后退出循环)
			// 但外部变量i保持为100
			// result = 15 + 100 = 115
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(115, script.Eval(s));
		}

		/// <summary>
		/// 测试循环变量与外部变量重名
		/// </summary>
		[TestMethod]
		public void TestLoopVariableHidesExternalVariable()
		{
			string s = @"
int sum = 0;
int i = 100;
for (int i = 1; i <= 5; i++) {
	sum += i;
}
int result = sum + i;
result
";
			// 循环结束后sum=15, i=6(最后一次i++变成6后退出循环)
			// 但外部变量i保持为100
			// result = 15 + 100 = 115
			var script = new Script();
			Assert.AreEqual(115, script.Eval(s));
		}

		/// <summary>
		/// 测试foreach循环变量与外部变量重名
		/// </summary>
		[TestMethod]
		public void TestForeachVariableHidesExternalVariable()
		{
			string s = @"
int sum = 0;
int item = 1000;
var list = new List<int>{1, 2, 3, 4, 5};
foreach(int item in list) {
	sum += item;
}
int result = sum + item;
result
";
			// foreach后sum=15, item应该保持1000
			// result = 15 + 1000 = 1015
			var script = new Script();
			Assert.AreEqual(1015, script.Eval(s));

			script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(1015, script.Eval(s));
		}
	}
}
