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
function obj:add(v) self.value = self.value + v return self end
function obj:mult(v) self.value = self.value * v return self end
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

		[TestMethod]
		public void Test40_StoreMethodReference_CompileAll()
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
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "lua" };
			Assert.AreEqual(10L, script.Eval(code));
		}

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
obj:value = 100
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
obj:value = 100
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
