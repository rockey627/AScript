using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptLinqTest
	{
		[TestMethod]
		public void Test03_2()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) }.AsEnumerable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsEnumerable();
			var q = from a in q1
					join bb in q2 on a.Name equals bb.UserName into bbb
					from b in bbb.DefaultIfEmpty()
					select new { a.Name, a.Age, b?.Address };
			Console.WriteLine(q.ToString());
			q.ToList();

			string s = @"
			var q1 = new[] { new Person(""tom"", 20), new Person(""jim"", 25), new Person(""san"", 18), new Person(""kit"", 30) }.AsEnumerable();
			var q2 = new[] { new AddressInfo(""jim"", ""a""), new AddressInfo(""cc"", ""b""), new AddressInfo(""tom"", ""c""), new AddressInfo(""ee"", ""d"") }.AsEnumerable();
			var q = from a in q1
					join bb in q2 on a.Name equals bb.UserName into bbb
					from b in bbb.DefaultIfEmpty()
					select new { a.Name, a.Age, b?.Address };
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.AddType<AddressInfo>();
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
		public void Test03_1()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
			var q = from a in q1
					join bb in q2 on a.Name equals bb.UserName into bbb
					from b in bbb.DefaultIfEmpty()
					select new { a.Name, a.Age, b?.Address };
			q.ToList();

			Expression<Func<Person, string>> lambda = p => p == null ? null : p.Name;
			Console.WriteLine(lambda.ToString());
		}

		[TestMethod]
		public void Test03_0()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();
			var q = from a in q1
					group a.Name by a.Age into g
					select new { g.Key, Count = g.Count() };
			//q.ToList();
			Console.WriteLine(q.ToString());

			var qq = q1.GroupBy(a => a.Age, a => a.Name)
				.Select(g => new { g.Key, Count = g.Count() });
			Console.WriteLine(q.ToString());
		}

		[TestMethod]
		public void Test03()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
			var q = from a in q1
					join bb in q2 on a.Name equals bb.UserName into bbb
					from b in bbb.DefaultIfEmpty()
					select new { a.Name, a.Age, b?.Address };
			Console.WriteLine(q.ToString());
			q.ToList();

			string s = @"
			var q1 = new[] { new Person(""tom"", 20), new Person(""jim"", 25), new Person(""san"", 18), new Person(""kit"", 30) };
			var q2 = new[] { new AddressInfo(""jim"", ""a""), new AddressInfo(""cc"", ""b""), new AddressInfo(""tom"", ""c""), new AddressInfo(""ee"", ""d"") };
			var q = from a in q1
					join bb in q2 on a.Name equals bb.UserName into bbb
					from b in bbb.DefaultIfEmpty()
					select new { a.Name, a.Age, b?.Address };
";
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.AddType<AddressInfo>();
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
		public void Test02_2()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) }.AsQueryable();
			var q2 = new[] { new Person("zh", 22), new Person("cc", 18), new Person("aa", 20), new Person("ee", 27) }.AsQueryable();
			var q = from a in q1
					join b in q2 on a.Age equals b.Age
					where a.Age == b.Age
					select new { a.Name, b.Age };
			Console.WriteLine(q.ToString());

			string s = @"
			var q1 = new[] { new Person(""tom"", 20), new Person(""jim"", 25), new Person(""san"", 18), new Person(""kit"", 30) }.AsQueryable();
			var q2 = new[] { new Person(""zh"", 22), new Person(""cc"", 18), new Person(""aa"", 20), new Person(""ee"", 27) }.AsQueryable();
			var q = from a in q1
					join b in q2 on a.Age equals b.Age
					where a.Age == b.Age
					select new { a.Name, b.Age };
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			script.Eval("q.ToList()");
		}

		[TestMethod]
		public void Test02()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) }.AsQueryable();
			var q2 = new[] { new Person("zh", 22), new Person("cc", 18), new Person("aa", 20), new Person("ee", 27) }.AsQueryable();
			var q = from a in q1
					join b in q2 on a.Age equals b.Age
					where a.Age == b.Age
					select new { a.Name, b.Age };
			Console.WriteLine(q.ToString());

			string s = @"
			var q1 = new[] { new Person(""tom"", 20), new Person(""jim"", 25), new Person(""san"", 18), new Person(""kit"", 30) }.AsQueryable();
			var q2 = new[] { new Person(""zh"", 22), new Person(""cc"", 18), new Person(""aa"", 20), new Person(""ee"", 27) }.AsQueryable();
			var q = from a in q1
					join b in q2 on a.Age equals b.Age
					where a.Age == b.Age
					select new { a.Name, b.Age };
";
			var script = new Script();
			script.Context.AddType<Person>();
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			script.Eval("q.ToList()");
		}

		[TestMethod]
		public void Test01_2()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) }.AsQueryable();
			var q2 = new[] { new Person("zh", 22), new Person("cc", 18), new Person("aa", 20), new Person("ee", 27) }.AsQueryable();
			var q3 = new[] { new Person("zh", 22), new Person("cc", 18), new Person("aa", 20), new Person("ee", 27) }.AsQueryable();
			var q = from a in q1
					where a.Age > 20
					from b in q2
					where b.Age > 18
					where a.Age == b.Age
					from c in q3
					where c.Age > 20
					where c.Age == a.Age
					select new { a.Name, b.Age };
			Console.WriteLine(q.ToString());
			var list = q.ToList();

			string s = @"
			var q1 = new Person[] { new Person(""tom"", 20), new Person(""jim"", 25), new Person(""san"", 18), new Person(""kit"", 30) }.AsQueryable();
			var q2 = new Person[] { new Person(""zh"", 22), new Person(""cc"", 18), new Person(""aa"", 20), new Person(""ee"", 27) }.AsQueryable();
			var q3 = new Person[] { new Person(""zh"", 22), new Person(""cc"", 18), new Person(""aa"", 20), new Person(""ee"", 27) }.AsQueryable();
			var q = from a in q1
					where a.Age > 20
					from b in q2
					where b.Age > 18
					where a.Age == b.Age
					from c in q3
					where c.Age > 20
					where c.Age == a.Age
					select new { a.Name, b.Age };
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			script.Eval("q.ToList()");
		}

		[TestMethod]
		public void Test01()
		{
			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) }.AsQueryable();
			var q2 = new[] { new Person("zh", 22), new Person("cc", 18), new Person("aa", 20), new Person("ee", 27) }.AsQueryable();
			var q3 = new[] { new Person("zh", 22), new Person("cc", 18), new Person("aa", 20), new Person("ee", 27) }.AsQueryable();
			var q = from a in q1
					where a.Age > 20
					from b in q2
					where b.Age > 18
					where a.Age == b.Age
					from c in q3
					where c.Age > 20
					where c.Age == a.Age
					select new { a.Name, b.Age };
			Console.WriteLine(q.ToString());
			var list = q.ToList();

			string s = @"
			var q1 = new Person[] { new Person(""tom"", 20), new Person(""jim"", 25), new Person(""san"", 18), new Person(""kit"", 30) }.AsQueryable();
			var q2 = new Person[] { new Person(""zh"", 22), new Person(""cc"", 18), new Person(""aa"", 20), new Person(""ee"", 27) }.AsQueryable();
			var q3 = new Person[] { new Person(""zh"", 22), new Person(""cc"", 18), new Person(""aa"", 20), new Person(""ee"", 27) }.AsQueryable();
			var q = from a in q1
					where a.Age > 20
					from b in q2
					where b.Age > 18
					where a.Age == b.Age
					from c in q3
					where c.Age > 20
					where c.Age == a.Age
					select new { a.Name, b.Age };
";
			var script = new Script();
			script.Context.AddType<Person>();
			var r = script.Eval(s);
			Console.WriteLine(r.ToString());
			script.Eval("q.ToList()");
		}
	}
}
