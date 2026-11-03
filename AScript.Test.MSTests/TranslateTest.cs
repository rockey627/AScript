using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class TranslateTest
	{
		[TestMethod]
		public void Test01()
		{
			var s = "Hello ${name}, today is ${DateTime.Now.ToString('yyyy-MM-dd')}";
			var script = new Script();
			script.Context.SetVar("name", "Tom");
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.ToString("yyyy-MM-dd")}", script.Translate(s));
		}

		[TestMethod]
		public void Test01_CompileAll()
		{
			var s = "Hello ${name}, today is ${DateTime.Now.ToString('yyyy-MM-dd')}";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("name", "Tom");
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.ToString("yyyy-MM-dd")}", script.Translate(s));
		}

		[TestMethod]
		public void Test01_CompileAll2()
		{
			var s = "Hello ${name}, today is ${DateTime.Now.ToString('yyyy-MM-dd')}";
			var script = new Script();
			script.Context.SetVar("name", "Tom");
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.ToString("yyyy-MM-dd")}", script.Translate(s, ECompileMode.All));
		}

		[TestMethod]
		public void Test02()
		{
			var s = "Hello ${name}, today is ${var now = DateTime.Now; {now=now.AddDays(1)} now.ToString('yyyy-MM-dd')+'ok}'}hi{aa}";
			var script = new Script();
			script.Context.SetVar("name", "Tom");
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "ok}"}" + "hi{aa}", script.Translate(s));
			Assert.IsNull(script.Context.EvalVar("now"));
		}

		[TestMethod]
		public void Test02_CompileAll()
		{
			var s = "Hello ${name}, today is ${var now = DateTime.Now; {now=now.AddDays(1)} now.ToString('yyyy-MM-dd')+'ok}'}hi{aa}";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("name", "Tom");
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "ok}"}" + "hi{aa}", script.Translate(s));
			Assert.IsNull(script.Context.EvalVar("now"));
		}

		[TestMethod]
		public void Test02_CompileAll2()
		{
			var s = "Hello ${name}, today is ${var now = DateTime.Now; {now=now.AddDays(1)} now.ToString('yyyy-MM-dd')+'ok}'}hi{aa}";
			var script = new Script();
			script.Context.SetVar("name", "Tom");
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")+"ok}"}" + "hi{aa}", script.Translate(s, ECompileMode.All));
			Assert.IsNull(script.Context.EvalVar("now"));
		}

		[TestMethod]
		public void Test03()
		{
			var s = @"Hello ${name}, today is ${
var now = DateTime.Now; 
{
	now = now.AddDays(1)
} 
now.ToString('yyyy-MM-dd')+'ok}'
}hi{aa}${now.Year}";
			var script = new Script();
			script.Context.SetVar("name", "Tom");
			var result = script.Translate(s);
			Console.WriteLine(result);
			var expected = $"Hello Tom, today is {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "ok}"}" + "hi{aa}" + DateTime.Now.Year;
			Assert.AreEqual(expected, result);
			Assert.IsNull(script.Context.EvalVar("now"));
		}

		[TestMethod]
		public void Test03_CompileAll()
		{
			var s = @"Hello ${name}, today is ${
var now = DateTime.Now; 
{
	now = now.AddDays(1)
} 
now.ToString('yyyy-MM-dd')+'ok}'
}hi{aa}${now.Year}";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("name", "Tom");
			var result = script.Translate(s);
			Console.WriteLine(result);
			var expected = $"Hello Tom, today is {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "ok}"}" + "hi{aa}" + DateTime.Now.Year;
			Assert.AreEqual(expected, result);
			Assert.IsNull(script.Context.EvalVar("now"));
		}

		[TestMethod]
		public void Test03_CompileAll2()
		{
			var s = @"Hello ${name}, today is ${
var now = DateTime.Now; 
{
	now = now.AddDays(1)
} 
now.ToString('yyyy-MM-dd')+'ok}'
}hi{aa}${now.Year}";
			var script = new Script();
			script.Context.SetVar("name", "Tom");
			var result = script.Translate(s, ECompileMode.All);
			Console.WriteLine(result);
			var expected = $"Hello Tom, today is {DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "ok}"}" + "hi{aa}" + DateTime.Now.Year;
			Assert.AreEqual(expected, result);
			Assert.IsNull(script.Context.EvalVar("now"));
		}
	}
}
