using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptBoolTest
	{
		[TestMethod]
		public void Test05_AndAlso2()
		{
			var s = @"
var a = 'ok';
true && (a+='hello')=='hello'
";
			var script = new Script();
			Assert.IsFalse(script.Eval<bool>(s));
			Assert.AreEqual("okhello", script.Eval("a"));
		}

		[TestMethod]
		public void Test05_AndAlso2_CompileAll()
		{
			var s = @"
var a = 'ok';
true && (a+='hello')=='hello'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.IsFalse(script.Eval<bool>(s));
			Assert.AreEqual("okhello", script.Eval("a"));
		}

		[TestMethod]
		public void Test05_AndAlso()
		{
			var a = "ok";
			Assert.IsFalse(false && (a += "hello") == "hello");
			Assert.AreEqual("ok", a);

			var s = @"
var a = 'ok';
false && (a+='hello')=='hello'
";
			var script = new Script();
			Assert.IsFalse(script.Eval<bool>(s));
			Assert.AreEqual("ok", script.Eval("a"));
		}

		[TestMethod]
		public void Test05_AndAlso_CompileAll()
		{
			var s = @"
var a = 'ok';
false && (a+='hello')=='hello'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.IsFalse(script.Eval<bool>(s));
			Assert.AreEqual("ok", script.Eval("a"));
		}

		[TestMethod]
		public void Test04_OrElse2()
		{
			var s = @"
var a = 'ok';
false || (a+='hello')=='okhello'
";
			var script = new Script();
			Assert.IsTrue(script.Eval<bool>(s));
			Assert.AreEqual("okhello", script.Eval("a"));
		}

		[TestMethod]
		public void Test04_OrElse2_CompileAll()
		{
			var s = @"
var a = 'ok';
false || (a+='hello')=='okhello'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.IsTrue(script.Eval<bool>(s));
			Assert.AreEqual("okhello", script.Eval("a"));
		}

		[TestMethod]
		public void Test04_OrElse()
		{
			var a = "ok";
			Assert.IsTrue(true || (a += "hello") == "hello");
			Assert.AreEqual("ok", a);

			var s = @"
var a = 'ok';
true || (a+='hello')=='hello'
";
			var script = new Script();
			Assert.IsTrue(script.Eval<bool>(s));
			Assert.AreEqual("ok", script.Eval("a"));
		}

		[TestMethod]
		public void Test04_OrElse_CompileAll()
		{
			var s = @"
var a = 'ok';
true || (a+='hello')=='hello'
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.IsTrue(script.Eval<bool>(s));
			Assert.AreEqual("ok", script.Eval("a"));
		}

		[TestMethod]
		public void Test03()
		{
			Assert.IsTrue(!true == false);
			var script = new Script();
			Assert.AreEqual(!true == false, script.Eval<bool>("!true == false"));
			Assert.AreEqual(!true == false, script.Eval<bool>("!true == false", ECompileMode.All));
			Assert.AreEqual(1 == 2 || 5 > 2, script.Eval<bool>("1==2||5>2"));
			Assert.AreEqual(1 == 2 && 5 > 2, script.Eval<bool>("1==2&&5>2"));
			Assert.AreEqual(1 <= 2 && 5 > 2, script.Eval<bool>("1<=2&&5>2"));
			Assert.AreEqual(1 == 2 || 5 > 2, script.Eval<bool>("1==2||5>2", ECompileMode.All));
			Assert.AreEqual(1 == 2 && 5 > 2, script.Eval<bool>("1==2&&5>2", ECompileMode.All));
			Assert.AreEqual(1 <= 2 && 5 > 2, script.Eval<bool>("1<=2&&5>2", ECompileMode.All));
		}

		[TestMethod]
		public void Test02()
		{
			var script = new Script();
			Assert.AreEqual(true, script.Eval("n=1>0; n==true"));
			Assert.AreEqual(false, script.Eval("n=1>0; n==false"));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			Assert.AreEqual(true, script.Eval("true"));
			Assert.AreEqual(false, script.Eval("false"));
		}
	}
}
