# AScript.Lang.JavaScript.axios

## 介绍
AScript.Lang.JavaScript脚本语言http网络模块。

## 安装
```
install-package AScript.Lang.JavaScript.axios
```

## 使用说明
* 命名空间：`using AScript.Lang.JavaScript.axios;`

#### 添加axios模块
```
JavaScriptLang.Instance.AddModule("axios", new JavaScriptAxiosModule());
```

#### get
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

#### post
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

#### 其他方法
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