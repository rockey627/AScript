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
	public class LuaTest04_call
	{
		private static readonly string s = @"return sum(5, 8)";
		private static readonly long r = 13;

		static LuaTest04_call()
		{
			Script.Langs.Set("lua", LuaLang.Instance, setDefault: true);
		}

		[Benchmark]
		public void AScript1()
		{
			var script = new Script();
			script.Context.AddFunc<long, long, long>("sum", (a, b) => a + b);
			var result = script.Eval<long>(s);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void LuaCSharp1()
		{
			var lua = Lua.LuaState.Create();
			lua.Environment["sum"] = new LuaFunction(async (context, cancellationToken) =>
			{
				var arg0 = context.GetArgument<long>(0);
				var arg1 = context.GetArgument<long>(1);
				context.Return(arg0 + arg1);
				return 1;
			});
			var result = lua.DoStringAsync(s).Result[0].Read<long>();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void MoonSharp1()
		{
			var script = new MoonSharp.Interpreter.Script();
			script.Globals["sum"] = new Func<long, long, long>((a, b) => a + b);
			var result = script.DoString(s).ToObject<long>();
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void NLua1()
		{
			var lua = new NLua.Lua();
			lua["sum"] = new Func<long, long, long>((a, b) => a + b);
			var result = (long)lua.DoString(s)[0];
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript2_Compile()
		{
			var script = new Script();
			script.Context.AddFunc<long, long, long>("sum", (a, b) => a + b);
			var result = script.Eval<long>(s, ECompileMode.All);
			if (result != r) throw new Exception("result error");
		}

		[Benchmark]
		public void AScript3_Cache()
		{
			var script = new Script();
			script.Context.AddFunc<long, long, long>("sum", (a, b) => a + b);
			var result = script.Eval<long>(s, -1);
			if (result != r) throw new Exception("result error");
		}
	}
}
