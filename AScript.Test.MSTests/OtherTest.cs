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
		public void Test02()
		{
			int n = 10;
			var sum = (int a, int b) =>
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

		[TestMethod]
		public void Test01()
		{
			var s = "hello";
			var span = s.AsSpan(1, 2);
			Assert.IsTrue(span.SequenceEqual("el"));
		}
	}
}
