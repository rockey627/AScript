using AScript.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptLinqDynamicTest
	{
		[TestMethod]
		public void Test01_context()
		{
			var context = new ScriptContext();
			context.SetVar("n", 3);
			context.AddFunc<int, bool>("isOdd", a => a % 2 == 0);
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % n == 0 && isOdd(a)", context).ToArray();
			CollectionAssert.AreEqual(new[] { 6 }, result);
		}

		[TestMethod]
		public void Test02_args()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % @0 == 0",3).ToArray();
			CollectionAssert.AreEqual(new[] { 3, 6 }, result);
		}

		[TestMethod]
		public void Test03_arg0_value()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % @0.Value == 0", new { Value = 3 }).ToArray();
			CollectionAssert.AreEqual(new[] { 3, 6 }, result);
		}

		[TestMethod]
		public void Test03_arg0_value2()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % Value == 0", new { Value = 3 }).ToArray();
			CollectionAssert.AreEqual(new[] { 3, 6 }, result);
		}

		[TestMethod]
		public void Test03_arg0_value3()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % Age == 0", new Person { Age = 3 }).ToArray();
			CollectionAssert.AreEqual(new[] { 3, 6 }, result);
		}

		[TestMethod]
		public void Test04_args()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % @0 == 0  && a > @1", new[] { 2, 3 }).ToArray();
			CollectionAssert.AreEqual(new[] { 4, 6 }, result);
		}

		[TestMethod]
		public void Test04_args1()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % @0 == 0  && a > @1", new object[] { 2, 3 }).ToArray();
			CollectionAssert.AreEqual(new[] { 4, 6 }, result);
		}

		[TestMethod]
		public void Test04_args2()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % @0 == 0  && a > @1", new [] { 2, 3 }).ToArray();
			CollectionAssert.AreEqual(new[] { 4, 6 }, result);
		}

		[TestMethod]
		public void Test04_args3()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var result = arr.WhereScript(a => "a % @0.Age == 0  && a > @1.Age", new[] { new Person("", 2), new Person("", 3) }).ToArray();
			CollectionAssert.AreEqual(new[] { 4, 6 }, result);
		}

		[TestMethod]
		public void Test05_dict()
		{
			var arr = new[] { 1, 2, 3, 4, 5, 6 };
			var dict = new Dictionary<string, int>
			{
				["m"] = 2,
				["n"] = 3
			};
			var result = arr.WhereScript(a => "a % m == 0  && a > n", dict).ToArray();
			CollectionAssert.AreEqual(new[] { 4, 6 }, result);
		}

		[TestMethod]
		public void Test05_string()
		{
			var arr = new [] { "Tom", "San", "John" };
			var result = arr.WhereScript(a => "a == @0", "San").ToArray();
			CollectionAssert.AreEqual(new[] { "San" }, result);
		}

	}
}
