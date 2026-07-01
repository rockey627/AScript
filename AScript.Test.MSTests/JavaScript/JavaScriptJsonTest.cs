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
			var result = script.Eval("JSON.parse('{\"a\":1}')");
			var json = (JObject)result;
			Assert.AreEqual(1L, json["a"].Value<long>());
		}

		[TestMethod]
		public void Test01_parse_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("JSON.parse('{\"a\":1}')");
			var json = (JObject)result;
			Assert.AreEqual(1L, json["a"].Value<long>());
		}

		// json parse array
		[TestMethod]
		public void Test02_parseArray()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("JSON.parse('[1, 2, 3]')");
			var arr = (JArray)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0].Value<long>());
			Assert.AreEqual(2L, arr[1].Value<long>());
			Assert.AreEqual(3L, arr[2].Value<long>());
		}

		[TestMethod]
		public void Test02_parseArray_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("JSON.parse('[1, 2, 3]')");
			var arr = (JArray)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0].Value<long>());
			Assert.AreEqual(2L, arr[1].Value<long>());
			Assert.AreEqual(3L, arr[2].Value<long>());
		}

		// json parse nested
		[TestMethod]
		public void Test03_parseNested()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("JSON.parse('{\"arr\":[1,2],\"obj\":{\"x\":1}}')");
			var json = (JObject)result;
			var arr = (JArray)json["arr"];
			var obj = (JObject)json["obj"];
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(1L, obj["x"].Value<long>());
		}

		[TestMethod]
		public void Test03_parseNested_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("JSON.parse('{\"arr\":[1,2],\"obj\":{\"x\":1}}')");
			var json = (JObject)result;
			var arr = (JArray)json["arr"];
			var obj = (JObject)json["obj"];
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(1L, obj["x"].Value<long>());
		}

		[TestMethod]
		public void Test03_parseNested2()
		{
			string s = @"
var d = JSON.parse('{""arr"":[1,2],""obj"":{""x"":3}}');
d.arr[0]+d.arr[1]+d.x
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
d.arr[0]+d.arr[1]+d.x
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
			var result = script.Eval("JSON.parse('{ \"a\" : 1 }')");
			var json = (JObject)result;
			Assert.AreEqual(1L, json["a"].Value<long>());
		}

		[TestMethod]
		public void Test08_parseWithSpaces_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("JSON.parse('{ \"a\" : 1 }')");
			var json = (JObject)result;
			Assert.AreEqual(1L, json["a"].Value<long>());
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
