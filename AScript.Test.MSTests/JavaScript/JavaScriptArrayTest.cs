using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptArrayTest
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

		// array creation
		[TestMethod]
		public void Test01_arrayCreation()
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
		public void Test01_arrayCreation_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3]");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(3L, arr[2]);
		}

		// array empty
		[TestMethod]
		public void Test02_arrayEmpty()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[]");
			var arr = (List<object>)result;
			Assert.AreEqual(0, arr.Count);
		}

		[TestMethod]
		public void Test02_arrayEmpty_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[]");
			var arr = (List<object>)result;
			Assert.AreEqual(0, arr.Count);
		}

		// array index access
		[TestMethod]
		public void Test03_indexAccess()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1L, script.Eval("[1, 2, 3][0]"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][1]"));
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][2]"));
		}

		[TestMethod]
		public void Test03_indexAccess_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1L, script.Eval("[1, 2, 3][0]"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][1]"));
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][2]"));
		}

		// array negative index
		[TestMethod]
		public void Test04_negativeIndex()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][-1]"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][-2]"));
			Assert.AreEqual(1L, script.Eval("[1, 2, 3][-3]"));
		}

		[TestMethod]
		public void Test04_negativeIndex_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][-1]"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][-2]"));
			Assert.AreEqual(1L, script.Eval("[1, 2, 3][-3]"));
		}

		// array length
		[TestMethod]
		public void Test05_length()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval("[1, 2, 3].length"));
			Assert.AreEqual(0L, script.Eval("[].length"));
			Assert.AreEqual(5L, script.Eval("[1, 2, 3, 4, 5].length"));
		}

		[TestMethod]
		public void Test05_length_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval("[1, 2, 3].length"));
			Assert.AreEqual(0L, script.Eval("[].length"));
			Assert.AreEqual(5L, script.Eval("[1, 2, 3, 4, 5].length"));
		}

		// array slicing
		[TestMethod]
		public void Test06_slice()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5][1:3]");
			var arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(2L, arr[0]);
			Assert.AreEqual(3L, arr[1]);
		}

		[TestMethod]
		public void Test06_slice_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5][1:3]");
			var arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(2L, arr[0]);
			Assert.AreEqual(3L, arr[1]);
		}

		// array slice from start
		[TestMethod]
		public void Test07_sliceFromStart()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5][:3]");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(3L, arr[2]);
		}

		[TestMethod]
		public void Test07_sliceFromStart_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5][:3]");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(3L, arr[2]);
		}

		// array slice to end
		[TestMethod]
		public void Test08_sliceToEnd()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5][2:]");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(3L, arr[0]);
			Assert.AreEqual(4L, arr[1]);
			Assert.AreEqual(5L, arr[2]);
		}

		[TestMethod]
		public void Test08_sliceToEnd_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5][2:]");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(3L, arr[0]);
			Assert.AreEqual(4L, arr[1]);
			Assert.AreEqual(5L, arr[2]);
		}

		// array slice with negative index
		[TestMethod]
		public void Test09_sliceNegativeIndex()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5][-2:]");
			var arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(4L, arr[0]);
			Assert.AreEqual(5L, arr[1]);
		}

		[TestMethod]
		public void Test09_sliceNegativeIndex_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5][-2:]");
			var arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(4L, arr[0]);
			Assert.AreEqual(5L, arr[1]);
		}

		// array index assignment
		[TestMethod]
		public void Test10_indexAssignment()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval("var arr = [1, 2, 3]; arr[0] = 10; arr[0]"));
			Assert.AreEqual(3L, script.Eval("var arr = [1, 2, 3]; arr[0] = 10; arr.length"));
		}

		[TestMethod]
		public void Test10_indexAssignment_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(10L, script.Eval("var arr = [1, 2, 3]; arr[0] = 10; arr[0]"));
			Assert.AreEqual(3L, script.Eval("var arr = [1, 2, 3]; arr[0] = 10; arr.length"));
		}

		// for-of loop
		[TestMethod]
		public void Test11_forOf()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var list = ['a', 'b', 'c'];
var s = '';
for(var item of list) {
	s += item;
}
s
";
			Assert.AreEqual("abc", script.Eval(code));
		}

		[TestMethod]
		public void Test11_forOf_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var list = ['a', 'b', 'c'];
var s = '';
for(var item of list) {
	s += item;
}
s
";
			Assert.AreEqual("abc", script.Eval(code));
		}

		// for-in loop with index
		[TestMethod]
		public void Test12_forIn()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var list = ['a', 'b', 'c'];
var s = '';
for(var i in list) {
	s += list[i];
}
s
";
			Assert.AreEqual("abc", script.Eval(code));
		}

		[TestMethod]
		public void Test12_forIn_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var list = ['a', 'b', 'c'];
var s = '';
for(var i in list) {
	s += list[i];
}
s
";
			Assert.AreEqual("abc", script.Eval(code));
		}

		// array with mixed types
		[TestMethod]
		public void Test13_mixedTypes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 'hello', true, null]");
			var arr = (List<object>)result;
			Assert.AreEqual(4, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual("hello", arr[1]);
			Assert.AreEqual(true, arr[2]);
			Assert.AreEqual(null, arr[3]);
		}

		[TestMethod]
		public void Test13_mixedTypes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 'hello', true, null]");
			var arr = (List<object>)result;
			Assert.AreEqual(4, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual("hello", arr[1]);
			Assert.AreEqual(true, arr[2]);
			Assert.AreEqual(null, arr[3]);
		}

		// array nested
		[TestMethod]
		public void Test14_nested()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[[1, 2], [3, 4]]");
			var arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			var inner1 = (List<object>)arr[0];
			var inner2 = (List<object>)arr[1];
			Assert.AreEqual(1L, inner1[0]);
			Assert.AreEqual(2L, inner1[1]);
			Assert.AreEqual(3L, inner2[0]);
			Assert.AreEqual(4L, inner2[1]);
		}

		[TestMethod]
		public void Test14_nested_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[[1, 2], [3, 4]]");
			var arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			var inner1 = (List<object>)arr[0];
			var inner2 = (List<object>)arr[1];
			Assert.AreEqual(1L, inner1[0]);
			Assert.AreEqual(2L, inner1[1]);
			Assert.AreEqual(3L, inner2[0]);
			Assert.AreEqual(4L, inner2[1]);
		}

		// array spread via concat
		[TestMethod]
		public void Test15_concat()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2].concat([3, 4])");
			var arr = (List<object>)result;
			Assert.AreEqual(4, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(3L, arr[2]);
			Assert.AreEqual(4L, arr[3]);
		}

		[TestMethod]
		public void Test15_concat_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2].concat([3, 4])");
			var arr = (List<object>)result;
			Assert.AreEqual(4, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(3L, arr[2]);
			Assert.AreEqual(4L, arr[3]);
		}

		// array join
		[TestMethod]
		public void Test16_join()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("1,2,3", script.Eval("[1, 2, 3].join(',')"));
			Assert.AreEqual("1 2 3", script.Eval("[1, 2, 3].join(' ')"));
			Assert.AreEqual("123", script.Eval("[1, 2, 3].join('')"));
		}

		[TestMethod]
		public void Test16_join_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("1,2,3", script.Eval("[1, 2, 3].join(',')"));
			Assert.AreEqual("1 2 3", script.Eval("[1, 2, 3].join(' ')"));
			Assert.AreEqual("123", script.Eval("[1, 2, 3].join('')"));
		}

		// array indexOf
		[TestMethod]
		public void Test17_indexOf()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("[1, 2, 3].indexOf(1)"));
			Assert.AreEqual(1L, script.Eval("[1, 2, 3].indexOf(2)"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3].indexOf(3)"));
			Assert.AreEqual(-1L, script.Eval("[1, 2, 3].indexOf(4)"));
		}

		[TestMethod]
		public void Test17_indexOf_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("[1, 2, 3].indexOf(1)"));
			Assert.AreEqual(1L, script.Eval("[1, 2, 3].indexOf(2)"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3].indexOf(3)"));
			Assert.AreEqual(-1L, script.Eval("[1, 2, 3].indexOf(4)"));
		}

		// array includes
		[TestMethod]
		public void Test18_includes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("[1, 2, 3].includes(1)"));
			Assert.AreEqual(true, script.Eval("[1, 2, 3].includes(2)"));
			Assert.AreEqual(false, script.Eval("[1, 2, 3].includes(4)"));
		}

		[TestMethod]
		public void Test18_includes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("[1, 2, 3].includes(1)"));
			Assert.AreEqual(true, script.Eval("[1, 2, 3].includes(2)"));
			Assert.AreEqual(false, script.Eval("[1, 2, 3].includes(4)"));
		}

		// array reverse
		[TestMethod]
		public void Test19_reverse()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3].reverse()");
			var arr = (List<object>)result;
			Assert.AreEqual(3L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(1L, arr[2]);
		}

		[TestMethod]
		public void Test19_reverse_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3].reverse()");
			var arr = (List<object>)result;
			Assert.AreEqual(3L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(1L, arr[2]);
		}

		// array filter
		[TestMethod]
		public void Test20_filter()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5].filter(x => x % 2 == 0)");
			var arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(2L, arr[0]);
			Assert.AreEqual(4L, arr[1]);
		}

		[TestMethod]
		public void Test20_filter_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3, 4, 5].filter(x => x % 2 == 0)");
			var arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(2L, arr[0]);
			Assert.AreEqual(4L, arr[1]);
		}

		// array map
		[TestMethod]
		public void Test21_map()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3].map(x => x * 2)");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(2L, arr[0]);
			Assert.AreEqual(4L, arr[1]);
			Assert.AreEqual(6L, arr[2]);
		}

		[TestMethod]
		public void Test21_map_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3].map(x => x * 2)");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(2L, arr[0]);
			Assert.AreEqual(4L, arr[1]);
			Assert.AreEqual(6L, arr[2]);
		}

		// array reduce
		[TestMethod]
		public void Test22_reduce()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(6L, script.Eval("[1, 2, 3].reduce((acc, x) => acc + x, 0)"));
			Assert.AreEqual(10L, script.Eval("[1, 2, 3, 4].reduce((acc, x) => acc + x, 0)"));
		}

		[TestMethod]
		public void Test22_reduce_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(6L, script.Eval("[1, 2, 3].reduce((acc, x) => acc + x, 0)"));
			Assert.AreEqual(10L, script.Eval("[1, 2, 3, 4].reduce((acc, x) => acc + x, 0)"));
		}

		// array every
		[TestMethod]
		public void Test23_every()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("[2, 4, 6].every(x => x % 2 == 0)"));
			Assert.AreEqual(false, script.Eval("[2, 3, 6].every(x => x % 2 == 0)"));
		}

		[TestMethod]
		public void Test23_every_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("[2, 4, 6].every(x => x % 2 == 0)"));
			Assert.AreEqual(false, script.Eval("[2, 3, 6].every(x => x % 2 == 0)"));
		}

		// array some
		[TestMethod]
		public void Test24_some()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("[1, 3, 5].some(x => x % 2 == 0)"));
			Assert.AreEqual(false, script.Eval("[1, 3, 5].some(x => x % 2 == 0)"));
		}

		[TestMethod]
		public void Test24_some_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("[1, 3, 5].some(x => x % 2 == 0)"));
			Assert.AreEqual(false, script.Eval("[1, 3, 5].some(x => x % 2 == 0)"));
		}

		// array find
		[TestMethod]
		public void Test25_find()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(2L, script.Eval("[1, 2, 3].find(x => x > 1)"));
			Assert.AreEqual(null, script.Eval("[1, 2, 3].find(x => x > 10)"));
		}

		[TestMethod]
		public void Test25_find_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(2L, script.Eval("[1, 2, 3].find(x => x > 1)"));
			Assert.AreEqual(null, script.Eval("[1, 2, 3].find(x => x > 10)"));
		}

		// array findIndex
		[TestMethod]
		public void Test26_findIndex()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1L, script.Eval("[1, 2, 3].findIndex(x => x > 1)"));
			Assert.AreEqual(-1L, script.Eval("[1, 2, 3].findIndex(x => x > 10)"));
		}

		[TestMethod]
		public void Test26_findIndex_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(1L, script.Eval("[1, 2, 3].findIndex(x => x > 1)"));
			Assert.AreEqual(-1L, script.Eval("[1, 2, 3].findIndex(x => x > 10)"));
		}

		// array fill
		[TestMethod]
		public void Test27_fill()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3].fill(0)");
			var arr = (List<object>)result;
			Assert.AreEqual(0L, arr[0]);
			Assert.AreEqual(0L, arr[1]);
			Assert.AreEqual(0L, arr[2]);
		}

		[TestMethod]
		public void Test27_fill_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("[1, 2, 3].fill(0)");
			var arr = (List<object>)result;
			Assert.AreEqual(0L, arr[0]);
			Assert.AreEqual(0L, arr[1]);
			Assert.AreEqual(0L, arr[2]);
		}

		// array with foreach
		[TestMethod]
		public void Test28_foreach()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var code = @"
var list = [1, 2, 3];
var s = '';
list.forEach(x => { s += x; });
s
";
			Assert.AreEqual("123", script.Eval(code));
		}

		[TestMethod]
		public void Test28_foreach_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var code = @"
var list = [1, 2, 3];
var s = '';
list.forEach(x => { s += x; });
s
";
			Assert.AreEqual("123", script.Eval(code));
		}
	}
}