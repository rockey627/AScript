using AScript.Exceptions;
using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			catch (ScriptException ex)
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

		[TestMethod]
		public void Test04_Const()
		{
			string s = "const PI = 3.14159; const RADIUS = 5; PI * RADIUS * RADIUS";
			var script = new Script();
			Assert.AreEqual(78.53975, (double)script.Eval(s), 0.0001);
		}

		[TestMethod]
		public void Test04_Const_CompileAll()
		{
			string s = "const PI = 3.14159; const RADIUS = 5; PI * RADIUS * RADIUS";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(78.53975, (double)script.Eval(s), 0.0001);
		}

		[TestMethod]
		public void Test05_Const()
		{
			string s = "const PI = 3.14159; const RADIUS = 5; PI=3; PI * RADIUS * RADIUS";
			var script = new Script();
			try
			{
				script.Eval(s);
			}
			catch (ScriptRuntimeException ex)
			{
				Assert.AreEqual("'PI' is readonly, can not modify", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test05_Const_CompileAll()
		{
			string s = "const PI = 3.14159; const RADIUS = 5; PI=3; PI * RADIUS * RADIUS";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			try
			{
				script.Eval(s);
			}
			catch (ScriptRuntimeException ex)
			{
				Assert.AreEqual("'PI' is readonly, can not modify", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test06_static_const()
		{
			string s = "static const a = 3; a * 5";
			var script = new Script();
			Assert.AreEqual(15, script.Eval(s));
		}

		[TestMethod]
		public void Test06_static_const_CompileAll()
		{
			string s = "static const a = 3; a * 5";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(15, script.Eval(s));
		}

		[TestMethod]
		public void Test07_static_const_CompileAll()
		{
			string s = "static const a = 3; a=4; a * 5";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			try
			{
				script.Eval(s);
			}
			catch (ScriptRuntimeException ex)
			{
				Assert.AreEqual("'a' is readonly, can not modify", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test08_static_const_CompileAll()
		{
			var script = new Script();
			script.Eval("const a = 3", ECompileMode.All);
			try
			{
				script.Eval("a=4");
			}
			catch (ScriptRuntimeException ex)
			{
				Assert.AreEqual("'a' is readonly, can not modify", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

	}
}
