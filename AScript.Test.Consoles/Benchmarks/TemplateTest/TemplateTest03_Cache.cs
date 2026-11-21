using BenchmarkDotNet.Attributes;
using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.Text;

namespace AScript.Test.Consoles.Benchmarks.TemplateTest
{
	[MaxColumn]
	[MinColumn]
	[MemoryDiagnoser]
	public class TemplateTest03_Cache
	{
		private static readonly string _Path = "./Benchmarks/TemplateTest/template_01.txt";
		private static readonly string _Path_JN = "./Benchmarks/TemplateTest/template_01_JN.txt";
		private static readonly string _result = "// created at " + DateTime.Now.ToString("yyyy-MM-dd") + @"
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

		static TemplateTest03_Cache()
		{
			// JNTemplate编译不缓存
			JinianNet.JNTemplate.Engine.Configure(options =>
			{
				options.Mode = EngineMode.Compiled;
				options.EnableCache = true;
				options.OutMode = OutMode.Auto;
			});
		}

		[Benchmark]
		public void Template()
		{
			var template = new Template(new FileInfo(_Path));
			template.WithCache(-1);
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
			var r = template.Render(null);
			if (r != _result)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void JNTemplate()
		{
			var template = JinianNet.JNTemplate.Engine.LoadTemplate(_Path_JN);
			template.Set("now", DateTime.Now.ToString("yyyy-MM-dd"));
			template.Set("columns", new[]
				{
					new { Name = "Id", TypeName = "int", Memo = "主键", IsPrimaryKey = true },
					new { Name = "Name", TypeName = "string", Memo = "姓名", IsPrimaryKey = false },
				});
			template.Set("entity", new
			{
				Name = "User",
				Memo = "用户",
				TableName = "Users"
			});
			var r = template.Render();
			//if (r != _result)
			//{
			//	throw new Exception("result error");
			//}
		}
	}
}
