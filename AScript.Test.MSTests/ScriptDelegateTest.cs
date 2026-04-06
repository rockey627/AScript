using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptDelegateTest
	{

		[TestMethod]
		public void Test05_2()
		{
			string s = @"
var func1 = _(int a)=>a+5;
var func2 = _(int a)=>a+1;
func1.Invoke(6) + func2.Invoke(7)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test05()
		{
			string s = @"
var func1 = _(int a)=>a+5;
var func2 = _(int a)=>a+1;
func1.Invoke(6) + func2.Invoke(7)
";
			var script = new Script();
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test04_2()
		{
			string s = @"
var func1 = int _(int a)=>{a+5}
var func2 = int _(int a)=>{a+1}
func1.Invoke(6) + func2.Invoke(7)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test04()
		{
			string s = @"
var func1 = int _(int a)=>{a+5}
var func2 = int _(int a)=>{a+1}
func1.Invoke(6) + func2.Invoke(7)
";
			var script = new Script();
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test03_2()
		{
			string s = @"
var func1 = int _(int a){a+5}
var func2 = int _(int a){a+1}
func1.Invoke(6) + func2.Invoke(7)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test03()
		{
			string s = @"
var func1 = int _(int a){a+5}
var func2 = int _(int a){a+1}
func1.Invoke(6) + func2.Invoke(7)
";
			var script = new Script();
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test02_2()
		{
			string s = @"
var func1 = int _(int a)=>a+5;
var func2 = int _(int a)=>a+1;
func1.Invoke(6) + func2.Invoke(7)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test02()
		{
			string s = @"
var func1 = int _(int a)=>a+5;
var func2 = int _(int a)=>a+1;
func1.Invoke(6) + func2.Invoke(7)
";
			var script = new Script();
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test01_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("func", (int a) => a + 5);
			Assert.AreEqual(11, script.Eval("func.Invoke(6)"));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			script.Context.SetVar("func", (int a) => a + 5);
			Assert.AreEqual(11, script.Eval("func.Invoke(6)"));
		}
	}
}
