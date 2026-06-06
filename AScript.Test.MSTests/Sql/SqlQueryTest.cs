using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Test.MSTests.Sql
{
	[TestClass]
	public class SqlQueryTest
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
		public void Test30_select_if_2()
		{
			var s = @"select Name, Age, if(Age>12,2,1) as Level from list";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("jim", result[1].Name);
			Assert.AreEqual("san", result[2].Name);
			Assert.AreEqual("qin", result[3].Name);
			Assert.AreEqual(2, result[0].Level);
			Assert.AreEqual(1, result[1].Level);
			Assert.AreEqual(2, result[2].Level);
			Assert.AreEqual(1, result[3].Level);
		}

		[TestMethod]
		public void Test30_select_if()
		{
			var s = @"select Name, Age, if(Age>12,2,1) as Level from list";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("jim", result[1].Name);
			Assert.AreEqual("san", result[2].Name);
			Assert.AreEqual("qin", result[3].Name);
			Assert.AreEqual(2, result[0].Level);
			Assert.AreEqual(1, result[1].Level);
			Assert.AreEqual(2, result[2].Level);
			Assert.AreEqual(1, result[3].Level);
		}

		[TestMethod]
		public void Test29_select_limit_offset_2()
		{
			var s = @"select Name, Age from list limit 2 offset 1";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
		}

		[TestMethod]
		public void Test29_select_limit_offset()
		{
			var s = @"select Name, Age from list limit 2 offset 1";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
		}

		[TestMethod]
		public void Test28_select_limit_4()
		{
			var s = @"select Name, Age from list limit 1,2";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
		}

		[TestMethod]
		public void Test28_select_limit_3()
		{
			var s = @"select Name, Age from list limit 1,2";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
		}

		[TestMethod]
		public void Test28_select_limit_2()
		{
			var s = @"select Name, Age from list limit 3";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public void Test28_select_limit()
		{
			var s = @"select Name, Age from list limit 3";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public void Test27_isnotnull_6()
		{
			Assert.AreEqual((Person)null, (AddressInfo)null);

			var s = @"from list where NOT Name IS NULL";
			var list = new[]
			{
				new Person("tom", 15),
				new Person(null, 10),
				new Person("san", 20),
				new Person(null, 18)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
			Assert.AreEqual(15, result[0].Age);
			Assert.AreEqual(20, result[1].Age);
		}

		[TestMethod]
		public void Test27_isnotnull_5()
		{
			Assert.AreEqual((Person)null, (AddressInfo)null);

			var s = @"from list where NOT Name IS NULL";
			var list = new[]
			{
				new Person("tom", 15),
				new Person(null, 10),
				new Person("san", 20),
				new Person(null, 18)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
			Assert.AreEqual(15, result[0].Age);
			Assert.AreEqual(20, result[1].Age);
		}

		[TestMethod]
		public void Test27_isnotnull_4()
		{
			Assert.AreEqual((Person)null, (AddressInfo)null);

			var s = @"from list where Name NOT IS NULL";
			var list = new[]
			{
				new Person("tom", 15),
				new Person(null, 10),
				new Person("san", 20),
				new Person(null, 18)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
			Assert.AreEqual(15, result[0].Age);
			Assert.AreEqual(20, result[1].Age);
		}

		[TestMethod]
		public void Test27_isnotnull_3()
		{
			Assert.AreEqual((Person)null, (AddressInfo)null);

			var s = @"from list where Name NOT IS NULL";
			var list = new[]
			{
				new Person("tom", 15),
				new Person(null, 10),
				new Person("san", 20),
				new Person(null, 18)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
			Assert.AreEqual(15, result[0].Age);
			Assert.AreEqual(20, result[1].Age);
		}

		[TestMethod]
		public void Test27_isnotnull_2()
		{
			Assert.AreEqual((Person)null, (AddressInfo)null);

			var s = @"from list where Name IS NOT NULL";
			var list = new[]
			{
				new Person("tom", 15),
				new Person(null, 10),
				new Person("san", 20),
				new Person(null, 18)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
			Assert.AreEqual(15, result[0].Age);
			Assert.AreEqual(20, result[1].Age);
		}

		[TestMethod]
		public void Test27_isnotnull()
		{
			Assert.AreEqual((Person)null, (AddressInfo)null);

			var s = @"from list where Name IS NOT NULL";
			var list = new[]
			{
				new Person("tom", 15),
				new Person(null, 10),
				new Person("san", 20),
				new Person(null, 18)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("san", result[1].Name);
			Assert.AreEqual(15, result[0].Age);
			Assert.AreEqual(20, result[1].Age);
		}

		[TestMethod]
		public void Test26_isnull_2()
		{
			Assert.AreEqual((Person)null, (AddressInfo)null);

			var s = @"from list where Name IS NULL";
			var list = new[]
			{
				new Person("tom", 15),
				new Person(null, 10),
				new Person("san", 20),
				new Person(null, 18)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.IsNull(result[0].Name);
			Assert.IsNull(result[1].Name);
			Assert.AreEqual(10, result[0].Age);
			Assert.AreEqual(18, result[1].Age);
		}

		[TestMethod]
		public void Test26_isnull()
		{
			Assert.AreEqual((Person)null, (AddressInfo)null);

			var s = @"from list where Name IS NULL";
			var list = new[]
			{
				new Person("tom", 15),
				new Person(null, 10),
				new Person("san", 20),
				new Person(null, 18)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.IsNull(result[0].Name);
			Assert.IsNull(result[1].Name);
			Assert.AreEqual(10, result[0].Age);
			Assert.AreEqual(18, result[1].Age);
		}

		[TestMethod]
		public void Test25_where_notlike_4()
		{
			var s = @"from list where not Name like '%m'";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test25_where_notlike_3()
		{
			var s = @"from list where not Name like '%m'";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test25_where_notlike_2()
		{
			var s = @"from list where Name not like '%m'";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test25_where_notlike()
		{
			var s = @"from list where Name not like '%m'";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test24_where_like_2()
		{
			var s = @"from list where Name like '%m'";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("jim", result[1].Name);
		}

		[TestMethod]
		public void Test24_where_like()
		{
			var s = @"from list where Name like '%m'";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("jim", result[1].Name);
		}

		[TestMethod]
		public void Test23_where_notin_select_4()
		{
			var s = @"from list where not Name in (select Name from nameList)";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var nameList = new[] { new { Name = "jim" }, new { Name = "tom" } };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Context.SetVar("nameList", nameList);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test23_where_notin_select_3()
		{
			var s = @"from list where not Name in (select Name from nameList)";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var nameList = new[] { new { Name = "jim" }, new { Name = "tom" } };
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Context.SetVar("nameList", nameList);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test23_where_notin_select_2()
		{
			var s = @"from list where Name not in (select Name from nameList)";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var nameList = new[] { new { Name = "jim" }, new { Name = "tom" } };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Context.SetVar("nameList", nameList);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test23_where_notin_select()
		{
			var s = @"from list where Name not in (select Name from nameList)";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var nameList = new[] { new { Name = "jim" }, new { Name = "tom" } };
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Context.SetVar("nameList", nameList);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test22_where_notin_4()
		{
			var s = @"from list where not Name in ('jim', 'tom')";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test22_where_notin_3()
		{
			var s = @"from list where not Name in ('jim', 'tom')";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test22_where_notin_2()
		{
			var s = @"from list where Name not in ('jim', 'tom')";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test22_where_notin()
		{
			var s = @"from list where Name not in ('jim', 'tom')";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("san", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test21_where_in_select_2()
		{
			var s = @"from list where Name in (select Name from nameList)";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var nameList = new[] { new { Name = "jim" }, new { Name = "tom" } };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Context.SetVar("nameList", nameList);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("jim", result[1].Name);
		}

		[TestMethod]
		public void Test21_where_in_select()
		{
			var s = @"from list where Name in (select Name from nameList)";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var nameList = new[] { new { Name = "jim" }, new { Name = "tom" } };
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Context.SetVar("nameList", nameList);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("jim", result[1].Name);
		}

		[TestMethod]
		public void Test20_where_in_2()
		{
			var s = @"from list where Name in ('jim', 'tom')";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("jim", result[1].Name);
		}

		[TestMethod]
		public void Test20_where_in()
		{
			var s = @"FROM list WHERE Name IN ('jim', 'tom')";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("tom", result[0].Name);
			Assert.AreEqual("jim", result[1].Name);
		}

		[TestMethod]
		public void Test19_groupby_2()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
			set q = select Age as Age2, count(1) as Count
					from q1
					group by Age;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(3, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(20, d0.Age2);
			Assert.AreEqual(1, d0.Count);
			dynamic d1 = list[1];
			Assert.AreEqual(25, d1.Age2);
			Assert.AreEqual(2, d1.Count);
			dynamic d2 = list[2];
			Assert.AreEqual(18, d2.Age2);
			Assert.AreEqual(1, d2.Count);
		}

		[TestMethod]
		public void Test19_groupby()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
			set q = select Age as Age2, count(1) as Count
					from q1
					group by Age;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(3, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(20, d0.Age2);
			Assert.AreEqual(1, d0.Count);
			dynamic d1 = list[1];
			Assert.AreEqual(25, d1.Age2);
			Assert.AreEqual(2, d1.Count);
			dynamic d2 = list[2];
			Assert.AreEqual(18, d2.Age2);
			Assert.AreEqual(1, d2.Count);
		}

		[TestMethod]
		public void Test18_select_2()
		{
			var s = @"from list where age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s);
			Console.WriteLine(result.GetType());
			int i = 0;
			foreach (var item in result)
			{
				if (i == 0)
				{
					Assert.AreEqual("jim", item.Name);
				}
				else if (i == 1)
				{
					Assert.AreEqual("qin", item.Name);
				}
				else
				{
					throw new Exception();
				}
				i++;
			}
		}

		[TestMethod]
		public void Test18_select()
		{
			var s = @"from list where age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s);
			Console.WriteLine(result.GetType());
			int i = 0;
			foreach (var item in result)
			{
				if (i == 0)
				{
					Assert.AreEqual("jim", item.Name);
				}
				else if (i == 1)
				{
					Assert.AreEqual("qin", item.Name);
				}
				else
				{
					throw new Exception();
				}
				i++;
			}
		}

		[TestMethod]
		public void Test17_select_2()
		{
			var s = @"select Name, Age from list where age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable>(s);
			var itemType = Script.AnonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var listType = typeof(IEnumerable<>).MakeGenericType(itemType);
			Assert.IsTrue(listType.IsAssignableFrom(result.GetType()));
			Console.WriteLine(result.GetType());
			int i = 0;
			foreach (var item in result)
			{
				if (i == 0)
				{
					Assert.AreEqual("jim", ((dynamic)item).Name);
				}
				else if (i == 1)
				{
					Assert.AreEqual("qin", ((dynamic)item).Name);
				}
				else
				{
					throw new Exception();
				}
				i++;
			}
		}

		[TestMethod]
		public void Test17_select()
		{
			var s = @"select Name, Age from list where age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable>(s);
			var itemType = Script.AnonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var listType = typeof(IEnumerable<>).MakeGenericType(itemType);
			Assert.IsTrue(listType.IsAssignableFrom(result.GetType()));
			Console.WriteLine(result.GetType());
			int i = 0;
			foreach (var item in result)
			{
				if (i == 0)
				{
					Assert.AreEqual("jim", ((dynamic)item).Name);
				}
				else if (i == 1)
				{
					Assert.AreEqual("qin", ((dynamic)item).Name);
				}
				else
				{
					throw new Exception();
				}
				i++;
			}
		}

		[TestMethod]
		public void Test16_group_2()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
			set q = select a.Age as Age2, count(1) as Count
					from q1 as a
					group by a.Age;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(3, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(20, d0.Age2);
			Assert.AreEqual(1, d0.Count);
			dynamic d1 = list[1];
			Assert.AreEqual(25, d1.Age2);
			Assert.AreEqual(2, d1.Count);
			dynamic d2 = list[2];
			Assert.AreEqual(18, d2.Age2);
			Assert.AreEqual(1, d2.Count);
		}

		[TestMethod]
		public void Test16_group()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
			set q = select a.Age as Age2, count(1) as Count
					from q1 as a
					group by a.Age;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(3, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(20, d0.Age2);
			Assert.AreEqual(1, d0.Count);
			dynamic d1 = list[1];
			Assert.AreEqual(25, d1.Age2);
			Assert.AreEqual(2, d1.Count);
			dynamic d2 = list[2];
			Assert.AreEqual(18, d2.Age2);
			Assert.AreEqual(1, d2.Count);
		}

		[TestMethod]
		public void Test15_group_2()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
			set q = select a.Age, count(1) as Count
					from q1 as a
					group by a.Age;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(3, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(20, d0.Age);
			Assert.AreEqual(1, d0.Count);
			dynamic d1 = list[1];
			Assert.AreEqual(25, d1.Age);
			Assert.AreEqual(2, d1.Count);
			dynamic d2 = list[2];
			Assert.AreEqual(18, d2.Age);
			Assert.AreEqual(1, d2.Count);
		}

		[TestMethod]
		public void Test15_group()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
			set q = select a.Age, count(1) as Count
					from q1 as a
					group by a.Age;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(3, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(20, d0.Age);
			Assert.AreEqual(1, d0.Count);
			dynamic d1 = list[1];
			Assert.AreEqual(25, d1.Age);
			Assert.AreEqual(2, d1.Count);
			dynamic d2 = list[2];
			Assert.AreEqual(18, d2.Age);
			Assert.AreEqual(1, d2.Count);
		}

		[TestMethod]
		public void Test14_rightjoin_2()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
			string s = @"
			set q = select b.Name, b.Age, a?.Address
					from q2 as a
					right join q1 as b on a.UserName = b.Name;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(4, list.Count);
			var type = Script.AnonymousTypes.CreateType(new[] { "Name", "Age", "Address" }, new[] { typeof(string), typeof(int), typeof(string) });
			var listType = typeof(List<>).MakeGenericType(type);
			Assert.IsInstanceOfType(list, listType);
			dynamic d0 = list[0];
			Assert.AreEqual("tom", d0.Name);
			Assert.AreEqual(20, d0.Age);
			Assert.AreEqual("c", d0.Address);
			dynamic d1 = list[1];
			Assert.AreEqual("jim", d1.Name);
			Assert.AreEqual(25, d1.Age);
			Assert.AreEqual("a", d1.Address);
			dynamic d2 = list[2];
			Assert.AreEqual("san", d2.Name);
			Assert.AreEqual(18, d2.Age);
			Assert.IsNull(d2.Address);
			dynamic d3 = list[3];
			Assert.AreEqual("kit", d3.Name);
			Assert.AreEqual(30, d3.Age);
			Assert.IsNull(d3.Address);
		}

		[TestMethod]
		public void Test14_rightjoin()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
			string s = @"
			set q = select b.Name, b.Age, a?.Address
					from q2 as a
					right join q1 as b on a.UserName = b.Name;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(4, list.Count);
			var type = Script.AnonymousTypes.CreateType(new[] { "Name", "Age", "Address" }, new[] { typeof(string), typeof(int), typeof(string) });
			var listType = typeof(List<>).MakeGenericType(type);
			Assert.IsInstanceOfType(list, listType);
			dynamic d0 = list[0];
			Assert.AreEqual("tom", d0.Name);
			Assert.AreEqual(20, d0.Age);
			Assert.AreEqual("c", d0.Address);
			dynamic d1 = list[1];
			Assert.AreEqual("jim", d1.Name);
			Assert.AreEqual(25, d1.Age);
			Assert.AreEqual("a", d1.Address);
			dynamic d2 = list[2];
			Assert.AreEqual("san", d2.Name);
			Assert.AreEqual(18, d2.Age);
			Assert.IsNull(d2.Address);
			dynamic d3 = list[3];
			Assert.AreEqual("kit", d3.Name);
			Assert.AreEqual(30, d3.Age);
			Assert.IsNull(d3.Address);
		}

		[TestMethod]
		public void Test13_leftjoin_4()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
			string s = @"
@lang sql
select a.Name, a.Age, b?.Address
from q1 as a
left join q2 as b on a.Name = b.UserName
where a.age > 22
order by a.age desc
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("kit", list[0].Name);
			Assert.AreEqual(30, list[0].Age);
			Assert.IsNull(list[0].Address);
			Assert.AreEqual("jim", list[1].Name);
			Assert.AreEqual(25, list[1].Age);
			Assert.AreEqual("a", list[1].Address);
		}

		[TestMethod]
		public void Test13_leftjoin_3()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
			string s = @"
@lang sql
select a.Name, a.Age, b?.Address
from q1 as a
left join q2 as b on a.Name = b.UserName
where a.age > 22
order by a.age desc
";
			var script = new Script();
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("kit", list[0].Name);
			Assert.AreEqual(30, list[0].Age);
			Assert.IsNull(list[0].Address);
			Assert.AreEqual("jim", list[1].Name);
			Assert.AreEqual(25, list[1].Age);
			Assert.AreEqual("a", list[1].Address);
		}

		[TestMethod]
		public void Test13_leftjoin_2()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
			string s = @"
			set q = select a.Name, a.Age, b?.Address
					from q1 as a
					left join q2 as b on a.Name = b.UserName;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(4, list.Count);
			var type = Script.AnonymousTypes.CreateType(new[] { "Name", "Age", "Address" }, new[] { typeof(string), typeof(int), typeof(string) });
			var listType = typeof(List<>).MakeGenericType(type);
			Assert.IsInstanceOfType(list, listType);
			dynamic d0 = list[0];
			Assert.AreEqual("tom", d0.Name);
			Assert.AreEqual(20, d0.Age);
			Assert.AreEqual("c", d0.Address);
			dynamic d1 = list[1];
			Assert.AreEqual("jim", d1.Name);
			Assert.AreEqual(25, d1.Age);
			Assert.AreEqual("a", d1.Address);
			dynamic d2 = list[2];
			Assert.AreEqual("san", d2.Name);
			Assert.AreEqual(18, d2.Age);
			Assert.IsNull(d2.Address);
			dynamic d3 = list[3];
			Assert.AreEqual("kit", d3.Name);
			Assert.AreEqual(30, d3.Age);
			Assert.IsNull(d3.Address);
		}

		[TestMethod]
		public void Test13_leftjoin()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
			string s = @"
set q = select a.Name, a.Age, b?.Address
		from q1 as a
		left join q2 as b on a.Name = b.UserName;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(4, list.Count);
			var type = Script.AnonymousTypes.CreateType(new[] { "Name", "Age", "Address" }, new[] { typeof(string), typeof(int), typeof(string) });
			var listType = typeof(List<>).MakeGenericType(type);
			Assert.IsInstanceOfType(list, listType);
			dynamic d0 = list[0];
			Assert.AreEqual("tom", d0.Name);
			Assert.AreEqual(20, d0.Age);
			Assert.AreEqual("c", d0.Address);
			dynamic d1 = list[1];
			Assert.AreEqual("jim", d1.Name);
			Assert.AreEqual(25, d1.Age);
			Assert.AreEqual("a", d1.Address);
			dynamic d2 = list[2];
			Assert.AreEqual("san", d2.Name);
			Assert.AreEqual(18, d2.Age);
			Assert.IsNull(d2.Address);
			dynamic d3 = list[3];
			Assert.AreEqual("kit", d3.Name);
			Assert.AreEqual(30, d3.Age);
			Assert.IsNull(d3.Address);
		}

		[TestMethod]
		public void Test12_thenby_2()
		{
			// ascending
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
set q = from q1 as a
		order by a.Age asc, a.Name desc
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(4, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(18, d0.Age);
			Assert.AreEqual("san", d0.Name);
			dynamic d1 = list[1];
			Assert.AreEqual(20, d1.Age);
			Assert.AreEqual("tom", d1.Name);
			dynamic d2 = list[2];
			Assert.AreEqual(25, d2.Age);
			Assert.AreEqual("kit", d2.Name);
			dynamic d3 = list[3];
			Assert.AreEqual(25, d3.Age);
			Assert.AreEqual("jim", d3.Name);
		}

		[TestMethod]
		public void Test12_thenby()
		{
			// ascending
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
set q = from q1 as a
		order by a.Age asc, a.Name desc
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(4, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(18, d0.Age);
			Assert.AreEqual("san", d0.Name);
			dynamic d1 = list[1];
			Assert.AreEqual(20, d1.Age);
			Assert.AreEqual("tom", d1.Name);
			dynamic d2 = list[2];
			Assert.AreEqual(25, d2.Age);
			Assert.AreEqual("kit", d2.Name);
			dynamic d3 = list[3];
			Assert.AreEqual(25, d3.Age);
			Assert.AreEqual("jim", d3.Name);
		}

		[TestMethod]
		public void Test11_orderby_2()
		{
			// ascending
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
set q = from q1 as a
		order by a.Age asc
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(4, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(18, d0.Age);
			Assert.AreEqual("san", d0.Name);
			dynamic d1 = list[1];
			Assert.AreEqual(20, d1.Age);
			Assert.AreEqual("tom", d1.Name);
			dynamic d2 = list[2];
			Assert.AreEqual(25, d2.Age);
			Assert.AreEqual("jim", d2.Name);
			dynamic d3 = list[3];
			Assert.AreEqual(25, d3.Age);
			Assert.AreEqual("kit", d3.Name);
		}

		[TestMethod]
		public void Test11_orderby()
		{
			// ascending
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();

			string s = @"
set q = from q1 as a
		order by a.Age asc
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("q1", q1);
			script.Context.SetVar("q2", q2);
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			var list = script.Eval<IList>("q.ToList()");
			Assert.AreEqual(4, list.Count);
			dynamic d0 = list[0];
			Assert.AreEqual(18, d0.Age);
			Assert.AreEqual("san", d0.Name);
			dynamic d1 = list[1];
			Assert.AreEqual(20, d1.Age);
			Assert.AreEqual("tom", d1.Name);
			dynamic d2 = list[2];
			Assert.AreEqual(25, d2.Age);
			Assert.AreEqual("jim", d2.Name);
			dynamic d3 = list[3];
			Assert.AreEqual(25, d3.Age);
			Assert.AreEqual("kit", d3.Name);
		}

		[TestMethod]
		public void Test10_select_2()
		{
			var s = @"select a.Name as Name2, a.Age from list as a where a.age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable>(s);
			var itemType = Script.AnonymousTypes.CreateType(new[] { "Name2", "Age" }, new[] { typeof(string), typeof(int) });
			var listType = typeof(IEnumerable<>).MakeGenericType(itemType);
			Assert.IsTrue(listType.IsAssignableFrom(result.GetType()));
			Console.WriteLine(result.GetType());
			int i = 0;
			foreach (var item in result)
			{
				if (i == 0)
				{
					Assert.AreEqual("jim", ((dynamic)item).Name2);
				}
				else if (i == 1)
				{
					Assert.AreEqual("qin", ((dynamic)item).Name2);
				}
				else
				{
					throw new Exception();
				}
				i++;
			}
		}

		[TestMethod]
		public void Test10_select()
		{
			var s = @"select a.Name as Name2, a.Age from list as a where a.age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable>(s);
			var itemType = Script.AnonymousTypes.CreateType(new[] { "Name2", "Age" }, new[] { typeof(string), typeof(int) });
			var listType = typeof(IEnumerable<>).MakeGenericType(itemType);
			Assert.IsTrue(listType.IsAssignableFrom(result.GetType()));
			Console.WriteLine(result.GetType());
			int i = 0;
			foreach (var item in result)
			{
				if (i == 0)
				{
					Assert.AreEqual("jim", ((dynamic)item).Name2);
				}
				else if (i == 1)
				{
					Assert.AreEqual("qin", ((dynamic)item).Name2);
				}
				else
				{
					throw new Exception();
				}
				i++;
			}
		}

		[TestMethod]
		public void Test09_select_4()
		{
			var s = @"select Name, Age from list where age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual(10, result[0].Age);
			Assert.AreEqual("qin", result[1].Name);
			Assert.AreEqual(10, result[1].Age);
		}

		[TestMethod]
		public void Test09_select_3()
		{
			var s = @"select Name, Age from list where age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual(10, result[0].Age);
			Assert.AreEqual("qin", result[1].Name);
			Assert.AreEqual(10, result[1].Age);
		}

		[TestMethod]
		public void Test09_select_2()
		{
			var s = @"select a.Name, a.Age from list as a where a.age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable>(s);
			var itemType = Script.AnonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var listType = typeof(IEnumerable<>).MakeGenericType(itemType);
			Assert.IsTrue(listType.IsAssignableFrom(result.GetType()));
			Console.WriteLine(result.GetType());
			int i = 0;
			foreach (var item in result)
			{
				if (i == 0)
				{
					Assert.AreEqual("jim", ((dynamic)item).Name);
				}
				else if (i == 1)
				{
					Assert.AreEqual("qin", ((dynamic)item).Name);
				}
				else
				{
					throw new Exception();
				}
				i++;
			}
		}

		[TestMethod]
		public void Test09_select()
		{
			var s = @"select a.Name, a.Age from list as a where a.age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable>(s);
			var itemType = Script.AnonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var listType = typeof(IEnumerable<>).MakeGenericType(itemType);
			Assert.IsTrue(listType.IsAssignableFrom(result.GetType()));
			Console.WriteLine(result.GetType());
			int i = 0;
			foreach (var item in result)
			{
				if (i == 0)
				{
					Assert.AreEqual("jim", ((dynamic)item).Name);
				}
				else if (i == 1)
				{
					Assert.AreEqual("qin", ((dynamic)item).Name);
				}
				else
				{
					throw new Exception();
				}
				i++;
			}
		}

		[TestMethod]
		public void Test08_where_2()
		{
			var s = @"from list as a where a.age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test08_where()
		{
			var s = @"from list as a where a.age=10";
			var list = new[]
			{
				new Person("tom", 15),
				new Person("jim", 10),
				new Person("san", 20),
				new Person("qin", 10)
			};
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval<IEnumerable<Person>>(s).ToList();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual("qin", result[1].Name);
		}

		[TestMethod]
		public void Test07_lang_2()
		{
			string s = @"
bool a = #lang sql age=10 #end
a = !a;
a ? 1 : 2;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("age", 10);
			Assert.AreEqual(2, script.Eval(s));
			script.Context.SetVar("age", 12);
			Assert.AreEqual(1, script.Eval(s));
			script.Context.SetVar("age", 10);
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test07_lang()
		{
			string s = @"
bool a = #lang sql age=10 #end
a = !a;
a ? 1 : 2;
";
			var script = new Script();
			script.Context.SetVar("age", 10);
			Assert.AreEqual(2, script.Eval(s));
			script.Context.SetVar("age", 12);
			Assert.AreEqual(1, script.Eval(s));
			script.Context.SetVar("age", 10);
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test06_lang_2()
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
		public void Test06_lang()
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
		public void Test05_lang_2()
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
		public void Test05_lang()
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
		public void Test04_lang_2()
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
		public void Test04_lang()
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
		public void Test03_lang_2()
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
		public void Test03_lang()
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
		public void Test01_like_6()
		{
			string s = "p.Age>20 AND p.Age<50 Or p.Name like 'to%' OR p.Name='san'";
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
		public void Test01_like_5()
		{
			string s = "p.Age>20 AND p.Age<50 Or p.Name like 'to%' OR p.Name='san'";
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

		[TestMethod]
		public void Test01_like_4()
		{
			string s = "p.Age>20 AND p.Age<50 OR p.Name like 'to%' OR p.Name='san'";
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
		public void Test01_like_3()
		{
			string s = "p.Age>20 AND p.Age<50 OR p.Name like 'to%' OR p.Name='san'";
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

		[TestMethod]
		public void Test01_like_2()
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
		public void Test01_like()
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
