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

#### 正则表达式
```
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval<List<object>>("'hello world World'.match(/world/gi)");
Assert.AreEqual(2, result.Count);
Assert.AreEqual("world", result[0]);
Assert.AreEqual("World", result[1]);
```

#### 解构
* 对象解构
```
var {name, age} = {name: 'Alice', age: 25};
var {a, b = 100} = {a: 10};
var {inner: {name}} = {inner: {name: 'Tom', age: 20}};
var {name: aliasName} = {name: 'Bob'};
```

* 数组解构
```
var [x, y, z] = [10, 20, 30];
var [x, , z] = [1, 2, 3];
var [a = 5, b = 10] = [3];
var [[inner], outer] = [[10], 20];
```

* 对象/数组混合解构
```
var [first, {name}] = [1, {name: 'John'}];
```

#### 字符串插值
```
var s = @"
name='tom'; 
`hello ${name}, 5+8=${5+8}`
";
var script = new Script();
script.Context.Langs = new [] { "js" };
Assert.AreEqual("hello tom, 5+8=13", script.Eval(s));
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

#### 时间Date
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

#### Promise
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

#### 数学函数
```
Math.abs(-5); // 5
Math.acos(0.5);
Math.acosh(2);
Math.asin(0.5);
Math.asinh(2);
Math.atan(1);
Math.atan2(1, 1);
Math.atanh(0.5);
Math.tan(0);
Math.tanh(0);
Math.sign(5);
Math.sin(0);
Math.sinh(0);
Math.cbrt(27);
Math.ceil(5.1);
Math.clz32(1);
Math.cos(0);
Math.cosh(0);
Math.exp(1);
Math.expm1(1);
Math.floor(5.9); // 5.0
Math.fround(5.5);
Math.hypot(3, 4);
Math.imul(2, 3);
Math.log(1);
Math.log10(100);
Math.log1p(1);
Math.log2(8);
Math.max(1, 10, 5); // 10.0
Math.min(1, 10, 5); // 1.0
Math.pow(2, 3); // 8.0
Math.random(); // [0,1)
Math.round(5.4); // 5.0
Math.sqrt(4); // 2.0
Math.pow(3, 2); // 9.0
Math.trunc(5.9); // 5.0
Math.E;
Math.PI;
Math.SQRT2;
Math.SQRT1_2;
Math.LN2;
Math.LN10;
Math.LOG2E;
Math.LOG10E;
```

#### setTimeout/clearTimeout
```
string code = @"
var result = 0;
function onTimeout(a, b) {
result = a + b;
}
var handle = setTimeout(onTimeout, 50, 10, 20);
handle;
";
var script = new Script();
script.Context.Langs = new[] { "js" };
script.Eval<object>(code);
Thread.Sleep(100);
Assert.AreEqual(30L, script.Eval("result"));
```

#### setInterval/clearInterval
```
string code = @"
var count = 0;
function handler() { count = 1; }
var timer = setInterval(handler, 40);
timer;
";
var script = new Script();
script.Context.Langs = new[] { "js" };
var timerObj = script.Eval<object>(code);
Thread.Sleep(70);
script.Eval("clearInterval(timer)");
Assert.AreEqual(1L, script.Eval("count"));
```

#### fs文件操作
注：服务端项目请谨慎添加文件模块，避免脚本中恶意删除、修改文件。

* 添加fs模块
```
// 添加nuget包
install-package AScript.Lang.JavaScript.fs

// 添加fs模块
JavaScriptLang.Instance.AddModule("fs", new JavaScriptFileSystemModule());
```
* 同步读文件
```
File.WriteAllText("test.txt", "hello world", System.Text.Encoding.UTF8);
string s = @"
var fs = require('fs');
fs.readFileSync('test.txt', 'utf-8')
";
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval(s);
Assert.AreEqual("hello world", result);
```
* 异步读文件
```
File.WriteAllText("test.txt", "async content", System.Text.Encoding.UTF8);
string s = @"
var fs = require('fs');
await fs.readFile('test.txt', 'utf-8')
";
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = await script.EvalAsync<string>(s);
Assert.AreEqual("async content", result);
```
* 同步写文件
```
string s = @"
var fs = require('fs');
fs.writeFileSync('test.txt', 'hello world', 'utf-8');
true
";
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval(s);
Assert.AreEqual(true, result);
Assert.AreEqual("hello world", File.ReadAllText("test.txt", System.Text.Encoding.UTF8));
```
* 异步写文件
```
string s = @"
var fs = require('fs');
await fs.writeFile('test.txt', 'async write', 'utf-8');
true
";
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = await script.EvalAsync<bool>(s);
Assert.AreEqual(true, result);
Assert.AreEqual("async write", File.ReadAllText("test.txt", System.Text.Encoding.UTF8));
```
* 其他方法
```
var fs = require('fs');
fs.appendFileSync('test.txt', 'append content', 'utf-8'); // 同步追加文件
fs.appendFile('test.txt', 'append context', 'utf-8'); // 异步追加文件
fs.copyFileSync('test.txt', 'test_copy.txt'); // 同步拷贝文件
fs.copyFile('test.txt', 'test_copy.txt'); // 异步拷贝文件
fs.unlinkSync('test.txt'); // 同步删除文件
fs.unlink('test.txt'); // 异步删除文件
var reader = fs.createReadStream('test.txt'); // 读文件流
var writer = fs.createWriteStream('test2.txt'); // 写文件流
reader.pipe(writer); // 读取文件流并写入目标文件流
// 读文件流事件：open/data/error/end/close
reader.on('data', function(chunk) {
    console.log(chunk);
});
reader.on('end', function() {
    console.log('end');
});
// 写文件流事件：open/error/finish/close
writer.on('open', ()=>console.log('open'));
writer.on('close', ()=>console.log('close'));
writer.write('hello');
writer.end();
```

#### axios
http网络模块。

* 添加axios模块
```
// 添加nuget包
install-package AScript.Lang.JavaScript.axios

// 添加axios模块
JavaScriptLang.Instance.AddModule("axios", new JavaScriptAxiosModule());
```
* get
```
string s = @"
var axios = require('axios');
await axios.get('http://test.com/api/user/list'); 
";
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval<dynamic>(s);
var data = result.data;
```
* post
```
string s = @"
var axios = require('axios');
await axios.post('http://test.com/api/user/update', {code:'123',name:'tom'}); 
";
var script = new Script();
script.Context.Langs = new[] { "js" };
var result = script.Eval<dynamic>(s);
var data = result.data;
```
* 其他方法
```
var axios = require('axios');
var instance = axios.create({baseURL:'http://test.com'});
instance.get('/api/user/list')
	.then(res=>{})
	.catch(err=>{});
instance.post('/api/user/update', {code:'123',name:'tom'})
	.then(res=>{})
	.catch(err=>{});
instance.put('/api/user', {});
instance.delete('/api/user', {});
// mock
var mockInstance = axios.createMock([{code:'123',name:'tom'}, {code:'124',name:'john'}]);
var resp = await mockInstance.get('http://test.com/api/user/list'); // resp.data: [{code:'123',name:'tom'}, {code:'124',name:'john'}]
```