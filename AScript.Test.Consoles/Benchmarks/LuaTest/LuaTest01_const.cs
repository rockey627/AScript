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
	public class LuaTest01_const
	{
		private static readonly string s = "return 100 * (5 + 5) * (6-2)";
		private static readonly long r = 100 * (5 + 5) * (6 - 2);

		static LuaTest01_const()
		{
			Script.Langs.Set("lua", LuaLang.Instance, setDefault: true);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			var result = script.Eval<long>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void LuaCSharp1()
		{
			var lua = Lua.LuaState.Create();
			var result = lua.DoStringAsync(s).Result[0].Read<long>();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void MoonSharp1()
		{
			var script = new MoonSharp.Interpreter.Script();
			var result = script.DoString(s).ToObject<long>();
			if (result != r) throw new Exception("result error");
		}

		// 测试过程超级慢
		//private static MoonSharp.Interpreter.Script _MoonSharp1_Script;

		//[Benchmark]
		//public void MoonSharp1_Context()
		//{
		//	if (_MoonSharp1_Script == null)
		//	{
		//		_MoonSharp1_Script = new MoonSharp.Interpreter.Script();
		//	}
		//	var result = _MoonSharp1_Script.DoString(s).ToObject<long>();
		//	if (result != r) throw new Exception("result error");
		//}

		[Benchmark]
		public void NLua1()
		{
			var lua = new NLua.Lua();
			var result = (long)lua.DoString(s)[0];
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			var result = script.Eval<long>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			var result = script.Eval<long>(s, -1);
			if (result != r) throw new Exception("result error");
		}
	}
}
