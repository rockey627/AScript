using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptDotTest
	{
		[TestMethod]
		public void Test02_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("hello".Substring(1, 2), script.Eval("'hello'.Substring(1, 2)"));
			Assert.AreEqual("ha ha" + "hello".Substring(1, 2) + 5, script.Eval("'ha ha'+'hello'.Substring(1, 2)+5"));
		}

		[TestMethod]
		public void Test02()
		{
			var script = new Script();
			Assert.AreEqual("hello".Substring(1, 2), script.Eval("'hello'.Substring(1, 2)"));
			Assert.AreEqual("ha ha" + "hello".Substring(1, 2) + 5, script.Eval("'ha ha'+'hello'.Substring(1, 2)+5"));
		}

		[TestMethod]
		public void Test01_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(DateTime.Now.Year, script.Eval("DateTime.Now.Year"));
			Assert.AreEqual(int.MaxValue, script.Eval("int.MaxValue"));
			Assert.AreEqual("hello".Length, script.Eval("'hello'.Length"));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			Assert.AreEqual(DateTime.Now.Year, script.Eval("DateTime.Now.Year"));
			Assert.AreEqual(int.MaxValue, script.Eval("int.MaxValue"));
			Assert.AreEqual("hello".Length, script.Eval("'hello'.Length"));
		}
	}
}
