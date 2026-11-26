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

		[TestMethod]
		public void Test04()
		{
			string s = @"
@{
int calcAge(DateTime birthday) {
    return DateTime.Now.Year - birthday.Year
}
var userSex = user.Sex == 0 ? '女士' : '先生';
//var userAge = DateTime.Now.Year - user.Birthday.Year;
//var userAge = calcAge(user.Birthday);
}
尊敬的 ${user.Name}${userSex} 您好，祝您${calcAge(user.Birthday)}岁生日快乐！
";
			// 模拟获取业务数据
			var user = new { Name = "张三", Sex = 1, Birthday = new DateTime(1990, 10, 8), CreateTime = new DateTime(2026, 5, 1) };
			var script = new Script();
			// 注入业务数据
			script.Context.SetVar("user", user);
			// 转化模板
			var result = script.Translate(s)?.Trim();
			Console.WriteLine(result);
			var expected = $"尊敬的 张三先生 您好，祝您{DateTime.Now.Year - user.Birthday.Year}岁生日快乐！";
			Console.WriteLine(expected);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test04_CompileAll()
		{
			string s = @"
@{
int calcAge(DateTime birthday) {
    return DateTime.Now.Year - birthday.Year
}
var userSex = user.Sex == 0 ? '女士' : '先生';
//var userAge = DateTime.Now.Year - user.Birthday.Year;
//var userAge = calcAge(user.Birthday);
}
尊敬的 ${user.Name}${userSex} 您好，祝您${calcAge(user.Birthday)}岁生日快乐！
";
			// 模拟获取业务数据
			var user = new { Name = "张三", Sex = 1, Birthday = new DateTime(1990, 10, 8), CreateTime = new DateTime(2026, 5, 1) };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			// 注入业务数据
			script.Context.SetVar("user", user);
			// 转化模板
			var result = script.Translate(s)?.Trim();
			Console.WriteLine(result);
			var expected = $"尊敬的 张三先生 您好，祝您{DateTime.Now.Year - user.Birthday.Year}岁生日快乐！";
			Console.WriteLine(expected);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test04_CompileAll2()
		{
			string s = @"
@{
int calcAge(DateTime birthday) {
    return DateTime.Now.Year - birthday.Year
}
var userSex = user.Sex == 0 ? '女士' : '先生';
//var userAge = DateTime.Now.Year - user.Birthday.Year;
//var userAge = calcAge(user.Birthday);
}
尊敬的 ${user.Name}${userSex} 您好，祝您${calcAge(user.Birthday)}岁生日快乐！
";
			// 模拟获取业务数据
			var user = new { Name = "张三", Sex = 1, Birthday = new DateTime(1990, 10, 8), CreateTime = new DateTime(2026, 5, 1) };
			var script = new Script();
			// 注入业务数据
			script.Context.SetVar("user", user);
			// 转化模板
			var result = script.Translate(s, ECompileMode.All)?.Trim();
			Console.WriteLine(result);
			var expected = $"尊敬的 张三先生 您好，祝您{DateTime.Now.Year - user.Birthday.Year}岁生日快乐！";
			Console.WriteLine(expected);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test05_output()
		{
			string s = @"
@{
int calcAge(DateTime birthday) {
    return DateTime.Now.Year - birthday.Year
}
output.Append('hello');
for(var i=0; i<5; i++) {
	output.Append(i);
}
var userSex = user.Sex == 0 ? '女士' : '先生';
//var userAge = DateTime.Now.Year - user.Birthday.Year;
//var userAge = calcAge(user.Birthday);
}
尊敬的 ${user.Name}${userSex} 您好，祝您${calcAge(user.Birthday)}岁生日快乐！
";
			// 模拟获取业务数据
			var user = new { Name = "张三", Sex = 1, Birthday = new DateTime(1990, 10, 8), CreateTime = new DateTime(2026, 5, 1) };
			var script = new Script();
			// 注入业务数据
			script.Context.SetVar("user", user);
			// 转化模板
			var result = script.Translate(s)?.Trim();
			Console.WriteLine(result);
			// 脚本块独占一行，换行符被移除
			var expected = $"hello01234尊敬的 张三先生 您好，祝您{DateTime.Now.Year - user.Birthday.Year}岁生日快乐！";
			Console.WriteLine(expected);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test05_output_CompileAll()
		{
			string s = @"
@{
int calcAge(DateTime birthday) {
    return DateTime.Now.Year - birthday.Year
}
output.Append('hello');
for(var i=0; i<5; i++) {
	output.Append(i);
}
var userSex = user.Sex == 0 ? '女士' : '先生';
//var userAge = DateTime.Now.Year - user.Birthday.Year;
//var userAge = calcAge(user.Birthday);
}
尊敬的 ${user.Name}${userSex} 您好，祝您${calcAge(user.Birthday)}岁生日快乐！
";
			// 模拟获取业务数据
			var user = new { Name = "张三", Sex = 1, Birthday = new DateTime(1990, 10, 8), CreateTime = new DateTime(2026, 5, 1) };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			// 注入业务数据
			script.Context.SetVar("user", user);
			// 转化模板
			var result = script.Translate(s)?.Trim();
			Console.WriteLine(result);
			// 脚本块独占一行，换行符被移除
			var expected = $"hello01234尊敬的 张三先生 您好，祝您{DateTime.Now.Year - user.Birthday.Year}岁生日快乐！";
			Console.WriteLine(expected);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test06_output()
		{
			string s = @"
@{
	int calcAge(DateTime birthday) {
		return DateTime.Now.Year - birthday.Year
	}
	output.Append('hello');
	for(var i=0; i<5; i++) {
		output.Append(i);
	}
	var userSex = user.Sex == 0 ? '女士' : '先生';
	//var userAge = DateTime.Now.Year - user.Birthday.Year;
	//var userAge = calcAge(user.Birthday);
}
尊敬的 ${user.Name}${userSex} 您好，祝您${calcAge(user.Birthday)}岁生日快乐！
@{
	output.Replace('\r', '').Replace('\n', '')
}";
			// 模拟获取业务数据
			var user = new { Name = "张三", Sex = 1, Birthday = new DateTime(1990, 10, 8), CreateTime = new DateTime(2026, 5, 1) };
			var script = new Script();
			// 注入业务数据
			script.Context.SetVar("user", user);
			// 转化模板
			var result = script.Translate(s);
			Console.WriteLine(result);
			var expected = $"hello01234尊敬的 张三先生 您好，祝您{DateTime.Now.Year - user.Birthday.Year}岁生日快乐！";
			Console.WriteLine(expected);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test06_output_CompileAll()
		{
			string s = @"
@{
	int calcAge(DateTime birthday) {
		return DateTime.Now.Year - birthday.Year
	}
	output.Append('hello');
	for(var i=0; i<5; i++) {
		output.Append(i);
	}
	var userSex = user.Sex == 0 ? '女士' : '先生';
	//var userAge = DateTime.Now.Year - user.Birthday.Year;
	//var userAge = calcAge(user.Birthday);
}
尊敬的 ${user.Name}${userSex} 您好，祝您${calcAge(user.Birthday)}岁生日快乐！
@{
	output.Replace('\r', '').Replace('\n', '')
}";
			// 模拟获取业务数据
			var user = new { Name = "张三", Sex = 1, Birthday = new DateTime(1990, 10, 8), CreateTime = new DateTime(2026, 5, 1) };
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			// 注入业务数据
			script.Context.SetVar("user", user);
			// 转化模板
			var result = script.Translate(s);
			Console.WriteLine(result);
			var expected = $"hello01234尊敬的 张三先生 您好，祝您{DateTime.Now.Year - user.Birthday.Year}岁生日快乐！";
			Console.WriteLine(expected);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test07_escape()
		{
			// \ 可以转义 \、@、$ 这三个字符
			var s = @"a\\b \@ c \$ d";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("a\\b @ c $ d", result);
		}

		[TestMethod]
		public void Test08_escape_script()
		{
			// 转义后的 @ 和 $ 不作为脚本标记
			var s = @"\@{ 1 + 1 } \${ 1 + 1 }";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("@{ 1 + 1 } ${ 1 + 1 }", result);
		}

		[TestMethod]
		public void Test09_escape_other()
		{
			// 其他字符不能被转义
			var s = @"a\nb\tc";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual(@"a\nb\tc", result);
		}

		[TestMethod]
		public void Test10_comment()
		{
			// 注释块内容被跳过
			var s = "Hello @* this is a comment *@ World";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello  World", result);
		}

		[TestMethod]
		public void Test11_comment_line()
		{
			// 注释块独占一行时，移除该行的换行符
			var s = "Hello\r\n@* this is a comment\r\n       can span lines *@\r\nWorld";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello\r\nWorld", result);
		}

		[TestMethod]
		public void Test11_comment_line_indent()
		{
			// 注释块之前有空白字符，不算独占一行，保留换行符
			var s = "Hello\r\n    @* this is a comment\r\n       can span lines *@\r\nWorld";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello\r\n    \r\nWorld", result);
		}

		[TestMethod]
		public void Test12_comment_inline()
		{
			// 注释块未独占一行时，保留换行符
			var s = "Hello @* this is a comment *@ World\r\nNext";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello  World\r\nNext", result);
		}

		[TestMethod]
		public void Test13_comment_not_closed()
		{
			// 注释块未闭合时抛出异常
			var s = "Hello @* this is a comment";
			var script = new Script();
			Assert.ThrowsException<Exceptions.ScriptAnalyzingException>(() => script.Translate(s));
		}

		[TestMethod]
		public void Test14_script_line()
		{
			// 脚本块独占一行时，移除该行的换行符
			var s = "Hello\r\n@{ var a = 1; }\r\nWorld";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello\r\nWorld", result);
		}

		[TestMethod]
		public void Test14_script_line_indent()
		{
			// 脚本块之前有空白字符，不算独占一行，保留换行符
			var s = "Hello\r\n    @{ var a = 1; }\r\nWorld";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello\r\n    \r\nWorld", result);
		}

		[TestMethod]
		public void Test14_script_line_tail()
		{
			// 脚本块之后有空白字符，不算独占一行，保留换行符
			var s = "Hello\r\n@{ var a = 1; }  \r\nWorld";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello\r\n  \r\nWorld", result);
		}

		[TestMethod]
		public void Test15_script_inline()
		{
			// 脚本块未独占一行时，保留换行符
			var s = "Hello @{ var a = 1; } World\r\nNext";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello  World\r\nNext", result);
		}

		[TestMethod]
		public void Test16_script_line_output()
		{
			// 脚本块独占一行时，脚本块的内容输出到该行
			var s = "Hello\r\n@{ output.Append('world'); }\r\nNext";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello\r\nworldNext", result);
		}

		[TestMethod]
		public void Test17_expression_line()
		{
			// 表达式不独占一行时，不移除换行符
			var s = "Hello\r\n${ 1 + 1 }\r\nNext";
			var script = new Script();
			var result = script.Translate(s);
			Console.WriteLine(result);
			Assert.AreEqual("Hello\r\n2\r\nNext", result);
		}

		[TestMethod]
		public void Test18_readme()
		{
			// README中的示例
			string s = @"
@{
	int calcAge(DateTime birthday) {
		return DateTime.Now.Year - birthday.Year
	}
	output.Append('hello');
	for(var i=0; i<5; i++) {
		output.Append(i);
	}
	var userSex = user.Sex == 0 ? '女士' : '先生';
	//var userAge = DateTime.Now.Year - user.Birthday.Year;
	//var userAge = calcAge(user.Birthday);
}
尊敬的 ${user.Name}${userSex} 您好，祝您${calcAge(user.Birthday)}岁生日快乐！
";
			var user = new { Name = "张三", Sex = 1, Birthday = new DateTime(1990, 10, 8), CreateTime = new DateTime(2026, 5, 1) };
			var script = new Script();
			script.Context.SetVar("user", user);
			var result = script.Translate(s)?.Trim();
			Console.WriteLine(result);
			var expected = $"hello01234尊敬的 张三先生 您好，祝您{DateTime.Now.Year - user.Birthday.Year}岁生日快乐！";
			Console.WriteLine(expected);
			Assert.AreEqual(expected, result);
		}
	}
}
