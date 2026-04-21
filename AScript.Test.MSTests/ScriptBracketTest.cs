using System;
using System.Collections;
using System.Collections.Generic;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptBracketTest
	{
		#region IndexOperator - 负索引测试

		[TestMethod]
		public void TestArray_NegativeIndex()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 10, 20, 30, 40, 50 });
			Assert.AreEqual(50, script.Eval("arr[-1]"));
			Assert.AreEqual(40, script.Eval("arr[-2]"));
			Assert.AreEqual(30, script.Eval("arr[-3]"));
			Assert.AreEqual(20, script.Eval("arr[-4]"));
			Assert.AreEqual(10, script.Eval("arr[-5]"));
		}

		[TestMethod]
		public void TestArray_NegativeIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("arr", new[] { 10, 20, 30, 40, 50 });
			Assert.AreEqual(50, script.Eval("arr[-1]"));
			Assert.AreEqual(40, script.Eval("arr[-2]"));
			Assert.AreEqual(30, script.Eval("arr[-3]"));
		}

		[TestMethod]
		public void TestList_NegativeIndex()
		{
			var script = new Script();
			script.Context.SetVar("list", new List<int> { 10, 20, 30, 40, 50 });
			Assert.AreEqual(50, script.Eval("list[-1]"));
			Assert.AreEqual(40, script.Eval("list[-2]"));
			Assert.AreEqual(30, script.Eval("list[-3]"));
		}

		[TestMethod]
		public void TestList_NegativeIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", new List<int> { 10, 20, 30, 40, 50 });
			Assert.AreEqual(50, script.Eval("list[-1]"));
			Assert.AreEqual(40, script.Eval("list[-2]"));
			Assert.AreEqual(30, script.Eval("list[-3]"));
		}

		[TestMethod]
		public void TestString_NegativeIndex()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual('o', script.Eval("str[-1]"));
			Assert.AreEqual('l', script.Eval("str[-2]"));
			Assert.AreEqual('H', script.Eval("str[-5]")); // -5 + 5 = 0 -> 'H'
		}

		[TestMethod]
		public void TestString_NegativeIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual('o', script.Eval("str[-1]"));
			Assert.AreEqual('l', script.Eval("str[-2]"));
		}

		[TestMethod]
		public void TestArray_PositiveIndex()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 10, 20, 30, 40, 50 });
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
			Assert.AreEqual(50, script.Eval("arr[4]"));
		}

		[TestMethod]
		public void TestList_PositiveIndex()
		{
			var script = new Script();
			script.Context.SetVar("list", new List<int> { 10, 20, 30, 40, 50 });
			Assert.AreEqual(10, script.Eval("list[0]"));
			Assert.AreEqual(30, script.Eval("list[2]"));
			Assert.AreEqual(50, script.Eval("list[4]"));
		}

		[TestMethod]
		public void TestString_PositiveIndex()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual('H', script.Eval("str[0]"));
			Assert.AreEqual('e', script.Eval("str[1]"));
			Assert.AreEqual('o', script.Eval("str[4]"));
		}

		#endregion

		#region IndexStartEndOperator - 起始结束索引测试

		[TestMethod]
		public void TestArray_Slice_PositiveIndex()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[1:4]");
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_PositiveIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[1:4]");
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_NegativeIndex()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[-3:-1]");
			CollectionAssert.AreEqual(new List<int> { 2, 3 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_NegativeIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[-3:-1]");
			CollectionAssert.AreEqual(new List<int> { 2, 3 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_OnlyStart()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[2:]");
			CollectionAssert.AreEqual(new List<int> { 2, 3, 4 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_OnlyStart_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[2:]");
			CollectionAssert.AreEqual(new List<int> { 2, 3, 4 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_OnlyEnd()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[:3]");
			CollectionAssert.AreEqual(new List<int> { 0, 1, 2 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_OnlyEnd_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[:3]");
			CollectionAssert.AreEqual(new List<int> { 0, 1, 2 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_NegativeStart()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[-2:]");
			CollectionAssert.AreEqual(new List<int> { 3, 4 }, result);
		}

		[TestMethod]
		public void TestArray_Slice_NegativeEnd()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[:-2]");
			CollectionAssert.AreEqual(new List<int> { 0, 1, 2 }, result);
		}

		[TestMethod]
		public void TestList_Slice_PositiveIndex()
		{
			var script = new Script();
			script.Context.SetVar("list", new List<int> { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("list[1:4]");
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result);
		}

		[TestMethod]
		public void TestList_Slice_NegativeIndex()
		{
			var script = new Script();
			script.Context.SetVar("list", new List<int> { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("list[-3:-1]");
			CollectionAssert.AreEqual(new List<int> { 2, 3 }, result);
		}

		[TestMethod]
		public void TestList_Slice_OnlyStart()
		{
			var script = new Script();
			script.Context.SetVar("list", new List<int> { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("list[2:]");
			CollectionAssert.AreEqual(new List<int> { 2, 3, 4 }, result);
		}

		[TestMethod]
		public void TestList_Slice_OnlyEnd()
		{
			var script = new Script();
			script.Context.SetVar("list", new List<int> { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("list[:3]");
			CollectionAssert.AreEqual(new List<int> { 0, 1, 2 }, result);
		}

		[TestMethod]
		public void TestString_Slice_PositiveIndex()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("ell", script.Eval("str[1:4]"));
		}

		[TestMethod]
		public void TestString_Slice_PositiveIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("ell", script.Eval("str[1:4]"));
		}

		[TestMethod]
		public void TestString_Slice_NegativeIndex()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("ell", script.Eval("str[-4:-1]"));
		}

		[TestMethod]
		public void TestString_Slice_NegativeIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("ell", script.Eval("str[-4:-1]"));
		}

		[TestMethod]
		public void TestString_Slice_OnlyStart()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("ello", script.Eval("str[1:]"));
		}

		[TestMethod]
		public void TestString_Slice_OnlyStart_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("ello", script.Eval("str[1:]"));
		}

		[TestMethod]
		public void TestString_Slice_OnlyEnd()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("Hel", script.Eval("str[:3]"));
		}

		[TestMethod]
		public void TestString_Slice_OnlyEnd_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("Hel", script.Eval("str[:3]"));
		}

		[TestMethod]
		public void TestString_Slice_NegativeStart()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("lo", script.Eval("str[-2:]"));
		}

		[TestMethod]
		public void TestString_Slice_NegativeEnd()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("Hel", script.Eval("str[:-2]"));
		}

		[TestMethod]
		public void TestString_Slice_FullCopy()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("Hello", script.Eval("str[:]"));
		}

		[TestMethod]
		public void TestString_Slice_FullCopy_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("Hello", script.Eval("str[:]"));
		}

		#endregion

		#region 边界情况测试

		[TestMethod]
		public void TestArray_Slice_EmptyResult()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 0, 1, 2, 3, 4 });
			var result = (List<int>)script.Eval("arr[3:3]");
			CollectionAssert.AreEqual(new List<int>(), result);
		}

		[TestMethod]
		public void TestString_Slice_EmptyResult()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			var result = script.Eval("str[2:2]");
			Assert.AreEqual("", result);
		}

		[TestMethod]
		public void TestArray_Slice_CombinedWithIndex()
		{
			var script = new Script();
			script.Context.SetVar("arr", new[] { 10, 20, 30, 40, 50 });
			// 先切片再索引
			Assert.AreEqual(30, script.Eval("arr[1:4][1]"));
		}

		[TestMethod]
		public void TestString_Slice_CombinedWithIndex()
		{
			var script = new Script();
			script.Context.SetVar("str", "Hello");
			// 先切片再索引
			Assert.AreEqual('l', script.Eval("str[1:4][1]"));
		}

		#endregion
	}
}