# AScript.Lang.Sql

## 介绍

支持SqlServer/MySql基础语法和数据类型：
* 支持SELECT查询语法
* 支持INSERT插入语法
* 支持UPDATE修改语法
* 支持DELETE删除语法
* 支持创建存储过程
* 支持创建表
* 支持定义变量
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

```

#### UPDATE
```C#

```

#### DELETE
```C#

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
CREATE PROCEDURE AddPerson(IN @name VARCHAR, IN @age INT)
BEGIN
	INSERT INTO list (name, age) VALUES(@name, @age)
END

CALL AddPerson('tom', 20)
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

#### 创建表
```C#

```

#### SQL to LINQ to SQL
```C#

```