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
		public void Test26_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			try
			{
				script.Eval("$'hello {'tom'}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Assert.AreEqual("invalid string at (1,15), expect '", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test26()
		{
			var script = new Script();
			try
			{
				script.Eval("$'hello {'tom'}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Assert.AreEqual("invalid string at (1,15), expect '", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test25_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			try
			{
				script.Eval("$'hello");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Assert.AreEqual("invalid string at (1,7), expect '", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test25()
		{
			var script = new Script();
			try
			{
				script.Eval("$'hello");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Assert.AreEqual("invalid string at (1,7), expect '", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test24_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			try
			{
				script.Eval("'hello");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Assert.AreEqual("invalid string at (1,6), expect '", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test24()
		{
			var script = new Script();
			try
			{
				script.Eval("'hello");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Assert.AreEqual("invalid string at (1,6), expect '", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test23()
		{
			// 字符串内插中使用复杂表达式
			string s = "var a=1; var b=2; var c=3; $'result={(a+b)*c}'";
			var script = new Script();
			Assert.AreEqual("result=9", script.Eval(s));
		}

		[TestMethod]
		public void Test22()
		{
			// 字符串内插中包含转义字符
			string s = "var n='newline'; $'{n}\\ntest'";
			var script = new Script();
			Assert.AreEqual("newline\ntest", script.Eval(s));
		}

		[TestMethod]
		public void Test13()
		{
			string s = "var name='tom'; { int age=16; $'hello {name+'ok'}, {{age + 10}' }";
			var script = new Script();
			var name = "tom";
			int age = 16;
			Assert.AreEqual($"hello {name + "ok"}, {{age + 10}}", script.Eval(s));
		}

		[TestMethod]
		public void Test12()
		{
			string s = "var name='tom'; { int age=16; $'hello {name}, {{age + 10}' }";
			var script = new Script();
			var name = "tom";
			int age = 16;
			Assert.AreEqual($"hello {name}, {{age + 10}}", script.Eval(s));
		}

		[TestMethod]
		public void Test11()
		{
			string s = "var name='tom'; { int age=16; $'hello {name}, {{age + 10}}' }";
			var script = new Script();
			var name = "tom";
			int age = 16;
			Assert.AreEqual($"hello {name}, {{age + 10}}", script.Eval(s));
		}

		[TestMethod]
		public void Test10_2()
		{
			string s = "var name='tom'; { int age=16; } $'hello {name}, {age+10}'";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			try
			{
				script.Eval(s);
			}
			catch (Exception ex)
			{
				Assert.AreEqual("variable age is not exists", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test10()
		{
			string s = "var name='tom'; { int age=16; } $'hello {name}, {age+10}'";
			var script = new Script();
			try
			{
				script.Eval(s);
			}
			catch (Exception ex)
			{
				Assert.AreEqual("variable age is not exists", ex.Message);
				return;
			}
			Assert.IsTrue(false);
		}

		[TestMethod]
		public void Test09_2()
		{
			string s = "var name='tom'; { int age=16; $'hello {name}, {age+10}' }";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var name = "tom";
			int age = 16;
			Assert.AreEqual($"hello {name}, {age + 10}", script.Eval(s));
		}

		[TestMethod]
		public void Test09()
		{
			string s = "var name='tom'; { int age=16; $'hello {name}, {age+10}' }";
			var script = new Script();
			var name = "tom";
			int age = 16;
			Assert.AreEqual($"hello {name}, {age + 10}", script.Eval(s));
		}

		[TestMethod]
		public void Test08_2()
		{
			string s = "$'hello tom'";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual($"hello tom", script.Eval(s));
		}

		[TestMethod]
		public void Test08()
		{
			string s = "$'hello tom'";
			var script = new Script();
			Assert.AreEqual($"hello tom", script.Eval(s));
		}

		[TestMethod]
		public void Test07_2()
		{
			string s = "var name='tom'; $'hello {name}, 5+8={5+8}'";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("hello tom, 5+8=13", script.Eval(s));
		}

		[TestMethod]
		public void Test07()
		{
			string s = "var name='tom'; $'hello {name}, 5+8={5+8}'";
			var script = new Script();
			Assert.AreEqual("hello tom, 5+8=13", script.Eval(s));
		}

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
				Assert.AreEqual("unknown string escape:\\e", ex.Message);
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
