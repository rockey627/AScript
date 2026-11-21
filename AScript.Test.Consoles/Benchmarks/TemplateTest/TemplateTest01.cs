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
	public class TemplateTest01
	{
		private static readonly string _template_text = "Hello ${name1}, my name is ${name2}";
		private static readonly string _result = "Hello Tom, my name is John";

		static TemplateTest01()
		{
			// JNTemplate默认为编译缓存，手动设置为解释执行
			JinianNet.JNTemplate.Engine.Configure(options =>
			{
				options.Mode = EngineMode.Interpreted;
				options.EnableCache = false;
			});
		}

		[Benchmark]
		public void Translate()
		{
			var script = new Script();
			script.Context.SetVar("name1", "Tom");
			script.Context.SetVar("name2", "John");
			var r = script.Translate(_template_text);
			if (r != _result)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void Template()
		{
			var template = new Template(_template_text);
			template.Context.SetVar("name1", "Tom");
			template.Context.SetVar("name2", "John");
			var r = template.Render(null);
			if (r != _result)
			{
				throw new Exception("result error");
			}
		}

		[Benchmark]
		public void JNTemplate()
		{
			var template = JinianNet.JNTemplate.Engine.CreateTemplate(_template_text);
			template.Set("name1", "Tom");
			template.Set("name2", "John");
			var r = template.Render();
			if (r != _result)
			{
				throw new Exception("result error");
			}
		}
	}
}
