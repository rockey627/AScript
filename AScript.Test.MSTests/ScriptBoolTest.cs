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
		public void Test03()
		{
			var script = new Script();
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
