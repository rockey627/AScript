using System;

namespace AScript.Test.MSTests
{
	public static class PersonExtensions
	{
		// 扩展构造函数
		public static Person new_Person(string firstName, string lastName)
		{
			return new Person { Name = firstName + lastName, Age = 18 };
		}

		// 扩展静态方法
		public static Person Person_Create(string firstName, string lastName)
		{
			var name = firstName + lastName;
			return new Person { Name = name, Age = 18 };
		}

		// 扩展静态属性
		public static string Person_get_DefaultName()
		{
			return "ABC";
		}

		// 扩展实例属性
		public static string get_FullInfo(Person p)
		{
			return $"name:{p.Name},age:{p.Age}";
		}

		// 扩展实例方法
		public static string SayHi(Person p, string yourName)
		{
			return $"hi {yourName}, my name is {p.Name}";
		}
	}
}
