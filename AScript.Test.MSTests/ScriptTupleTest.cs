using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptTupleTest
	{
		[TestMethod]
		public void TestBasicTuple()
		{
			var script = new Script();
			// 基本二元组
			var result = script.Eval("(1, 2)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(ValueTuple<int, int>));
			var tuple2 = (ValueTuple<int, int>)result;
			Assert.AreEqual(1, tuple2.Item1);
			Assert.AreEqual(2, tuple2.Item2);
		}

		[TestMethod]
		public void TestTupleWithDifferentTypes()
		{
			var script = new Script();
			// 不同类型元素
			var result = script.Eval("(1, \"hello\", true)");
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(ValueTuple<int, string, bool>));
			var tuple3 = (ValueTuple<int, string, bool>)result;
			Assert.AreEqual(1, tuple3.Item1);
			Assert.AreEqual("hello", tuple3.Item2);
			Assert.AreEqual(true, tuple3.Item3);
		}

		[TestMethod]
		public void TestTupleWithVariables()
		{
			var script = new Script();
			// 使用变量
			var result = script.Eval("a=1; b=2; (a, b)");
			Assert.IsNotNull(result);
			var tuple = (ValueTuple<int, int>)result;
			Assert.AreEqual(1, tuple.Item1);
			Assert.AreEqual(2, tuple.Item2);
		}

		[TestMethod]
		public void TestNestedTuple()
		{
			var script = new Script();
			// 嵌套元组
			var result = script.Eval("(1, (2, 3))");
			Assert.IsNotNull(result);
			var tuple = (ValueTuple<int, ValueTuple<int, int>>)result;
			Assert.AreEqual(1, tuple.Item1);
			Assert.AreEqual(2, tuple.Item2.Item1);
			Assert.AreEqual(3, tuple.Item2.Item2);
		}

		[TestMethod]
		public void TestTupleInFunctionCall()
		{
			var script = new Script();
			// 元组作为函数参数
			script.Eval("int add(int a, int b) { return a + b }");
			var result = script.Eval("add((1, 2).Item1, (3, 4).Item2)");
			Assert.AreEqual(5, result);
		}

		[TestMethod]
		public void TestTupleEquality()
		{
			Assert.IsFalse(Tuple.Create(1, 2) == (Tuple.Create(1, 2)));
			Assert.IsTrue(Tuple.Create(1, 2).Equals(Tuple.Create(1, 2)));
			var script = new Script();
			// 元组相等比较
			var result = script.Eval("(1, 2) == (1, 2)");
			Assert.AreEqual((1, 2) == (1, 2), result);
			result = script.Eval("(1, 2) == (1, 3)");
			Assert.AreEqual((1, 2) == (1, 3), result);
		}

		[TestMethod]
		public void TestTupleInCollection()
		{
			var script = new Script();
			// 元组在集合中使用
			script.Eval("arr = [(1, 2), (3, 4)]");
			var result = script.Eval("arr[0].Item1");
			Assert.AreEqual(1, result);
			result = script.Eval("arr[1].Item2");
			Assert.AreEqual(4, result);
		}

		//[TestMethod]
		//public void TestTupleForeachUnpack()
		//{
		//	var script = new Script();
		//	// foreach 解构元组
		//	script.Eval("arr = [(1, 2), (3, 4)]");
		//	script.Eval("sum = 0");
		//	script.Eval("foreach((a, b) in arr) { sum = sum + a + b }");
		//	var result = script.Eval("sum");
		//	Assert.AreEqual(10, result); // 1+2+3+4
		//}

		[TestMethod]
		public void TestSingleElementTuple()
		{
			var script = new Script();
			// 单元素元组 (实际上是表达式，不是元组)
			var result = script.Eval("(1)");
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void TestLargeTuple()
		{
			var script = new Script();
			// 七元组
			var result = script.Eval("(1, 2, 3, 4, 5, 6, 7)");
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetType().Name.Contains("ValueTuple"));
		}

		[TestMethod]
		public void TestTupleWithExpressions()
		{
			var script = new Script();
			// 表达式作为元组元素
			var result = script.Eval("(1 + 2, 3 * 4)");
			Assert.IsNotNull(result);
			var tuple = (ValueTuple<int, int>)result;
			Assert.AreEqual(3, tuple.Item1);
			Assert.AreEqual(12, tuple.Item2);
		}

		[TestMethod]
		public void TestTupleWithFunctionCall()
		{
			var script = new Script();
			// 函数调用结果作为元组元素
			script.Eval("int getValue() { return 10 }");
			var result = script.Eval("(getValue(), getValue() + 5)");
			var tuple = (ValueTuple<int, int>)result;
			Assert.AreEqual(10, tuple.Item1);
			Assert.AreEqual(15, tuple.Item2);
		}

		[TestMethod]
		public void TestTupleCompileModeAll()
		{
			var script = new Script();
			// ECompileMode.All 模式下测试
			var result = script.Eval("(1, 2)", ECompileMode.All);
			Assert.IsNotNull(result);
			var tuple = (ValueTuple<int, int>)result;
			Assert.AreEqual(1, tuple.Item1);
			Assert.AreEqual(2, tuple.Item2);
		}

		[TestMethod]
		public void TestTupleWithNull()
		{
			var script = new Script();
			// 包含null的元组
			var result = script.Eval("(1, null, 3)");
			Assert.IsNotNull(result);
			var tuple = (ValueTuple<int, object, int>)result;
			Assert.AreEqual(1, tuple.Item1);
			Assert.IsNull(tuple.Item2);
			Assert.AreEqual(3, tuple.Item3);
		}
	}
}