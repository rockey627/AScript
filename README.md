# AScript

#### 介绍

动态脚本解析与计算

* 支持表达式运算
* 支持注入变量
* 支持定义变量
* 支持注入函数
* 支持定义函数
* 支持注入类型
* 支持C#语法
* 支持16进制整数表示：0x0A
* 支持多语句：用分号分隔多条语句
* 支持行注释：// 行注释
* 支持块注释：/* 块注释 */
* 支持if/else语句
* 支持for/while/foreach语句
* 支持continue/break/return
* 支持自定义语法解析（自定义关键字）
* 支持流式读取表达式
* 支持自定义语言环境，默认已实现CSharpLang
* 支持3种执行模式：
1. 解析执行：解析过程中计算结果，对于非循环语句有较高的性能及低内存，有循环语句则建议使用第2种编译执行方式
```C#
var script = new Script();
var result = script.Eval("5+8*6");
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
3. 先构建完整表达式树，再执行或编译执行（不建议该方式，内存消耗相对较高）
* 上下文环境会缓存临时变量及函数，编译执行模式可关闭该缓存（获得更高的性能）
```C#
var script = new Script();
script.Options.CompileMode = ECompileMode.All;
script.Options.ThrowIfVariableNotExists = true;
//script.Options.RewriteVariables = false;
//script.Options.RewriteFunctions = false;
script.Eval("int sum(int a, int b)=>a+b;int n=10;sum(n,5)");
script.Eval("n+6"); // 如果RewriteVariables为false，则抛异常变量n不存在
script.Eval("sum(10,20)"); // 如果RewriteFunctions为false，则抛异常函数sum不存在
var sum = script.Context.GetFunc<int, int, int>("sum");
int result = sum(10, 20);
```

#### 安装

install-package AScript

#### 使用说明

* 命名空间：using AScript;
* 已内置C#常用数据类型，如：int/bool/string/long/double/DateTime等
* 已内置Convert数据转换方法，使用示例：'12'.ToInt32() 等同于 ToInt32('12') 或者 Convert.ToInt32('12')

###### 变量

```C#
var script = new Script();
script.Context.SetVar("m", 6);
var result = script.Eval("int n=8;n+m+10*(3+0x0A)");
Assert.AreEqual(8 + 6 + 10 * (3 + 0x0A), result);
```

###### 注入函数
```C#
var script = new Script();
script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
script.Context.AddFunc<int, int, int, int>("sum", (a, b, c) => a + b + c);
script.Context.AddFunc<int, int, int>("mult", (a, b) => a * b);
Assert.AreEqual(1+2+8+1+2+3+5*6, script.Eval("sum(1,2)+8+sum(1,2,3)+mult(5,6)"));
```

###### 自定义函数
```C#
var script = new Script();
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
var result = script.Eval(s);
Assert.AreEqual(1 + 2 + 8 + 1 + 2 + 3 + 5 * 6, result);
Assert.AreEqual(1 + 2 + 8, script.Eval("sum(1,2)+8"));
Assert.AreEqual(5 * 10 + 2 + 8, script.Eval("exec(5,2)+8"));
```

###### 递归
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

###### 点操作符
```C#
var script = new Script();
Assert.AreEqual(DateTime.Now.Year, script.Eval("DateTime.Now.Year"));
Assert.AreEqual(int.MaxValue, script.Eval("int.MaxValue"));
Assert.AreEqual("hello".Length, script.Eval("'hello'.Length"));
Assert.AreEqual("hello".Substring(1, 2), script.Eval("'hello'.Substring(1, 2)"));
```

###### foreach
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

int n = 0;
var list = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
foreach (var item in list)
{
	if (item % 2 == 0) continue;
	if (item > 10) break;
	n += item;
}

var script = new Script();
Assert.AreEqual(n, script.Eval(s));
// 编译缓存方式执行
Assert.AreEqual(n, script.Eval(s, -1));
```

###### 自定义语法解析

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

#### 参与贡献

1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request


#### 特技

1.  使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2.  Gitee 官方博客 [blog.gitee.com](https://blog.gitee.com)
3.  你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解 Gitee 上的优秀开源项目
4.  [GVP](https://gitee.com/gvp) 全称是 Gitee 最有价值开源项目，是综合评定出的优秀开源项目
5.  Gitee 官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6.  Gitee 封面人物是一档用来展示 Gitee 会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)
