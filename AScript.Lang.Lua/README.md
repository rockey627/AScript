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
* 匿名函数：`local f = function(args) ... end`
* 局部变量: `local var = value`
* 表构造器
    - 数组风格: `{ value1, value2, value3 }`
    - 字典风格: `{ key1 = value1, key2 = value2 }`
    - 混合风格: `{ 1, 2, name = "test" }`
* 单行注释：-- 单行注释
* 多行注释：--[[ 多行注释 ]]

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
两个字符串连接使用`..`操作符而不是`+`操作符。
```
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual("hello123", script.Eval("'hello'..123"));
```

#### 数据打包/解包
* pack打包
```
string s = @"
local format='<i4i4fc10'
local hp=12500
local mp=8700
local atk=156.5
local name='john'
local data = string.pack(format, hp, mp, atk, name)
print(#data) -- 控制台输出data长度
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
script.Eval(s);
```
* unpack解包
```
script.Eval("local a,b,c,d=string.unpack(format, data)");
Assert.AreEqual(12500, script.Eval("a"));
Assert.AreEqual(8700, script.Eval("b"));
Assert.AreEqual(156.5, script.Eval("c"));
Assert.AreEqual("john", script.Eval("d"));
```

#### 条件语句
```
string code = @"
local x = 10
if x >= 10 and x < 50 then
	local a = 5
	local b = 15
	x = a + b + 80
elseif x >= 50 and x < 100 then
	x = 200
else
	x = 300
end
x
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual(100L, script.Eval(code));
```

#### while循环
```
string code = @"
local i = 0
local sum = 0
while i < 5 do
	i = i + 1
	sum = sum + i
end
sum
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual(15L, script.Eval(code));
```

#### 数值for循环
* 语法：`for i=start,end[,step] do ... end`，其中step默认为1
* 注：循环体内部改变循环变量i的值，不影响循环次数
```
string code = @"
local sum = 0
for i = 1, 5 do
	i = i + 10
	sum = sum + i
end
sum
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual(65L, script.Eval(code));
```

#### 泛型for循环
```
string code = @"
local sum = 0
-- for v in ipairs({1, 2, 3, 4, 5}) do
for i, v in ipairs({1, 2, 3, 4, 5}) do
	sum = sum + v
end
sum
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual(15L, script.Eval(code));
```

#### 函数
```
string code = @"
function factorial(n)
	if n <= 1 then
		return 1
	end
	return n * factorial(n - 1)
end
factorial(5)
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual(120L, script.Eval(code));
```

#### 表（数组）
```
string code = @"
local arr = {10, 20, 30}
arr[1] + arr[2] + arr[3]
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual(60L, script.Eval(code));
```

#### 表（字典）
```
string code = @"
local t = { x = 3, y = 5 }
t.sum = function(a,b) 
	print(a,b)
	return a+b 
end
t.sum(t.x,t.y)
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual(8L, script.Eval(code));
```

#### 元表
```
string code = @"
local t = {}
local metatable = { __index = { name = 'tom' } }
setmetatable(t, metatable)
t.name
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual("tom", script.Eval(code));
```

#### 面向对象(OOP)
```
string code = @"
-- 定义动物类（Animal）
Animal = {name = 'Unknown'}

-- Animal 类的构造函数
function Animal:new(name)
  local o = {}  -- 如果没有传入对象，则创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Animal 的方法
  self.__index = self  -- 让对象可以访问 Animal 的方法
  o.name = name or 'Unknown'  -- 设置名称，默认为 'Unknown'
  return o
end

-- Animal 类的方法：叫声
function Animal:speak()
  return self.name .. ' makes a sound.'
end


-- 定义狗类（Dog），继承自 Animal
Dog = Animal:new()  -- Dog 继承 Animal 类

-- 重写狗类的构造函数
function Dog:new(name, breed)
  local o = {}  -- 如果没有传入对象，则创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Dog 和 Animal 的方法
  self.__index = self  -- 让对象可以访问 Dog 的方法
  o.name = name or 'Unknown'
  o.breed = breed or 'Unknown'
  return o
end

-- 重写狗类的叫声方法（重写 Animal 的 speak 方法）
function Dog:speak()
  return self.name .. ' barks.'
end

-- 创建 Animal 对象
local animal = Animal:new('Generic Animal')
local s1 = animal:speak()  -- 输出 'Generic Animal makes a sound.'

-- 创建 Dog 对象
local dog = Dog:new('Buddy', 'Golden Retriever')
local s2 = dog:speak()  -- 输出 'Buddy barks.'
s1 .. ';' .. s2
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual("Generic Animal makes a sound.;Buddy barks.", script.Eval(code));
```

#### 模块
使用`require`引入模块，优先查找`IScriptModule`模块，再查找模块目录中的模块文件。

1. 添加模块目录
```
LuaLang.Instance.Modules.AddDir("./lua/modules");
```

2. 模块目录中添加`Person.lua`文件
```
Person = {name = '', age = 0}

function Person:new(name, age)
    local obj = {}  -- 创建一个新的表作为对象
    setmetatable(obj, self)  -- 设置元表，使其成为 Person 的实例
    self.__index = self  -- 设置索引元方法，指向 Person
    obj.name = name
    obj.age = age
    return obj
end

function Person:introduce()
	return 'My name is ' .. self.name .. ' and I am ' .. self.age .. ' years old.'
end

return Person
```

3. 引入并使用模块
```
string code = @"
require 'Person'
local person1 = Person:new('Alice', 30)
person1:introduce()
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
Assert.AreEqual("My name is Alice and I am 30 years old.", script.Eval(code));
```

#### io模块
文件操作，服务端请谨慎使用，避免脚本恶意修改、删除文件。

1. 安装模块
```
install-package AScript.Lang.Lua.io
```

2. 注册io模块
```
LuaLang.Instance.Modules.Add("io", new AScript.Lang.Lua.io.LuaIOModule());
```

3. 使用io
```
string code = $@"
require 'io'
local f = io.open(file, 'w')
f:write('hello')
f:close()
local f = io.open(file, 'r')
local content = f:read()
f:close()
content
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
script.Context.SetVar("file", "./test.txt");
Assert.AreEqual("hello", script.Eval(code));
```