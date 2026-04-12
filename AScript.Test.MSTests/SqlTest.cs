using AScript.Nodes;
using AScript.Operators;
using AScript.Syntaxs;
using AScript.Test.MSTests.Sql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class SqlTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["sql"] = SqlLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("sql", out _);
		}

		[TestMethod]
		public void Test06_2()
		{
			string s = @"
var matchedList = new List<Person>();
foreach(var p in list) {
	if (#lang sql p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='pen') matchedList.Add(p);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			var matchedList = script.Eval<List<Person>>(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test06()
		{
			string s = @"
var matchedList = new List<Person>();
foreach(var p in list) {
	if (#lang sql p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='pen') matchedList.Add(p);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			var matchedList = script.Eval<List<Person>>(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test05_2()
		{
			string s = @"
bool isMatch(Person p) {
	#lang sql
	p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='pen';
}
var matchedList = new List<Person>();
foreach(var item in list) {
	if (isMatch(item)) matchedList.Add(item);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			var matchedList = script.Eval<List<Person>>(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test05()
		{
			string s = @"
bool isMatch(Person p) {
	#lang sql
	p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='pen';
}
var matchedList = new List<Person>();
foreach(var item in list) {
	if (isMatch(item)) matchedList.Add(item);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			var matchedList = script.Eval<List<Person>>(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test04_2()
		{
			string s = @"
bool isMatch(Person p) => 
#lang sql
p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='pen';
#end
var matchedList = new List<Person>();
foreach(var item in list) {
	if (isMatch(item)) matchedList.Add(item);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			var matchedList = script.Eval<List<Person>>(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test04()
		{
			string s = @"
bool isMatch(Person p) => 
#lang sql
p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='pen';
#end
var matchedList = new List<Person>();
foreach(var item in list) {
	if (isMatch(item)) matchedList.Add(item);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			var matchedList = script.Eval<List<Person>>(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test03_2()
		{
			string s = @"
bool isMatch(Person p) => #lang sql p.Age>20 and p.Age<50 or p.Name like 'to%'; #end
foreach(var item in list) {
	if (isMatch(item)) matchedList.Add(item);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25),
				new Person("lin", 70)
			};
			var matchedList = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			script.Context.SetVar("matchedList", matchedList);
			script.Eval(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test03()
		{
			string s = @"
bool isMatch(Person p) => #lang sql p.Age>20 and p.Age<50 or p.Name like 'to%'; #end
foreach(var item in list) {
	if (isMatch(item)) matchedList.Add(item);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25),
				new Person("lin", 70)
			};
			var matchedList = new List<Person>();
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			script.Context.SetVar("matchedList", matchedList);
			script.Eval(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test02_2()
		{
			string s = "p.Name like 'to%' or p.Age>20 and p.Age<50";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var matchFunc = script.Compile<Person, bool>(s, "p");
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25),
				new Person("lin", 70)
			};
			var matchedList = list.Where(matchFunc).ToList();
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test02()
		{
			string s = "p.Age>20 and p.Age<50 or p.Name like 'to%'";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var matchFunc = script.Compile<Person, bool>(s, "p");
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var matchedList = list.Where(matchFunc).ToList();
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = "p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='san'";
			var p = new Person("tom", 60);
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("p", p);
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			Assert.AreEqual(false, script.Eval(s));
			p.Name = "san";
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			p.Age = 30;
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = "p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='san'";
			var p = new Person("tom", 60);
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("p", p);
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			Assert.AreEqual(false, script.Eval(s));
			p.Name = "san";
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			p.Age = 30;
			Assert.AreEqual(true, script.Eval(s));
		}
	}
}
