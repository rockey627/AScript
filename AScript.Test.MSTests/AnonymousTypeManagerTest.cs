using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class AnonymousTypeManagerTest
	{
		[TestMethod]
		public void Test09_3()
		{
			var anonymousTypes1 = new AnonymousTypeManager { DefaultUseGeneric = true };
			var anonymousTypes2 = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type1 = anonymousTypes1.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var type2 = anonymousTypes2.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsTrue(type1.IsGenericType);
			Assert.IsFalse(type2.IsGenericType);
			Assert.AreNotEqual(type1, type2);
			var v1 = Activator.CreateInstance(type1, new object[] { "tom", 10 });
			var v2 = Activator.CreateInstance(type2, new object[] { "tom", 10 });
			Assert.IsNotNull(v1);
			Assert.IsNotNull(v2);
			Assert.AreEqual("{ Name = tom, Age = 10 }", v1.ToString());
			Assert.AreNotEqual(v1, v2);
			Assert.AreNotEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
			var set = new HashSet<object>();
			set.Add(v1);
			set.Add(v2);
			Assert.AreEqual(2, set.Count);
		}

		[TestMethod]
		public void Test09_2()
		{
			var anonymousTypes1 = new AnonymousTypeManager { DefaultUseGeneric = false };
			var anonymousTypes2 = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type1 = anonymousTypes1.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var type2 = anonymousTypes2.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsFalse(type1.IsGenericType);
			Assert.IsFalse(type2.IsGenericType);
			Assert.AreNotEqual(type1, type2);
			var v1 = Activator.CreateInstance(type1, new object[] { "tom", 10 });
			var v2 = Activator.CreateInstance(type2, new object[] { "tom", 10 });
			Assert.IsNotNull(v1);
			Assert.IsNotNull(v2);
			Assert.AreNotEqual(v1, v2);
			Assert.AreNotEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
			var set = new HashSet<object>();
			set.Add(v1);
			set.Add(v2);
			Assert.AreEqual(2, set.Count);
		}

		[TestMethod]
		public void Test09()
		{
			var anonymousTypes1 = new AnonymousTypeManager { DefaultUseGeneric = true };
			var anonymousTypes2 = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type1 = anonymousTypes1.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			var type2 = anonymousTypes2.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsTrue(type1.IsGenericType);
			Assert.IsTrue(type2.IsGenericType);
			Assert.AreNotEqual(type1, type2);
			var v1 = Activator.CreateInstance(type1, new object[] { "tom", 10 });
			var v2 = Activator.CreateInstance(type2, new object[] { "tom", 10 });
			Assert.IsNotNull(v1);
			Assert.IsNotNull(v2);
			Assert.AreNotEqual(v1, v2);
			Assert.AreNotEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
			var set = new HashSet<object>();
			set.Add(v1);
			set.Add(v2);
			Assert.AreEqual(2, set.Count);
		}

		[TestMethod]
		public void Test08_2()
		{
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsFalse(type.IsGenericType);
			var v1 = Activator.CreateInstance(type, new object[] { null, 10 });
			var v2 = Activator.CreateInstance(type, new object[] { "tom", null });
			Assert.IsNotNull(v1);
			Assert.AreEqual("{ Name = , Age = 10 }", v1.ToString());
			Assert.AreEqual("{ Name = tom, Age =  }", v2.ToString());
			Assert.AreNotEqual(v1, v2);
			Assert.AreNotEqual(v2, v1);
			Assert.AreNotEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test08()
		{
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsTrue(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsFalse(type.IsGenericType);
			var v1 = Activator.CreateInstance(type, new object[] { null, null });
			var v2 = Activator.CreateInstance(type, new object[] { null, null });
			Assert.IsNotNull(v1);
			Assert.AreEqual("{ Name = , Age =  }", v1.ToString());
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test07()
		{
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsTrue(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsFalse(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int?) });
			Assert.IsTrue(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			Assert.IsFalse(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			Assert.IsTrue(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			Assert.IsFalse(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			Assert.IsTrue(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type = anonymousTypes.CreateType(null, null);
			Assert.IsFalse(type.IsGenericType);
			var v1 = Activator.CreateInstance(type);
			var v2 = Activator.CreateInstance(type);
			Assert.IsNotNull(v1);
			Assert.AreEqual("{ }", v1.ToString());
			Assert.AreEqual(v1, v2);
			Assert.AreEqual(v2, v1);
			Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
		}

		[TestMethod]
		public void Test03()
		{
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type = anonymousTypes.CreateType(null, null);
			Assert.IsFalse(type.IsGenericType);
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
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type1 = anonymousTypes.CreateType(null, null);
			var type2 = anonymousTypes.CreateType(null, null);
			Assert.IsFalse(type1.IsGenericType);
			Assert.AreEqual(0, type1.GetProperties().Length);
			Assert.AreEqual(type1, type2);
			Assert.AreEqual(type2, type1);
			Assert.IsFalse(type1.IsGenericType);
		}

		[TestMethod]
		public void Test02()
		{
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type1 = anonymousTypes.CreateType(null, null);
			var type2 = anonymousTypes.CreateType(null, null);
			Assert.IsFalse(type1.IsGenericType);
			Assert.AreEqual(0, type1.GetProperties().Length);
			Assert.AreEqual(type1, type2);
			Assert.AreEqual(type2, type1);
			Assert.IsFalse(type1.IsGenericType);
		}

		[TestMethod]
		public void Test01_2()
		{
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = false };
			var type1 = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type2 = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type3 = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(long) });
			var type4 = anonymousTypes.CreateType(new[] { "Name", "Height" }, new[] { typeof(string), typeof(int) });
			var type5 = anonymousTypes.CreateType(new[] { "Name", "Age", "Height" }, new[] { typeof(string), typeof(int), typeof(int) });
			var type6 = anonymousTypes.CreateType(new[] { "Name2", "Age" }, new[] { typeof(string), typeof(long) });
			Assert.IsFalse(type1.IsGenericType);
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
		}

		[TestMethod]
		public void Test01()
		{
			var anonymousTypes = new AnonymousTypeManager { DefaultUseGeneric = true };
			var type1 = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type2 = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
			var type3 = anonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(long) });
			var type4 = anonymousTypes.CreateType(new[] { "Name", "Height" }, new[] { typeof(string), typeof(int) });
			var type5 = anonymousTypes.CreateType(new[] { "Name", "Age", "Height" }, new[] { typeof(string), typeof(int), typeof(int) });
			var type6 = anonymousTypes.CreateType(new[] { "Name2", "Age" }, new[] { typeof(string), typeof(long) });
			Assert.IsTrue(type1.IsGenericType);
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
			Assert.AreEqual(type1.GetGenericTypeDefinition(), type3.GetGenericTypeDefinition());
			Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type4.GetGenericTypeDefinition());
			Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type5.GetGenericTypeDefinition());
			Assert.AreNotEqual(type1.GetGenericTypeDefinition(), type6.GetGenericTypeDefinition());
		}
	}
}
