using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using AScript.Nodes;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptCompileTest
	{
		#region Compile<T> 基础类型测试

		[TestMethod]
		public void Compile_Int_Expression()
		{
			var script = new Script();
			var func = script.Compile<int>("1 + 2");
			Assert.IsNotNull(func);
			Assert.AreEqual(3, func());
		}

		[TestMethod]
		public void Compile_String_Expression()
		{
			var script = new Script();
			var func = script.Compile<string>("\"hello\" + \"world\"");
			Assert.IsNotNull(func);
			Assert.AreEqual("helloworld", func());
		}

		[TestMethod]
		public void Compile_Bool_Expression()
		{
			var script = new Script();
			var func = script.Compile<bool>("1 == 1");
			Assert.IsNotNull(func);
			Assert.IsTrue(func());
		}

		[TestMethod]
		public void Compile_Double_Expression()
		{
			var script = new Script();
			var func = script.Compile<double>("3.14 * 2");
			Assert.IsNotNull(func);
			Assert.AreEqual(6.28, func());
		}

		[TestMethod]
		public void Compile_Variable_Declare()
		{
			var script = new Script();
			var func = script.Compile<int>("int a = 5; a + 3");
			Assert.IsNotNull(func);
			Assert.AreEqual(8, func());
		}

		[TestMethod]
		public void Compile_Null_Expression()
		{
			var script = new Script();
			var func = script.Compile<object>("null");
			Assert.IsNotNull(func);
			Assert.IsNull(func());
		}

		#endregion

		#region Compile<T> 带缓存测试

		[TestMethod]
		public void Compile_WithCache_NoCacheKey()
		{
			var script = new Script();
			var func1 = script.Compile<int>("1 + 2", cacheTime: 1000);
			var func2 = script.Compile<int>("1 + 2", cacheTime: 1000);
			Assert.IsNotNull(func1);
			Assert.IsNotNull(func2);
			Assert.AreEqual(3, func1());
			Assert.AreEqual(3, func2());
		}

		[TestMethod]
		public void Compile_WithCache_WithCacheKey_SameResult()
		{
			var script = new Script();
			var func1 = script.Compile<int>("1 + 2", cacheTime: 1000, cacheKey: "testKey");
			var func2 = script.Compile<int>("1 + 2", cacheTime: 1000, cacheKey: "testKey");
			Assert.IsNotNull(func1);
			Assert.IsNotNull(func2);
			Assert.AreEqual(3, func1());
			Assert.AreEqual(3, func2());
		}

		[TestMethod]
		public void Compile_WithCache_DifferentCacheKeys()
		{
			var script = new Script();
			var func1 = script.Compile<int>("1 + 2", cacheTime: 1000, cacheKey: "key1");
			var func2 = script.Compile<int>("3 + 4", cacheTime: 1000, cacheKey: "key2");
			Assert.IsNotNull(func1);
			Assert.IsNotNull(func2);
			Assert.AreEqual(3, func1());
			Assert.AreEqual(7, func2());
		}

		[TestMethod]
		public void Compile_WithCache_Versions()
		{
			var script = new Script();
			var func1 = script.Compile<int>("1 + 2", cacheTime: 1000, cacheKey: "testKey", cacheVersion: "v1");
			var func2 = script.Compile<int>("1 + 2", cacheTime: 1000, cacheKey: "testKey", cacheVersion: "v2");
			Assert.IsNotNull(func1);
			Assert.IsNotNull(func2);
			Assert.AreEqual(3, func1());
			Assert.AreEqual(3, func2());
		}

		[TestMethod]
		public void Compile_WithCache_PermanentCache()
		{
			var script = new Script();
			var func1 = script.Compile<int>("5 + 5", cacheTime: -1, cacheKey: "permanent");
			var func2 = script.Compile<int>("5 + 5", cacheTime: -1, cacheKey: "permanent");
			Assert.IsNotNull(func1);
			Assert.IsNotNull(func2);
			Assert.AreEqual(10, func1());
			Assert.AreEqual(10, func2());
		}

		[TestMethod]
		public void Compile_WithoutCache_CacheTime0()
		{
			var script = new Script();
			var func1 = script.Compile<int>("1 + 2", cacheTime: 0);
			var func2 = script.Compile<int>("1 + 2", cacheTime: 0);
			Assert.IsNotNull(func1);
			Assert.IsNotNull(func2);
			Assert.AreEqual(3, func1());
			Assert.AreEqual(3, func2());
		}

		#endregion

		#region Compile<T> Stream测试

		[TestMethod]
		public void Compile_Stream_Int()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("1 + 2"));
			var func = script.Compile<int>(stream);
			Assert.IsNotNull(func);
			Assert.AreEqual(3, func());
		}

		[TestMethod]
		public void Compile_Stream_String()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("\"test\" + \"stream\""));
			var func = script.Compile<string>(stream);
			Assert.IsNotNull(func);
			Assert.AreEqual("teststream", func());
		}

		[TestMethod]
		public void Compile_Stream_WithCache()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("10 * 2"));
			var func = script.Compile<int>(stream, cacheTime: 1000, cacheKey: "streamKey");
			Assert.IsNotNull(func);
			Assert.AreEqual(20, func());
		}

		[TestMethod]
		public void Compile_Stream_Null()
		{
			var script = new Script();
			var func = script.Compile<int>((Stream)null);
			Assert.IsNull(func);
		}

		[TestMethod]
		public void Compile_Stream_Empty()
		{
			var script = new Script();
			var stream = new MemoryStream();
			var func = script.Compile<int>(stream);
			Assert.IsNull(func);
		}

		#endregion

		#region Compile<T> Func<string> 测试

		[TestMethod]
		public void Compile_FuncString_Int()
		{
			var script = new Script();
			var func = script.Compile<int>(() => "3 + 4");
			Assert.IsNotNull(func);
			Assert.AreEqual(7, func());
		}

		[TestMethod]
		public void Compile_FuncString_WithCache()
		{
			var script = new Script();
			var func = script.Compile<int>(() => "5 * 5", cacheTime: 1000, cacheKey: "funcKey");
			Assert.IsNotNull(func);
			Assert.AreEqual(25, func());
		}

		[TestMethod]
		public void Compile_FuncString_Null()
		{
			var script = new Script();
			var func = script.Compile<int>((Func<string>)null);
			Assert.IsNull(func);
		}

		#endregion

		#region Compile<T> Func<Stream> 测试

		[TestMethod]
		public void Compile_FuncStream_Int()
		{
			var script = new Script();
			var func = script.Compile<int>(() => new MemoryStream(Encoding.UTF8.GetBytes("6 + 6")));
			Assert.IsNotNull(func);
			Assert.AreEqual(12, func());
		}

		[TestMethod]
		public void Compile_FuncStream_WithCache()
		{
			var script = new Script();
			var func = script.Compile<int>(() => new MemoryStream(Encoding.UTF8.GetBytes("100 / 2")), cacheTime: 1000, cacheKey: "funcStreamKey");
			Assert.IsNotNull(func);
			Assert.AreEqual(50, func());
		}

		[TestMethod]
		public void Compile_FuncStream_Null()
		{
			var script = new Script();
			var func = script.Compile<int>((Func<Stream>)null);
			Assert.IsNull(func);
		}

		#endregion

		#region Compile<T> ITreeNode 测试

		[TestMethod]
		public void Compile_TreeNode_Int()
		{
			var script = new Script();
			var node = script.BuildNode("7 + 8");
			var func = script.Compile<int>(node);
			Assert.IsNotNull(func);
			Assert.AreEqual(15, func());
		}

		[TestMethod]
		public void Compile_TreeNode_String()
		{
			var script = new Script();
			var node = script.BuildNode("\"hello\" + \"tree\"");
			var func = script.Compile<string>(node);
			Assert.IsNotNull(func);
			Assert.AreEqual("hellotree", func());
		}

		[TestMethod]
		public void Compile_TreeNode_Null()
		{
			var script = new Script();
			var func = script.Compile<int>((ITreeNode)null);
			Assert.IsNull(func);
		}

		#endregion

		#region Compile(expression, argTypes, argNames, returnType) 测试

		[TestMethod]
		public void Compile_WithArgs_SingleArg()
		{
			var script = new Script();
			var func = script.Compile("a * 2", new[] { typeof(int) }, new[] { "a" }, typeof(int));
			Assert.IsNotNull(func);
			var result = func.DynamicInvoke(5);
			Assert.AreEqual(10, result);
		}

		[TestMethod]
		public void Compile_WithArgs_TwoArgs()
		{
			var script = new Script();
			var func = script.Compile("a + b", new[] { typeof(int), typeof(int) }, new[] { "a", "b" }, typeof(int));
			Assert.IsNotNull(func);
			var result = func.DynamicInvoke(3, 4);
			Assert.AreEqual(7, result);
		}

		[TestMethod]
		public void Compile_WithArgs_ThreeArgs()
		{
			var script = new Script();
			var func = script.Compile("a + b + c", new[] { typeof(int), typeof(int), typeof(int) }, new[] { "a", "b", "c" }, typeof(int));
			Assert.IsNotNull(func);
			var result = func.DynamicInvoke(1, 2, 3);
			Assert.AreEqual(6, result);
		}

		[TestMethod]
		public void Compile_WithArgs_StringArg()
		{
			var script = new Script();
			var func = script.Compile("a.Length", new[] { typeof(string) }, new[] { "a" }, typeof(int));
			Assert.IsNotNull(func);
			var result = func.DynamicInvoke("hello");
			Assert.AreEqual(5, result);
		}

		[TestMethod]
		public void Compile_WithArgs_NoArgs()
		{
			var script = new Script();
			var func = script.Compile("10 + 20", null, null, typeof(int));
			Assert.IsNotNull(func);
			var result = func.DynamicInvoke();
			Assert.AreEqual(30, result);
		}

		[TestMethod]
		public void Compile_WithArgs_TypeMismatch()
		{
			var script = new Script();
			var func = script.Compile("a + b", new[] { typeof(string), typeof(string) }, new[] { "a", "b" }, typeof(string));
			Assert.IsNotNull(func);
			var result = func.DynamicInvoke("hello", "world");
			Assert.AreEqual("helloworld", result);
		}

		[TestMethod]
		public void Compile_WithArgs_ArgTypesAndNamesCountMismatch()
		{
			var script = new Script();
			try
			{
				script.Compile("a + b", new[] { typeof(int) }, new[] { "a", "b" }, typeof(int));
				Assert.Fail("Expected exception");
			}
			catch (Exception ex)
			{
				Assert.AreEqual("argTypes数量[1]与argNames数量[2]不一致", ex.Message);
			}
		}

		[TestMethod]
		public void Compile_WithArgs_EmptyExpression()
		{
			var script = new Script();
			var func = script.Compile("", new[] { typeof(int) }, new[] { "a" }, typeof(int));
			Assert.IsNull(func);
		}

		#endregion

		#region Compile Stream(expression, argTypes, argNames, returnType) 测试

		[TestMethod]
		public void Compile_Stream_WithArgs_SingleArg()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("a * 10"));
			var func = script.Compile(stream, new[] { typeof(int) }, new[] { "a" }, typeof(int));
			Assert.IsNotNull(func);
			var result = func.DynamicInvoke(7);
			Assert.AreEqual(70, result);
		}

		[TestMethod]
		public void Compile_Stream_WithArgs_TwoArgs()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("a - b"));
			var func = script.Compile(stream, new[] { typeof(int), typeof(int) }, new[] { "a", "b" }, typeof(int));
			Assert.IsNotNull(func);
			var result = func.DynamicInvoke(10, 3);
			Assert.AreEqual(7, result);
		}

		[TestMethod]
		public void Compile_Stream_WithArgs_NullStream()
		{
			var script = new Script();
			var func = script.Compile((Stream)null, new[] { typeof(int) }, new[] { "a" }, typeof(int));
			Assert.IsNull(func);
		}

		#endregion

		#region Compile<TDelegate> 测试

		[TestMethod]
		public void Compile_DelegateType_FuncIntInt()
		{
			var script = new Script();
			var func = script.Compile<Func<int, int>>("a * 2", new[] { "a" });
			Assert.IsNotNull(func);
			Assert.AreEqual(10, func(5));
		}

		[TestMethod]
		public void Compile_DelegateType_FuncIntIntInt()
		{
			var script = new Script();
			var func = script.Compile<Func<int, int, int>>("a + b", new[] { "a", "b" });
			Assert.IsNotNull(func);
			Assert.AreEqual(15, func(7, 8));
		}

		[TestMethod]
		public void Compile_DelegateType_FuncIntIntIntInt()
		{
			var script = new Script();
			var func = script.Compile<Func<int, int, int, int>>("a + b + c", new[] { "a", "b", "c" });
			Assert.IsNotNull(func);
			Assert.AreEqual(20, func(5, 7, 8));
		}

		[TestMethod]
		public void Compile_DelegateType_EmptyExpression()
		{
			var script = new Script();
			var func = script.Compile<Func<int, int>>("", new[] { "a" });
			Assert.IsNull(func);
		}

		#endregion

		#region Compile Stream<TDelegate> 测试

		[TestMethod]
		public void Compile_Stream_DelegateType_FuncIntInt()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("a * 3"));
			var func = script.Compile<Func<int, int>>(stream, new[] { "a" });
			Assert.IsNotNull(func);
			Assert.AreEqual(15, func(5));
		}

		[TestMethod]
		public void Compile_Stream_DelegateType_NullStream()
		{
			var script = new Script();
			var func = script.Compile<Func<int, int>>((Stream)null, new[] { "a" });
			Assert.IsNull(func);
		}

		#endregion

		#region Compile<T1, TReturn> 强类型泛型测试

		[TestMethod]
		public void Compile_Complex()
		{
			string s = @"
int add(int a, int b) { return a + b + x; }
int square(int a) { return a * a * x; }
int result = add(square(2), square(3)) + x;
result
";
			var script = new Script();
			var func = script.Compile<int, int>(s, "x");
			Assert.IsNotNull(func);
			Assert.AreEqual(2 * 2 * 5 + 3 * 3 * 5 + 5 + 5, func(5));
		}

		[TestMethod]
		public void Compile_Gen_T1TReturn_IntInt()
		{
			var script = new Script();
			var func = script.Compile<int, int>("n + 100", "n");
			Assert.IsNotNull(func);
			Assert.AreEqual(105, func(5));
		}

		[TestMethod]
		public void Compile_Gen_T1TReturn_StringInt()
		{
			var script = new Script();
			var func = script.Compile<string, int>("s.Length", "s");
			Assert.IsNotNull(func);
			Assert.AreEqual(5, func("hello"));
		}

		[TestMethod]
		public void Compile_Gen_T1TReturn_IntString()
		{
			var script = new Script();
			var func = script.Compile<int, string>("\"number:\" + n", "n");
			Assert.IsNotNull(func);
			Assert.AreEqual("number:42", func(42));
		}

		[TestMethod]
		public void Compile_Gen_T1TReturn_Stream()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("a * a"));
			var func = script.Compile<int, int>(stream, "a");
			Assert.IsNotNull(func);
			Assert.AreEqual(25, func(5));
		}

		#endregion

		#region Compile<T1, T2, TReturn> 双参数泛型测试

		[TestMethod]
		public void Compile_Gen_T1T2TReturn_IntIntInt()
		{
			var script = new Script();
			var func = script.Compile<int, int, int>("a * b", "a", "b");
			Assert.IsNotNull(func);
			Assert.AreEqual(24, func(6, 4));
		}

		[TestMethod]
		public void Compile_Gen_T1T2TReturn_StringIntString()
		{
			var script = new Script();
			var func = script.Compile<string, int, string>("s + n", "s", "n");
			Assert.IsNotNull(func);
			Assert.AreEqual("hello5", func("hello", 5));
		}

		[TestMethod]
		public void Compile_Gen_T1T2TReturn_Stream()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("a + b"));
			var func = script.Compile<int, int, int>(stream, "a", "b");
			Assert.IsNotNull(func);
			Assert.AreEqual(100, func(60, 40));
		}

		#endregion

		#region Compile<T1, T2, T3, TReturn> 三参数泛型测试

		[TestMethod]
		public void Compile_Gen_T1T2T3TReturn()
		{
			var script = new Script();
			var func = script.Compile<int, int, int, int>("(a + b) * c", "a", "b", "c");
			Assert.IsNotNull(func);
			Assert.AreEqual(21, func(3, 4, 3));
		}

		[TestMethod]
		public void Compile_Gen_T1T2T3TReturn_Stream()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("a + b + c"));
			var func = script.Compile<int, int, int, int>(stream, "a", "b", "c");
			Assert.IsNotNull(func);
			Assert.AreEqual(30, func(10, 10, 10));
		}

		#endregion

		#region Compile<T1, T2, T3, T4, TReturn> 四参数泛型测试

		[TestMethod]
		public void Compile_Gen_T1T2T3T4TReturn()
		{
			var script = new Script();
			var func = script.Compile<int, int, int, int, int>("a + b + c + d", "a", "b", "c", "d");
			Assert.IsNotNull(func);
			Assert.AreEqual(40, func(10, 10, 10, 10));
		}

		[TestMethod]
		public void Compile_Gen_T1T2T3T4TReturn_Stream()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("a * b * c * d"));
			var func = script.Compile<int, int, int, int, int>(stream, "a", "b", "c", "d");
			Assert.IsNotNull(func);
			Assert.AreEqual(24, func(2, 2, 2, 3));
		}

		#endregion

		#region Compile<T1, T2, T3, T4, T5, TReturn> 五参数泛型测试

		[TestMethod]
		public void Compile_Gen_T1T2T3T4T5TReturn()
		{
			var script = new Script();
			var func = script.Compile<int, int, int, int, int, int>("a + b + c + d + e", "a", "b", "c", "d", "e");
			Assert.IsNotNull(func);
			Assert.AreEqual(50, func(10, 10, 10, 10, 10));
		}

		[TestMethod]
		public void Compile_Gen_T1T2T3T4T5TReturn_Stream()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("a * 2 + b * 3 + c + d + e"));
			var func = script.Compile<int, int, int, int, int, int>(stream, "a", "b", "c", "d", "e");
			Assert.IsNotNull(func);
			Assert.AreEqual(5 * 2 + 5 * 3 + 5 + 5 + 5, func(5, 5, 5, 5, 5));
		}

		#endregion

		#region 复杂表达式测试

		[TestMethod]
		public void Compile_Complex_Arithmetic()
		{
			var script = new Script();
			var func = script.Compile<int>("(10 + 20) * 3 - 50 / 2");
			Assert.IsNotNull(func);
			Assert.AreEqual((10 + 20) * 3 - 50 / 2, func());
		}

		[TestMethod]
		public void Compile_Complex_Conditional()
		{
			var script = new Script();
			var func = script.Compile("a > b ? a : b", new[] { typeof(int), typeof(int) }, new[] { "a", "b" }, typeof(int));
			Assert.IsNotNull(func);
			Assert.AreEqual(10, func.DynamicInvoke(10, 5));
			Assert.AreEqual(10, func.DynamicInvoke(5, 10));
		}

		[TestMethod]
		public void Compile_Complex_MethodCall()
		{
			var script = new Script();
			script.Context.AddFunc<int, int>("double", n => n * 2);
			var func = script.Compile<int>("double(double(5))");
			Assert.IsNotNull(func);
			Assert.AreEqual(20, func());
		}

		[TestMethod]
		public void Compile_Complex_VariableAssignment()
		{
			var script = new Script();
			var func = script.Compile<int>("int a = 10; int b = 20; a + b");
			Assert.IsNotNull(func);
			Assert.AreEqual(30, func());
		}

		[TestMethod]
		public void Compile_Complex_NullCoalescing()
		{
			var script = new Script();
			var func = script.Compile<string>("string s = null; s ?? \"default\"");
			Assert.IsNotNull(func);
			Assert.AreEqual("default", func());
		}

		[TestMethod]
		public void Compile_Complex_StringConcat()
		{
			var script = new Script();
			var func = script.Compile<string>("\"hello\" + \" \" + \"world\"");
			Assert.IsNotNull(func);
			Assert.AreEqual("hello world", func());
		}

		#endregion

		#region 边界情况测试

		[TestMethod]
		public void Compile_EmptyExpression()
		{
			var script = new Script();
			var func = script.Compile<int>("");
			Assert.IsNull(func);
		}

		[TestMethod]
		public void Compile_WhitespaceExpression()
		{
			var script = new Script();
			var func = script.Compile<Action>("   ");
			func();
		}

		[TestMethod]
		public void Compile_NegativeNumber()
		{
			var script = new Script();
			var func = script.Compile<int>("-100");
			Assert.IsNotNull(func);
			Assert.AreEqual(-100, func());
		}

		[TestMethod]
		public void Compile_DecimalNumber()
		{
			var script = new Script();
			var func = script.Compile<double>("3.14159");
			Assert.IsNotNull(func);
			Assert.AreEqual(3.14159, func());
		}

		[TestMethod]
		public void Compile_BooleanTrue()
		{
			var script = new Script();
			var func = script.Compile<bool>("true");
			Assert.IsNotNull(func);
			Assert.IsTrue(func());
		}

		[TestMethod]
		public void Compile_BooleanFalse()
		{
			var script = new Script();
			var func = script.Compile<bool>("false");
			Assert.IsNotNull(func);
			Assert.IsFalse(func());
		}

		#endregion

		#region CompileGlobal 对比测试

		[TestMethod]
		public void Compile_CompareWithCompileGlobal_Int()
		{
			var script = new Script();
			var compileFunc = script.Compile<int>("1 + 2");
			var compileGlobalFunc = script.CompileGlobal<int>("1 + 2");

			Assert.IsNotNull(compileFunc);
			Assert.IsNotNull(compileGlobalFunc);
			Assert.AreEqual(compileGlobalFunc(script.Context), compileFunc());
		}

		[TestMethod]
		public void Compile_CompareWithCompileGlobal_Complex()
		{
			var script = new Script();
			var expression = "int a = 5; int b = 10; a + b * 2";

			var compileFunc = script.Compile<int>(expression);
			var compileGlobalFunc = script.CompileGlobal<int>(expression);

			Assert.IsNotNull(compileFunc);
			Assert.IsNotNull(compileGlobalFunc);
			Assert.AreEqual(compileGlobalFunc(script.Context), compileFunc());
		}

		#endregion
	}
}
