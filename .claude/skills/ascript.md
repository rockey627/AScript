# AScript Skill

## 概述

AScript 是一个 C# 动态脚本解析、编译和执行引擎，支持注入变量、函数、类型，以及 LINQ、Lambda、异步等特性。

## 项目结构

```
AScript/                    # 核心引擎
├── Script.cs               # 主入口类
├── ScriptContext.cs        # 脚本上下文
├── BaseContext.cs          # 基础上下文
├── ExpressionUtils.cs      # 表达式工具
├── Nodes/                  # AST 节点
├── Operators/              # 操作符处理
├── Syntaxs/                # 语法分析
├── TokenHandlers/          # token 处理器
├── Lang/                   # 内置语言(CSharp/Python3/SQL/JavaScript)
AScript.Lang.*/             # 各语言实现
AScript.Test.MSTests/       # 单元测试
```

## 核心 API

### Script 类
```csharp
var script = new Script();
var result = script.Eval("5+8*6");                    // 解析执行
var result = script.Eval("5+8*6", ECompileMode.All);  // 编译执行
var result = script.Eval("5+8*6", -1);                // 编译缓存执行
```

### 上下文操作
```csharp
script.Context.SetVar("m", 6);                        // 设置变量
script.Context.AddFunc<int, int, int>("sum", (a,b) => a + b);  // 添加函数
script.Context.AddType<Person>();                      // 注入类型
script.Context.AddTokenHandler("继续", ContinueTokenHandler.Instance);  // 自定义语法
```

### 编译与 Lambda
```csharp
var func = script.Compile<Func<int, int, int>>("a+b*2", new[] { "a", "b" });
var whereCondition = script.Lambda<Person, bool>("p.Name=='tom'", "p");
```

## 常见任务

### 变量操作
```csharp
script.Context.SetVar("name", "tom");
script.Context.SetVar("age", 20);
var name = script.Eval<string>("name");
var age = script.Eval<int>("age");
```

### 类型注入
```csharp
script.Context.AddType<Person>();
script.Eval("var p = new Person('tom', 20); p.SayHello()");
```

### 自定义函数
```csharp
script.Context.AddFunc<int, int, int>("mult", (a, b) => a * b);
var result = script.Eval("mult(5, 6)");
```

### 异步执行
```csharp
var result = await script.EvalAsync("await sum(5, 10)");
var result = script.Eval("await sum(5, 10)", out var type);
```

### 多语言嵌入
```csharp
var s = @"
int n=10;
@lang python3
def sum(a,b):
    return a+b
@end
sum(n, 5)";
```

### 自定义语法
```csharp
script.Context.AddTokenHandler("继续", ContinueTokenHandler.Instance);
script.Context.AddTokenHandler("中断", BreakTokenHandler.Instance);
```

## 测试

```bash
dotnet test AScript.Test.MSTests/AScript.Test.MSTests.csproj --filter "FullyQualifiedName~ScriptCommonTest"
dotnet test AScript.Test.MSTests/AScript.Test.MSTests.csproj --filter "FullyQualifiedName~Python3"
```

## 构建

```bash
dotnet build AScript.sln -c Release
dotnet build AScript/AScript.csproj -c Release
```

## 关键文件

- `Script.cs:1` - 主入口，Eval/Compile/Lambda 方法
- `ScriptContext.cs:1` - 上下文管理，变量/函数/类型注入
- `BaseContext.cs:1` - 基础上下文，TokenHandler 管理
- `ExpressionUtils.cs:1` - 表达式构建工具
- `ScriptLang.cs:1` - 语言注册与管理
