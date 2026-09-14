using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoArrayTest
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

		[TestMethod]
		public void Test01_ArrayDeclarationWithSize()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(5, script.Eval("arr[4]"));
		}

		[TestMethod]
		public void Test01_ArrayDeclarationWithSize_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(5, script.Eval("arr[4]"));
		}

		[TestMethod]
		public void Test02_ArrayDeclarationWithoutSize()
		{
			var s = @"
var arr = []int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test02_ArrayDeclarationWithoutSize_CompileAll()
		{
			var s = @"
var arr = []int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test03_ArrayShortDeclaration()
		{
			var s = @"
arr := [3]int{10, 20, 30}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test03_ArrayShortDeclaration_CompileAll()
		{
			var s = @"
arr := [3]int{10, 20, 30}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test04_ArrayIndexAccess()
		{
			var s = @"
var arr = [5]int{10, 20, 30, 40, 50}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(20, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
			Assert.AreEqual(40, script.Eval("arr[3]"));
			Assert.AreEqual(50, script.Eval("arr[4]"));
		}

		[TestMethod]
		public void Test04_ArrayIndexAccess_CompileAll()
		{
			var s = @"
var arr = [5]int{10, 20, 30, 40, 50}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(20, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
			Assert.AreEqual(40, script.Eval("arr[3]"));
			Assert.AreEqual(50, script.Eval("arr[4]"));
		}

		[TestMethod]
		public void Test05_ArrayIndexAssignment()
		{
			var s = @"
var arr = [3]int{1, 2, 3}
arr[0] = 100
arr[2] = 300
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(300, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test05_ArrayIndexAssignment_CompileAll()
		{
			var s = @"
var arr = [3]int{1, 2, 3}
arr[0] = 100
arr[2] = 300
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(300, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test06_ArrayLen()
		{
			var s = @"
var arr = [10]int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(3, script.Eval("arr[2]"));
			Assert.AreEqual(4, script.Eval("arr[3]"));
			Assert.AreEqual(5, script.Eval("arr[4]"));
			Assert.AreEqual(0, script.Eval("arr[5]"));
			Assert.AreEqual(0, script.Eval("arr[6]"));
			Assert.AreEqual(0, script.Eval("arr[7]"));
			Assert.AreEqual(0, script.Eval("arr[8]"));
			Assert.AreEqual(0, script.Eval("arr[9]"));
		}

		[TestMethod]
		public void Test06_ArrayLen_CompileAll()
		{
			var s = @"
var arr = [10]int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(3, script.Eval("arr[2]"));
			Assert.AreEqual(4, script.Eval("arr[3]"));
			Assert.AreEqual(5, script.Eval("arr[4]"));
			Assert.AreEqual(0, script.Eval("arr[5]"));
			Assert.AreEqual(0, script.Eval("arr[6]"));
			Assert.AreEqual(0, script.Eval("arr[7]"));
			Assert.AreEqual(0, script.Eval("arr[8]"));
			Assert.AreEqual(0, script.Eval("arr[9]"));
		}

		[TestMethod]
		public void Test06_ArrayLen2()
		{
			var s = @"
var arrLength = 10
var arr = [arrLength]int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(3, script.Eval("arr[2]"));
			Assert.AreEqual(4, script.Eval("arr[3]"));
			Assert.AreEqual(5, script.Eval("arr[4]"));
			Assert.AreEqual(0, script.Eval("arr[5]"));
			Assert.AreEqual(0, script.Eval("arr[6]"));
			Assert.AreEqual(0, script.Eval("arr[7]"));
			Assert.AreEqual(0, script.Eval("arr[8]"));
			Assert.AreEqual(0, script.Eval("arr[9]"));
		}

		[TestMethod]
		public void Test06_ArrayLen2_CompileAll()
		{
			var s = @"
var arrLength = 10
var arr = [arrLength]int{1, 2, 3, 4, 5}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(3, script.Eval("arr[2]"));
			Assert.AreEqual(4, script.Eval("arr[3]"));
			Assert.AreEqual(5, script.Eval("arr[4]"));
			Assert.AreEqual(0, script.Eval("arr[5]"));
			Assert.AreEqual(0, script.Eval("arr[6]"));
			Assert.AreEqual(0, script.Eval("arr[7]"));
			Assert.AreEqual(0, script.Eval("arr[8]"));
			Assert.AreEqual(0, script.Eval("arr[9]"));
		}

		[TestMethod]
		public void Test07_ArrayForRange()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var sum = 0
for i := 0; i < len(arr); i++ {
    sum += arr[i]
}
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(15, script.Eval(s));
		}

		[TestMethod]
		public void Test07_ArrayForRange_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var sum = 0
for i := 0; i < len(arr); i++ {
    sum += arr[i]
}
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(15, script.Eval(s));
		}

		[TestMethod]
		public void Test08_ArraySlicing()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[1:3]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(2, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test08_ArraySlicing_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[1:3]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(2, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test09_ArraySliceFromStart()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[:3]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test09_ArraySliceFromStart_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[:3]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test10_ArraySliceToEnd()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[2:]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test10_ArraySliceToEnd_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[2:]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test11_ArraySliceFull()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[:]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test11_ArraySliceFull_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[:]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test12_Array2DDeclaration()
		{
			var s = @"
var arr = [2][3]int{{1, 2, 3}, {4, 5, 6}}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(2, script.Eval("len(arr)"));
			Assert.AreEqual(3, script.Eval("len(arr[0])"));
			Assert.AreEqual(1, script.Eval("arr[0][0]"));
			Assert.AreEqual(6, script.Eval("arr[1][2]"));
		}

		[TestMethod]
		public void Test12_Array2DDeclaration_CompileAll()
		{
			var s = @"
var arr = [2][3]int{{1, 2, 3}, {4, 5, 6}}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(2, script.Eval("len(arr)"));
			Assert.AreEqual(3, script.Eval("len(arr[0])"));
			Assert.AreEqual(1, script.Eval("arr[0][0]"));
			Assert.AreEqual(6, script.Eval("arr[1][2]"));
		}

		[TestMethod]
		public void Test13_Array2DIndexAccess()
		{
			var s = @"
var arr = [2][2]int{{10, 20}, {30, 40}}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0][0]"));
			Assert.AreEqual(20, script.Eval("arr[0][1]"));
			Assert.AreEqual(30, script.Eval("arr[1][0]"));
			Assert.AreEqual(40, script.Eval("arr[1][1]"));
		}

		[TestMethod]
		public void Test13_Array2DIndexAccess_CompileAll()
		{
			var s = @"
var arr = [2][2]int{{10, 20}, {30, 40}}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0][0]"));
			Assert.AreEqual(20, script.Eval("arr[0][1]"));
			Assert.AreEqual(30, script.Eval("arr[1][0]"));
			Assert.AreEqual(40, script.Eval("arr[1][1]"));
		}

		[TestMethod]
		public void Test14_Array2DIndexAssignment()
		{
			var s = @"
var arr = [2][2]int{{10, 20}, {30, 40}}
arr[0][0] = 100
arr[1][1] = 400
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("arr[0][0]"));
			Assert.AreEqual(20, script.Eval("arr[0][1]"));
			Assert.AreEqual(30, script.Eval("arr[1][0]"));
			Assert.AreEqual(400, script.Eval("arr[1][1]"));
		}

		[TestMethod]
		public void Test14_Array2DIndexAssignment_CompileAll()
		{
			var s = @"
var arr = [2][2]int{{10, 20}, {30, 40}}
arr[0][0] = 100
arr[1][1] = 400
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(100, script.Eval("arr[0][0]"));
			Assert.AreEqual(20, script.Eval("arr[0][1]"));
			Assert.AreEqual(30, script.Eval("arr[1][0]"));
			Assert.AreEqual(400, script.Eval("arr[1][1]"));
		}

		[TestMethod]
		public void Test15_ArrayAppend()
		{
			var s = @"
var arr = []int{1, 2, 3}
var arr2 = append(arr, 4, 5)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(3, script.Eval("arr[2]"));
			Assert.AreEqual(5, script.Eval("len(arr2)"));
			Assert.AreEqual(1, script.Eval("arr2[0]"));
			Assert.AreEqual(2, script.Eval("arr2[1]"));
			Assert.AreEqual(3, script.Eval("arr2[2]"));
			Assert.AreEqual(4, script.Eval("arr2[3]"));
			Assert.AreEqual(5, script.Eval("arr2[4]"));
		}

		[TestMethod]
		public void Test15_ArrayAppend_CompileAll()
		{
			var s = @"
var arr = []int{1, 2, 3}
var arr2 = append(arr, 4, 5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(3, script.Eval("arr[2]"));
			Assert.AreEqual(5, script.Eval("len(arr2)"));
			Assert.AreEqual(1, script.Eval("arr2[0]"));
			Assert.AreEqual(2, script.Eval("arr2[1]"));
			Assert.AreEqual(3, script.Eval("arr2[2]"));
			Assert.AreEqual(4, script.Eval("arr2[3]"));
			Assert.AreEqual(5, script.Eval("arr2[4]"));
		}

		[TestMethod]
		public void Test15_ArrayAppend4()
		{
			var s = @"
var arr = []int{1, 2, 3}
var arr2 = append(arr, 4, 5, 6, 7)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(3, script.Eval("arr[2]"));
			Assert.AreEqual(7, script.Eval("len(arr2)"));
			Assert.AreEqual(1, script.Eval("arr2[0]"));
			Assert.AreEqual(2, script.Eval("arr2[1]"));
			Assert.AreEqual(3, script.Eval("arr2[2]"));
			Assert.AreEqual(4, script.Eval("arr2[3]"));
			Assert.AreEqual(5, script.Eval("arr2[4]"));
			Assert.AreEqual(6, script.Eval("arr2[5]"));
			Assert.AreEqual(7, script.Eval("arr2[6]"));
		}

		[TestMethod]
		public void Test15_ArrayAppend4_CompileAll()
		{
			var s = @"
var arr = []int{1, 2, 3}
var arr2 = append(arr, 4, 5, 6, 7)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(3, script.Eval("arr[2]"));
			Assert.AreEqual(7, script.Eval("len(arr2)"));
			Assert.AreEqual(1, script.Eval("arr2[0]"));
			Assert.AreEqual(2, script.Eval("arr2[1]"));
			Assert.AreEqual(3, script.Eval("arr2[2]"));
			Assert.AreEqual(4, script.Eval("arr2[3]"));
			Assert.AreEqual(5, script.Eval("arr2[4]"));
			Assert.AreEqual(6, script.Eval("arr2[5]"));
			Assert.AreEqual(7, script.Eval("arr2[6]"));
		}

		[TestMethod]
		public void Test16_ArrayAppendSingle()
		{
			var s = @"
var arr = []int{1, 2, 3}
arr = append(arr, 4)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(4, script.Eval("len(arr)"));
			Assert.AreEqual(4, script.Eval("arr[3]"));
		}

		[TestMethod]
		public void Test16_ArrayAppendSingle_CompileAll()
		{
			var s = @"
var arr = []int{1, 2, 3}
arr = append(arr, 4)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(4, script.Eval("len(arr)"));
			Assert.AreEqual(4, script.Eval("arr[3]"));
		}

		[TestMethod]
		public void Test17_ArrayAppendSlice()
		{
			var s = @"
var arr1 = []int{1, 2, 3}
var arr2 = []int{4, 5, 6}
arr1 = append(arr1, arr2...)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(6, script.Eval("len(arr1)"));
		}

		[TestMethod]
		public void Test17_ArrayAppendSlice_CompileAll()
		{
			var s = @"
var arr1 = []int{1, 2, 3}
var arr2 = []int{4, 5, 6}
arr1 = append(arr1, arr2...)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(6, script.Eval("len(arr1)"));
		}

		[TestMethod]
		public void Test18_ArrayCopy()
		{
			var s = @"
var src = []int{1, 2, 3, 4, 5}
var dst = make([]int, 3)
copy(dst, src)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("dst[0]"));
			Assert.AreEqual(2, script.Eval("dst[1]"));
			Assert.AreEqual(3, script.Eval("dst[2]"));
		}

		[TestMethod]
		public void Test18_ArrayCopy_CompileAll()
		{
			var s = @"
var src = []int{1, 2, 3, 4, 5}
var dst = make([]int, 3)
copy(dst, src)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1, script.Eval("dst[0]"));
			Assert.AreEqual(2, script.Eval("dst[1]"));
			Assert.AreEqual(3, script.Eval("dst[2]"));
		}

		[TestMethod]
		public void Test19_ArrayAsFunctionParameter()
		{
			var s = @"
func sum(arr []int) int {
    var total = 0
    for i := 0; i < len(arr); i++ {
        total += arr[i]
    }
    return total
}
var arr = []int{1, 2, 3, 4, 5}
sum(arr)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(15, script.Eval(s));
		}

		[TestMethod]
		public void Test19_ArrayAsFunctionParameter_CompileAll()
		{
			var s = @"
func sum(arr []int) int {
    var total = 0
    for i := 0; i < len(arr); i++ {
        total += arr[i]
    }
    return total
}
var arr = []int{1, 2, 3, 4, 5}
sum(arr)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(15, script.Eval(s));
		}

		[TestMethod]
		public void Test20_ArrayReturnFromFunction()
		{
			var s = @"
func createArray() []int {
    return []int{1, 2, 3}
}
var arr = createArray()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
		}

		[TestMethod]
		public void Test20_ArrayReturnFromFunction_CompileAll()
		{
			var s = @"
func createArray() []int {
    return []int{1, 2, 3}
}
var arr = createArray()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
		}

		[TestMethod]
		public void Test21_ArrayElementUpdate()
		{
			var s = @"
var arr = [3]int{10, 20, 30}
arr[1] = arr[1] + 5
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(25, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test21_ArrayElementUpdate_CompileAll()
		{
			var s = @"
var arr = [3]int{10, 20, 30}
arr[1] = arr[1] + 5
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(25, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test22_ArrayCompositeLiteral()
		{
			var s = @"
var arr = [4]int{
    1,
    2,
    3,
    4,
}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(4, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(4, script.Eval("arr[3]"));
		}

		[TestMethod]
		public void Test22_ArrayCompositeLiteral_CompileAll()
		{
			var s = @"
var arr = [4]int{
    1,
    2,
    3,
    4,
}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(4, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(4, script.Eval("arr[3]"));
		}

		[TestMethod]
		public void Test23_ArrayStringElement()
		{
			var s = @"
var arr = [3]string{""hello"", ""world"", ""go""}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("hello", script.Eval("arr[0]"));
			Assert.AreEqual("world", script.Eval("arr[1]"));
			Assert.AreEqual("go", script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test23_ArrayStringElement_CompileAll()
		{
			var s = @"
var arr = [3]string{""hello"", ""world"", ""go""}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("hello", script.Eval("arr[0]"));
			Assert.AreEqual("world", script.Eval("arr[1]"));
			Assert.AreEqual("go", script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test24_ArrayPartialInit()
		{
			var s = @"
var arr = [5]int{1, 2}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(0, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test24_ArrayPartialInit_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(arr)"));
			Assert.AreEqual(1, script.Eval("arr[0]"));
			Assert.AreEqual(2, script.Eval("arr[1]"));
			Assert.AreEqual(0, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test25_ArrayIndexExpression()
		{
			var s = @"
var arr = [3]int{10, 20, 30}
var idx = 1
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(20, script.Eval("arr[idx]"));
		}

		[TestMethod]
		public void Test25_ArrayIndexExpression_CompileAll()
		{
			var s = @"
var arr = [3]int{10, 20, 30}
var idx = 1
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(20, script.Eval("arr[idx]"));
		}

		[TestMethod]
		public void Test26_ArrayNestedSlice()
		{
			var s = @"
var arr = [][]int{{1, 2}, {3, 4, 5}, {6}}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(2, script.Eval("len(arr[0])"));
			Assert.AreEqual(3, script.Eval("len(arr[1])"));
			Assert.AreEqual(1, script.Eval("len(arr[2])"));
		}

		[TestMethod]
		public void Test26_ArrayNestedSlice_CompileAll()
		{
			var s = @"
var arr = [][]int{{1, 2}, {3, 4, 5}, {6}}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(arr)"));
			Assert.AreEqual(2, script.Eval("len(arr[0])"));
			Assert.AreEqual(3, script.Eval("len(arr[1])"));
			Assert.AreEqual(1, script.Eval("len(arr[2])"));
		}

		[TestMethod]
		public void Test27_ArrayEmptySlice()
		{
			var s = @"
var arr = []int{}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test27_ArrayEmptySlice_CompileAll()
		{
			var s = @"
var arr = []int{}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test28_ArrayEmptyWithSize()
		{
			var s = @"
var arr = [0]int{}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test28_ArrayEmptyWithSize_CompileAll()
		{
			var s = @"
var arr = [0]int{}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(0, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test29_ArrayIterationSum()
		{
			var s = @"
var arr = [4]int{10, 20, 30, 40}
var sum = 0
for i := 0; i < len(arr); i++ {
    sum += arr[i]
}
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(100, script.Eval(s));
		}

		[TestMethod]
		public void Test29_ArrayIterationSum_CompileAll()
		{
			var s = @"
var arr = [4]int{10, 20, 30, 40}
var sum = 0
for i := 0; i < len(arr); i++ {
    sum += arr[i]
}
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(100, script.Eval(s));
		}

		[TestMethod]
		public void Test30_ArrayMake()
		{
			var s = @"
var arr = make([]int, 5)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test30_ArrayMake_CompileAll()
		{
			var s = @"
var arr = make([]int, 5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(5, script.Eval("len(arr)"));
		}

		[TestMethod]
		public void Test31_ArrayMakeWithValues()
		{
			var s = @"
var arr = make([]int, 3)
arr[0] = 10
arr[1] = 20
arr[2] = 30
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(20, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test31_ArrayMakeWithValues_CompileAll()
		{
			var s = @"
var arr = make([]int, 3)
arr[0] = 10
arr[1] = 20
arr[2] = 30
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(10, script.Eval("arr[0]"));
			Assert.AreEqual(20, script.Eval("arr[1]"));
			Assert.AreEqual(30, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test32_ArrayBooleanValues()
		{
			var s = @"
var arr = [3]bool{true, false, true}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(true, script.Eval("arr[0]"));
			Assert.AreEqual(false, script.Eval("arr[1]"));
			Assert.AreEqual(true, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test32_ArrayBooleanValues_CompileAll()
		{
			var s = @"
var arr = [3]bool{true, false, true}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(true, script.Eval("arr[0]"));
			Assert.AreEqual(false, script.Eval("arr[1]"));
			Assert.AreEqual(true, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test33_ArrayFloatValues()
		{
			var s = @"
var arr = [3]float64{1.5, 2.5, 3.5}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1.5, script.Eval("arr[0]"));
			Assert.AreEqual(2.5, script.Eval("arr[1]"));
			Assert.AreEqual(3.5, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test33_ArrayFloatValues_CompileAll()
		{
			var s = @"
var arr = [3]float64{1.5, 2.5, 3.5}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(1.5, script.Eval("arr[0]"));
			Assert.AreEqual(2.5, script.Eval("arr[1]"));
			Assert.AreEqual(3.5, script.Eval("arr[2]"));
		}

		[TestMethod]
		public void Test34_ArrayForRangeWithIndex()
		{
			var s = @"
var arr = [3]int{10, 20, 30}
var sum = 0
for i := 0; i < len(arr); i++ {
    sum += arr[i]
}
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(60, script.Eval(s));
		}

		[TestMethod]
		public void Test34_ArrayForRangeWithIndex_CompileAll()
		{
			var s = @"
var arr = [3]int{10, 20, 30}
var sum = 0
for i := 0; i < len(arr); i++ {
    sum += arr[i]
}
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(60, script.Eval(s));
		}

		[TestMethod]
		public void Test35_ArrayMultipleAssignment()
		{
			var s = @"
var arr = [2]int{1, 2}
arr[0], arr[1] = arr[1], arr[0]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(2, script.Eval("arr[0]"));
			Assert.AreEqual(1, script.Eval("arr[1]"));
		}

		[TestMethod]
		public void Test35_ArrayMultipleAssignment_CompileAll()
		{
			var s = @"
var arr = [2]int{1, 2}
arr[0], arr[1] = arr[1], arr[0]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(2, script.Eval("arr[0]"));
			Assert.AreEqual(1, script.Eval("arr[1]"));
		}

		[TestMethod]
		public void Test36_ArraySliceModifyOriginal()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[1:4]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test36_ArraySliceModifyOriginal_CompileAll()
		{
			var s = @"
var arr = [5]int{1, 2, 3, 4, 5}
var s1 = arr[1:4]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(s1)"));
		}

		[TestMethod]
		public void Test37_ArrayNestedInMap()
		{
			var s = @"
var m = make(map[string][]int)
m[""key""] = []int{1, 2, 3}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(m[\"key\"])"));
		}

		[TestMethod]
		public void Test37_ArrayNestedInMap_CompileAll()
		{
			var s = @"
var m = make(map[string][]int)
m[""key""] = []int{1, 2, 3}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(3, script.Eval("len(m[\"key\"])"));
		}
	}
}
