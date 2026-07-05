# AScript

## 相关文章

* [AScript轻量级动态脚本引擎](https://mp.weixin.qq.com/s/0n17ecNjLd96FgujBUNt9w)
* [AScript如何实现中文脚本引擎](https://mp.weixin.qq.com/s/x7Pb2dRlKu83cdDsHd0KLQ)
* [AScript扩展多种脚本语言](https://mp.weixin.qq.com/s/TJE20AvQGQjRxOKM5U3Tow)
* [AScript函数体系详解](https://mp.weixin.qq.com/s/rzLzCrTAvOEFTtAGtTX6BQ)
* [AScript之eval函数详解](https://mp.weixin.qq.com/s/781Sw5FdFXJxe0eWqWjCCw)
* [基于AScript的python3脚本语言发布啦！](https://mp.weixin.qq.com/s/tcrPXFaLPz8kI2hlw-ZkuA)
* [AScript中一个很有意思的语法](https://mp.weixin.qq.com/s/JrXlUosfpWbSfKLVIl7pzg)
* [AScript之匿名类型与动态类型](https://mp.weixin.qq.com/s/6PzH9JxRsNlLMi5eISd7kg)
* [AScript如何实现LINQ语法](https://mp.weixin.qq.com/s/ZEz2BPE5kayvXRQom73khw)
* [AScript异步执行与await关键字](https://mp.weixin.qq.com/s/LPjcCugmJK6o-ONhle2Hkw)
* [AScript定制left/right join查询语法](https://mp.weixin.qq.com/s/IPKrsDlFc32bupVKLxOKug)
* [AScript之事件处理脚本](https://mp.weixin.qq.com/s/0F6vRWp4KxsXlCB9CdDH7A)
* [基于AScript的SQL脚本语言发布啦！](https://mp.weixin.qq.com/s/oKDiJ7_Pcd8ODGxYOLWXGQ)

## 介绍

C#动态脚本解析编译执行引擎

* 支持注入变量
* 支持定义变量
* 支持注入函数
* 支持定义函数
* 支持注入类型
* 支持LINQ语法、Lambda表达式
* 支持元组类型、匿名类型、动态类型
* 支持事件处理
* 支持异步
* 支持16进制整数表示：0x0A
* 支持多语句：用分号分隔多条语句
* 支持行注释：// 行注释
* 支持块注释：/* 块注释 */
* 支持if/else语句
* 支持for/while/foreach语句
* 支持continue/break/return
* 支持自定义语法解析（自定义关键字）
* 支持流式读取表达式
* 支持自定义脚本语言，已实现CSharp/Python3/SQL三种语言
* 支持2种执行模式：
1. 解析执行：解析过程中计算结果，对于非循环语句有较高的性能及低内存，有循环语句则建议使用第2种编译执行方式
```C#
var script = new Script();
var result = script.Eval("5+8*6");
Assert.AreEqual(53, result);
```
2. 编译执行：解析过程中构建Expression表达式树，编译结果可缓存，对于执行频率高的表达式建议使用编译缓存方式执行，提高性能
```C#
// 方式1
var script = new Script();
var result = script.Eval("5+8*6", ECompileMode.All);
// 方式2
var script = new Script();
script.Options.CompileMode = ECompileMode.All;
var result = script.Eval("5+8*6");
// 方式3（缓存）
var script = new Script();
// -1表示缓存时间为永久缓存
var result = script.Eval("5+8*6", -1);
```
* 上下文环境会缓存临时变量及函数，编译执行模式可关闭该缓存（获得更高的性能）
```C#
var script = new Script();
script.Options.CompileMode = ECompileMode.All;
//script.Options.RewriteVariables = false;
//script.Options.RewriteFunctions = false;
script.Eval("int sum(int a, int b)=>a+b;int n=10;sum(n,5)");
script.Eval("n+6"); // 如果RewriteVariables为false，则抛异常变量n不存在
script.Eval("sum(10,20)"); // 如果RewriteFunctions为false，则抛异常函数sum不存在
var sum = script.Context.GetFunc<int, int, int>("sum");
int result = sum(10, 20);
```

## 安装

`install-package AScript`

## 使用说明

* 命名空间：`using AScript;`
* 已内置C#常用数据类型：`int/bool/string/long/double/DateTime`等
* 已内置Convert数据转换方法：`'12'.ToInt32()`等同于`ToInt32('12')`或者`Convert.ToInt32('12')`
* 已内置Enumerable扩展方法：`list.Where(a=>xxx).Select(a=>new { xxx })`
* 已内置Queryable扩展方法：`query.Where(a=>xxx).Select(a=>new { xxx })`

#### 注入类型及类型中的方法
```C#
var script = new Script();
script.Context.AddType<Person>();
script.Eval("var p1 = new Person('tom', 20); p1.SayHello()"); // Hello, my name is tom, I'm 20 years old
script.Eval("var p2 = Person.Create('john', 35); p2.SayHello()"); // Hello, my name is john, I'm 35 years old

// 添加类中的所有公开静态方法
script.Context.AddFunc<Person>();
script.Eval("var p3 = Create('jim', 15); p3.SayHello()"); // Hello, my name is jim, I'm 15 years old
// 添加类中的所有公开实例方法
var p = new Person("san", 27);
script.Context.AddFunc(p);
script.Eval("SayHello()"); // Hello, my name is san, I'm 27 years old

public class Person
{
	public string Name { get; set; }
	public int Age { get; set; }

	public event EventHandler<EventArgs> Saying;

	public Person() { }
	public Person(string name, int age)
	{
		this.Name = name;
		this.Age = age;
	}
	
	protected virtual void OnSaying(EventArgs e)
	{
		this.Saying?.Invoke(this, e);
	}

	public string SayHello()
	{
		OnSaying(EventArgs.Empty);
		return $"Hello, my name is {this.Name}, I'm {this.Age} years old";
	}

	public static Person Create(string name, int age)
	{
		return new Person(name, age);
	}
}
```

#### 变量

```C#
var script = new Script();
script.Context.SetVar("m", 6);
var result = script.Eval("int n=8;n+m+10*(3+0x0A)");
Assert.AreEqual(8 + 6 + 10 * (3 + 0x0A), result);
```

#### 匿名类型
```C#
string s = @"
var a = new { Name='tony', Age=20 }
a.Name + ':' + a.Age
";
var script = new Script();
Assert.AreEqual("tony:20", script.Eval(s));
var a = script.Eval("a");
// 获取脚本中定义的匿名类型
var type = Script.AnonymousTypes.CreateType(new[] { "Name", "Age" }, new[] { typeof(string), typeof(int) });
Assert.IsInstanceOfType(a, type);
dynamic d = a;
Assert.AreEqual("tony", d.Name);
Assert.AreEqual(20, d.Age);
Assert.AreEqual("{ Name = tony, Age = 20 }", a.ToString());
```

#### 动态类型
```C#
string s = @"
var a = new ExpandoObject();
a.Name = 'jim';
a.Age = 23;
a
";
var script = new Script();
dynamic a = script.Eval(s);
Assert.IsInstanceOfType(a, typeof(ExpandoObject));
Assert.AreEqual("jim", a.Name);
Assert.AreEqual(23, a.Age);
```

#### 类型转换
```C#
string s = "int n = 10; n+(long)n";
var script = new Script();
var r = script.Eval(s, out var type);
Assert.AreEqual(20L, r);
Assert.AreEqual(typeof(long), type);
```

#### 注入函数
```C#
var script = new Script();
script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
script.Context.AddFunc<int, int, int, int>("sum", (a, b, c) => a + b + c);
script.Context.AddFunc<int, int, int>("mult", (a, b) => a * b);
Assert.AreEqual(1+2+8+1+2+3+5*6, script.Eval("sum(1,2)+8+sum(1,2,3)+mult(5,6)"));
```

#### 自定义函数
```C#
string s = @"
int exec(int a, int b) {
	var n=mult(a,10);
	n+b;
}
// 2个数相加
int sum(int a, int b)=>a+b;
// 3个数相加
int sum(int a, int b, int c)=>a+b+c;
// 乘法
int mult(int a, int b)=>a*b;
/* 
调用函数计算结果：
1 + 2 + 8 + 1 + 2 + 3 + 5 * 6
*/
sum(1,2)+8+sum(1,2,3)+mult(5,6)
";
var script = new Script();
var result = script.Eval(s);
Assert.AreEqual(1 + 2 + 8 + 1 + 2 + 3 + 5 * 6, result);
Assert.AreEqual(1 + 2 + 8, script.Eval("sum(1,2)+8"));
Assert.AreEqual(5 * 10 + 2 + 8, script.Eval("exec(5,2)+8"));
```

#### 递归
```C#
string s = @"
int exec(int a) {
	if (a < 1) return 0;
	a + exec(a-1);
}
exec(5)
";
var script = new Script();
Assert.AreEqual(15, script.Eval(s));
Assert.AreEqual(55, script.Eval("exec(10)"));
```

#### 事件
示例1：
```C#
var s = @"
var p = new Person('tom', 20);
p.Saying += (ss,ee)=>{
	(ss as Person).Age+=1;
}
p.SayHello();
p.SayHello();
";
var script = new Script();
script.Context.AddType<Person>();
Assert.AreEqual("Hello, my name is tom, I'm 22 years old", script.Eval(s));
```
示例2：
```C#
var s = @"
void saying(object sender, EventArgs e) {
	(sender as Person).Age+=1;
}
var p = new Person('tom', 20);
p.Saying += saying;
p.SayHello();
p.Saying -= saying;
p.SayHello();
";
var script = new Script();
script.Context.AddType<Person>();
Assert.AreEqual("Hello, my name is tom, I'm 21 years old", script.Eval(s));
```
示例3：
```C#
var s = @"
void saying(object sender, EventArgs e) {
	(sender as Person).Age+=1;
}
var p = new Person('tom', 20);
p.Saying += saying;
p.SayHello();
p.SayHello();
";
var script = new Script();
script.Context.AddType<Person>();
Assert.AreEqual("Hello, my name is tom, I'm 22 years old", script.Eval(s));
var p = script.Eval<Person>("p");
Assert.AreEqual("Hello, my name is tom, I'm 23 years old", p.SayHello());
var handle = script.Context.GetEvent<EventHandler<EventArgs>>("saying");
p.Saying -= handle;
Assert.AreEqual("Hello, my name is tom, I'm 23 years old", p.SayHello());
```

#### 字符串插值
```C#
string s = "var name='tom'; $'hello {name}, 5+8={5+8}'";
var script = new Script();
Assert.AreEqual("hello tom, 5+8=13", script.Eval(s));
```

#### 字符串索引和截取
```C#
var script = new Script();
Assert.AreEqual('e', script.Eval("'hello'[1]"));
Assert.AreEqual('e', script.Eval("'hello'[-4]"));
Assert.AreEqual("ell", script.Eval("'hello'[1:4]"));
Assert.AreEqual("ell", script.Eval("'hello'[-4:-1]"));
```

#### 数组
```C#
var arr = new [] { 1, 2, 3};
var arr = new int[] { 1, 2, 3 };
arr.Length;
arr[0];
```

#### 列表
```
var script = new Script();
var result1 = (List<int>)script.Eval("var list1 = [0,1,2,3,4]; list1[1:4]");
var result2 = (List<int>)script.Eval("var list2 = new List<int> { 0,1,2,3,4 }; list2[-4:-1]");
CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result1);
CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, result2);
Assert.AreEqual(1, script.Eval("list1[1]"));
Assert.AreEqual(1, script.Eval("list1[-4]"));
```

#### 点操作符
```C#
var script = new Script();
Assert.AreEqual(DateTime.Now.Year, script.Eval("DateTime.Now.Year"));
Assert.AreEqual(int.MaxValue, script.Eval("int.MaxValue"));
Assert.AreEqual("hello".Length, script.Eval("'hello'.Length"));
Assert.AreEqual("hello".Substring(1, 2), script.Eval("'hello'.Substring(1, 2)"));
```

#### for
```C#
string s = @"
int total=0;
for(var i=1; i<=10; i++) {
	total += i;
}
total";
var script = new Script();
Assert.AreEqual(55, script.Eval(s));
```

#### foreach
```C#
string s = @"
int n=0;
var list = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
foreach(var item in list)
{
	if (item % 2 == 0) continue;
	if (item >10) break;
	n+=item;
}
n";

var script = new Script();
Assert.AreEqual(25, script.Eval(s));
// 编译缓存方式执行
Assert.AreEqual(25, script.Eval(s, -1));
```

#### while
```C#
string s = @"
int total=0;
int n = 1;
while(n <= 10) {
	total += n;
	n++;
}
total";
var script = new Script();
Assert.AreEqual(55, script.Eval(s));
```

#### case when
与C#中的switch语法不同，语句块中不需要break，并且整体语句返回对应when或default语句的值。
```C#
var s = @"
var n=1;
case(n) {
when 0:
when 1:
	int m=6;
	m+4;
when 2: 20;
default:
	int m=9
	m+21;
}
";
var script = new Script();
Assert.AreEqual(10, script.Eval(s));
```

#### LINQ
* group
```C#
string s = @"
var persons = new[] { new Person(""tom"", 20), new Person(""jim"", 25), new Person(""san"", 18), new Person(""kit"", 25) }.AsQueryable();
var q = from a in persons
		group a by a.Age into g
		select new { g.Key, Count = g.Count() };
q.ToList()
";
var script = new Script();
script.Context.AddType<Person>();
var list = script.Eval<IList>(s);
Assert.AreEqual(3, list.Count);
dynamic d0 = list[0];
Assert.AreEqual(20, d0.Key);
Assert.AreEqual(1, d0.Count);
dynamic d1 = list[1];
Assert.AreEqual(25, d1.Key);
Assert.AreEqual(2, d1.Count);
dynamic d2 = list[2];
Assert.AreEqual(18, d2.Key);
Assert.AreEqual(1, d2.Count);
```
* join
```C#
from p in persons
join a in addresses on p.Id equals a.UserId
where p.Age > 20
```
* left join（AScript定制语法）
```C#
// 写法1
from p in persons
join a in addresses on p.Id equals a.UserId into aa
from a in aa.DefaultIfEmpty()
// 写法2
from p in persons
left join a in addresses on p.Id equals a.UserId
```
* right join（AScript定制语法）
```C#
from p in persons
right join a in addresses on p.Id equals a.UserId
```
实际会转换为left join：
```C#
from a in addresses
left join p in persons on a.UserId equals p.Id
```
* orderby
asc/desc为AScript定制语法，也支持C#标准语法ascending/descending
```C#
from p in persons
orderby p.Age desc
```

#### Queryable扩展方法
Queryable中的扩展方法，包含Where/Select/OrderBy/Skip/Take/FirstOrDefault/SelectMany/Join等等。
注：.NET10新增了LeftJoin/RightJoin扩展方法
```C#
string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Where(a=>a%2==0).ToList();
";
var script = new Script();
var r = script.Eval(s);
Assert.IsInstanceOfType(r, typeof(List<int>));
Assert.AreEqual("2,4", string.Join(',', (List<int>)r));
```

#### Enumerable扩展方法
Enumerable中的扩展方法，包含Where/Select/OrderBy/Skip/Take/FirstOrDefault/SelectMany/Join等等。
```C#
string s = @"
var r = [1,2,3,4,5].Where(a=>a%2==0).ToList();
";
var script = new Script();
var r = script.Eval(s);
Assert.IsInstanceOfType(r, typeof(List<int>));
Assert.AreEqual("2,4", string.Join(',', (List<int>)r));
```

#### static语句
static语句是在编译期间进行解析执行，即static语句不参与编译，而是直接执行结果。
注意：static语句中使用的变量也必须在static语句中定义的，否则报错变量不存在。
```C#
var s = @"
static int n = 10; // 直接执行，不参与编译
static x = n * 2; // 直接执行，不参与编译
int y = static n * 2; // 编译结果：y = 20
/*
int m=20;
int a = static m * 2; // 报错：variable m is not exists
*/
int z = n * 2; // 编译结果：z = n * 2
x+y+z;
";
var script = new Script();
// 编译
var func = script.Compile<int>(s);
Assert.AreEqual(10, script.Context.EvalVar("n"));
Assert.AreEqual(20, script.Context.EvalVar("x"));
Assert.IsNull(script.Context.EvalVar("y"));
Assert.IsNull(script.Context.EvalVar("z"));
// 执行
Assert.AreEqual(60, func());
```

#### 自定义语法解析

* 注：自定义语法解析需要谨慎处理，否则会破坏后续语法解析，导致脚本执行异常或者执行结果不符合预期。

```C#
string s = @"
int n=0;
var list = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
foreach(var item in list)
{
	if (item % 2 == 0) 继续;
	if (item >10) 中断;
	n+=item;
}
n";

var script = new Script();
script.Context.AddTokenHandler("继续", ContinueTokenHandler.Instance);
script.Context.AddTokenHandler("中断", BreakTokenHandler.Instance);
Assert.AreEqual(25, script.Eval(s));
```

#### 多脚本语言
脚本中使用`@lang/@end`语法嵌入其他语言，如果没有`@end`，则表示后面的脚本都指定该语言执行。
```C#
string s = @"
int n=10;
@lang python3
def sum(a,b):
	return a+b
@end
@lang sql
declare m int;
set m=20;
@end
sum(m, n)";
var script = new Script();
Assert.AreEqual(30, script.Eval(s));
```

#### 编译
```C#
var script = new Script();
//var func = script.Compile<int, int, int>("a+b*2", "a", "b");
var func = script.Compile<Func<int, int, int>>("a+b*2", new[] { "a", "b" });
Assert.IsNotNull(func);
Assert.AreEqual(11, func(3, 4));
```

#### Lambda
脚本生成Lambda表达式，应用场景：脚本->LINQ
```C#
var script = new Script();
var whereCondition = script.Lambda<Person, bool>("p.Name=='tom' || p.Name=='jim'", "p");
IQueryable<Person> query = ...;
var list = query.Where(whereCondition).ToList();
```

#### 异步和await
```C#
Func<int, int, Task<int>> sum = async (a, b) =>
{
	await Task.Delay(1000);
	return a + b;
};
var script = new Script();
script.Context.AddFunc("sum", sum);
// 方式一：EvalAsync异步执行await脚本，传递async/await到脚本
var result = await script.EvalAsync("await sum(5, 10)");
Assert.AreEqual(15, result.Value);
Assert.AreEqual(typeof(int), result.Type);
// 方式二：Eval同步执行await脚本，脚本实际调用Task.Wait() + Task<T>.Result
var result = script.Eval("await sum(5, 10)", out var type);
Assert.AreEqual(15, result);
Assert.AreEqual(typeof(int), type);
// 超时取消
var cts = new CancellationTokenSource(100);
await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
{
	await script.EvalAsync("(await sum(5, 10)) + 5", cancellationToken: cts.Token);
});
// @@CancellationToken关键字：异步执行值为参数中的cts.Token，同步执行值为CancellationToken.None
await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
{
	await script.EvalAsync("await Task.Delay(1000, @@CancellationToken)", cancellationToken: cts.Token);
});
```

#### 扩展构造函数
方法名格式：new_{类型名称}_
```
string s = @"
// 脚本中扩展构造函数
Person new_Person(string firstName, string lastName) {
	var name = firstName + lastName;
	return new Person { Name = name };
}
// 调用脚本中扩展的构造函数
var p1 = new Person('张', '三');
// 调用外部扩展的构造函数
var p2 = new Person('李四');
";
var script = new Script();
script.Context.AddType<Person>();
script.Context.AddFunc<string, Person>("new_Person", name => new Person { Name = name });
script.Eval(s);
var p1 = script.Eval<Person>("p1");
var p2 = script.Eval<Person>("p2");
Assert.AreEqual("张三", p1.Name);
Assert.AreEqual("李四", p2.Name);
```

#### 扩展静态方法
方法名格式：{类型名称}_{方法名}
```
string s = @"
// 脚本中扩展静态方法
Person Person_Create(string firstName, string lastName) {
	var name = firstName + lastName;
	return new Person { Name = name };
}
// 调用脚本中扩展的静态方法
var p1 = Person.Create('张', '三');
// 调用外部扩展的静态方法
var p2 = Person.Create('李四');
";
var script = new Script();
script.Context.AddType<Person>();
script.Context.AddFunc<string, Person>("Person_Create", name => new Person { Name = name });
script.Eval(s);
var p1 = script.Eval<Person>("p1");
var p2 = script.Eval<Person>("p2");
Assert.AreEqual("张三", p1.Name);
Assert.AreEqual("李四", p2.Name);
```

#### 扩展实例方法
方法第1个参数为要扩展的实例类型参数
```
string s = @"
// 脚本中扩展实例方法
string SayHi(Person p, string yourName) {
	var hi = $'hi {yourName}, my name is {p.Name}';
	return hi;
}
var p = new Person('tom', 20);
// 调用脚本中扩展的实例方法
var hi = p.SayHi('john');
// 调用外部扩展的实例方法
var night = p.SayGoodNight('john');
";
var script = new Script();
script.Context.AddType<Person>();
script.Context.AddFunc<Person, string, string>("SayGoodNight", (p, yourName) => $"good night {yourName}, my name is {p.Name}");
script.Eval(s);
var hi = script.Eval<string>("hi");
var night = script.Eval<string>("night");
Assert.AreEqual("hi john, my name is tom", hi);
Assert.AreEqual("good night john, my name is tom", night);
```

#### 扩展静态属性
暂只支持扩展读属性，方法名格式：{类名}_get_{属性名}
```
string s = @"
// 脚本中扩展静态属性
string Person_get_DefaultName() {
	return 'ABC';
}
Person.DefaultName;
";
var script = new Script();
script.Context.AddType<Person>();
Assert.AreEqual("ABC", script.Eval(s));
```

#### 扩展实例属性
暂只支持扩展读属性，方法第1个参数为该类型参数，方法名格式：get_{属性名}
```
string s = @"
// 脚本中扩展实例属性
string get_FullInfo(Person p) {
	var info = $'name:{p.Name},age:{p.Age}';
	return info;
}
var p = new Person('tom', 20);
// 调用脚本中扩展的实例属性
var fullInfo = p.FullInfo;
";
var script = new Script();
script.Context.AddType<Person>();
Assert.AreEqual("name:tom,age:20", script.Eval(s));
```

#### 脚本模块
接口IScriptModule定义脚本模块，集中管理变量、方法、类型等，这在自定义其他脚本语言时非常有用，比如js语言。
* 定义扩展PersonExtensions
```
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
```
* 定义模块PersonModule
```
public class PersonModule : IScriptModule
{
	public void Install(BaseContext context)
	{
		context.AddType<Person>();
		context.AddFunc(typeof(PersonExtensions));
	}

	public void Uninstall(BaseContext context)
	{
	}
}
```
* 向上下文或指定语言添加模块
```
var script = new Script();
script.Context.AddModule(new PersonModule());
// 或者指定语言添加模块
// CSharpLang.Instance.AddModule(new PersonModule());
```
* 脚本中调用扩展
```
string s = @"
var p1 = new Person('张', '三');
var p2 = Person.Create('李', '四');
var hi = p1.SayHi(Person.DefaultName);
var info = p2.FullInfo;
";
script.Eval(s);
var hi = script.Eval("hi");
var info = script.Eval("info");
Assert.AreEqual("hi ABC, my name is 张三", hi);
Assert.AreEqual("name:李四,age:18", info);
```