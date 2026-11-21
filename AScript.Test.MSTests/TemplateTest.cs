using AScript.Readers;
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
			template.CompileMode = ECompileMode.All;
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
			template.CompileMode = ECompileMode.All;
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
			template.CompileMode = ECompileMode.All;
			template.Context.SetVar("name", "Tom");
			var result = template.Render(null);
			Assert.AreEqual("尊敬的 Tom先生 您好！", result);
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
			template.CompileMode = ECompileMode.All;
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
			template.CompileMode = ECompileMode.All;
			template.Context.SetVar("list", new List<string> { "a", "b" });
			var result = template.Render(null);
			Console.WriteLine(result);
			var expected = @"	[a]
	[b]
end";
			Assert.AreEqual(expected, result);
		}

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
		public void Test05_TemplateFile_CompileAll()
		{
			var tpl = File.ReadAllText("template_01.txt");
			var template = new Template(tpl);
			template.CompileMode = ECompileMode.All;
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
		public void Test06_TemplateFile_CompileAll()
		{
			var template = new Template(new FileInfo("template_01.txt"));
			template.CompileMode = ECompileMode.All;
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

	}
}
