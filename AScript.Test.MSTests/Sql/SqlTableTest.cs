using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AScript.Test.MSTests.Sql
{
	[TestClass]
	public class SqlTableTest
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
		public void Test_create_table_basic()
		{
			var s = @"CREATE TABLE person (name varchar, age int)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var table = script.Eval<SqlTable>(s);
			Assert.AreEqual("person", table.TableName);
			Assert.AreEqual(2, table.Columns.Count);
			Assert.AreEqual("name", table.Columns[0].ColumnName);
			Assert.AreEqual(typeof(string), table.Columns[0].DataType);
			Assert.AreEqual("age", table.Columns[1].ColumnName);
			Assert.AreEqual(typeof(int), table.Columns[1].DataType);
		}

		[TestMethod]
		public void Test_create_table_if_not_exists()
		{
			var s = @"CREATE TABLE IF NOT EXISTS person (name varchar, age int)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var table1 = script.Eval<SqlTable>(s);
			var table2 = script.Eval<SqlTable>(s);
			Assert.AreSame(table1, table2);
		}

		[TestMethod]
		public void Test_create_table_if_not_exists_2()
		{
			var s = @"CREATE TABLE IF NOT EXISTS person (name varchar, age int)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var table1 = script.Eval<SqlTable>(s);
			var table2 = script.Eval<SqlTable>(s);
			Assert.AreSame(table1, table2);
		}

		[TestMethod]
		public void Test_create_table_duplicate_error()
		{
			var s = @"CREATE TABLE person (name varchar, age int)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			Assert.ThrowsException<Exceptions.ScriptRuntimeException>(() => script.Eval(s));
		}

		[TestMethod]
		public void Test_create_table_and_insert()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(1, table.Rows.Count);
			Assert.AreEqual("tom", table.Rows[0]["name"]);
			Assert.AreEqual(20, table.Rows[0]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_insert_2()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(1, table.Rows.Count);
			Assert.AreEqual("tom", table.Rows[0]["name"]);
			Assert.AreEqual(20, table.Rows[0]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_select()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
SELECT * FROM person WHERE age > 22";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval<IEnumerable<SqlTable>>(s).First();
			Assert.AreEqual(1, result.Rows.Count);
			Assert.AreEqual("jim", result.Rows[0]["name"]);
			Assert.AreEqual(25, result.Rows[0]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_select_2()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
SELECT * FROM person WHERE age > 22";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval<IEnumerable<DataRow>>(s).ToList();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("jim", result[0]["name"]);
			Assert.AreEqual(25, result[0]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_update()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
UPDATE person SET age = 30 WHERE name = 'tom'";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(2, table.Rows.Count);
			Assert.AreEqual("tom", table.Rows[0]["name"]);
			Assert.AreEqual(30, table.Rows[0]["age"]);
			Assert.AreEqual("jim", table.Rows[1]["name"]);
			Assert.AreEqual(25, table.Rows[1]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_update_2()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
UPDATE person SET age = 30 WHERE name = 'tom'";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(2, table.Rows.Count);
			Assert.AreEqual("tom", table.Rows[0]["name"]);
			Assert.AreEqual(30, table.Rows[0]["age"]);
			Assert.AreEqual("jim", table.Rows[1]["name"]);
			Assert.AreEqual(25, table.Rows[1]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_delete()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
DELETE FROM person WHERE name = 'tom'";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(1, table.Rows.Count);
			Assert.AreEqual("jim", table.Rows[0]["name"]);
			Assert.AreEqual(25, table.Rows[0]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_delete_2()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
DELETE FROM person WHERE name = 'tom'";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(1, table.Rows.Count);
			Assert.AreEqual("jim", table.Rows[0]["name"]);
			Assert.AreEqual(25, table.Rows[0]["age"]);
		}

		[TestMethod]
		public void Test_create_table_multiple_columns()
		{
			var s = @"CREATE TABLE info (id int, name varchar, score decimal, birthday datetime)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var table = script.Eval<SqlTable>(s);
			Assert.AreEqual("info", table.TableName);
			Assert.AreEqual(4, table.Columns.Count);
			Assert.AreEqual(typeof(int), table.Columns[0].DataType);
			Assert.AreEqual(typeof(string), table.Columns[1].DataType);
			Assert.AreEqual(typeof(decimal), table.Columns[2].DataType);
			Assert.AreEqual(typeof(DateTime), table.Columns[3].DataType);
		}

		[TestMethod]
		public void Test_create_table_and_insert_multiple_rows()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20), ('jim', 25), ('lily', 30)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(3, table.Rows.Count);
		}
	}
}
