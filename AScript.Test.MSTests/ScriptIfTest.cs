using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AScript.Nodes;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptIfTest
	{
		[TestMethod]
		public void Test20_4()
		{
			string s = @"
int n=30;
int m=0;
if (n>1)
	if (n>5)
		if (n>10)
			if (n>20) m=1;
m+=2;
m
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(3, script.Eval(s));
		}

		[TestMethod]
		public void Test20_3()
		{
			string s = @"
int n=30;
int m=0;
if (n>1)
	if (n>5)
		if (n>10)
			if (n>20) m=1;
m+=2;
m
";
			var script = new Script();
			Assert.AreEqual(3, script.Eval(s));
		}

		[TestMethod]
		public void Test20_2()
		{
			string s = @"
int n=20;
int m=0;
if (n>1)
	if (n>5)
		if (n>10)
			if (n>20) m=1;
m+=2;
m
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test20()
		{
			string s = @"
int n=20;
int m=0;
if (n>1)
	if (n>5)
		if (n>10)
			if (n>20) m=1;
m+=2;
m
";
			var script = new Script();
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test19_4()
		{
			int m = 1;
			int n = 10;
			if (n > 0)
			{
				if (n > 10) m = 2;
				else if (n > 20) m = 5;
				else m = 3;
			}
			else m = 4;
			Assert.AreEqual(3, m);

			string s = @"
int m = 1;
int n = 10;
if (n > 0) {
	if (n > 10) m = 2;
	else if (n > 20) m = 5;
	else m = 3;
}
else m = 4;
m+2;
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test19_3()
		{
			int m = 1;
			int n = 10;
			if (n > 20)
			{
				if (n > 10) m = 2;
				else if (n > 20) m = 5;
				else m = 3;
			}
			else m = 4;
			Assert.AreEqual(4, m);

			string s = @"
int m = 1;
int n = 10;
if (n > 20) {
	if (n > 10) m = 2;
	else if (n > 20) m = 5;
	else m = 3;
}
else m = 4;
m+2;
";
			var script = new Script();
			Assert.AreEqual(6, script.Eval(s));
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(6, script.Eval(s));
		}

		[TestMethod]
		public void Test19_2_2()
		{
			int m = 1;
			int n = 10;
			if (n > 20)
				if (n > 10) m = 2;
				else if (n > 20) m = 5;
				else m = 3;
			else m = 4;
			Assert.AreEqual(4, m);

			string s = @"
int m = 1;
int n = 10;
if (n > 20)
	if (n > 10) m = 2;
	else if (n > 20) m = 5;
	else m = 3;
else m = 4;
m+2
";
			var script = new Script();
			Assert.AreEqual(6, script.Eval(s));
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(6, script.Eval(s));
		}

		[TestMethod]
		public void Test19_2()
		{
			int m = 1;
			int n = 10;
			if (n > 20)
				if (n > 10) m = 2;
				else if (n > 20) m = 5;
				else m = 3;
			else m = 4;
			Assert.AreEqual(4, m);

			string s = @"
int m = 1;
int n = 10;
if (n > 20)
	if (n > 10) m = 2;
	else if (n > 20) m = 5;
	else m = 3;
else m = 4;
m+2;
";
			var script = new Script();
			Assert.AreEqual(6, script.Eval(s));
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(6, script.Eval(s));
		}

		[TestMethod]
		public void Test19_1_2()
		{
			int m = 1;
			int n = 10;
			if (n > 0)
				if (n > 10) m = 2;
				else if (n > 20) m = 5;
				else m = 3;
			else m = 4;
			Assert.AreEqual(3, m);

			string s = @"
int m = 1;
int n = 10;
if (n > 0)
	if (n > 10) m = 2;
	else if (n > 20) m = 5;
	else m = 3;
else m = 4;
m+2
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test19_1()
		{
			int m = 1;
			int n = 10;
			if (n > 0)
				if (n > 10) m = 2;
				else if (n > 20) m = 5;
				else m = 3;
			else m = 4;
			Assert.AreEqual(3, m);

			string s = @"
int m = 1;
int n = 10;
if (n > 0)
	if (n > 10) m = 2;
	else if (n > 20) m = 5;
	else m = 3;
else m = 4;
m+2;
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test18_5_4()
		{
			string s = @"
n=0;
if(n==0) n+=1;
n+2;
";
			var script = new Script();
			var treeNode = script.BuildNode(s);
			Assert.AreEqual(3, script.Eval(treeNode, out _));
			Assert.AreEqual(3, script.Eval(treeNode, out _));
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(3, script.Eval(treeNode, out _));
			Assert.AreEqual(1, script.Context.EvalVar("n"));
			Assert.AreEqual(1, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_5_3()
		{
			string s = @"
n=0;
if(n==0) n+=1;
n+2;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var treeNode = script.BuildNode(s);
			Assert.AreEqual(3, script.Eval(treeNode, out _));
			Assert.AreEqual(1, script.Context.EvalVar("n"));
			Assert.AreEqual(1, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_5_2()
		{
			string s = @"
n=0;
if(n==0) n+=1;
n+2;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(3, script.Eval(s));
			Assert.AreEqual(1, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_5()
		{
			string s = @"
n=0;
if(n==0) n+=1;
n+2;
";
			var script = new Script();
			Assert.AreEqual(3, script.Eval(s));
			Assert.AreEqual(1, script.Eval("n"));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(3, script.Eval(s));
			Assert.AreEqual(1, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_4()
		{
			string s = @"
n=0;
if(n==0) n+=1;
n+2
";
			var script = new Script();
			Assert.AreEqual(3, script.Eval(s));
			Assert.AreEqual(1, script.Eval("n"));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(3, script.Eval(s));
			Assert.AreEqual(1, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_3()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else if(n==0) n+=2;
else n+=10;
n;
";
			var script = new Script();
			Assert.AreEqual(2, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(2, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_2_4()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else if(n==0) n+=2;
n+2;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(4, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_2_3()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else if(n==0) n+=2;
else n+=10;
n+2;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(4, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_2_2()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else if(n==0) n+=2;
else n+=10;
n+2;
";
			var script = new Script();
			Assert.AreEqual(4, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(4, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));
		}

		[TestMethod]
		public void Test18_2()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else if(n==0) n+=2;
else n+=10;
n+2
";
			var script = new Script();
			Assert.AreEqual(4, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(4, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));
		}

		[TestMethod]
		public void Test18()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else if(n==0) n+=2;
else n+=10;
n
";
			var script = new Script();
			Assert.AreEqual(2, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(2, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));
		}

		[TestMethod]
		public void Test17_2()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else if(n==2) n+=2;
else n+=10;
n+2
";
			var script = new Script();
			Assert.AreEqual(12, script.Eval(s));
			Assert.AreEqual(10, script.Eval("n"));
		}

		[TestMethod]
		public void Test17()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else if(n==2) n+=2;
else n+=10;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
			Assert.AreEqual(10, script.Eval("n"));
		}

		[TestMethod]
		public void Test16()
		{
			string s = @"
n=1;
if(n==1) n+=1;
else n+=10;
";
			var script = new Script();
			Assert.AreEqual(2, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));
		}

		[TestMethod]
		public void Test15()
		{
			string s = @"
n=0;
if(n==1) n+=1;
else n+=10;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
			Assert.AreEqual(10, script.Eval("n"));
		}

		[TestMethod]
		public void Test14()
		{
			string s = @"
n=0;
n++;
if(++n==2) {
	n=4;
	m=(5+n)*2;
	return m+3
}
n=8;
n
";
			var script = new Script();
			Assert.AreEqual(21, script.Eval(s));
			Assert.AreEqual(4, script.Eval("n"));
		}

		[TestMethod]
		public void Test13()
		{
			string s = @"
n=0;
n++;
if(++n==2) return {
	n=4;
	m=(5+n)*2;
	m+3
}
n=8;
n
";
			var script = new Script();
			Assert.AreEqual(21, script.Eval(s));
			Assert.AreEqual(4, script.Eval("n"));
		}

		[TestMethod]
		public void Test12()
		{
			string s = @"
n=0;
n++;
if(++n==2) return(n=4;m=(5+n)*2;m+3);
n=8;
n
";
			var script = new Script();
			Assert.AreEqual(21, script.Eval(s));
			Assert.AreEqual(4, script.Eval("n"));
		}

		[TestMethod]
		public void Test11()
		{
			string s = @"
n=0;
n++;
if(++n==2) return{n=4;m=(5+n)*2;m+3}
n=8;
n
";
			var script = new Script();
			Assert.AreEqual(21, script.Eval(s));
			Assert.AreEqual(4, script.Eval("n"));
		}

		[TestMethod]
		public void Test10()
		{
			string s = @"
n=0;
n++;
if(++n==2) return(5+n)*2;
n=8;
n
";
			var script = new Script();
			Assert.AreEqual(14, script.Eval(s));
			Assert.AreEqual(2, script.Eval("n"));
		}

		[TestMethod]
		public void Test09()
		{
			string s = @"
n=0;
n++;
if(++n==2) return(5+n)*2;
n
";
			var script = new Script();
			Assert.AreEqual(14, script.Eval(s));
		}

		[TestMethod]
		public void Test08()
		{
			string s = @"
n=0;
n++;
if(++n==2) return(5+n);
n
";
			var script = new Script();
			Assert.AreEqual(7, script.Eval(s));
		}

		[TestMethod]
		public void Test07()
		{
			string s = @"
n=0;
n++;
if(++n==2) return 5+n;
n
";
			var script = new Script();
			Assert.AreEqual(7, script.Eval(s));
		}

		[TestMethod]
		public void Test06()
		{
			string s = @"
n=0;
n++;
if(++n==2) return 5;
n
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test05()
		{
			string s = @"
n=0;
n++;
if(++n==2) n=5;
n
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test04()
		{
			string s = @"
n=1;
n++;
if(n++==2) n=5;
n
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void Test03()
		{
			string s = @"
n=0;
n++;
if(n++==2) n=5;
n
";
			var script = new Script();
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test02()
		{
			string s = @"
n=0;
n++;
if(n==2) n=5;
n
";
			var script = new Script();
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
n=1;
n++;
if(n==2) n=5;
n
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));
		}
	}
}
