using AScript.Test.MSTests.Python;
using AScript.Test.MSTests.Sql;
using System;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class Lang_PythonTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["python"] = PythonLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("python", out _);
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
m=0
if a>0 and a<10:
  m=1
  m=m+1
elif a>=10 and a<20:
  m=2
  m=m+2
else :
  m=3
m=m+3
";
			var script = new Script();
			script.Context.Langs = new[] { "python" };
			script.Context.SetVar("a", 6);
			Assert.AreEqual(5, script.Eval(s));
		}
	}
}
