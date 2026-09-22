using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoGoTest
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
		public void Test01_GoBasicFunction()
		{
			var s = @"
import(""time"")
var result = 0
func add() {
    result = 10
}
go add()
time.Sleep(100)
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(10, result);
		}

		[TestMethod]
		public void Test01_GoBasicFunction_CompileAll()
		{
			var s = @"
import(""time"")
var result = 0
func add() {
    result = 10
}
go add()
time.Sleep(100)
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(10, result);
		}

		[TestMethod]
		public void Test02_GoFunctionWithParams()
		{
			var s = @"
import(""time"")
var result = 0
func add(a int, b int) {
    result = a + b
}
go add(3, 5)
time.Sleep(1000)
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(8, result);
		}

		[TestMethod]
		public void Test02_GoFunctionWithParams_CompileAll()
		{
			var s = @"
import(""time"")
var result = 0
func add(a int, b int) {
    result = a + b
}
go add(3, 5)
time.Sleep(100)
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(8, result);
		}

		[TestMethod]
		public void Test03_GoMultipleGoroutines()
		{
			var s = @"
import('time' 'sync')
var mutex sync.Mutex
var counter = 0
func increment() {
	mutex.Lock()
    counter = counter + 1
	mutex.Unlock()
}
go increment()
go increment()
go increment()
time.Sleep(1000)
counter
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void Test03_GoMultipleGoroutines_CompileAll()
		{
			var s = @"
import('time' 'sync')
var mutex sync.Mutex
var counter = 0
func increment() {
	mutex.Lock()
    counter = counter + 1
	mutex.Unlock()
}
go increment()
go increment()
go increment()
time.Sleep(100)
counter
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void Test04_GoAnonymousFunction()
		{
			var s = @"
import(""time"")
var result = 0
go func() {
    result = 42
}()
time.Sleep(100)
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void Test04_GoAnonymousFunction_CompileAll()
		{
			var s = @"
import(""time"")
var result = 0
go func() {
    result = 42
}()
time.Sleep(100)
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void Test05_GoAnonymousFunctionWithParams()
		{
			var s = @"
import(""time"")
var result = 0
go func(a int, b int) {
    result = a * b
}(6, 7)
time.Sleep(100)
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void Test05_GoAnonymousFunctionWithParams_CompileAll()
		{
			var s = @"
import(""time"")
var result = 0
go func(a int, b int) {
    result = a * b
}(6, 7)
time.Sleep(100)
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void Test06_GoFunctionReturningValue()
		{
			var s = @"
import(""time"")
var result = 0
func getValue() int {
    return 99
}
go func() {
    result = getValue()
}()
time.Sleep(100)
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(99, result);
		}

		[TestMethod]
		public void Test06_GoFunctionReturningValue_CompileAll()
		{
			var s = @"
import(""time"")
var result = 0
func getValue() int {
    return 99
}
go func() {
    result = getValue()
}()
time.Sleep(100)
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(99, result);
		}

		[TestMethod]
		public void Test07_GoWithMap()
		{
			var s = @"
import(""time"")
var m = make(map[string]int)
go func() {
    m[""key""] = 123
}()
time.Sleep(100)
m[""key""]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(123, result);
		}

		[TestMethod]
		public void Test07_GoWithMap_CompileAll()
		{
			var s = @"
import(""time"")
var m = make(map[string]int)
go func() {
    m[""key""] = 123
}()
time.Sleep(100)
m[""key""]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(123, result);
		}

		[TestMethod]
		public void Test08_GoWithSlice()
		{
			var s = @"
import(""time"")
var arr = []int{1, 2, 3}
go func() {
    arr[0] = 100
}()
time.Sleep(100)
arr[0]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(100, result);
		}

		[TestMethod]
		public void Test08_GoWithSlice_CompileAll()
		{
			var s = @"
import(""time"")
var arr = []int{1, 2, 3}
go func() {
    arr[0] = 100
}()
time.Sleep(100)
arr[0]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(100, result);
		}

		[TestMethod]
		public void Test09_GoWithLoop()
		{
			var s = @"
import(""time"")
var sum = 0
func addNumbers() {
    for i := 1; i <= 5; i++ {
        sum += i
    }
}
go addNumbers()
time.Sleep(100)
sum
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(15, result);
		}

		[TestMethod]
		public void Test09_GoWithLoop_CompileAll()
		{
			var s = @"
import(""time"")
var sum = 0
func addNumbers() {
    for i := 1; i <= 5; i++ {
        sum += i
    }
}
go addNumbers()
time.Sleep(100)
sum
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(15, result);
		}

		[TestMethod]
		public void Test10_GoNestedFunction()
		{
			var s = @"
import(""time"")
var result = 0
func outer() {
    inner := func() int {
        return 50
    }
    go func() {
        result = inner()
    }()
}
outer()
time.Sleep(100)
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(50, result);
		}

		[TestMethod]
		public void Test10_GoNestedFunction_CompileAll()
		{
			var s = @"
import(""time"")
var result = 0
func outer() {
    inner := func() int {
        return 50
    }
    go func() {
        result = inner()
    }()
}
outer()
time.Sleep(100)
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual(50, result);
		}
	}
}