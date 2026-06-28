# AScript.Lang.JavaScript

## 介绍
支持JavaScript基础语法和数据类型，以及数组、集合、字典、对象操作。

## 安装
```
install-package AScript
install-package AScript.Lang.JavaScript
```

## 使用说明
* 命名空间：using AScript.Lang.JavaScript;
* 已内置JavaScript常用数据类型：String/Set/Map/Array/Date/Math

#### 注册JavaScript语言
```
Script.Langs.Set("js", JavaScriptLang.Instance);
// 可全局设置为默认语言
// Script.Langs.Set("js", JavaScriptLang.Instance, setDefault: true);
```

#### 上下文中指定JavaScript语言
如果已全局设置默认语言则无需指定
```
var script = new Script();
script.Context.Langs = new [] { "js" };
var s = @"
function sum(a,b) {
	return a+b;
}
sum(10,20)
";
Assert.AreEqual(30L, script.Eval(s));
```

#### 使用@lang指定JavaScript语言
```
var s = @"
// 默认csharp语言
int mult(int a, int b) => a*b;
// 嵌入js语言
@lang js
function sum(a,b) {
	return a+b;
}
@end
int m = 10;
int n = 20;
mult(m, n) + sum(m, n);
";
var script = new Script();
Assert.AreEqual(230, script.Eval(s));
```

#### 字符串插值
```
var s = @"
name='tom'; 
`hello {name}, 5+8={5+8}`
";
var script = new Script();
script.Context.Langs = new [] { "js" };
Assert.AreEqual("hello tom, 5+8=13", script.Eval(s));
```

#### 正则表达式
```
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval<List<object>>("'hello world World'.match(/world/gi)");
Assert.AreEqual(2, result.Count);
Assert.AreEqual("world", result[0]);
Assert.AreEqual("World", result[1]);
```

#### 字符串函数
```
String.fromCharCode(65,66,67); // 'ABC'
'hello'.startsWith('he'); // true
'hello'.endsWith('a'); // false
'hello'.includes('e'); // true
'hello'.indexOf('el'); // 1
'hello'.indexOf('el', 2); // -1
'hello'.lastIndexOf('l'); // 3
'hello'.search('el'); // 1
'hello'.search(/el/gi); // 1
'hello'.substr(-2); // 'lo'
```
