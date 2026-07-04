using AScript.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptExtensionTest
	{
		[TestMethod]
		public void Test13_Person_Module()
		{
			string s = @"
var p1 = new Person('张', '三');
var p2 = Person.Create('李', '四');
var hi = p1.SayHi(Person.DefaultName);
var info = p2.FullInfo;
";
			var script = new Script();
			script.Context.AddModule(new PersonModule());
			script.Eval(s);
			var hi = script.Eval("hi");
			var info = script.Eval("info");
			Assert.AreEqual("hi ABC, my name is 张三", hi);
			Assert.AreEqual("name:李四,age:18", info);
		}

		[TestMethod]
		public void Test12_Person_DefaultName()
		{
			string s = @"
// 脚本中扩展静态属性
string Person_get_DefaultName() {
	return 'ABC';
}
Person.DefaultName;
";
			var script = new Script();
			script.Context.AddType<Person>();
			Assert.AreEqual("ABC", script.Eval(s));
		}

		[TestMethod]
		public void Test12_Person_DefaultName2()
		{
			string s = @"
// 脚本中扩展静态属性
string Person_get_DefaultName() {
	return 'ABC';
}
Person.DefaultName;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			Assert.AreEqual("ABC", script.Eval(s));
		}

		[TestMethod]
		public void Test11_Person_FullInfo()
		{
			string s = @"
// 脚本中扩展实例属性
string get_FullInfo(Person p) {
	var info = $'name:{p.Name},age:{p.Age}';
	return info;
}
var p = new Person('tom', 20);
// 调用脚本中扩展的实例属性
var fullInfo = p.FullInfo;
";
			var script = new Script();
			script.Context.AddType<Person>();
			Assert.AreEqual("name:tom,age:20", script.Eval(s));
		}

		[TestMethod]
		public void Test11_Person_FullInfo2()
		{
			string s = @"
// 脚本中扩展实例属性
string get_FullInfo(Person p) {
	var info = $'name:{p.Name},age:{p.Age}';
	return info;
}
var p = new Person('tom', 20);
// 调用脚本中扩展的实例属性
var fullInfo = p.FullInfo;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			Assert.AreEqual("name:tom,age:20", script.Eval(s));
		}

		[TestMethod]
		public void Test10_Person_SayHi()
		{
			string s = @"
// 脚本中扩展实例方法
string SayHi(Person p, string yourName) {
	var hi = $'hi {yourName}, my name is {p.Name}';
	return hi;
}
var p = new Person('tom', 20);
// 调用脚本中扩展的实例方法
var hi = p.SayHi('john');
// 调用外部扩展的实例方法
var night = p.SayGoodNight('john');
";
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.AddFunc<Person, string, string>("SayGoodNight", (p, yourName) => $"good night {yourName}, my name is {p.Name}");
			script.Eval(s);
			var hi = script.Eval<string>("hi");
			var night = script.Eval<string>("night");
			Assert.AreEqual("hi john, my name is tom", hi);
			Assert.AreEqual("good night john, my name is tom", night);
		}

		[TestMethod]
		public void Test10_Person_SayHi2()
		{
			string s = @"
// 脚本中扩展实例方法
string SayHi(Person p, string yourName) {
	var hi = $'hi {yourName}, my name is {p.Name}';
	return hi;
}
var p = new Person('tom', 20);
// 调用脚本中扩展的实例方法
var hi = p.SayHi('john');
// 调用外部扩展的实例方法
var night = p.SayGoodNight('john');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.AddFunc<Person, string, string>("SayGoodNight", (p, yourName) => $"good night {yourName}, my name is {p.Name}");
			script.Eval(s);
			var hi = script.Eval<string>("hi");
			var night = script.Eval<string>("night");
			Assert.AreEqual("hi john, my name is tom", hi);
			Assert.AreEqual("good night john, my name is tom", night);
		}

		[TestMethod]
		public void Test09_Person_Create()
		{
			string s = @"
// 脚本中扩展静态方法
Person Person_Create(string firstName, string lastName) {
	var name = firstName + lastName;
	return new Person { Name = name };
}
// 调用脚本中扩展的静态方法
var p1 = Person.Create('张', '三');
// 调用外部扩展的静态方法
var p2 = Person.Create('李四');
";
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.AddFunc<string, Person>("Person_Create", name => new Person { Name = name });
			script.Eval(s);
			var p1 = script.Eval<Person>("p1");
			var p2 = script.Eval<Person>("p2");
			Assert.AreEqual("张三", p1.Name);
			Assert.AreEqual("李四", p2.Name);
		}

		[TestMethod]
		public void Test09_Person_Create2()
		{
			string s = @"
// 脚本中扩展静态方法
Person Person_Create(string firstName, string lastName) {
	var name = firstName + lastName;
	return new Person { Name = name };
}
// 调用脚本中扩展的静态方法
var p1 = Person.Create('张', '三');
// 调用外部扩展的静态方法
var p2 = Person.Create('李四');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.AddFunc<string, Person>("Person_Create", name => new Person { Name = name });
			script.Eval(s);
			var p1 = script.Eval<Person>("p1");
			var p2 = script.Eval<Person>("p2");
			Assert.AreEqual("张三", p1.Name);
			Assert.AreEqual("李四", p2.Name);
		}

		[TestMethod]
		public void Test08_Person_Contruct()
		{
			string s = @"
// 脚本中扩展构造函数
Person new_Person(string firstName, string lastName) {
	var name = firstName + lastName;
	return new Person { Name = name };
}
// 调用脚本中扩展的构造函数
var p1 = new Person('张', '三');
// 调用外部扩展的构造函数
var p2 = new Person('李四');
";
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.AddFunc<string, Person>("new_Person", name => new Person { Name = name });
			script.Eval(s);
			var p1 = script.Eval<Person>("p1");
			var p2 = script.Eval<Person>("p2");
			Assert.AreEqual("张三", p1.Name);
			Assert.AreEqual("李四", p2.Name);
		}

		[TestMethod]
		public void Test08_Person_Contruct2()
		{
			string s = @"
// 脚本中扩展构造函数
Person new_Person(string firstName, string lastName) {
	var name = firstName + lastName;
	return new Person { Name = name };
}
// 调用脚本中扩展的构造函数
var p1 = new Person('张', '三');
// 调用外部扩展的构造函数
var p2 = new Person('李四');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.AddFunc<string, Person>("new_Person", name => new Person { Name = name });
			script.Eval(s);
			var p1 = script.Eval<Person>("p1");
			var p2 = script.Eval<Person>("p2");
			Assert.AreEqual("张三", p1.Name);
			Assert.AreEqual("李四", p2.Name);
		}

		[TestMethod]
		public void Test07_2()
		{
			var p = new Person { Name = "san" };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddFunc(typeof(Person), p);
			Assert.AreEqual("san play game xx", script.Eval("Play('xx')"));
			Assert.AreEqual("san play game mm", script.Eval("'mm'.Play()"));
		}

		[TestMethod]
		public void Test07()
		{
			var p = new Person { Name = "san" };
			var script = new Script();
			script.Context.AddFunc(typeof(Person), p);
			Assert.AreEqual("san play game xx", script.Eval("Play('xx')"));
			Assert.AreEqual("san play game mm", script.Eval("'mm'.Play()"));
		}

		[TestMethod]
		public void Test06_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(12, script.Eval("'12'.ToInt32()"));
		}

		[TestMethod]
		public void Test06()
		{
			var script = new Script();
			Assert.AreEqual(12, script.Eval("'12'.ToInt32()"));
		}

		[TestMethod]
		public void Test05_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			try
			{
				script.Eval("'5+6'.eval2()");
				Assert.IsTrue(false);
			}
			catch (Exception ex)
			{
				Assert.AreEqual("unknown function: System.String.eval2()", ex.Message);
			}
		}

		[TestMethod]
		public void Test05()
		{
			var script = new Script();
			try
			{
				script.Eval("'5+6'.eval2()");
			}
			catch (ScriptException ex)
			{
				Assert.AreEqual("unknown function: eval2(System.String)", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test04_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(11, script.Eval("'5+6'.eval()"));
		}

		[TestMethod]
		public void Test04()
		{
			var script = new Script();
			Assert.AreEqual(11, script.Eval("'5+6'.eval()"));
		}

		[TestMethod]
		public void Test03_4()
		{
			string s = @"
string Goodbye(Person p) => 'good bye ' + p.Name;
Goodbye(person);
Goodbye(person)
";
			var person = new Person { Name = "jim" };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.SetVar("person", person);
			//script.Eval(s);
			Assert.AreEqual("good bye jim", script.Eval(s));
			//Assert.AreEqual("good bye jim", script.Eval("Goodbye(person)"));
			//Assert.AreEqual("good bye jim", script.Eval("Goodbye(person)"));
		}

		private static Delegate _test;

		public static void SetDelegate(Delegate d)
		{
			_test = d;
		}

		public static readonly MethodInfo Method_String_Concat3 = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string), typeof(string) });

		[TestMethod]
		public void Test03_7()
		{
			var p0 = Expression.Parameter(typeof(string));
			var varN = Expression.Variable(typeof(string));
			var nameParam = Expression.Parameter(typeof(string));

			var concatMethod = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string), typeof(string) });
			var assignN = Expression.Assign(varN, Expression.Constant("good "));
			var addExpr = Expression.Call(concatMethod, varN, Expression.Constant("bye "), nameParam);
			var goodbyeLambda = Expression.Lambda(addExpr, nameParam);
			//var compiledLambda = goodbyeLambda.Compile();
			// 使用 Quote 标记内部 Lambda 对外部变量 varN 的捕获
			var quotedLambda = Expression.Quote(goodbyeLambda);
			var innerLambda = Expression.Lambda(Expression.Invoke(quotedLambda, nameParam), nameParam);
			var setDelegateExpr = Expression.Call(typeof(ScriptExtensionTest).GetMethod("SetDelegate"), innerLambda);
			var callGoodbyeExpr = Expression.Invoke(quotedLambda, p0);
			var body = Expression.Block(new[] { varN }, assignN, setDelegateExpr, callGoodbyeExpr);
			var lambda = Expression.Lambda(body, p0);
			var func = (Func<string, string>)lambda.Compile();
			Assert.AreEqual("good bye tom", func("tom"));
			Assert.AreEqual("good bye tom", ((Func<string, string>)_test)("tom"));
		}

		[TestMethod]
		public void Test03_6()
		{
			/**
			Func<string, string> func = p0 => {
				string n = "good ";
				Func<string, string> goodbye = name => n + "bye " + name;
				return goodby(p0);
			}
			 * */
			var p0 = Expression.Parameter(typeof(string));
			var varN = Expression.Variable(typeof(string));
			var assignN = Expression.Assign(varN, Expression.Constant("good "));
			var nameParameter = Expression.Parameter(typeof(string));
			var addExpr = Expression.Call(Method_String_Concat3, varN, Expression.Constant("bye "), nameParameter);
			var goodbyeLambda = Expression.Lambda(addExpr, nameParameter);
			var callGoodbyeExpr = Expression.Invoke(Expression.Quote(goodbyeLambda), p0);
			var body = Expression.Block(new[] { varN }, assignN, callGoodbyeExpr);
			var lambda = Expression.Lambda(body, p0);
			var func = (Func<string, string>)lambda.Compile();
			Assert.AreEqual("good bye tom", func("tom"));
		}

		[TestMethod]
		public void Test03_5()
		{
			string s = @"
string Goodbye(string name) => 'good bye ' + name;
Goodbye(personName)
";
			var person = new Person { Name = "jim" };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.SetVar("personName", person.Name);
			//script.Eval(s);
			Assert.AreEqual("good bye jim", script.Eval(s));
			Assert.AreEqual("good bye jim", script.Eval("Goodbye('jim')"));
			//Assert.AreEqual("good bye jim", script.Eval("Goodbye(person)"));
		}

		[TestMethod]
		public void Test03_3()
		{
			string s = @"
string Goodbye(Person p) => 'good bye ' + p.Name;
Goodbye(person)
";
			var person = new Person { Name = "jim" };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.SetVar("person", person);
			//script.Eval(s);
			Assert.AreEqual("good bye jim", script.Eval(s));
			Assert.AreEqual("good bye jim", script.Eval("Goodbye(person)"));
			//Assert.AreEqual("good bye jim", script.Eval("Goodbye(person)"));
		}

		[TestMethod]
		public void Test03_2()
		{
			string s = @"
string Goodbye(Person p) => 'good bye ' + p.Name;
var person = new Person { Name = 'jim' };
person.Goodbye();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			Assert.AreEqual("good bye jim", script.Eval(s));
			Assert.AreEqual("good bye jim", script.Eval("Goodbye(person)"));
		}

		[TestMethod]
		public void Test03()
		{
			string s = @"
string Goodbye(Person p) => 'good bye ' + p.Name;
var person = new Person { Name = 'jim' };
person.Goodbye();
";
			var script = new Script();
			script.Context.AddType<Person>();
			Assert.AreEqual("good bye jim", script.Eval(s));
			Assert.AreEqual("good bye jim", script.Eval("Goodbye(person)"));
		}

		[TestMethod]
		public void Test02_2()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			Assert.AreEqual(11, script.Eval("5.sum(6)", ECompileMode.All));
		}

		[TestMethod]
		public void Test02()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			Assert.AreEqual(11, script.Eval("5.sum(6)"));
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = @"
int sum(int a, int b)=>a+b;
5.sum(6)
";
			var script = new Script();
			Assert.AreEqual(11, script.Eval(s, ECompileMode.All));
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
int sum(int a, int b)=>a+b;
5.sum(6)
";
			var script = new Script();
			Assert.AreEqual(11, script.Eval(s));
		}
	}
}
