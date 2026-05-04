# AScript.Lang.Python3

#### 介绍
python3语法

#### 安装
```
install-package AScript
install-package AScript.Lang.Python3
```

#### 使用说明
* 命名空间：using AScript.Lang.Python3;
* 已内置python3常用数据类型：int/float/bool/str/list/set/dict

###### 注册python3语言
```
Script.Langs.Set("python3", Python3Lang.Instance);
// 可全局设置为默认语言
// Script.Langs.Set("python3", Python3Lang.Instance, setDefault: true);
```

###### 上下文中指定python3语言
如果已全局设置默认语言则无需指定
```
var script = new Script();
script.Context.Langs = new [] { "python3" };
var s = @"
def sum(a,b):
	return a+b
sum(10,20)
";
Assert.AreEqual(30L, script.Eval(s));
```

###### 使用@lang指定python3语言
```
var s = @"
// 默认csharp语言
int mult(int a, int b) => a*b;
// 嵌入python3语言
@lang python3
def sum(a,b):
	return a+b
@end
int m = 10;
int n = 20;
mult(m, n) + sum(m, n);
";
var script = new Script();
Assert.AreEqual(230, script.Eval(s));
```

###### 字符串内插值
```
string s = @"
name='tom'; 
f'hello {name}, 5+8={5+8}'
";
var script = new Script();
script.Context.Langs = new [] { "python3" };
Assert.AreEqual("hello tom, 5+8=13", script.Eval(s));
```

###### 字符串索引和截取
```
var script = new Script();
script.Context.Langs = new [] { "python3" };
Assert.AreEqual('e', script.Eval("'hello'[1]"));
Assert.AreEqual('e', script.Eval("'hello'[-4]"));
Assert.AreEqual("ell", script.Eval("'hello'[1:4]"));
Assert.AreEqual("ell", script.Eval("'hello'[-4:-1]"));
```

###### 数组
```
var script = new Script();
script.Context.Langs = new [] { "python3" };
var result1 = (List<object>)script.Eval("var list1 = [0,1,2,3,4]; list1[1:4]");
var result2 = (List<object>)script.Eval("var list2 = [0,1,2,3,4]; list2[-4:-1]");
CollectionAssert.AreEqual(new List<object> { 1, 2, 3 }, result1);
CollectionAssert.AreEqual(new List<object> { 1, 2, 3 }, result2);
Assert.AreEqual(1, script.Eval("arr1[1]"));
Assert.AreEqual(1, script.Eval("arr1[-4]"));
```

###### 字典
```
var s = @"
p = {'name': '张三', 'age': 18}
p['age']=20
p
";
var script = new Script();
script.Context.Langs = new [] { "python3" };
var dict = script.Eval<Dictionary<object, object>>(s);
Assert.AreEqual(2, dict.Count);
Assert.AreEqual("张三", dict["name"]);
Assert.AreEqual(20L, dict["age"]);
```

###### 集合
集合会自动去重。
```
var s = @"
s = {1, 2}
s.add(3)
s.add(2)
s
";
var script = new Script();
script.Context.Langs = new [] { "python3" };
var set = script.Eval<HashSet<object>>(s);
Assert.AreEqual(3, set.Count);
```

###### for遍历值
```
var code = @"
total = 0
for x in [1, 2, 3]:
	total += x
total
";
var script = new Script();
script.Context.Langs = new[] { "python3" };
Assert.AreEqual(6L, script.Eval(code));
```

###### for遍历值和索引
```
var code = @"
result = ''
for i, x in enumerate([1, 2, 3]):
    result += f'{i}:{x},'
result
";
var script = new Script();
script.Context.Langs = new[] { "python3" };
Assert.AreEqual("0:1,1:2,2:3,", script.Eval(code));
```

###### 列表推导式
```
var code = @"[x * 2 for x in [1, 2, 3]]";
var script = new Script();
script.Context.Langs = new[] { "python3" };
var list = (List<object>)script.Eval(code);
Assert.AreEqual(3, list.Count);
Assert.AreEqual(2L, list[0]);
Assert.AreEqual(4L, list[1]);
Assert.AreEqual(6L, list[2]);
```

###### lambda
```
string s = @"
f = lambda a,b: a+b
f(10,20)
";
var script = new Script();
script.Context.Langs = new[] { "python3" };
Assert.AreEqual(30L, script.Eval(s));
```

###### 类型注解
指定变量类型及函数返回值类型。
```
var script = new Script();
script.Context.Langs = new [] { "python3" };
var s = @"
def sum(a:int,b:int)->int:
	return a+b
m:int=10
n:int=20
sum(m,n)
";
Assert.AreEqual(30L, script.Eval(s));
```