using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AScript.Test.MSTests.Sql
{
	[TestClass]
	public class SqlProcedureTest
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
		public void Test_mysql_call_procedure_declare_2()
		{
			var s = @"
CREATE PROCEDURE AddPerson(@name VARCHAR, @age INT)
BEGIN
	DECLARE @name2 VARCHAR
	DECLARE @age2 INT
	SET @name2 = @name + '2'
	SET @age2 = @age + 10
	INSERT INTO list (name, age) VALUES(@name, @age),(@name2, @age2)
END
CALL AddPerson('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(2, script.Eval(s));
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
			Assert.AreEqual("tom2", list[1].Name);
			Assert.AreEqual(30, list[1].Age);
		}

		[TestMethod]
		public void Test_mysql_call_procedure_declare()
		{
			var s = @"
CREATE PROCEDURE AddPerson(@name VARCHAR, @age INT)
BEGIN
	DECLARE @name2 VARCHAR
	DECLARE @age2 INT
	SET @name2 = @name + '2'
	SET @age2 = @age + 10
	INSERT INTO list (name, age) VALUES(@name, @age),(@name2, @age2)
END
CALL AddPerson('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(2, script.Eval(s));
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
			Assert.AreEqual("tom2", list[1].Name);
			Assert.AreEqual(30, list[1].Age);
		}

		[TestMethod]
		public void Test_mysql_create_procedure()
		{
			var s = @"
CREATE PROCEDURE AddPerson(@name VARCHAR, @age INT)
BEGIN
	insert into list (name, age) values(@name, @age)
END";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(0, list.Count);
		}

		[TestMethod]
		public void Test_mysql_call_procedure()
		{
			var s = @"
CREATE PROCEDURE AddPerson(IN @name VARCHAR, IN @age INT)
BEGIN
	INSERT INTO list (name, age) VALUES(@name, @age)
END
CALL AddPerson('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_mysql_call_procedure_2()
		{
			var s = @"
CREATE PROCEDURE AddPerson(@name VARCHAR, @age INT)
BEGIN
	insert into list (name, age) values(@name, @age)
END
CALL AddPerson('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_mysql_call_procedure_3()
		{
			var s = @"
CREATE PROCEDURE AddPerson(@name VARCHAR, @age INT)
	insert into list (name, age) values(@name, @age)

CALL AddPerson('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_mysql_call_procedure_4()
		{
			var s = @"
CREATE PROCEDURE AddPerson(@name VARCHAR, @age INT)
	insert into list (name, age) values(@name, @age)

CALL AddPerson('tom', 20)";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_sqlserver_create_procedure()
		{
			var s = @"
CREATE PROCEDURE AddPerson
	@name VARCHAR,
	@age INT
AS
BEGIN
	insert into list (name, age) values(@name, @age)
END";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			script.Eval(s);
			Assert.AreEqual(0, list.Count);
		}

		[TestMethod]
		public void Test_sqlserver_exec_procedure()
		{
			var s = @"
CREATE PROCEDURE AddPerson
	@name VARCHAR,
	@age INT
AS
BEGIN
	INSERT INTO list (name, age) VALUES(@name, @age)
END
EXEC AddPerson 'tom', 20";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_sqlserver_exec_procedure_2()
		{
			var s = @"
CREATE PROCEDURE AddPerson
	@name VARCHAR,
	@age INT
AS
BEGIN
	insert into list (name, age) values(@name, @age)
END
EXEC AddPerson 'tom', 20";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_sqlserver_exec_procedure_3()
		{
			var s = @"
CREATE PROCEDURE AddPerson
	@name VARCHAR,
	@age INT
AS
	insert into list (name, age) values(@name, @age)

EXEC AddPerson 'tom', 20";
			var list = new List<Person>();
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

		[TestMethod]
		public void Test_sqlserver_exec_procedure_4()
		{
			var s = @"
CREATE PROCEDURE AddPerson
	@name VARCHAR,
	@age INT
AS
	insert into list (name, age) values(@name, @age)

EXEC AddPerson 'tom', 20";
			var list = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("list", list);
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(20, list[0].Age);
		}

	}
}