using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptPointerTest
	{
		[TestMethod]
		public void Test01()
		{
			var s = @"
int a = 5;
var p = &a;
*p
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test01_CompileAll()
		{
			var s = @"
int a = 5;
var p = &a;
*p
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test02()
		{
			var s = @"
int a = 5;
var p = &a;
*p = 6;
a
";
			var script = new Script();
			Assert.AreEqual(6, script.Eval(s));
		}

		[TestMethod]
		public void Test02_CompileAll()
		{
			var s = @"
int a = 5;
var p = &a;
*p = 6;
a
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(6, script.Eval(s));
		}
	}
}
