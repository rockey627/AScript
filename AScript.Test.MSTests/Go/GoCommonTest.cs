using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoCommonTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["go"] = GoLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("go");
		}

		[TestMethod]
		public void Test01()
		{
			var s = @"
var a int = 10
var b int = 20
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
		}

		[TestMethod]
		public void Test01_CompileAll()
		{
			var s = @"
var a int = 10
var b int = 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
		}

		[TestMethod]
		public void Test02()
		{
			var s = @"
var a = 10
var b = 20
a+b
";
			var script = new Script();
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
		}

		[TestMethod]
		public void Test02_CompileAll()
		{
			var s = @"
var a = 10
var b = 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
		}

		[TestMethod]
		public void Test03_CompileAll()
		{
			var s = @"
var a, b = 10, 20
a+b
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			Assert.AreEqual(30, script.Eval(s));
		}
	}
}
