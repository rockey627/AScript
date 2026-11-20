using AScript.Readers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AScript
{
	public class Template
	{
		private readonly object _template;

		public ScriptContext Context { get; private set; }

		public int CacheTime { get; set; }
		public string CacheKey { get; set; }
		public string CacheVersion { get; set; }

		public ECompileMode? CompileMode { get; set; }

		public string OutputVar { get; set; } = "output";
		public string ModelVar { get; set; } = "model";

		public Template(string template) : this(new ScriptContext(), template) { }
		public Template(Stream template) : this(new ScriptContext(), template) { }
		public Template(Func<string> template) : this(new ScriptContext(), template) { }
		public Template(Func<Stream> template) : this(new ScriptContext(), template) { }
		public Template(ScriptContext context, string template)
		{
			this.Context = context;
			_template = template;
		}
		public Template(ScriptContext context, Stream template)
		{
			this.Context = context;
			_template = template;
		}
		public Template(ScriptContext context, Func<string> template)
		{
			this.Context = context;
			_template = template;
		}
		public Template(ScriptContext context, Func<Stream> template)
		{
			this.Context = context;
			_template = template;
		}

		public Template WithCompileMode(ECompileMode? compileMode)
		{
			this.CompileMode = compileMode;
			return this;
		}

		public Template WithCache(int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			this.CacheTime = cacheTime;
			this.CacheKey = cacheKey;
			this.CacheVersion = cacheVersion;
			return this;
		}

		public string Render(object model)
		{
			var script = new Script(new ScriptContext(this.Context));
			script.Options.CompileMode = this.CompileMode;
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream) { OutputVar = this.OutputVar, OutputMethod = "Append" };
			script.Context.AddAction<StringBuilder, object>("Write", (o, v) => o.Append(v));
			script.Context.AddAction<StringBuilder>("WriteLine", (o) => o.AppendLine());
			script.Context.AddAction<StringBuilder, object>("WriteLine", (o, v) => o.AppendLine(v?.ToString()));
			var output = new StringBuilder();
			script.Context.SetVar(this.OutputVar, output);
			script.Context.SetVar(this.ModelVar, model);
			Eval(script);
			return output.ToString();
		}

		public void Render(object model, TextWriter output)
		{
			var script = new Script(new ScriptContext(this.Context));
			script.Options.CompileMode = this.CompileMode;
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream) { OutputVar = this.OutputVar, OutputMethod = "Write" };
			script.Context.AddAction<TextWriter, object>("Append", (o, v) => o.Write(v));
			script.Context.AddAction<TextWriter>("AppendLine", (o) => o.WriteLine());
			script.Context.AddAction<TextWriter, object>("AppendLine", (o, v) => o.WriteLine(v?.ToString()));
			script.Context.SetVar(this.OutputVar, output);
			script.Context.SetVar(this.ModelVar, model);
			Eval(script);
		}

		public Task RenderAsync(object model, TextWriter output)
		{
			var script = new Script(new ScriptContext(this.Context));
			script.Options.CompileMode = this.CompileMode;
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream) { OutputVar = this.OutputVar, OutputMethod = "WriteAsync" };
			script.Context.AddAction<TextWriter, object>("Append", (o, v) => o.Write(v));
			script.Context.AddAction<TextWriter>("AppendLine", (o) => o.WriteLine());
			script.Context.AddAction<TextWriter, object>("AppendLine", (o, v) => o.WriteLine(v?.ToString()));
			script.Context.SetVar(this.OutputVar, output);
			script.Context.SetVar(this.ModelVar, model);
			return EvalAsync(script);
		}

		public void Render(object model, string outputPath, Encoding encoding = null)
		{
			using (var writer = new StreamWriter(outputPath, false, encoding ?? Encoding.UTF8))
			{
				Render(model, writer);
			}
		}

		public void Render(object model, Stream output, Encoding encoding = null)
		{
			using (var writer = new StreamWriter(output, encoding ?? Encoding.UTF8))
			{
				Render(model, writer);
			}
		}

		public Task RenderAsync(object model, string outputPath, Encoding encoding = null)
		{
			using (var writer = new StreamWriter(outputPath, false, encoding ?? Encoding.UTF8))
			{
				return RenderAsync(model, writer);
			}
		}

		public Task RenderAsync(object model, Stream output, Encoding encoding = null)
		{
			using (var writer = new StreamWriter(output, encoding ?? Encoding.UTF8))
			{
				return RenderAsync(model, writer);
			}
		}

		private void Eval(Script script)
		{
			if (_template is string s)
			{
				script.Eval(s, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else if (_template is Stream stream)
			{
				script.Eval(stream, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else if (_template is Func<string> stringFunc)
			{
				script.Eval(stringFunc, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else if (_template is Func<Stream> streamFunc)
			{
				script.Eval(streamFunc, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else
			{
				throw new NotSupportedException($"unsuport template of type '{_template.GetType()}'");
			}
		}

		private Task EvalAsync(Script script)
		{
			if (_template is string s)
			{
				return script.EvalAsync(s, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else if (_template is Stream stream)
			{
				return script.EvalAsync(stream, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else if (_template is Func<string> stringFunc)
			{
				return script.EvalAsync(stringFunc, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else if (_template is Func<Stream> streamFunc)
			{
				return script.EvalAsync(streamFunc, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else
			{
				throw new NotSupportedException($"unsuport template of type '{_template.GetType()}'");
			}
		}
	}
}
