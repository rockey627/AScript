using AScript.Lang.Lua;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests.Lua
{
	[TestClass]
	public class LuaStringTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["lua"] = LuaLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("lua");
		}

		[TestMethod]
		public void Test01_String_Len()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(5L, script.Eval("string.len('hello')"));
			Assert.AreEqual(0L, script.Eval("string.len('')"));
			Assert.AreEqual(11L, script.Eval("string.len('hello world')"));
		}

		[TestMethod]
		public void Test01_String_Len_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(5L, script.Eval("string.len('hello')"));
			Assert.AreEqual(0L, script.Eval("string.len('')"));
			Assert.AreEqual(11L, script.Eval("string.len('hello world')"));
		}

		[TestMethod]
		public void Test02_String_Sub()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			// sub(s, i) - 从位置 i 截取到末尾
			Assert.AreEqual("ello", script.Eval("string.sub('hello', 2)"));
			Assert.AreEqual("hello", script.Eval("string.sub('hello', 1)"));
			Assert.AreEqual("o", script.Eval("string.sub('hello', 5)"));
			Assert.AreEqual("", script.Eval("string.sub('hello', 6)"));

			// sub(s, i, j) - 从位置 i 截取到位置 j
			Assert.AreEqual("ell", script.Eval("string.sub('hello', 2, 4)"));
			Assert.AreEqual("hello", script.Eval("string.sub('hello', 1, 5)"));
			Assert.AreEqual("o", script.Eval("string.sub('hello', 5, 5)"));
		}

		[TestMethod]
		public void Test02_String_Sub_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("ello", script.Eval("string.sub('hello', 2)"));
			Assert.AreEqual("hello", script.Eval("string.sub('hello', 1)"));
			Assert.AreEqual("o", script.Eval("string.sub('hello', 5)"));
			Assert.AreEqual("", script.Eval("string.sub('hello', 6)"));

			Assert.AreEqual("ell", script.Eval("string.sub('hello', 2, 4)"));
			Assert.AreEqual("hello", script.Eval("string.sub('hello', 1, 5)"));
			Assert.AreEqual("o", script.Eval("string.sub('hello', 5, 5)"));
		}

		[TestMethod]
		public void Test03_String_Sub_NegativeIndex()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			// 负数索引 (Lua: -1 表示最后一个字符)
			Assert.AreEqual("ello", script.Eval("string.sub('hello', -4)"));
			Assert.AreEqual("o", script.Eval("string.sub('hello', -1)"));
			Assert.AreEqual("lo", script.Eval("string.sub('hello', -2)"));
			Assert.AreEqual("hell", script.Eval("string.sub('hello', 1, -2)"));
			Assert.AreEqual("ello", script.Eval("string.sub('hello', 2, -1)"));
			Assert.AreEqual("", script.Eval("string.sub('hello', -5, -6)"));
		}

		[TestMethod]
		public void Test03_String_Sub_NegativeIndex_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("ello", script.Eval("string.sub('hello', -4)"));
			Assert.AreEqual("o", script.Eval("string.sub('hello', -1)"));
			Assert.AreEqual("lo", script.Eval("string.sub('hello', -2)"));
			Assert.AreEqual("hell", script.Eval("string.sub('hello', 1, -2)"));
			Assert.AreEqual("ello", script.Eval("string.sub('hello', 2, -1)"));
			Assert.AreEqual("", script.Eval("string.sub('hello', -5, -6)"));
		}

		[TestMethod]
		public void Test04_String_Find()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			script.Eval("local startIndex, endIndex = string.find('hello', 'el')");
			Assert.AreEqual(2L, script.Eval("startIndex"));
			Assert.AreEqual(3L, script.Eval("endIndex"));

			// find(s, pattern) - 返回 (起始位置, 结束位置)，Lua 使用 1-based 索引
#if NETFRAMEWORK
			var result1 = script.Eval("string.find('hello', 'he')") as Tuple<long, long>;
			Assert.IsNotNull(result1);
			Assert.AreEqual(1L, result1.Item1);
			Assert.AreEqual(2L, result1.Item2);

			var result2 = script.Eval("string.find('hello', 'el')") as Tuple<long, long>;
			Assert.IsNotNull(result2);
			Assert.AreEqual(2L, result2.Item1);
			Assert.AreEqual(3L, result2.Item2);

			var result3 = script.Eval("string.find('hello', 'lo')") as Tuple<long, long>;
			Assert.IsNotNull(result3);
			Assert.AreEqual(4L, result3.Item1);
			Assert.AreEqual(5L, result3.Item2);

			var result4 = script.Eval("string.find('hello', 'xyz')") as Tuple<long, long>;
			Assert.IsNull(result4);

			// find(s, pattern, init) - 从 init 位置开始查找
			var result5 = script.Eval("string.find('hello', 'l', 3)") as Tuple<long, long>;
			Assert.IsNotNull(result5);
			Assert.AreEqual(3L, result5.Item1);
			Assert.AreEqual(3L, result5.Item2);

			var result6 = script.Eval("string.find('hello', 'l', 4)") as Tuple<long, long>;
			Assert.IsNotNull(result6);
			Assert.AreEqual(4L, result6.Item1);
			Assert.AreEqual(4L, result6.Item2);

			var result7 = script.Eval("string.find('hello', 'l', 5)") as Tuple<long, long>;
			Assert.IsNull(result7);
#else
			var (item1_1, item1_2) = ((long, long))script.Eval("string.find('hello', 'he')");
			Assert.AreEqual(1L, item1_1);
			Assert.AreEqual(2L, item1_2);

			var (item2_1, item2_2) = ((long, long))script.Eval("string.find('hello', 'el')");
			Assert.AreEqual(2L, item2_1);
			Assert.AreEqual(3L, item2_2);

			var (item3_1, item3_2) = ((long, long))script.Eval("string.find('hello', 'lo')");
			Assert.AreEqual(4L, item3_1);
			Assert.AreEqual(5L, item3_2);

			var (item4_1, item4_2) = ((long, long))script.Eval("string.find('hello', 'xyz')");
			Assert.AreEqual(0L, item4_1);
			Assert.AreEqual(0L, item4_2);

			// find(s, pattern, init) - 从 init 位置开始查找
			var (item5_1, item5_2) = ((long, long))script.Eval("string.find('hello', 'l', 3)");
			Assert.AreEqual(3L, item5_1);
			Assert.AreEqual(3L, item5_2);

			var (item6_1, item6_2) = ((long, long))script.Eval("string.find('hello', 'l', 4)");
			Assert.AreEqual(4L, item6_1);
			Assert.AreEqual(4L, item6_2);

			var (item7_1, item7_2) = ((long, long))script.Eval("string.find('hello', 'l', 5)");
			Assert.AreEqual(0L, item7_1);
			Assert.AreEqual(0L, item7_2);
#endif
		}

		[TestMethod]
		public void Test04_String_Find_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

#if NETFRAMEWORK
			var result1 = script.Eval("string.find('hello', 'he')") as Tuple<long, long>;
			Assert.IsNotNull(result1);
			Assert.AreEqual(1L, result1.Item1);
			Assert.AreEqual(2L, result1.Item2);

			var result2 = script.Eval("string.find('hello', 'el')") as Tuple<long, long>;
			Assert.IsNotNull(result2);
			Assert.AreEqual(2L, result2.Item1);
			Assert.AreEqual(3L, result2.Item2);

			var result3 = script.Eval("string.find('hello', 'lo')") as Tuple<long, long>;
			Assert.IsNotNull(result3);
			Assert.AreEqual(4L, result3.Item1);
			Assert.AreEqual(5L, result3.Item2);

			var result4 = script.Eval("string.find('hello', 'xyz')") as Tuple<long, long>;
			Assert.IsNull(result4);

			var result5 = script.Eval("string.find('hello', 'l', 3)") as Tuple<long, long>;
			Assert.IsNotNull(result5);
			Assert.AreEqual(3L, result5.Item1);
			Assert.AreEqual(3L, result5.Item2);

			var result6 = script.Eval("string.find('hello', 'l', 4)") as Tuple<long, long>;
			Assert.IsNotNull(result6);
			Assert.AreEqual(4L, result6.Item1);
			Assert.AreEqual(4L, result6.Item2);

			var result7 = script.Eval("string.find('hello', 'l', 5)") as Tuple<long, long>;
			Assert.IsNull(result7);
#else
			var (item1_1, item1_2) = ((long, long))script.Eval("string.find('hello', 'he')");
			Assert.AreEqual(1L, item1_1);
			Assert.AreEqual(2L, item1_2);

			var (item2_1, item2_2) = ((long, long))script.Eval("string.find('hello', 'el')");
			Assert.AreEqual(2L, item2_1);
			Assert.AreEqual(3L, item2_2);

			var (item3_1, item3_2) = ((long, long))script.Eval("string.find('hello', 'lo')");
			Assert.AreEqual(4L, item3_1);
			Assert.AreEqual(5L, item3_2);

			var (item4_1, item4_2) = ((long, long))script.Eval("string.find('hello', 'xyz')");
			Assert.AreEqual(0L, item4_1);
			Assert.AreEqual(0L, item4_2);

			var (item5_1, item5_2) = ((long, long))script.Eval("string.find('hello', 'l', 3)");
			Assert.AreEqual(3L, item5_1);
			Assert.AreEqual(3L, item5_2);

			var (item6_1, item6_2) = ((long, long))script.Eval("string.find('hello', 'l', 4)");
			Assert.AreEqual(4L, item6_1);
			Assert.AreEqual(4L, item6_2);

			var (item7_1, item7_2) = ((long, long))script.Eval("string.find('hello', 'l', 5)");
			Assert.AreEqual(0L, item7_1);
			Assert.AreEqual(0L, item7_2);
#endif
		}

		[TestMethod]
		public void Test05_String_Lower_Upper()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello", script.Eval("string.lower('HELLO')"));
			Assert.AreEqual("hello world", script.Eval("string.lower('HELLO WORLD')"));
			Assert.AreEqual("HELLO", script.Eval("string.upper('hello')"));
			Assert.AreEqual("HELLO WORLD", script.Eval("string.upper('hello world')"));
		}

		[TestMethod]
		public void Test05_String_Lower_Upper_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello", script.Eval("string.lower('HELLO')"));
			Assert.AreEqual("hello world", script.Eval("string.lower('HELLO WORLD')"));
			Assert.AreEqual("HELLO", script.Eval("string.upper('hello')"));
			Assert.AreEqual("HELLO WORLD", script.Eval("string.upper('hello world')"));
		}

		[TestMethod]
		public void Test06_String_Format()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello", script.Eval("string.format('%s', 'hello')"));
			Assert.AreEqual("hello 123", script.Eval("string.format('%s %d', 'hello', 123)"));
			Assert.AreEqual("abc 123 def", script.Eval("string.format('%s %d %s', 'abc', 123, 'def')"));
		}

		[TestMethod]
		public void Test06_String_Format_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello", script.Eval("string.format('%s', 'hello')"));
			Assert.AreEqual("hello 123", script.Eval("string.format('%s %d', 'hello', 123)"));
			Assert.AreEqual("abc 123 def", script.Eval("string.format('%s %d %s', 'abc', 123, 'def')"));
		}

		[TestMethod]
		public void Test07_String_Char()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			// string.char(72, 69, 76, 76, 79) -> "HELLO"
			Assert.AreEqual("HELLO", script.Eval("string.char(72, 69, 76, 76, 79)"));
			Assert.AreEqual("A", script.Eval("string.char(65)"));
			Assert.AreEqual("AB", script.Eval("string.char(65, 66)"));
		}

		[TestMethod]
		public void Test07_String_Char_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("HELLO", script.Eval("string.char(72, 69, 76, 76, 79)"));
			Assert.AreEqual("A", script.Eval("string.char(65)"));
			Assert.AreEqual("AB", script.Eval("string.char(65, 66)"));
		}

		[TestMethod]
		public void Test08_String_Byte()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			// 'H' = 72, 'e' = 101, 'l' = 108, 'o' = 111
			Assert.AreEqual(72L, script.Eval("string.byte('Hello')"));
			Assert.AreEqual(72L, script.Eval("string.byte('Hello', 1)"));
			Assert.AreEqual(101L, script.Eval("string.byte('Hello', 2)"));
			Assert.AreEqual(108L, script.Eval("string.byte('Hello', 3)"));
			Assert.AreEqual(111L, script.Eval("string.byte('Hello', 5)"));

			// 负数索引
			Assert.AreEqual(111L, script.Eval("string.byte('Hello', -1)"));
			Assert.AreEqual(108L, script.Eval("string.byte('Hello', -2)"));
		}

		[TestMethod]
		public void Test08_String_Byte_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual(72L, script.Eval("string.byte('Hello')"));
			Assert.AreEqual(72L, script.Eval("string.byte('Hello', 1)"));
			Assert.AreEqual(101L, script.Eval("string.byte('Hello', 2)"));
			Assert.AreEqual(108L, script.Eval("string.byte('Hello', 3)"));
			Assert.AreEqual(111L, script.Eval("string.byte('Hello', 5)"));

			Assert.AreEqual(111L, script.Eval("string.byte('Hello', -1)"));
			Assert.AreEqual(108L, script.Eval("string.byte('Hello', -2)"));
		}

		[TestMethod]
		public void Test09_String_Byte_Range()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			// byte(s, i, j) 返回多个值
			var result1 = script.Eval("string.byte('Hello', 1, 3)") as List<object>;
			Assert.IsNotNull(result1);
			Assert.AreEqual(3, result1.Count);
			Assert.AreEqual(72L, result1[0]);
			Assert.AreEqual(101L, result1[1]);
			Assert.AreEqual(108L, result1[2]);

			var result2 = script.Eval("string.byte('Hello', -3, -1)") as List<object>;
			Assert.IsNotNull(result2);
			Assert.AreEqual(3, result2.Count);
			Assert.AreEqual(108L, result2[0]);
			Assert.AreEqual(108L, result2[1]);
			Assert.AreEqual(111L, result2[2]);
		}

		[TestMethod]
		public void Test09_String_Byte_Range_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			var result1 = script.Eval("string.byte('Hello', 1, 3)") as List<object>;
			Assert.IsNotNull(result1);
			Assert.AreEqual(3, result1.Count);
			Assert.AreEqual(72L, result1[0]);
			Assert.AreEqual(101L, result1[1]);
			Assert.AreEqual(108L, result1[2]);

			var result2 = script.Eval("string.byte('Hello', -3, -1)") as List<object>;
			Assert.IsNotNull(result2);
			Assert.AreEqual(3, result2.Count);
			Assert.AreEqual(108L, result2[0]);
			Assert.AreEqual(108L, result2[1]);
			Assert.AreEqual(111L, result2[2]);
		}

		[TestMethod]
		public void Test10_String_Rep()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hellohellohello", script.Eval("string.rep('hello', 3)"));
			Assert.AreEqual("hello", script.Eval("string.rep('hello', 1)"));
			Assert.AreEqual("", script.Eval("string.rep('hello', 0)"));
		}

		[TestMethod]
		public void Test10_String_Rep_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hellohellohello", script.Eval("string.rep('hello', 3)"));
			Assert.AreEqual("hello", script.Eval("string.rep('hello', 1)"));
			Assert.AreEqual("", script.Eval("string.rep('hello', 0)"));
		}

		[TestMethod]
		public void Test11_String_Rep_WithSeparator()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello,world,hello", script.Eval("string.rep('hello', ',', 3)"));
			Assert.AreEqual("a-b-c", script.Eval("string.rep('a', '-', 3)"));
			Assert.AreEqual("aaa", script.Eval("string.rep('a', '', 3)"));
		}

		[TestMethod]
		public void Test11_String_Rep_WithSeparator_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello,world,hello", script.Eval("string.rep('hello', ',', 3)"));
			Assert.AreEqual("a-b-c", script.Eval("string.rep('a', '-', 3)"));
			Assert.AreEqual("aaa", script.Eval("string.rep('a', '', 3)"));
		}

		[TestMethod]
		public void Test12_String_Reverse()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("olleh", script.Eval("string.reverse('hello')"));
			Assert.AreEqual("54321", script.Eval("string.reverse('12345')"));
			Assert.AreEqual("", script.Eval("string.reverse('')"));
		}

		[TestMethod]
		public void Test12_String_Reverse_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("olleh", script.Eval("string.reverse('hello')"));
			Assert.AreEqual("54321", script.Eval("string.reverse('12345')"));
			Assert.AreEqual("", script.Eval("string.reverse('')"));
		}

		[TestMethod]
		public void Test13_String_Gsub()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("h*llo", script.Eval("string.gsub('hello', 'l', '*')"));
			Assert.AreEqual("he**o", script.Eval("string.gsub('hello', 'l', '**')"));
			Assert.AreEqual("worldworldworld", script.Eval("string.gsub('hellohellohello', 'hello', 'world')"));
		}

		[TestMethod]
		public void Test13_String_Gsub_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("h*llo", script.Eval("string.gsub('hello', 'l', '*')"));
			Assert.AreEqual("he**o", script.Eval("string.gsub('hello', 'l', '**')"));
			Assert.AreEqual("worldworldworld", script.Eval("string.gsub('hellohellohello', 'hello', 'world')"));
		}

		[TestMethod]
		public void Test14_String_Gsub_WithCount()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			// 限制替换次数
			Assert.AreEqual("h*llo", script.Eval("string.gsub('hellohello', 'l', '*', 1)"));
			Assert.AreEqual("h*llo*ello", script.Eval("string.gsub('hellohello', 'l', '*', 2)"));
			Assert.AreEqual("h*llo*llo", script.Eval("string.gsub('hellohello', 'l', '*', 3)"));
			Assert.AreEqual("h*llo*llo", script.Eval("string.gsub('hellohello', 'l', '*', 10)"));
		}

		[TestMethod]
		public void Test14_String_Gsub_WithCount_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("h*llo", script.Eval("string.gsub('hellohello', 'l', '*', 1)"));
			Assert.AreEqual("h*llo*ello", script.Eval("string.gsub('hellohello', 'l', '*', 2)"));
			Assert.AreEqual("h*llo*llo", script.Eval("string.gsub('hellohello', 'l', '*', 3)"));
			Assert.AreEqual("h*llo*llo", script.Eval("string.gsub('hellohello', 'l', '*', 10)"));
		}

		[TestMethod]
		public void Test15_String_Match()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			var result = script.Eval("string.match('hello world', 'world')") as List<object>;
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("world", result[0]);

			var result2 = script.Eval("string.match('hello 123', '%d+')") as List<object>;
			Assert.IsNotNull(result2);
			Assert.AreEqual("123", result2[0]);
		}

		[TestMethod]
		public void Test15_String_Match_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			var result = script.Eval("string.match('hello world', 'world')") as List<object>;
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("world", result[0]);

			var result2 = script.Eval("string.match('hello 123', '%d+')") as List<object>;
			Assert.IsNotNull(result2);
			Assert.AreEqual("123", result2[0]);
		}

		[TestMethod]
		public void Test16_String_Gmatch()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			var result = script.Eval("string.gmatch('hello world world', 'world')") as List<object>;
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("world", result[0]);
			Assert.AreEqual("world", result[1]);

			var result2 = script.Eval("string.gmatch('1 2 3 4 5', '%d+')") as List<object>;
			Assert.IsNotNull(result2);
			Assert.AreEqual(5, result2.Count);
		}

		[TestMethod]
		public void Test16_String_Gmatch_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			var result = script.Eval("string.gmatch('hello world world', 'world')") as List<object>;
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("world", result[0]);
			Assert.AreEqual("world", result[1]);

			var result2 = script.Eval("string.gmatch('1 2 3 4 5', '%d+')") as List<object>;
			Assert.IsNotNull(result2);
			Assert.AreEqual(5, result2.Count);
		}

		[TestMethod]
		public void Test17_String_Pack_Unpack()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			// string.pack
			Assert.AreEqual("ABC", script.Eval("string.pack('', 65, 66, 67)"));

			// string.unpack
			var unpackResult = script.Eval("string.unpack('', 'ABC', 1)") as List<object>;
			Assert.IsNotNull(unpackResult);
			Assert.AreEqual(3, unpackResult.Count);
			Assert.AreEqual(65L, unpackResult[0]);
			Assert.AreEqual(66L, unpackResult[1]);
			Assert.AreEqual(67L, unpackResult[2]);
		}

		[TestMethod]
		public void Test17_String_Pack_Unpack_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("ABC", script.Eval("string.pack('', 65, 66, 67)"));

			var unpackResult = script.Eval("string.unpack('', 'ABC', 1)") as List<object>;
			Assert.IsNotNull(unpackResult);
			Assert.AreEqual(3, unpackResult.Count);
			Assert.AreEqual(65L, unpackResult[0]);
			Assert.AreEqual(66L, unpackResult[1]);
			Assert.AreEqual(67L, unpackResult[2]);
		}
	}
}
