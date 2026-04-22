using System;
using System.Collections.Generic;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptListTest
	{
		[TestMethod]
		public void Test12_2()
		{
			string s = @"
var list = new List<int>{0,0,0,0,0};
list[0]=15;
list[2]=18;
list[0]+list[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(33, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list.Count"));
			Assert.AreEqual(15, script.Eval("list[0]"));
			Assert.AreEqual(0, script.Eval("list[1]"));
			Assert.AreEqual(18, script.Eval("list[2]"));
			Assert.AreEqual(0, script.Eval("list[3]"));
			Assert.AreEqual(0, script.Eval("list[4]"));
		}

		[TestMethod]
		public void Test12()
		{
			string s = @"
var list = new List<int>{0,0,0,0,0};
list[0]=15;
list[2]=18;
list[0]+list[2]
";
			var script = new Script();
			Assert.AreEqual(33, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list.Count"));
			Assert.AreEqual(15, script.Eval("list[0]"));
			Assert.AreEqual(0, script.Eval("list[1]"));
			Assert.AreEqual(18, script.Eval("list[2]"));
			Assert.AreEqual(0, script.Eval("list[3]"));
			Assert.AreEqual(0, script.Eval("list[4]"));
		}

		[TestMethod]
		public void Test11_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddFunc<IEnumerable<int>, string>("ToString2", list => string.Join(',', list));
			script.Context.SetVar("list", new[] { 10, 20, 30, 40, 50, 60, 70 });
			Assert.AreEqual("20,30", script.Eval("list[1:3].ToString2()"));
		}

		[TestMethod]
		public void Test11()
		{
			var script = new Script();
			script.Context.AddFunc<IEnumerable<int>, string>("ToString2", list => string.Join(',', list));
			script.Context.SetVar("list", new[] { 10, 20, 30, 40, 50, 60, 70 });
			Assert.AreEqual("20,30", script.Eval("list[1:3].ToString2()"));
		}

		//		[TestMethod]
		//		public void Test10()
		//		{
		//			string s = @"
		//var list = new int[]{11,20,36,41,69,78,7};
		//var list2 = list.Where(a=>a%2==0).ToList();
		//list2.Count
		//";
		//			var script = new Script();
		//			Assert.AreEqual(3, script.Eval(s));
		//			Assert.AreEqual(20, script.Eval("list2[0]"));
		//			Assert.AreEqual(36, script.Eval("list2[1]"));
		//			Assert.AreEqual(78, script.Eval("list2[2]"));
		//		}

		[TestMethod]
		public void Test09_2()
		{
			string s = @"
var n = 4;
var list = new int[n+1]{10,20,30,40};
list[0]=15;
list[3-1]=18;
list[0]+list[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(33, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list.Length"));
			Assert.AreEqual(15, script.Eval("list[0]"));
			Assert.AreEqual(20, script.Eval("list[1]"));
			Assert.AreEqual(18, script.Eval("list[2]"));
			Assert.AreEqual(40, script.Eval("list[3]"));
			Assert.AreEqual(0, script.Eval("list[4]"));
		}

		[TestMethod]
		public void Test09()
		{
			string s = @"
var n = 4;
var list = new int[n+1]{10,20,30,40};
list[0]=15;
list[3-1]=18;
list[0]+list[2]
";
			var script = new Script();
			Assert.AreEqual(33, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list.Length"));
			Assert.AreEqual(15, script.Eval("list[0]"));
			Assert.AreEqual(20, script.Eval("list[1]"));
			Assert.AreEqual(18, script.Eval("list[2]"));
			Assert.AreEqual(40, script.Eval("list[3]"));
			Assert.AreEqual(0, script.Eval("list[4]"));
		}

		[TestMethod]
		public void Test08_2()
		{
			string s = @"
var n = 4;
var list = new int[n+1];
list[0]=15;
list[3-1]=18;
list[0]+list[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(33, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list.Length"));
			Assert.AreEqual(15, script.Eval("list[0]"));
			Assert.AreEqual(0, script.Eval("list[1]"));
			Assert.AreEqual(18, script.Eval("list[2]"));
			Assert.AreEqual(0, script.Eval("list[3]"));
			Assert.AreEqual(0, script.Eval("list[4]"));
		}

		[TestMethod]
		public void Test08()
		{
			string s = @"
var n = 4;
var list = new int[n+1];
list[0]=15;
list[3-1]=18;
list[0]+list[2]
";
			var script = new Script();
			Assert.AreEqual(33, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list.Length"));
			Assert.AreEqual(15, script.Eval("list[0]"));
			Assert.AreEqual(0, script.Eval("list[1]"));
			Assert.AreEqual(18, script.Eval("list[2]"));
			Assert.AreEqual(0, script.Eval("list[3]"));
			Assert.AreEqual(0, script.Eval("list[4]"));
		}

		[TestMethod]
		public void Test07_2()
		{
			string s = @"
var list = new int[5];
list[0]=15;
list[2]=18;
list[0]+list[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(33, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list.Length"));
			Assert.AreEqual(15, script.Eval("list[0]"));
			Assert.AreEqual(0, script.Eval("list[1]"));
			Assert.AreEqual(18, script.Eval("list[2]"));
			Assert.AreEqual(0, script.Eval("list[3]"));
			Assert.AreEqual(0, script.Eval("list[4]"));
		}

		[TestMethod]
		public void Test07()
		{
			string s = @"
var list = new int[5];
list[0]=15;
list[2]=18;
list[0]+list[2]
";
			var script = new Script();
			Assert.AreEqual(33, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list.Length"));
			Assert.AreEqual(15, script.Eval("list[0]"));
			Assert.AreEqual(0, script.Eval("list[1]"));
			Assert.AreEqual(18, script.Eval("list[2]"));
			Assert.AreEqual(0, script.Eval("list[3]"));
			Assert.AreEqual(0, script.Eval("list[4]"));
		}

		[TestMethod]
		public void Test06_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			string s = "var list = new int[]{10,20,30};list[1]";
			Assert.AreEqual(20, script.Eval(s));
			Assert.AreEqual(3, script.Eval("list.Length"));
		}

		[TestMethod]
		public void Test06()
		{
			var script = new Script();
			string s = "var list = new int[]{10,20,30};list[1]";
			Assert.AreEqual(20, script.Eval(s));
			Assert.AreEqual(3, script.Eval("list.Length"));
		}

		[TestMethod]
		public void Test05_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			string s = "var list = new List<int>();list.Add(5);list.Count";
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list[0]"));
		}

		[TestMethod]
		public void Test05()
		{
			var script = new Script();
			string s = "var list = new List<int>();list.Add(5);list.Count";
			Assert.AreEqual(1, script.Eval(s));
			Assert.AreEqual(5, script.Eval("list[0]"));
		}

		[TestMethod]
		public void Test04()
		{
			var script = new Script();
			script.Context.SetVar("list", new[] { 10, 20, 30 });
			Assert.AreEqual(5, script.Eval("list[1]=5"));
			Assert.AreEqual(15, script.Eval("list[1]=15", ECompileMode.All));
		}

		[TestMethod]
		public void Test03_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", new[] { 10, 20, 30 });
			Assert.AreEqual(5, script.Eval("list[1]=5;list[1]"));
			Assert.AreEqual(45, script.Eval("list[1]+list[0]+list[2]"));
		}

		[TestMethod]
		public void Test03()
		{
			var script = new Script();
			script.Context.SetVar("list", new[] { 10, 20, 30 });
			Assert.AreEqual(5, script.Eval("list[1]=5;list[1]"));
			Assert.AreEqual(45, script.Eval("list[1]+list[0]+list[2]"));
		}

		[TestMethod]
		public void Test02_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", new [] { 10, 20, 30 });
			Assert.AreEqual(20, script.Eval("list[1]"));
			Assert.AreEqual(60, script.Eval("list[1]+list[0]+list[2]"));
		}

		[TestMethod]
		public void Test02()
		{
			var script = new Script();
			script.Context.SetVar("list", new [] { 10, 20, 30 });
			Assert.AreEqual(20, script.Eval("list[1]"));
			Assert.AreEqual(60, script.Eval("list[1]+list[0]+list[2]"));
		}

		[TestMethod]
		public void Test01_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", new List<int> { 10, 20, 30 });
			Assert.AreEqual(20, script.Eval("list[1]"));
			Assert.AreEqual(60, script.Eval("list[1]+list[0]+list[2]"));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			script.Context.SetVar("list", new List<int> { 10, 20, 30 });
			Assert.AreEqual(20, script.Eval("list[1]"));
			Assert.AreEqual(60, script.Eval("list[1]+list[0]+list[2]"));
		}
	}
}
