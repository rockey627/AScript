using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptVarTest
	{
		[TestMethod]
		public void Test03_2()
		{
			string s = "5+n";
			var script = new AScript.Script();
			script.Options.CompileMode = ECompileMode.All;
			try
			{
				script.Eval(s);
			}
			catch (Exception ex)
			{
				Assert.AreEqual("variable n is not exists", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test03()
		{
			string s = "5+n";
			var script = new AScript.Script();
			try
			{
				script.Eval(s);
			}
			catch(Exception ex)
			{
				Assert.AreEqual("variable n is not exists", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test02()
		{
			string s = "int v=5;100 * (v + 5) * (6-2)";
			var script = new AScript.Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(100 * (5 + 5) * (6 - 2), script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			//Assert.AreEqual(0, script.Eval("int a"));
			Assert.IsNull(script.Eval("int a", out var type));
			Assert.AreEqual(typeof(int), type);
			Assert.AreEqual(8, script.Eval("int a;a+8"));
			Assert.AreEqual(11, script.Eval("int a=5;a+6"));
		}
	}
}
