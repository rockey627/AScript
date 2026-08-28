using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class OtherTest
	{
		[TestMethod]
		public void Test03()
		{
			Assert.AreEqual(0, new EvalResult(0));
			Assert.AreEqual(5, new EvalResult(5));
			Assert.AreEqual(5L, new EvalResult(5L));
			Assert.AreEqual(5.0, new EvalResult(5.0));
			Assert.AreEqual(5.6, new EvalResult(5.6));
			Assert.AreEqual(true, new EvalResult(true));
			Assert.AreEqual(false, new EvalResult(false));
			Assert.IsTrue(new EvalResult(true));
			Assert.IsFalse(new EvalResult(false));
			Assert.AreEqual("hello", new EvalResult("hello"));
		}

		[TestMethod]
		public void Test02()
		{
			int n = 10;
			Func<int, int, int> sum = (int a, int b) =>
			{
				n++;
				return a + b + n;
			};
			int m = sum(1, 2);
			Console.WriteLine(m + " " + n);
			n = 20;
			m = sum(1, 2);
			Console.WriteLine(m + " " + n);
		}

#if NET7_0_OR_GREATER
		[TestMethod]
		public void Test01()
		{
			var s = "hello";
			var span = s.AsSpan(1, 2);
			Assert.IsTrue(span.SequenceEqual("el"));
		}
#endif
	}
}
