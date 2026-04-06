using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptWhileTest
	{
		[TestMethod]
		public void Test02_2()
		{
			string s = @"
int n = 6;
int m=10;
while(n>0){
	m++;
	n--;
	if (n==3) break;
}
m+10";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(23, script.Eval(s));
			Assert.AreEqual(3, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test02()
		{
			string s = @"
int n = 6;
int m=10;
while(n>0){
	m++;
	n--;
	if (n==3) break;
}
m+10";
			var script = new Script();
			Assert.AreEqual(23, script.Eval(s));
			Assert.AreEqual(3, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = @"int n = 6;int m=10;while(n>0){m++;n--}m+10";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(26, script.Eval(s));
			Assert.AreEqual(0, script.Context.EvalVar("n", out var type));
			Assert.AreEqual(typeof(int), type);
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"int n = 6;int m=10;while(n>0){m++;n--}m+10";
			var script = new Script();
			Assert.AreEqual(26, script.Eval(s));
			Assert.AreEqual(0, script.Context.EvalVar("n"));
		}
	}
}
