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
		public void Test04_2()
		{
			string s = @"
string exec(int a) {
#lang python
	#使用python语言
	m=0
	s=''
	if a>0 and a<10: 
	  m=1
	  s='大于0且小于10'
	elif @lang csharp a>=10 && a<20 @end : # 条件嵌入csharp语言
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
    m+','+s
#end
}
exec(26)
";
			var script = new Script();
			// 编译执行模式
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test04()
		{
			string s = @"
string exec(int a) {
#lang python
	#使用python语言
	m=0
	s=''
	if a>0 and a<10: 
	  m=1
	  s='大于0且小于10'
	elif @lang csharp a>=10 && a<20 @end: # 条件嵌入csharp语言
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
    m+','+s
#end
}
exec(26)
";
			var script = new Script();
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test03()
		{
			string s = @"
string exec(int a) {
#lang python
	#使用python语言
	m=0
	s=''
	if a>0 and a<10: 
	  m=1
	  s='大于0且小于10'
	elif a>=10 and a<20:
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
    m+','+s
#end
}
exec(26)
";
			var script = new Script();
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
		}

		[TestMethod]
		public void Test02()
		{
			string s = @"
m=0
s=''#行注释
if a>0'''中间注释''' and a<10: #行注释 (0~10)
  m=1
'''
多行注释1
多行注释2
多选注释3
'''
  s='大于0且小于10' #行注释说明
''''''#空注释
elif a>=10 and a""""""中间注释""""""<20:#行注释[10,20)
  m=2
  s='大于等于10且小于20'
else :
'''
多行注释1
多行注释2
多选注释3
'''
  m=3#其他
m+','+s
";
			var script = new Script();
			script.Context.Langs = new[] { "python" };
			script.Context.SetVar("a", 6);
			Assert.AreEqual("1,大于0且小于10", script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
m=0
s=''
if a>0 and a<10: 
  m=1
  s='大于0且小于10'
elif a>=10 and a<20:
  m=2
  s='大于等于10且小于20'
elif a>=20 and a<30:
  m=3
  s='大于等于20且小于30'
else :
  m=4
  s='大于等于30'
m+','+s
";
			var script = new Script();
			script.Context.Langs = new[] { "python" };
			script.Context.SetVar("a", 6);
			Assert.AreEqual("1,大于0且小于10", script.Eval(s));
			script.Context.SetVar("a", 16);
			Assert.AreEqual("2,大于等于10且小于20", script.Eval(s));
			script.Context.SetVar("a", 26);
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			script.Context.SetVar("a", 36);
			Assert.AreEqual("4,大于等于30", script.Eval(s));
		}
	}
}
