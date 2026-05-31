# AScript.Lang.Sql

## 介绍
支持SqlServer/MySql基础语法和数据类型。

* 支持SELECT查询语法
* 支持INSERT插入语法
* 支持UPDATE修改语法
* 支持DELETE删除语法
* 支持定义变量
* 支持创建存储过程
* 支持创建表
* 支持调用外部方法和变量
* 已内置常用数据类型：`tinyint/smallint/int/bigint/decimal/float/real/double/bit/char/nchar/varchar/nvarchar/text/datetime`
* 类型不支持长度、精度定义，比如：decimal(2,10)、varchar(50)这样是不支持的
* 存储过程暂不支持OUT参数

## 安装
```
install-package AScript
install-package AScript.Lang.Sql
```

## 使用说明
* 命名空间：`using AScript.Lang.Sql;`

#### 注册sql语言
```
Script.Langs.Set("sql", SqlLang.Instance);
// 可全局设置为默认语言
// Script.Langs.Set("sql", SqlLang.Instance, setDefault: true);
```

#### 上下文中指定sql语言
如果已全局设置默认语言则无需指定
```
var script = new Script();
script.Context.Langs = new [] { "sql" };

```

#### 使用@lang指定sql语言
```

```

#### SELECT

#### FROM

#### INSERT

#### UPDATE

#### DELETE

#### SqlServer存储过程
```
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
```
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