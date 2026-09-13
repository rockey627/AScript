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