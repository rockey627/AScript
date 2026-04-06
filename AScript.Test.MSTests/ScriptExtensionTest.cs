using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptExtensionTest
	{
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
				Assert.IsTrue(false);
			}
			catch (Exception ex)
			{
				Assert.AreEqual("unknown function: System.String.eval2()", ex.Message);
			}
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
