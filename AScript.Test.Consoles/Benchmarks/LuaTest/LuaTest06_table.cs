using AScript.Lang.Lua;
using BenchmarkDotNet.Attributes;
using Lua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.Benchmarks.LuaTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class LuaTest06_table
	{
		private static readonly string s = @"
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
	return 'My name is ' .. self.name .. ' and I am ' .. self.age .. ' years old.'
end

-- 创建一个 Person 对象
local person1 = Person:new('Alice', 30)

-- 调用对象的方法
return person1:introduce()";
		private static readonly string r = "My name is Alice and I am 30 years old.";

		static LuaTest06_table()
		{
			Script.Langs.Set("lua", LuaLang.Instance, setDefault: true);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<string>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void LuaCSharp1()
		{
			var lua = Lua.LuaState.Create();
			// 需要手动添加setmetatable函数
			lua.Environment["setmetatable"] = new LuaFunction(async (context, cancellationToken) =>
			{
				var arg0 = context.GetArgument<Lua.LuaTable>(0);
				var arg1 = context.GetArgument<Lua.LuaTable>(1);
				arg0.Metatable = arg1;
				context.Return(arg0);
				return 1;
			});
			var result = lua.DoStringAsync(s).Result[0].Read<string>();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void MoonSharp1()
		{
			var script = new MoonSharp.Interpreter.Script();
			var result = script.DoString(s).ToObject<string>();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void NLua1()
		{
			var lua = new NLua.Lua();
			var result = (string)lua.DoString(s)[0];
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			var result = script.Eval<string>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			var result = script.Eval<string>(s, -1);
			if (result != r) throw new Exception("result error");
		}
	}
}
