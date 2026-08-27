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
	public class LuaTest03_var
	{
		private static readonly string s = @"return a * (b + 5) * (c-2)";
		private static readonly long r = 100 * (5 + 5) * (6 - 2);

		static LuaTest03_var()
		{
			Script.Langs.Set("lua", LuaLang.Instance, setDefault: true);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = script.Eval<long>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void LuaCSharp1()
		{
			var lua = Lua.LuaState.Create();
			lua.Environment["a"] = 100;
			lua.Environment["b"] = 5;
			lua.Environment["c"] = 6;
			var result = lua.DoStringAsync(s).Result[0].Read<long>();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void MoonSharp1()
		{
			var script = new MoonSharp.Interpreter.Script();
			script.Globals["a"] = 100;
			script.Globals["b"] = 5;
			script.Globals["c"] = 6;
			var result = script.DoString(s).ToObject<long>();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void NLua1()
		{
			var lua = new NLua.Lua();
			lua["a"] = 100;
			lua["b"] = 5;
			lua["c"] = 6;
			var result = (long)lua.DoString(s)[0];
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = script.Eval<long>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = script.Eval<long>(s, -1);
			if (result != r) throw new Exception("result error");
		}
	}
}
