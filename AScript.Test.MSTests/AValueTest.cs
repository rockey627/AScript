using AScript.Values;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class AValueTest
	{
		[TestMethod]
		public void Test01()
		{
			Assert.AreEqual(5, AValue.Create(5));
			//object obj = AValue.Create(5);
			//Assert.AreEqual(5, obj);
		}
	}
}
