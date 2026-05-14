using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptLinqTest
	{
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
		}
	}
}
