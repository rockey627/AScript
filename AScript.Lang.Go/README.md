# AScript.Lang.Go

## 介绍
基于 AScript 扩展的 Go 脚本语言，支持 Go 基础语法与数据类型。
内置 `fmt`、`strconv`、`sync`、`regexp`、`time` 等常用标准库。

当前**不支持**：

* 泛型定义
* 接口（`interface`）类型定义
* 通道（`chan`）与 `select` 语句
* `goto`、`fallthrough`

## 安装
```
install-package AScript
install-package AScript.Lang.Go
```

## 使用说明
* 命名空间：using AScript.Lang.Go;

### 注册Go语言
```csharp
Script.Langs.Set("go", GoLang.Instance);
// 可全局设置为默认语言
// Script.Langs.Set("go", GoLang.Instance, setDefault: true);
```

### 数据类型
| Go 类型 | 底层 .NET 类型 |
| --- | --- |
| `int` / `int32` | `int` |
| `int8` | `byte` |
| `int16` | `short` |
| `int64` | `long` |
| `uint` / `uint32` | `uint` |
| `uint8` | `sbyte` |
| `uint16` | `ushort` |
| `uint64` | `ulong` |
| `float32` | `float` |
| `float64` | `double` |
| `bool` | `bool` |
| `string` | `string` |
| `byte` | `byte` |
| `rune` | `char` |
| `any` | `object` |

### 变量与常量
```csharp
var s = @"
var a = 5            // 声明并初始化
var b int = 10       // 带类型声明
var c, d = 1, 2      // 多变量声明
e := 3               // := 短变量声明
const f = 100        // 常量
var g string         // 声明变量
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

### 运算符
| 类别 | 运算符 |
| --- | --- |
| 算术 | `+` `-` `*` `/` `%` |
| 位运算 | `&` `\|` `^` `<<` `>>` `&^` |
| 比较 | `==` `!=` `<` `>` `<=` `>=` |
| 逻辑 | `&&` `||` `!` |
| 赋值 | `=` `:=` `+=` `-=` `*=` `/=` `%=` `&=` `|=` `^=` `<<=` `>>=` |
| 自增自减 | `++` `--` |
| 其他 | `.` `[]` `[:]` `&`（取地址） `*`（解引用） |

`&^` 为 Go 的"与非"运算符，优先级与 `&` 相同。

### 字符串
* 字符串使用单引号 `'...'` 或双引号 `"..."` 均可；
* 支持转义：`\n` `\r` `\t` `\\` `\"` `\'` 等；
* 支持反引号包裹的原始字符串（不做任何转义处理），推荐用于正则：

```
re := regexp.MustCompile(`\d+`)
```

### 流程控制
#### if
```csharp
var s = @"
var b int
if a > 0 {
    b = 1
} else if a < 0 {
    b = -1
} else {
    b = 0
}
";
var script = new Script();
script.Context.Lang = "go";
script.Context.SetVar("a", 5);
script.Eval(s);
```

* 支持 `if` / `else if` / `else`、嵌套 if、以及 `if x := 1; x > 0 { ... }` 的带初始化语句写法。

#### for
```csharp
var s = @"
// 传统 for 循环
for i := 0; i < 10; i++ {
}

// while 型 for
for i < 10 {
    i++
}

// 无限循环
for {
    break
    continue
    return
}

// range 循环
for k, v := range m {
}
for i := range arr {
}
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

#### switch
不支持 `fallthrough`
```csharp
var s = @"
switch x {
case 1:
    y = 1
case 2, 3:
    y = 2
default:
    y = 0
}
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

#### break / continue / return
`break`、`continue` 可用于 `for` 循环，`return` 用于函数返回。

### 函数
```csharp
var s = @"
// 函数声明
func add(a int, b int) int {
    return a + b
}

// 无返回值
func log(msg string) {
    fmt.Println(msg)
}

// 匿名函数（立即调用）
func() {
    fmt.Println('hi')
}()

// 多返回值
func div(a int, b int) (int, string) {
    return a / b, ''
}

// 命名返回值
func named() (result int) {
    result = 1
    return
}
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

* 支持匿名函数、闭包、多返回值与命名返回值。
* 函数作为值：可赋值给变量（`f := func(a int) int { return a }`）。

### defer
* `defer` 语句在这里是在 **语句块结束时执行**，与go语言不一样，go语言是在函数退出时执行
* 多个 `defer` 按**后进先出**（LIFO）顺序执行。
```csharp
var s = @"
import 'fmt'

func work() {
    defer fmt.Println('deferred')
    fmt.Println('body')
}
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

### go
`go` 语句用于启动一个并发调用：
```csharp
var s = @"
import 'fmt'
go func() {
    fmt.Println('in goroutine')
}()
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

### 数组与切片
```csharp
var s = @"
// 固定长度数组
var a = [5]int{1,2,3,4,5}

// 自动推断长度
var b = [...]int{1,2,3,4,5}

// 切片
var c = []int{1,2,3,4,5}

// 空数组 / 空切片
var d = [5]int{}
var e = []int{}

// make 创建切片
var f = make([]int, 5, 10)

// 下标访问与赋值
a[0] = 10

// 切片表达式
var g = c[1:3]
var h = c[:2]
var i = c[2:]

// 二维数组
var m = [2][3]int{{1,2,3},{4,5,6}}
m[1][2] = 100

// 遍历
for i, v := range a {
}
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

* **数组是值类型**：赋值或传参时会拷贝整个数组。

```
var s = @"
var b = [5]int{1,2,3,4,5}
var c = b
c[0] = 10      // b[0] 仍为 1

// 通过数组指针传递引用
var d = &b
d[0] = 10      // b[0] 变为 10

func setarr(x *[5]int) {
    x[0] = 100
}
var arr = [5]int{1,2,3,4,5}
setarr(&arr)   // arr[0] 变为 100
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

### map
无序的键值对集合。
```csharp
var s = @"
var m = map[string]int{
    'apple': 1,
    'banana': 2,
    'orange': 3,
}

// make 创建
var m2 = make(map[string]int)

// 访问与赋值
m['apple'] = 10
var v = m['apple']

// 取值并判断是否存在
val, ok := m['apple']

// 删除
delete(m, 'apple')

// 长度
var n = len(m)

// 遍历
for k, v := range m {
}
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

### 结构体
```csharp
var s = @"
type Person struct {
    name string
    age int
}

// 定义方法
func (p Person) GetAge() int {
    return p.age
}

// 指针
func (p *Person) SetAge(age int) {
    p.age = age
}
// 创建实例
var p = Person{}

// 带初始值创建
var q = Person{name: 'John', age: 20}

// 字段访问与赋值
q.age = 25

// 调用方法
q.SetAge(28)
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

### 指针
```csharp
var s = @"
var a = 10

// 取地址
var p = &a

// 解引用
var v = *p

// 通过指针赋值
*p = 20

// 带类型的指针
var i int = 10
var pi *int = &i

// 多级指针
var pp = &p
var vv = **pp
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

### 内置函数
| 函数 | 说明 |
| --- | --- |
| `len(x)` | 字符串长度、数组/切片长度、map 元素个数 |
| `cap(x)` | 切片容量 |
| `append(s, v...)` | 返回追加元素后的新切片（最多可追加 3 个元素） |
| `copy(dst, src)` | 复制切片元素，返回复制的个数 |
| `delete(m, k)` | 删除 map 中的键 |
| `string(x)` | 转换为字符串 |
| `float64(x)` | 转换为浮点数 |
| `complex(real, imag)` | 创建复数 |
| `real(c)` / `imag(c)` | 取复数的实部 / 虚部 |
| `make([]T, len, cap)` | 创建切片 |
| `make(map[K]V)` | 创建 map |

### 标准库模块
需要先 `import` 才能使用

#### import
导入模块，优先检索内置模块，再检索目录文件模块。
```csharp
var s = @"
import 'fmt' // 导入单个模块
import ('fmt' 'strconv') // 导入多个模块
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

#### fmt
```csharp
var s = @"
import 'fmt'
fmt.Println('hello', 'world')
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

#### strconv
```csharp
var s = @"
import 'strconv'
n, err := strconv.Atoi('123')
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

* `strconv.Atoi(s)`：解析整数，成功返回 `(值, null)`，失败返回 `(0, 错误信息)`。

#### sync
```csharp
var s = @"
import 'sync'
var m sync.Mutex
m.Lock()
// ... 临界区
m.Unlock()
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

* `sync.Mutex` 提供 `Lock()`、`TryLock()`、`Unlock()`。

#### regexp
正则表达式相关功能。

```csharp
var s = @"
import 'regexp'

// 编译（表达式非法时抛出异常）
re := regexp.MustCompile(`\d+`)

// 编译（返回正则和错误信息）
re2, err := regexp.Compile(`\d+`)

// 是否匹配
re.MatchString('abc123')            // true
regexp.MatchString(`\d+`, 'abc123') // 包级函数

// 查找
re.FindString('abc123')        // '123'
re.FindStringIndex('abc123')   // [3, 6]
re.FindStringSubmatch('a=1')   // ['a=1', 'a', '1']
re.FindAllString('a1b22', -1)  // ['1', '22']

// 替换
re.ReplaceAllString('a1b22', '#')        // 'a#b#'
re.ReplaceAllString('a=1', '$2=$1')      // '1=a'

// 分割
re.Split('a1b22', -1)           // ['a', 'b', '']

// 捕获组
re3 := regexp.MustCompile(`(?P<key>\w+)=(?P<value>\w+)`)
re3.NumSubexp()                 // 2
re3.SubexpNames()               // ['', 'key', 'value']
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```

#### time
```csharp
var s = @"
import 'time'

var now = time.Now()   // 当前时间
time.Sleep(100)     // 休眠 100 毫秒
";
var script = new Script();
script.Context.Lang = "go";
script.Eval(s);
```
