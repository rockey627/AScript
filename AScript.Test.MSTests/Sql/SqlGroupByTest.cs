using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.Sql
{
	[TestClass]
	public class SqlGroupByTest
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
		public void Test01_2()
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

	}
}
