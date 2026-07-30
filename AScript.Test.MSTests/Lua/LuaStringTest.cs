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
			var result1 = script.Eval<Tuple<long, long>>("string.find('hello', 'he')");
			Assert.IsNotNull(result1);
			Assert.AreEqual(1L, result1.Item1);
			Assert.AreEqual(2L, result1.Item2);

			var result2 = script.Eval<Tuple<long, long>>("string.find('hello', 'el')");
			Assert.IsNotNull(result2);
			Assert.AreEqual(2L, result2.Item1);
			Assert.AreEqual(3L, result2.Item2);

			var result3 = script.Eval<Tuple<long, long>>("string.find('hello', 'lo')");
			Assert.IsNotNull(result3);
			Assert.AreEqual(4L, result3.Item1);
			Assert.AreEqual(5L, result3.Item2);

			var result4 = script.Eval<Tuple<long, long>>("string.find('hello', 'xyz')");
			Assert.IsNull(result4);

			// find(s, pattern, init) - 从 init 位置开始查找
			var result5 = script.Eval<Tuple<long, long>>("string.find('hello', 'l', 3)");
			Assert.IsNotNull(result5);
			Assert.AreEqual(3L, result5.Item1);
			Assert.AreEqual(3L, result5.Item2);

			var result6 = script.Eval<Tuple<long, long>>("string.find('hello', 'l', 4)");
			Assert.IsNotNull(result6);
			Assert.AreEqual(4L, result6.Item1);
			Assert.AreEqual(4L, result6.Item2);

			var result7 = script.Eval<Tuple<long, long>>("string.find('hello', 'l', 5)");
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
			var result1 = script.Eval<Tuple<long, long>>("string.find('hello', 'he')");
			Assert.IsNotNull(result1);
			Assert.AreEqual(1L, result1.Item1);
			Assert.AreEqual(2L, result1.Item2);

			var result2 = script.Eval<Tuple<long, long>>("string.find('hello', 'el')");
			Assert.IsNotNull(result2);
			Assert.AreEqual(2L, result2.Item1);
			Assert.AreEqual(3L, result2.Item2);

			var result3 = script.Eval<Tuple<long, long>>("string.find('hello', 'lo')");
			Assert.IsNotNull(result3);
			Assert.AreEqual(4L, result3.Item1);
			Assert.AreEqual(5L, result3.Item2);

			var result4 = script.Eval<Tuple<long, long>>("string.find('hello', 'xyz')");
			Assert.IsNull(result4);

			var result5 = script.Eval<Tuple<long, long>>("string.find('hello', 'l', 3)");
			Assert.IsNotNull(result5);
			Assert.AreEqual(3L, result5.Item1);
			Assert.AreEqual(3L, result5.Item2);

			var result6 = script.Eval<Tuple<long, long>>("string.find('hello', 'l', 4)");
			Assert.IsNotNull(result6);
			Assert.AreEqual(4L, result6.Item1);
			Assert.AreEqual(4L, result6.Item2);

			var result7 = script.Eval<Tuple<long, long>>("string.find('hello', 'l', 5)");
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
			var result1 = script.Eval<List<object>>("string.byte('Hello', 1, 3)");
			Assert.IsNotNull(result1);
			Assert.AreEqual(3, result1.Count);
			Assert.AreEqual(72L, result1[0]);
			Assert.AreEqual(101L, result1[1]);
			Assert.AreEqual(108L, result1[2]);

			var result2 = script.Eval<List<object>>("string.byte('Hello', -3, -1)");
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

			var result1 = script.Eval<List<object>>("string.byte('Hello', 1, 3)");
			Assert.IsNotNull(result1);
			Assert.AreEqual(3, result1.Count);
			Assert.AreEqual(72L, result1[0]);
			Assert.AreEqual(101L, result1[1]);
			Assert.AreEqual(108L, result1[2]);

			var result2 = script.Eval<List<object>>("string.byte('Hello', -3, -1)");
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

			Assert.AreEqual("hello,hello,hello", script.Eval("string.rep('hello', ',', 3)"));
			Assert.AreEqual("a-a-a", script.Eval("string.rep('a', '-', 3)"));
			Assert.AreEqual("aaa", script.Eval("string.rep('a', '', 3)"));
		}

		[TestMethod]
		public void Test11_String_Rep_WithSeparator_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			Assert.AreEqual("hello,hello,hello", script.Eval("string.rep('hello', ',', 3)"));
			Assert.AreEqual("a-a-a", script.Eval("string.rep('a', '-', 3)"));
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

#if NETFRAMEWORK
			var result1 = script.Eval<Tuple<string, long>>("string.gsub('hello', 'l', '*')");
			Assert.IsNotNull(result1);
			Assert.AreEqual("he**o", result1.Item1);
			Assert.AreEqual(2L, result1.Item2);

			var result2 = script.Eval<Tuple<string, long>>("string.gsub('hello', 'l', '**')");
			Assert.IsNotNull(result2);
			Assert.AreEqual("he****o", result2.Item1);
			Assert.AreEqual(2L, result2.Item2);

			var result3 = script.Eval<Tuple<string, long>>("string.gsub('hellohellohello', 'hello', 'world')");
			Assert.IsNotNull(result3);
			Assert.AreEqual("worldworldworld", result3.Item1);
			Assert.AreEqual(3L, result3.Item2);
#else
			var (r1, c1) = ((string, long))script.Eval("string.gsub('hello', 'l', '*')");
			Assert.AreEqual("he**o", r1);
			Assert.AreEqual(2L, c1);

			var (r2, c2) = ((string, long))script.Eval("string.gsub('hello', 'l', '**')");
			Assert.AreEqual("he****o", r2);
			Assert.AreEqual(2L, c2);

			var (r3, c3) = ((string, long))script.Eval("string.gsub('hellohellohello', 'hello', 'world')");
			Assert.AreEqual("worldworldworld", r3);
			Assert.AreEqual(3L, c3);
#endif
		}

		[TestMethod]
		public void Test13_String_Gsub_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

#if NETFRAMEWORK
			var result1 = script.Eval<Tuple<string, long>>("string.gsub('hello', 'l', '*')");
			Assert.IsNotNull(result1);
			Assert.AreEqual("he**o", result1.Item1);
			Assert.AreEqual(2L, result1.Item2);

			var result2 = script.Eval<Tuple<string, long>>("string.gsub('hello', 'l', '**')");
			Assert.IsNotNull(result2);
			Assert.AreEqual("he****o", result2.Item1);
			Assert.AreEqual(2L, result2.Item2);

			var result3 = script.Eval<Tuple<string, long>>("string.gsub('hellohellohello', 'hello', 'world')");
			Assert.IsNotNull(result3);
			Assert.AreEqual("worldworldworld", result3.Item1);
			Assert.AreEqual(3L, result3.Item2);
#else
			var (r1, c1) = ((string, long))script.Eval("string.gsub('hello', 'l', '*')");
			Assert.AreEqual("he**o", r1);
			Assert.AreEqual(2L, c1);

			var (r2, c2) = ((string, long))script.Eval("string.gsub('hello', 'l', '**')");
			Assert.AreEqual("he****o", r2);
			Assert.AreEqual(2L, c2);

			var (r3, c3) = ((string, long))script.Eval("string.gsub('hellohellohello', 'hello', 'world')");
			Assert.AreEqual("worldworldworld", r3);
			Assert.AreEqual(3L, c3);
#endif
		}

		[TestMethod]
		public void Test14_String_Gsub_WithCount()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

#if NETFRAMEWORK
			// 限制替换次数
			var result1 = script.Eval<Tuple<string, long>>("string.gsub('hellohello', 'l', '*', 1)");
			Assert.IsNotNull(result1);
			Assert.AreEqual("he*lohello", result1.Item1);
			Assert.AreEqual(1L, result1.Item2);

			var result2 = script.Eval<Tuple<string, long>>("string.gsub('hellohello', 'l', '*', 2)");
			Assert.IsNotNull(result2);
			Assert.AreEqual("he**ohello", result2.Item1);
			Assert.AreEqual(2L, result2.Item2);

			var result3 = script.Eval<Tuple<string, long>>("string.gsub('hellohello', 'l', '*', 3)");
			Assert.IsNotNull(result3);
			Assert.AreEqual("he**ohe*lo", result3.Item1);
			Assert.AreEqual(3L, result3.Item2);

			var result4 = script.Eval<Tuple<string, long>>("string.gsub('hellohello', 'l', '*', 10)");
			Assert.IsNotNull(result4);
			Assert.AreEqual("he**ohe**o", result4.Item1);
			Assert.AreEqual(4L, result4.Item2);
#else
			// 限制替换次数
			var (r1, c1) = ((string, long))script.Eval("string.gsub('hellohello', 'l', '*', 1)");
			Assert.AreEqual("he*lohello", r1);
			Assert.AreEqual(1L, c1);

			var (r2, c2) = ((string, long))script.Eval("string.gsub('hellohello', 'l', '*', 2)");
			Assert.AreEqual("he**ohello", r2);
			Assert.AreEqual(2L, c2);

			var (r3, c3) = ((string, long))script.Eval("string.gsub('hellohello', 'l', '*', 3)");
			Assert.AreEqual("he**ohe*lo", r3);
			Assert.AreEqual(3L, c3);

			var (r4, c4) = ((string, long))script.Eval("string.gsub('hellohello', 'l', '*', 10)");
			Assert.AreEqual("he**ohe**o", r4);
			Assert.AreEqual(4L, c4);
#endif
		}

		[TestMethod]
		public void Test14_String_Gsub_WithCount_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

#if NETFRAMEWORK
			// 限制替换次数
			var result1 = script.Eval<Tuple<string, long>>("string.gsub('hellohello', 'l', '*', 1)");
			Assert.IsNotNull(result1);
			Assert.AreEqual("he*lohello", result1.Item1);
			Assert.AreEqual(1L, result1.Item2);

			var result2 = script.Eval<Tuple<string, long>>("string.gsub('hellohello', 'l', '*', 2)");
			Assert.IsNotNull(result2);
			Assert.AreEqual("he**ohello", result2.Item1);
			Assert.AreEqual(2L, result2.Item2);

			var result3 = script.Eval<Tuple<string, long>>("string.gsub('hellohello', 'l', '*', 3)");
			Assert.IsNotNull(result3);
			Assert.AreEqual("he**ohe*lo", result3.Item1);
			Assert.AreEqual(3L, result3.Item2);

			var result4 = script.Eval<Tuple<string, long>>("string.gsub('hellohello', 'l', '*', 10)");
			Assert.IsNotNull(result4);
			Assert.AreEqual("he**ohe**o", result4.Item1);
			Assert.AreEqual(4L, result4.Item2);
#else
			// 限制替换次数
			var (r1, c1) = ((string, long))script.Eval("string.gsub('hellohello', 'l', '*', 1)");
			Assert.AreEqual("he*lohello", r1);
			Assert.AreEqual(1L, c1);

			var (r2, c2) = ((string, long))script.Eval("string.gsub('hellohello', 'l', '*', 2)");
			Assert.AreEqual("he**ohello", r2);
			Assert.AreEqual(2L, c2);

			var (r3, c3) = ((string, long))script.Eval("string.gsub('hellohello', 'l', '*', 3)");
			Assert.AreEqual("he**ohe*lo", r3);
			Assert.AreEqual(3L, c3);

			var (r4, c4) = ((string, long))script.Eval("string.gsub('hellohello', 'l', '*', 10)");
			Assert.AreEqual("he**ohe**o", r4);
			Assert.AreEqual(4L, c4);
#endif
		}

		[TestMethod]
		public void Test15_String_Match()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			var result = script.Eval<List<object>>("string.match('hello world', 'world')");
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("world", result[0]);

			var result2 = script.Eval<List<object>>("string.match('hello 123', '%d+')");
			Assert.IsNotNull(result2);
			Assert.AreEqual("123", result2[0]);
		}

		[TestMethod]
		public void Test15_String_Match_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			var result = script.Eval<List<object>>("string.match('hello world', 'world')");
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("world", result[0]);

			var result2 = script.Eval<List<object>>("string.match('hello 123', '%d+')");
			Assert.IsNotNull(result2);
			Assert.AreEqual("123", result2[0]);
		}

		[TestMethod]
		public void Test16_String_Gmatch()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			var result = script.Eval<List<object>>("string.gmatch('hello world world', 'world')");
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("world", result[0]);
			Assert.AreEqual("world", result[1]);

			var result2 = script.Eval<List<object>>("string.gmatch('1 2 3 4 5', '%d+')");
			Assert.IsNotNull(result2);
			Assert.AreEqual(5, result2.Count);
		}

		[TestMethod]
		public void Test16_String_Gmatch_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			var result = script.Eval<List<object>>("string.gmatch('hello world world', 'world')");
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("world", result[0]);
			Assert.AreEqual("world", result[1]);

			var result2 = script.Eval<List<object>>("string.gmatch('1 2 3 4 5', '%d+')");
			Assert.IsNotNull(result2);
			Assert.AreEqual(5, result2.Count);
		}

		[TestMethod]
		public void Test17_String_Pack_Unpack()
		{
			var script = new Script();
			script.Context.Langs = new[] { "lua" };

			// string.pack - 空格式字符串
			Assert.AreEqual("ABC", script.Eval("string.pack('', 65, 66, 67)"));

			// string.unpack - 空格式字符串
			var unpackResult = script.Eval<List<object>>("string.unpack('', 'ABC', 1)");
			Assert.IsNotNull(unpackResult);
			Assert.AreEqual(3, unpackResult.Count);
			Assert.AreEqual(65L, unpackResult[0]);
			Assert.AreEqual(66L, unpackResult[1]);
			Assert.AreEqual(67L, unpackResult[2]);

			// string.pack - B (unsigned byte)
			var packedB = script.Eval<string>("string.pack('BBB', 65, 66, 67)");
			Console.WriteLine("packedB:" + packedB);
			Assert.IsNotNull(packedB);
			Assert.AreEqual("ABC", packedB);

			// string.unpack - B (unsigned byte)
			var unpackB = script.Eval<List<object>>("string.unpack('BBB', 'ABC', 1)");
			Assert.IsNotNull(unpackB);
			Assert.AreEqual(4, unpackB.Count);
			Assert.AreEqual(65L, unpackB[0]);
			Assert.AreEqual(66L, unpackB[1]);
			Assert.AreEqual(67L, unpackB[2]);
			Assert.AreEqual(4L, unpackB[3]); // next position

			script.Eval("local a,b,c,d=string.unpack('BBB', 'ABC', 1)");
			Assert.AreEqual(65L, script.Eval("a"));
			Assert.AreEqual(66L, script.Eval("b"));
			Assert.AreEqual(67L, script.Eval("c"));
			Assert.AreEqual(4L, script.Eval("d")); // next position

			// string.pack - i4 (signed int 4 bytes)
			var packedI = script.Eval<string>("string.pack('i4', 256)");
			Console.WriteLine("packedI:" + packedI);
			Assert.IsNotNull(packedI);
			Assert.AreEqual(4, packedI.Length); // 4 bytes for i4

			script.Context.SetVar("packedI", packedI);
			var unpackI = script.Eval<List<object>>("string.unpack('i4', packedI)");
			Assert.AreEqual(2, unpackI.Count);
			Assert.AreEqual(256, unpackI[0]);
			Assert.AreEqual(5L, unpackI[1]);

			// string.pack - f (float)
			var packedF = script.Eval<string>("string.pack('f', 3.14)");
			Console.WriteLine("packedF:" + packedF);
			Assert.IsNotNull(packedF);
			Assert.AreEqual(4, packedF.Length); // 4 bytes for float

			script.Context.SetVar("packedF", packedF);
			var unpackF = script.Eval<List<object>>("string.unpack('f', packedF)");
			Assert.AreEqual(2, unpackF.Count);
			Assert.AreEqual(3.14, (double)unpackF[0], 0.000001);
			Assert.AreEqual(5L, unpackF[1]);

			// string.pack - d (double)
			var packedD = script.Eval<string>("string.pack('d', 3.14159)");
			Console.WriteLine("packedD:" + packedD);
			Assert.IsNotNull(packedD);
			Assert.AreEqual(8, packedD.Length); // 8 bytes for double

			script.Context.SetVar("packedD", packedD);
			var unpackD = script.Eval<List<object>>("string.unpack('d', packedD)");
			Assert.AreEqual(2, unpackD.Count);
			Assert.AreEqual(3.14159, (double)unpackD[0]);
			Assert.AreEqual(9L, unpackD[1]);

			// string.pack - h (signed short)
			var packedH = script.Eval<string>("string.pack('h', 256)");
			Console.WriteLine("packedH:" + packedH);
			Assert.IsNotNull(packedH);
			Assert.AreEqual(2, packedH.Length); // 2 bytes for short

			script.Context.SetVar("packedH", packedH);
			var unpackH = script.Eval<List<object>>("string.unpack('h', packedH)");
			Assert.AreEqual(2, unpackH.Count);
			Assert.AreEqual((short)256, unpackH[0]);
			Assert.AreEqual(3L, unpackH[1]);

			// string.pack - l (signed long)
			var packedL = script.Eval<string>("string.pack('l', 123456)");
			Console.WriteLine("packedL:" + packedL);
			Assert.IsNotNull(packedL);
			Assert.AreEqual(8, packedL.Length); // 8 bytes for long

			script.Context.SetVar("packedL", packedL);
			var unpackL = script.Eval<List<object>>("string.unpack('l', packedL)");
			Assert.AreEqual(2, unpackL.Count);
			Assert.AreEqual(123456L, unpackL[0]);
			Assert.AreEqual(9L, unpackL[1]);

		}

		[TestMethod]
		public void Test17_String_Pack_Unpack_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };

			// string.pack - 空格式字符串
			Assert.AreEqual("ABC", script.Eval("string.pack('', 65, 66, 67)"));

			// string.unpack - 空格式字符串
			var unpackResult = script.Eval<List<object>>("string.unpack('', 'ABC', 1)");
			Assert.IsNotNull(unpackResult);
			Assert.AreEqual(3, unpackResult.Count);
			Assert.AreEqual(65L, unpackResult[0]);
			Assert.AreEqual(66L, unpackResult[1]);
			Assert.AreEqual(67L, unpackResult[2]);

			// string.pack - B (unsigned byte)
			var packedB = script.Eval<string>("string.pack('BBB', 65, 66, 67)");
			Assert.IsNotNull(packedB);
			Assert.AreEqual("ABC", packedB);

			// string.unpack - B (unsigned byte)
			var unpackB = script.Eval<List<object>>("string.unpack('BBB', 'ABC', 1)");
			Assert.IsNotNull(unpackB);
			Assert.AreEqual(4, unpackB.Count);
			Assert.AreEqual(65L, unpackB[0]);
			Assert.AreEqual(66L, unpackB[1]);
			Assert.AreEqual(67L, unpackB[2]);
			Assert.AreEqual(4L, unpackB[3]); // next position

			script.Eval("local a,b,c,d=string.unpack('BBB', 'ABC', 1)");
			Assert.AreEqual(65L, script.Eval("a"));
			Assert.AreEqual(66L, script.Eval("b"));
			Assert.AreEqual(67L, script.Eval("c"));
			Assert.AreEqual(4L, script.Eval("d")); // next position

			// string.pack - i4 (signed int 4 bytes)
			var packedI = script.Eval<string>("string.pack('i4', 256)");
			Assert.IsNotNull(packedI);
			Assert.AreEqual(4, packedI.Length); // 4 bytes for i4

			script.Context.SetVar("packedI", packedI);
			var unpackI = script.Eval<List<object>>("string.unpack('i4', packedI)");
			Assert.AreEqual(2, unpackI.Count);
			Assert.AreEqual(256, unpackI[0]);
			Assert.AreEqual(5L, unpackI[1]);

			// string.pack - f (float)
			var packedF = script.Eval<string>("string.pack('f', 3.14)");
			Assert.IsNotNull(packedF);
			Assert.AreEqual(4, packedF.Length); // 4 bytes for float

			script.Context.SetVar("packedF", packedF);
			var unpackF = script.Eval<List<object>>("string.unpack('f', packedF)");
			Assert.AreEqual(2, unpackF.Count);
			Assert.AreEqual(3.14, (double)unpackF[0], 0.000001);
			Assert.AreEqual(5L, unpackF[1]);

			// string.pack - d (double)
			var packedD = script.Eval<string>("string.pack('d', 3.14159)");
			Assert.IsNotNull(packedD);
			Assert.AreEqual(8, packedD.Length); // 8 bytes for double

			script.Context.SetVar("packedD", packedD);
			var unpackD = script.Eval<List<object>>("string.unpack('d', packedD)");
			Assert.AreEqual(2, unpackD.Count);
			Assert.AreEqual(3.14159, (double)unpackD[0]);
			Assert.AreEqual(9L, unpackD[1]);

			// string.pack - h (signed short)
			var packedH = script.Eval<string>("string.pack('h', 256)");
			Assert.IsNotNull(packedH);
			Assert.AreEqual(2, packedH.Length); // 2 bytes for short

			script.Context.SetVar("packedH", packedH);
			var unpackH = script.Eval<List<object>>("string.unpack('h', packedH)");
			Assert.AreEqual(2, unpackH.Count);
			Assert.AreEqual((short)256, unpackH[0]);
			Assert.AreEqual(3L, unpackH[1]);

			// string.pack - l (signed long)
			var packedL = script.Eval<string>("string.pack('l', 123456)");
			Assert.IsNotNull(packedL);
			Assert.AreEqual(8, packedL.Length); // 8 bytes for long

			script.Context.SetVar("packedL", packedL);
			var unpackL = script.Eval<List<object>>("string.unpack('l', packedL)");
			Assert.AreEqual(2, unpackL.Count);
			Assert.AreEqual(123456L, unpackL[0]);
			Assert.AreEqual(9L, unpackL[1]);

		}
	}
}
