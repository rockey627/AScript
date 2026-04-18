namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptMathTest
	{
		[TestMethod]
		public void Test14()
		{
			var script = new Script();
			Assert.AreEqual(-5.0, script.Eval("Math.Floor(-9.0/2)"));
		}

		[TestMethod]
		public void Test13_Power()
		{
			var script = new Script();
			Assert.AreEqual(8, script.Eval("2**3"));
		}

		[TestMethod]
		public void Test12()
		{
			var script = new Script();
			var n = 1.0;
			Assert.AreEqual(++n, script.Eval("n=1.0;++n"));
			Assert.AreEqual(n++, script.Eval("n++"));
		}

		[TestMethod]
		public void Test11()
		{
			var script = new Script();
			int n = 5;
			Assert.AreEqual(n == +5, script.Eval("n=5;n==+5"));
			Assert.AreEqual(true, script.Eval("n++==+5"));
		}

		[TestMethod]
		public void Test10()
		{
			var script = new Script();
			Assert.AreEqual(5 + 2, script.Eval(File.OpenRead("DefaultLexicalAnalyzerTest_Test12.txt")));
		}

		[TestMethod]
		public void Test09()
		{
			var script = new Script();
			Assert.AreEqual(7, script.Eval("n=4;m=3;m+=n"));
			Assert.AreEqual(8, script.Eval("n=5;m=3;m+=n;m;"));
			Assert.AreEqual(-2, script.Eval("n=5;m=3;m-=n"));
			Assert.AreEqual(5, script.Context.EvalVar("n", out var type));
			Assert.AreEqual(typeof(int), type);
		}

		[TestMethod]
		public void Test08()
		{
			var script = new Script();
			Assert.AreEqual(72, script.Eval("n=5;n+67"));
			Assert.AreEqual(5, script.Context.EvalVar("n", out var type));
			Assert.AreEqual(typeof(int), type);
		}

		[TestMethod]
		public void Test07()
		{
			var script = new Script();
			Assert.AreEqual(5, script.Eval("n=5"));
			Assert.AreEqual(5, script.Context.EvalVar("n", out var type));
			Assert.AreEqual(typeof(int), type);
		}

		[TestMethod]
		public void Test06()
		{
			var script = new Script();
			Assert.AreEqual(15, script.Eval("+5+10"));
			Assert.AreEqual(4, script.Eval("-6+10"));
			Assert.AreEqual(5 * -3 + 10, script.Eval("5* -3+10"));
			Assert.AreEqual(5 * -3 + 10, script.Eval("5*-3+10"));
			Assert.AreEqual(5 + -3 + 10, script.Eval("5+ -3+10"));
			Assert.AreEqual(5 + -3 + 10, script.Eval("5+-3+10"));
			Assert.AreEqual(5 - -3 + 10, script.Eval("5- -3+10"));
			int n = 0;
			try
			{
				Console.WriteLine(script.Eval("5--3+10"));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				n += 1;
			}
			Assert.AreEqual(1, n);
		}

		[TestMethod]
		public void Test05()
		{
			var script = new Script();
			Assert.IsTrue((bool)script.Eval("10>5"));
			Assert.AreEqual(10.0 > 5, script.Eval("10.0>5"));
			Assert.IsTrue((bool)script.Eval("10>5.0"));
			Assert.IsTrue((bool)script.Eval("10>=5"));
			Assert.IsFalse((bool)script.Eval("10<5"));
			Assert.AreEqual(10 <= 5, script.Eval("10<=5"));
			Assert.AreEqual(10 == 5, script.Eval("10==5"));
			Assert.AreEqual(10 == 10, script.Eval("10==10"));
			Assert.IsTrue((bool)script.Eval("!(10<5)"));
			Assert.IsFalse((bool)script.Eval("! !(10<5)"));
			Assert.IsFalse((bool)script.Eval("!!(10<5)"));
		}

		[TestMethod]
		public void Test04()
		{
			var script = new Script();
			script.Context.SetVar("m", 6);
			var result = script.Eval("n=8;n+m+10*(3+0x0A)");
			Assert.AreEqual(8 + 6 + 10 * (3 + 0x0A), result);
		}

		[TestMethod]
		public void Test03()
		{
			string s = "m1+n2+10";
			var script = new Script();
			script.Context.SetVar("m1", 5);
			script.Context.SetVar("n2", 4);
			Assert.AreEqual(19, script.Eval(s));
		}

		[TestMethod]
		public void Test02_2()
		{
			Test02(-1);
		}

		[TestMethod]
		public void Test02_1()
		{
			Test02(0);
		}

		private void Test02(int cache)
		{
			var script = new Script();
			Assert.AreEqual("510", script.Eval("5+'10'", cache));
			Assert.AreEqual(15, script.Eval("5+10", cache));
			Assert.AreEqual(15D, script.Eval("5+10D", cache));
			Assert.AreEqual(15F, script.Eval("5+10F", cache));
			Assert.AreEqual(15M, script.Eval("5+10M", cache));
			Assert.AreEqual(18, script.Eval("5+10+3", cache));
			Assert.AreEqual(12, script.Eval("5+10-3", cache));
			Assert.AreEqual(5 + 10 - 3.2, script.Eval("5+10-3.2", cache));
			Assert.AreEqual(-2, script.Eval("5-10+3", cache));
			Assert.AreEqual(-8, script.Eval("5-(10+3)", cache));
			Assert.AreEqual(35, script.Eval("5+10*3", cache));
			Assert.AreEqual(5 + 10.7 * 3, script.Eval("5+10.7*3", cache));
			Assert.AreEqual(5 / 12, script.Eval("5/12", cache));
			Assert.AreEqual(12 / 5, script.Eval("12/5", cache));
			Assert.AreEqual(12 / 5.0, script.Eval("12/5.0", cache));
			Assert.AreEqual(12 / 5.3, script.Eval("12/5.3", cache));
			Assert.AreEqual(12 / 5D, script.Eval("12/5D", cache));
			Assert.AreEqual(12 / 5f, script.Eval("12/5f", cache));
			Assert.AreEqual(12 / 5m, script.Eval("12/5m", cache));
			Assert.AreEqual(5 % 12, script.Eval("5%12", cache));
			Assert.AreEqual(5.6 % 12, script.Eval("5.6%12", cache));
			Assert.AreEqual(12 % 5, script.Eval("12%5", cache));
			Assert.AreEqual(12 & 5, script.Eval("12&5", cache));
			Assert.AreEqual(12 | 5, script.Eval("12|5", cache));
			Assert.AreEqual(12 ^ 5, script.Eval("12^5", cache));
			Assert.AreEqual(12 ^ 5 | 3 & 6 % 3 / 2, script.Eval("12 ^ 5 | 3 & 6 % 3 / 2", cache));
			Assert.AreEqual(~12, script.Eval("~12", cache));
			Assert.AreEqual(!(12 == 5), script.Eval("!(12==5)", cache));
			Assert.IsTrue(12 == 12D);
			Assert.IsTrue(12 == 12.0);
			Assert.IsTrue(12F == 12.0);
			Assert.AreEqual(!(12 == 5.6), script.Eval("!(12==5.6)", cache));
			Assert.AreEqual(!(12 == 12), script.Eval("!(12==12)", cache));
			Assert.AreEqual(!(12 == 12D), script.Eval("!(12==12D)", cache));
			Assert.AreEqual(12F == 12D, script.Eval("12F==12D", cache));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			Assert.AreEqual(5, script.Eval("5"));
			Assert.AreEqual(5D, script.Eval("5d"));
			Assert.AreEqual(5D, script.Eval("5D"));
			Assert.AreEqual(5f, script.Eval("5f"));
			Assert.AreEqual(5F, script.Eval("5F"));
			Assert.AreEqual(5m, script.Eval("5m"));
			Assert.AreEqual(5M, script.Eval("5M"));
			Assert.AreEqual(5.0, script.Eval("5.0"));
			Assert.AreEqual(0x0a, script.Eval("0x0A"));
			Assert.AreEqual(0x0b010a0a0a, script.Eval("0x0b010a0a0a"));
			Assert.AreEqual(076, script.Eval("076"));
			Assert.AreEqual(.9, script.Eval(".9"));
		}
	}
}