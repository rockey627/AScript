using AScript.Lang.Python3;
using BenchmarkDotNet.Attributes;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;

namespace AScript.Test.Consoles.Benchmarks
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class PythonTest01
	{
		private static readonly string s = @"
def exec(a) :
    m=0
    s=''
    if a>0 and a<10 : 
        m=1
        s='大于0且小于10'
    elif a>=10 and a<20 :
        m=2
        s='大于等于10且小于20'
    elif a>=20 and a<30 :
        m=3
        s='大于等于20且小于30'
    else :
        m=4
        s='大于等于30'
    return (s)

exec(26)
";
		private static readonly string r = "大于等于20且小于30";

		private Microsoft.Scripting.Hosting.ScriptEngine engine = Python.CreateEngine();

		static PythonTest01()
        {
            Script.Langs.Set("python3", Python3Lang.Instance, true);
        }

		[Benchmark]
		public void AScript1()
		{
			var script = new AScript.Script();
			var result = script.Eval<string>(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache()
		{
			var script = new AScript.Script();
			var result = script.Eval<string>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript2_NoCache2()
		{
			var script = new AScript.Script();
			script.Options.RewriteFunctions = false;
			var result = script.Eval<string>(s, ECompileMode.All);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache()
		{
			var script = new AScript.Script();
			var result = script.Eval<string>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void AScript3_UseCache2()
		{
			var script = new AScript.Script();
			script.Options.RewriteFunctions = false;
			var result = script.Eval<string>(s, -1);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void IronPython1()
		{
			ScriptScope scope = engine.CreateScope();
			var result = engine.Execute(s, scope);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void IronPython2()
		{
			var result = engine.Execute(s);
			if (result != r)
			{
				throw new Exception("result error");
			}
		}
	}
}
