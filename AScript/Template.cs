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

		private Delegate _compiled;

		/// <summary>
		/// 模板上下文
		/// </summary>
		public ScriptContext Context { get; private set; }

		public bool IsCompiled => _compiled != null;

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

		public Template<TModel> Compile<TModel>(bool standalone = false)
		{
			Compile(typeof(TModel), standalone);
			return new Template<TModel>(this.Context, (Action<TModel, IOutput>)_compiled);
		}

		public Template Compile(Type modelType, bool standalone = false)
		{
			var script = new Script(new ScriptContext(this.Context));
			script.Options.CompileMode = this.CompileMode;
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream) { OutputVar = this.OutputVar, OutputMethod = "Write" };
			script.Options.Standalone = standalone;
			if (modelType == null) modelType = typeof(object);
			if (_template is string s)
			{
				_compiled = script.Compile(s, new[] { this.ModelVar, this.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else if (_template is FileInfo fileInfo)
			{
				_compiled = script.Compile(fileInfo.OpenRead(), new[] { this.ModelVar, this.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else if (_template is Stream stream)
			{
				_compiled = script.Compile(stream, new[] { this.ModelVar, this.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else if (_template is Func<string> fs)
			{
				_compiled = script.Compile(fs(), new[] { this.ModelVar, this.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else if (_template is Func<Stream> fstream)
			{
				_compiled = script.Compile(fstream(), new[] { this.ModelVar, this.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else
			{
				throw new NotSupportedException($"unsuport template of type '{_template.GetType()}'");
			}
			return this;
		}

		/// <summary>
		/// 渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public void Render(object model, IOutput output)
		{
			try
			{
				if (_compiled != null)
				{
					_compiled.DynamicInvoke(model, output);
					return;
				}
				var script = new Script(new ScriptContext(this.Context));
				script.Options.CompileMode = this.CompileMode;
				script.Options.RewriteFunctions = false;
				script.Options.RewriteVariables = false;
				script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream) { OutputVar = this.OutputVar, OutputMethod = "Write" };
				script.Context.SetVar(this.OutputVar, output);
				script.Context.SetVar(this.ModelVar, model);
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
			finally
			{
				(output as IDisposable)?.Dispose();
			}
		}

		/// <summary>
		/// 异步渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public async Task RenderAsync(object model, IOutput output)
		{
			try
			{
				if (_compiled != null)
				{
					_compiled.DynamicInvoke(model, output);
					return;
				}
				var script = new Script(new ScriptContext(this.Context));
				script.Options.CompileMode = this.CompileMode;
				script.Options.RewriteFunctions = false;
				script.Options.RewriteVariables = false;
				script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream) { OutputVar = this.OutputVar, OutputMethod = "WriteAsync" };
				script.Context.SetVar(this.OutputVar, output);
				script.Context.SetVar(this.ModelVar, model);
				if (_template is string s)
				{
					await script.EvalAsync(s, this.CacheTime, this.CacheKey, this.CacheVersion).ConfigureAwait(false);
				}
				else if (_template is FileInfo fi)
				{
					await script.EvalFileAsync(fi.FullName, this.CacheTime, this.CacheKey, this.CacheVersion).ConfigureAwait(false);
				}
				else if (_template is Stream stream)
				{
					await script.EvalAsync(stream, this.CacheTime, this.CacheKey, this.CacheVersion).ConfigureAwait(false);
				}
				else if (_template is Func<string> stringFunc)
				{
					await script.EvalAsync(stringFunc, this.CacheTime, this.CacheKey, this.CacheVersion).ConfigureAwait(false);
				}
				else if (_template is Func<Stream> streamFunc)
				{
					await script.EvalAsync(streamFunc, this.CacheTime, this.CacheKey, this.CacheVersion).ConfigureAwait(false);
				}
				else
				{
					throw new NotSupportedException($"unsuport template of type '{_template.GetType()}'");
				}
			}
			finally
			{
				(output as IDisposable)?.Dispose();
			}
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
			Render(model, new StringBuilderOutput(output));
		}

		/// <summary>
		/// 渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <param name="autoDisposeOutput"></param>
		public void Render(object model, TextWriter output, bool autoDisposeOutput = true)
		{
			Render(model, new TextWriterOutput(output, autoDisposeOutput));
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
			Render(model, new StreamWriter(output, encoding ?? Encoding.UTF8), autoDisposeOutput);
		}

		/// <summary>
		/// 异步渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <param name="autoDisposeOutput"></param>
		public Task RenderAsync(object model, TextWriter output, bool autoDisposeOutput = true)
		{
			return RenderAsync(model, new TextWriterOutput(output, autoDisposeOutput));
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
		public Task RenderAsync(object model, Stream output, Encoding encoding = null, bool autoDisposeOutput = true)
		{
			return RenderAsync(model, new StreamWriter(output, encoding ?? Encoding.UTF8), autoDisposeOutput);
		}

		public interface IOutput
		{
			void Write(object value);
			void WriteLine();
			void WriteLine(object value);
			Task WriteAsync(object value);
			Task WriteLineAsync();
			Task WriteLineAsync(object value);
		}

		public class StringBuilderOutput : IOutput
		{
			private readonly StringBuilder _builder;

			public StringBuilderOutput(StringBuilder sb)
			{
				_builder = sb;
			}

			public void Write(object value)
			{
				_builder.Append(value);
			}

			public async Task WriteAsync(object value)
			{
				_builder.Append(value);
			}

			public void WriteLine()
			{
				_builder.AppendLine();
			}

			public void WriteLine(object value)
			{
				_builder.AppendLine(value?.ToString());
			}

			public async Task WriteLineAsync()
			{
				_builder.AppendLine();
			}

			public async Task WriteLineAsync(object value)
			{
				_builder.AppendLine(value?.ToString());
			}
		}

		public class TextWriterOutput : IOutput, IDisposable
		{
			private readonly TextWriter _writer;
			private readonly bool _owner;

			public TextWriterOutput(TextWriter writer, bool owner)
			{
				_writer = writer;
				_owner = owner;
			}

			public void Write(object value)
			{
				_writer.Write(value);
			}

			public Task WriteAsync(object value)
			{
				return _writer.WriteAsync(value?.ToString());
			}

			public void WriteLine()
			{
				_writer.WriteLine();
			}

			public void WriteLine(object value)
			{
				_writer.WriteLine(value);
			}

			public Task WriteLineAsync()
			{
				return _writer.WriteLineAsync();
			}

			public Task WriteLineAsync(object value)
			{
				return _writer.WriteLineAsync(value?.ToString());
			}

			public void Dispose()
			{
				if (_owner)
				{
					_writer.Dispose();
				}
			}
		}
	}

	public class Template<TModel>
	{
		private readonly Action<TModel, Template.IOutput> _compiled;

		public ScriptContext Context { get; private set; }

		public Template(ScriptContext context, Action<TModel, Template.IOutput> compiled)
		{
			this.Context = context;
			this._compiled = compiled;
		}

		/// <summary>
		/// 渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public void Render(TModel model, Template.IOutput output)
		{
			try
			{
				_compiled(model, output);
			}
			finally
			{
				(output as IDisposable)?.Dispose();
			}
		}

		/// <summary>
		/// 渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public void Render(TModel model, StringBuilder output)
		{
			Render(model, new Template.StringBuilderOutput(output));
		}

		/// <summary>
		/// 渲染模板，返回结果
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public string Render(TModel model)
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
		/// <param name="autoDisposeOutput"></param>
		public void Render(TModel model, TextWriter output, bool autoDisposeOutput = true)
		{
			Render(model, new Template.TextWriterOutput(output, autoDisposeOutput));
		}

		/// <summary>
		/// 渲染模板，输出到outputPath文件
		/// </summary>
		/// <param name="model"></param>
		/// <param name="outputPath"></param>
		/// <param name="encoding"></param>
		public void Render(TModel model, string outputPath, Encoding encoding = null)
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
		public void Render(TModel model, Stream output, Encoding encoding = null, bool autoDisposeOutput = true)
		{
			Render(model, new StreamWriter(output, encoding ?? Encoding.UTF8), autoDisposeOutput);
		}
	}
}
