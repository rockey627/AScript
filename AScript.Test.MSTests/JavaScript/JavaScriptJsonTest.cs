using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptJsonTest
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

		// json parse
		[TestMethod]
		public void Test01_parse()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("JSON.parse('{\"a\":1}')");
			Assert.AreEqual(1L, result.a);
		}

		[TestMethod]
		public void Test01_parse_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("JSON.parse('{\"a\":1}')");
			Assert.AreEqual(1L, result.a);
		}

		[TestMethod]
		public void Test01_parse2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = JSON.parse('{\"a\":1}'); d.a");
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test01_parse2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d = JSON.parse('{\"a\":1}'); d.a");
			Assert.AreEqual(1L, result);
		}

		// json parse array
		[TestMethod]
		public void Test02_parseArray()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<List<object>>("JSON.parse('[1, 2, 3]')");
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(1L, result[0]);
			Assert.AreEqual(2L, result[1]);
			Assert.AreEqual(3L, result[2]);
		}

		[TestMethod]
		public void Test02_parseArray_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<List<object>>("JSON.parse('[1, 2, 3]')");
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(1L, result[0]);
			Assert.AreEqual(2L, result[1]);
			Assert.AreEqual(3L, result[2]);
		}

		[TestMethod]
		public void Test02_parseArray2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d=JSON.parse('[1, 2, 3]'); d[0]+d[1]+d[2]");
			Assert.AreEqual(6L, result);
		}

		[TestMethod]
		public void Test02_parseArray2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var d=JSON.parse('[1, 2, 3]'); d[0]+d[1]+d[2]");
			Assert.AreEqual(6L, result);
		}

		// json parse nested
		[TestMethod]
		public void Test03_parseNested()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("JSON.parse('{\"arr\":[1,2],\"obj\":{\"x\":1}}')");
			Assert.AreEqual(2, result.arr.Count);
			Assert.AreEqual(1L, result.arr[0]);
			Assert.AreEqual(2L, result.arr[1]);
			Assert.AreEqual(1L, result.obj.x);
		}

		[TestMethod]
		public void Test03_parseNested_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("JSON.parse('{\"arr\":[1,2],\"obj\":{\"x\":1}}')");
			Assert.AreEqual(2, result.arr.Count);
			Assert.AreEqual(1L, result.arr[0]);
			Assert.AreEqual(2L, result.arr[1]);
			Assert.AreEqual(1L, result.obj.x);
		}

		[TestMethod]
		public void Test03_parseNested2()
		{
			string s = @"
var d = JSON.parse('{""arr"":[1,2],""obj"":{""x"":3}}');
d.arr[0]+d.arr[1]+d.obj.x
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(6L, script.Eval(s));
		}

		[TestMethod]
		public void Test03_parseNested2_CompileAll()
		{
			string s = @"
var d = JSON.parse('{""arr"":[1,2],""obj"":{""x"":3}}');
d.arr[0]+d.arr[1]+d.obj.x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(6L, script.Eval(s));
		}

		// json stringify
		[TestMethod]
		public void Test04_stringify()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("{\"a\":1}", script.Eval("JSON.stringify({a: 1})"));
		}

		[TestMethod]
		public void Test04_stringify_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("{\"a\":1}", script.Eval("JSON.stringify({a: 1})"));
		}

		// json stringify array
		[TestMethod]
		public void Test05_stringifyArray()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("[1,2,3]", script.Eval("JSON.stringify([1, 2, 3])"));
		}

		[TestMethod]
		public void Test05_stringifyArray_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("[1,2,3]", script.Eval("JSON.stringify([1, 2, 3])"));
		}

		// json property access
		[TestMethod]
		public void Test06_propertyAccess()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var json = script.Eval("var json = JSON.parse('{\"name\":\"test\",\"value\":123}')");
			Assert.AreEqual("test", script.Eval("json.name"));
			Assert.AreEqual(123L, script.Eval("json.value"));
		}

		[TestMethod]
		public void Test06_propertyAccess_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var json = script.Eval("var json = JSON.parse('{\"name\":\"test\",\"value\":123}')");
			Assert.AreEqual("test", script.Eval("json.name"));
			Assert.AreEqual(123L, script.Eval("json.value"));
		}

		// json parse with spaces
		[TestMethod]
		public void Test08_parseWithSpaces()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("JSON.parse('{ \"a\" : 1 }')");
			Assert.AreEqual(1L, result.a);
		}

		[TestMethod]
		public void Test08_parseWithSpaces_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("JSON.parse('{ \"a\" : 1 }')");
			Assert.AreEqual(1L, result.a);
		}

		[TestMethod]
		public void Test09_propertyAccess()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var json = JSON.parse('{\"a\":1,\"b\":123}'); json.a+json.b");
			Assert.AreEqual(124L, result);
		}

		[TestMethod]
		public void Test09_propertyAccess_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var json = JSON.parse('{\"a\":1,\"b\":123}'); json.a+json.b");
			Assert.AreEqual(124L, result);
		}

	}
}
