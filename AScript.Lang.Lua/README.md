# AScript.Lang.Lua

基于 AScript 扩展的 Lua 脚本语言，支持 Lua 基础语法和数据类型，以及表、元表、面向对象、模块等功能。

## 安装
```
install-package AScript
install-package AScript.Lang.Lua
```

## 使用说明
* 命名空间：using AScript.Lang.Lua;
* 常用数据类型：
    - `nil`: 空值
    - `boolean`: 布尔值 (`true`, `false`)
    - `number`: 数值 (整数和浮点数)
    - `string`: 字符串
    - `table`: 表 (关联数组)
* 运算符
    - 算术运算符: `+`, `-`, `*`, `/`, `%`, `^` (幂), `//` (整数除法)
    - 关系运算符: `<`, `>`, `<=`, `>=`, `==`, `~=`
    - 逻辑运算符: `and`, `or`, `not`
    - 连接运算符: `..` (字符串连接), `#` (长度)
    - 赋值运算符: `=`, `+=`, `-=`, `*=`, `/=`, `%=`, `^=`, `..=`
* 控制结构
    - 条件语句: `if ... then ... elseif ... then ... else ... end`
    - while循环: `while ... do ... end`
    - repeat循环：`repeat ... until ...`
    - 数值for循环: `for i = start, end, step do ... end`
    - 泛型for循环: `for v in expr do ... end`
    - 泛型for循环: `for i,v in expr do ... end`
* 函数定义: `function name(args) ... end`
* 局部变量: `local var = value`
* 表构造器
    - 数组风格: `{ value1, value2, value3 }`
    - 字典风格: `{ key1 = value1, key2 = value2 }`
    - 混合风格: `{ 1, 2, name = "test" }`

#### 注册Lua语言
```
Script.Langs.Set("lua", LuaLang.Instance);
// 可全局设置为默认语言
// Script.Langs.Set("lua", LuaLang.Instance, setDefault: true);
```

#### 上下文中指定Lua语言
如果已全局设置默认语言则无需指定
```
var s = @"
function sum(a,b) {
	return a+b
}
sum(10,20)
";
var script = new Script();
script.Context.Langs = new [] { "lua" };
Assert.AreEqual(30L, script.Eval(s));
```

#### 使用@lang指定Lua语言
```
var s = @"
// 默认csharp语言
int mult(int a, int b) => a*b;
// 嵌入lua语言
@lang lua
function sum(a,b) {
	return a+b
}
@end
int m = 10;
int n = 20;
mult(m, n) + sum(m, n);
";
var script = new Script();
Assert.AreEqual(230, script.Eval(s));
```

#### 字符串连接
两个字符串连接使用`..`操作符而不是+号操作符。
```
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual("hello123", script.Eval("'hello'..123"));
```


### 条件语句
```lua
if a > b then
    print("a is greater")
elseif a == b then
    print("equal")
else
    print("b is greater")
end
```

### while循环
```lua
local i = 1
while i <= 10 do
    print(i)
    i = i + 1
end
```

### 数值for循环
```

```

### 泛型for循环
```

```

### 函数
```lua
function factorial(n)
    if n <= 1 then
        return 1
    end
    return n * factorial(n - 1)
end
```

### 表
```lua
local arr = {1, 2, 3, 4, 5}
local dict = {name = "test", value = 100}
```

### 元表

### 面向对象(OOP)

### 模块

### io模块