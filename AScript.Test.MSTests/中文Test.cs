using AScript.Test.MSTests.中文;
using System;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class 中文Test
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["中文"] = 中文语言.实例;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("中文", out _);
		}

		[TestMethod]
		public void Test02()
		{
			string s = @"
int n=10;
#lang 中文,CSharp
整型 m=20;
#end
m+n";
			var script = new Script();
			Assert.AreEqual(30, script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
整型 n=10;
文本 s='';
如果 n<5 则 {
	s='小于5';
} 否则 如果 n<20 则 {
	s='大于等于5且小于20';
} 否则 {
	s='大于等于20';
}
返回 $'{n},{s}';
";
			var script = new Script();
			Assert.AreEqual("10,大于等于5且小于20", script.Eval(s));
			Assert.AreEqual("10,大于等于5且小于20", script.Eval(s, ECompileMode.All));
		}
	}
}
