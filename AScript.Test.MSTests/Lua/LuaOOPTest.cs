using AScript.Lang.Lua;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.Lua
{
	[TestClass]
	public class LuaOOPTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["lua"] = LuaLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("lua");
		}

		[TestMethod]
		public void Test00_01()
		{
			string code = @"
-- 定义 Person 类
Person = {name = '', age = 0}

-- Person 的构造函数
function Person:new(name, age)
    local obj = {}  -- 创建一个新的表作为对象
    setmetatable(obj, self)  -- 设置元表，使其成为 Person 的实例
    self.__index = self  -- 设置索引元方法，指向 Person
    obj.name = name
    obj.age = age
    return obj
end

-- 添加方法：打印个人信息
function Person:introduce()
	local s = 'My name is ' .. self.name .. ' and I am ' .. self.age .. ' years old.'
    print(s)
	return s
end

-- 创建一个 Person 对象
local person1 = Person:new('Alice', 30)

-- 调用对象的方法
person1:introduce()  -- 输出 'My name is Alice and I am 30 years old.'
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("My name is Alice and I am 30 years old.", script.Eval(code));
		}

		[TestMethod]
		public void Test00_01_CompileAll()
		{
			string code = @"
-- 定义 Person 类
Person = {name = '', age = 0}

-- Person 的构造函数
function Person:new(name, age)
    local obj = {}  -- 创建一个新的表作为对象
    setmetatable(obj, self)  -- 设置元表，使其成为 Person 的实例
    self.__index = self  -- 设置索引元方法，指向 Person
    obj.name = name
    obj.age = age
    return obj
end

-- 添加方法：打印个人信息
function Person:introduce()
	local s = 'My name is ' .. self.name .. ' and I am ' .. self.age .. ' years old.'
    print(s)
	return s
end

-- 创建一个 Person 对象
local person1 = Person:new('Alice', 30)

-- 调用对象的方法
person1:introduce()  -- 输出 'My name is Alice and I am 30 years old.'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("My name is Alice and I am 30 years old.", script.Eval(code));
		}

		[TestMethod]
		public void Test00_02()
		{
			string code = @"
-- 定义矩形类
Rectangle = {area = 0, length = 0, breadth = 0}

-- 创建矩形对象的构造函数
function Rectangle:new(o, length, breadth)
  o = o or {}  -- 如果未传入对象，创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Rectangle 的方法
  self.__index = self  -- 确保在访问时能找到方法和属性
  o.length = length or 0  -- 设置长度，默认为 0
  o.breadth = breadth or 0  -- 设置宽度，默认为 0
  o.area = o.length * o.breadth  -- 计算面积
  return o
end

-- 打印矩形的面积
function Rectangle:printArea()
  print('矩形面积为 ', self.area)
end

r = Rectangle:new(nil,10,20)
r:printArea()
r.area
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(200L, script.Eval(code));
		}

		[TestMethod]
		public void Test00_02_CompileAll()
		{
			string code = @"
-- 定义矩形类
Rectangle = {area = 0, length = 0, breadth = 0}

-- 创建矩形对象的构造函数
function Rectangle:new(o, length, breadth)
  o = o or {}  -- 如果未传入对象，创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Rectangle 的方法
  self.__index = self  -- 确保在访问时能找到方法和属性
  o.length = length or 0  -- 设置长度，默认为 0
  o.breadth = breadth or 0  -- 设置宽度，默认为 0
  o.area = o.length * o.breadth  -- 计算面积
  return o
end

-- 打印矩形的面积
function Rectangle:printArea()
  print('矩形面积为 ', self.area)
end

r = Rectangle:new(nil,10,20)
r:printArea()
r.area
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(200L, script.Eval(code));
		}

		[TestMethod]
		public void Test00_03()
		{
			string code = @"
-- 定义矩形类
Rectangle = {area = 0, length = 0, breadth = 0}

-- 创建矩形对象的构造函数
function Rectangle:new(o, length, breadth)
  o = o or {}  -- 如果未传入对象，创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Rectangle 的方法
  self.__index = self  -- 确保在访问时能找到方法和属性
  o.length = length or 0  -- 设置长度，默认为 0
  o.breadth = breadth or 0  -- 设置宽度，默认为 0
  o.area = o.length * o.breadth  -- 计算面积
  return o
end

-- 打印矩形的面积
function Rectangle:printArea()
  print('矩形面积为 ', self.area)
end

-- 定义正方形类，继承自矩形类
Square = Rectangle:new()  -- Square 继承 Rectangle 类

-- 重写构造函数（正方形的边长相等）
function Square:new(o, side)
  o = o or {}  -- 如果未传入对象，创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Rectangle 的方法
  self.__index = self  -- 确保在访问时能找到方法和属性
  o.length = side or 0  -- 设置边长
  o.breadth = side or 0  -- 正方形的宽度和长度相等
  o.area = o.length * o.breadth  -- 计算面积
  return o
end

-- 运行实例：
local rect = Rectangle:new(nil, 5, 10)  -- 创建一个长为 5，宽为 10 的矩形
rect:printArea()  -- 输出 '矩形面积为 50'

local square = Square:new(nil, 4)  -- 创建一个边长为 4 的正方形
square:printArea()  -- 输出 '矩形面积为 16'

rect.area ..','..square.area
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("50,16", script.Eval(code));
		}

		[TestMethod]
		public void Test00_03_CompileAll()
		{
			string code = @"
-- 定义矩形类
Rectangle = {area = 0, length = 0, breadth = 0}

-- 创建矩形对象的构造函数
function Rectangle:new(o, length, breadth)
  o = o or {}  -- 如果未传入对象，创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Rectangle 的方法
  self.__index = self  -- 确保在访问时能找到方法和属性
  o.length = length or 0  -- 设置长度，默认为 0
  o.breadth = breadth or 0  -- 设置宽度，默认为 0
  o.area = o.length * o.breadth  -- 计算面积
  return o
end

-- 打印矩形的面积
function Rectangle:printArea()
  print('矩形面积为 ', self.area)
end

-- 定义正方形类，继承自矩形类
Square = Rectangle:new()  -- Square 继承 Rectangle 类

-- 重写构造函数（正方形的边长相等）
function Square:new(o, side)
  o = o or {}  -- 如果未传入对象，创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Rectangle 的方法
  self.__index = self  -- 确保在访问时能找到方法和属性
  o.length = side or 0  -- 设置边长
  o.breadth = side or 0  -- 正方形的宽度和长度相等
  o.area = o.length * o.breadth  -- 计算面积
  return o
end

-- 运行实例：
local rect = Rectangle:new(nil, 5, 10)  -- 创建一个长为 5，宽为 10 的矩形
rect:printArea()  -- 输出 '矩形面积为 50'

local square = Square:new(nil, 4)  -- 创建一个边长为 4 的正方形
square:printArea()  -- 输出 '矩形面积为 16'

rect.area ..','..square.area
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("50,16", script.Eval(code));
		}

		[TestMethod]
		public void Test00_04()
		{
			string code = @"
-- 定义动物类（Animal）
Animal = {name = 'Unknown'}

-- Animal 类的构造函数
function Animal:new(o, name)
  o = o or {}  -- 如果没有传入对象，则创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Animal 的方法
  self.__index = self  -- 让对象可以访问 Animal 的方法
  o.name = name or 'Unknown'  -- 设置名称，默认为 'Unknown'
  return o
end

-- Animal 类的方法：叫声
function Animal:speak()
  local ss = self.name .. ' makes a sound.'
  print(ss)
  return ss
end


-- 定义狗类（Dog），继承自 Animal
Dog = Animal:new()  -- Dog 继承 Animal 类

-- 重写狗类的构造函数
function Dog:new(o, name, breed)
  o = o or {}  -- 如果没有传入对象，则创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Dog 和 Animal 的方法
  self.__index = self  -- 让对象可以访问 Dog 的方法
  o.name = name or 'Unknown'
  o.breed = breed or 'Unknown'
  return o
end

-- 重写狗类的叫声方法（重写 Animal 的 speak 方法）
function Dog:speak()
  local ss = self.name .. ' barks.'
  print(ss)
  return ss
end


-- 创建 Animal 对象
local animal = Animal:new(nil, 'Generic Animal')
local s1 = animal:speak()  -- 输出 'Generic Animal makes a sound.'

-- 创建 Dog 对象
local dog = Dog:new(nil, 'Buddy', 'Golden Retriever')
local s2 = dog:speak()  -- 输出 'Buddy barks.'
s1 .. ';' .. s2
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Generic Animal makes a sound.;Buddy barks.", script.Eval(code));
		}

		[TestMethod]
		public void Test00_04_CompileAll()
		{
			string code = @"
-- 定义动物类（Animal）
Animal = {name = 'Unknown'}

-- Animal 类的构造函数
function Animal:new(o, name)
  o = o or {}  -- 如果没有传入对象，则创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Animal 的方法
  self.__index = self  -- 让对象可以访问 Animal 的方法
  o.name = name or 'Unknown'  -- 设置名称，默认为 'Unknown'
  return o
end

-- Animal 类的方法：叫声
function Animal:speak()
  local ss = self.name .. ' makes a sound.'
  print(ss)
  return ss
end


-- 定义狗类（Dog），继承自 Animal
Dog = Animal:new()  -- Dog 继承 Animal 类

-- 重写狗类的构造函数
function Dog:new(o, name, breed)
  o = o or {}  -- 如果没有传入对象，则创建一个新的空表
  setmetatable(o, self)  -- 设置元表，使其继承 Dog 和 Animal 的方法
  self.__index = self  -- 让对象可以访问 Dog 的方法
  o.name = name or 'Unknown'
  o.breed = breed or 'Unknown'
  return o
end

-- 重写狗类的叫声方法（重写 Animal 的 speak 方法）
function Dog:speak()
  local ss = self.name .. ' barks.'
  print(ss)
  return ss
end


-- 创建 Animal 对象
local animal = Animal:new(nil, 'Generic Animal')
local s1 = animal:speak()  -- 输出 'Generic Animal makes a sound.'

-- 创建 Dog 对象
local dog = Dog:new(nil, 'Buddy', 'Golden Retriever')
local s2 = dog:speak()  -- 输出 'Buddy barks.'
s1 .. ';' .. s2
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Generic Animal makes a sound.;Buddy barks.", script.Eval(code));
		}

		#region Basic OOP - Table Method Definition

		[TestMethod]
		public void Test01_BasicMethod_DefineAndCall()
		{
			// function obj:method(args) 语法定义方法
			string code = @"
local obj = {}
function obj:add(value)
	return self.x + value
end
obj.x = 10
obj:add(5)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test01_BasicMethod_DefineAndCall_CompileAll()
		{
			string code = @"
local obj = {}
function obj:add(value)
	return self.x + value
end
obj.x = 10
obj:add(5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_MultipleMethods()
		{
			string code = @"
local obj = {}
function obj:setX(x) self.x = x end
function obj:setY(y) self.y = y end
function obj:sum() return self.x + self.y end
obj:setX(10)
obj:setY(20)
obj:sum()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void Test02_MultipleMethods_CompileAll()
		{
			string code = @"
local obj = {}
function obj:setX(x) self.x = x end
function obj:setY(y) self.y = y end
function obj:sum() return self.x + self.y end
obj:setX(10)
obj:setY(20)
obj:sum()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(30L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_ConstructorPattern()
		{
			// 模拟构造函数：创建新对象并初始化
			string code = @"
local function newPoint(x, y)
	local obj = {}
	function obj:setX(v) self.x = v end
	function obj:setY(v) self.y = v end
	function obj:getX() return self.x end
	function obj:getY() return self.y end
	function obj:sum() return self.x + self.y end
	obj:setX(x)
	obj:setY(y)
	return obj
end
local p = newPoint(3, 4)
p:sum()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(7L, script.Eval(code));
		}

		[TestMethod]
		public void Test03_ConstructorPattern_CompileAll()
		{
			string code = @"
local function newPoint(x, y)
	local obj = {}
	function obj:setX(v) self.x = v end
	function obj:setY(v) self.y = v end
	function obj:getX() return self.x end
	function obj:getY() return self.y end
	function obj:sum() return self.x + self.y end
	obj:setX(x)
	obj:setY(y)
	return obj
end
local p = newPoint(3, 4)
p:sum()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(7L, script.Eval(code));
		}

		[TestMethod]
		public void Test04_ChainedMethodCalls()
		{
			// 链式调用
			string code = @"
local obj = {value = 0}
function obj:add(v) self.value = self.value + v return self end
function obj:mult(v) self.value = self.value * v return self end
function obj:getValue() return self.value end
obj:add(5):mult(3):getValue()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test04_ChainedMethodCalls_CompileAll()
		{
			string code = @"
local obj = {value = 0}
function obj:add(v) 
	self.value = self.value + v 
	print('add('..v..'):'..self.value)
	return self 
end
function obj:mult(v) 
	self.value = self.value * v 
	print('mult('..v..'):'..self.value)
	return self 
end
function obj:getValue() return self.value end
obj:add(5):mult(3):getValue()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(15L, script.Eval(code));
		}

		[TestMethod]
		public void Test05_NoArgsMethod()
		{
			string code = @"
local obj = {count = 0}
function obj:increment() self.count = self.count + 1 end
function obj:getCount() return self.count end
obj:increment()
obj:increment()
obj:increment()
obj:getCount()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test05_NoArgsMethod_CompileAll()
		{
			string code = @"
local obj = {count = 0}
function obj:increment() self.count = self.count + 1 end
function obj:getCount() return self.count end
obj:increment()
obj:increment()
obj:increment()
obj:getCount()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_MethodWithMultipleArgs()
		{
			string code = @"
local obj = {}
function obj:add(a, b, c) return a + b + c + self.x end
obj.x = 10
obj:add(1, 2, 3)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(16L, script.Eval(code));
		}

		[TestMethod]
		public void Test06_MethodWithMultipleArgs_CompileAll()
		{
			string code = @"
local obj = {}
function obj:add(a, b, c) return a + b + c + self.x end
obj.x = 10
obj:add(1, 2, 3)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(16L, script.Eval(code));
		}

		#endregion

		#region Direct Function Assignment (without self)

		[TestMethod]
		public void Test10_DirectFunctionAssign()
		{
			// 直接赋值函数，不使用 self
			string code = @"
local t = {}
t.sum = function(a, b)
	return a + b
end
t.sum(3, 5)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(8L, script.Eval(code));
		}

		[TestMethod]
		public void Test10_DirectFunctionAssign_CompileAll()
		{
			string code = @"
local t = {}
t.sum = function(a, b)
	return a + b
end
t.sum(3, 5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(8L, script.Eval(code));
		}

//		[TestMethod]
//		public void Test11_ColonSyntaxAssign()
//		{
//			// 使用 : 语法赋值
//			string code = @"
//local t = {}
//:t.sum = function(a, b)
//	return a + b
//end
//t.sum(3, 5)
//";
//			var script = new Script();
//			script.Context.Langs = new[] { "lua" };
//			Assert.AreEqual(8L, script.Eval(code));
//		}

//		[TestMethod]
//		public void Test11_ColonSyntaxAssign_CompileAll()
//		{
//			string code = @"
//local t = {}
//:t.sum = function(a, b)
//	return a + b
//end
//t.sum(3, 5)
//";
//			var script = new Script();
//			script.Options.CompileMode = ECompileMode.All;
//			script.Context.Langs = new[] { "lua" };
//			Assert.AreEqual(8L, script.Eval(code));
//		}

		#endregion

		#region OOP with Table Literals

		[TestMethod]
		public void Test20_TableLiteralWithMethods()
		{
			string code = @"
local obj = {
	x = 0,
	add = function(self, v)
		self.x = self.x + v
	end,
	get = function(self)
		return self.x
	end
}
obj:add(5)
obj:get()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test20_TableLiteralWithMethods_CompileAll()
		{
			string code = @"
local obj = {
	x = 0,
	--[[add = function(self, v)
		self.x = self.x + v
	end,]]
	get = function(self)
		return self.x
	end
}
function obj:add(v)
	self.x = self.x + v
end
obj:add(5)
obj:get()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(5L, script.Eval(code));
		}

		[TestMethod]
		public void Test21_MixedTable()
		{
			// table 中混合数据和方法
			string code = @"
local person = {
	name = 'default',
	age = 0,
	setName = function(self, n)
		self.name = n
	end,
	setAge = function(self, a)
		self.age = a
	end,
	getInfo = function(self)
		return self.name .. ',' .. self.age
	end
}
person:setName('Tom')
person:setAge(20)
person:getInfo()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Tom,20", script.Eval(code));
		}

		[TestMethod]
		public void Test21_MixedTable_CompileAll()
		{
			string code = @"
local person = {
	name = 'default',
	age = 0,
	--[[setName = function(self, n)
		self.name = n
	end,
	setAge = function(self, a)
		self.age = a
	end,]]
	getInfo = function(self)
		return self.name .. ',' .. self.age
	end
}
function person:setName(n)
	self.name = n
end
function person:setAge(a)
	self.age = a
end
person:setName('Tom')
person:setAge(20)
person:getInfo()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Tom,20", script.Eval(code));
		}

		#endregion

		#region Return Table from Function (Class Pattern)

		[TestMethod]
		public void Test30_ClassPattern_Basic()
		{
			string code = @"
local function Point(x, y)
	return {
		x = x,
		y = y,
		__add = function(self, other)
			return Point(self.x + other.x, self.y + other.y)
		end,
		getX = function(self) return self.x end,
		getY = function(self) return self.y end
	}
end
local p1 = Point(1, 2)
local p2 = Point(3, 4)
local p3 = p1:__add(p2)
p3:getX()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(4L, script.Eval(code));
		}

		[TestMethod]
		public void Test30_ClassPattern_Basic_CompileAll()
		{
			string code = @"
local function Point(x, y)
	return {
		x = x,
		y = y,
		__add = function(self, other)
			return Point(self.x + other.x, self.y + other.y)
		end,
		getX = function(self) return self.x end,
		getY = function(self) return self.y end
	}
end
local p1 = Point(1, 2)
local p2 = Point(3, 4)
local p3 = p1:__add(p2)
p3:getX()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(4L, script.Eval(code));
		}

		[TestMethod]
		public void Test31_ClassPattern_Inherit()
		{
			// 简单的继承模式
			string code = @"
local function Animal(name)
	return {
		name = name,
		speak = function(self)
			return self.name .. ' makes a sound'
		end,
		getName = function(self)
			return self.name
		end
	}
end

local function Dog(name, breed)
	local obj = Animal(name)
	obj.breed = breed
	obj.speak = function(self)
		return self.name .. ' barks'
	end
	obj.getBreed = function(self)
		return self.breed
	end
	return obj
end

local d = Dog('Buddy', 'Labrador')
d:getName() .. ',' .. d:getBreed() .. ',' .. d:speak()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Buddy,Labrador,Buddy barks", script.Eval(code));
		}

		[TestMethod]
		public void Test31_ClassPattern_Inherit_CompileAll()
		{
			string code = @"
local function Animal(name)
	return {
		name = name,
		speak = function(self)
			return self.name .. ' makes a sound'
		end,
		getName = function(self)
			return self.name
		end
	}
end

local function Dog(name, breed)
	local obj = Animal(name)
	obj.breed = breed
	obj.speak = function(self)
		return self.name .. ' barks'
	end
	obj.getBreed = function(self)
		return self.breed
	end
	return obj
end

local d = Dog('Buddy', 'Labrador')
d:getName() .. ',' .. d:getBreed() .. ',' .. d:speak()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Buddy,Labrador,Buddy barks", script.Eval(code));
		}

		#endregion

		#region Method as First Class Citizen

		[TestMethod]
		public void Test40_StoreMethodReference()
		{
			string code = @"
local obj = {x = 10}
function obj:getX()
	return self.x
end
local f = obj.getX
f(obj)
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

//		[TestMethod]
//		public void Test40_StoreMethodReference_CompileAll()
//		{
//			string code = @"
//local obj = {x = 10}
//function obj:getX()
//	return self.x
//end
//local f = obj.getX
//f(obj)
//";
//			var script = new Script();
//			script.Options.CompileMode = ECompileMode.All;
//			script.Context.Langs = new[] { "lua" };
//			Assert.AreEqual(10L, script.Eval(code));
//		}

//		[TestMethod]
//		public void Test41_PassMethodAsArgument()
//		{
//			string code = @"
//local obj = {value = 0}
//function obj:increment()
//	self.value = self.value + 1
//end
//function obj:getValue()
//	return self.value
//end

//local function twice(f)
//	f()
//	f()
//end

//obj:increment()
//twice(obj.increment)
//obj:getValue()
//";
//			var script = new Script();
//			script.Context.Langs = new[] { "lua" };
//			Assert.AreEqual(3L, script.Eval(code));
//		}

//		[TestMethod]
//		public void Test41_PassMethodAsArgument_CompileAll()
//		{
//			string code = @"
//local obj = {value = 0}
//function obj:increment()
//	self.value = self.value + 1
//end
//function obj:getValue()
//	return self.value
//end

//local function twice(f)
//	f()
//	f()
//end

//obj:increment()
//twice(obj.increment)
//obj:getValue()
//";
//			var script = new Script();
//			script.Options.CompileMode = ECompileMode.All;
//			script.Context.Langs = new[] { "lua" };
//			Assert.AreEqual(3L, script.Eval(code));
//		}

		#endregion

		#region Edge Cases

		[TestMethod]
		public void Test50_EmptyObject()
		{
			string code = @"
local obj = {}
function obj:noop() end
function obj:getValue()
	return self.value
end
obj.value = 100
obj:getValue()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test50_EmptyObject_CompileAll()
		{
			string code = @"
local obj = {}
function obj:noop() end
function obj:getValue()
	return self.value
end
obj.value = 100
obj:getValue()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(100L, script.Eval(code));
		}

		[TestMethod]
		public void Test51_MethodReturnsObject()
		{
			string code = @"
local obj = {}
function obj:clone()
	return {
		x = self.x,
		y = self.y,
		getX = function(self) return self.x end
	}
end
obj.x = 5
local c = obj:clone()
c.x = 10
obj.x .. ',' .. c.x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("5,10", script.Eval(code));
		}

		[TestMethod]
		public void Test51_MethodReturnsObject_CompileAll()
		{
			string code = @"
local obj = {}
function obj:clone()
	local tmp = {
		x = self.x,
		y = self.y,
		getX = function(self) return self.x end
	}
	function tmp:setX(x)
		self.x = x
	end
	return tmp
end
obj.x = 5
local c = obj:clone()
--c.x = 10
c:setX(10)
obj.x .. ',' .. c.x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("5,10", script.Eval(code));
		}

		[TestMethod]
		public void Test52_NestedTableMethod()
		{
			string code = @"
local outer = {}
outer.inner = {value = 0}
function outer.inner:inc(v)
	self.value = self.value + v
end
function outer.inner:get()
	return self.value
end
outer.inner:inc(10)
outer.inner:get()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test52_NestedTableMethod_CompileAll()
		{
			string code = @"
local outer = {}
outer.inner = {value = 0}
function outer.inner:inc(v)
	self.value = self.value + v
end
function outer.inner:get()
	return self.value
end
outer.inner:inc(10)
outer.inner:get()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test53_MethodWithStringReturn()
		{
			string code = @"
local obj = {}
function obj:greet()
	return 'Hello, ' .. self.name
end
obj.name = 'World'
obj:greet()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Hello, World", script.Eval(code));
		}

		[TestMethod]
		public void Test53_MethodWithStringReturn_CompileAll()
		{
			string code = @"
local obj = {}
function obj:greet()
	return 'Hello, ' .. self.name
end
obj.name = 'World'
obj:greet()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual("Hello, World", script.Eval(code));
		}

		[TestMethod]
		public void Test54_MethodWithBooleanReturn()
		{
			string code = @"
local obj = {}
function obj:isPositive()
	return self.value > 0
end
obj.value = 5
obj:isPositive()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(true, script.Eval(code));
		}

		[TestMethod]
		public void Test54_MethodWithBooleanReturn_CompileAll()
		{
			string code = @"
local obj = {}
function obj:isPositive()
	return self.value > 0
end
obj.value = 5
obj:isPositive()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(true, script.Eval(code));
		}

		[TestMethod]
		public void Test55_MethodWithNilReturn()
		{
			string code = @"
local obj = {}
function obj:named()
	local x = 10
end
obj:named()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test55_MethodWithNilReturn_CompileAll()
		{
			string code = @"
local obj = {}
function obj:named()
	local x = 10
end
obj:named()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

		[TestMethod]
		public void Test56_ReassignMethod()
		{
			string code = @"
local obj = {x = 0}
function obj:inc()
	self.x = self.x + 1
end
function obj:dec()
	self.x = self.x - 1
end
obj:inc()
obj:inc()
obj:dec()
obj.x
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test56_ReassignMethod_CompileAll()
		{
			string code = @"
local obj = {x = 0}
function obj:inc()
	self.x = self.x + 1
end
function obj:dec()
	self.x = self.x - 1
end
obj:inc()
obj:inc()
obj:dec()
obj.x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(1L, script.Eval(code));
		}

		[TestMethod]
		public void Test57_SelfModify()
		{
			string code = @"
local obj = {items = {}}
function obj:add(item)
	table.insert(self.items, item)
end
function obj:count()
	return #self.items
end
obj:add(1)
obj:add(2)
obj:add(3)
obj:count()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		[TestMethod]
		public void Test57_SelfModify_CompileAll()
		{
			string code = @"
local obj = {items = {}}
function obj:add(item)
	table.insert(self.items, item)
end
function obj:count()
	return #self.items
end
obj:add(1)
obj:add(2)
obj:add(3)
obj:count()
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(3L, script.Eval(code));
		}

		#endregion
	}
}
