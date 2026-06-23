using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptTupleTest
	{
		[TestMethod]
		public void Test07_2()
		{
			string s = @"
var (a) = ('1', 2, '3');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			//#if NET45
			//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
			//			var t = (Tuple<string, int, string>)r;
			//#else
			//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
			//			var t = (ValueTuple<string, int, string>)r;
			//#endif
			//			Assert.AreEqual("1", t.Item1);
			//			Assert.AreEqual(2, t.Item2);
			//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test07()
		{
			string s = @"
var (a) = ('1', 2, '3');
";
			var script = new Script();
			var r = script.Eval(s);
			//#if NET45
			//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
			//			var t = (Tuple<string, int, string>)r;
			//#else
			//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
			//			var t = (ValueTuple<string, int, string>)r;
			//#endif
			//			Assert.AreEqual("1", t.Item1);
			//			Assert.AreEqual(2, t.Item2);
			//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test06_2()
		{
			string s = @"
var (a, _, _) = ('1', 2, '3');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
//			var t = (Tuple<string, int, string>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
//			var t = (ValueTuple<string, int, string>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test06()
		{
			string s = @"
var (a, _, _) = ('1', 2, '3');
";
			var script = new Script();
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
//			var t = (Tuple<string, int, string>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
//			var t = (ValueTuple<string, int, string>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test05_2()
		{
			string s = @"
var (a, _, c) = ('1', 2, '3');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
//			var t = (Tuple<string, int, string>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
//			var t = (ValueTuple<string, int, string>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual("3", script.Context.EvalVar("c"));
		}

		[TestMethod]
		public void Test05()
		{
			string s = @"
var (a, _, c) = ('1', 2, '3');
";
			var script = new Script();
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
//			var t = (Tuple<string, int, string>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
//			var t = (ValueTuple<string, int, string>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual("3", script.Context.EvalVar("c"));
		}

		[TestMethod]
		public void Test04_2()
		{
			string s = @"
var (a, b) = ('1', 2, '3');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int>));
//			var t = (Tuple<string, int>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int>));
//			var t = (ValueTuple<string, int>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual(2, script.Context.EvalVar("b"));
		}

		[TestMethod]
		public void Test04()
		{
			string s = @"
var (a, b) = ('1', 2, '3');
";
			var script = new Script();
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int>));
//			var t = (Tuple<string, int>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int>));
//			var t = (ValueTuple<string, int>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual(2, script.Context.EvalVar("b"));
		}

		[TestMethod]
		public void Test03_2()
		{
			string s = @"
(a, b, c) = ('1', 2, '3');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
//			var t = (Tuple<string, int, string>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
//			var t = (ValueTuple<string, int, string>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual(2, script.Context.EvalVar("b"));
			Assert.AreEqual("3", script.Context.EvalVar("c"));
		}

		[TestMethod]
		public void Test03()
		{
			string s = @"
(a, b, c) = ('1', 2, '3');
";
			var script = new Script();
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
//			var t = (Tuple<string, int, string>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
//			var t = (ValueTuple<string, int, string>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual(2, script.Context.EvalVar("b"));
			Assert.AreEqual("3", script.Context.EvalVar("c"));
		}

		[TestMethod]
		public void Test02_2()
		{
			string s = @"
var (a, b, c) = ('1', 2, '3');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			//#if NET45
			//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
			//			var t = (Tuple<string, int, string>)r;
			//#else
			//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
			//			var t = (ValueTuple<string, int, string>)r;
			//#endif
			//			Assert.AreEqual("1", t.Item1);
			//			Assert.AreEqual(2, t.Item2);
			//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual(2, script.Context.EvalVar("b"));
			Assert.AreEqual("3", script.Context.EvalVar("c"));
		}

		[TestMethod]
		public void Test02()
		{
			string s = @"
var (a, b, c) = ('1', 2, '3');
";
			var script = new Script();
			var r = script.Eval(s);
//#if NET45
//			Assert.IsInstanceOfType(r, typeof(Tuple<string, int, string>));
//			var t = (Tuple<string, int, string>)r;
//#else
//			Assert.IsInstanceOfType(r, typeof(ValueTuple<string, int, string>));
//			var t = (ValueTuple<string, int, string>)r;
//#endif
//			Assert.AreEqual("1", t.Item1);
//			Assert.AreEqual(2, t.Item2);
//			Assert.AreEqual("3", t.Item3);
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual(2, script.Context.EvalVar("b"));
			Assert.AreEqual("3", script.Context.EvalVar("c"));
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = @"
var (a, b, c) = ('1', 2, '3');
a+c
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("13", script.Eval(s));
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual(2, script.Context.EvalVar("b"));
			Assert.AreEqual("3", script.Context.EvalVar("c"));
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
var (a, b, c) = ('1', 2, '3');
a+c
";
			var script = new Script();
			Assert.AreEqual("13", script.Eval(s));
			Assert.AreEqual("1", script.Context.EvalVar("a"));
			Assert.AreEqual(2, script.Context.EvalVar("b"));
			Assert.AreEqual("3", script.Context.EvalVar("c"));
		}

		[TestMethod]
		public void TestBasicTuple()
		{
			var script = new Script();
			var result = script.Eval("(1, 2)");
			Assert.IsNotNull(result);
#if NET45
			Assert.IsInstanceOfType(result, typeof(Tuple<int, int>));
			var tuple2 = (Tuple<int, int>)result;
#else
			Assert.IsInstanceOfType(result, typeof(ValueTuple<int, int>));
			var tuple2 = (ValueTuple<int, int>)result;
#endif
			Assert.AreEqual(1, tuple2.Item1);
			Assert.AreEqual(2, tuple2.Item2);
		}

		[TestMethod]
		public void TestBasicTuple_CompileModeAll()
		{
			var script = new Script();
			var result = script.Eval("(1, 2)", ECompileMode.All);
			Assert.IsNotNull(result);
#if NET45
			Assert.IsInstanceOfType(result, typeof(Tuple<int, int>));
			var tuple2 = (Tuple<int, int>)result;
#else
			Assert.IsInstanceOfType(result, typeof(ValueTuple<int, int>));
			var tuple2 = (ValueTuple<int, int>)result;
#endif
			Assert.AreEqual(1, tuple2.Item1);
			Assert.AreEqual(2, tuple2.Item2);
		}

		[TestMethod]
		public void TestTupleWithDifferentTypes()
		{
			var script = new Script();
			var result = script.Eval("(1, \"hello\", true)");
			Assert.IsNotNull(result);
#if NET45
			Assert.IsInstanceOfType(result, typeof(Tuple<int, string, bool>));
			var tuple3 = (Tuple<int, string, bool>)result;
#else
			Assert.IsInstanceOfType(result, typeof(ValueTuple<int, string, bool>));
			var tuple3 = (ValueTuple<int, string, bool>)result;
#endif
			Assert.AreEqual(1, tuple3.Item1);
			Assert.AreEqual("hello", tuple3.Item2);
			Assert.AreEqual(true, tuple3.Item3);
		}

		[TestMethod]
		public void TestTupleWithDifferentTypes_CompileModeAll()
		{
			var script = new Script();
			var result = script.Eval("(1, \"hello\", true)", ECompileMode.All);
			Assert.IsNotNull(result);
#if NET45
			Assert.IsInstanceOfType(result, typeof(Tuple<int, string, bool>));
			var tuple3 = (Tuple<int, string, bool>)result;
#else
			Assert.IsInstanceOfType(result, typeof(ValueTuple<int, string, bool>));
			var tuple3 = (ValueTuple<int, string, bool>)result;
#endif
			Assert.AreEqual(1, tuple3.Item1);
			Assert.AreEqual("hello", tuple3.Item2);
			Assert.AreEqual(true, tuple3.Item3);
		}

		[TestMethod]
		public void TestTupleWithVariables()
		{
			var script = new Script();
			var result = script.Eval("a=1; b=2; (a, b)");
			Assert.IsNotNull(result);
#if NET45
			var tuple = (Tuple<int, int>)result;
#else
			var tuple = (ValueTuple<int, int>)result;
#endif
			Assert.AreEqual(1, tuple.Item1);
			Assert.AreEqual(2, tuple.Item2);
		}

		[TestMethod]
		public void TestTupleWithVariables_CompileModeAll()
		{
			var script = new Script();
			var result = script.Eval("a=1; b=2; (a, b)", ECompileMode.All);
			Assert.IsNotNull(result);
#if NET45
			var tuple = (Tuple<int, int>)result;
#else
			var tuple = (ValueTuple<int, int>)result;
#endif
			Assert.AreEqual(1, tuple.Item1);
			Assert.AreEqual(2, tuple.Item2);
		}

		[TestMethod]
		public void TestNestedTuple()
		{
			var script = new Script();
			var result = script.Eval("(1, (2, 3))");
			Assert.IsNotNull(result);
#if NET45
			var tuple = (Tuple<int, Tuple<int, int>>)result;
#else
			var tuple = (ValueTuple<int, ValueTuple<int, int>>)result;
#endif
			Assert.AreEqual(1, tuple.Item1);
			Assert.AreEqual(2, tuple.Item2.Item1);
			Assert.AreEqual(3, tuple.Item2.Item2);
		}

		[TestMethod]
		public void TestNestedTuple_CompileModeAll()
		{
			var script = new Script();
			var result = script.Eval("(1, (2, 3))", ECompileMode.All);
			Assert.IsNotNull(result);
#if NET45
			var tuple = (Tuple<int, Tuple<int, int>>)result;
#else
			var tuple = (ValueTuple<int, ValueTuple<int, int>>)result;
#endif
			Assert.AreEqual(1, tuple.Item1);
			Assert.AreEqual(2, tuple.Item2.Item1);
			Assert.AreEqual(3, tuple.Item2.Item2);
		}

		[TestMethod]
		public void TestTupleInFunctionCall()
		{
			var script = new Script();
			script.Eval("int add(int a, int b) { return a + b }");
			var result = script.Eval("add((1, 2).Item1, (3, 4).Item2)");
			Assert.AreEqual(5, result);
		}

		[TestMethod]
		public void TestTupleInFunctionCall_CompileModeAll()
		{
			var script = new Script();
			script.Eval("int add(int a, int b) { return a + b }", ECompileMode.All);
			var result = script.Eval("add((1, 2).Item1, (3, 4).Item2)", ECompileMode.All);
			Assert.AreEqual(5, result);
		}

#if NET7_0_OR_GREATER
		[TestMethod]
		public void TestTupleNotEquality()
		{
			var script = new Script();
			var result = script.Eval("(1, 2) != (1, 2)");
			Assert.AreEqual((1, 2) != (1, 2), result);
			result = script.Eval("(1, 2) != (1, 3)");
			Assert.AreEqual((1, 2) != (1, 3), result);
		}

		[TestMethod]
		public void TestTupleNotEquality_CompileModeAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = script.Eval("(1, 2) != (1, 2)");
			Assert.AreEqual((1, 2) != (1, 2), result);
			result = script.Eval("(1, 2) != (1, 3)");
			Assert.AreEqual((1, 2) != (1, 3), result);
		}

		[TestMethod]
		public void TestTupleEquality()
		{
			var script = new Script();
			var result = script.Eval("(1, 2) == (1, 2)");
			Assert.AreEqual((1, 2) == (1, 2), result);
			result = script.Eval("(1, 2) == (1, 3)");
			Assert.AreEqual((1, 2) == (1, 3), result);
		}

		[TestMethod]
		public void TestTupleEquality_CompileModeAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = script.Eval("(1, 2) == (1, 2)");
			Assert.AreEqual((1, 2) == (1, 2), result);
			result = script.Eval("(1, 2) == (1, 3)");
			Assert.AreEqual((1, 2) == (1, 3), result);
		}
#endif

		[TestMethod]
		public void TestTupleInCollection()
		{
			var script = new Script();
			script.Eval("arr = [(1, 2), (3, 4)]");
			var result = script.Eval("arr[0].Item1");
			Assert.AreEqual(1, result);
			result = script.Eval("arr[1].Item2");
			Assert.AreEqual(4, result);
		}

		[TestMethod]
		public void TestTupleInCollection_CompileModeAll()
		{
			var script = new Script();
			script.Eval("arr = [(1, 2), (3, 4)]", ECompileMode.All);
			var result = script.Eval("arr[0].Item1", ECompileMode.All);
			Assert.AreEqual(1, result);
			result = script.Eval("arr[1].Item2", ECompileMode.All);
			Assert.AreEqual(4, result);
		}

		[TestMethod]
		public void TestSingleElementTuple()
		{
			var script = new Script();
			var result = script.Eval("(1)");
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void TestSingleElementTuple_CompileModeAll()
		{
			var script = new Script();
			var result = script.Eval("(1)", ECompileMode.All);
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void TestLargeTuple()
		{
			var script = new Script();
			var result = script.Eval("(1, 2, 3, 4, 5, 6, 7)");
			Assert.IsNotNull(result);
#if NET45
			Assert.IsTrue(result.GetType().Name.StartsWith("Tuple`"));
#else
			Assert.IsTrue(result.GetType().Name.StartsWith("ValueTuple`"));
#endif
		}

		[TestMethod]
		public void TestLargeTuple_CompileModeAll()
		{
			var script = new Script();
			var result = script.Eval("(1, 2, 3, 4, 5, 6, 7)", ECompileMode.All);
			Assert.IsNotNull(result);
#if NET45
			Assert.IsTrue(result.GetType().Name.StartsWith("Tuple`"));
#else
			Assert.IsTrue(result.GetType().Name.StartsWith("ValueTuple`"));
#endif
		}

		[TestMethod]
		public void TestTupleWithExpressions()
		{
			var script = new Script();
			var result = script.Eval("(1 + 2, 3 * 4)");
			Assert.IsNotNull(result);
#if NET45
			var tuple = (Tuple<int, int>)result;
#else
			var tuple = (ValueTuple<int, int>)result;
#endif
			Assert.AreEqual(3, tuple.Item1);
			Assert.AreEqual(12, tuple.Item2);
		}

		[TestMethod]
		public void TestTupleWithExpressions_CompileModeAll()
		{
			var script = new Script();
			var result = script.Eval("(1 + 2, 3 * 4)", ECompileMode.All);
			Assert.IsNotNull(result);
#if NET45
			var tuple = (Tuple<int, int>)result;
#else
			var tuple = (ValueTuple<int, int>)result;
#endif
			Assert.AreEqual(3, tuple.Item1);
			Assert.AreEqual(12, tuple.Item2);
		}

		[TestMethod]
		public void TestTupleWithFunctionCall()
		{
			var script = new Script();
			script.Eval("int getValue() { return 10 }");
			var result = script.Eval("(getValue(), getValue() + 5)");
#if NET45
			var tuple = (Tuple<int, int>)result;
#else
			var tuple = (ValueTuple<int, int>)result;
#endif
			Assert.AreEqual(10, tuple.Item1);
			Assert.AreEqual(15, tuple.Item2);
		}

		[TestMethod]
		public void TestTupleWithFunctionCall_CompileModeAll()
		{
			var script = new Script();
			script.Eval("int getValue() { return 10 }", ECompileMode.All);
			var result = script.Eval("(getValue(), getValue() + 5)", ECompileMode.All);
#if NET45
			var tuple = (Tuple<int, int>)result;
#else
			var tuple = (ValueTuple<int, int>)result;
#endif
			Assert.AreEqual(10, tuple.Item1);
			Assert.AreEqual(15, tuple.Item2);
		}

		[TestMethod]
		public void TestTupleWithNull()
		{
			var script = new Script();
			var result = script.Eval("(1, null, 3)");
			Assert.IsNotNull(result);
#if NET45
			var tuple = (Tuple<int, object, int>)result;
#else
			var tuple = (ValueTuple<int, object, int>)result;
#endif
			Assert.AreEqual(1, tuple.Item1);
			Assert.IsNull(tuple.Item2);
			Assert.AreEqual(3, tuple.Item3);
		}

		[TestMethod]
		public void TestTupleWithNull_CompileModeAll()
		{
			var script = new Script();
			var result = script.Eval("(1, null, 3)", ECompileMode.All);
			Assert.IsNotNull(result);
#if NET45
			var tuple = (Tuple<int, object, int>)result;
#else
			var tuple = (ValueTuple<int, object, int>)result;
#endif
			Assert.AreEqual(1, tuple.Item1);
			Assert.IsNull(tuple.Item2);
			Assert.AreEqual(3, tuple.Item3);
		}
	}
}