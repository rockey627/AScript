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
	public class JavaScriptPromiseTest
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

		// promise creation with resolve
		[TestMethod]
		public void Test01_promiseResolve()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve(42)).then(x => x)");
			Assert.AreEqual(42L, result);
		}

		[TestMethod]
		public void Test01_promiseResolve_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve(42)).then(x => x)");
			Assert.AreEqual(42L, result);
		}

		[TestMethod]
		public void Test01_promiseResolve2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise(function (resolve, reject) { resolve(42) }).then(x => x)");
			Assert.AreEqual(42L, result);
		}

		[TestMethod]
		public void Test01_promiseResolve2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise(function (resolve, reject) { resolve(42) }).then(x => x)");
			Assert.AreEqual(42L, result);
		}

		// promise creation with reject
		[TestMethod]
		public void Test02_promiseReject()
		{
			var s = @"
var errMsg;
await new Promise((resolve, reject) => reject('error')).catch(e => { errMsg = e; return null; });
errMsg;
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual("error", result);
		}

		[TestMethod]
		public void Test02_promiseReject_CompileAll()
		{
			var s = @"
var errMsg;
await new Promise((resolve, reject) => reject('error')).catch(e => { errMsg = e; return null; });
errMsg;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual("error", result);
		}

		// promise then with transformation
		[TestMethod]
		public void Test03_promiseThenTransform()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve(10)).then(x => x * 2)");
			Assert.AreEqual(20L, result);
		}

		[TestMethod]
		public void Test03_promiseThenTransform_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve(10)).then(x => x * 2)");
			Assert.AreEqual(20L, result);
		}

		// promise chain
		[TestMethod]
		public void Test04_promiseChain()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve(1)).then(x => x + 1).then(x => x + 2)");
			Assert.AreEqual(4L, result);
		}

		[TestMethod]
		public void Test04_promiseChain_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve(1)).then(x => x + 1).then(x => x + 2)");
			Assert.AreEqual(4L, result);
		}

		// promise catch after then
		[TestMethod]
		public void Test05_promiseCatchAfterThen()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var errMsg;
await new Promise((resolve, reject) => {
    resolve(10);
}).then(x => {
    throw 'error in then';
}).catch(e => { errMsg = e; return null; });
errMsg
";
			var result = script.Eval(code);
			Assert.AreEqual("error in then", result);
		}

		[TestMethod]
		public void Test05_promiseCatchAfterThen_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var errMsg;
await new Promise((resolve, reject) => {
    resolve(10);
}).then(x => {
    throw 'error in then';
}).catch(e => { errMsg = e; return null; });
errMsg
";
			var result = script.Eval(code);
			Assert.AreEqual("error in then", result);
		}

		// promise resolve with object
		[TestMethod]
		public void Test06_promiseResolveObject()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve({value: 42})).then(obj => obj.value)");
			Assert.AreEqual(42L, result);
		}

		[TestMethod]
		public void Test06_promiseResolveObject_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve({value: 42})).then(obj => obj.value)");
			Assert.AreEqual(42L, result);
		}

		// promise resolve with array
		[TestMethod]
		public void Test07_promiseResolveArray()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve([1, 2, 3])).then(arr => arr.length)");
			Assert.AreEqual(3L, result);
		}

		[TestMethod]
		public void Test07_promiseResolveArray_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve([1, 2, 3])).then(arr => arr.length)");
			Assert.AreEqual(3L, result);
		}

		// promise with null value
		[TestMethod]
		public void Test08_promiseResolveNull()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve(null)).then(x => x)");
			Assert.AreEqual(null, result);
		}

		[TestMethod]
		public void Test08_promiseResolveNull_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve(null)).then(x => x)");
			Assert.AreEqual(null, result);
		}

		// promise with string value
		[TestMethod]
		public void Test09_promiseResolveString()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve('hello')).then(x => x + ' world')");
			Assert.AreEqual("hello world", result);
		}

		[TestMethod]
		public void Test09_promiseResolveString_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("await new Promise((resolve, reject) => resolve('hello')).then(x => x + ' world')");
			Assert.AreEqual("hello world", result);
		}

		// promise then with both success and failure handlers
		[TestMethod]
		public void Test10_promiseThenWithBothHandlers()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
await new Promise((resolve, reject) => resolve(10))
    .then(x => x * 2, e => 'error')
";
			var result = script.Eval(code);
			Assert.AreEqual(20L, result);
		}

		[TestMethod]
		public void Test10_promiseThenWithBothHandlers_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
await new Promise((resolve, reject) => resolve(10))
    .then(x => x * 2, e => 'error')
";
			var result = script.Eval(code);
			Assert.AreEqual(20L, result);
		}

		// Promise.all with multiple promises
		[TestMethod]
		public void Test11_promiseAll()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<IList<object>>("await Promise.all([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)])");
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(1L, result[0]);
			Assert.AreEqual(2L, result[1]);
			Assert.AreEqual(3L, result[2]);
		}

		[TestMethod]
		public void Test11_promiseAll_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<IList<object>>("await Promise.all([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)])");
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(1L, result[0]);
			Assert.AreEqual(2L, result[1]);
			Assert.AreEqual(3L, result[2]);
		}

		[TestMethod]
		public void Test12_promiseAll()
		{
			var s = @"
var arr = await Promise.all([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)]);
arr[0] + arr[1] + arr[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(6L, result);
		}

		[TestMethod]
		public void Test12_promiseAll_CompileAll()
		{
			var s = @"
var arr = await Promise.all([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)]);
arr[0] + arr[1] + arr[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(6L, result);
		}

		// Promise.any - returns the first resolved promise value
		[TestMethod]
		public void Test13_promiseAny()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<long>("await Promise.any([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)])");
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test13_promiseAny_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<long>("await Promise.any([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)])");
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test14_promiseAny()
		{
			var s = @"
var result = await Promise.any([Promise.resolve(10), Promise.resolve(20)]);
result
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(10L, result);
		}

		[TestMethod]
		public void Test14_promiseAny_CompileAll()
		{
			var s = @"
var result = await Promise.any([Promise.resolve(10), Promise.resolve(20)]);
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(10L, result);
		}
	}
}
