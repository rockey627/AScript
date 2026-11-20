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

		/// <summary>
		/// 模板上下文
		/// </summary>
		public ScriptContext Context { get; private set; }

		/// <summary>
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </summary>
		public int CacheTime { get; set; }
		/// <summary>
		/// 缓存key
		/// </summary>
		public string CacheKey { get; set; }
		/// <summary>
		/// 缓存版本号，如果版本号有变化，则重新编译并缓存
		/// </summary>
		public string CacheVersion { get; set; }

		/// <summary>
		/// 编译模式
		/// </summary>
		public ECompileMode? CompileMode { get; set; }

		/// <summary>
		/// 输出流变量名
		/// </summary>
		public string OutputVar { get; set; } = "output";
		/// <summary>
		/// 模型对象变量名
		/// </summary>
		public string ModelVar { get; set; } = "model";

		public Template(string template) : this(new ScriptContext(), template) { }
		public Template(FileInfo template) : this(new ScriptContext(), template) { }
		public Template(Stream template) : this(new ScriptContext(), template) { }
		public Template(Func<string> template) : this(new ScriptContext(), template) { }
		public Template(Func<Stream> template) : this(new ScriptContext(), template) { }
		public Template(ScriptContext context, string template)
		{
			this.Context = context;
			_template = template;
		}
		public Template(ScriptContext context, FileInfo template)
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

		/// <summary>
		/// 设置编译模式
		/// </summary>
		/// <param name="compileMode"></param>
		/// <returns></returns>
		public Template WithCompileMode(ECompileMode? compileMode)
		{
			this.CompileMode = compileMode;
			return this;
		}

		/// <summary>
		/// 设置缓存策略
		/// </summary>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">缓存key</param>
		/// <param name="cacheVersion">缓存版本号，如果版本号有变化，则重新编译并缓存</param>
		/// <returns></returns>
		public Template WithCache(int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			this.CacheTime = cacheTime;
			this.CacheKey = cacheKey;
			this.CacheVersion = cacheVersion;
			return this;
		}

		/// <summary>
		/// 渲染模板，返回结果
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public string Render(object model)
		{
			var output = new StringBuilder();
			Render(model, output);
			return output.ToString();
		}

		/// <summary>
		/// 渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public void Render(object model, StringBuilder output)
		{
			var script = new Script(new ScriptContext(this.Context));
			script.Options.CompileMode = this.CompileMode;
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream) { OutputVar = this.OutputVar, OutputMethod = "Append" };
			script.Context.AddAction<StringBuilder, object>("Write", (o, v) => o.Append(v));
			script.Context.AddAction<StringBuilder>("WriteLine", (o) => o.AppendLine());
			script.Context.AddAction<StringBuilder, object>("WriteLine", (o, v) => o.AppendLine(v?.ToString()));
			script.Context.SetVar(this.OutputVar, output);
			script.Context.SetVar(this.ModelVar, model);
			Eval(script);
		}

		/// <summary>
		/// 渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <param name="autoDisposeOutput"></param>
		public void Render(object model, TextWriter output, bool autoDisposeOutput = true)
		{
			try
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
			finally
			{
				if (autoDisposeOutput)
				{
					output.Dispose();
				}
			}
		}

		/// <summary>
		/// 异步渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <param name="autoDisposeOutput"></param>
		/// <returns></returns>
		public async Task RenderAsync(object model, TextWriter output, bool autoDisposeOutput = true)
		{
			try
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
				await EvalAsync(script).ConfigureAwait(false);
			}
			finally
			{
				if (autoDisposeOutput)
				{
					output.Dispose();
				}
			}
		}

		/// <summary>
		/// 渲染模板，输出到outputPath文件
		/// </summary>
		/// <param name="model"></param>
		/// <param name="outputPath"></param>
		/// <param name="encoding"></param>
		public void Render(object model, string outputPath, Encoding encoding = null)
		{
			Render(model, new StreamWriter(outputPath, false, encoding ?? Encoding.UTF8), true);
		}

		/// <summary>
		/// 渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <param name="encoding"></param>
		/// <param name="autoDisposeOutput"></param>
		public void Render(object model, Stream output, Encoding encoding = null, bool autoDisposeOutput = true)
		{
			try
			{
				Render(model, new StreamWriter(output, encoding ?? Encoding.UTF8), true);
			}
			finally
			{
				if (autoDisposeOutput)
				{
					output.Dispose();
				}
			}
		}

		/// <summary>
		/// 异步渲染模板，输出到outputPath文件
		/// </summary>
		/// <param name="model"></param>
		/// <param name="outputPath"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public Task RenderAsync(object model, string outputPath, Encoding encoding = null)
		{
			return RenderAsync(model, new StreamWriter(outputPath, false, encoding ?? Encoding.UTF8), true);
		}

		/// <summary>
		/// 异步渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <param name="encoding"></param>
		/// <param name="autoDisposeOutput"></param>
		/// <returns></returns>
		public async Task RenderAsync(object model, Stream output, Encoding encoding = null, bool autoDisposeOutput = true)
		{
			try
			{
				await RenderAsync(model, new StreamWriter(output, encoding ?? Encoding.UTF8), true).ConfigureAwait(false);
			}
			finally
			{
				if (autoDisposeOutput)
				{
					output.Dispose();
				}
			}
		}

		private void Eval(Script script)
		{
			if (_template is string s)
			{
				script.Eval(s, this.CacheTime, this.CacheKey, this.CacheVersion);
			}
			else if (_template is FileInfo fi)
			{
				script.EvalFile(fi.FullName, this.CacheTime, this.CacheKey, this.CacheVersion);
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
			else if (_template is FileInfo fi)
			{
				return script.EvalFileAsync(fi.FullName, this.CacheTime, this.CacheKey, this.CacheVersion);
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
