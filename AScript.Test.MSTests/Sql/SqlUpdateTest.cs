using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AScript.Test.MSTests.Sql
{
	[TestClass]
	public class SqlUpdateTest
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
		public void Test_update_basic_2()
		{
			var s = @"update list set Name='tom', Age=20 where Name='jim'";
			var list = new List<Person>
			{
				new Person("jim", 18)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(1, count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_update_basic()
		{
			var s = @"update list set Name='tom', Age=20 where Name='jim'";
			var list = new List<Person>
			{
				new Person("jim", 18)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var count = (int)script.Eval(s);
			Assert.AreEqual(1, count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_update_no_condition_2()
		{
			var s = @"update list set Age=30";
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
			Assert.AreEqual(30, list[0].Age);
			Assert.AreEqual(30, list[1].Age);
		}

		[TestMethod]
		public void Test_update_no_condition()
		{
			var s = @"update list set Age=30";
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
			Assert.AreEqual(30, list[0].Age);
			Assert.AreEqual(30, list[1].Age);
		}

		[TestMethod]
		public void Test_update_with_value_reference_2()
		{
			var s = @"update list set Age=Age+10 where Name='tom'";
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
			Assert.AreEqual(18, list[0].Age);
			Assert.AreEqual(30, list[1].Age);
		}

		[TestMethod]
		public void Test_update_with_value_reference()
		{
			var s = @"update list set Age=Age+10 where Name='tom'";
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
			Assert.AreEqual(18, list[0].Age);
			Assert.AreEqual(30, list[1].Age);
		}

		[TestMethod]
		public void Test_update_multiple_matching_2()
		{
			var s = @"update list set Age=99 where Age<25";
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
			Assert.AreEqual(99, list[0].Age);
			Assert.AreEqual(99, list[1].Age);
			Assert.AreEqual(30, list[2].Age);
		}

		[TestMethod]
		public void Test_update_multiple_matching()
		{
			var s = @"update list set Age=99 where Age<25";
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
			Assert.AreEqual(99, list[0].Age);
			Assert.AreEqual(99, list[1].Age);
			Assert.AreEqual(30, list[2].Age);
		}

		[TestMethod]
		public void Test_update_single_field_2()
		{
			var s = @"update list set Name='updated' where Name='tom'";
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
			Assert.AreEqual("jim", list[0].Name);
			Assert.AreEqual("updated", list[1].Name);
		}

		[TestMethod]
		public void Test_update_single_field()
		{
			var s = @"update list set Name='updated' where Name='tom'";
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
			Assert.AreEqual("jim", list[0].Name);
			Assert.AreEqual("updated", list[1].Name);
		}

		[TestMethod]
		public void Test_update_no_match_2()
		{
			var s = @"update list set Age=99 where Name='notexist'";
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
			Assert.AreEqual(18, list[0].Age);
			Assert.AreEqual(20, list[1].Age);
		}

		[TestMethod]
		public void Test_update_no_match()
		{
			var s = @"update list set Age=99 where Name='notexist'";
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
			Assert.AreEqual(18, list[0].Age);
			Assert.AreEqual(20, list[1].Age);
		}
	}
}