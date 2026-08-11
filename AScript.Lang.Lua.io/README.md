# Lua io模块

## 安装模块
```
install-package AScript.Lang.Lua.io
```

## 注册io模块
```
LuaLang.Instance.Modules.Add("io", new AScript.Lang.Lua.io.LuaIOModule());
```

## 使用io
```
string code = $@"
require 'io'
local f = io.open(file, 'w')
f:write('hello')
f:close()
local f = io.open(file, 'r')
local content = f:read()
f:close()
content
";
var script = new Script();
script.Context.Langs = new[] { "lua" };
script.Context.SetVar("file", "./test.txt");
Assert.AreEqual("hello", script.Eval(code));
```