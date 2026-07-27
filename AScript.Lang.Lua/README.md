# AScript.Lang.Lua

基于 AScript 扩展的 Lua 脚本语言，支持 Lua 基础语法及数据类型。

## 支持的功能

### 数据类型
- `nil`: 空值
- `boolean`: 布尔值 (`true`, `false`)
- `number`: 数值 (整数和浮点数)
- `string`: 字符串
- `table`: 表 (关联数组)
- `function`: 函数

### 运算符
- 算术运算符: `+`, `-`, `*`, `/`, `%`, `^` (幂), `//` (整数除法)
- 关系运算符: `<`, `>`, `<=`, `>=`, `==`, `~=`
- 逻辑运算符: `and`, `or`, `not`
- 连接运算符: `..` (字符串连接), `#` (长度)
- 赋值运算符: `=`, `+=`, `-=`, `*=`, `/=`, `%=`, `^=`, `..=`

### 控制结构
- 条件语句: `if ... then ... elseif ... then ... else ... end`
- 循环语句: `while ... do ... end`, `repeat ... until ...`, `for ... do ... end`
- 数值for循环: `for i = start, end, step do ... end`
- 泛型for循环: `for var in expr do ... end`

### 函数
- 函数定义: `function name(args) ... end`
- 局部变量: `local var = value`

### 表构造器
- 数组风格: `{ value1, value2, value3 }`
- 字典风格: `{ key1 = value1, key2 = value2 }`
- 混合风格: `{ 1, 2, name = "test" }`

## 快速开始

```csharp
// 注册lua脚本语言
Script.Langs.Set("lua", LuaLang.Instance);

// 执行lua脚本
string s = @"
@lang lua
function sum(a, b)
    return a + b
end
local n = 10
sum(n, 5)
";

var script = new Script();
var result = script.Eval(s);
// result == 15
```

## 示例

### 变量和运算
```lua
local a = 10
local b = 3
local c = a + b * 2
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
