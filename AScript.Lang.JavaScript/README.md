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
[1, 2].concat([3, 4]); // [1, 2, 3, 4]
[1, 2, 3].join(','); // '1,2,3'
[1, 2, 3].indexOf(2); // 1
[1, 2, 3].includes(2); // true
[1, 2, 3].reverse(); // [3, 2, 1]
[1, 2, 3, 4, 5].filter(x => x % 2 == 0); // [2, 4]
[1, 2, 3].map(x => x * 2); // [2, 4, 6]
[1, 2, 3].reduce((acc, x) => acc + x); // 6
[1, 2, 3].reduce((acc, x) => acc + x, 10); // 16
[2, 4, 6].every(x => x % 2 == 0); // true
[1, 3, 5].some(x => x % 2 == 0); // false
[1, 2, 3].find(x => x > 1); // 2
[1, 2, 3].findIndex(x => x > 1); // 1
[1, 2, 3].fill(0); // [0, 0, 0]
[1, 2, 3].forEach(x => { }); // 遍历数组
[1, 2, 3].pop(); // 3
[1, 2, 3].push(5, 6, 7); // 数组结尾处理添加元素
[1, 2, 3].shift(); // 移除并返回首部元素
[1, 2, 3].unshift(5, 9, 10); // 从首部插入3个元素
[1, 2, 3, 4, 5].slice(2, 4); // 截取列表[2,4)：[3, 4]
[1, 2, 3, 4, 5].splice(1, 2); // 截取并返回列表：[2, 3]
```

#### 时间
```
new Date(); // 当前时间
Date.now(); // 当前时间戳
new Date(2026, 6, 4); // 2026-07-04
var time = new Date('2026-7-4');
time.getYear(); // 获取年份-1900：126
time.getMonth(); // 获取月份-1：6
time.getDate(); // 获取月份中的天(1~31)：4
time.getDay(); // 获取星期：6
time.getHours();
time.getMinutes();
time.getSeconds();
time.getMilliseconds();
time.getTime(); // 获取时间戳
time.toString('yyyy-MM-dd'); // '2026-07-04'
time.setFullYear(2025);
time.setFullYear(2025, 6);
time.setFullYear(2025, 6, 4);
time.setHours(21);
time.setMinutes(36);
time.setSeconds(15);
time.setMilliseconds(238);
```

#### 字典Map
```
var m = new Map([['a', 1], ['b', 2], ['c', 3]]);
m.set('d', 4);
m.size; // 4
m.get('a'); // 1
m.has('a'); // true
m.delete('c');
m.clear();
m.keys();
m.values();
m.entries();
m.forEach(function(value, key) { });
```

#### 集合Set
```
var set = new Set([1, 2, 3]);
set.add(4);
set.has(2);
set.delete(3);
set.clear();
set.size;
set.forEach(x => { });
```

#### 异步Promise
* then/catch/finally
```
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval("await new Promise((resolve, reject) => resolve(1)).then(x => x + 1).then(x => x + 2)");
Assert.AreEqual(4L, result);
```
* Promise.all
```
var s = @"
var arr = await Promise.all([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)]);
arr[0] + arr[1] + arr[2]
";
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval(s);
Assert.AreEqual(6L, result);
```
* Promise.any
```
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval<long>("await Promise.any([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)])");
Assert.AreEqual(1L, result);
```

#### axios
```

```