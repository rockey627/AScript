using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ExpressionTest
	{
		[TestMethod]
		public void Test16_3()
		{
			string s = "a+=2;a+3";
			var script = new Script();
			script.Context.SetVar("a", 1);
			var d = script.CompileGlobal(s);
			var result = d.DynamicInvoke(script.Context);
			Assert.AreEqual(6, result);
			Assert.AreEqual(3, script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test16_2()
		{
			var v = Expression.Variable(typeof(object));
			var assignExpr = Expression.Assign(v, Expression.Constant(1, typeof(object)));
			// dynamic方式使用+=无效
			//var addAssignExpr = Expression.Dynamic(ExpressionUtils.Binder_AddAssign, typeof(object), v, Expression.Constant(2));
			var addExpr1 = Expression.Dynamic(ExpressionUtils.Binder_Add, typeof(object), v, Expression.Constant(2));
			var assignExpr2 = Expression.Assign(v, addExpr1);
			var addExpr = Expression.Dynamic(ExpressionUtils.Binder_Add, typeof(object), v, Expression.Constant(3));
			var block = Expression.Block(new[] { v }, assignExpr, assignExpr2, addExpr);
			var lambda = Expression.Lambda(block);
			var f = lambda.Compile();
			var result = f.DynamicInvoke();
			Assert.AreEqual(6, result);
		}

		[TestMethod]
		public void Test16_1()
		{
			var v = Expression.Variable(typeof(int));
			var assignExpr = Expression.Assign(v, Expression.Constant(1));
			var addAssignExpr = Expression.AddAssign(v, Expression.Constant(2));
			var addExpr = Expression.Add(v, Expression.Constant(3));
			var block = Expression.Block(new[] { v }, assignExpr, addAssignExpr, addExpr);
			var lambda = Expression.Lambda(block);
			var f = lambda.Compile();
			var result = f.DynamicInvoke();
			Assert.AreEqual(6, result);
		}

		[TestMethod]
		public void Test15_5()
		{
			string s = "int a=1;a+2";
			var script = new Script();
			var result = script.Eval(s);
			Assert.AreEqual(3, result);
			Assert.AreEqual(1, script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test15_4()
		{
			string s = "a=1;a+2";
			var script = new Script();
			var d = script.CompileGlobal(s);
			var result = d.DynamicInvoke(script.Context);
			Assert.AreEqual(3, result);
			Assert.AreEqual(1, script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test15_3()
		{
			string s = "{a=1;a+2}";
			var script = new Script();
			var d = script.CompileGlobal(s);
			var result = d.DynamicInvoke(script.Context);
			Assert.AreEqual(3, result);
			Assert.IsNull(script.Context.EvalVar("a", out var type));
			Assert.IsNull(type);
		}

		[TestMethod]
		public void Test15_2()
		{
			string s = "{int a=1;a+2}";
			var script = new Script();
			var d = script.CompileGlobal(s);
			var result = d.DynamicInvoke(script.Context);
			Assert.AreEqual(3, result);
			Assert.IsNull(script.Context.EvalVar("a", out var type));
			Assert.IsNull(type);
		}

		[TestMethod]
		public void Test15()
		{
			string s = "int a=1;a+2";
			var script = new Script();
			var d = script.CompileGlobal(s);
			var result = d.DynamicInvoke(script.Context);
			Assert.AreEqual(3, result);
			Assert.AreEqual(1, script.Context.EvalVar("a"));
		}

		[TestMethod]
		public void Test14()
		{
			string s1 = "int sum(int a, int b)=>a+b";
			var script1 = new Script();
			var d1 = script1.CompileGlobal(s1, -1);
			d1.DynamicInvoke(script1.Context);
			Assert.AreEqual(5, script1.Eval("sum(2,3)"));

			var script2 = new Script();
			var d2 = script2.CompileGlobal(s1, -1);
			Assert.AreEqual(d1, d2);
			d2.DynamicInvoke(script2.Context);
			Assert.AreEqual(5, script2.Eval("sum(2,3)"));
		}

		[TestMethod]
		public void Test13()
		{
			string s1 = "int sum(int a, int b)=>a+b";
			var script1 = new Script();
			var d1 = script1.CompileGlobal(s1, -1);
			d1.DynamicInvoke(script1.Context);
			Assert.AreEqual(5, script1.Eval("sum(2,3)"));

			var script2 = new Script();
			var d2 = script2.CompileGlobal(s1, -1);
			Assert.AreEqual(d1, d2);
			int n = 0;
			try
			{
				Assert.AreEqual(5, script2.Eval("sum(2,3)"));
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);
				n++;
			}
			Assert.AreEqual(1, n);
		}

		[TestMethod]
		public void Test12()
		{
			string s1 = "int sum(int a, int b)=a+b";
			var script1 = new Script();
			var d1 = script1.CompileGlobal(s1, -1);
			int n = 0;
			try
			{
				Assert.AreEqual(5, script1.Eval("sum(2,3)"));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				n++;
			}
			Assert.AreEqual(1, n);

			var script2 = new Script();
			var d2 = script2.CompileGlobal(s1, -1);
			Assert.AreEqual(d1, d2);
			n = 0;
			try
			{
				Assert.AreEqual(5, script2.Eval("sum(2,3)"));
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);
				n++;
			}
			Assert.AreEqual(1, n);
		}

		[TestMethod]
		public void Test11()
		{
			string s1 = "int sum(int a, int b)=>a+b";
			var script1 = new Script();
			script1.CompileGlobal(s1, -1).DynamicInvoke(script1.Context);
			Assert.AreEqual(5, script1.Eval("sum(2,3)"));
		}

		//[TestMethod]
		//public void Test10()
		//{
		//	ParameterExpression variable = Expression.Variable(typeof(int), "myVar");
		//	BlockExpression block = Expression.Block(
		//		new[] { variable },
		//		Expression.Assign(variable, Expression.Constant(10)),
		//		Expression.AddAssign(variable, Expression.Constant(5)), // myVar=15
		//		variable
		//	);
		//	var lambda = Expression.Lambda<Func<int>>(block).Compile();

		//	// --- 动态调用该委托 ---

		//	// 目标：等效调用 lambda()，预期输出15

		//	// 步骤1：获取方法信息
		//	MethodInfo method = lambda.Method;
		//	object target = lambda.Target; // 此处无闭包，target为null

		//	// 步骤2：构造调用表达式
		//	MethodCallExpression callExpression = Expression.Call(
		//		target == null ? null : Expression.Constant(target),
		//		method,
		//		new Expression[0] // 无参数
		//	);

		//	// 步骤3：构建Lambda表达式并编译
		//	var callLambda = Expression.Lambda<Func<int>>(callExpression).Compile();

		//	// 步骤4：执行
		//	int result = callLambda();
		//	Assert.AreEqual(15, result);
		//}

		//[TestMethod]
		//public void Test09()
		//{
		//	int n = 10;

		//	var vExpr = Expression.Variable(typeof(int));
		//	var vAssignExpr = Expression.Assign(vExpr, Expression.Constant(10));
		//	var pExpr = Expression.Parameter(typeof(int));
		//	var addExpr = Expression.Add(vExpr, pExpr);
		//	var blockExpr = Expression.Block(new[] { vExpr }, vAssignExpr, addExpr);
		//	var funcType = typeof(Func<int, int>);
		//	var func = Expression.Lambda(funcType, blockExpr, pExpr).Compile();

		//	Assert.AreEqual(13, func.DynamicInvoke(3));

		//	Delegate d = func;
		//	var method = d.Method;
		//	var expr = Expression.Call(method, Expression.Constant(d.Target), Expression.Constant(3));
		//	var t = Expression.Lambda(expr).Compile();
		//	Assert.AreEqual(13, t.DynamicInvoke());
		//}

		[TestMethod]
		public void Test08()
		{
			int n = 10;
			Func<int, int> func = a => a + n;
			var script = new Script();
			script.Context.AddFunc("sum", func);
			var t = script.CompileGlobal("sum(5)+100");
			Assert.AreEqual(115, t.DynamicInvoke(script.Context));
			Assert.AreEqual(215, script.Eval("sum(5)+200"));
		}

		[TestMethod]
		public void Test07_5()
		{
			string s = "int test(int a,int b){int m=6;sum(a,m)+b};int sum(int x, int y)=>x+y;100 * test(5,12) * (6-2)";
			var script = new Script();
			Assert.AreEqual(100 * (5 + 6 + 12) * (6 - 2), script.Eval(s));
		}

		[TestMethod]
		public void Test07_4()
		{
			string s = "int test(int a,int b)=>10+a  ";
			var script = new Script();
			var func1 = script.CompileGlobal<int>(s, -1);
			//Assert.AreEqual(100 * (5 + 6 + 12) * (6 - 2), func1(script.Context));
		}

		[TestMethod]
		public void Test07_3()
		{
			string s = "int test(int a,int b){10+a} ";
			var script = new Script();
			var func1 = script.CompileGlobal<int>(s, -1);
			//Assert.AreEqual(100 * (5 + 6 + 12) * (6 - 2), func1(script.Context));
		}

		[TestMethod]
		public void Test07_2()
		{
			string s = "int test(int a,int b){10+a} ";
			var script = new Script();
			//var func1 = script.CompileGlobal<int>(s, -1);
			script.Eval(s);
			//Assert.AreEqual(100 * (5 + 6 + 12) * (6 - 2), func1(script.Context));
		}

		[TestMethod]
		public void Test07()
		{
			string s = "int test(int a,int b){int m=6;sum(a,m)+b};int sum(int x, int y)=>x+y;100 * test(5,12) * (6-2)";
			var script = new Script();
			var func1 = script.CompileGlobal<int>(s, -1);
			Assert.AreEqual(100 * (5 + 6 + 12) * (6 - 2), func1(script.Context));
		}

		[TestMethod]
		public void Test06_2()
		{
			string s = "int test(int a,int b)=>a+b;100 * test(5,5) * (6-2)";
			var script = new Script();
			var func = script.CompileGlobal(s);
			Assert.IsInstanceOfType(func, typeof(Func<ScriptContext, int>));
			Assert.AreEqual(100 * (5 + 5) * (6 - 2), func.DynamicInvoke(script.Context));
		}

		[TestMethod]
		public void Test06_1()
		{
			string s = "int test(int a,int b)=>a+b;100 * test(5,5) * (6-2)";
			var script = new Script();
			var func1 = script.CompileGlobal<int>(s, -1);
			Assert.AreEqual(100 * (5 + 5) * (6 - 2), func1(script.Context));
		}

		[TestMethod]
		public void Test05_2()
		{
			string s = "100 * (5 + 5) * (6-2)";
			var script = new Script();
			var func = script.CompileGlobal(s);
			Assert.IsInstanceOfType(func, typeof(Func<ScriptContext, int>));
			Assert.AreEqual(100 * (5 + 5) * (6 - 2), func.DynamicInvoke(script.Context));
		}

		[TestMethod]
		public void Test05_1()
		{
			string s = "100 * (5 + 5) * (6-2)";
			var script = new Script();
			var func1 = script.CompileGlobal<int>(s, -1);
			Assert.AreEqual(100 * (5 + 5) * (6 - 2), func1(script.Context));
			var func2 = script.CompileGlobal<int>(s, -1);
			Assert.AreEqual(func1, func2);
		}

		[TestMethod]
		public void Test04_2()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Options.ThrowIfVariableNotExists = true;
			int n = 0;
			try
			{
				var func1 = script.CompileGlobal<int>(s);
			}
			catch (Exception ex)
			{
				n++;
				Console.WriteLine(ex);
			}
			Assert.AreEqual(1, n);
		}

		[TestMethod]
		public void Test04_1()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Options.ThrowIfVariableNotExists = false;
			var func1 = script.CompileGlobal<int>(s, -1);
			var func2 = script.CompileGlobal<string>(s, -1);
			script.Context.SetVar("n", 5);
			Assert.AreEqual(16, func1(script.Context));
			script.Context.SetVar("n", "hello");
			Assert.AreEqual("5hello6", func2(script.Context));
		}

		[TestMethod]
		public void Test03_5()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Options.DynamicVariableType = true;
			var func1 = script.CompileGlobal<int>(s, -1);
			var func2 = script.CompileGlobal<string>(s, -1);
			script.Context.SetVar("n", 5);
			Assert.AreEqual(16, func1(script.Context));
			script.Context.SetVar("n", "hello");
			Assert.AreEqual("5hello6", func2(script.Context));
		}

		[TestMethod]
		public void Test03_4()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Options.DynamicVariableType = true;
			var func = script.CompileGlobal(s);
			script.Context.SetVar("n", 5);
			Assert.AreEqual(16, func.DynamicInvoke(script.Context));
			script.Context.SetVar("n", "hello");
			Assert.AreEqual("5hello6", func.DynamicInvoke(script.Context));
		}

		[TestMethod]
		public void Test03_3()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Options.ThrowIfVariableNotExists = false;
			var func = script.CompileGlobal(s);
			Assert.IsInstanceOfType(func, typeof(Func<ScriptContext, object>));
		}

		[TestMethod]
		public void Test03_2()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Context.SetVar("n", 5);
			var func = script.CompileGlobal(s);
			Assert.AreEqual(16, func.DynamicInvoke(script.Context));
			int n = 0;
			try
			{
				script.Context.SetVar("n", "hello");
				func.DynamicInvoke(script.Context);
			}
			catch (Exception ex)
			{
				n++;
				Console.WriteLine(ex);
			}
			Assert.AreEqual(1, n);
		}

		[TestMethod]
		public void Test03_1()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Context.SetVar("n", 5);
			var func1 = script.CompileGlobal<int>(s);
			Assert.AreEqual(16, func1(script.Context));

			script.Context.SetVar("n", "hello");
			var func2 = script.CompileGlobal<string>(s);
			Assert.AreEqual("5hello6", func2(script.Context));
		}

		[TestMethod]
		public void Test02()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Context.SetVar("n", 5);
			var func = script.CompileGlobal<int>(s);
			Assert.AreEqual(16, func(script.Context));
			script.Context.SetVar("n", 10);
			Assert.AreEqual(21, func(script.Context));
		}

		[TestMethod]
		public void Test01_5()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Options.DynamicVariableType = true;
			var func = script.CompileGlobal(s);
			script.Context.SetVar("n", 5);
			Assert.IsInstanceOfType(func, typeof(Func<ScriptContext, object>));
			Assert.AreEqual(16, func.DynamicInvoke(script.Context));
		}

		[TestMethod]
		public void Test01_4()
		{
			string s = "5+n+6";
			var script1 = new Script();
			script1.Context.SetVar("n", 5);
			var func = script1.CompileGlobal(s);
			Assert.IsInstanceOfType(func, typeof(Func<ScriptContext, int>));

			Assert.AreEqual(16, func.DynamicInvoke(script1.Context));

			var script2 = new Script();
			script2.Context.SetVar("n", 10);
			Assert.AreEqual(21, func.DynamicInvoke(script2.Context));
		}

		[TestMethod]
		public void Test01_3()
		{
			string s = "5+n+6";
			var script1 = new Script();
			script1.Context.SetVar("n", 5);
			var func = script1.CompileGlobal(s);
			Assert.IsInstanceOfType(func, typeof(Func<ScriptContext, int>));
			Assert.AreEqual(16, func.DynamicInvoke(script1.Context));

			var script2 = new Script();
			script2.Context.SetVar("n", 10);
			Assert.AreEqual(21, func.DynamicInvoke(script2.Context));
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = "5+n+6";
			var script1 = new Script();
			script1.Context.SetVar("n", 5);
			var func = script1.CompileGlobal<object>(s);
			Assert.AreEqual(16, func(script1.Context));

			var script2 = new Script();
			script2.Context.SetVar("n", 10);
			Assert.AreEqual(21, func(script2.Context));
		}

		[TestMethod]
		public void Test01_1()
		{
			string s = "5+n+6";
			var script = new Script();
			script.Context.SetVar("n", 5);
			var func = script.CompileGlobal<object>(s);
			Assert.AreEqual(16, func(script.Context));
			script.Context.SetVar("n", 10);
			Assert.AreEqual(21, func(script.Context));
		}
	}
}
