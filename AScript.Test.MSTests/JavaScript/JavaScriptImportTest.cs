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
import m from 'mymodule';
m.sum(1, 2) + m.fib(3);
";
			Assert.AreEqual(9L, script.Eval(code));
		}

		[TestMethod]
		public void Test01_ImportDefault_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };

			var code = @"
import m from 'mymodule';
m.sum(1, 2) + m.fib(3);
";
			Assert.AreEqual(9L, script.Eval(code));
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
		}
	}
}
