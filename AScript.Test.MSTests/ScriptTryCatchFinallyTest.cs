using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptTryCatchFinallyTest
	{
		[TestMethod]
		public void Test01_TryCatchFinally_Basic()
		{
			string s = @"
int n=0;
try {
	n=1;
	throw new Exception(""error"");
	n=2;
} catch(Exception ex) {
	n=10;
} finally {
	n+=100;
}
n;
";
			var script = new Script();
			Assert.AreEqual(110, script.Eval(s));
			Assert.AreEqual(110, script.Eval("n"));
		}

		[TestMethod]
		public void Test01_TryCatchFinally_Basic_CompileAll()
		{
			string s = @"
int n=0;
try {
	n=1;
	throw new Exception(""error"");
	n=2;
} catch(Exception ex) {
	n=10;
} finally {
	n+=100;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(110, script.Eval(s));
			Assert.AreEqual(110, script.Eval("n"));
		}

		[TestMethod]
		public void Test02_TryCatch_CatchBlock()
		{
			string s = @"
int n=0;
try {
	n=1;
	throw new Exception(""error"");
} catch(Exception ex) {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test02_TryCatch_CatchBlock_CompileAll()
		{
			string s = @"
int n=0;
try {
	n=1;
	throw new Exception(""error"");
} catch(Exception ex) {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test03_TryFinally_NoCatch()
		{
			string s = @"
int n=0;
try {
	n=1;
} finally {
	n+=100;
}
n;
";
			var script = new Script();
			Assert.AreEqual(101, script.Eval(s));
		}

		[TestMethod]
		public void Test03_TryFinally_NoCatch_CompileAll()
		{
			string s = @"
int n=0;
try {
	n=1;
} finally {
	n+=100;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(101, script.Eval(s));
		}

		[TestMethod]
		public void Test04_TryNoThrow_CatchNotExecuted()
		{
			string s = @"
int n=0;
try {
	n=1;
} catch(Exception ex) {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test04_TryNoThrow_CatchNotExecuted_CompileAll()
		{
			string s = @"
int n=0;
try {
	n=1;
} catch(Exception ex) {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test05_CatchBracketsNoVar()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch() {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test05_CatchBracketsNoVar_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch() {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test06_CatchNoBrackets()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test06_CatchNoBrackets_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test07_CatchWithTypeOnly()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch(Exception) {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test07_CatchWithTypeOnly_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch(Exception) {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test08_CatchWithTypeAndVar()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch(Exception ex) {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test08_CatchWithTypeAndVar_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch(Exception ex) {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test09_MultipleCatch()
		{
			string s = @"
int n=0;
try {
	throw new ArgumentException(""error"");
} catch(ArgumentException) {
	n=1;
} catch(FormatException) {
	n=2;
} catch(Exception) {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test09_MultipleCatch_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new ArgumentException(""error"");
} catch(ArgumentException) {
	n=1;
} catch(FormatException) {
	n=2;
} catch(Exception) {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test10_MultipleCatch_SecondMatch()
		{
			string s = @"
int n=0;
try {
	throw new FormatException(""error"");
} catch(ArgumentException) {
	n=1;
} catch(FormatException) {
	n=2;
} catch(Exception) {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test10_MultipleCatch_SecondMatch_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new FormatException(""error"");
} catch(ArgumentException) {
	n=1;
} catch(FormatException) {
	n=2;
} catch(Exception) {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test11_MultipleCatch_LastMatch()
		{
			string s = @"
int n=0;
try {
	throw new InvalidOperationException(""error"");
} catch(ArgumentException) {
	n=1;
} catch(FormatException) {
	n=2;
} catch(Exception) {
	n=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test11_MultipleCatch_LastMatch_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new InvalidOperationException(""error"");
} catch(ArgumentException) {
	n=1;
} catch(FormatException) {
	n=2;
} catch(Exception) {
	n=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(10, script.Eval(s));
		}

		[TestMethod]
		public void Test12_CatchExceptionVar_AccessMessage()
		{
			string s = @"
string msg = """";
try {
	throw new Exception(""test error"");
} catch(Exception ex) {
	msg = ex.Message;
}
msg;
";
			var script = new Script();
			Assert.AreEqual("test error", script.Eval(s));
		}

		[TestMethod]
		public void Test12_CatchExceptionVar_AccessMessage_CompileAll()
		{
			string s = @"
string msg = """";
try {
	throw new Exception(""test error"");
} catch(Exception ex) {
	msg = ex.Message;
}
msg;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("test error", script.Eval(s));
		}

		[TestMethod]
		public void Test13_NestedTryCatch()
		{
			string s = @"
int n=0;
try {
	try {
		throw new Exception(""inner"");
	} catch {
		n=1;
	}
	throw new Exception(""outer"");
} catch {
	n+=10;
}
n;
";
			var script = new Script();
			Assert.AreEqual(11, script.Eval(s));
		}

		[TestMethod]
		public void Test13_NestedTryCatch_CompileAll()
		{
			string s = @"
int n=0;
try {
	try {
		throw new Exception(""inner"");
	} catch {
		n=1;
	}
	throw new Exception(""outer"");
} catch {
	n+=10;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(11, script.Eval(s));
		}

		[TestMethod]
		public void Test14_FinallyAlwaysExecutes_OnSuccess()
		{
			string s = @"
int n=0;
try {
	n=1;
} finally {
	n=100;
}
n;
";
			var script = new Script();
			Assert.AreEqual(100, script.Eval(s));
		}

		[TestMethod]
		public void Test14_FinallyAlwaysExecutes_OnSuccess_CompileAll()
		{
			string s = @"
int n=0;
try {
	n=1;
} finally {
	n=100;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(100, script.Eval(s));
		}

		[TestMethod]
		public void Test15_FinallyAlwaysExecutes_OnException()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch {
	n=1;
} finally {
	n+=100;
}
n;
";
			var script = new Script();
			Assert.AreEqual(101, script.Eval(s));
		}

		[TestMethod]
		public void Test15_FinallyAlwaysExecutes_OnException_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch {
	n=1;
} finally {
	n+=100;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(101, script.Eval(s));
		}

		[TestMethod]
		public void Test16_TryCatchFinally_ReturnInTry()
		{
			string s = @"
int n=0;
try {
	return 999;
} catch {
	n=10;
} finally {
	n=100;
}
n;
";
			var script = new Script();
			Assert.AreEqual(999, script.Eval(s));
		}

		[TestMethod]
		public void Test16_TryCatchFinally_ReturnInTry_CompileAll()
		{
			string s = @"
int n=0;
try {
	return 999;
} catch {
	n=10;
} finally {
	n=100;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(999, script.Eval(s));
		}

		[TestMethod]
		public void Test17_TryCatchFinally_ReturnInCatch()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch {
	n=10;
	return 999;
} finally {
	n=100;
}
n;
";
			var script = new Script();
			Assert.AreEqual(999, script.Eval(s));
		}

		[TestMethod]
		public void Test17_TryCatchFinally_ReturnInCatch_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new Exception(""error"");
} catch {
	n=10;
	return 999;
} finally {
	n=100;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(999, script.Eval(s));
		}

		[TestMethod]
		public void Test18_CatchSpecificType()
		{
			string s = @"
int n=0;
try {
	throw new ArgumentException(""error"");
} catch(ArgumentException ex) {
	n=1;
} catch(Exception) {
	n=2;
}
n;
";
			var script = new Script();
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test18_CatchSpecificType_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new ArgumentException(""error"");
} catch(ArgumentException ex) {
	n=1;
} catch(Exception) {
	n=2;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test19_ExceptionTypeHierarchy()
		{
			string s = @"
int n=0;
try {
	throw new ArgumentNullException(""error"");
} catch(ArgumentException) {
	n=1;
} catch(Exception) {
	n=2;
}
n;
";
			var script = new Script();
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test19_ExceptionTypeHierarchy_CompileAll()
		{
			string s = @"
int n=0;
try {
	throw new ArgumentNullException(""error"");
} catch(ArgumentException) {
	n=1;
} catch(Exception) {
	n=2;
}
n;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(1, script.Eval(s));
		}

		[TestMethod]
		public void Test20_TryCatchInLoop()
		{
			int total = 0;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					if (i == 1) throw new Exception("error");
					total += 1;
				}
				catch
				{
					total += 10;
				}
			}
			Assert.AreEqual(12, total);

			string s = @"
int total=0;
for(int i=0;i<3;i++) {
	try {
		if(i==1) throw new Exception(""error"");
		total+=1;
	} catch {
		total+=10;
	}
}
total;
";
			var script = new Script();
			Assert.AreEqual(12, script.Eval(s));
		}

		[TestMethod]
		public void Test20_TryCatchInLoop_CompileAll()
		{
			string s = @"
int total=0;
for(int i=0;i<3;i++) {
	try {
		if(i==1) throw new Exception(""error"");
		total+=1;
	} catch {
		total+=10;
	}
}
total;
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(12, script.Eval(s));
		}
	}
}
