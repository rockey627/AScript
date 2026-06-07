using AScript.Lang.Python3;
using AScript.Lang.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptLangTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["python3"] = Python3Lang.Instance;
			Script.Langs["sql"] = SqlLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("python3");
			Script.Langs.TryRemove("sql");
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = @"
int n=10;
@lang python3
def sum(a,b):
	return a+b
@end
@lang sql
declare m int;
set m=20;
@end
sum(m, n)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(30, script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
int n=10;
@lang python3
def sum(a,b):
	return a+b
@end
@lang sql
declare m int;
set m=20;
@end
sum(m, n)";
			var script = new Script();
			Assert.AreEqual(30, script.Eval(s));
		}
	}
}
