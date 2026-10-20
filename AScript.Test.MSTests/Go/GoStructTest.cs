using AScript;
using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoStructTest
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
		public void Test01_StructDefinition()
		{
			var s = @"
type Person struct {
    name string
    age int
}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			var personType = script.Eval("Person");
			Assert.IsNotNull(personType);
			Assert.IsInstanceOfType(personType, typeof(TypeWrapper));
			Assert.AreEqual("Person", ((ClassDefine)((TypeWrapper)personType).Type).Name);
		}

		[TestMethod]
		public void Test01_StructDefinition_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			var personType = script.Eval("Person");
			Assert.IsNotNull(personType);
			Assert.IsInstanceOfType(personType, typeof(TypeWrapper));
			Assert.AreEqual("Person", ((ClassDefine)((TypeWrapper)personType).Type).Name);
		}

		[TestMethod]
		public void Test02_StructCreateInstance()
		{
			var s = @"
type Person struct {
    name string
    age int
}
var p = Person{}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			var p = script.Eval("p");
			Assert.IsNotNull(p);
			Assert.IsInstanceOfType(p, typeof(ClassObject));
			Assert.AreEqual("Person", ((ClassObject)p).Define.Name);
		}

		[TestMethod]
		public void Test02_StructCreateInstance_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
var p = Person{}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			var p = script.Eval("p");
			Assert.IsNotNull(p);
			Assert.IsInstanceOfType(p, typeof(ClassObject));
			Assert.AreEqual("Person", ((ClassObject)p).Define.Name);
		}

		[TestMethod]
		public void Test03_StructCreateInstanceWithInit()
		{
			var s = @"
type Person struct {
    name string
    age int
}
var p = Person{name: 'John', age: 20}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			var p = script.Eval("p");
			Assert.IsNotNull(p);
			Assert.IsInstanceOfType(p, typeof(ClassObject));
			Assert.AreEqual("John", script.Eval("p.name"));
			Assert.AreEqual(20, script.Eval("p.age"));
		}

		[TestMethod]
		public void Test03_StructCreateInstanceWithInit_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
var p = Person{name: 'John', age: 20}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			var p = script.Eval("p");
			Assert.IsNotNull(p);
			Assert.IsInstanceOfType(p, typeof(ClassObject));
			Assert.AreEqual("John", script.Eval("p.name"));
			Assert.AreEqual(20, script.Eval("p.age"));
		}

		[TestMethod]
		public void Test04_StructPropertyAccess()
		{
			var s = @"
type Person struct {
    name string
    age int
}
var p = Person{name: 'John', age: 20}
p.age = 25
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(25, script.Eval("p.age"));
		}

		[TestMethod]
		public void Test04_StructPropertyAccess_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
var p = Person{name: 'John', age: 20}
p.age = 25
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(25, script.Eval("p.age"));
		}

		[TestMethod]
		public void Test05_StructMethodDefinition()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) setName(name string) {
    p.name = name
}
var p = Person{name: 'John', age: 20}
p.setName('Tom')
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("John", script.Eval("p.name"));
		}

		[TestMethod]
		public void Test05_StructMethodDefinition_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) setName(name string) {
    p.name = name
}
var p = Person{name: 'John', age: 20}
p.setName('Tom')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("John", script.Eval("p.name"));
		}

		[TestMethod]
		public void Test05_StructMethodDefinition2()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p *Person) setName(name string) {
    p.name = name
}
var p = Person{name: 'John', age: 20}
p.setName('Tom')
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("Tom", script.Eval("p.name"));
		}

		[TestMethod]
		public void Test05_StructMethodDefinition2_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p *Person) setName(name string) {
    p.name = name
}
var p = Person{name: 'John', age: 20}
p.setName('Tom')
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("Tom", script.Eval("p.name"));
		}

		[TestMethod]
		public void Test06_StructMethodWithReturn()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) getName() string {
    return p.name
}
var p = Person{name: 'John', age: 20}
p.getName()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual("John", result);
		}

		[TestMethod]
		public void Test06_StructMethodWithReturn_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) getName() string {
    return p.name
}
var p = Person{name: 'John', age: 20}
p.getName()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual("John", result);
		}

		[TestMethod]
		public void Test07_StructMultipleMethods()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) setName(name string) {
    p.name = name
}
func (p Person) setAge(age int) {
    p.age = age
}
func (p Person) getInfo() string {
    return p.name + ',' + string(p.age)
}
var p = Person{name: 'John', age: 20}
p.setName('Tom')
p.setAge(25)
p.getInfo()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual("John,20", result);
		}

		[TestMethod]
		public void Test07_StructMultipleMethods_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) setName(name string) {
    p.name = name
}
func (p Person) setAge(age int) {
    p.age = age
}
func (p Person) getInfo() string {
    return p.name + ',' + string(p.age)
}
var p = Person{name: 'John', age: 20}
p.setName('Tom')
p.setAge(25)
p.getInfo()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual("John,20", result);
		}

		[TestMethod]
		public void Test07_StructMultipleMethods2()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p *Person) setName(name string) {
    p.name = name
}
func (p *Person) setAge(age int) {
    p.age = age
}
func (p Person) getInfo() string {
    return p.name + ',' + string(p.age)
}
var p = Person{name: 'John', age: 20}
p.setName('Tom')
p.setAge(25)
p.getInfo()
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual("Tom,25", result);
		}

		[TestMethod]
		public void Test07_StructMultipleMethods2_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p *Person) setName(name string) {
    p.name = name
}
func (p *Person) setAge(age int) {
    p.age = age
}
func (p Person) getInfo() string {
    return p.name + ',' + string(p.age)
}
var p = Person{name: 'John', age: 20}
p.setName('Tom')
p.setAge(25)
p.getInfo()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual("Tom,25", result);
		}

		[TestMethod]
		public void Test08_StructWithPointer()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) birthday() {
    p.age = p.age + 1
}
var p = Person{name: 'John', age: 20}
p.birthday()
p.age
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			// 值类型传递，不会修改原值
			Assert.AreEqual(20, script.Eval(s));
		}

		[TestMethod]
		public void Test08_StructWithPointer_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) birthday() {
    p.age = p.age + 1
}
var p = Person{name: 'John', age: 20}
p.birthday()
p.age
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			// 值类型传递，不会修改原值
			Assert.AreEqual(20, script.Eval(s));
		}

		[TestMethod]
		public void Test08_StructWithPointer2()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p *Person) birthday() {
    p.age = p.age + 1
}
var p = Person{name: 'John', age: 20}
p.birthday()
p.age
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual(21, script.Eval("p.age"));
		}

		[TestMethod]
		public void Test08_StructWithPointer2_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p *Person) birthday() {
    p.age = p.age + 1
}
var p = Person{name: 'John', age: 20}
p.birthday()
p.age
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(21, script.Eval(s));
		}

		[TestMethod]
		public void Test09_NestedStruct()
		{
			var s = @"
type Address struct {
    city string
    zip string
}
type Person struct {
    name string
    addr Address
}
var p = Person{name: 'John', addr: Address{city: 'Beijing', zip: '100000'}}
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("John", script.Eval("p.name"));
			Assert.AreEqual("Beijing", script.Eval("p.addr.city"));
			Assert.AreEqual("100000", script.Eval("p.addr.zip"));
		}

		[TestMethod]
		public void Test09_NestedStruct_CompileAll()
		{
			var s = @"
type Address struct {
    city string
    zip string
}
type Person struct {
    name string
    addr Address
}
var p = Person{name: 'John', addr: Address{city: 'Beijing', zip: '100000'}}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("John", script.Eval("p.name"));
			Assert.AreEqual("Beijing", script.Eval("p.addr.city"));
			Assert.AreEqual("100000", script.Eval("p.addr.zip"));
		}

		[TestMethod]
		public void Test10_StructCompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
var p = Person{name: 'John', age: 20}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("John", script.Eval("p.name"));
			Assert.AreEqual(20, script.Eval("p.age"));
		}

		[TestMethod]
		public void Test11_StructMethodCompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func (p Person) getName() string {
    return p.name
}
var p = Person{name: 'John', age: 20}
p.getName()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			var result = script.Eval(s);
			Assert.AreEqual("John", result);
		}

		[TestMethod]
		public void Test12_StructCreateInstanceWithArgs()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func new_Person(name string, age int) Person {
    return Person{name: name, age: age}
}
var p = new_Person('Tom', 30)
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("Tom", script.Eval("p.name"));
			Assert.AreEqual(30, script.Eval("p.age"));
		}

		[TestMethod]
		public void Test13_StructCreateInstanceWithArgs_CompileAll()
		{
			var s = @"
type Person struct {
    name string
    age int
}
func new_Person(name string, age int) Person {
    return Person{name: name, age: age}
}
var p = new_Person('Tom', 30)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			script.Eval(s);
			Assert.AreEqual("Tom", script.Eval("p.name"));
			Assert.AreEqual(30, script.Eval("p.age"));
		}
	}
}
