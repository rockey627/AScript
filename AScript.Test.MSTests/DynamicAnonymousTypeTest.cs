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
		public void Test08_2()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = true;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var v1 = Activator.CreateInstance(type, new object[] { null, 10 });
			var v2 = Activator.CreateInstance(type, new object[] { "tom", null });
			Assert.IsNotNull(v1);
			Assert.AreNotEqual(v1, v2);
			Assert.AreNotEqual(v2, v1);
			Assert.AreNotEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test08()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = false;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var v1 = Activator.CreateInstance(type, new object[] { null, 10 });
			var v2 = Activator.CreateInstance(type, new object[] { "tom", null });
			Assert.IsNotNull(v1);
			Assert.AreNotEqual(v1, v2);
			Assert.AreNotEqual(v2, v1);
			Assert.AreNotEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test07_2()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = true;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var v1 = Activator.CreateInstance(type, new object[] { null, null });
			var v2 = Activator.CreateInstance(type, new object[] { null, null });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test07()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = false;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var v1 = Activator.CreateInstance(type, new object[] { null, null });
			var v2 = Activator.CreateInstance(type, new object[] { null, null });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test06_2()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = true;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var v1 = Activator.CreateInstance(type, new object[] { null, 20 });
			var v2 = Activator.CreateInstance(type, new object[] { null, 20 });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test06()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = false;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var v1 = Activator.CreateInstance(type, new object[] { null, 20 });
			var v2 = Activator.CreateInstance(type, new object[] { null, 20 });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test05_2()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = true;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var v1 = Activator.CreateInstance(type, new object[] { null, 20 });
			var v2 = Activator.CreateInstance(type, new object[] { null, 20 });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test05()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = false;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var v1 = Activator.CreateInstance(type, new object[] { null, 20 });
			var v2 = Activator.CreateInstance(type, new object[] { null, 20 });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test04_2()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = true;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var v1 = Activator.CreateInstance(type, new object[] { "tom", 20 });
			var v2 = Activator.CreateInstance(type, new object[] { "tom", 20 });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test04()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = false;
			var type = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var v1 = Activator.CreateInstance(type, new object[] { "tom", 20 });
			var v2 = Activator.CreateInstance(type, new object[] { "tom", 20 });
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test03_2()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = true;
			var type = DynamicAnonymousType.CreateType(null, null);
			var v1 = Activator.CreateInstance(type);
			var v2 = Activator.CreateInstance(type);
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test03()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = false;
			var type = DynamicAnonymousType.CreateType(null, null);
			var v1 = Activator.CreateInstance(type);
			var v2 = Activator.CreateInstance(type);
			Assert.IsNotNull(v1);
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test02_2()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = true;
			var type1 = DynamicAnonymousType.CreateType(null, null);
			var type2 = DynamicAnonymousType.CreateType(null, null);
			Assert.AreEqual(0, type1.GetProperties().Length);
			Assert.AreEqual(type1, type2);
			Assert.AreEqual(type2, type1);
			Assert.IsFalse(type1.IsGenericType);
		}

		[TestMethod]
		public void Test02()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = false;
			var type1 = DynamicAnonymousType.CreateType(null, null);
			var type2 = DynamicAnonymousType.CreateType(null, null);
			Assert.AreEqual(0, type1.GetProperties().Length);
			Assert.AreEqual(type1, type2);
			Assert.AreEqual(type2, type1);
			Assert.IsFalse(type1.IsGenericType);
		}

		[TestMethod]
		public void Test01_2()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = true;
			var type1 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type2 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type3 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(long) });
			var type4 = DynamicAnonymousType.CreateType(new[] { "Name", "Height" }, new[] { typeof(string), typeof(int) });
			var type5 = DynamicAnonymousType.CreateType(new[] { "Name", "Age", "Height" }, new[] { typeof(string), typeof(int), typeof(int) });
			var type6 = DynamicAnonymousType.CreateType(new[] { "Name2", "Age" }, new[] { typeof(string), typeof(long) });
			Assert.AreNotEqual(DynamicAnonymousType.DefaultUseNonGenericAnonymousType, type1.IsGenericType);
			Assert.AreEqual(type1, type2);
			Assert.AreEqual(type2, type1);
			Assert.AreNotEqual(type1, type3);
			Assert.AreNotEqual(type3, type1);
			Assert.AreNotEqual(type1, type4);
			Assert.AreNotEqual(type4, type1);
			Assert.AreNotEqual(type1, type5);
			Assert.AreNotEqual(type5, type1);
			Assert.AreNotEqual(type1, type6);
			Assert.AreNotEqual(type6, type1);
			if (type1.IsGenericType)
			{
				Assert.AreEqual(type1.GetGenericTypeDefinition(), type3.GetGenericTypeDefinition());
				Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type4.GetGenericTypeDefinition());
				Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type5.GetGenericTypeDefinition());
				Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type6.GetGenericTypeDefinition());
			}
		}

		[TestMethod]
		public void Test01()
		{
			DynamicAnonymousType.DefaultUseNonGenericAnonymousType = false;
			var type1 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type2 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type3 = DynamicAnonymousType.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(long) });
			var type4 = DynamicAnonymousType.CreateType(new[] { "Name", "Height" }, new[] { typeof(string), typeof(int) });
			var type5 = DynamicAnonymousType.CreateType(new[] { "Name", "Age", "Height" }, new[] { typeof(string), typeof(int), typeof(int) });
			var type6 = DynamicAnonymousType.CreateType(new[] { "Name2", "Age" }, new[] { typeof(string), typeof(long) });
			Assert.AreNotEqual(DynamicAnonymousType.DefaultUseNonGenericAnonymousType, type1.IsGenericType);
			Assert.AreEqual(type1, type2);
			Assert.AreEqual(type2, type1);
			Assert.AreNotEqual(type1, type3);
			Assert.AreNotEqual(type3, type1);
			Assert.AreNotEqual(type1, type4);
			Assert.AreNotEqual(type4, type1);
			Assert.AreNotEqual(type1, type5);
			Assert.AreNotEqual(type5, type1);
			Assert.AreNotEqual(type1, type6);
			Assert.AreNotEqual(type6, type1);
			if (type1.IsGenericType)
			{
				Assert.AreEqual(type1.GetGenericTypeDefinition(), type3.GetGenericTypeDefinition());
				Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type4.GetGenericTypeDefinition());
				Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type5.GetGenericTypeDefinition());
				Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type6.GetGenericTypeDefinition());
			}
		}
	}
}
