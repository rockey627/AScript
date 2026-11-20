using AScript.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class TemplateTest
	{
		/// <summary>
		/// 执行模板
		/// </summary>
		private static string Translate(string template, ScriptContext context)
		{
			var script = new Script(context);
			var output = new StringBuilder();
			context.SetVar("out", output);
			var buildContext = new BuildContext();
			var analyzer = Script.DefaultSyntaxAnalyzer;
			var templateReader = new TemplateTokenReader(new DefaultTokenStream(template), false) { OutMethod = "Append" };
			var node = analyzer.Build(buildContext, context, script.Options, templateReader);
			var body = node.Build(buildContext, context, script.Options);
			var lambda = buildContext.Build(context, script.Options, new[] { body });
			lambda.Compile().DynamicInvoke(context);
			return output.ToString();
		}

		[TestMethod]
		public void Test01_Text()
		{
			var context = ScriptContext.Create();
			var result = Translate("Hello World!", context);
			Assert.AreEqual("Hello World!", result);
		}

		[TestMethod]
		public void Test02_Expression()
		{
			var context = ScriptContext.Create();
			context.SetVar("name", "Tom");
			var result = Translate("Hello ${name}, today is ${DateTime.Now.Year}", context);
			Assert.AreEqual($"Hello Tom, today is {DateTime.Now.Year}", result);
		}

		[TestMethod]
		public void Test03_ScriptBlock()
		{
			var context = ScriptContext.Create();
			context.SetVar("name", "Tom");
			var result = Translate("@{var userSex = name == 'Tom' ? '先生' : '女士';}尊敬的 ${name}${userSex} 您好！", context);
			Assert.AreEqual("尊敬的 Tom先生 您好！", result);
		}

		[TestMethod]
		public void Test04_Statement()
		{
			var context = ScriptContext.Create();
			context.SetVar("list", new List<string> { "a", "b" });
			var result = Translate("@foreach(var item in list) {\r\n\t[${item}]\r\n@}\r\nend", context);
			Assert.AreEqual("\t[a]\r\n\t[b]\r\nend", result);
		}

		[TestMethod]
		public void Test04_Statement2()
		{
			var context = ScriptContext.Create();
			context.SetVar("list", new List<string> { "a", "b" });
			var template = @"@foreach(var item in list) {
	[${item}]
@}
end";
			var result = Translate(template, context);
			Console.WriteLine(result);
			var expected = @"	[a]
	[b]
end";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void Test05_TemplateFile()
		{
			var context = ScriptContext.Create();
			context.SetVar("entity", new
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
			var template = File.ReadAllText("template_01.txt");
			var result = Translate(template, context);
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
