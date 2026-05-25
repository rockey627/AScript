using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class Lang_SqlTest
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

//		[TestMethod]
//		public void Test15_group_2()
//		{
//			var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 25) }.AsQueryable();
//			var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") }.AsQueryable();
			
//			string s = @"
//			set q = select a.Age, count(1) as Count
//					from q1 as a
//					group by a.Age;
//";
//			var script = new Script();
//			script.Options.CompileMode = ECompileMode.All;
//			script.Context.Langs = new[] { "sql" };
//			script.Context.SetVar("q1", q1);
//			var r = script.Eval(s);
//			Console.WriteLine(r.ToString());
//			var list = script.Eval<IList>("q.ToList()");
//			Assert.AreEqual(3, list.Count);
//			dynamic d0 = list[0];
//			Assert.AreEqual(20, d0.Key);
//			Assert.AreEqual(1, d0.Count);
//			dynamic d1 = list[1];
//			Assert.AreEqual(25, d1.Key);
//			Assert.AreEqual(2, d1.Count);
//			dynamic d2 = list[2];
//			Assert.AreEqual(18, d2.Key);
//			Assert.AreEqual(1, d2.Count);
//		}

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
		public void Test01_6()
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
		public void Test01_5()
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
		public void Test01_4()
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
		public void Test01_3()
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
