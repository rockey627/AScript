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
