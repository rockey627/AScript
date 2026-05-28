using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AScript.Test.MSTests.Sql
{
	[TestClass]
	public class SqlInsertTest
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
		public void Test_insert_autoconvert_2()
		{
			var s = @"insert into list (name, age) values ('tom', '20')";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_insert_autoconvert()
		{
			var s = @"insert into list (name, age) values ('tom', '20')";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_insert_lowercase_2()
		{
			var s = @"insert into list (name, age) values ('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_insert_lowercase()
		{
			var s = @"insert into list (name, age) values ('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_insert_single_row_2()
		{
			var s = @"insert into list (Name, Age) values ('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_insert_single_row()
		{
			var s = @"insert into list (Name, Age) values ('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_insert_multiple_rows_2()
		{
			var s = @"insert into list (Name, Age) values ('tom', 20), ('jim', 25)";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
			Assert.AreEqual("jim", list[1].Name);
			Assert.AreEqual(25, list[1].Age);
		}

		[TestMethod]
		public void Test_insert_multiple_rows()
		{
			var s = @"insert into list (Name, Age) values ('tom', 20), ('jim', 25)";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
			Assert.AreEqual("jim", list[1].Name);
			Assert.AreEqual(25, list[1].Age);
		}

		[TestMethod]
		public void Test_insert_into_existing_list_2()
		{
			var s = @"insert into list (Name, Age) values ('tom', 20)";
			var list = new List<Person>
			{
				new Person("jim", 18)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("jim", list[0].Name);
			Assert.AreEqual(18, list[0].Age);
			Assert.AreEqual("tom", list[1].Name);
			Assert.AreEqual(20, list[1].Age);
		}

		[TestMethod]
		public void Test_insert_into_existing_list()
		{
			var s = @"insert into list (Name, Age) values ('tom', 20)";
			var list = new List<Person>
			{
				new Person("jim", 18)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("jim", list[0].Name);
			Assert.AreEqual(18, list[0].Age);
			Assert.AreEqual("tom", list[1].Name);
			Assert.AreEqual(20, list[1].Age);
		}
	}
}