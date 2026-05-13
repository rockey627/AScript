using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptEventTest
	{
		[TestMethod]
		public void Test03_2()
		{
			var s = @"
void saying(object sender, EventArgs e) {
	(sender as Person).Age+=1;
}
var p = new Person('tom', 20);
p.Saying += saying;
p.SayHello();
p.SayHello();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			Assert.AreEqual("Hello, my name is tom, I'm 22 years old", script.Eval(s));
			var p = script.Eval<Person>("p");
			Assert.AreEqual("Hello, my name is tom, I'm 23 years old", p.SayHello());
			var handle = script.Context.GetEvent<EventHandler<EventArgs>>("saying");
			p.Saying -= handle;
			Assert.AreEqual("Hello, my name is tom, I'm 23 years old", p.SayHello());
		}

		[TestMethod]
		public void Test03()
		{
			var s = @"
void saying(object sender, EventArgs e) {
	(sender as Person).Age+=1;
}
var p = new Person('tom', 20);
p.Saying += saying;
p.SayHello();
p.SayHello();
";
			var script = new Script();
			script.Context.AddType<Person>();
			Assert.AreEqual("Hello, my name is tom, I'm 22 years old", script.Eval(s));
			var p = script.Eval<Person>("p");
			Assert.AreEqual("Hello, my name is tom, I'm 23 years old", p.SayHello());
			var handle = script.Context.GetEvent<EventHandler<EventArgs>>("saying");
			p.Saying -= handle;
			Assert.AreEqual("Hello, my name is tom, I'm 23 years old", p.SayHello());
		}

		[TestMethod]
		public void Test02_2()
		{
			var s = @"
void saying(object sender, EventArgs e) {
	(sender as Person).Age+=1;
}
var p = new Person('tom', 20);
p.Saying += saying;
p.SayHello();
p.Saying -= saying;
p.SayHello();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			Assert.AreEqual("Hello, my name is tom, I'm 21 years old", script.Eval(s));
		}

		[TestMethod]
		public void Test02()
		{
			var s = @"
void saying(object sender, EventArgs e) {
	(sender as Person).Age+=1;
}
var p = new Person('tom', 20);
p.Saying += saying;
p.SayHello();
p.Saying -= saying;
p.SayHello();
";
			var script = new Script();
			script.Context.AddType<Person>();
			Assert.AreEqual("Hello, my name is tom, I'm 21 years old", script.Eval(s));
		}

		[TestMethod]
		public void Test01_2()
		{
			var s = @"
void saying(object sender, EventArgs e) {
	(sender as Person).Age+=1;
}
var p = new Person('tom', 20);
p.Saying += saying;
p.SayHello();
p.SayHello();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			Assert.AreEqual("Hello, my name is tom, I'm 22 years old", script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			var s = @"
void saying(object sender, EventArgs e) {
	(sender as Person).Age+=1;
}
var p = new Person('tom', 20);
p.Saying += saying;
p.SayHello();
p.SayHello();
";
			var script = new Script();
			script.Context.AddType<Person>();
			Assert.AreEqual("Hello, my name is tom, I'm 22 years old", script.Eval(s));
		}

	}
}
