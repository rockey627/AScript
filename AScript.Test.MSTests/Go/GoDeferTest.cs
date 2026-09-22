using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoDeferTest
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
		public void Test01_DeferBasic()
		{
			var s = @"
var order = """"
func foo() {
    order += ""A""
}
func bar() {
    order += ""B""
}
foo()
defer bar()
order += 'C'
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("AC", script.Eval(s));
			Assert.AreEqual("ACB", script.Eval("order"));
		}

		[TestMethod]
		public void Test01_DeferBasic_CompileAll()
		{
			var s = @"
var order = """"
func foo() {
    order += ""A""
}
func bar() {
    order += ""B""
}
foo()
defer bar()
order += 'C'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("AC", script.Eval(s));
			Assert.AreEqual("ACB", script.Eval("order"));
		}

		[TestMethod]
		public void Test02_DeferOrderLIFO()
		{
			var s = @"
var order = """"
defer func() {
    order += ""1""
}()
defer func() {
    order += ""2""
}()
defer func() {
    order += ""3""
}()
order
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("", script.Eval(s));
			Assert.AreEqual("321", script.Eval("order"));
		}

		[TestMethod]
		public void Test02_DeferOrderLIFO_CompileAll()
		{
			var s = @"
var order = """"
defer func() {
    order += ""1""
}()
defer func() {
    order += ""2""
}()
defer func() {
    order += ""3""
}()
order
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("", script.Eval(s));
			Assert.AreEqual("321", script.Eval("order"));
		}

		[TestMethod]
		public void Test03_DeferWithReturn()
		{
			var s = @"
func foo() int {
    var result = 0
    defer func() {
        result = 100
    }()
    return result
}
foo()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(0, script.Eval(s));
		}

		[TestMethod]
		public void Test03_DeferWithReturn_CompileAll()
		{
			var s = @"
func foo() int {
    var result = 0
    defer func() {
        result = 100
    }()
    return result
}
foo()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(0, script.Eval(s));
		}

		[TestMethod]
		public void Test04_DeferWithMutex()
		{
			var s = @"
import('sync')
var mutex sync.Mutex
var counter = 0
func increment() {
    mutex.Lock()
    defer mutex.Unlock()
    counter = counter + 1
}
increment()
increment()
counter
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test04_DeferWithMutex_CompileAll()
		{
			var s = @"
import('sync')
var mutex sync.Mutex
var counter = 0
func increment() {
    mutex.Lock()
    defer mutex.Unlock()
    counter = counter + 1
}
increment()
increment()
counter
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test05_DeferWithFileOperation()
		{
			var s = @"
var opened = false
var closed = false
func open() {
    opened = true
}
func close() {
    closed = true
}
func process() {
    open()
    defer close()
}
process()
opened && closed
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test05_DeferWithFileOperation_CompileAll()
		{
			var s = @"
var opened = false
var closed = false
func open() {
    opened = true
}
func close() {
    closed = true
}
func process() {
    open()
    defer close()
}
process()
opened && closed
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test06_DeferWithParameters()
		{
			var s = @"
var result = """"
func foo(x int) {
    result += ""x=$x""
}
func bar() {
    result += ""bar""
}
foo(1)
defer bar()
foo(2)
result
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("x=2x=1bar", script.Eval(s));
		}

		[TestMethod]
		public void Test06_DeferWithParameters_CompileAll()
		{
			var s = @"
var result = """"
func foo(x int) {
    result += ""x=$x""
}
func bar() {
    result += ""bar""
}
foo(1)
defer bar()
foo(2)
result
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("x=2x=1bar", script.Eval(s));
		}

		[TestMethod]
		public void Test07_DeferInLoop()
		{
			var s = @"
var order = """"
func process() {
    for i := 1; i <= 3; i++ {
        defer func() {
            order += ""$i""
        }()
    }
}
process()
order
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s) as string;
			Assert.AreEqual("321", result);
		}

		[TestMethod]
		public void Test07_DeferInLoop_CompileAll()
		{
			var s = @"
var order = """"
func process() {
    for i := 1; i <= 3; i++ {
        defer func() {
            order += ""$i""
        }()
    }
}
process()
order
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s) as string;
			Assert.AreEqual("321", result);
		}

		[TestMethod]
		public void Test08_DeferMultipleInFunction()
		{
			var s = @"
var order = """"
func foo() {
    order += ""start""
    defer func() {
        order += ""1""
    }()
    order += ""mid""
    defer func() {
        order += ""2""
    }()
    order += ""end""
}
foo()
order
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("startmidend21", script.Eval(s));
		}

		[TestMethod]
		public void Test08_DeferMultipleInFunction_CompileAll()
		{
			var s = @"
var order = """"
func foo() {
    order += ""start""
    defer func() {
        order += ""1""
    }()
    order += ""mid""
    defer func() {
        order += ""2""
    }()
    order += ""end""
}
foo()
order
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("startmidend21", script.Eval(s));
		}

		[TestMethod]
		public void Test09_DeferWithNamedReturn()
		{
			var s = @"
func foo() (result int) {
    defer func() {
        result = 999
    }()
    return 0
}
foo()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(999, script.Eval(s));
		}

		[TestMethod]
		public void Test09_DeferWithNamedReturn_CompileAll()
		{
			var s = @"
func foo() (result int) {
    defer func() {
        result = 999
    }()
    return 0
}
foo()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(999, script.Eval(s));
		}

		[TestMethod]
		public void Test10_DeferWithMap()
		{
			var s = @"
var m = make(map[string]int)
func foo() {
    m[""a""] = 1
    defer func() {
        m[""b""] = 2
    }()
    m[""c""] = 3
}
foo()
m[""a""] + m[""b""] + m[""c""]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(6, script.Eval(s));
		}

		[TestMethod]
		public void Test10_DeferWithMap_CompileAll()
		{
			var s = @"
var m = make(map[string]int)
func foo() {
    m[""a""] = 1
    defer func() {
        m[""b""] = 2
    }()
    m[""c""] = 3
}
foo()
m[""a""] + m[""b""] + m[""c""]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(6, script.Eval(s));
		}

		[TestMethod]
		public void Test11_DeferWithSlice()
		{
			var s = @"
var arr = []int{1, 2, 3}
func foo() {
    arr[0] = 100
    defer func() {
        arr[1] = 200
    }()
    arr[2] = 300
}
foo()
arr[0] + arr[1] + arr[2]
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(600, script.Eval(s));
		}

		[TestMethod]
		public void Test11_DeferWithSlice_CompileAll()
		{
			var s = @"
var arr = []int{1, 2, 3}
func foo() {
    arr[0] = 100
    defer func() {
        arr[1] = 200
    }()
    arr[2] = 300
}
foo()
arr[0] + arr[1] + arr[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(600, script.Eval(s));
		}

		[TestMethod]
		public void Test12_DeferNestedFunction()
		{
			var s = @"
var order = """"
func outer() {
    defer func() {
        order += ""outer""
    }()
    inner := func() {
        defer func() {
            order += ""inner""
        }()
        order += ""body""
    }
    inner()
}
outer()
order
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("bodyinnerouter", script.Eval(s));
		}

		[TestMethod]
		public void Test12_DeferNestedFunction_CompileAll()
		{
			var s = @"
var order = """"
func outer() {
    defer func() {
        order += ""outer""
    }()
    inner := func() {
        defer func() {
            order += ""inner""
        }()
        order += ""body""
    }
    inner()
}
outer()
order
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("bodyinnerouter", script.Eval(s));
		}

		[TestMethod]
		public void Test13_DeferMethod()
		{
			var s = @"
var order = """"
type Person struct {
    name string
}
func (p Person) greet() {
    order += ""hello""
}
func (p Person) cleanup() {
    order += ""bye""
}
var p = Person{""Tom""}
p.greet()
defer p.cleanup()
order
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("hellobye", script.Eval(s));
		}

		[TestMethod]
		public void Test13_DeferMethod_CompileAll()
		{
			var s = @"
var order = """"
type Person struct {
    name string
}
func (p Person) greet() {
    order += ""hello""
}
func (p Person) cleanup() {
    order += ""bye""
}
var p = Person{""Tom""}
p.greet()
defer p.cleanup()
order
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual("hellobye", script.Eval(s));
		}
	}
}