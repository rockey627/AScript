using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptCommonTest
	{
		[TestMethod]
		public void Test26_null_2()
		{
			string s = "FileInfo a = null;a==null";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Options.ThrowIfVariableNotExists = true;
			Assert.AreEqual(true, script.Eval(s));
			Assert.IsNull(script.Eval("a", out var type));
			Assert.AreEqual(typeof(FileInfo), type);
		}

		[TestMethod]
		public void Test26_null()
		{
			string s = "FileInfo a = null;a==null";
			var script = new Script();
			script.Options.ThrowIfVariableNotExists = true;
			Assert.AreEqual(true, script.Eval(s));
			Assert.IsNull(script.Eval("a", out var type));
			Assert.AreEqual(typeof(FileInfo), type);
		}

		[TestMethod]
		public void Test25_null_2()
		{
			string s = "object a = null;a==null";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Options.ThrowIfVariableNotExists = true;
			Assert.AreEqual(true, script.Eval(s));
			Assert.IsNull(script.Eval("a"));
		}

		[TestMethod]
		public void Test25_null()
		{
			string s = "object a = null;a==null";
			var script = new Script();
			script.Options.ThrowIfVariableNotExists = true;
			Assert.AreEqual(true, script.Eval(s));
			Assert.IsNull(script.Eval("a"));
		}

		[TestMethod]
		public void Test24_5_2()
		{
			string s = @"
int exec(int a) {
	if (a < 1) return 0;
	a + exec(a-1);
}
exec(5)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(15, script.Eval(s));
			Assert.AreEqual(55, script.Eval("exec(10)"));
		}

		[TestMethod]
		public void Test24_5()
		{
			string s = @"
int exec(int a) {
	if (a < 1) return 0;
	a + exec(a-1);
}
exec(5)
";
			var script = new Script();
			Assert.AreEqual(15, script.Eval(s));
			Assert.AreEqual(55, script.Eval("exec(10)"));
		}

		[TestMethod]
		public void Test24_2()
		{
			string s = @"
int exec(int a) {
	if (a < 1) return 0;
	a + exec(a-1);
}
exec(2)
";
			var script = new Script();
			Assert.AreEqual(3, script.Eval(s));
		}

		[TestMethod]
		public void Test24_1_2()
		{
			string s = @"
int exec(int a) {
	if (a < 1) return 0;
	a + exec(a-1);
}
exec(1)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test24_1()
		{
			string s = @"
int exec(int a) {
	if (a < 1) return 0;
	a + exec(a-1);
}
exec(1)
";
			var script = new Script();
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test24_0()
		{
			string s = @"
int exec(int a) {
	if (a < 1) return 0;
	a + exec(a-1);
}
exec(0)
";
			var script = new Script();
			Assert.AreEqual(0, script.Eval(s));
		}

		[TestMethod]
		public void Test23_DateTime()
		{
			var script = new Script();
			Assert.AreEqual(DateTime.Now.Year, script.Eval("DateTime.Now.Year"));
			Assert.AreEqual(DateTime.Now.Year, script.Eval("DateTime.Now.Year", ECompileMode.All));
		}

		[TestMethod]
		public void Test22_4()
		{
			var s = "int n=1;1==0?++n+10:n+=5";
			int n = 1;
			int m = 1 == 0 ? ++n + 10 : n += 5;
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Options.RewriteVariables = false;
			script.Options.ThrowIfVariableNotExists = true;
			Assert.AreEqual(m, script.Eval(s));
			try
			{
				Assert.AreEqual(n, script.Eval("n"));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Assert.AreEqual("variable n is not exists", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test22_3()
		{
			var s = "int n=1;1==0?++n+10:n+=5";
			int n = 1;
			int m = 1 == 0 ? ++n + 10 : n += 5;
			{
				var script = new Script();
				Assert.AreEqual(m, script.Eval(s));
				Assert.AreEqual(n, script.Context.EvalVar("n"));
			}
			{
				var script = new Script();
				script.Options.CompileMode = ECompileMode.All;
				Assert.AreEqual(m, script.Eval(s));
				Assert.AreEqual(n, script.Context.EvalVar("n"));
			}
		}

		[TestMethod]
		public void Test22_2()
		{
			var s = "int n=1;1==0?++n+10:(n+=5)";
			int n = 1;
			int m = 1 == 0 ? ++n + 10 : (n += 5);
			{
				var script = new Script();
				Assert.AreEqual(m, script.Eval(s));
				Assert.AreEqual(n, script.Context.EvalVar("n"));
			}
			{
				var script = new Script();
				script.Options.CompileMode = ECompileMode.All;
				Assert.AreEqual(m, script.Eval(s));
				Assert.AreEqual(n, script.Context.EvalVar("n"));
			}
		}

		[TestMethod]
		public void Test22()
		{
			var s = "int n=1;1==0? ++n+10:(n+=5)";
			int n = 1;
			int m = 1 == 0 ? ++n + 10 : (n += 5);
			{
				var script = new Script();
				Assert.AreEqual(m, script.Eval(s));
				Assert.AreEqual(n, script.Context.EvalVar("n"));
			}
			{
				var script = new Script();
				script.Options.CompileMode = ECompileMode.All;
				Assert.AreEqual(m, script.Eval(s));
				Assert.AreEqual(n, script.Context.EvalVar("n"));
			}
		}

		[TestMethod]
		public void Test21()
		{
			var s = "int n=1;1==1? ++n+10:(n+=2)";
			{
				var script = new Script();
				Assert.AreEqual(12, script.Eval(s));
				Assert.AreEqual(2, script.Context.EvalVar("n"));
			}
			{
				var script = new Script();
				script.Options.CompileMode = ECompileMode.All;
				Assert.AreEqual(12, script.Eval(s));
				Assert.AreEqual(2, script.Context.EvalVar("n"));
			}
		}

		[TestMethod]
		public void Test20()
		{
			var script = new Script();
			Assert.AreEqual(2, script.Eval("1==1?2:3"));
			Assert.AreEqual(2, script.Eval("1==1?2:3", ECompileMode.All));
			Assert.AreEqual(2, script.Eval("(1==1?2:3)"));
			Assert.AreEqual(2, script.Eval("(1==1?2:3)", ECompileMode.All));
			Assert.AreEqual(5, script.Eval("(1==1?2:3)+3"));
			Assert.AreEqual(5, script.Eval("(1==1?2:3)+3", ECompileMode.All));
			Assert.AreEqual(3, script.Eval("1==0?2:3"));
			Assert.AreEqual(3, script.Eval("1==0?2:3", ECompileMode.All));
			Assert.AreEqual(6, script.Eval("n=(1==0?2:3)+3"));
		}

		//[TestMethod]
		//public void Test19()
		//{
		//	string s = "new List<int>{1,2,3}";
		//	var script = new Script();
		//	var r = script.Eval(s);
		//	Assert.AreEqual(typeof(List<int>), r.GetType());
		//	Assert.AreEqual("1;2;3", string.Join(";", ((List<int>)r).Select(a => a.ToString())));
		//}

		[TestMethod]
		public void Test18_1()
		{
			var script = new Script();
			script.Context.AddType<Person>();
			script.Eval("var p = new Person('tom', 20);", ECompileMode.All);
			var p = new Person("tom", 20);
			p.Age = 30;
			Assert.AreEqual(p.SayHello(), script.Eval("p.Age=30;p.SayHello()"));
			Assert.AreEqual(p.SayHello(), script.Eval("p.Age=30;p.SayHello()", ECompileMode.All));
			p.Age += 10;
			Assert.AreEqual(p.SayHello(), script.Eval("p.Age+=10;p.SayHello()", ECompileMode.All));
			p.Age += 20;
			Assert.AreEqual(p.SayHello(), script.Eval("p.Age+=20;p.SayHello()"));
			p.Name2 = "yyy";
			Assert.AreEqual(p.SayHello(), script.Eval("p.Name2='yyy';p.SayHello()"));
			p.Name2 = "xxx";
			Assert.AreEqual(p.SayHello(), script.Eval("p.Name2='xxx';p.SayHello()", ECompileMode.All));
		}

		[TestMethod]
		public void Test17_8()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			string s = "var p = new Person{Name='tom',Age=20};p.Name";
			Assert.AreEqual("tom", script.Eval(s));
			Assert.AreEqual("tom", script.Eval("p.Name"));
			Assert.AreEqual(20, script.Eval("p.Age"));
		}

		[TestMethod]
		public void Test17_7()
		{
			var script = new Script();
			script.Context.AddType<Person>();
			string s = "var p = new Person{Name='tom',Age=20};p.Name";
			Assert.AreEqual("tom", script.Eval(s));
			Assert.AreEqual("tom", script.Eval("p.Name"));
			Assert.AreEqual(20, script.Eval("p.Age"));
		}

		[TestMethod]
		public void Test17_6()
		{
			var script = new Script();
			script.Context.AddType<Person>();
			var p0 = new Person { Name = "tom", Age = 20 };
			var p = (Person)script.Eval("var p = new Person{Name='tom',Age=20}");
			Assert.AreEqual(p0.SayHello(), script.Eval("p.SayHello()"));
			Assert.AreEqual(p0.SayHello(), script.Eval("p.SayHello()", ECompileMode.All));
			Assert.AreEqual(p0.SayHello(), p.SayHello());
		}

		[TestMethod]
		public void Test17_5()
		{
			var script = new Script();
			script.Context.AddType<Person>();
			var p0 = new Person { Name = "tom", Age = 20 };
			var p = (Person)script.Eval("var p = new Person{Name='tom',Age=20}", ECompileMode.All);
			Assert.AreEqual(p0.SayHello(), script.Eval("p.SayHello()"));
			Assert.AreEqual(p0.SayHello(), script.Eval("p.SayHello()", ECompileMode.All));
			Assert.AreEqual(p0.SayHello(), p.SayHello());
		}

		[TestMethod]
		public void Test17_4()
		{
			var script = new Script();
			script.Context.AddType<Person>();
			var p0 = new Person { Name = "tom", Age = 20 };
			var p = (Person)script.Eval("var p = new Person('tom', 20)", ECompileMode.All);
			Assert.AreEqual(p0.SayHello(), script.Eval("p.SayHello()"));
			Assert.AreEqual(p0.SayHello(), script.Eval("p.SayHello()", ECompileMode.All));
			Assert.AreEqual(p0.SayHello(), p.SayHello());
		}

		[TestMethod]
		public void Test17_3()
		{
			var script = new Script();
			script.Context.AddType<Person>();
			var p0 = new Person { Name = "tom", Age = 20 };
			var p = (Person)script.Eval("var p = new Person('tom', 20)");
			Assert.AreEqual(p0.SayHello(), script.Eval("p.SayHello()"));
			Assert.AreEqual(p0.SayHello(), script.Eval("p.SayHello()", ECompileMode.All));
			Assert.AreEqual(p0.SayHello(), p.SayHello());
		}

		[TestMethod]
		public void Test17_2()
		{
			var script = new Script();
			script.Context.AddType<Person>();
			var p = (Person)script.Eval("var p = Person.Create('tom', 20)", ECompileMode.All);
			Assert.AreEqual(p.SayHello(), script.Eval("p.SayHello()"));
			Assert.AreEqual(p.SayHello(), script.Eval("p.SayHello()", ECompileMode.All));
		}

		[TestMethod]
		public void Test17()
		{
			var script = new Script();
			script.Context.AddType<Person>();
			var p = (Person)script.Eval("var p = Person.Create('tom', 20)");
			Assert.AreEqual(p.SayHello(), script.Eval("p.SayHello()"));
			Assert.AreEqual(p.SayHello(), script.Eval("p.SayHello()", ECompileMode.All));
		}

		[TestMethod]
		public void Test16_2()
		{
			var script = new Script();
			Assert.AreEqual(2, script.Eval("n+2"));
		}

		[TestMethod]
		public void Test16_1()
		{
			var script = new Script();
			script.Options.ThrowIfVariableNotExists = true;
			int n = 0;
			try
			{
				script.Eval("n+2");
			}
			catch (Exception ex)
			{
				n++;
				Console.WriteLine(ex);
			}
			Assert.AreEqual(1, n);
		}

		[TestMethod]
		public void Test15()
		{
			var script = new Script();
			Assert.AreEqual(8, script.Eval("s='a+5';{a=3;eval(s)}"));
			Assert.AreEqual(null, script.Eval("a"));
		}

		[TestMethod]
		public void Test14()
		{
			var script = new Script();
			Assert.AreEqual(8, script.Eval("s='a+5';a=3;eval(s)"));
			Assert.AreEqual(3, script.Eval("a"));
		}

		[TestMethod]
		public void Test13()
		{
			var script = new Script();
			Assert.AreEqual(26, script.Eval("a=5;b=6;a+=b;return (a+2)*2;a+=2;a+5"));
			Assert.AreEqual(11, script.Eval("a"));
			Assert.AreEqual(6, script.Eval("b"));
		}

		[TestMethod]
		public void Test12()
		{
			var script = new Script();
			Assert.AreEqual(13, script.Eval("a=5;b=6;a+=b;return (a+2);a+=2;a+5"));
			Assert.AreEqual(11, script.Eval("a"));
			Assert.AreEqual(6, script.Eval("b"));
		}

		[TestMethod]
		public void Test11()
		{
			var script = new Script();
			Assert.AreEqual(13, script.Eval("a=5;{b=6;a+=b;return (a+2)}a+=2;a+5"));
			Assert.AreEqual(11, script.Eval("a"));
			Assert.AreEqual(null, script.Eval("b"));
		}

		[TestMethod]
		public void Test10()
		{
			var script = new Script();
			Assert.AreEqual(11, script.Eval("a=5;{b=6;a+=b;return a}a+=2;a+5"));
			Assert.AreEqual(11, script.Eval("a"));
			Assert.AreEqual(null, script.Eval("b"));
		}

		[TestMethod]
		public void Test09()
		{
			var script = new Script();
			Assert.AreEqual(18, script.Eval("a=5;{b=6;a+=b;}a+=2;a+5"));
			Assert.AreEqual(13, script.Eval("a"));
			Assert.AreEqual(null, script.Eval("b"));
		}

		[TestMethod]
		public void Test08_3()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(6, script.Eval("a=5;{b=6}"));
			Assert.AreEqual(5, script.Eval("a"));
			Assert.AreEqual(null, script.Eval("b"));
		}

		[TestMethod]
		public void Test08_2()
		{
			var script = new Script();
			Assert.AreEqual(6, script.Eval("a=5;{b=6}"));
			Assert.AreEqual(5, script.Eval("a"));
			Assert.AreEqual(null, script.Eval("b"));
		}

		[TestMethod]
		public void Test08_1()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(11, script.Eval("a=5;{b=6;a+=b;}"));
			Assert.AreEqual(11, script.Eval("a"));
			Assert.AreEqual(null, script.Eval("b"));
		}

		[TestMethod]
		public void Test08()
		{
			var script = new Script();
			Assert.AreEqual(11, script.Eval("a=5;{b=6;a+=b;}"));
			Assert.AreEqual(11, script.Eval("a"));
			Assert.AreEqual(null, script.Eval("b"));
		}

		[TestMethod]
		public void Test07_3()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			int a = 5;
			string s = null;
			string t = s ?? ("hello" + (++a)) + 20;
			Assert.AreEqual(t, script.Eval("a=5;string s;s?=('hello'+(++a))+20"));
			Assert.AreEqual(t, script.Eval("s"));
			Assert.AreEqual(6, script.Eval("a"));
		}

		[TestMethod]
		public void Test07_2()
		{
			var script = new Script();
			int a = 5;
			string s = null;
			string t = s ?? ("hello" + (++a)) + 20;
			Assert.AreEqual(t, script.Eval("a=5;string s;s?=('hello'+(++a))+20"));
			Assert.AreEqual(t, script.Eval("s"));
			Assert.AreEqual(6, script.Eval("a"));
		}

		[TestMethod]
		public void Test07()
		{
			var script = new Script();
			int a = 5;
			string s = null;
			Assert.AreEqual(s ?? "hello" + (++a), script.Eval("a=5;string s;s?='hello'+(++a)"));
			Assert.AreEqual("hello6", script.Eval("s"));
			Assert.AreEqual(6, script.Eval("a"));
		}

		[TestMethod]
		public void Test06()
		{
			var script = new Script();
			int a = 5;
			string s = "";
			Assert.AreEqual(s ?? "hello" + (++a), script.Eval("a=5;s='';s?='hello'+(++a)"));
			Assert.AreEqual("", script.Eval("s"));
			Assert.AreEqual(5, script.Eval("a"));
		}

		[TestMethod]
		public void Test05_4()
		{
			var script = new Script();
			int a = 5;
			string s1 = null;
			string s2 = "";
			Assert.AreEqual(s1 ?? s2 ?? "hello" + (++a), script.Eval("a=5;string s1;string s2='';s1??s2??'hello'+(++a)"));
			Assert.AreEqual(5, script.Eval("a"));
		}

		[TestMethod]
		public void Test05_3()
		{
			var script = new Script();
			int a = 5;
			string s = null;
			Assert.AreEqual(s ?? "hello" + (++a), script.Eval("a=5;string s1;string s2;s1??s2??'hello'+(++a)"));
			Assert.AreEqual(6, script.Eval("a"));
		}

		[TestMethod]
		public void Test05_2()
		{
			var script = new Script();
			int a = 5;
			string s = null;
			Assert.AreEqual(s ?? ("hello" + (++a)) + 6, script.Eval("a=5;string s;s??('hello'+(++a))+6"));
			Assert.AreEqual(6, script.Eval("a"));
		}

		[TestMethod]
		public void Test05()
		{
			var script = new Script();
			int a = 5;
			string s = null;
			Assert.AreEqual(s ?? "hello" + (++a), script.Eval("a=5;string s;s??'hello'+(++a)"));
			Assert.AreEqual(6, script.Eval("a"));
		}

		[TestMethod]
		public void Test04_4()
		{
			var script = new Script();
			int a = 5;
			string s = "";
			Assert.AreEqual(s ?? ("hello" + (++a)) + 6, script.Eval("a=5;s='';s??'hello'+(++a)+6"));
			Assert.AreEqual(5, script.Eval("a"));
		}

		[TestMethod]
		public void Test04_3()
		{
			var script = new Script();
			int a = 5;
			string s = "";
			Assert.AreEqual(7, script.Eval("a=5;s='';s??('hello'+(++a))+6;7"));
			Assert.AreEqual(5, script.Eval("a"));
		}

		[TestMethod]
		public void Test04_2()
		{
			var script = new Script();
			int a = 5;
			string s = "";
			Assert.AreEqual(s ?? ("hello" + (++a)) + 6, script.Eval("a=5;s='';s??('hello'+(++a))+6"));
			Assert.AreEqual(5, script.Eval("a"));
		}

		[TestMethod]
		public void Test04()
		{
			var script = new Script();
			int a = 5;
			string s = "";
			Assert.AreEqual(s ?? "hello" + (++a), script.Eval("a=5;s='';s??'hello'+(++a)"));
			Assert.AreEqual(5, script.Eval("a"));
		}

		[TestMethod]
		public void Test03()
		{
			var script = new Script();
			Assert.AreEqual("hello6", script.Eval("a=5;'hello'+(++a)"));
		}

		[TestMethod]
		public void Test02_3()
		{
			var script = new Script();
			Assert.AreEqual("hello", script.Eval("string s;s??'hello'"));
			Assert.AreEqual("", script.Eval("s='';s??'hello'"));
		}

		[TestMethod]
		public void Test02_2()
		{
			var script = new Script();
			Assert.AreEqual("", script.Eval("s='';s??'hello'"));
		}

		//[TestMethod]
		//public void Test02_AScript()
		//{
		//	var script = new AScript.CSharp.CSharpScript();
		//	Assert.AreEqual("hello", script.Eval("string s;s??'hello'"));
		//}

		[TestMethod]
		public void Test02()
		{
			var script = new Script();
			Assert.AreEqual("hello", script.Eval("string s;s??'hello'"));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			Assert.AreEqual(6, script.Eval("4 6"));
		}
	}
}
