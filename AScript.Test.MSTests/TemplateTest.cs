using AScript.Exceptions;
using AScript.Lang.Go;
using AScript.Lang.JavaScript;
using AScript.Lang.Lua;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class TemplateTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["go"] = GoLang.Instance;
			Script.Langs["js"] = JavaScriptLang.Instance;
			Script.Langs["lua"] = LuaLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("go");
			Script.Langs.TryRemove("js");
			Script.Langs.TryRemove("lua");
		}

		[TestMethod]
		public void Test01_Text()
		{
			var template = new Template("Hello World!");
			var result = template.Render(null);
			Assert.AreEqual("Hello World!", result);
		}

		[TestMethod]
		public void Test01_Text_CompileAll()
		{
			var template = new Template("Hello World!");
			template.Options.CompileMode = ECompileMode.All;
			var result = template.Render(null);
			Assert.AreEqual("Hello World!", result);
		}

		[TestMethod]
		public void Test02_Expression()
		{
			var template = new Template("Hello ${name}, today is ${DateTime.Now.Year}");
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.Year}", result);
		}

		[TestMethod]
		public void Test02_Expression_CompileAll()
		{
			var template = new Template("Hello ${name}, today is ${DateTime.Now.Year}");
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.Year}", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock()
		{
			var template = new Template("@{var userSex = name == 'Tom' ? '先生' : '女士';}尊敬的 ${name}${userSex} 您好！");
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			Assert.AreEqual("尊敬的 Tom先生 您好！", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_CompileAll()
		{
			var template = new Template("@{var userSex = name == 'Tom' ? '先生' : '女士';}尊敬的 ${name}${userSex} 您好！");
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			Assert.AreEqual("尊敬的 Tom先生 您好！", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren()
		{
			var template = new Template("@(var userSex = name == 'Tom' ? '先生' : '女士';)尊敬的 ${name}${userSex} 您好！");
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			Assert.AreEqual("尊敬的 Tom先生 您好！", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_CompileAll()
		{
			var template = new Template("@(var userSex = name == 'Tom' ? '先生' : '女士';)尊敬的 ${name}${userSex} 您好！");
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			Assert.AreEqual("尊敬的 Tom先生 您好！", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Async()
		{
			var template = new Template("@(var userSex = name == 'Tom' ? '先生' : '女士';)尊敬的 ${name}${userSex} 您好！");
			template.Context.SetVar("name", "Tom");
			var result = template.RenderAsync(null).Result;
			Assert.AreEqual("尊敬的 Tom先生 您好！", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Braces()
		{
			// 括号块内可以包含大括号，且嵌套的括号需要正确配对
			var template = new Template("@(int n=0;if (list.Count > 0) { n = list.Count;})共 ${n} 项");
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Assert.AreEqual("共 2 项", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Braces_CompileAll()
		{
			// 括号块内可以包含大括号，且嵌套的括号需要正确配对
			var template = new Template("@(int n=0;if (list.Count > 0) { n = list.Count;})共 ${n} 项");
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Assert.AreEqual("共 2 项", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Braces2()
		{
			// 括号块内可以包含大括号，且嵌套的括号需要正确配对
			var template = new Template("@(if (list.Count > 0) { var n = list.Count;)共 ${n} 项@(} else {)无数据@}");
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Assert.AreEqual("共 2 项", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Braces3()
		{
			// 括号块内可以包含大括号，且嵌套的括号需要正确配对
			var template = new Template("@(if (list.Count > 0) { var n = list.Count;)共 ${n} 项@(} else {)无数据@}");
			template.Context.SetVar("list", new List<string> { });
			var result = template.Render(null);
			Assert.AreEqual("无数据", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Braces4()
		{
			string s = @"@if (list.Count > 0) { var n = list.Count;
共 ${n} 项
@} else {
无数据
@}";
			var template = new Template(s);
			template.Context.SetVar("list", new List<string> { });
			var result = template.Render(null);
			Assert.AreEqual("无数据\r\n", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Braces4_CompileAll()
		{
			string s = @"@if (list.Count > 0) { var n = list.Count;
共 ${n} 项
@} else {
无数据
@}";
			var template = new Template(s);
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("list", new List<string> { });
			var result = template.Render(null);
			Assert.AreEqual("无数据\r\n", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Braces5()
		{
			string s = @"@if (list.Count > 0) { var n = list.Count;
共 ${n} 项
@} else {
无数据
@}";
			var template = new Template(s);
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Assert.AreEqual("共 2 项\r\n", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock_Paren_Braces5_CompileAll()
		{
			string s = @"@if (list.Count > 0) { var n = list.Count;
共 ${n} 项
@} else {
无数据
@}";
			var template = new Template(s);
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Assert.AreEqual("共 2 项\r\n", result);
		}

		[TestMethod]
		public void Test03_Comment()
		{
			// 注释块@*...*@内的内容不输出
			var template = new Template("Hello @* this is ${name} a @{ok()}@*comment @*@World\\@* hello *@");
			var result = template.Render(null);
			// 最后一个@为脚本行
			Assert.AreEqual("Hello World@* hello *", result);
		}

		[TestMethod]
		public void Test03_Comment_CompileAll()
		{
			var template = new Template("Hello @* this is ${name} a @{ok()}@*comment @*@World");
			template.Options.CompileMode = ECompileMode.All;
			var result = template.Render(null);
			Assert.AreEqual("Hello World", result);
		}

		[TestMethod]
		public void Test03_Comment_Line()
		{
			// 注释独占一行时，移除该行的换行符
			var tpl = "@* comment *@\r\nHello\r\n@* multi\r\nline ${name} comment *@\r\nWorld";
			var template = new Template(tpl);
			var result = template.Render(null);
			Assert.AreEqual("Hello\r\nWorld", result);
		}

		[TestMethod]
		public void Test03_Comment_Async()
		{
			var template = new Template("Hello @* this is a comment *@World");
			var result = template.RenderAsync(null).Result;
			Assert.AreEqual("Hello World", result);
		}

		[TestMethod]
		public void Test04_Statement()
		{
			var template = new Template("@foreach(var item in list) {\r\n\t[${item}]\r\n@}\r\nend");
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Assert.AreEqual("\t[a]\r\n\t[b]\r\nend", result);
		}

		[TestMethod]
		public void Test04_Statement_CompileAll()
		{
			var template = new Template("@foreach(var item in list) {\r\n\t[${item}]\r\n@}\r\nend");
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Assert.AreEqual("\t[a]\r\n\t[b]\r\nend", result);
		}

		[TestMethod]
		public void Test04_Statement2()
		{
			var tpl = @"@foreach(var item in list) {
	[${item}]
@}
end";
			var template = new Template(tpl);
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Console.WriteLine(result);
			var expected = @"	[a]
	[b]
end";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test04_Statement2_CompileAll()
		{
			var tpl = @"@foreach(var item in list) {
	[${item}]
@}
end";
			var template = new Template(tpl);
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Console.WriteLine(result);
			var expected = @"	[a]
	[b]
end";
			Assert.AreEqual(expected, result);
		}

		private static readonly string _template01_expected = "// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @"
using System;

namespace MyTest.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	[Table(""Users"")]
	public class User {
		/// <summary>
		/// 主键
		/// </summary>
		[PrimaryKey]
		public int Id { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
	}
}";

		[TestMethod]
		public void Test05_TemplateFile()
		{
			var tpl = File.ReadAllText("template_01.txt");
			var template = new Template(tpl);
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			result = template.Render(null);
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test05_TemplateFile_CompileAll()
		{
			var tpl = File.ReadAllText("template_01.txt");
			var template = new Template(tpl);
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);

			Assert.AreEqual(_template01_expected, result);

			result = template.Render(null);
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile()
		{
			var template = new Template(new FileInfo("template_01.txt"));
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			template.Render(null, "template_01_result.txt");
			var result = File.ReadAllText("template_01_result.txt");
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			template.Render(null, "template_01_result.txt");
			result = File.ReadAllText("template_01_result.txt");
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile_CompileAll()
		{
			var template = new Template(new FileInfo("template_01.txt"));
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			template.Render(null, "template_01_result.txt");
			var result = File.ReadAllText("template_01_result.txt");
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			template.Render(null, "template_01_result.txt");
			result = File.ReadAllText("template_01_result.txt");
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile_go()
		{
			var template = new Template(new FileInfo("template_01_go.txt"));
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			result = template.Render(null);
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile_go_CompileAll()
		{
			var template = new Template(new FileInfo("template_01_go.txt"));
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			result = template.Render(null);
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile_js()
		{
			var template = new Template(new FileInfo("template_01_js.txt"));
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			result = template.Render(null);
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile_js_CompileAll()
		{
			var template = new Template(new FileInfo("template_01_js.txt"));
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			result = template.Render(null);
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile_lua()
		{
			var template = new Template(new FileInfo("template_01_lua.txt"));
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			result = template.Render(null);
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile_lua_Compile()
		{
			var template = new Template(new FileInfo("template_01_lua.txt"));
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			template.Compile(null, true);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test06_TemplateFile_lua_CompileAll()
		{
			var template = new Template(new FileInfo("template_01_lua.txt"));
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(_template01_expected, result);

			result = template.RenderAsync(null).Result;
			Assert.AreEqual(_template01_expected, result);
		}

		[TestMethod]
		public void Test07_Compile()
		{
			var template = new Template("Hello ${name}, today is ${DateTime.Now.Year}");
			template.Context.SetVar("name", "Tom");
			template.Compile(null);

			{
				var result = template.Render(null);
				Assert.AreEqual($"Hello Tom, today is {DateTime.Now.Year}", result);
			}

			{
				template.Context.SetVar("name", "John");
				var result = template.Render(null);
				Assert.AreEqual($"Hello John, today is {DateTime.Now.Year}", result);
			}
		}

		[TestMethod]
		public void Test07_Compile_standalone()
		{
			var template = new Template("Hello ${name}, today is ${DateTime.Now.Year}");
			template.Context.SetVar("name", "Tom");
			template.Compile(null, true);

			{
				var result = template.Render(null);
				Assert.AreEqual($"Hello Tom, today is {DateTime.Now.Year}", result);
			}

			{
				// 模板使用standalone编译，上下文数据更改不影响渲染
				template.Context.SetVar("name", "John");
				var result = template.Render(null);
				Assert.AreEqual($"Hello Tom, today is {DateTime.Now.Year}", result);
			}
		}

		[TestMethod]
		public void Test08_Compile_Person()
		{
			var template = new Template("Hello ${model.Name}, ${age}, today is ${DateTime.Now.Year}");
			template.Context.SetVar("age", 20);
			template.Compile<Person>();

			{
				var result = template.Render(new Person { Name = "Tom" });
				Assert.AreEqual($"Hello Tom, 20, today is {DateTime.Now.Year}", result);
			}

			{
				template.Context.SetVar("age", 30);
				var result = template.Render(new Person { Name = "John" });
				Assert.AreEqual($"Hello John, 30, today is {DateTime.Now.Year}", result);
			}
		}

		[TestMethod]
		public void Test08_Compile_Person_standalone()
		{
			var template = new Template("Hello ${model.Name}, ${age}, today is ${DateTime.Now.Year}");
			template.Context.SetVar("age", 20);
			template.Compile<Person>(true);

			{
				var result = template.Render(new Person { Name = "Tom" });
				Assert.AreEqual($"Hello Tom, 20, today is {DateTime.Now.Year}", result);
			}

			{
				// 模板使用standalone编译，上下文数据更改不影响渲染
				template.Context.SetVar("age", 30);
				var result = template.Render(new Person { Name = "John" });
				Assert.AreEqual($"Hello John, 20, today is {DateTime.Now.Year}", result);
			}
		}

		[TestMethod]
		public void Test09_Compiled_Person()
		{
			var template = new Template("Hello ${model.Name}, ${age}, today is ${DateTime.Now.Year}");
			template.Context.SetVar("age", 20);
			var compiled = template.Compile<Person>();

			{
				var result = compiled.Render(new Person { Name = "Tom" });
				Assert.AreEqual($"Hello Tom, 20, today is {DateTime.Now.Year}", result);
			}

			{
				compiled.Context.SetVar("age", 30);
				var result = compiled.Render(new Person { Name = "John" });
				Assert.AreEqual($"Hello John, 30, today is {DateTime.Now.Year}", result);
			}
		}

		[TestMethod]
		public void Test09_Compiled_Person_standalone()
		{
			var template = new Template("Hello ${model.Name}, ${age}, today is ${DateTime.Now.Year}");
			template.Context.SetVar("age", 20);
			var compiled = template.Compile<Person>(true);

			{
				var result = compiled.Render(new Person { Name = "Tom" });
				Assert.AreEqual($"Hello Tom, 20, today is {DateTime.Now.Year}", result);
			}

			{
				// 模板使用standalone编译，上下文数据更改不影响渲染
				compiled.Context.SetVar("age", 30);
				var result = compiled.Render(new Person { Name = "John" });
				Assert.AreEqual($"Hello John, 20, today is {DateTime.Now.Year}", result);
			}
		}

		[TestMethod]
		public void Test10_empty()
		{
			var tpl = @"Hello ${}, @{}today is ${DateTime.Now.Year}
@
end";
			var template = new Template(tpl);
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			var expected = $@"Hello , today is {DateTime.Now.Year}
end";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public async Task Test11_RenderAsync()
		{
			var template = new Template(new FileInfo("template_01.txt"));
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = await template.RenderAsync(null);
			Console.WriteLine(result);
			var expected = "// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @"
using System;

namespace MyTest.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	[Table(""Users"")]
	public class User {
		/// <summary>
		/// 主键
		/// </summary>
		[PrimaryKey]
		public int Id { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
	}
}";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public async Task Test12_RenderAsync_file()
		{
			var template = new Template(new FileInfo("template_01.txt"));
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			await template.RenderAsync(null, "template_01_result_Test11_RenderAsync.txt");
			var result = File.ReadAllText("template_01_result_Test11_RenderAsync.txt");
			Console.WriteLine(result);
			var expected = "// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @"
using System;

namespace MyTest.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	[Table(""Users"")]
	public class User {
		/// <summary>
		/// 主键
		/// </summary>
		[PrimaryKey]
		public int Id { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
	}
}";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test13_load()
		{
			//var tpl = File.ReadAllText("template_01.txt");
			var tpl = @"hello@load('template_01.txt')";
			var template = new Template(tpl);
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			var expected = "hello// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @"
using System;

namespace MyTest.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	[Table(""Users"")]
	public class User {
		/// <summary>
		/// 主键
		/// </summary>
		[PrimaryKey]
		public int Id { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
	}
}";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test13_load_CompileAll()
		{
			//var tpl = File.ReadAllText("template_01.txt");
			var tpl = @"hello@load('template_01.txt')";
			var template = new Template(tpl);
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			var result = template.Render(null);
			Console.WriteLine(result);
			var expected = "hello// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @"
using System;

namespace MyTest.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	[Table(""Users"")]
	public class User {
		/// <summary>
		/// 主键
		/// </summary>
		[PrimaryKey]
		public int Id { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
	}
}";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test14_load_Compile()
		{
			//var tpl = File.ReadAllText("template_01.txt");
			var tpl = @"hello@load('template_01.txt')";
			var template = new Template(tpl);
			template.Context.SetVar("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			});
			template.Compile(null);
			var result = template.Render(null);
			Console.WriteLine(result);
			var expected = "hello// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @"
using System;

namespace MyTest.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	[Table(""Users"")]
	public class User {
		/// <summary>
		/// 主键
		/// </summary>
		[PrimaryKey]
		public int Id { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
	}
}";
			Assert.AreEqual(expected, result);

			result = template.Render(null);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test15_escape()
		{
			var tpl = @"a\${name}\$${name} b\@{DateTime.Now} c\\\@ d\$ e\f";
			var template = new Template(tpl);
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual(@"a${name}$Tom b@{DateTime.Now} c\@ d$ e\f", result);
		}

		[TestMethod]
		public async Task Test15_escape_Async()
		{
			var tpl = @"a\${name}\$${name} b\@{DateTime.Now} c\\\@ d\$ e\f";
			var template = new Template(tpl);
			template.Context.SetVar("name", "Tom");
			var result = await template.RenderAsync(null);
			Console.WriteLine(result);
			Assert.AreEqual(@"a${name}$Tom b@{DateTime.Now} c\@ d$ e\f", result);
		}

		[TestMethod]
		public void Test16_include()
		{
			// include：包含模板文件，在宿主模板中渲染，可以访问宿主模板的变量
			var tpl = @"@{ var title = ""Hello""; }@include('template_include_01.txt')";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("[Hello]", result);
		}

		[TestMethod]
		public void Test16_include_CompileAll()
		{
			// 编译模式下，包含的模板与宿主模板共享上下文
			var tpl = @"@{ var title = ""Hello""; }@include('template_include_01.txt')";
			var template = new Template(tpl);
			template.Options.CompileMode = ECompileMode.All;
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("[Hello]", result);
		}

		[TestMethod]
		public void Test16_include02()
		{
			// include：包含模板文件，在宿主模板中渲染，可以访问宿主模板的变量
			var tpl = @"@{ var title = ""Hello""; }@{output.Write('hi');include('template_include_01.txt');output.Write('ok')}end";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("hi[Hello]okend", result);
		}

		[TestMethod]
		public void Test16_include02_Compile()
		{
			// include：包含模板文件，在宿主模板中渲染，可以访问宿主模板的变量
			var tpl = @"@{ var title = ""Hello""; }@{output.Write('hi');include('template_include_01.txt');output.Write('ok')}end";
			var template = new Template(tpl);
			var result = template.Compile(null).Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("hi[Hello]okend", result);
		}

		[TestMethod]
		public void Test16_include02_CompileAll()
		{
			// include：包含模板文件，在宿主模板中渲染，可以访问宿主模板的变量
			var tpl = @"@{ var title = ""Hello""; }@{output.Write('hi');include('template_include_01.txt');output.Write('ok')}end";
			var template = new Template(tpl);
			template.Options.CompileMode = ECompileMode.All;
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("hi[Hello]okend", result);
		}

		[TestMethod]
		public void Test16_include03()
		{
			// include：包含模板文件，在宿主模板中渲染，可以访问宿主模板的变量
			var tpl = @"@{ var title = ""Hello""; }@{output.Write('hi');include  
(  'template_include_01.txt' );output.Write('ok')}end";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("hi[Hello]okend", result);
		}

		[TestMethod]
		public void Test16_include04()
		{
			// include 可以写在 ${} 表达式中
			var tpl = @"@{ var title = ""Hello""; }A${include('template_include_01.txt')}B";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("A[Hello]B", result);
		}

		[TestMethod]
		public async Task Test16_include04_Async()
		{
			// include 可以写在 ${} 表达式中
			var tpl = @"@{ var title = ""Hello""; }A${include('template_include_01.txt')}B";
			var template = new Template(tpl);
			var result = await template.RenderAsync(null);
			Console.WriteLine(result);
			Assert.AreEqual("A[Hello]B", result);
		}

		[TestMethod]
		public void Test16_include05()
		{
			// include：包含模板文件，在宿主模板中渲染，可以访问宿主模板的变量
			var tpl = @"@{ var title = ""Hello""; }@include   ( 'template_include_01.txt')
end";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("[Hello]\r\nend", result);
		}

		[TestMethod]
		public void Test16_include06()
		{
			// include：包含模板文件，在宿主模板中渲染，可以访问宿主模板的变量
			var tpl = @"@{ var title = ""Hello""; }
@include   ( 'template_include_01.txt')
end";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("[Hello]\r\nend", result);
		}

		[TestMethod]
		public void Test16_include07()
		{
			// include：包含模板文件，在宿主模板中渲染，可以访问宿主模板的变量
			var tpl = @"@{ var title = ""Hello""; }
@include   ( 'template_include_01.txt')

end";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("[Hello]\r\n\r\nend", result);
		}

		[TestMethod]
		public void Test16_include08()
		{
			var tpl = @"@{ var title = ""Hello""; }
A${include('template_include_01.txt')}B";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("A[Hello]B", result);
		}

		[TestMethod]
		public void Test16_include09()
		{
			var tpl = @"@{ var title = ""Hello""; }
A${include('template_include_01.txt')}
B";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("A[Hello]\r\nB", result);
		}

		[TestMethod]
		public void Test16_include10()
		{
			var tpl = @"@{ var title = ""Hello""; }
A
${include('template_include_01.txt')}
B";
			var template = new Template(tpl);
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("A\r\n[Hello]\r\nB", result);
		}

		[TestMethod]
		public void Test17_include_file()
		{
			// 相对路径以模板文件所在目录为基准
			var template = new Template(new FileInfo("template_include_host.txt"));
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("beginend[Hello]", result);
		}

		[TestMethod]
		public void Test17_include_file_CompileAll()
		{
			// 相对路径以模板文件所在目录为基准
			var template = new Template(new FileInfo("template_include_host.txt"));
			template.Options.CompileMode = ECompileMode.All;
			var result = template.Render(null);
			Console.WriteLine(result);
			Assert.AreEqual("beginend[Hello]", result);
		}

		[TestMethod]
		public async Task Test18_include_Async()
		{
			var tpl = @"@{ var title = ""Hello""; }@include('template_include_01.txt')";
			var template = new Template(tpl);
			var result = await template.RenderAsync(null);
			Console.WriteLine(result);
			Assert.AreEqual("[Hello]", result);
		}

		[TestMethod]
		public async Task Test18_include_Async_CompileAll()
		{
			var tpl = @"@{ var title = ""Hello""; }@include('template_include_01.txt')";
			var template = new Template(tpl);
			template.Options.CompileMode = ECompileMode.All;
			var result = await template.RenderAsync(null);
			Console.WriteLine(result);
			Assert.AreEqual("[Hello]", result);
			result = await template.RenderAsync(null);
			Assert.AreEqual("[Hello]", result);
		}

		[TestMethod]
		public void Test19_Cache()
		{
			var template = new Template("Hello ${name}, today is ${DateTime.Now.Year}");
			template.CacheTime = -1;

			{
				template.Context.SetVar("name", "Tom");
				var result = template.Render(null);
				Assert.AreEqual($"Hello Tom, today is {DateTime.Now.Year}", result);
			}

			{
				template.Context.SetVar("name", "John");
				var result = template.Render(null);
				Assert.AreEqual($"Hello John, today is {DateTime.Now.Year}", result);
			}
		}

		[TestMethod]
		public void Test20_TemplateFile_model()
		{
			var template = new Template(new FileInfo("template_02_model.txt"));
			template.Context.SetVar("author", "Tom");
			var entity = new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			};
			var result = template.Render(entity);
			Console.WriteLine(result);
			var expected = "// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @" by Tom
using System;

namespace MyTest.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	[Table(""Users"")]
	public class User {
		/// <summary>
		/// 主键
		/// </summary>
		[PrimaryKey]
		public int Id { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
	}
}";
			Assert.AreEqual(expected, result);

			result = template.Render(entity);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test20_TemplateFile_model_CompileAll()
		{
			var template = new Template(new FileInfo("template_02_model.txt"));
			template.Options.CompileMode = ECompileMode.All;
			template.Context.SetVar("author", "Tom");
			var entity = new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users",
				Columns = new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				}
			};
			var result = template.Render(entity);
			Console.WriteLine(result);
			var expected = "// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @" by Tom
using System;

namespace MyTest.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	[Table(""Users"")]
	public class User {
		/// <summary>
		/// 主键
		/// </summary>
		[PrimaryKey]
		public int Id { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
	}
}";
			Assert.AreEqual(expected, result);

			result = template.Render(entity);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test21_var()
		{
			var template = new Template("hello ${name}!");
			Assert.ThrowsException<ScriptAnalyzingException>(() =>
			{
				var result = template.Render(null);
			});
		}

		[TestMethod]
		public void Test21_var_CompileAll()
		{
			var template = new Template("hello ${name}!");
			template.Options.CompileMode = ECompileMode.All;
			Assert.ThrowsException<ScriptAnalyzingException>(() =>
			{
				var result = template.Render(null);
			});
		}

		[TestMethod]
		public void Test21_var2()
		{
			var template = new Template("hello ${name}!");
			template.Options.ThrowIfVariableNotExists = false;
			var result = template.Render(null);
			Assert.AreEqual("hello !", result);
		}

		[TestMethod]
		public void Test21_var2_CompileAll()
		{
			var template = new Template("hello ${name}!");
			template.Options.ThrowIfVariableNotExists = false;
			template.Options.CompileMode = ECompileMode.All;
			var result = template.Render(null);
			Assert.AreEqual("hello !", result);
		}

		[TestMethod]
		public void Test21_var3()
		{
			var template = new Template("hello ${user?.name}!");
			template.Options.ThrowIfVariableNotExists = false;
			var result = template.Render(null);
			Assert.AreEqual("hello !", result);
		}

		[TestMethod]
		public void Test21_var3_CompileAll()
		{
			var template = new Template("hello ${user?.name}!");
			template.Options.ThrowIfVariableNotExists = false;
			template.Options.CompileMode = ECompileMode.All;
			var result = template.Render(null);
			Assert.AreEqual("hello !", result);
		}

		[TestMethod]
		public void Test22_empty()
		{
			var template = new Template("hello ${}!");
			var result = template.Render(null);
			Assert.AreEqual("hello !", result);
		}

		[TestMethod]
		public void Test22_empty_CompileAll()
		{
			var template = new Template("hello ${}!");
			template.Options.CompileMode = ECompileMode.All;
			var result = template.Render(null);
			Assert.AreEqual("hello !", result);
		}
	}
}
