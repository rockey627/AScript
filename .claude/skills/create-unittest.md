# Create UnitTest Skill

## 触发条件

当用户请求生成单元测试时触发，例如：
- "生成单元测试"
- "创建单元测试"
- "generate unit test"
- "create unittest"

## 输入参数

- `language`: 目标脚本语言（如：Python3, JavaScript, CSharp, SQL, Lua, Ruby, Go 等）

## 测试方法命名规则

参考 `GoArrayTest.cs`，测试方法命名规则如下：
- `Test{NN}_{FeatureName}` - 普通执行模式测试
- `Test{NN}_{FeatureName}_CompileAll` - 编译模式测试

其中：
- `NN` 是从 01 开始的序号
- `FeatureName` 是功能特性名称，采用 PascalCase 风格

## 测试类结构

每个测试类应包含：

### 1. 类初始化和清理
```csharp
[ClassInitialize]
public static void Init(TestContext context)
{
    Script.Langs["{langKey}"] = {LangClass}.Instance;
}

[ClassCleanup]
public static void Cleanup()
{
    Script.Langs.TryRemove("{langKey}");
}
```

### 2. 普通测试和编译模式测试配对

每个功能特性生成两个测试方法：
- 普通模式：测试运行时解析执行
- CompileAll 模式：测试编译缓存执行

```csharp
[TestMethod]
public void Test01_FeatureName()
{
    var s = @"{script code}";
    var script = new Script();
    script.Context.Langs = new[] { "{langKey}" };
    // assertions
}

[TestMethod]
public void Test01_FeatureName_CompileAll()
{
    var s = @"{script code}";
    var script = new Script();
    script.Options.CompileMode = ECompileMode.All;
    script.Context.Langs = new[] { "{langKey}" };
    // assertions
}
```

## 测试覆盖范围

根据脚本语言的语法特性，覆盖以下测试场景：

### 基础语法
1. 变量声明与赋值
2. 基础数据类型（整数、浮点、字符串、布尔）
3. 算术运算符
4. 比较运算符
5. 逻辑运算符
6. 位运算符

### 复合类型
1. 数组/列表声明与初始化
2. 数组/列表索引访问与赋值
3. 数组/列表切片操作
4. 数组/列表长度计算
5. 多维数组/嵌套结构
6. 字典/映射/对象

### 控制流
1. if/else 条件语句
2. for/while 循环
3. break/continue 语句
4. switch/match 语句
5. 三元/条件表达式

### 函数
1. 函数定义与调用
2. 函数参数传递
3. 函数返回值
4. 匿名函数/Lambda
5. 闭包
6. 递归函数

### 高级特性
1. 字符串操作（拼接、切片、格式化）
2. 内置函数（len, type, range 等）
3. 异常/错误处理
4. 类与对象（如果支持）
5. 模块/导入（如果支持）

## 输出

生成完整的测试类文件，保存在 `AScript.Test.MSTests/{Language}/` 目录下，文件名为 `{Language}{Feature}Test.cs`

## 示例

用户输入：`生成 Python3 列表的单元测试`

输出：生成 `AScript.Test.MSTests/Python3/Python3ListTest.cs`，包含 Python3 列表的全面测试用例
