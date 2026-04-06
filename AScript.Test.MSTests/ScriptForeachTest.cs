using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptForeachTest
	{
		[TestMethod]
		public void Test02_2()
		{
			string s = @"
int n=0;
foreach(var item in list)
{
	// 跳过偶数
	if (item % 2 == 0) continue;
	if (item >/*不超过10*/10) break;
	n+=item;
}
n+10";

			var list = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			int n = 0;
			foreach (var item in list)
			{
				// 跳过偶数
				if (item % 2 == 0) continue;
				if (item >/*不超过10*/ 10) break;
				n += item;
			}

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", list);
			Assert.AreEqual(n+10, script.Eval(s));
			Assert.AreEqual(n, script.Context.EvalVar("n"));
			Assert.IsNull(script.Context.EvalVar("item", out var type));
			Assert.IsNull(type);
		}

		[TestMethod]
		public void Test02()
		{
			string s = @"
int n=0;
foreach(var item in list)
{
	// 跳过偶数
	if (item % 2 == 0) continue;
	if (item >/*不超过10*/10) break;
	n+=item;
}
n";

			var list = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			int n = 0;
			foreach (var item in list)
			{
				// 跳过偶数
				if (item % 2 == 0) continue;
				if (item >/*不超过10*/ 10) break;
				n += item;
			}

			var script = new Script();
			script.Context.SetVar("list", list);
			Assert.AreEqual(n, script.Eval(s));
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = "int n;foreach(var item in list) n+=item;n";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("list", new int[] { 1, 3, 5 });
			Assert.AreEqual(9, script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = "int n;foreach(var item in list) n+=item;n";
			var script = new Script();
			script.Context.SetVar("list", new int[] { 1, 3, 5 });
			Assert.AreEqual(9, script.Eval(s));
		}
	}
}
