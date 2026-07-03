using System;
using AScript.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptCaseWhenTest
	{
		[TestMethod]
		public void ReturnValue3()
		{
			string s = @"
case(3) {
when 1:
n=10;
when 2:
n=20;
default:0
}
";
			var script = new Script();
			Assert.AreEqual(0, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(0, script.Eval(s));
		}

		[TestMethod]
		public void ReturnValue2()
		{
			string s = @"
case(2) {
when 1:
n=10;
when 2:
n=20;
}
";
			var script = new Script();
			Assert.AreEqual(20, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(20, script.Eval(s));
		}

		[TestMethod]
		public void ReturnValue()
		{
			string s = @"
case(1) {
when 1:
n=10;
when 2:
n=20;
}
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Int_3()
		{
			string s = @"
n=1;
case(n) {
when 1:
	int m=6;
	n=m+4;
when 2:
	int m=8;
	n=m+12;
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
			Assert.AreEqual(false, ScriptUtils.IsVariableExists(null, script.Context, "m"));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Int_2()
		{
			string s = @"
n=1;
case(n) {
when 1:
{
	int m=6;
	n=m+4;
}
when 2:
{
	int m=8;
	n=m+12;
}
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
			Assert.AreEqual(false, ScriptUtils.IsVariableExists(null, script.Context, "m"));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Int()
		{
			string s = @"
n=1;
case(n) {
when 1:
n=10;
when 2:
n=20;
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		//		[TestMethod]
		//		public void TestBasicSwitch_IntWithBreak()
		//		{
		//			string s = @"
		//n=1;
		//case(n) {
		//when 1:
		//n=10;
		//break;
		//when 2:
		//n=20;
		//break;
		//}
		//n;
		//";
		//			var script = new Script();
		//			Assert.AreEqual(10, script.Eval(s));

		//			script.Options.CompileMode = ECompileMode.All;
		//			Assert.AreEqual(10, script.Eval(s));
		//		}

		[TestMethod]
		public void WithDefault()
		{
			string s = @"
n=5;
case(n) {
when 1:
n=10;
when 2:
n=20;
default:
n=100;
}
n;
";
			var script = new Script();
			Assert.AreEqual(100, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(100, script.Eval(s));
		}

		[TestMethod]
		public void NoDefault()
		{
			string s = @"
n=5;
case(n) {
when 1:
n=10;
when 2:
n=20;
}
n;
";
			var script = new Script();
			Assert.AreEqual(5, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(5, script.Eval(s));
		}

		[TestMethod]
		public void FallThrough()
		{
			string s = @"
n=1;
case(n) {
when 1:
when 2:
n=50;
when 3:
n=30;
}
n;
";
			var script = new Script();
			Assert.AreEqual(50, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(50, script.Eval(s));
		}

		[TestMethod]
		public void MultipleCases()
		{
			string s = @"
n=2;
case(n) {
when 1:
n+=1;
when 2:
n+=2;
when 3:
n+=3;
default:
n+=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(4, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(4, script.Eval(s));
		}

		[TestMethod]
		public void ReturnValue4()
		{
			string s = @"
n=2;
case(n) {
when 1:
n=10;
when 2:
n=20;
}
n+5;
";
			var script = new Script();
			Assert.AreEqual(25, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(25, script.Eval(s));
		}

		[TestMethod]
		public void NestedInIf()
		{
			string s = @"
n=1;
m=0;
if(n==1) {
	case(n) {
	when 1:
	m=100;
	default:
	m=200;
	}
}
m;
";
			var script = new Script();
			Assert.AreEqual(100, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(100, script.Eval(s));
		}

		[TestMethod]
		public void WithExpression()
		{
			string s = @"
n=15;
case(n) {
when 10:
n=1;
when 15:
n=2;
when 20:
n=3;
}
n;
";
			var script = new Script();
			Assert.AreEqual(2, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void BreakInWhile()
		{
			string s = @"
n=0;
m=0;
while(n<5) {
n+=1;
case(n) {
when 3:
m=100;
default:
m+=10;
}
}
m;
";
			var script = new Script();
			Assert.AreEqual(120, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(120, script.Eval(s));
		}

		[TestMethod]
		public void CompileModeAll()
		{
			string s = @"
n=1;
case(n) {
when 1:
n=10;
when 2:
n=20;
default:
n=30;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void DefaultOnly()
		{
			string s = @"
n=999;
case(n) {
default:
n=1;
}
n;
";
			var script = new Script();
			Assert.ThrowsException<Exceptions.ScriptAnalyzingException>(() => script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.ThrowsException<Exceptions.ScriptAnalyzingException>(() => script.Eval(s));
		}

		[TestMethod]
		public void AllCasesNoMatch()
		{
			string s = @"
n=100;
case(n) {
when 1:
n=10;
when 2:
n=20;
when 3:
n=30;
}
n;
";
			var script = new Script();
			Assert.AreEqual(100, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(100, script.Eval(s));
		}

		[TestMethod]
		public void ReturnValueWithDefault()
		{
			string s = @"
n=5;
case(n) {
when 1:
n=10;
default:
n=99;
}
n;
";
			var script = new Script();
			Assert.AreEqual(99, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(99, script.Eval(s));
		}

		[TestMethod]
		public void NestedSwitch()
		{
			string s = @"
n=1;
m=0;
case(n) {
when 1:
case(2) {
when 2:
m=50;
}
when 2:
m=100;
}
m;
";
			var script = new Script();
			Assert.AreEqual(50, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(50, script.Eval(s));
		}

		[TestMethod]
		public void InForLoop()
		{
			string s = @"
m=0;
for(i=1;i<=3;i+=1) {
case(i) {
when 2:
m+=10;
default:
m+=1;
}
}
m;
";
			var script = new Script();
			Assert.AreEqual(12, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(12, script.Eval(s));
		}

		[TestMethod]
		public void CaseWithCalculation()
		{
			string s = @"
n=3;
case(n+1) {
when 2:
n=100;
when 4:
n=200;
default:
n=300;
}
n;
";
			var script = new Script();
			Assert.AreEqual(200, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(200, script.Eval(s));
		}

		[TestMethod]
		public void FirstMatchOnly()
		{
			string s = @"
n=2;
case(n) {
when 2:
n=20;
when 2:
n=30;
when 2:
n=40;
}
n;
";
			var script = new Script();
			Assert.AreEqual(20, script.Eval(s));

			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(20, script.Eval(s));
		}
	}
}