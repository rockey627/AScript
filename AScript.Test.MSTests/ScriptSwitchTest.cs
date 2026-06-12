//using System;
//using AScript.Nodes;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace AScript.Test.MSTests
//{
//	[TestClass]
//	public class ScriptSwitchTest
//	{
//		[TestMethod]
//		public void TestBasicSwitch_ReturnValue3()
//		{
//			string s = @"
//switch(3) {
//case 1:
//n=10;
//case 2:
//n=20;
//default:0
//}
//";
//			var script = new Script();
//			Assert.AreEqual(0, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(0, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestBasicSwitch_ReturnValue2()
//		{
//			string s = @"
//switch(2) {
//case 1:
//n=10;
//case 2:
//n=20;
//}
//";
//			var script = new Script();
//			Assert.AreEqual(20, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(20, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestBasicSwitch_ReturnValue()
//		{
//			string s = @"
//switch(1) {
//case 1:
//n=10;
//case 2:
//n=20;
//}
//";
//			var script = new Script();
//			Assert.AreEqual(10, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(10, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestBasicSwitch_Int_3()
//		{
//			string s = @"
//n=1;
//switch(n) {
//case 1:
//	int m=6;
//	n=m+4;
//case 2:
//	int m=8;
//	n=m+12;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(10, script.Eval(s));
//			Assert.AreEqual(false, ScriptUtils.IsVariableExists(null, script.Context, "m"));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(10, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestBasicSwitch_Int_2()
//		{
//			string s = @"
//n=1;
//switch(n) {
//case 1:
//{
//	int m=6;
//	n=m+4;
//}
//case 2:
//{
//	int m=8;
//	n=m+12;
//}
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(10, script.Eval(s));
//			Assert.AreEqual(false, ScriptUtils.IsVariableExists(null, script.Context, "m"));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(10, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestBasicSwitch_Int()
//		{
//			string s = @"
//n=1;
//switch(n) {
//case 1:
//n=10;
//case 2:
//n=20;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(10, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(10, script.Eval(s));
//		}

////		[TestMethod]
////		public void TestBasicSwitch_IntWithBreak()
////		{
////			string s = @"
////n=1;
////switch(n) {
////case 1:
////n=10;
////break;
////case 2:
////n=20;
////break;
////}
////n;
////";
////			var script = new Script();
////			Assert.AreEqual(10, script.Eval(s));

////			script.Options.CompileMode = ECompileMode.All;
////			Assert.AreEqual(10, script.Eval(s));
////		}

//		[TestMethod]
//		public void TestSwitch_WithDefault()
//		{
//			string s = @"
//n=5;
//switch(n) {
//case 1:
//n=10;
//case 2:
//n=20;
//default:
//n=100;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(100, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(100, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_NoDefault()
//		{
//			string s = @"
//n=5;
//switch(n) {
//case 1:
//n=10;
//case 2:
//n=20;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(5, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(5, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_FallThrough()
//		{
//			string s = @"
//n=1;
//switch(n) {
//case 1:
//case 2:
//n=50;
//case 3:
//n=30;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(50, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(50, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_MultipleCases()
//		{
//			string s = @"
//n=2;
//switch(n) {
//case 1:
//n+=1;
//case 2:
//n+=2;
//case 3:
//n+=3;
//default:
//n+=10;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(4, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(4, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_ReturnValue()
//		{
//			string s = @"
//n=2;
//switch(n) {
//case 1:
//n=10;
//case 2:
//n=20;
//}
//n+5;
//";
//			var script = new Script();
//			Assert.AreEqual(25, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(25, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_NestedInIf()
//		{
//			string s = @"
//n=1;
//m=0;
//if(n==1) {
//	switch(n) {
//	case 1:
//	m=100;
//	default:
//	m=200;
//	}
//}
//m;
//";
//			var script = new Script();
//			Assert.AreEqual(100, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(100, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_WithExpression()
//		{
//			string s = @"
//n=15;
//switch(n) {
//case 10:
//n=1;
//case 15:
//n=2;
//case 20:
//n=3;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(2, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(2, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_BreakInWhile()
//		{
//			string s = @"
//n=0;
//m=0;
//while(n<5) {
//n+=1;
//switch(n) {
//case 3:
//m=100;
//default:
//m+=10;
//}
//}
//m;
//";
//			var script = new Script();
//			Assert.AreEqual(120, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(120, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_CompileModeAll()
//		{
//			string s = @"
//n=1;
//switch(n) {
//case 1:
//n=10;
//case 2:
//n=20;
//default:
//n=30;
//}
//n;
//";
//			var script = new Script();
//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(10, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_DefaultOnly()
//		{
//			string s = @"
//n=999;
//switch(n) {
//default:
//n=1;
//}
//n;
//";
//			var script = new Script();
//			Assert.ThrowsException<Exceptions.ScriptAnalyzingException>(() => script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.ThrowsException<Exceptions.ScriptAnalyzingException>(() => script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_AllCasesNoMatch()
//		{
//			string s = @"
//n=100;
//switch(n) {
//case 1:
//n=10;
//case 2:
//n=20;
//case 3:
//n=30;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(100, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(100, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_ReturnValueWithDefault()
//		{
//			string s = @"
//n=5;
//switch(n) {
//case 1:
//n=10;
//default:
//n=99;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(99, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(99, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_NestedSwitch()
//		{
//			string s = @"
//n=1;
//m=0;
//switch(n) {
//case 1:
//switch(2) {
//case 2:
//m=50;
//}
//case 2:
//m=100;
//}
//m;
//";
//			var script = new Script();
//			Assert.AreEqual(50, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(50, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_InForLoop()
//		{
//			string s = @"
//m=0;
//for(i=1;i<=3;i+=1) {
//switch(i) {
//case 2:
//m+=10;
//default:
//m+=1;
//}
//}
//m;
//";
//			var script = new Script();
//			Assert.AreEqual(12, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(12, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_CaseWithCalculation()
//		{
//			string s = @"
//n=3;
//switch(n+1) {
//case 2:
//n=100;
//case 4:
//n=200;
//default:
//n=300;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(200, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(200, script.Eval(s));
//		}

//		[TestMethod]
//		public void TestSwitch_FirstMatchOnly()
//		{
//			string s = @"
//n=2;
//switch(n) {
//case 2:
//n=20;
//case 2:
//n=30;
//case 2:
//n=40;
//}
//n;
//";
//			var script = new Script();
//			Assert.AreEqual(20, script.Eval(s));

//			script.Options.CompileMode = ECompileMode.All;
//			Assert.AreEqual(20, script.Eval(s));
//		}
//	}
//}