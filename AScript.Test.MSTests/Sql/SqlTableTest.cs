using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
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
		public void Test03_student_scores_4()
		{
			var s = @"
CREATE TABLE student_scores (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    subject VARCHAR(20),
    score INT NOT NULL DEFAULT 0,
    class_level VARCHAR(10)
);

INSERT INTO student_scores (name,subject,score,class_level) VALUES
('张三','数学',85,'A'),
('李四','数学',92,'B'),
('王五','数学',78,'A'),
('赵六','数学',45,'C');

UPDATE student_scores
SET class_level = 
    CASE 
        WHEN score >= 90 THEN 
            CASE WHEN class_level='A' THEN 'A+' ELSE 'A' END
        WHEN score >= 80 THEN 'B'
        ELSE 'C'
    END;

SELECT id,name,subject,score,class_level FROM student_scores;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual(1, list[0].id);
			Assert.AreEqual("张三", list[0].name);
			Assert.AreEqual(85, list[0].score);
			Assert.AreEqual("数学", list[0].subject);
			Assert.AreEqual("B", list[0].class_level);
			Assert.AreEqual(2, list[1].id);
			Assert.AreEqual("李四", list[1].name);
			Assert.AreEqual(92, list[1].score);
			Assert.AreEqual("数学", list[1].subject);
			Assert.AreEqual("A", list[1].class_level);
			Assert.AreEqual(3, list[2].id);
			Assert.AreEqual("王五", list[2].name);
			Assert.AreEqual(78, list[2].score);
			Assert.AreEqual("数学", list[2].subject);
			Assert.AreEqual("C", list[2].class_level);
			Assert.AreEqual(4, list[3].id);
			Assert.AreEqual("赵六", list[3].name);
			Assert.AreEqual(45, list[3].score);
			Assert.AreEqual("数学", list[3].subject);
			Assert.AreEqual("C", list[3].class_level);
		}

		[TestMethod]
		public void Test03_student_scores_3()
		{
			var s = @"
CREATE TABLE student_scores (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    subject VARCHAR(20),
    score INT NOT NULL DEFAULT 0,
    class_level VARCHAR(10)
);

INSERT INTO student_scores (name,subject,score,class_level) VALUES
('张三','数学',85,'A'),
('李四','数学',92,'B'),
('王五','数学',78,'A'),
('赵六','数学',45,'C');

UPDATE student_scores
SET class_level = 
    CASE 
        WHEN score >= 90 THEN 
            CASE WHEN class_level='A' THEN 'A+' ELSE 'A' END
        WHEN score >= 80 THEN 'B'
        ELSE 'C'
    END;

SELECT id,name,subject,score,class_level FROM student_scores;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual(1, list[0].id);
			Assert.AreEqual("张三", list[0].name);
			Assert.AreEqual(85, list[0].score);
			Assert.AreEqual("数学", list[0].subject);
			Assert.AreEqual("B", list[0].class_level);
			Assert.AreEqual(2, list[1].id);
			Assert.AreEqual("李四", list[1].name);
			Assert.AreEqual(92, list[1].score);
			Assert.AreEqual("数学", list[1].subject);
			Assert.AreEqual("A", list[1].class_level);
			Assert.AreEqual(3, list[2].id);
			Assert.AreEqual("王五", list[2].name);
			Assert.AreEqual(78, list[2].score);
			Assert.AreEqual("数学", list[2].subject);
			Assert.AreEqual("C", list[2].class_level);
			Assert.AreEqual(4, list[3].id);
			Assert.AreEqual("赵六", list[3].name);
			Assert.AreEqual(45, list[3].score);
			Assert.AreEqual("数学", list[3].subject);
			Assert.AreEqual("C", list[3].class_level);
		}

		[TestMethod]
		public void Test03_student_scores_2()
		{
			var s = @"
CREATE TABLE student_scores (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    subject VARCHAR(20),
    score INT,
    class_level VARCHAR(10)
);

INSERT INTO student_scores (name,subject,score,class_level) VALUES
('张三','数学',85,'A'),
('李四','数学',92,'B'),
('王五','数学',78,'A'),
('赵六','数学',45,'C');

UPDATE student_scores
SET class_level = 
    CASE 
        WHEN score >= 90 THEN 
            CASE WHEN class_level='A' THEN 'A+' ELSE 'A' END
        WHEN score >= 80 THEN 'B'
        ELSE 'C'
    END;

SELECT id,name,subject,score,class_level FROM student_scores;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual(1, list[0].id);
			Assert.AreEqual("张三", list[0].name);
			Assert.AreEqual(85, list[0].score);
			Assert.AreEqual("数学", list[0].subject);
			Assert.AreEqual("B", list[0].class_level);
			Assert.AreEqual(2, list[1].id);
			Assert.AreEqual("李四", list[1].name);
			Assert.AreEqual(92, list[1].score);
			Assert.AreEqual("数学", list[1].subject);
			Assert.AreEqual("A", list[1].class_level);
			Assert.AreEqual(3, list[2].id);
			Assert.AreEqual("王五", list[2].name);
			Assert.AreEqual(78, list[2].score);
			Assert.AreEqual("数学", list[2].subject);
			Assert.AreEqual("C", list[2].class_level);
			Assert.AreEqual(4, list[3].id);
			Assert.AreEqual("赵六", list[3].name);
			Assert.AreEqual(45, list[3].score);
			Assert.AreEqual("数学", list[3].subject);
			Assert.AreEqual("C", list[3].class_level);
		}

		[TestMethod]
		public void Test03_student_scores()
		{
			var s = @"
CREATE TABLE student_scores (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    subject VARCHAR(20),
    score INT,
    class_level VARCHAR(10)
);

INSERT INTO student_scores (name,subject,score,class_level) VALUES
('张三','数学',85,'A'),
('李四','数学',92,'B'),
('王五','数学',78,'A'),
('赵六','数学',45,'C');

UPDATE student_scores
SET class_level = 
    CASE 
        WHEN score >= 90 THEN 
            CASE WHEN class_level='A' THEN 'A+' ELSE 'A' END
        WHEN score >= 80 THEN 'B'
        ELSE 'C'
    END;

SELECT id,name,subject,score,class_level FROM student_scores;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual(1, list[0].id);
			Assert.AreEqual("张三", list[0].name);
			Assert.AreEqual(85, list[0].score);
			Assert.AreEqual("数学", list[0].subject);
			Assert.AreEqual("B", list[0].class_level);
			Assert.AreEqual(2, list[1].id);
			Assert.AreEqual("李四", list[1].name);
			Assert.AreEqual(92, list[1].score);
			Assert.AreEqual("数学", list[1].subject);
			Assert.AreEqual("A", list[1].class_level);
			Assert.AreEqual(3, list[2].id);
			Assert.AreEqual("王五", list[2].name);
			Assert.AreEqual(78, list[2].score);
			Assert.AreEqual("数学", list[2].subject);
			Assert.AreEqual("C", list[2].class_level);
			Assert.AreEqual(4, list[3].id);
			Assert.AreEqual("赵六", list[3].name);
			Assert.AreEqual(45, list[3].score);
			Assert.AreEqual("数学", list[3].subject);
			Assert.AreEqual("C", list[3].class_level);
		}

		[TestMethod]
		public void Test02_student_scores_2()
		{
			var s = @"
CREATE TABLE student_scores (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    subject VARCHAR(20),
    score INT,
    class_level VARCHAR(10)
);

INSERT INTO student_scores (name,subject,score,class_level) VALUES
('张三','数学',85,'A'),
('李四','数学',92,'B'),
('王五','数学',78,'A'),
('赵六','数学',45,'C');

SELECT 
	id,
    name,
    subject,
    score
FROM student_scores
WHERE 
    CASE 
        WHEN class_level='A' THEN score > 80
        WHEN class_level='B' THEN score > 70
        ELSE score > 60
    END;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(1, list[0].id);
			Assert.AreEqual("张三", list[0].name);
			Assert.AreEqual(85, list[0].score);
			Assert.AreEqual("数学", list[0].subject);
			Assert.AreEqual(2, list[1].id);
			Assert.AreEqual("李四", list[1].name);
			Assert.AreEqual(92, list[1].score);
			Assert.AreEqual("数学", list[1].subject);
		}

		[TestMethod]
		public void Test02_student_scores()
		{
			var s = @"
CREATE TABLE student_scores (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    subject VARCHAR(20),
    score INT,
    class_level VARCHAR(10)
);

INSERT INTO student_scores (name,subject,score,class_level) VALUES
('张三','数学',85,'A'),
('李四','数学',92,'B'),
('王五','数学',78,'A'),
('赵六','数学',45,'C');

SELECT 
	id,
    name,
    subject,
    score
FROM student_scores
WHERE 
    CASE 
        WHEN class_level='A' THEN score > 80
        WHEN class_level='B' THEN score > 70
        ELSE score > 60
    END;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(1, list[0].id);
			Assert.AreEqual("张三", list[0].name);
			Assert.AreEqual(85, list[0].score);
			Assert.AreEqual("数学", list[0].subject);
			Assert.AreEqual(2, list[1].id);
			Assert.AreEqual("李四", list[1].name);
			Assert.AreEqual(92, list[1].score);
			Assert.AreEqual("数学", list[1].subject);
		}

		[TestMethod]
		public void Test01_student_scores_2()
		{
			var s = @"
CREATE TABLE student_scores (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    subject VARCHAR(20),
    score INT,
    class_level VARCHAR(10)
);

INSERT INTO student_scores (name,subject,score,class_level) VALUES
('张三','数学',85,'A'),
('李四','数学',92,'B'),
('王五','数学',78,'A'),
('赵六','数学',45,'C');

SELECT 
	id,
    name,
    score,
    CASE 
        WHEN score >= 90 THEN '优秀'
        WHEN score >= 80 THEN '良好'
        WHEN score >= 60 THEN '及格'
        ELSE '不及格'
    END AS basic_grade,
    CASE 
        WHEN score >= 90 THEN 
            CASE WHEN class_level='A' THEN '顶尖' ELSE '优秀' END
        WHEN score >= 80 THEN '潜力'
        ELSE '需加强'
    END AS advanced_grade
FROM student_scores;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual(1, list[0].id);
			Assert.AreEqual("张三", list[0].name);
			Assert.AreEqual(85, list[0].score);
			Assert.AreEqual("良好", list[0].basic_grade);
			Assert.AreEqual("潜力", list[0].advanced_grade);
			Assert.AreEqual(2, list[1].id);
			Assert.AreEqual("李四", list[1].name);
			Assert.AreEqual(92, list[1].score);
			Assert.AreEqual("优秀", list[1].basic_grade);
			Assert.AreEqual("优秀", list[1].advanced_grade);
			Assert.AreEqual(3, list[2].id);
			Assert.AreEqual("王五", list[2].name);
			Assert.AreEqual(78, list[2].score);
			Assert.AreEqual("及格", list[2].basic_grade);
			Assert.AreEqual("需加强", list[2].advanced_grade);
			Assert.AreEqual(4, list[3].id);
			Assert.AreEqual("赵六", list[3].name);
			Assert.AreEqual(45, list[3].score);
			Assert.AreEqual("不及格", list[3].basic_grade);
			Assert.AreEqual("需加强", list[3].advanced_grade);
		}

		[TestMethod]
		public void Test01_student_scores()
		{
			var s = @"
CREATE TABLE student_scores (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    subject VARCHAR(20),
    score INT,
    class_level VARCHAR(10)
);

INSERT INTO student_scores (name,subject,score,class_level) VALUES
('张三','数学',85,'A'),
('李四','数学',92,'B'),
('王五','数学',78,'A'),
('赵六','数学',45,'C');

SELECT 
	id,
    name,
    score,
    CASE 
        WHEN score >= 90 THEN '优秀'
        WHEN score >= 80 THEN '良好'
        WHEN score >= 60 THEN '及格'
        ELSE '不及格'
    END AS basic_grade,
    CASE 
        WHEN score >= 90 THEN 
            CASE WHEN class_level='A' THEN '顶尖' ELSE '优秀' END
        WHEN score >= 80 THEN '潜力'
        ELSE '需加强'
    END AS advanced_grade
FROM student_scores;
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual(1, list[0].id);
			Assert.AreEqual("张三", list[0].name);
			Assert.AreEqual(85, list[0].score);
			Assert.AreEqual("良好", list[0].basic_grade);
			Assert.AreEqual("潜力", list[0].advanced_grade);
			Assert.AreEqual(2, list[1].id);
			Assert.AreEqual("李四", list[1].name);
			Assert.AreEqual(92, list[1].score);
			Assert.AreEqual("优秀", list[1].basic_grade);
			Assert.AreEqual("优秀", list[1].advanced_grade);
			Assert.AreEqual(3, list[2].id);
			Assert.AreEqual("王五", list[2].name);
			Assert.AreEqual(78, list[2].score);
			Assert.AreEqual("及格", list[2].basic_grade);
			Assert.AreEqual("需加强", list[2].advanced_grade);
			Assert.AreEqual(4, list[3].id);
			Assert.AreEqual("赵六", list[3].name);
			Assert.AreEqual(45, list[3].score);
			Assert.AreEqual("不及格", list[3].basic_grade);
			Assert.AreEqual("需加强", list[3].advanced_grade);
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
CREATE TABLE person (name varchar, Age int)
INSERT INTO person (Name, age) VALUES ('tom', 20)";
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
CREATE TABLE person (name varchar, Age int)
INSERT INTO person (Name, age) VALUES ('tom', 20)";
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
CREATE TABLE person (name varchar, Age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
SELECT Name,age FROM person WHERE age > 22";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual(25, result[0].age);
		}

		[TestMethod]
		public void Test_create_table_and_select_2()
		{
			var s = @"
CREATE TABLE person (name varchar, Age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
SELECT Name,age FROM person WHERE age > 22";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual(25, result[0].age);
		}

		[TestMethod]
		public void Test_create_table_and_select_3()
		{
			var s = @"
CREATE TABLE person (name varchar, Age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
SELECT Name, age, case age when 20 then 1 else 2 end as level FROM person WHERE age > 22";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual(25, result[0].age);
			Assert.AreEqual(2, result[0].level);
		}

		[TestMethod]
		public void Test_create_table_and_select_4()
		{
			var s = @"
CREATE TABLE person (name varchar, Age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
SELECT Name, age, case age when 20 then 1 else 2 end as level FROM person WHERE age > 22";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("jim", result[0].Name);
			Assert.AreEqual(25, result[0].age);
			Assert.AreEqual(2, result[0].level);
		}

		[TestMethod]
		public void Test_create_table_and_update()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
UPDATE person SET Age = 30 WHERE Name = 'tom'";
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
		public void Test_create_table_and_update_delete_select_2()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20),('jim', 25),('san', 18)
UPDATE person SET age = 30 WHERE name = 'tom'
DELETE FROM person WHERE Name = 'jim'
SELECT Name,age FROM person WHERE age > 22
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(30, list[0].age);
		}

		[TestMethod]
		public void Test_create_table_and_update_delete_select()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20),('jim', 25),('san', 18)
UPDATE person SET age = 30 WHERE name = 'tom'
DELETE FROM person WHERE Name = 'jim'
SELECT Name,age FROM person WHERE age > 22
";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("tom", list[0].Name);
			Assert.AreEqual(30, list[0].age);
		}

		[TestMethod]
		public void Test_create_table_and_delete()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', 20)
INSERT INTO person (name, age) VALUES ('jim', 25)
DELETE FROM person WHERE Name = 'tom'";
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

		[TestMethod]
		public void Test_create_table_and_insert_null_value()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', null)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(1, table.Rows.Count);
			Assert.AreEqual("tom", table.Rows[0]["name"]);
			Assert.AreEqual(DBNull.Value, table.Rows[0]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_insert_null_value_2()
		{
			var s = @"
CREATE TABLE person (name varchar, age int)
INSERT INTO person (name, age) VALUES ('tom', null)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Eval(s);
			var table = (SqlTable)script.Context.EvalVar("person");
			Assert.AreEqual(1, table.Rows.Count);
			Assert.AreEqual("tom", table.Rows[0]["name"]);
			Assert.AreEqual(DBNull.Value, table.Rows[0]["age"]);
		}

		[TestMethod]
		public void Test_create_table_and_insert_null_value_3()
		{
			var s = @"
CREATE TABLE person (name varchar, age int not null)
INSERT INTO person (name, age) VALUES ('tom', null)";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			Assert.ThrowsException<System.Data.NoNullAllowedException>(() => script.Eval(s));
		}

		[TestMethod]
		public void Test_create_table_and_insert_null_value_4()
		{
			var s = @"
CREATE TABLE person (name varchar, age int not null)
INSERT INTO person (name, age) VALUES ('tom', null)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			Assert.ThrowsException<System.Data.NoNullAllowedException>(() => script.Eval(s));
		}
	}
}
