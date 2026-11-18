# AScript.Lang.Go

## 介绍
支持go基础语法和数据类型。

## 安装
```
install-package AScript
install-package AScript.Lang.Go
```

## 使用说明
* 命名空间：using AScript.Lang.Go;

#### 注册Go语言
```
Script.Langs.Set("go", GoLang.Instance);
// 可全局设置为默认语言
// Script.Langs.Set("go", GoLang.Instance, setDefault: true);
```

#### switch
* 不支持`fallthrough`

#### 数组
数组是值类型,赋值或传参时会拷贝整个数组，可通过数组指针传递引用。
* 数组
```
var a = [5]int{}
var b = [...]int{1,2,3,4,5}
var c = b
c[0]=10 // b[0]还是1
var d = &b
d[0] = 10 // b[0]为10

// 数组指针参数
func setarr(x *[5]int) {
	(*x)[0] = 100
}
```
* 列表
```
var a = []int
append(a, 1)
```

#### 原始字符串
使用反引号包裹的字符串为原始字符串，内容不做转义处理，常用于书写正则表达式。
```
var re = `\d+\.\d+`
```

#### regexp
支持go的regexp包，用法与go基本一致。
```
import 'regexp'

// 编译（表达式非法时抛出异常）
re := regexp.MustCompile(`\d+`)

// 编译（返回正则和错误信息）
re, err := regexp.Compile(`\d+`)

// 是否匹配
re.MatchString('abc123')

// 查找
re.FindString('abc123')        // 123
re.FindStringIndex('abc123')    // [3, 6]
re.FindStringSubmatch('a=1')    // ["a=1", "a", "1"]
re.FindAllString('a1b22', -1)   // ["1", "22"]

// 替换
re.ReplaceAllString('a1b22', '#')        // a#b#
re.ReplaceAllString('a=1', '$2=$1')      // 1=a

// 分割
re.Split('a1b22', -1)           // ["a", "b", ""]

// 捕获组
re := regexp.MustCompile(`(?P<key>\w+)=(?P<value>\w+)`)
re.NumSubexp()                 // 2
re.SubexpNames()               // ["", "key", "value"]
```