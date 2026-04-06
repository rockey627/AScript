using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptStringTest
	{
		[TestMethod]
		public void Test06()
		{
			string s = "string s = 'hel';s+='lo5'";
			var script = new Script();
			Assert.AreEqual("hello5", script.Eval(s));
			Assert.AreEqual("hello567", script.Eval("s+=67"));
		}

		//[TestMethod]
		//public void Test05_AScript()
		//{
		//	string s = "string s = 'hel';s+='lo5'";
		//	var script = new AScript.CSharp.CSharpScript();
		//	Assert.AreEqual("hello5", script.Eval(s, ECompileMode.All));
		//	Assert.AreEqual("hello567", script.Eval("s+=67"));
		//}

		[TestMethod]
		public void Test05()
		{
			string s = "string s = 'hel';s+='lo5'";
			var script = new Script();
			Assert.AreEqual("hello5", script.Eval(s, ECompileMode.All));
			Assert.AreEqual("hello567", script.Eval("s+=67"));
		}

		[TestMethod]
		public void Test04()
		{
			var script = new Script();
			Assert.AreEqual("hello", script.Eval("'hel'+'lo'", ECompileMode.All));
		}

		[TestMethod]
		public void Test03()
		{
			var script = new Script();
			Assert.AreEqual("hello'everyone", script.Eval("'hello\\'everyone'"));
			Assert.AreEqual("hello\"everyone", script.Eval("'hello\"everyone'"));
			Assert.AreEqual("hello\"everyone", script.Eval("\"hello\\\"everyone\""));
			Assert.AreEqual("hello'everyone", script.Eval("\"hello'everyone\""));
			Assert.AreEqual("hello\neveryone", script.Eval("'hello\\neveryone'"));
			int n = 0;
			try
			{
				script.Eval("'hello\\everyone'");
				Assert.IsTrue(false);
			}
			catch (Exception ex)
			{
				n++;
				Assert.AreEqual("unknown string escape:'hello\\everyone'", ex.Message);
			}
			Assert.AreEqual(1, n);
		}

		[TestMethod]
		public void Test02_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("hello5", script.Eval("'hello'+5"));
			Assert.AreEqual("hello5", script.Eval("\"hello\"+5"));
		}

		[TestMethod]
		public void Test02()
		{
			var script = new Script();
			Assert.AreEqual("hello5", script.Eval("'hello'+5"));
			Assert.AreEqual("hello5", script.Eval("\"hello\"+5"));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			Assert.AreEqual("", script.Eval("''"));
			Assert.AreEqual("hello", script.Eval("'hello'"));
			Assert.AreEqual("", script.Eval("\"\""));
			Assert.AreEqual("hello", script.Eval("\"hello\""));
		}
	}
}
