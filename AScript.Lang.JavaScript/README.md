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
'hello'.substr(-2,1); // 'l'
'hello'.substring(2); // 'llo'
'hello'.substring(2,4); // 'll'
'hello'.slice(2); // 'llo'
'hello'.slice(2,4); // 'll'
'hello'.slice(-3); // 'llo'
'hello'.slice(-3,-1); // 'll'
'Hello'.toLowerCase(); // 'hello'
'Hello'.toUpperCase(); // 'HELLO'
' hello '.trim(); // 'hello'
' hello '.trimStart(); // 'hello '
' hello '.trimEnd(); // ' hello'
'hello'.padStart(7); // '  hello'
'hello'.padStart(7, 'x'); // 'xxhello'
'hello'.padEnd(7); // 'hello  '
'hello'.padEnd(7, 'x'); // 'helloxx'
'hello'.charAt(1); // 'e'
'hello'.charCodeAt(1); // 101L
'hello tom, I am Tony'.match('to'); // ['to']
'hello tom, I am Tony'.match(/to/); // ['to']
'hello tom, I am Tony'.match(/to/gi); // ['to', 'To']
'hello tom, I am Tony'.replace('to', 'x'); // 替换第1项：'hello xm, I am Tony'
'hello tom, I am Tony'.replaceAll('to', 'x'); // 替换所有项（区分大小写）：'hello xm, I am Tony'
'hello tom, I am Tony'.replace(/to/g, 'x'); // 替换匹配项：'hello xm, I am Tony'
'hello tom, I am Tony'.replace(/to/gi, 'x'); // 替换匹配项：'hello xm, I am xny'
'a,b,c'.split(','); // ['a','b','c']
'hello'.repeat(2); // 'hellohello'
'hello'.concat(' ', 'world'); // 'hello world'
```

#### 创建数组
```
var s = @"
var arr1 = [1,2,3];
var arr2 = new Array(1,2,3);
var arr3 = new Array(2);
";
var script = new Script();
script.Context.Langs = new[] { "js" };
script.Eval(s);
var arr1 = script.Eval<List<object>>("arr1");
Assert.AreEqual(3, arr1.Count);
Assert.AreEqual(1L, arr1[0]);
Assert.AreEqual(2L, arr1[1]);
Assert.AreEqual(3L, arr1[2]);
var arr3 = script.Eval<List<object>>("arr3");
Assert.AreEqual(2, arr3.Count);
Assert.IsNull(arr3[0]);
Assert.IsNull(arr3[1]);
```

#### 数组函数
```

```