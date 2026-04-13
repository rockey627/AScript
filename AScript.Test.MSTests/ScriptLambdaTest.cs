using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptLambdaTest
	{
		[TestMethod]
		public void Test04()
		{
			var script = new Script();
			var expr = script.Lambda<Func<int>>("int test(int a,int b){ return a+b;}100 * test(5,5) * (6-2)", null);
			Assert.IsNotNull(expr);
			Console.WriteLine(expr.ToString());
			var func = expr.Compile();
			Assert.AreEqual(4000, func());
		}

		[TestMethod]
		public void Test03()
		{
			var script = new Script();
			var expr = script.Lambda<int, bool>("int n=10;a>0 && a<n", "a");
			Assert.IsNotNull(expr);
			Console.WriteLine(expr.ToString());
			var func = expr.Compile();
			Assert.IsTrue(func(5));
			Assert.IsFalse(func(-1));
			Assert.IsFalse(func(20));
		}

		[TestMethod]
		public void Test02()
		{
			var script = new Script();
			var expr = script.Lambda<int, bool>("a>0 && a<10", "a");
			Assert.IsNotNull(expr);
			Console.WriteLine(expr.ToString());
			var func = expr.Compile();
			Assert.IsTrue(func(5));
			Assert.IsFalse(func(-1));
			Assert.IsFalse(func(20));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			var expr = script.Lambda<int, bool>("a>0", "a");
			Assert.IsNotNull(expr);
			Console.WriteLine(expr.ToString());
			var func = expr.Compile();
			Assert.IsTrue(func(5));
			Assert.IsFalse(func(-1));
		}
	}
}
