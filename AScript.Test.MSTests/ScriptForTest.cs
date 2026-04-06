using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AScript.TokenHandlers;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptForTest
	{
		[TestMethod]
		public void Test11()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	if (i == 11) {return n+100;};
	n+=3;
}
n+10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(40, script.Eval(s));
			Assert.AreEqual(30, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test10_5()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	if (i == 5) return n+100;
	n+=3;
}
n+10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(115, script.Eval(s));
			Assert.AreEqual(15, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test10_4()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	if (i == 5) {return n+100;}
	n+=3;
}
n+10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(115, script.Eval(s));
			Assert.AreEqual(15, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test10_3()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	if (i == 5) {return n+100;};
	n+=3;
}
n+10
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(115, script.Eval(s));
			Assert.AreEqual(15, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test10_2()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	foreach(var a in list) {
		n+=a+i;
		if(a%2==0){n+=3;n+=2*5;continue}
		else if(a<4) {n+=2;break;}
		else return n+100;
		n++;
	}
}
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			bool isReturn = false;
			for (int i = 0; i < 10; i++)
			{
				foreach (var a in list)
				{
					n += a + i;
					if (a % 2 == 0) { n += 3; n += 2 * 5; continue; }
					else if (a < 4) { n += 2; break; }
					else
					{
						n = n + 100;
						isReturn = true;
						break;
					}
					n++;
				}
				if (isReturn) break;
			}
			Console.WriteLine(n);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test10_1()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	foreach(var a in list) {
		n+=a+i;
		if(a%2==0){n+=3;n+=2*5;继续}
		else if(a<4) {n+=2;中断;}
		else return n+100;
		n++;
	}
}
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			bool isReturn = false;
			for (int i = 0; i < 10; i++)
			{
				foreach (var a in list)
				{
					n += a + i;
					if (a % 2 == 0) { n += 3; n += 2 * 5; continue; }
					else if (a < 4) { n += 2; break; }
					else
					{
						n = n + 100;
						isReturn = true;
						break;
					}
					n++;
				}
				if (isReturn) break;
			}

			var script = new Script();
			script.Options.ThrowIfVariableNotExists = true;
			script.Context.AddTokenHandler("继续", ContinueTokenHandler.Instance);
			script.Context.AddTokenHandler("中断", BreakTokenHandler.Instance);
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test10()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	foreach(var a in list) {
		n+=a+i;
		if(a%2==0){n+=3;n+=2*5;continue}
		else if(a<4) {n+=2;break;}
		else return n+100;
		n++;
	}
}
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			bool isReturn = false;
			for (int i = 0; i < 10; i++)
			{
				foreach (var a in list)
				{
					n += a + i;
					if (a % 2 == 0) { n += 3; n += 2 * 5; continue; }
					else if (a < 4) { n += 2; break; }
					else
					{
						n = n + 100;
						isReturn = true;
						break;
					}
					n++;
				}
				if (isReturn) break;
			}

			var script = new Script();
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test09_2()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	foreach(var a in list) {
		n+=a+i;
		if(a%2==0){n+=3;n+=2*5;continue}
		else if(a<4) {n+=2;break;}
		else{n-=5}
		n++;
	}
}
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			for (int i = 0; i < 10; i++)
			{
				foreach (var a in list)
				{
					n += a + i;
					if (a % 2 == 0) { n += 3; n += 2 * 5; continue; }
					else if (a < 4) { n += 2; break; }
					else { n -= 5; }
					n++;
				}
			}

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test09()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	foreach(var a in list) {
		n+=a+i;
		if(a%2==0){n+=3;n+=2*5;continue}
		else if(a<4) {n+=2;break;}
		else{n-=5}
		n++;
	}
}
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			for (int i = 0; i < 10; i++)
			{
				foreach (var a in list)
				{
					n += a + i;
					if (a % 2 == 0) { n += 3; n += 2 * 5; continue; }
					else if (a < 4) { n += 2; break; }
					else { n -= 5; }
					n++;
				}
			}

			var script = new Script();
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test08_4()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++)
	foreach(var a in list) {
		n+=a+i;
		n++;
	};
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			for (int i = 0; i < 10; i++)
				foreach (var a in list)
				{
					n += a + i;
					n++;
				}

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test08_3()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++)
	foreach(var a in list) {
		n+=a+i;
		n++;
	}
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			for (int i = 0; i < 10; i++)
				foreach (var a in list)
				{
					n += a + i;
					n++;
				}

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test08_2()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	foreach(var a in list) {
		n+=a+i;
		n++;
	}
}
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			for (int i = 0; i < 10; i++)
			{
				foreach (var a in list)
				{
					n += a + i;
					n++;
				}
			}

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test08()
		{
			string s = @"
int n = 0;
for(int i =0;i<10;i++){
	foreach(var a in list) {
		n+=a+i;
		n++;
	}
}
n
";
			var list = new List<int> { 1, 2, 3, 4, 5 };
			int n = 0;
			for (int i = 0; i < 10; i++)
			{
				foreach (var a in list)
				{
					n += a + i;
					n++;
				}
			}

			var script = new Script();
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		/*
.Loop  {
    .If ($n < 10) {
        .Block() {
            .Block() {
                .If (
                    $n % 2 == 0
                ) {
					$m += 1;
                    .Continue #Label1 { }
                } .Else {
                    .Default(System.Void)
                };
                $m += 3
            };
            .Label
            .LabelTarget #Label1:;
            $n++
        }
    } .Else {
        .Break #Label2 { }
    }
}
.LabelTarget #Label2:
		 * */
		[TestMethod]
		public void Test07_3()
		{
			string s = @"
int m=0;
int n = 0;
for(;n<10;n++) {
	if(n%2==0) {
		m+=1;
		continue;
	}
	m+=3;
}
m+=4;
m+15";

			int m = 0;
			int n = 0;
			for (; n < 10; n++)
			{
				if (n % 2 == 0)
				{
					m += 1;
					continue;
				}
				m += 3;
			}
			m += 4;
			int r = m + 15;

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(r, script.Eval(s));
			Assert.AreEqual(m, script.Context.EvalVar("m"));
			Assert.AreEqual(n, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test07_2()
		{
			string s = @"
int m=0;
int n = 0;
for(;n<10;n++) {
	m+=2;
	if(n%2==0) {
		m+=1;
		continue;
	}
	m+=3;
}
m+=4;
m+15";

			int m = 0;
			int n = 0;
			for (; n < 10; n++)
			{
				m += 2;
				if (n % 2 == 0)
				{
					m += 1;
					continue;
				}
				m += 3;
			}
			m += 4;
			int r = m + 15;

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(r, script.Eval(s));
			Assert.AreEqual(m, script.Context.EvalVar("m"));
			Assert.AreEqual(n, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test07()
		{
			string s = @"
int m=0;
int n = 0;
for(;n<10;n++) {
	m+=2;
	if(n%2==0) {
		m+=1;
		continue;
	}
	m+=3;
}
m+=3;
m+15";

			int m = 0;
			int n = 0;
			for (; n < 10; n++)
			{
				m += 2;
				if (n % 2 == 0)
				{
					m += 1;
					continue;
				}
				m += 3;
			}
			m += 3;
			int r = m + 15;

			var script = new Script();
			Assert.AreEqual(r, script.Eval(s));
			Assert.AreEqual(m, script.Context.EvalVar("m"));
			Assert.AreEqual(n, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test06_2()
		{
			string s = "int m=0;int n=5;for(;n<10;) {m+=2;n++}m+=3;m+15";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(28, script.Eval(s));
			Assert.AreEqual(13, script.Context.EvalVar("m"));
			Assert.AreEqual(10, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test06()
		{
			string s = "int m=0;int n=5;for(;n<10;) {m+=2;n++}m+=3;m+15";
			var script = new Script();
			Assert.AreEqual(28, script.Eval(s));
			Assert.AreEqual(13, script.Context.EvalVar("m"));
			Assert.AreEqual(10, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test05_2()
		{
			string s = "int m=0;int n=5;for(;n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(28, script.Eval(s));
			Assert.AreEqual(13, script.Context.EvalVar("m"));
			Assert.AreEqual(10, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test05()
		{
			string s = "int m=0;int n=5;for(;n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			Assert.AreEqual(28, script.Eval(s));
			Assert.AreEqual(13, script.Context.EvalVar("m"));
			Assert.AreEqual(10, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test04_2()
		{
			string s = "int m=0;int n=5;for(;n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(28, script.Eval(s));
			Assert.AreEqual(13, script.Context.EvalVar("m"));
			Assert.AreEqual(10, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test04()
		{
			string s = "int m=0;int n=5;for(;n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			Assert.AreEqual(28, script.Eval(s));
			Assert.AreEqual(13, script.Context.EvalVar("m"));
			Assert.AreEqual(10, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test03_2()
		{
			string s = "int m=0;int a=0;for((int n=0;a=3); n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(38, script.Eval(s));
			Assert.AreEqual(23, script.Context.EvalVar("m"));
			Assert.AreEqual(null, script.Context.EvalVar("n"));
			Assert.AreEqual(3, script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test03()
		{
			string s = "int m=0;int a=0;for((int n=0;a=3); n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			Assert.AreEqual(38, script.Eval(s));
			Assert.AreEqual(23, script.Context.EvalVar("m"));
			Assert.AreEqual(null, script.Context.EvalVar("n"));
			Assert.AreEqual(3, script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test02_2()
		{
			string s = "int m=0;int n=5;for(n=0; n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(38, script.Eval(s));
			Assert.AreEqual(23, script.Context.EvalVar("m"));
			Assert.AreEqual(10, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test02()
		{
			string s = "int m=0;int n=5;for(n=0; n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			Assert.AreEqual(38, script.Eval(s));
			Assert.AreEqual(23, script.Context.EvalVar("m"));
			Assert.AreEqual(10, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = "int m=0;for(int n=0; n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(38, script.Eval(s));
			Assert.AreEqual(23, script.Context.EvalVar("m"));
			Assert.AreEqual(null, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test01()
		{
			string s = "int m=0;for(int n=0; n<10;n++) m+=2;m+=3;m+15";
			var script = new Script();
			Assert.AreEqual(38, script.Eval(s));
			Assert.AreEqual(23, script.Context.EvalVar("m"));
			Assert.AreEqual(null, script.Context.EvalVar("n"));
		}
	}
}
