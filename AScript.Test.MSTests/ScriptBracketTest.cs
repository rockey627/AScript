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
		public void TestArray_Slice_PositiveIndex2()
		{
			var script = new Script();
			var result1 = (List<int>)script.Eval("var arr1 = [0,1,2,3,4]; arr1[1:4]");
			var result2 = (List<int>)script.Eval("var arr2 = [0,1,2,3,4]; arr2[-4:-1]");
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result1);
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result2);
			Assert.AreEqual(1, script.Eval("arr1[1]"));
			Assert.AreEqual(1, script.Eval("arr1[-4]"));
		}

		[TestMethod]
		public void TestArray_Slice_PositiveIndex2_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result1 = (List<int>)script.Eval("var arr1 = [0,1,2,3,4]; arr1[1:4]");
			var result2 = (List<int>)script.Eval("var arr2 = [0,1,2,3,4]; arr2[-4:-1]");
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result1);
			CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result2);
			Assert.AreEqual(1, script.Eval("arr1[1]"));
			Assert.AreEqual(1, script.Eval("arr1[-4]"));
		}

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
			Assert.AreEqual("ell", script.Eval("'hello'[-4:-1]"));
		}

		[TestMethod]
		public void TestString_Slice_NegativeIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("str", "Hello");
			Assert.AreEqual("ell", script.Eval("str[-4:-1]"));
			Assert.AreEqual("ell", script.Eval("'hello'[-4:-1]"));
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

		#region ArrayLiteral - 数组字面量测试

		[TestMethod]
		public void TestArrayLiteral_Int()
		{
			var script = new Script();
			var result = (Array)script.Eval("[1,2,3,4,5]");
			Assert.AreEqual(5, result.Length);
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(5, result.GetValue(4));
		}

		[TestMethod]
		public void TestArrayLiteral_Int_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = (Array)script.Eval("[1,2,3,4,5]");
			Assert.AreEqual(5, result.Length);
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(5, result.GetValue(4));
		}

		[TestMethod]
		public void TestArrayLiteral_String()
		{
			var script = new Script();
			var result = (Array)script.Eval("['a','b','c']");
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual("a", result.GetValue(0));
			Assert.AreEqual("c", result.GetValue(2));
		}

		[TestMethod]
		public void TestArrayLiteral_String_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = (Array)script.Eval("['a','b','c']");
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual("a", result.GetValue(0));
			Assert.AreEqual("c", result.GetValue(2));
		}

		[TestMethod]
		public void TestArrayLiteral_Mixed()
		{
			var script = new Script();
			var result = (Array)script.Eval("[1,'hello',3.14,true]");
			Assert.AreEqual(4, result.Length);
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual("hello", result.GetValue(1));
			Assert.AreEqual(3.14, result.GetValue(2));
			Assert.AreEqual(true, result.GetValue(3));
		}

		[TestMethod]
		public void TestArrayLiteral_Mixed_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = (Array)script.Eval("[1,'hello',3.14,true]");
			Assert.AreEqual(4, result.Length);
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual("hello", result.GetValue(1));
			Assert.AreEqual(3.14, result.GetValue(2));
			Assert.AreEqual(true, result.GetValue(3));
		}

		[TestMethod]
		public void TestArrayLiteral_Empty()
		{
			var script = new Script();
			var result = (Array)script.Eval("[]");
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void TestArrayLiteral_Empty_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = (Array)script.Eval("[]");
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void TestArrayLiteral_SingleElement()
		{
			var script = new Script();
			var result = (Array)script.Eval("[42]");
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual(42, result.GetValue(0));
		}

		[TestMethod]
		public void TestArrayLiteral_SingleElement_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = (Array)script.Eval("[42]");
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual(42, result.GetValue(0));
		}

		[TestMethod]
		public void TestArrayLiteral_AssignToVar()
		{
			var script = new Script();
			script.Eval("var arr = [10,20,30]");
			Assert.AreEqual(20, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void TestArrayLiteral_AssignToVar_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Eval("var arr = [10,20,30]");
			Assert.AreEqual(20, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void TestArrayLiteral_NestedIndex()
		{
			var script = new Script();
			var result = script.Eval("[[1,2],[3,4]][0]");
			var arr = (Array)result;
			Assert.AreEqual(1, arr.GetValue(0));
			Assert.AreEqual(2, arr.GetValue(1));
		}

		[TestMethod]
		public void TestArrayLiteral_NestedIndex_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = script.Eval("[[1,2],[3,4]][0]");
			var arr = (Array)result;
			Assert.AreEqual(1, arr.GetValue(0));
			Assert.AreEqual(2, arr.GetValue(1));
		}

		[TestMethod]
		public void TestArrayLiteral_WithVariable()
		{
			var script = new Script();
			script.Context.SetVar("x", 100);
			script.Context.SetVar("y", 200);
			var result = (Array)script.Eval("[x, y, x + y]");
			Assert.AreEqual(100, result.GetValue(0));
			Assert.AreEqual(200, result.GetValue(1));
			Assert.AreEqual(300, result.GetValue(2));
		}

		[TestMethod]
		public void TestArrayLiteral_WithVariable_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("x", 100);
			script.Context.SetVar("y", 200);
			var result = (Array)script.Eval("[x, y, x + y]");
			Assert.AreEqual(100, result.GetValue(0));
			Assert.AreEqual(200, result.GetValue(1));
			Assert.AreEqual(300, result.GetValue(2));
		}

		[TestMethod]
		public void TestArrayLiteral_Double()
		{
			var script = new Script();
			var result = (Array)script.Eval("[1.1, 2.2, 3.3]");
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(1.1, result.GetValue(0));
			Assert.AreEqual(3.3, result.GetValue(2));
		}

		[TestMethod]
		public void TestArrayLiteral_Double_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = (Array)script.Eval("[1.1, 2.2, 3.3]");
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(1.1, result.GetValue(0));
			Assert.AreEqual(3.3, result.GetValue(2));
		}

		[TestMethod]
		public void TestArrayLiteral_Bool()
		{
			var script = new Script();
			var result = (Array)script.Eval("[true, false, true]");
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(true, result.GetValue(0));
			Assert.AreEqual(false, result.GetValue(1));
			Assert.AreEqual(true, result.GetValue(2));
		}

		[TestMethod]
		public void TestArrayLiteral_Bool_Compiled()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = (Array)script.Eval("[true, false, true]");
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(true, result.GetValue(0));
			Assert.AreEqual(false, result.GetValue(1));
			Assert.AreEqual(true, result.GetValue(2));
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