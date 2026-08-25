using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptImportTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["js"] = JavaScriptLang.Instance;
			JavaScriptLang.Instance.Modules.FileOptions.CompileMode = ECompileMode.All;
			JavaScriptLang.Instance.Modules.AddDir("./JavaScript/modules");
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("js");
		}

		// import m from 'modulename'
		[TestMethod]
		public void Test01_ImportDefault()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			var code = @"
import m, { getTotal } from 'mymodule';
m.sum(1, 2) + m.fib(3);
";
			Assert.AreEqual(9L, script.Eval(code));
			Assert.AreEqual(2L, script.Eval("getTotal()"));
			Assert.AreEqual(30L, script.Eval("m.sum(10, 20)"));
			Assert.AreEqual(3L, script.Eval("getTotal()"));
		}

		[TestMethod]
		public void Test01_ImportDefault_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			var code = @"
import m, { getTotal } from 'mymodule';
m.sum(1, 2) + m.fib(3);
";
			Assert.AreEqual(9L, script.Eval(code));
			Assert.AreEqual(2L, script.Eval("getTotal()"));
			Assert.AreEqual(30L, script.Eval("m.sum(10, 20)"));
			Assert.AreEqual(3L, script.Eval("getTotal()"));
		}

		[TestMethod]
		public void Test02_Import_Decontruct()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };

			var code = @"
import { sum, fib as fib2 } from 'mymodule';
sum(1, 2) + fib2(3);
";
			Assert.AreEqual(9L, script.Eval(code));
			Assert.AreEqual(21L, script.Eval("sum(13, 8)"));
		}

		[TestMethod]
		public void Test02_Import_Decontruct_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			var code = @"
import { sum, fib as fib2 } from 'mymodule';
sum(1, 2) + fib2(3);
";
			Assert.AreEqual(9L, script.Eval(code));
			Assert.AreEqual(21L, script.Eval("sum(13, 8)"));
		}

		[TestMethod]
		public void Test03_getTotal()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			var code = @"
import { sum, getTotal } from 'mymodule';
sum(1, 2);
";
			Assert.AreEqual(3L, script.Eval(code));
			Assert.AreEqual(2L, script.Eval("getTotal()"));
			Assert.AreEqual(21L, script.Eval("sum(13, 8)"));
			Assert.AreEqual(3L, script.Eval("getTotal()"));
			script.Eval("import { getTotal as getTotal2 } from 'mymodule'");
			Assert.AreEqual(3L, script.Eval("getTotal2()"));
		}

		[TestMethod]
		public void Test03_getTotal_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			var code = @"
import { sum, getTotal } from 'mymodule';
sum(1, 2);
";
			Assert.AreEqual(3L, script.Eval(code));
			Assert.AreEqual(2L, script.Eval("getTotal()"));
			Assert.AreEqual(21L, script.Eval("sum(13, 8)"));
			Assert.AreEqual(3L, script.Eval("getTotal()"));
			script.Eval("import { getTotal as getTotal2 } from 'mymodule'");
			Assert.AreEqual(3L, script.Eval("getTotal2()"));
		}
	}
}
