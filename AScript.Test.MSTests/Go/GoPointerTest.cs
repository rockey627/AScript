using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoPointerTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["go"] = GoLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("go");
		}

		private static Script NewScript(bool compileAll = false)
		{
			var script = new Script();
			if (compileAll) script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			return script;
		}

		[TestMethod]
		public void Test01_AddressOf()
		{
			var s = @"
var a = 10
var p = &a
";
			var script = NewScript();
			script.Eval(s);
			Assert.IsNotNull(script.Eval("p"));
		}

		[TestMethod]
		public void Test01_AddressOf_CompileAll()
		{
			var s = @"
var a = 10
var p = &a
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.IsNotNull(script.Eval("p"));
		}

		[TestMethod]
		public void Test02_Deref()
		{
			var s = @"
var a = 10
var p = &a
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("*p"));
		}

		[TestMethod]
		public void Test02_Deref_CompileAll()
		{
			var s = @"
var a = 10
var p = &a
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("*p"));
		}

		[TestMethod]
		public void Test03_AssignThroughPointer()
		{
			var s = @"
var a = 10
var p = &a
*p = 20
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(20, script.Eval("a"));
		}

		[TestMethod]
		public void Test03_AssignThroughPointer_CompileAll()
		{
			var s = @"
var a = 10
var p = &a
*p = 20
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(20, script.Eval("a"));
		}

		[TestMethod]
		public void Test04_TypedPointerVar()
		{
			var s = @"
var a int = 10
var p *int = &a
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("*p"));
		}

		[TestMethod]
		public void Test04_TypedPointerVar_CompileAll()
		{
			var s = @"
var a int = 10
var p *int = &a
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("*p"));
		}

		[TestMethod]
		public void Test05_PointerToPointer()
		{
			var s = @"
var a = 10
var p = &a
var pp = &p
";
			var script = NewScript();
			script.Eval(s);
			Assert.IsNotNull(script.Eval("pp"));
			Assert.AreEqual(10, script.Eval("**pp"));
		}

		[TestMethod]
		public void Test05_PointerToPointer_CompileAll()
		{
			var s = @"
var a = 10
var p = &a
var pp = &p
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.IsNotNull(script.Eval("pp"));
			Assert.AreEqual(10, script.Eval("**pp"));
		}

		[TestMethod]
		public void Test06_ArrayPointerRead()
		{
			var s = @"
var b = [5]int{1, 2, 3, 4, 5}
var d = &b
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("d[0]"));
			Assert.AreEqual(5, script.Eval("d[4]"));
		}

		[TestMethod]
		public void Test06_ArrayPointerRead_CompileAll()
		{
			var s = @"
var b = [5]int{1, 2, 3, 4, 5}
var d = &b
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("d[0]"));
			Assert.AreEqual(5, script.Eval("d[4]"));
		}

		[TestMethod]
		public void Test07_ArrayPointerModify()
		{
			var s = @"
var b = [5]int{1, 2, 3, 4, 5}
var d = &b
d[0] = 10
d[1] = 20
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("b[0]"));
			Assert.AreEqual(20, script.Eval("b[1]"));
			Assert.AreEqual(3, script.Eval("b[2]"));
		}

		[TestMethod]
		public void Test07_ArrayPointerModify_CompileAll()
		{
			var s = @"
var b = [5]int{1, 2, 3, 4, 5}
var d = &b
d[0] = 10
d[1] = 20
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("b[0]"));
			Assert.AreEqual(20, script.Eval("b[1]"));
			Assert.AreEqual(3, script.Eval("b[2]"));
		}

		[TestMethod]
		public void Test08_ArrayPointerFuncParam()
		{
			var s = @"
func setarr(x *[5]int) {
	x[0] = 100
}
var arr = [5]int{1, 2, 3, 4, 5}
setarr(&arr)
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
		}

		[TestMethod]
		public void Test08_ArrayPointerFuncParam_CompileAll()
		{
			var s = @"
func setarr(x *[5]int) {
	x[0] = 100
}
var arr = [5]int{1, 2, 3, 4, 5}
setarr(&arr)
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
		}

		[TestMethod]
		public void Test09_ArrayPointerFuncParamModify()
		{
			var s = @"
func setarr(x *[5]int) {
	x[1] = 200
	x[2] = 300
}
var arr = [5]int{1, 2, 3, 4, 5}
setarr(&arr)
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(200, script.Eval("arr[1]"));
			Assert.AreEqual(300, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test09_ArrayPointerFuncParamModify_CompileAll()
		{
			var s = @"
func setarr(x *[5]int) {
	x[1] = 200
	x[2] = 300
}
var arr = [5]int{1, 2, 3, 4, 5}
setarr(&arr)
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(200, script.Eval("arr[1]"));
			Assert.AreEqual(300, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test10_ArrayCopyAndPointer()
		{
			var s = @"
var b = [5]int{1, 2, 3, 4, 5}
var c = b
c[0] = 10
var d = &b
d[1] = 20
";
			var script = NewScript();
			script.Eval(s);
			// 数组是值类型，赋值会拷贝整个数组
			Assert.AreEqual(1, script.Eval("b[0]"));
			// 通过数组指针修改的是原数组
			Assert.AreEqual(20, script.Eval("b[1]"));
			// 拷贝出来的数组不受影响
			Assert.AreEqual(10, script.Eval("c[0]"));
			Assert.AreEqual(2, script.Eval("c[1]"));
		}

		[TestMethod]
		public void Test10_ArrayCopyAndPointer_CompileAll()
		{
			var s = @"
var b = [5]int{1, 2, 3, 4, 5}
var c = b
c[0] = 10
var d = &b
d[1] = 20
";
			var script = NewScript(true);
			script.Eval(s);
			// 数组是值类型，赋值会拷贝整个数组
			Assert.AreEqual(1, script.Eval("b[0]"));
			// 通过数组指针修改的是原数组
			Assert.AreEqual(20, script.Eval("b[1]"));
			// 拷贝出来的数组不受影响
			Assert.AreEqual(10, script.Eval("c[0]"));
			Assert.AreEqual(2, script.Eval("c[1]"));
		}

		[TestMethod]
		public void Test11_SlicePointerModify()
		{
			var s = @"
var b = []int{1, 2, 3, 4, 5}
var d = &b
d[0] = 10
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("b[0]"));
			Assert.AreEqual(5, script.Eval("len(b)"));
		}

		[TestMethod]
		public void Test11_SlicePointerModify_CompileAll()
		{
			var s = @"
var b = []int{1, 2, 3, 4, 5}
var d = &b
d[0] = 10
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("b[0]"));
			Assert.AreEqual(5, script.Eval("len(b)"));
		}

		[TestMethod]
		public void Test12_ArrayPointerFuncParamRead()
		{
			var s = @"
func getarr(x *[5]int) int {
	return (*x)[0]
}
var arr = [5]int{7, 8, 9, 10, 11}
";
			var script = NewScript();
			script.Eval(s);
			Assert.AreEqual(7, script.Eval("getarr(&arr)"));
		}

		[TestMethod]
		public void Test12_ArrayPointerFuncParamRead_CompileAll()
		{
			var s = @"
func getarr(x *[5]int) int {
	return (*x)[0]
}
var arr = [5]int{7, 8, 9, 10, 11}
";
			var script = NewScript(true);
			script.Eval(s);
			Assert.AreEqual(7, script.Eval("getarr(&arr)"));
		}
	}
}
