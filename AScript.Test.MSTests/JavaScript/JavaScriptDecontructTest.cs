using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptDecontructTest
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
		public void TestDestructuring_Array_Basic2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var [a, b] = [1, 2];
a + b;
";
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Array_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var [a, b] = [1, 2];
a + b;
";
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Array_Multi2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var [x, y, z] = [10, 20, 30];
x + y + z;
";
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Array_Multi()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var [x, y, z] = [10, 20, 30];
x + y + z;
";
			Assert.AreEqual(60L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Array_Partial2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var [first, , third] = [1, 2, 3];
first + third;
";
			Assert.AreEqual(4L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Array_Partial()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var [first, , third] = [1, 2, 3];
first + third;
";
			Assert.AreEqual(4L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Array_DefaultValue2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var [a = 5, b = 10] = [3];
a + b;
";
			Assert.AreEqual(13L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Array_DefaultValue()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var [a = 5, b = 10] = [3];
a + b;
";
			Assert.AreEqual(13L, script.Eval(code));
		}

		//		[TestMethod]
		//		public void TestDestructuring_Array_Swap()
		//		{
		//			var script = new Script();
		//			script.Context.Langs = new[] { "js" };

		//			string code = @"
		//var a = 1;
		//var b = 2;
		//[a, b] = [b, a];
		//a + b;
		//";
		//			Assert.AreEqual(3L, script.Eval(code));
		//		}

		[TestMethod]
		public void TestDestructuring_Array_Nested2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var [[inner], outer] = [[10], 20];
inner + outer;
";
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Array_Nested()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var [[inner], outer] = [[10], 20];
inner + outer;
";
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_Basic_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var {name, age} = {name: 'Alice', age: 25};
name;
";
			Assert.AreEqual("Alice", script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_Basic()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var {name, age} = {name: 'Alice', age: 25};
name;
";
			Assert.AreEqual("Alice", script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_Another_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var {name, age} = {name: 'Alice', age: 25};
age;
";
			Assert.AreEqual(25L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_Another()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var {name, age} = {name: 'Alice', age: 25};
age;
";
			Assert.AreEqual(25L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_DefaultValue2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var {a, b = 100} = {a: 10};
a + b;
";
			Assert.AreEqual(110L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_DefaultValue()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var {a, b = 100} = {a: 10};
a + b;
";
			Assert.AreEqual(110L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_Nested2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var {inner: {value}} = {inner: {value: 42}};
value;
";
			Assert.AreEqual(42L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_Nested()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var {inner: {value}} = {inner: {value: 42}};
value;
";
			Assert.AreEqual(42L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_Alias2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var {name: aliasName} = {name: 'Bob'};
aliasName;
";
			Assert.AreEqual("Bob", script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Object_Alias()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var {name: aliasName} = {name: 'Bob'};
aliasName;
";
			Assert.AreEqual("Bob", script.Eval(code));
		}

		//		[TestMethod]
		//		public void TestDestructuring_FunctionParam_Object()
		//		{
		//			var script = new Script();
		//			script.Context.Langs = new[] { "js" };

		//			string code = @"
		//function greet({name, age}) {
		//	return name + ' is ' + age;
		//}
		//greet({name: 'Tom', age: 20});
		//";
		//			Assert.AreEqual("Tom is 20", script.Eval(code));
		//		}

		//		[TestMethod]
		//		public void TestDestructuring_FunctionParam_Array()
		//		{
		//			var script = new Script();
		//			script.Context.Langs = new[] { "js" };

		//			string code = @"
		//function sum([a, b, c]) {
		//	return a + b + c;
		//}
		//sum([10, 20, 30]);
		//";
		//			Assert.AreEqual(60L, script.Eval(code));
		//		}

		[TestMethod]
		public void TestDestructuring_Combined2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			string code = @"
var [first, {name}] = [1, {name: 'John'}];
first + name.length;
";
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void TestDestructuring_Combined()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			string code = @"
var [first, {name}] = [1, {name: 'John'}];
first + name.length;
";
			Assert.AreEqual(5L, script.Eval(code));
		}
	}
}
