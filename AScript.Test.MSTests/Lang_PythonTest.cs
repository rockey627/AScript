using AScript.Lang.Python3;
using System;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class Lang_PythonTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["python3"] = Python3Lang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("python3");
		}

		[TestMethod]
		public void Test13_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(true, script.Eval("True"));
			Assert.AreEqual(false, script.Eval("False"));
		}

		[TestMethod]
		public void Test13()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(true, script.Eval("True"));
			Assert.AreEqual(false, script.Eval("False"));
		}

		[TestMethod]
		public void Test12_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			script.Context.SetVar("name", "tom");
			Assert.AreEqual("hello tom", script.Eval("f'hello {name}'"));
		}

		[TestMethod]
		public void Test12()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			script.Context.SetVar("name", "tom");
			Assert.AreEqual("hello tom", script.Eval("f'hello {name}'"));
		}

		[TestMethod]
		public void Test11_range_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			var r = script.Eval("range(-2, 4)");
			Assert.IsTrue(r is IReadOnlyList<int>);
			var list = (IReadOnlyList<int>)r;
			Assert.AreEqual(6, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual(i - 2, list[i]);
			}
		}

		[TestMethod]
		public void Test11_range()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			var r = script.Eval("range(-2, 4)");
			Assert.IsTrue(r is IReadOnlyList<int>);
			var list = (IReadOnlyList<int>)r;
			Assert.AreEqual(6, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual(i - 2, list[i]);
			}
		}

		[TestMethod]
		public void Test10_range_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			var r = script.Eval("range(4)");
			Assert.IsTrue(r is IReadOnlyList<int>);
			var list = (IReadOnlyList<int>)r;
			Assert.AreEqual(4, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual(i, list[i]);
			}
		}

		[TestMethod]
		public void Test10_range()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			var r = script.Eval("range(4)");
			Assert.IsTrue(r is IReadOnlyList<int>);
			var list = (IReadOnlyList<int>)r;
			Assert.AreEqual(4, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual(i, list[i]);
			}
		}

		[TestMethod]
		public void Test09()
		{
			Console.WriteLine(Math.Floor(-9.0 / 2));
			//Console.WriteLine(2 ** 3);
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(2.5, script.Eval("10/4"));
			Assert.AreEqual(2.5, script.Eval("n=10\nn/=4"));
			Assert.AreEqual(2, script.Eval("10//4"));
			Assert.AreEqual(2, script.Eval("n=10\nn//=4"));
			Assert.AreEqual(2.0, script.Eval("10.4//4"));
			Assert.AreEqual(2.0, script.Eval("n:=10.4//4"));
			Assert.AreEqual(2.0, script.Eval("n=10.4\nn//=4"));
			Assert.AreEqual(4, script.Eval("9//2"));
			Assert.AreEqual(-5, script.Eval("-9//2"));
			Assert.AreEqual(-5, script.Eval("n:=-9//2"));
			Assert.AreEqual(-5, script.Eval("n=-9\nn//=2"));
		}

		[TestMethod]
		public void Test08()
		{
			string s = @"
'''
多行文本1
多行文本2
多行文本3
'''
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			string r = @"
多行文本1
多行文本2
多行文本3
";
			Assert.AreEqual(r, script.Eval(s));
		}

		[TestMethod]
		public void Test07()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			script.Eval("print('hello')");
			script.Eval("exec('print(\\'hello everyone\\')')");
		}

		[TestMethod]
		public void Test06_2()
		{
			string s = @"
string exec(int a) {
#lang python3
	#使用python3语言
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
			var tasks = new Task[100];
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = Task.Run(() =>
				{
					var script = new Script();
					script.Options.CompileMode = ECompileMode.All;
					Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
					Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
				});
			}
			Task.WaitAll(tasks);
		}

		[TestMethod]
		public void Test06()
		{
			string s = @"
string exec(int a) {
#lang python3
	#使用python3语言
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
			var tasks = new Task[100];
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = Task.Run(() =>
				{
					var script = new Script();
					Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
					Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
				});
			}
			Task.WaitAll(tasks);
		}

		[TestMethod]
		public void Test05_2()
		{
			string s = @"
def exec(a) :
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

exec(26)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test05()
		{
			string s = @"
def exec(a) :
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

exec(26)
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test04_2()
		{
			string s = @"
string exec(int a) {
#lang python3
	#使用python3语言
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
    return m+','+s
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
#lang python3
	#使用python3语言
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
    return m+','+s
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
#lang python3
	#使用python3语言
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
if a>0 and a<10: #行注释 (0~10)
  m=1
  '''
多行注释1
多行注释2
多选注释3
'''
  s='大于0且小于10' #行注释说明
  ''''''#空注释
elif a>=10 and a<20:#行注释[10,20)
  m=2
  s='大于等于10且小于20'
else :
  '''
多行文本1
多行文本2
多选文本3
'''
  m=3#其他
m+','+s
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
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
			script.Context.Langs = new[] { "python3" };
			script.Context.SetVar("a", 6);
			Assert.AreEqual("1,大于0且小于10", script.Eval(s));
			script.Context.SetVar("a", 16);
			Assert.AreEqual("2,大于等于10且小于20", script.Eval(s));
			script.Context.SetVar("a", 26);
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			script.Context.SetVar("a", 36);
			Assert.AreEqual("4,大于等于30", script.Eval(s));
		}

		[TestMethod]
		public void Test00_2()
		{
			string s = @"
def exec(a) :
    m=0
    s=''
    if a>0 and a<10 : 
        m=1
        s='大于0且小于10'
    elif a>=10 and a<20 :
        m=2
        s='大于等于10且小于20'
    elif a>=20 and a<30 :
        m=3
        s='大于等于20且小于30'
    else :
        m=4
        s='大于等于30'
    return f'{m},{s}'

exec(26)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
		}

		[TestMethod]
		public void Test00()
		{
			string s = @"
def exec(a) :
    m=0
    s=''
    if a>0 and a<10 : 
        m=1
        s='大于0且小于10'
    elif a>=10 and a<20 :
        m=2
        s='大于等于10且小于20'
    elif a>=20 and a<30 :
        m=3
        s='大于等于20且小于30'
    else :
        m=4
        s='大于等于30'
    return f'{m},{s}'

exec(26)
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
		}
	}
}
