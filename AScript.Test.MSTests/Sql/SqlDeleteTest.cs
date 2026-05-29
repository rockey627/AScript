using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AScript.Test.MSTests.Sql
{
	[TestClass]
	public class SqlDeleteTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["sql"] = SqlLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("sql");
		}

		[TestMethod]
		public void Test_delete_basic_2()
		{
			var s = @"delete from list where Name='jim'";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(1, count);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
		}

		[TestMethod]
		public void Test_delete_basic()
		{
			var s = @"delete from list where Name='jim'";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(1, count);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
		}

		[TestMethod]
		public void Test_delete_no_condition_2()
		{
			var s = @"delete from list";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(2, count);
			Assert.AreEqual(0, list.Count);
		}

		[TestMethod]
		public void Test_delete_no_condition()
		{
			var s = @"delete from list";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(2, count);
			Assert.AreEqual(0, list.Count);
		}

		[TestMethod]
		public void Test_delete_ignorecase_2()
		{
			var s = @"delete from list where name='tom'";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(1, count);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("jim", list[0].Name);
		}

		[TestMethod]
		public void Test_delete_ignorecase()
		{
			var s = @"delete from list where name='tom'";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(1, count);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("jim", list[0].Name);
		}

		[TestMethod]
		public void Test_delete_multiple_2()
		{
			var s = @"delete from list where Age<25";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20),
				new Person("lily", 30)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(2, count);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("lily", list[0].Name);
		}

		[TestMethod]
		public void Test_delete_multiple()
		{
			var s = @"delete from list where Age<25";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20),
				new Person("lily", 30)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(2, count);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("lily", list[0].Name);
		}

		[TestMethod]
		public void Test_delete_no_match_2()
		{
			var s = @"delete from list where Name='notexist'";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(0, count);
			Assert.AreEqual(2, list.Count);
		}

		[TestMethod]
		public void Test_delete_no_match()
		{
			var s = @"delete from list where Name='notexist'";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tom", 20)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(0, count);
			Assert.AreEqual(2, list.Count);
		}
	}
}