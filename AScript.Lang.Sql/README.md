# AScript.Lang.Sql

## 介绍
支持SqlServer/MySql基础语法和数据类型：
* 支持SELECT查询语法：`FROM/WHERE/LEFT JOIN/RIGHT JOIN/INNER JOIN/GROUP BY/ORDER BY/LIMIT`
* 支持INSERT插入语法
* 支持UPDATE修改语法
* 支持DELETE删除语法
* 支持创建存储过程：Sqlserver/MySql语法都支持
* 支持创建函数
* 支持创建表：字段支持长度定义（不支持精度），SqlServer自增IDENTITY(1,1)，Mysql自增AUTO_INCREMENT
* 支持定义变量：不支持长度和精度定义
* 字段名、关键字不区分大小写
* 支持调用外部方法和变量
* 已内置常用数据类型：`tinyint/smallint/int/bigint/decimal/float/real/double/bit/char/nchar/varchar/nvarchar/text/datetime`

不支持：
* 存储过程暂不支持OUT参数
* SELECT查询不支持*符号
* SELECT查询不支持聚合函数

## 安装
```
install-package AScript
install-package AScript.Lang.Sql
```

## 使用说明
* 命名空间：`using AScript.Lang.Sql;`

#### 注册sql语言
```C#
Script.Langs.Set("sql", SqlLang.Instance);
// 可全局设置为默认语言
// Script.Langs.Set("sql", SqlLang.Instance, setDefault: true);
```

#### 单表查询
```C#
var s = @"select Name, Age from list where age=10";
var list = new[] { new Person("tom", 15), new Person("jim", 10), new Person("san", 20), new Person("qin", 10) };
var script = new Script();
script.Context.Langs = new[] { "sql" };
script.Context.SetVar("list", list);
var result = script.Eval<IEnumerable<dynamic>>(s).ToList();
Assert.AreEqual(2, result.Count);
Assert.AreEqual("jim", result[0].Name);
Assert.AreEqual(10, result[0].Age);
Assert.AreEqual("qin", result[1].Name);
Assert.AreEqual(10, result[1].Age);
```

#### 单独FROM语句
```C#
var s = @"from list where age=10";
var list = new[] { new Person("tom", 15), new Person("jim", 10), new Person("san", 20), new Person("qin", 10) };
var script = new Script();
script.Context.Langs = new[] { "sql" };
script.Context.SetVar("list", list);
var result = script.Eval<IEnumerable<Person>>(s).ToList();
Assert.AreEqual(2, result.Count);
Assert.AreEqual("jim", result[0].Name);
Assert.AreEqual(10, result[0].Age);
Assert.AreEqual("qin", result[1].Name);
Assert.AreEqual(10, result[1].Age);
```

#### 多表查询
```C#
var q1 = new[] { new Person("tom", 20), new Person("jim", 25), new Person("san", 18), new Person("kit", 30) };
var q2 = new[] { new AddressInfo("jim", "a"), new AddressInfo("cc", "b"), new AddressInfo("tom", "c"), new AddressInfo("ee", "d") };
string s = @"
@lang sql
select a.Name, a.Age, b?.Address
from q1 as a
left join q2 as b on a.Name = b.UserName
where a.age > 22
order by a.age desc
";
var script = new Script();
script.Context.SetVar("q1", q1);
script.Context.SetVar("q2", q2);
var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
Assert.AreEqual(2, list.Count);
Assert.AreEqual("kit", list[0].Name);
Assert.AreEqual(30, list[0].Age);
Assert.IsNull(list[0].Address);
Assert.AreEqual("jim", list[1].Name);
Assert.AreEqual(25, list[1].Age);
Assert.AreEqual("a", list[1].Address);
```

#### INSERT
```C#
var s = @"insert into list (Name, Age) values ('tom', 20), ('jim', 25)";
var list = new List<Person>();
var script = new Script();
script.Context.Langs = new[] { "sql" };
script.Context.SetVar("list", list);
Assert.AreEqual(2, script.Eval(s));
Assert.AreEqual(2, list.Count);
Assert.AreEqual("tom", list[0].Name);
Assert.AreEqual(20, list[0].Age);
Assert.AreEqual("jim", list[1].Name);
Assert.AreEqual(25, list[1].Age);
```

#### UPDATE
```C#
var s = @"update list set Age=28 where Age<25";
var list = new List<Person>
{
	new Person("jim", 18),
	new Person("tom", 20),
	new Person("lily", 30)
};
var script = new Script();
script.Context.Langs = new[] { "sql" };
script.Context.SetVar("list", list);
var count = (int)script.Eval(s);
Assert.AreEqual(2, count);
Assert.AreEqual(28, list[0].Age);
Assert.AreEqual(28, list[1].Age);
Assert.AreEqual(30, list[2].Age);
```

#### DELETE
```C#
var s = @"delete from list where Age<25";
var list = new List<Person>
{
	new Person("jim", 18),
	new Person("tom", 20),
	new Person("lily", 30)
};
var script = new Script();
script.Options.CompileMode = ECompileMode.All;
script.Context.Langs = new[] { "sql" };
script.Context.SetVar("list", list);
var count = (int)script.Eval(s);
Assert.AreEqual(2, count);
Assert.AreEqual(1, list.Count);
Assert.AreEqual("lily", list[0].Name);
```

#### SqlServer存储过程
```C#
var s = @"
CREATE PROCEDURE AddPerson
	@name VARCHAR,
	@age INT
AS
BEGIN
	INSERT INTO list (name, age) VALUES(@name, @age)
END

EXEC AddPerson 'tom', 20
";
var list = new List<Person>();
var script = new Script();
script.Context.Langs = new[] { "sql" };
script.Context.SetVar("list", list);
Assert.AreEqual(1, script.Eval(s));
Assert.AreEqual(1, list.Count);
Assert.AreEqual("tom", list[0].Name);
Assert.AreEqual(20, list[0].Age);
```

#### MySql存储过程
```C#
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
```

#### 创建函数
```C#
var s = @"
CREATE FUNCTION Calc(@a INT, @b INT)
RETURNS INT
BEGIN
	DECLARE @sum INT
	SET @sum = @a * @b + @a + @b
	RETURN @sum
END
SELECT Calc(2, 3)
";
var script = new Script();
script.Context.Langs = new[] { "sql" };
var result = script.Eval(s);
Assert.AreEqual(11, result);
```

#### 创建表
示例1：
```C#
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
```
示例2：
```C#
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
```

#### SQL to LINQ to SQL
操作DbContext，将SQL语句转为LINQ查询，实现SQL to LINQ to SQL闭环：
```C#
using (var context = new TestSqliteContext())
{
	var s = @"
select p.Id, p.Name, p.Age, a.Address as MyAddress, case p.Age when 20 then 1 when 22 then 2 else 3 end as Level
from context.Persons as p
left join context.AddressInfos as a on p.Id = a.UserId
";
	var script = new Script();
	script.Context.Langs = new[] { "sql" };
	script.Context.SetVar("context", context);
	var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
	Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));
}
```
生成的SQL语句：
```
SELECT "p"."Id", "p"."Name", "p"."Age", "a"."Address" AS "MyAddress", CASE
    WHEN "p"."Age" = 20 THEN 1
    WHEN "p"."Age" = 22 THEN 2
    ELSE 3
END AS "Level"
FROM "Persons" AS "p"
LEFT JOIN "AddressInfos" AS "a" ON "p"."Id" = "a"."UserId"
ORDER BY "p"."Age" DESC
```