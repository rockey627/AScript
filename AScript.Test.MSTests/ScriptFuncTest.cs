using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptFuncTest
	{
		[TestMethod]
		public void Test20_2()
		{
			string s = @"
int n = 10;
int exec() {
	int t = 0;
	int i = 0;
	int func() => t += i+n;
	for(i = 0; i<n; i++) {
		func();
	}
	t;
}
exec();
";
			int n = 10;
			var exec = () =>
			{
				int t = 0;
				int i = 0;
				var func = () => t += i + n;
				for (i = 0; i < n; i++)
				{
					func();
				}
				return t;
			};
			var r = exec();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(r, script.Eval(s));
			var t = script.Context.EvalVar("t", out var type);
			Assert.IsNull(t);
			Assert.IsNull(type);
			var func = script.Context.GetFunc("func");
			Assert.IsNull(func);
			try
			{
				script.Eval("func()");
				Assert.IsTrue(false);
			}
			catch (Exception ex)
			{
				//Assert.AreEqual("unknown function: func()", ex.Message);
				Assert.AreEqual("Exception has been thrown by the target of an invocation.", ex.Message);
			}
		}

		[TestMethod]
		public void Test20()
		{
			string s = @"
int n = 10;
int exec() {
	int t = 0;
	int i = 0;
	int func() => t += i+n;
	for(i = 0; i<n; i++) {
		func();
	}
	t;
}
exec();
";
			int n = 10;
			var exec = () =>
			{
				int t = 0;
				int i = 0;
				var func = () => t += i + n;
				for (i = 0; i < n; i++)
				{
					func();
				}
				return t;
			};
			var r = exec();
			var script = new Script();
			Assert.AreEqual(r, script.Eval(s));
			var t = script.Context.EvalVar("t", out var type);
			Assert.IsNull(t);
			Assert.IsNull(type);
			try
			{
				script.Eval("func()");
				Assert.IsTrue(false);
			}
			catch (Exception ex)
			{
				Assert.AreEqual("unknown function: func()", ex.Message);
			}
		}

		[TestMethod]
		public void Test19_2()
		{
			string s = @"
int m;
int sum(int a, int b)=>a+b+n;
{
	int n = 11;
	m = sum(1,2)
}
int n = 10;
sum(1,2)
";
			{
				var script = new Script();
				script.Options.ThrowIfVariableNotExists = true;
				script.Options.CompileMode = ECompileMode.All;
				try
				{
					Assert.AreEqual(13, script.Eval(s));
					Assert.IsTrue(false);
				}
				catch (Exception ex)
				{
					Assert.AreEqual("variable n is not exists", ex.Message);
				}
			}
		}

		[TestMethod]
		public void Test19()
		{
			string s = @"
int m;
int sum(int a, int b)=>a+b+n;
{
	int n = 11;
	m = sum(1,2)
}
int n = 10;
sum(1,2)
";
			{
				var script = new Script();
				Assert.AreEqual(13, script.Eval(s));
				Assert.AreEqual(14, script.Eval("m"));
			}
		}

		[TestMethod]
		public void Test18_2()
		{
			string s = @"
int n = 10;
int sum(int a, int b)=>a+b+n;
{
	int n = 20;
	sum(1,2)
}
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(13, script.Eval(s));
		}

		[TestMethod]
		public void Test18()
		{
			string s = @"
int n = 10;
int sum(int a, int b)=>a+b+n;
{
	int n = 11;
	sum(1,2)
}
";
			var script = new Script();
			Assert.AreEqual(14, script.Eval(s));
		}

		[TestMethod]
		public void Test17_2()
		{
			string s = @"
int n = 10;
int sum(int a, int b)=>a+b+n;
n=11;
sum(1,2)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(14, script.Eval(s));
		}

		[TestMethod]
		public void Test17()
		{
			string s = @"
int n = 10;
int sum(int a, int b)=>a+b+n;
n=11;
sum(1,2)
";
			var script = new Script();
			Assert.AreEqual(14, script.Eval(s));
		}

		[TestMethod]
		public void Test16_2()
		{
			string s = "test(int a,int b){ return a+b;}";

			{
				var script = new Script();
				var obj = script.Eval(s);
				Assert.IsNotNull(obj);
				Assert.IsInstanceOfType(obj, typeof(IFunctionObject));
				Assert.AreEqual(8, ((IFunctionObject)obj).DynamicInvoke(3, 5));
			}
			{
				var script = new Script();
				var obj = script.Eval(s, ECompileMode.All);
				Assert.IsNotNull(obj);
				Assert.IsInstanceOfType(obj, typeof(Func<int, int, int>));
			}
			{
				var script = new Script();
				var obj = script.Eval(s, -1);
				Assert.IsNotNull(obj);
				Assert.IsInstanceOfType(obj, typeof(Func<int, int, int>));
			}
		}

		[TestMethod]
		public void Test16()
		{
			string s = "int test(int a,int b){ return a+b;}";

			{
				var script = new Script();
				var obj = script.Eval(s);
				Assert.IsNotNull(obj);
				//Assert.IsInstanceOfType(obj, typeof(Func<int, int, int>));
				Assert.IsInstanceOfType(obj, typeof(IFunctionObject));
				Assert.AreEqual(8, ((IFunctionObject)obj).DynamicInvoke(3, 5));
			}
			{
				var script = new Script();
				var obj = script.Eval(s, ECompileMode.All);
				Assert.IsNotNull(obj);
				Assert.IsInstanceOfType(obj, typeof(Func<int, int, int>));
			}
			{
				var script = new Script();
				var obj = script.Eval(s, -1);
				Assert.IsNotNull(obj);
				Assert.IsInstanceOfType(obj, typeof(Func<int, int, int>));
			}
		}

		[TestMethod]
		public void Test15()
		{
			string s = "test()=>{ return 10;}100 * test() * (6-2)";
			int r = 100 * 10 * (6 - 2);

			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, ECompileMode.All));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, -1));
			}
		}

		[TestMethod]
		public void Test14()
		{
			string s = "int test(){ return 10;}100 * test() * (6-2)";
			int r = 100 * 10 * (6 - 2);

			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, ECompileMode.All));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, -1));
			}
		}

		[TestMethod]
		public void Test13_3()
		{
			string s = "test(int a,int b){ return a+b;}100 * test(5,5) * (6-2)";
			int r = 100 * 10 * (6 - 2);

			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, ECompileMode.All));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, -1));
			}
		}

		[TestMethod]
		public void Test13_2()
		{
			string s = "test(int a,int b)=>{ return a+b;}100 * test(5,5) * (6-2)";
			int r = 100 * 10 * (6 - 2);

			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, ECompileMode.All));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, -1));
			}
		}

		[TestMethod]
		public void Test13_1()
		{
			string s = "int test(int a,int b){ return a+b;}100 * test(5,5) * (6-2)";
			int r = 100 * 10 * (6 - 2);

			var script = new Script();
			Assert.AreEqual(r, script.Eval<int>(s, ECompileMode.All));
		}

		[TestMethod]
		public void Test13()
		{
			string s = "int test(int a,int b){ return a+b;}100 * test(5,5) * (6-2)";
			int r = 100 * 10 * (6 - 2);

			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s));
			}
			{
				var script = new Script();
				Assert.AreEqual(r, script.Eval<int>(s, -1));
			}
		}

		[TestMethod]
		public void Test12()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (x, y) => x + y);
			var result = script.Eval("int exec(int a)=>sum(a,10);exec(5)", out var type);
			Assert.AreEqual(15, result);
		}

		[TestMethod]
		public void Test11()
		{
			var script = new Script();
			var result = script.Eval("int exec(int a)=>sum(a,10);int sum(int a, int b) => a+b;exec(5)", out var type);
			Assert.AreEqual(15, result);
		}

		[TestMethod]
		public void Test10_6()
		{
			string s = @"
Console.WriteLine(n);
int sum(int a, int b) =>{ 
	//Console.WriteLine(n);
	a+b+n
}
sum(1,2)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("n", 10);
			var result = script.Eval(s);
			Assert.AreEqual(13, result);
		}

		[TestMethod]
		public void Test10_5()
		{
			string s = @"
int n=10;
Console.WriteLine(n);
int sum(int a, int b) =>{ 
	//Console.WriteLine(n);
	a+b+n
}
sum(1,2)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = script.Eval(s);
			Assert.AreEqual(13, result);
		}

		[TestMethod]
		public void Test10_4()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = script.Eval("int n=10;int sum(int a, int b) =>{ int m=5; a+b+m+n};sum(1,2)");
			Assert.AreEqual(18, result);
		}

		[TestMethod]
		public void Test10_3()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = script.Eval("int n=10;int sum(int a, int b) =>{ int m=5; a+b+m+n}", out var type);
			Assert.IsNotNull(result);
			Assert.AreEqual(typeof(Func<int, int, int>), result.GetType());
			Assert.AreEqual(typeof(Func<int, int, int>), type);
			Assert.AreEqual(23, ((Func<int, int, int>)result)(3, 5));
		}

		[TestMethod]
		public void Test10_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.Function;
			var result = script.Eval("int sum(int a, int b) => a+b", out var type);
			Assert.IsNotNull(result);
			Assert.AreEqual(typeof(Func<int, int, int>), result.GetType());
			Assert.AreEqual(typeof(Func<int, int, int>), type);
			Assert.AreEqual(8, ((Func<int, int, int>)result)(3, 5));
			Assert.IsInstanceOfType(result, typeof(Delegate));
			Assert.AreEqual(8, ((Delegate)result).DynamicInvoke(3, 5));
		}

		[TestMethod]
		public void Test10()
		{
			var script = new Script();
			var result = script.Eval("int sum(int a, int b) => a+b", out var type);
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(IFunctionObject));
			Assert.AreEqual(8, ((IFunctionObject)result).DynamicInvoke(3, 5));
		}

		[TestMethod]
		public void Test09()
		{
			var script = new Script();
			script.Context.AddFunc<ScriptContext, int, int>("test", (context, a) => a + 2 + (int)context.EvalVar("n"));
			Assert.AreEqual(8, script.Eval("{n=5;test(1)}"));
			Assert.AreEqual(null, script.Context.EvalVar("n"));
		}

		[TestMethod]
		public void Test08_2()
		{
			var script = new Script();
			string s = @"
int exec(int a, int b) {
	var n=mult(a,10);
	int m=10;
	return n+b;
}
int mult(int a, int b)=>sum(a,15)*b;
int sum(int a, int b)=>a+b;
int sum(int a, int b, int c)=>a+b+c;
sum(1,2)+8+sum(1,2,3)+mult(5,6)
";
			var result = script.Eval(s);
			Assert.AreEqual(1 + 2 + 8 + 1 + 2 + 3 + (5 + 15) * 6, result);
			Assert.AreEqual(1 + 2 + 8, script.Eval("sum(1,2)+8"));
			Assert.AreEqual((5 + 15) * 10 + 2 + 8, script.Eval("exec(5,2)+8"));
		}

		[TestMethod]
		public void Test08()
		{
			var script = new Script();
			string s = @"
int exec(int a, int b) {
	var n=mult(a,10);
	n+b;
}
int mult(int a, int b)=>sum(a,15)*b;
int sum(int a, int b)=>a+b;
int sum(int a, int b, int c)=>a+b+c;
sum(1,2)+8+sum(1,2,3)+mult(5,6)
";
			var result = script.Eval(s);
			Assert.AreEqual(1 + 2 + 8 + 1 + 2 + 3 + (5 + 15) * 6, result);
			Assert.AreEqual(1 + 2 + 8, script.Eval("sum(1,2)+8"));
			Assert.AreEqual((5 + 15) * 10 + 2 + 8, script.Eval("exec(5,2)+8"));
		}

		[TestMethod]
		public void Test07()
		{
			var script = new Script();
			string s = @"
int exec(int a, int b) {
	var n=mult(a,10);
	n+b;
}
// 2个数相加
int sum(int a, int b)=>a+b;
// 重载函数，3个数相加
int sum(int a, int b, int c)=>a+b+c;
// 乘法
int mult(int a, int b)=>a*b;
/* 
调用函数计算结果：
1 + 2 + 8 + 1 + 2 + 3 + 5 * 6
*/
sum(1,2)+8+sum(1,2,3)+mult(5,6)
";
			var result = script.Eval(s);
			Assert.AreEqual(1 + 2 + 8 + 1 + 2 + 3 + 5 * 6, result);
			Assert.AreEqual(1 + 2 + 8, script.Eval("sum(1,2)+8"));
			Assert.AreEqual(5 * 10 + 2 + 8, script.Eval("exec(5,2)+8"));
		}

		[TestMethod]
		public void Test06()
		{
			var script = new Script();
			var result = script.Eval("int sum(int a, int b)=>a+b; sum(1,2)+10");
			Assert.AreEqual(1 + 2 + 10, result);
			Assert.AreEqual(1 + 2 + 8, script.Eval("sum(1,2)+8"));
			Assert.AreEqual(1 + 5 + 20 + 8, script.Eval("sum(1,sum(5, 20))+8"));
		}

		[TestMethod]
		public void Test05()
		{
			var script = new Script();
			var result = script.Eval("int sum(int a, int b) { a+b } sum(1,2)+10");
			Assert.AreEqual(13, result);
			Assert.AreEqual(11, script.Eval("sum(1,2)+8"));
		}

		[TestMethod]
		public void Test04_3()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Options.RewriteFunctions = false;
			var result = script.Eval("int sum(int a, int b) { a+b }");
			Assert.IsNotNull(result);
			var sum = script.Context.GetFunc<int, int, int>("sum");
			Assert.IsNull(sum);
		}

		[TestMethod]
		public void Test04_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var result = script.Eval("int sum(int a, int b) { a+b }");
			Assert.IsNotNull(result);
			Assert.AreEqual(11, script.Eval("sum(1,2)+8"));
			var sum = script.Context.GetFunc<int, int, int>("sum");
			Assert.IsNotNull(sum);
			Assert.AreEqual(17, sum(8, 9));
		}

		[TestMethod]
		public void Test04()
		{
			var script = new Script();
			var result = script.Eval("int sum(int a, int b) { a+b }");
			Assert.IsNotNull(result);
			Assert.AreEqual(11, script.Eval("sum(1,2)+8"));
		}

		[TestMethod]
		public void Test03()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			script.Context.AddFunc<int, int, int, int>("sum", (a, b, c) => a + b + c);
			script.Context.AddFunc<int, int, int>("mult", (a, b) => a * b);
			Assert.AreEqual(11, script.Eval("sum(1,2)+8"));
			Assert.AreEqual(1 + 2 + 8 + 1 + 2 + 3 + 5 * 6, script.Eval("sum(1,2)+8+sum(1,2,3)+mult(5,6)"));
			Assert.AreEqual(24, script.Eval("13+sum(1,2)+8"));
			Assert.AreEqual(30, script.Eval("sum(sum(3,sum(6,7+4)),2)+8"));
			Assert.AreEqual(41, script.Eval("n=13+sum(1,2)+8;n+sum(5,2*6)"));
		}

		[TestMethod]
		public void Test02()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			Assert.AreEqual(11, script.Eval("sum(1,2)+8"));
			Assert.AreEqual(24, script.Eval("13+sum(1,2)+8"));
			Assert.AreEqual(30, script.Eval("sum(sum(3,sum(6,7+4)),2)+8"));
			Assert.AreEqual(41, script.Eval("n=13+sum(1,2)+8;n+sum(5,2*6)"));
		}

		[TestMethod]
		public void Test01()
		{
			var script = new Script();
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			Assert.AreEqual(3, script.Eval("sum(1,2)"));
		}
	}
}
