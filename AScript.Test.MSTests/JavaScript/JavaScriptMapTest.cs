using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptMapTest
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

		// map creation
		[TestMethod]
		public void Test01_mapCreation()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Map()");
			var map = (Dictionary<object, object>)result;
			Assert.AreEqual(0, map.Count);
		}

		[TestMethod]
		public void Test01_mapCreation_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Map()");
			var map = (Dictionary<object, object>)result;
			Assert.AreEqual(0, map.Count);
		}

		// map creation with iterable
		[TestMethod]
		public void Test02_mapCreationWithIterable()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Map([['a', 1], ['b', 2], ['c', 3]])");
			var map = (Dictionary<object, object>)result;
			Assert.AreEqual(3, map.Count);
			Assert.AreEqual(1L, map["a"]);
			Assert.AreEqual(2L, map["b"]);
			Assert.AreEqual(3L, map["c"]);
		}

		[TestMethod]
		public void Test02_mapCreationWithIterable_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("new Map([['a', 1], ['b', 2], ['c', 3]])");
			var map = (Dictionary<object, object>)result;
			Assert.AreEqual(3, map.Count);
			Assert.AreEqual(1L, map["a"]);
			Assert.AreEqual(2L, map["b"]);
			Assert.AreEqual(3L, map["c"]);
		}

		// map set
		[TestMethod]
		public void Test03_set()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map();
m.set('a', 1);
m.set('b', 2);
m.set('c', 3);
m.size
";
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_set_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map();
m.set('a', 1);
m.set('b', 2);
m.set('c', 3);
m.size
";
			Assert.AreEqual(3L, script.Eval(code));
		}

		// map set update existing key
		[TestMethod]
		public void Test04_setUpdate()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map();
m.set('a', 1);
m.set('a', 10);
m.size
";
			Assert.AreEqual(1L, script.Eval(code));
			Assert.AreEqual(10L, script.Eval("var m = new Map(); m.set('a', 1); m.set('a', 10); m.get('a')"));
		}

		[TestMethod]
		public void Test04_setUpdate_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map();
m.set('a', 1);
m.set('a', 10);
m.size
";
			Assert.AreEqual(1L, script.Eval(code));
			Assert.AreEqual(10L, script.Eval("var m = new Map(); m.set('a', 1); m.set('a', 10); m.get('a')"));
		}

		// map get
		[TestMethod]
		public void Test05_get()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1L, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.get('a')"));
			Assert.AreEqual(2L, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.get('b')"));
		}

		[TestMethod]
		public void Test05_get_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1L, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.get('a')"));
			Assert.AreEqual(2L, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.get('b')"));
		}

		// map get non-existent key
		[TestMethod]
		public void Test06_getNonExistent()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(JavaScriptUndefined.Instance, script.Eval("var m = new Map([['a', 1]]); m.get('nonexistent')"));
		}

		[TestMethod]
		public void Test06_getNonExistent_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(JavaScriptUndefined.Instance, script.Eval("var m = new Map([['a', 1]]); m.get('nonexistent')"));
		}

		// map has
		[TestMethod]
		public void Test07_has()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.has('a')"));
			Assert.AreEqual(true, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.has('b')"));
			Assert.AreEqual(false, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.has('c')"));
		}

		[TestMethod]
		public void Test07_has_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.has('a')"));
			Assert.AreEqual(true, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.has('b')"));
			Assert.AreEqual(false, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.has('c')"));
		}

		// map delete
		[TestMethod]
		public void Test08_delete()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.delete('a')"));
			Assert.AreEqual(false, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.delete('c')"));
			Assert.AreEqual(false, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.delete('a'); m.has('a')"));
			Assert.AreEqual(1L, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.delete('a'); m.size"));
		}

		[TestMethod]
		public void Test08_delete_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.delete('a')"));
			Assert.AreEqual(false, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.delete('c')"));
			Assert.AreEqual(false, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.delete('a'); m.has('a')"));
			Assert.AreEqual(1L, script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.delete('a'); m.size"));
		}

		// map delete non-existent
		[TestMethod]
		public void Test09_deleteNonExistent()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(false, script.Eval("var m = new Map([['a', 1]]); m.delete('nonexistent')"));
			Assert.AreEqual(1L, script.Eval("var m = new Map([['a', 1]]); m.delete('nonexistent'); m.size"));
		}

		[TestMethod]
		public void Test09_deleteNonExistent_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(false, script.Eval("var m = new Map([['a', 1]]); m.delete('nonexistent')"));
			Assert.AreEqual(1L, script.Eval("var m = new Map([['a', 1]]); m.delete('nonexistent'); m.size"));
		}

		// map clear
		[TestMethod]
		public void Test10_clear()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("var m = new Map([['a', 1], ['b', 2], ['c', 3]]); m.clear(); m.size"));
		}

		[TestMethod]
		public void Test10_clear_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("var m = new Map([['a', 1], ['b', 2], ['c', 3]]); m.clear(); m.size"));
		}

		// map forEach
		[TestMethod]
		public void Test11_foreach()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map([['a', 1], ['b', 2], ['c', 3]]);
var sum = 0;
m.forEach(function(v, k) { sum += v; });
sum
";
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test11_foreach_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map([['a', 1], ['b', 2], ['c', 3]]);
var sum = 0;
m.forEach(function(v, k) { sum += v; });
sum
";
			Assert.AreEqual(6L, script.Eval(code));
		}

		// map forEach with key
		[TestMethod]
		public void Test12_foreachWithKey()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map([['a', 1], ['b', 2]]);
var keys = '';
m.forEach(function(v, k) { keys += k; });
keys
";
			Assert.AreEqual("ab", script.Eval(code));
		}

		[TestMethod]
		public void Test12_foreachWithKey_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map([['a', 1], ['b', 2]]);
var keys = '';
m.forEach(function(v, k) { keys += k; });
keys
";
			Assert.AreEqual("ab", script.Eval(code));
		}

		// map keys
		[TestMethod]
		public void Test13_keys()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var m = new Map([['a', 1], ['b', 2], ['c', 3]]); m.keys()");
			var keys = (List<object>)result;
			Assert.AreEqual(3, keys.Count);
			Assert.AreEqual("a", keys[0]);
			Assert.AreEqual("b", keys[1]);
			Assert.AreEqual("c", keys[2]);
		}

		[TestMethod]
		public void Test13_keys_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var m = new Map([['a', 1], ['b', 2], ['c', 3]]); m.keys()");
			var keys = (List<object>)result;
			Assert.AreEqual(3, keys.Count);
			Assert.AreEqual("a", keys[0]);
			Assert.AreEqual("b", keys[1]);
			Assert.AreEqual("c", keys[2]);
		}

		// map values
		[TestMethod]
		public void Test14_values()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var m = new Map([['a', 1], ['b', 2], ['c', 3]]); m.values()");
			var values = (List<object>)result;
			Assert.AreEqual(3, values.Count);
			Assert.AreEqual(1L, values[0]);
			Assert.AreEqual(2L, values[1]);
			Assert.AreEqual(3L, values[2]);
		}

		[TestMethod]
		public void Test14_values_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var m = new Map([['a', 1], ['b', 2], ['c', 3]]); m.values()");
			var values = (List<object>)result;
			Assert.AreEqual(3, values.Count);
			Assert.AreEqual(1L, values[0]);
			Assert.AreEqual(2L, values[1]);
			Assert.AreEqual(3L, values[2]);
		}

		// map entries
		[TestMethod]
		public void Test15_entries()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.entries()");
			var entries = (List<List<object>>)result;
			Assert.AreEqual(2, entries.Count);
			var entry1 = (List<object>)entries[0];
			var entry2 = (List<object>)entries[1];
			Assert.AreEqual("a", entry1[0]);
			Assert.AreEqual(1L, entry1[1]);
			Assert.AreEqual("b", entry2[0]);
			Assert.AreEqual(2L, entry2[1]);
		}

		[TestMethod]
		public void Test15_entries_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var m = new Map([['a', 1], ['b', 2]]); m.entries()");
			var entries = (List<List<object>>)result;
			Assert.AreEqual(2, entries.Count);
			var entry1 = (List<object>)entries[0];
			var entry2 = (List<object>)entries[1];
			Assert.AreEqual("a", entry1[0]);
			Assert.AreEqual(1L, entry1[1]);
			Assert.AreEqual("b", entry2[0]);
			Assert.AreEqual(2L, entry2[1]);
		}

		// map size
		[TestMethod]
		public void Test16_size()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Map().size"));
			Assert.AreEqual(2L, script.Eval("new Map([['a', 1], ['b', 2]]).size"));
			Assert.AreEqual(5L, script.Eval("new Map([['a', 1], ['b', 2], ['c', 3], ['d', 4], ['e', 5]]).size"));
		}

		[TestMethod]
		public void Test16_size_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("new Map().size"));
			Assert.AreEqual(2L, script.Eval("new Map([['a', 1], ['b', 2]]).size"));
			Assert.AreEqual(5L, script.Eval("new Map([['a', 1], ['b', 2], ['c', 3], ['d', 4], ['e', 5]]).size"));
		}

		// map with different key types
		[TestMethod]
		public void Test17_differentKeyTypes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map();
m.set(1, 'number');
m.set('str', 'string');
m.set(true, 'boolean');
m.size
";
			Assert.AreEqual(3L, script.Eval(code));
			Assert.AreEqual("number", script.Eval("var m = new Map(); m.set(1, 'number'); m.get(1)"));
			Assert.AreEqual("string", script.Eval("var m = new Map(); m.set('str', 'string'); m.get('str')"));
			Assert.AreEqual("boolean", script.Eval("var m = new Map(); m.set(true, 'boolean'); m.get(true)"));
		}

		[TestMethod]
		public void Test17_differentKeyTypes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map();
m.set(1, 'number');
m.set('str', 'string');
m.set(true, 'boolean');
m.size
";
			Assert.AreEqual(3L, script.Eval(code));
			Assert.AreEqual("number", script.Eval("var m = new Map(); m.set(1, 'number'); m.get(1)"));
			Assert.AreEqual("string", script.Eval("var m = new Map(); m.set('str', 'string'); m.get('str')"));
			Assert.AreEqual("boolean", script.Eval("var m = new Map(); m.set(true, 'boolean'); m.get(true)"));
		}

		// map operations chain
		[TestMethod]
		public void Test18_operationsChain()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map();
m.set('a', 1);
m.set('b', 2);
m.set('c', 3);
m.delete('b');
m.has('b')
";
			Assert.AreEqual(false, script.Eval(code));
		}

		[TestMethod]
		public void Test18_operationsChain_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map();
m.set('a', 1);
m.set('b', 2);
m.set('c', 3);
m.delete('b');
m.has('b')
";
			Assert.AreEqual(false, script.Eval(code));
		}

		// map with null value
		[TestMethod]
		public void Test19_nullValue()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var m = new Map(); m.set('a', null); m.has('a')"));
			Assert.AreEqual(null, script.Eval("var m = new Map(); m.set('a', null); m.get('a')"));
		}

		[TestMethod]
		public void Test19_nullValue_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("var m = new Map(); m.set('a', null); m.has('a')"));
			Assert.AreEqual(null, script.Eval("var m = new Map(); m.set('a', null); m.get('a')"));
		}

		// map for-of loop
		[TestMethod]
		public void Test20_forOf()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map([['a', 1], ['b', 2], ['c', 3]]);
var sum = 0;
for (var entry of m.entries()) {
	sum += entry[1];
}
sum
";
			Assert.AreEqual(6L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_forOf_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var m = new Map([['a', 1], ['b', 2], ['c', 3]]);
var sum = 0;
for (var entry of m.entries()) {
	sum += entry[1];
}
sum
";
			Assert.AreEqual(6L, script.Eval(code));
		}
	}
}
