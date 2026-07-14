# AScript.Lang.JavaScript.fs

## 介绍
AScript.Lang.JavaScript脚本语言文件模块。

## 安装
```
install-package AScript.Lang.JavaScript.js
```

## 使用说明
* 命名空间：`using AScript.Lang.JavaScript.js;`

注：服务端项目请谨慎添加文件模块，避免脚本中恶意删除、修改文件。

#### 添加fs模块
```
JavaScriptLang.Instance.AddModule("fs", new JavaScriptFileSystemModule());
```

#### 同步读文件
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

#### 异步读文件
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

#### 同步写文件
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

#### 异步写文件
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

#### 其他方法
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