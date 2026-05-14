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
