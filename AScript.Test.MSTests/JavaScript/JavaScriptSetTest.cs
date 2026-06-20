using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptSetTest
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

		// set creation
		[TestMethod]
		public void Test01_setCreation()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Set()");
			var set = (HashSet<object>)result;
			Assert.AreEqual(0, set.Count);
		}

		[TestMethod]
		public void Test01_setCreation_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Set()");
			var set = (HashSet<object>)result;
			Assert.AreEqual(0, set.Count);
		}

		// set creation with iterable
		[TestMethod]
		public void Test02_setCreationWithIterable()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Set([1, 2, 3])");
			var set = (HashSet<object>)result;
			Assert.AreEqual(3, set.Count);
		}

		[TestMethod]
		public void Test02_setCreationWithIterable_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Set([1, 2, 3])");
			var set = (HashSet<object>)result;
			Assert.AreEqual(3, set.Count);
		}

		// set add
		[TestMethod]
		public void Test03_add()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var s = new Set(); s.add(1); s.add(2); s.add(3); s.size");
			Assert.AreEqual(3L, result);
		}

		[TestMethod]
		public void Test03_add_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var s = new Set(); s.add(1); s.add(2); s.add(3); s.size");
			Assert.AreEqual(3L, result);
		}

		// set add duplicate (should not increase size)
		[TestMethod]
		public void Test04_addDuplicate()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var s = new Set(); s.add(1); s.add(1); s.size");
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test04_addDuplicate_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var s = new Set(); s.add(1); s.add(1); s.size");
			Assert.AreEqual(1L, result);
		}

		// set has
		[TestMethod]
		public void Test05_has()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var s = new Set([1, 2, 3]); s.has(1)"));
			Assert.AreEqual(true, script.Eval("var s = new Set([1, 2, 3]); s.has(2)"));
			Assert.AreEqual(true, script.Eval("var s = new Set([1, 2, 3]); s.has(3)"));
			Assert.AreEqual(false, script.Eval("var s = new Set([1, 2, 3]); s.has(4)"));
		}

		[TestMethod]
		public void Test05_has_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var s = new Set([1, 2, 3]); s.has(1)"));
			Assert.AreEqual(true, script.Eval("var s = new Set([1, 2, 3]); s.has(2)"));
			Assert.AreEqual(true, script.Eval("var s = new Set([1, 2, 3]); s.has(3)"));
			Assert.AreEqual(false, script.Eval("var s = new Set([1, 2, 3]); s.has(4)"));
		}

		// set delete
		[TestMethod]
		public void Test06_delete()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var s = new Set([1, 2, 3]); s.delete(2)"));
			Assert.AreEqual(false, script.Eval("var s = new Set([1, 2, 3]); s.delete(4)"));
			Assert.AreEqual(2L, script.Eval("var s = new Set([1, 2, 3]); s.delete(2); s.size"));
		}

		[TestMethod]
		public void Test06_delete_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var s = new Set([1, 2, 3]); s.delete(2)"));
			Assert.AreEqual(false, script.Eval("var s = new Set([1, 2, 3]); s.delete(4)"));
			Assert.AreEqual(2L, script.Eval("var s = new Set([1, 2, 3]); s.delete(2); s.size"));
		}

		// set delete non-existent
		[TestMethod]
		public void Test07_deleteNonExistent()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(false, script.Eval("var s = new Set([1, 2, 3]); s.delete(5)"));
			Assert.AreEqual(3L, script.Eval("var s = new Set([1, 2, 3]); s.delete(5); s.size"));
		}

		[TestMethod]
		public void Test07_deleteNonExistent_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(false, script.Eval("var s = new Set([1, 2, 3]); s.delete(5)"));
			Assert.AreEqual(3L, script.Eval("var s = new Set([1, 2, 3]); s.delete(5); s.size"));
		}

		// set clear
		[TestMethod]
		public void Test08_clear()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("var s = new Set([1, 2, 3]); s.clear(); s.size"));
		}

		[TestMethod]
		public void Test08_clear_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("var s = new Set([1, 2, 3]); s.clear(); s.size"));
		}

		// set forEach
		[TestMethod]
		public void Test09_foreach()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set([1, 2, 3]);
var sum = 0;
s.forEach(x => { sum += x; });
sum
";
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test09_foreach_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set([1, 2, 3]);
var sum = 0;
s.forEach(x => { sum += x; });
sum
";
			Assert.AreEqual(6L, script.Eval(code));
		}

		// set forEach with string concatenation
		[TestMethod]
		public void Test10_foreachStringConcat()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set(['a', 'b', 'c']);
var str = '';
s.forEach(x => { str += x; });
str
";
			Assert.AreEqual("abc", script.Eval(code));
		}

		[TestMethod]
		public void Test10_foreachStringConcat_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set(['a', 'b', 'c']);
var str = '';
s.forEach(x => { str += x; });
str
";
			Assert.AreEqual("abc", script.Eval(code));
		}

		// set size
		[TestMethod]
		public void Test11_size()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Set().size"));
			Assert.AreEqual(3L, script.Eval("new Set([1, 2, 3]).size"));
			Assert.AreEqual(5L, script.Eval("new Set([1, 2, 3, 4, 5]).size"));
		}

		[TestMethod]
		public void Test11_size_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Set().size"));
			Assert.AreEqual(3L, script.Eval("new Set([1, 2, 3]).size"));
			Assert.AreEqual(5L, script.Eval("new Set([1, 2, 3, 4, 5]).size"));
		}

		// set with mixed types
		[TestMethod]
		public void Test12_mixedTypes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set();
s.add(1);
s.add('hello');
s.add(true);
s.size
";
			Assert.AreEqual(3L, script.Eval(code));
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(1); s.add('hello'); s.add(true); s.has(1)"));
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(1); s.add('hello'); s.add(true); s.has('hello')"));
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(1); s.add('hello'); s.add(true); s.has(true)"));
			Assert.AreEqual(false, script.Eval("var s = new Set(); s.add(1); s.add('hello'); s.add(true); s.has(null)"));
		}

		[TestMethod]
		public void Test12_mixedTypes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set();
s.add(1);
s.add('hello');
s.add(true);
s.size
";
			Assert.AreEqual(3L, script.Eval(code));
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(1); s.add('hello'); s.add(true); s.has(1)"));
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(1); s.add('hello'); s.add(true); s.has('hello')"));
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(1); s.add('hello'); s.add(true); s.has(true)"));
			Assert.AreEqual(false, script.Eval("var s = new Set(); s.add(1); s.add('hello'); s.add(true); s.has(null)"));
		}

		// set for-of loop
		[TestMethod]
		public void Test13_forOf()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set([1, 2, 3]);
var sum = 0;
for (var item of s) {
	sum += item;
}
sum
";
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test13_forOf_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set([1, 2, 3]);
var sum = 0;
for (var item of s) {
	sum += item;
}
sum
";
			Assert.AreEqual(6L, script.Eval(code));
		}

		// set operations chain
		[TestMethod]
		public void Test14_operationsChain()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set();
s.add(1);
s.add(2);
s.add(3);
s.delete(2);
s.has(2)
";
			Assert.AreEqual(false, script.Eval(code));
		}

		[TestMethod]
		public void Test14_operationsChain_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var s = new Set();
s.add(1);
s.add(2);
s.add(3);
s.delete(2);
s.has(2)
";
			Assert.AreEqual(false, script.Eval(code));
		}

		// set with null and undefined
		[TestMethod]
		public void Test15_nullUndefined()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(null); s.has(null)"));
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(undefined); s.has(undefined)"));
		}

		[TestMethod]
		public void Test15_nullUndefined_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(null); s.has(null)"));
			Assert.AreEqual(true, script.Eval("var s = new Set(); s.add(undefined); s.has(undefined)"));
		}
	}
}
