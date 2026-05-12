using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class DynamicAnonymousTypeTest
	{
		[TestMethod]
		public void Test04()
		{
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var v1 = Activator.CreateInstance(type, new object[] { "tom", 20 });
			var v2 = Activator.CreateInstance(type, new object[] { "tom", 20 });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test03()
		{
			var type = DynamicAnonymousType.CreateType(null, null);
			var v1 = Activator.CreateInstance(type);
			var v2 = Activator.CreateInstance(type);
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test02()
		{
			var type1 = DynamicAnonymousType.CreateType(null, null);
			var type2 = DynamicAnonymousType.CreateType(null, null);
			Assert.AreEqual(0, type1.GetProperties().Length);
			Assert.AreEqual(type1, type2);
			Assert.IsFalse(type1.IsGenericType);
		}

		[TestMethod]
		public void Test01()
		{
			var type1 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type2 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type3 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(long) });
			var type4 = DynamicAnonymousType.CreateType(new[] { "Name", "Height" }, new[] { typeof(string), typeof(int) });
			var type5 = DynamicAnonymousType.CreateType(new[] { "Name", "Age", "Height" }, new[] { typeof(string), typeof(int), typeof(int) });
			Assert.IsTrue(type1.IsGenericType);
			Assert.AreEqual(type1, type2);
			Assert.AreNotEqual(type1, type3);
			Assert.AreEqual(type1.GetGenericTypeDefinition(), type3.GetGenericTypeDefinition());
			Assert.AreNotEqual(type1, type4);
			Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type4.GetGenericTypeDefinition());
			Assert.AreNotEqual(type1, type5);
			Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type5.GetGenericTypeDefinition());
		}
	}
}
