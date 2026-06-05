using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AScript.Test.MSTests.Sql
{
	[TestClass]
	public class SqlFunctionTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["sql"] = SqlLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("sql");
		}

		[TestMethod]
		public void Test_mysql_create_function()
		{
			var s = @"
CREATE FUNCTION AddNum(@a INT, @b INT)
RETURNS INT
BEGIN
	RETURN @a + @b
END";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
		}

		[TestMethod]
		public void Test_mysql_call_function()
		{
			var s = @"
CREATE FUNCTION AddNum(@a INT, @b INT)
RETURNS INT
BEGIN
	RETURN @a + @b
END
SELECT AddNum(1, 2)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void Test_mysql_call_function_2()
		{
			var s = @"
CREATE FUNCTION AddNum(@a INT, @b INT)
RETURNS INT
BEGIN
	RETURN @a + @b
END
SELECT AddNum(10, 20)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(30, result);
		}

		[TestMethod]
		public void Test_mysql_call_function_3()
		{
			var s = @"
CREATE FUNCTION AddNum(@a INT, @b INT)
RETURNS INT
	RETURN @a + @b

SELECT AddNum(5, 3)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(8, result);
		}

		[TestMethod]
		public void Test_mysql_call_function_4()
		{
			var s = @"
CREATE FUNCTION AddNum(@a INT, @b INT)
RETURNS INT
	RETURN @a + @b

SELECT AddNum(100, 200)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(300, result);
		}

		[TestMethod]
		public void Test_mysql_call_function_with_string()
		{
			var s = @"
CREATE FUNCTION GetGreeting(@name VARCHAR)
RETURNS VARCHAR
BEGIN
	RETURN 'Hello ' + @name
END
SELECT GetGreeting('World')";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual("Hello World", result);
		}

		[TestMethod]
		public void Test_mysql_call_function_5()
		{
			var s = @"
CREATE FUNCTION AddPerson(@name VARCHAR, @age INT)
RETURNS INT
BEGIN
	INSERT INTO list (name, age) VALUES(@name, @age)
	RETURN len(list)
END
SELECT AddPerson('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval(s);
			Assert.AreEqual(1, result);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_mysql_call_function_6()
		{
			var s = @"
CREATE FUNCTION AddPerson(@name VARCHAR, @age INT)
RETURNS INT
BEGIN
	INSERT INTO list (name, age) VALUES(@name, @age)
	RETURN len(list)
END
SELECT AddPerson('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			var result = script.Eval(s);
			Assert.AreEqual(1, result);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_mysql_function_with_multiple_statements()
		{
			var s = @"
CREATE FUNCTION Calc(@a INT, @b INT)
RETURNS INT
BEGIN
	DECLARE @sum INT
	SET @sum = @a * @b + @a + @b
	RETURN @sum
END
SELECT Calc(2, 3)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(11, result);
		}

		[TestMethod]
		public void Test_mysql_function_with_begin_end()
		{
			var s = @"
CREATE FUNCTION Double(@n INT)
RETURNS INT
BEGIN
	DECLARE @result INT
	SET @result = @n * 2
	RETURN @result
END
SELECT Double(5)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(10, result);
		}

		[TestMethod]
		public void Test_mysql_function_with_begin_end_2()
		{
			var s = @"
CREATE FUNCTION Double(@n INT)
RETURNS INT
BEGIN
	DECLARE @result INT
	SET @result = @n * 2
	RETURN @result
END
SELECT Double(5)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(10, result);
		}

		[TestMethod]
		public void Test_mysql_function_no_args()
		{
			var s = @"
CREATE FUNCTION GetDefaultValue()
RETURNS INT
	RETURN 100

SELECT GetDefaultValue()";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(100, result);
		}

		[TestMethod]
		public void Test_mysql_function_no_args_2()
		{
			var s = @"
CREATE FUNCTION GetDefaultValue()
RETURNS INT
	RETURN 100

SELECT GetDefaultValue()";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual(100, result);
		}

		[TestMethod]
		public void Test_mysql_function_return_string()
		{
			var s = @"
CREATE FUNCTION Concat2(@a VARCHAR, @b VARCHAR)
RETURNS VARCHAR
BEGIN
	RETURN @a + @b
END
SELECT Concat2('Hello', 'World')";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval(s);
			Assert.AreEqual("HelloWorld", result);
		}
	}
}