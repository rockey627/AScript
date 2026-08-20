using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptStandaloneTest
	{
		[TestMethod]
		public void Test01()
		{
			string s = "a+=2;a+3";
			var script = new Script();
			script.Options.Standalone = true;
			script.Context.SetVar("a", 1);
			var d = script.Eval(s, -1);
			Assert.AreEqual(6, d);
			Assert.AreEqual(1, script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test02()
		{
			string s = "int a=1;a+2";
			var script = new Script();
			script.Options.Standalone = true;
			var result = script.Eval(s, ECompileMode.All);
			Assert.AreEqual(3, result);
			Assert.AreEqual(null, script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test03()
		{
			string s = "int sum(int a, int b)=>a+b; sum(4,6)";
			var script = new Script();
			script.Options.Standalone = true;
			var d = script.CompileGlobal(s, -1);
			Assert.AreEqual(10, d.DynamicInvoke());
			Assert.ThrowsException<Exceptions.ScriptRuntimeException>(() =>
			{
				script.Eval("sum(2,3)");
			});
		}

		[TestMethod]
		public void Test04()
		{
			string s = "a * (b + 5) * (c-2)";
			int r = 100 * (5 + 5) * (6 - 2);
			var script = new Script();
			script.Options.Standalone = true;
			script.Context.SetVar("a", 100);
			script.Context.SetVar("b", 5);
			script.Context.SetVar("c", 6);
			var result = script.Eval<int>(s, ECompileMode.All);
			Assert.AreEqual(r, result);
		}
	}
}
