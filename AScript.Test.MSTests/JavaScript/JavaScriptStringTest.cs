using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptStringTest
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

		// indexOf
		[TestMethod]
		public void Test01_indexOf()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("'hello'.indexOf('h')"));
			Assert.AreEqual(-1L, script.Eval("'hello'.indexOf('h', 1)"));
			Assert.AreEqual(1L, script.Eval("'hello'.indexOf('e')"));
			Assert.AreEqual(-1L, script.Eval("'hello'.indexOf('x')"));
			Assert.AreEqual(2L, script.Eval("'hello'.indexOf('l')"));
		}

		[TestMethod]
		public void Test01_indexOf_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("'hello'.indexOf('h')"));
			Assert.AreEqual(-1L, script.Eval("'hello'.indexOf('h', 1)"));
			Assert.AreEqual(1L, script.Eval("'hello'.indexOf('e')"));
			Assert.AreEqual(-1L, script.Eval("'hello'.indexOf('x')"));
			Assert.AreEqual(2L, script.Eval("'hello'.indexOf('l')"));
		}

		// lastIndexOf
		[TestMethod]
		public void Test02_lastIndexOf()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval("'hello'.lastIndexOf('l')"));
			Assert.AreEqual(-1L, script.Eval("'hello'.lastIndexOf('x')"));
			Assert.AreEqual(1L, script.Eval("'hello'.lastIndexOf('e')"));
		}

		[TestMethod]
		public void Test02_lastIndexOf_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(3L, script.Eval("'hello'.lastIndexOf('l')"));
			Assert.AreEqual(-1L, script.Eval("'hello'.lastIndexOf('x')"));
			Assert.AreEqual(1L, script.Eval("'hello'.lastIndexOf('e')"));
		}

		// charAt
		[TestMethod]
		public void Test03_charAt()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("h", script.Eval("'hello'.charAt(0)"));
			Assert.AreEqual("e", script.Eval("'hello'.charAt(1)"));
			Assert.AreEqual("o", script.Eval("'hello'.charAt(4)"));
			Assert.AreEqual("", script.Eval("'hello'.charAt(10)"));
		}

		[TestMethod]
		public void Test03_charAt_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("h", script.Eval("'hello'.charAt(0)"));
			Assert.AreEqual("e", script.Eval("'hello'.charAt(1)"));
			Assert.AreEqual("o", script.Eval("'hello'.charAt(4)"));
			Assert.AreEqual("", script.Eval("'hello'.charAt(10)"));
		}

		// charCodeAt
		[TestMethod]
		public void Test04_charCodeAt()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(104L, script.Eval("'hello'.charCodeAt(0)"));
			Assert.AreEqual(101L, script.Eval("'hello'.charCodeAt(1)"));
			Assert.AreEqual(111L, script.Eval("'hello'.charCodeAt(4)"));
			Assert.AreEqual(-1L, script.Eval("'hello'.charCodeAt(14)"));
		}

		[TestMethod]
		public void Test04_charCodeAt_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(104L, script.Eval("'hello'.charCodeAt(0)"));
			Assert.AreEqual(101L, script.Eval("'hello'.charCodeAt(1)"));
			Assert.AreEqual(111L, script.Eval("'hello'.charCodeAt(4)"));
			Assert.AreEqual(-1L, script.Eval("'hello'.charCodeAt(14)"));
		}

		// substring
		[TestMethod]
		public void Test05_substring()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'hello'.substring(0)"));
			Assert.AreEqual("ello", script.Eval("'hello'.substring(1)"));
			Assert.AreEqual("hel", script.Eval("'hello'.substring(0, 3)"));
			Assert.AreEqual("ell", script.Eval("'hello'.substring(1, 4)"));
		}

		[TestMethod]
		public void Test05_substring_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'hello'.substring(0)"));
			Assert.AreEqual("ello", script.Eval("'hello'.substring(1)"));
			Assert.AreEqual("hel", script.Eval("'hello'.substring(0, 3)"));
			Assert.AreEqual("ell", script.Eval("'hello'.substring(1, 4)"));
		}

		// substr
		[TestMethod]
		public void Test06_substr()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'hello'.substr(0)"));
			Assert.AreEqual("ello", script.Eval("'hello'.substr(1)"));
			Assert.AreEqual("hel", script.Eval("'hello'.substr(0, 3)"));
			Assert.AreEqual("ll", script.Eval("'hello'.substr(2, 2)"));
		}

		[TestMethod]
		public void Test06_substr_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'hello'.substr(0)"));
			Assert.AreEqual("ello", script.Eval("'hello'.substr(1)"));
			Assert.AreEqual("hel", script.Eval("'hello'.substr(0, 3)"));
			Assert.AreEqual("ll", script.Eval("'hello'.substr(2, 2)"));
		}

		// slice
		[TestMethod]
		public void Test07_slice()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'hello'.slice(0)"));
			Assert.AreEqual("ello", script.Eval("'hello'.slice(1)"));
			Assert.AreEqual("hel", script.Eval("'hello'.slice(0, 3)"));
			Assert.AreEqual("lo", script.Eval("'hello'.slice(-2)"));
			Assert.AreEqual("ell", script.Eval("'hello'.slice(1, -1)"));
		}

		[TestMethod]
		public void Test07_slice_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'hello'.slice(0)"));
			Assert.AreEqual("ello", script.Eval("'hello'.slice(1)"));
			Assert.AreEqual("hel", script.Eval("'hello'.slice(0, 3)"));
			Assert.AreEqual("lo", script.Eval("'hello'.slice(-2)"));
			Assert.AreEqual("ell", script.Eval("'hello'.slice(1, -1)"));
		}

		// toLowerCase
		[TestMethod]
		public void Test08_toLowerCase()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'HELLO'.toLowerCase()"));
			Assert.AreEqual("hello world", script.Eval("'HELLO WORLD'.toLowerCase()"));
		}

		[TestMethod]
		public void Test08_toLowerCase_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'HELLO'.toLowerCase()"));
			Assert.AreEqual("hello world", script.Eval("'HELLO WORLD'.toLowerCase()"));
		}

		// toUpperCase
		[TestMethod]
		public void Test09_toUpperCase()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("HELLO", script.Eval("'hello'.toUpperCase()"));
			Assert.AreEqual("HELLO WORLD", script.Eval("'hello world'.toUpperCase()"));
		}

		[TestMethod]
		public void Test09_toUpperCase_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("HELLO", script.Eval("'hello'.toUpperCase()"));
			Assert.AreEqual("HELLO WORLD", script.Eval("'hello world'.toUpperCase()"));
		}

		// trim
		[TestMethod]
		public void Test10_trim()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'  hello  '.trim()"));
			Assert.AreEqual("hello", script.Eval("'\\n\\thello\\n\\t'.trim()"));
		}

		[TestMethod]
		public void Test10_trim_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello", script.Eval("'  hello  '.trim()"));
			Assert.AreEqual("hello", script.Eval("'\\n\\thello\\n\\t'.trim()"));
		}

		// replace
		[TestMethod]
		public void Test11_replace()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello /js/gi", script.Eval("'hello '+/js/gi"));
			Assert.AreEqual("hello js", script.Eval("'hello world'.replace('world', 'js')"));
			Assert.AreEqual("hello ", script.Eval("'hello world'.replace('world', '')"));
			Assert.AreEqual("hellx world", script.Eval("'hello world'.replace('o', 'x')"));
			Assert.AreEqual("hellx world", script.Eval("'hello world'.replace(/o/, 'x')"));
			Assert.AreEqual("hellO wxrld", script.Eval("'hellO world'.replace(/o/, 'x')"));
			Assert.AreEqual("hellx world", script.Eval("'hellO world'.replace(/o/i, 'x')"));
			Assert.AreEqual("hellx wxrld", script.Eval("'hello world'.replace(/o/g, 'x')"));
			Assert.AreEqual("hellx wOrld", script.Eval("'hello wOrld'.replace(/o/g, 'x')"));
			Assert.AreEqual("hellx wxrld", script.Eval("'hello wOrld'.replace(/o/gi, 'x')"));
			Assert.AreEqual("hellO wxrld", script.Eval("'hellO world'.replace('o', 'x')"));
		}

		[TestMethod]
		public void Test11_replace_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello /js/gi", script.Eval("'hello '+/js/gi"));
			Assert.AreEqual("hello js", script.Eval("'hello world'.replace('world', 'js')"));
			Assert.AreEqual("hello ", script.Eval("'hello world'.replace('world', '')"));
			Assert.AreEqual("hellx world", script.Eval("'hello world'.replace('o', 'x')"));
			Assert.AreEqual("hellx world", script.Eval("'hello world'.replace(/o/, 'x')"));
			Assert.AreEqual("hellO wxrld", script.Eval("'hellO world'.replace(/o/, 'x')"));
			Assert.AreEqual("hellx world", script.Eval("'hellO world'.replace(/o/i, 'x')"));
			Assert.AreEqual("hellx wxrld", script.Eval("'hello world'.replace(/o/g, 'x')"));
			Assert.AreEqual("hellx wOrld", script.Eval("'hello wOrld'.replace(/o/g, 'x')"));
			Assert.AreEqual("hellx wxrld", script.Eval("'hello wOrld'.replace(/o/gi, 'x')"));
			Assert.AreEqual("hellO wxrld", script.Eval("'hellO world'.replace('o', 'x')"));
		}

		// split
		[TestMethod]
		public void Test12_split()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("'a,b,c'.split(',')");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual("a", arr[0]);
			Assert.AreEqual("b", arr[1]);
			Assert.AreEqual("c", arr[2]);
		}

		[TestMethod]
		public void Test12_split_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("'a,b,c'.split(',')");
			var arr = (List<object>)result;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual("a", arr[0]);
			Assert.AreEqual("b", arr[1]);
			Assert.AreEqual("c", arr[2]);
		}

		[TestMethod]
		public void Test12_split_2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("'a,b,c'.split('')");
			var arr = (List<object>)result;
			Assert.AreEqual(5, arr.Count);
			Assert.AreEqual("a", arr[0]);
			Assert.AreEqual(",", arr[1]);
			Assert.AreEqual("b", arr[2]);
			Assert.AreEqual(",", arr[3]);
			Assert.AreEqual("c", arr[4]);
		}

		[TestMethod]
		public void Test12_split_2_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("'a,b,c'.split('')");
			var arr = (List<object>)result;
			Assert.AreEqual(5, arr.Count);
			Assert.AreEqual("a", arr[0]);
			Assert.AreEqual(",", arr[1]);
			Assert.AreEqual("b", arr[2]);
			Assert.AreEqual(",", arr[3]);
			Assert.AreEqual("c", arr[4]);
		}

		// concat
		[TestMethod]
		public void Test13_concat()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello world", script.Eval("'hello'.concat(' ', 'world')"));
			Assert.AreEqual("helloworld", script.Eval("'hello'.concat('world')"));
			Assert.AreEqual("helloworld34567", script.Eval("'hello'.concat('world','3','4','5','6','7')"));
		}

		[TestMethod]
		public void Test13_concat_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello world", script.Eval("'hello'.concat(' ', 'world')"));
			Assert.AreEqual("helloworld", script.Eval("'hello'.concat('world')"));
			Assert.AreEqual("helloworld34567", script.Eval("'hello'.concat('world','3','4','5','6','7')"));
		}

		// startsWith
		[TestMethod]
		public void Test14_startsWith()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("'hello'.startsWith('hel')"));
			Assert.AreEqual(false, script.Eval("'hello'.startsWith('ello')"));
		}

		[TestMethod]
		public void Test14_startsWith_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("'hello'.startsWith('hel')"));
			Assert.AreEqual(false, script.Eval("'hello'.startsWith('ello')"));
		}

		// endsWith
		[TestMethod]
		public void Test15_endsWith()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("'hello'.endsWith('llo')"));
			Assert.AreEqual(false, script.Eval("'hello'.endsWith('hel')"));
		}

		[TestMethod]
		public void Test15_endsWith_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("'hello'.endsWith('llo')"));
			Assert.AreEqual(false, script.Eval("'hello'.endsWith('hel')"));
		}

		// includes
		[TestMethod]
		public void Test16_includes()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("'hello'.includes('ell')"));
			Assert.AreEqual(false, script.Eval("'hello'.includes('xyz')"));
		}

		[TestMethod]
		public void Test16_includes_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(true, script.Eval("'hello'.includes('ell')"));
			Assert.AreEqual(false, script.Eval("'hello'.includes('xyz')"));
		}

		// repeat
		[TestMethod]
		public void Test17_repeat()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("aaa", script.Eval("'a'.repeat(3)"));
			Assert.AreEqual("aaaaaaaa", script.Eval("'a'.repeat(8)"));
			Assert.AreEqual("", script.Eval("'hello'.repeat(0)"));
			Assert.AreEqual("", script.Eval("''.repeat(10)"));
			Assert.AreEqual("hellohello", script.Eval("'hello'.repeat(2)"));
		}

		[TestMethod]
		public void Test17_repeat_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("aaa", script.Eval("'a'.repeat(3)"));
			Assert.AreEqual("aaaaaaaa", script.Eval("'a'.repeat(8)"));
			Assert.AreEqual("", script.Eval("'hello'.repeat(0)"));
			Assert.AreEqual("", script.Eval("''.repeat(10)"));
			Assert.AreEqual("hellohello", script.Eval("'hello'.repeat(2)"));
		}

		// padStart
		[TestMethod]
		public void Test18_padStart()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("xxabc", script.Eval("'abc'.padStart(5, 'x')"));
			Assert.AreEqual("abc", script.Eval("'abc'.padStart(5, '')"));
			Assert.AreEqual("  abc", script.Eval("'abc'.padStart(5)"));
		}

		[TestMethod]
		public void Test18_padStart_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("xxabc", script.Eval("'abc'.padStart(5, 'x')"));
			Assert.AreEqual("abc", script.Eval("'abc'.padStart(5, '')"));
			Assert.AreEqual("  abc", script.Eval("'abc'.padStart(5)"));
		}

		// padEnd
		[TestMethod]
		public void Test19_padEnd()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("abcxx", script.Eval("'abc'.padEnd(5, 'x')"));
			Assert.AreEqual("abc", script.Eval("'abc'.padEnd(5, '')"));
			Assert.AreEqual("abc  ", script.Eval("'abc'.padEnd(5)"));
		}

		[TestMethod]
		public void Test19_padEnd_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("abcxx", script.Eval("'abc'.padEnd(5, 'x')"));
			Assert.AreEqual("abc", script.Eval("'abc'.padEnd(5, '')"));
			Assert.AreEqual("abc  ", script.Eval("'abc'.padEnd(5)"));
		}

		// search
		[TestMethod]
		public void Test20_search()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("'hello'.search('hel')"));
			Assert.AreEqual(2L, script.Eval("'hello'.search('l')"));
			Assert.AreEqual(-1L, script.Eval("'hello'.search('L')"));
			Assert.AreEqual(2L, script.Eval("'hello'.search(/l/)"));
			Assert.AreEqual(-1L, script.Eval("'hello'.search(/L/)"));
			Assert.AreEqual(2L, script.Eval("'hello'.search(/L/i)"));
			Assert.AreEqual(2L, script.Eval("'hello'.search(/L/gi)"));
			Assert.AreEqual(-1L, script.Eval("'hello'.search('xyz')"));
		}

		[TestMethod]
		public void Test20_search_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(0L, script.Eval("'hello'.search('hel')"));
			Assert.AreEqual(2L, script.Eval("'hello'.search('l')"));
			Assert.AreEqual(-1L, script.Eval("'hello'.search('L')"));
			Assert.AreEqual(2L, script.Eval("'hello'.search(/l/)"));
			Assert.AreEqual(-1L, script.Eval("'hello'.search(/L/)"));
			Assert.AreEqual(2L, script.Eval("'hello'.search(/L/i)"));
			Assert.AreEqual(2L, script.Eval("'hello'.search(/L/gi)"));
			Assert.AreEqual(-1L, script.Eval("'hello'.search('xyz')"));
		}

		// length
		[TestMethod]
		public void Test21_length()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(5L, script.Eval("'hello'.length"));
			Assert.AreEqual(0L, script.Eval("''.length"));
			Assert.AreEqual(11L, script.Eval("'hello world'.length"));
		}

		[TestMethod]
		public void Test21_length_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual(5L, script.Eval("'hello'.length"));
			Assert.AreEqual(0L, script.Eval("''.length"));
			Assert.AreEqual(11L, script.Eval("'hello world'.length"));
		}

		// trimStart / trimLeft
		[TestMethod]
		public void Test22_trimStart()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello  ", script.Eval("'  hello  '.trimStart()"));
			Assert.AreEqual("hello", script.Eval("'\\n\\thello'.trimStart()"));
		}

		[TestMethod]
		public void Test22_trimStart_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("hello  ", script.Eval("'  hello  '.trimStart()"));
			Assert.AreEqual("hello", script.Eval("'\\n\\thello'.trimStart()"));
		}

		// trimEnd / trimRight
		[TestMethod]
		public void Test23_trimEnd()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("  hello", script.Eval("'  hello  '.trimEnd()"));
			Assert.AreEqual("hello", script.Eval("'hello\\n\\t'.trimEnd()"));
		}

		[TestMethod]
		public void Test23_trimEnd_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("  hello", script.Eval("'  hello  '.trimEnd()"));
			Assert.AreEqual("hello", script.Eval("'hello\\n\\t'.trimEnd()"));
		}

		// String.fromCharCode
		[TestMethod]
		public void Test24_fromCharCode()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("a", script.Eval("String.fromCharCode(97)"));
			Assert.AreEqual("abc", script.Eval("String.fromCharCode(97, 98, 99)"));
		}

		[TestMethod]
		public void Test24_fromCharCode_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			Assert.AreEqual("a", script.Eval("String.fromCharCode(97)"));
			Assert.AreEqual("abc", script.Eval("String.fromCharCode(97, 98, 99)"));
		}

		//// String.fromCodePoint
		//[TestMethod]
		//public void Test25_fromCodePoint()
		//{
		//	var script = new Script();
		//	script.Context.Langs = new[] { "js" };
		//	Assert.AreEqual("a", script.Eval("String.fromCodePoint(97)"));
		//	Assert.AreEqual("abc", script.Eval("String.fromCodePoint(97, 98, 99)"));
		//}

		//[TestMethod]
		//public void Test25_fromCodePoint_CompileAll()
		//{
		//	var script = new Script();
		//	script.Options.CompileMode = ECompileMode.All;
		//	script.Context.Langs = new[] { "js" };
		//	Assert.AreEqual("a", script.Eval("String.fromCodePoint(97)"));
		//	Assert.AreEqual("abc", script.Eval("String.fromCodePoint(97, 98, 99)"));
		//}

		//// String.raw
		//[TestMethod]
		//public void Test26_raw()
		//{
		//	var script = new Script();
		//	script.Context.Langs = new[] { "js" };
		//	Assert.AreEqual("hello\\nworld", script.Eval("String.raw`hello\\nworld`"));
		//}

		//[TestMethod]
		//public void Test26_raw_CompileAll()
		//{
		//	var script = new Script();
		//	script.Options.CompileMode = ECompileMode.All;
		//	script.Context.Langs = new[] { "js" };
		//	Assert.AreEqual("hello\\nworld", script.Eval("String.raw`hello\\nworld`"));
		//}

		// match
		[TestMethod]
		public void Test27_match()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			// 基本正则匹配
			var result = script.Eval("'hello world'.match('world')");
			var arr = (List<object>)result;
			Assert.AreEqual(1, arr.Count);
			Assert.AreEqual("world", arr[0]);

			// 全局匹配
			result = script.Eval("'hello world world'.match(/world/g)");
			arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual("world", arr[0]);
			Assert.AreEqual("world", arr[1]);

			result = script.Eval("'hello world World'.match(/world/g)");
			arr = (List<object>)result;
			Assert.AreEqual(1, arr.Count);
			Assert.AreEqual("world", arr[0]);

			result = script.Eval("'hello world World'.match(/world/gi)");
			arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual("world", arr[0]);
			Assert.AreEqual("World", arr[1]);

			result = script.Eval("'hello world world'.match(/world2/g)");
			Assert.IsNull(result);

			// 无匹配返回null
			Assert.AreEqual(null, script.Eval("'hello'.match('xyz')"));
		}

		[TestMethod]
		public void Test27_match_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			// 基本正则匹配
			var result = script.Eval("'hello world'.match('world')");
			var arr = (List<object>)result;
			Assert.AreEqual(1, arr.Count);
			Assert.AreEqual("world", arr[0]);

			// 全局匹配
			result = script.Eval("'hello world world'.match(/world/g)");
			arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual("world", arr[0]);
			Assert.AreEqual("world", arr[1]);

			result = script.Eval("'hello world World'.match(/world/g)");
			arr = (List<object>)result;
			Assert.AreEqual(1, arr.Count);
			Assert.AreEqual("world", arr[0]);

			result = script.Eval("'hello world World'.match(/world/gi)");
			arr = (List<object>)result;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual("world", arr[0]);
			Assert.AreEqual("World", arr[1]);

			result = script.Eval("'hello world world'.match(/world2/g)");
			Assert.IsNull(result);

			// 无匹配返回null
			Assert.AreEqual(null, script.Eval("'hello'.match('xyz')"));
		}

		// 字符串插值
		[TestMethod]
		public void Test28_stringInterpolation()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			// 基本插值
			Assert.AreEqual("hello world", script.Eval("`hello ${'world'}`"));
			Assert.AreEqual("hello world", script.Eval("var name='world';`hello ${name}`"));
			// 表达式插值
			Assert.AreEqual("3", script.Eval("`${1+2}`"));
			Assert.AreEqual("ab", script.Eval("`${'a'+'b'}`"));
			// 多重插值
			Assert.AreEqual("a b c", script.Eval("var a='a',b='b',c='c';`${a} ${b} ${c}`"));
			// 空字符串
			Assert.AreEqual("", script.Eval("``"));
			// 只有文本
			Assert.AreEqual("hello", script.Eval("`hello`"));
			// 转义字符
			Assert.AreEqual("$\n", script.Eval("`$\\n`"));
			Assert.AreEqual("$\t", script.Eval("`$\\t`"));
		}

		[TestMethod]
		public void Test28_stringInterpolation_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			// 基本插值
			Assert.AreEqual("hello world", script.Eval("`hello ${'world'}`"));
			Assert.AreEqual("hello world", script.Eval("var name='world';`hello ${name}`"));
			// 表达式插值
			Assert.AreEqual("3", script.Eval("`${1+2}`"));
			Assert.AreEqual("ab", script.Eval("`${'a'+'b'}`"));
			// 多重插值
			Assert.AreEqual("a b c", script.Eval("var a='a',b='b',c='c';`${a} ${b} ${c}`"));
			// 空字符串
			Assert.AreEqual("", script.Eval("``"));
			// 只有文本
			Assert.AreEqual("hello", script.Eval("`hello`"));
			// 转义字符
			Assert.AreEqual("$\n", script.Eval("`$\\n`"));
			Assert.AreEqual("$\t", script.Eval("`$\\t`"));
		}
	}
}