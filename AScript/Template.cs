using AScript.Functions;
using AScript.Readers;
using AScript.TokenHandlers;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript
{
	public class Template
	{
		private static readonly MethodInfo Method_Output_Write = typeof(IOutput).GetMethod("Write", new[] { typeof(object) });
		private static readonly MethodInfo Method_Output_WriteAsync = typeof(IOutput).GetMethod("WriteAsync", new[] { typeof(object), typeof(CancellationToken) });
		private static readonly MethodInfo Method_Path_Combine = typeof(Path).GetMethod("Combine", new[] { typeof(string), typeof(string) });
		private static readonly MethodInfo Method_Template_Clone = typeof(Template).GetMethod("Clone", new[] { typeof(FileInfo) });
		private static readonly MethodInfo Method_TemplateRender_IOutput = typeof(Template).GetMethod("Render", new[] { typeof(object), typeof(IOutput) });

		public static readonly TemplateOptions DefaultOptions = new TemplateOptions
		{
			ThrowIfVariableNotExists = true,
			OutputVar = "output",
			ModelVar = "model",
			LoadMethod = "load",
			IncludeMethod = "include",
			// 文件版本号默认为文件修改时间
			CacheVersion = (tpl, cacheKey) => (tpl as FileInfo)?.LastWriteTime.ToFileTimeUtc().ToString()
		};

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
		/// <para>缓存版本号，如果版本号有变化，则重新编译并缓存</para>
		/// </summary>
		public object CacheVersion { get; set; }

		public TemplateOptions Options { get; private set; } = new TemplateOptions(DefaultOptions);

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
		/// 模板设置
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public Template WithOptions(Action<TemplateOptions> options)
		{
			options?.Invoke(this.Options);
			return this;
		}

		/// <summary>
		/// 设置缓存策略
		/// </summary>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>0表示不使用缓存</para>
		/// <para>-1表示永久缓存</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// <para>缓存key</para>
		/// <para>如果为空，则使用模板内容或模板文件路径作为key</para>
		/// </param>
		/// <returns></returns>
		/// <exception cref="Exception">stream or lazy template need a cacheKey</exception>
		public Template WithCache(int cacheTime, string cacheKey)
		{
			this.CacheTime = cacheTime;
			this.CacheKey = cacheKey;
			if (cacheTime != 0 && string.IsNullOrEmpty(cacheKey)
				&& !(_template is string || _template is FileInfo))
			{
				throw new Exception("stream or lazy template need a cacheKey");
			}
			return this;
		}

		/// <summary>
		/// 设置缓存策略
		/// </summary>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>0表示不使用缓存</para>
		/// <para>-1表示永久缓存</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// <para>缓存key</para>
		/// <para>如果为空，则使用模板内容或模板文件路径作为key</para>
		/// </param>
		/// <param name="cacheVersion">
		/// <para>缓存版本号，如果版本号有变化，则重新编译并缓存</para>
		/// <para>支持<c>Func&lt;string&gt;</c>动态获取版本号</para>
		/// <para>支持<c>Func&lt;string, string&gt;</c>根据cacheKey动态获取版本号</para>
		/// <para>如果模板是文件，则默认版本号为文件修改时间戳</para>
		/// </param>
		/// <returns></returns>
		/// <exception cref="Exception">stream or lazy template need a cacheKey</exception>
		public Template WithCache(int cacheTime, string cacheKey, object cacheVersion)
		{
			this.CacheTime = cacheTime;
			this.CacheKey = cacheKey;
			this.CacheVersion = cacheVersion;
			if (cacheTime != 0 && string.IsNullOrEmpty(cacheKey)
				&& !(_template is string || _template is FileInfo))
			{
				throw new Exception("stream or lazy template need a cacheKey");
			}
			return this;
		}

		/// <summary>
		/// 预编译模板
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="standalone">
		/// <para>编译结果是否隔离ScriptContext上下文</para>
		/// <para><c>false</c> - 上下文数据变化会影响模板渲染结果</para>
		/// <para><c>true</c> - 上下文数据变化不会影响模板渲染结果</para>
		/// </param>
		/// <returns></returns>
		public Template<TModel> Compile<TModel>(bool standalone = false)
		{
			Compile(typeof(TModel), standalone);
			return new Template<TModel>(this.Context, (Action<TModel, IOutput>)_compiled);
		}

		/// <summary>
		/// 预编译模板
		/// </summary>
		/// <param name="modelType"></param>
		/// <param name="standalone">
		/// <para>编译结果是否隔离ScriptContext上下文</para>
		/// <para><c>false</c> - 上下文数据变化会影响模板渲染结果</para>
		/// <para><c>true</c> - 上下文数据变化不会影响模板渲染结果</para>
		/// </param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException"></exception>
		public Template Compile(Type modelType, bool standalone = false)
		{
			var script = new Script(new ScriptContext(this.Context));
			script.Options.ThrowIfVariableNotExists = this.Options.ThrowIfVariableNotExists;
			script.Options.CompileMode = this.Options.CompileMode;
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream)
			{
				OutputVar = this.Options.OutputVar,
				OutputMethod = Method_Output_Write,
				IncludeMethod = this.Options.IncludeMethod,
				BaseDirectory = _template is FileInfo fi ? fi.DirectoryName : null,
			};
			script.Options.Standalone = standalone;
			// 兼容模板中的异步调用
			script.Context.AddFunc("await", AwaitFunction.Instance);
			script.Context.AddFunc<IOutput, object, Task>("WriteAsync", async (o, t) => o.Write(t));
			script.Context.AddFunc<IOutput, object, Task>("WriteLineAsync", async (o, t) => o.WriteLine(t));
			script.Context.AddTokenHandler("await", AwaitTokenHandler.Instance);
			script.Context.AddTokenHandler("@@CancellationToken", CancellationTokenHandler.Instance);
			// load(path)和load(path,model)
			script.Context.AddFunc("load", new LoadFunction(this));
			//
			if (modelType == null) modelType = typeof(object);
			if (_template is string s)
			{
				_compiled = script.Compile(s, new[] { this.Options.ModelVar, this.Options.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else if (_template is FileInfo fileInfo)
			{
				_compiled = script.Compile(fileInfo.OpenRead(), new[] { this.Options.ModelVar, this.Options.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else if (_template is Stream stream)
			{
				_compiled = script.Compile(stream, new[] { this.Options.ModelVar, this.Options.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else if (_template is Func<string> fs)
			{
				_compiled = script.Compile(fs(), new[] { this.Options.ModelVar, this.Options.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
			}
			else if (_template is Func<Stream> fstream)
			{
				_compiled = script.Compile(fstream(), new[] { this.Options.ModelVar, this.Options.OutputVar }, new Type[] { modelType, typeof(IOutput) }, typeof(void));
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
				var script = CreateScript(new ScriptContext(this.Context), model, output);
				// 兼容模板中的异步调用
				script.Context.AddFunc("await", AwaitFunction.Instance);
				script.Context.AddFunc<IOutput, object, Task>("WriteAsync", async (o, t) => o.Write(t));
				script.Context.AddFunc<IOutput, object, Task>("WriteLineAsync", async (o, t) => o.WriteLine(t));
				script.Context.AddTokenHandler("await", AwaitTokenHandler.Instance);
				script.Context.AddTokenHandler("@@CancellationToken", CancellationTokenHandler.Instance);
				// 渲染模板
				EvalTemplate(script);
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
		public async Task RenderAsync(object model, IOutput output, CancellationToken cancellationToken = default)
		{
			try
			{
				if (_compiled != null)
				{
					_compiled.DynamicInvoke(model, output);
					return;
				}
				var script = CreateScript(new ScriptContext(this.Context), model, output);
				// 异步支持
				script.Context.AddFunc("await", AwaitFunction.Instance);
				script.Context.AddFunc<IOutput, object, Task>("WriteAsync", (o, t) => o.WriteAsync(t));
				script.Context.AddFunc<IOutput, object, Task>("WriteLineAsync", (o, t) => o.WriteLineAsync(t));
				script.Context.AddTokenHandler("await", AwaitTokenHandler.Instance);
				script.Context.AddTokenHandler("@@CancellationToken", CancellationTokenHandler.Instance);
				// 异步渲染
				await EvalTemplateAsync(script, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				(output as IDisposable)?.Dispose();
			}
		}

		/// <summary>
		/// 创建脚本，并注册模板的内置函数
		/// </summary>
		/// <param name="context">脚本上下文</param>
		/// <param name="model">模型对象</param>
		/// <param name="output">输出流</param>
		/// <returns></returns>
		private Script CreateScript(ScriptContext context, object model, IOutput output)
		{
			var script = new Script(context);
			script.Options.ThrowIfVariableNotExists = this.Options.ThrowIfVariableNotExists;
			script.Options.CompileMode = this.Options.CompileMode;
			script.Options.RewriteFunctions = false;
			script.Options.RewriteVariables = false;
			script.Options.TokenReaderFunc = (stream, autoDisposeStream) => new TemplateTokenReader(stream, autoDisposeStream)
			{
				Output = this.CacheTime == 0 ? output : null,
				OutputVar = this.Options.OutputVar,
				OutputMethod = Method_Output_Write,
				OutputMethodAsync = Method_Output_WriteAsync,
				IncludeMethod = this.Options.IncludeMethod,
				BaseDirectory = _template is FileInfo fi ? fi.DirectoryName : null,
			};
			script.Context.SetVar(this.Options.OutputVar, output);
			script.Context.SetVar(this.Options.ModelVar, model);
			// load(path)和load(path,model)：加载模板文件并单独渲染
			script.Context.AddAction<string>(this.Options.LoadMethod, path =>
			{
				this.Clone(this.ResolveFile(path)).Render(model, output);
			});
			script.Context.AddAction<string, object>(this.Options.LoadMethod, (path, m) =>
			{
				this.Clone(this.ResolveFile(path)).Render(m, output);
			});
			// include(path) 由 TemplateTokenReader 在读取阶段展开，无需运行时函数
			return script;
		}

		/// <summary>
		/// 解析模板文件的路径，相对路径以当前模板文件所在目录为基准
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private FileInfo ResolveFile(string path)
		{
			if (_template is FileInfo fi && !string.IsNullOrEmpty(fi.DirectoryName))
			{
				path = Path.Combine(fi.DirectoryName, path);
			}
			return new FileInfo(path);
		}

		/// <summary>
		/// 执行模板
		/// </summary>
		/// <param name="script"></param>
		private void EvalTemplate(Script script)
		{
			var cacheTime = this.CacheTime;
			var cacheKey = this.CacheKey;
			string cacheVersion = null;
			if (cacheTime != 0)
			{
				if (string.IsNullOrEmpty(cacheKey) && _template is string st)
				{
					cacheKey = st;
				}
				if (this.CacheVersion is Func<string> cvf)
				{
					cacheVersion = cvf();
				}
				else if (this.CacheVersion is Func<string, string> cvfk)
				{
					cacheVersion = cvfk(cacheKey);
				}
				else
				{
					cacheVersion = this.CacheVersion?.ToString();
				}
				if (string.IsNullOrEmpty(cacheVersion))
				{
					cacheVersion = this.Options.CacheVersion?.Invoke(_template, cacheKey);
				}
			}
			if (_template is string s)
			{
				script.Eval(s, cacheTime, cacheKey, cacheVersion);
			}
			else if (_template is FileInfo fi)
			{
				script.EvalFile(fi.FullName, cacheTime, cacheKey, cacheVersion);
			}
			else if (_template is Stream stream)
			{
				script.Eval(stream, cacheTime, cacheKey, cacheVersion);
			}
			else if (_template is Func<string> stringFunc)
			{
				script.Eval(stringFunc, cacheTime, cacheKey, cacheVersion);
			}
			else if (_template is Func<Stream> streamFunc)
			{
				script.Eval(streamFunc, cacheTime, cacheKey, cacheVersion);
			}
			else
			{
				throw new NotSupportedException($"unsuport template of type '{_template.GetType()}'");
			}
		}

		/// <summary>
		/// 异步执行模板
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		private Task EvalTemplateAsync(Script script, CancellationToken cancellationToken)
		{
			var cacheTime = this.CacheTime;
			var cacheKey = this.CacheKey;
			string cacheVersion = null;
			if (cacheTime != 0)
			{
				if (string.IsNullOrEmpty(cacheKey) && _template is string st)
				{
					cacheKey = st;
				}
				if (this.CacheVersion is Func<string> cvf)
				{
					cacheVersion = cvf();
				}
				else if (this.CacheVersion is Func<string, string> cvfk)
				{
					cacheVersion = cvfk(cacheKey);
				}
				else
				{
					cacheVersion = this.CacheVersion?.ToString();
				}
				if (string.IsNullOrEmpty(cacheVersion))
				{
					cacheVersion = this.Options.CacheVersion?.Invoke(_template, cacheKey);
				}
			}
			if (_template is string s)
			{
				return script.EvalAsync(s, cacheTime, cacheKey, cacheVersion, cancellationToken);
			}
			else if (_template is FileInfo fi)
			{
				return script.EvalFileAsync(fi.FullName, cacheTime, cacheKey, cacheVersion, cancellationToken);
			}
			else if (_template is Stream stream)
			{
				return script.EvalAsync(stream, cacheTime, cacheKey, cacheVersion, cancellationToken);
			}
			else if (_template is Func<string> stringFunc)
			{
				return script.EvalAsync(stringFunc, cacheTime, cacheKey, cacheVersion, cancellationToken);
			}
			else if (_template is Func<Stream> streamFunc)
			{
				return script.EvalAsync(streamFunc, cacheTime, cacheKey, cacheVersion, cancellationToken);
			}
			else
			{
				throw new NotSupportedException($"unsuport template of type '{_template.GetType()}'");
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
		/// 渲染模板，返回结果
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<string> RenderAsync(object model, CancellationToken cancellationToken = default)
		{
			var output = new StringBuilder();
			await RenderAsync(model, output, cancellationToken).ConfigureAwait(false);
			return output.ToString();
		}

		/// <summary>
		/// 渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public Task RenderAsync(object model, StringBuilder output, CancellationToken cancellationToken = default)
		{
			return RenderAsync(model, new StringBuilderOutput(output), cancellationToken);
		}

		/// <summary>
		/// 异步渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <param name="autoDisposeOutput"></param>
		public Task RenderAsync(object model, TextWriter output, bool autoDisposeOutput = true, CancellationToken cancellationToken = default)
		{
			return RenderAsync(model, new TextWriterOutput(output, autoDisposeOutput), cancellationToken);
		}

		/// <summary>
		/// 异步渲染模板，输出到outputPath文件
		/// </summary>
		/// <param name="model"></param>
		/// <param name="outputPath"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public Task RenderAsync(object model, string outputPath, Encoding encoding = null, CancellationToken cancellationToken = default)
		{
			return RenderAsync(model, new StreamWriter(outputPath, false, encoding ?? Encoding.UTF8), true, cancellationToken);
		}

		/// <summary>
		/// 异步渲染模板，输出到output
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		/// <param name="encoding"></param>
		/// <param name="autoDisposeOutput"></param>
		/// <returns></returns>
		public Task RenderAsync(object model, Stream output, Encoding encoding = null, bool autoDisposeOutput = true, CancellationToken cancellationToken = default)
		{
			return RenderAsync(model, new StreamWriter(output, encoding ?? Encoding.UTF8), autoDisposeOutput, cancellationToken);
		}

		public Template Clone(FileInfo template)
		{
			return new Template(this.Context, template)
			{
				CacheTime = this.CacheTime,
				Options = this.Options
			};
		}

		public interface IOutput
		{
			void Write(object value);
			void WriteLine();
			void WriteLine(object value);
			Task WriteAsync(object value, CancellationToken cancellationToken = default);
			Task WriteLineAsync(CancellationToken cancellationToken = default);
			Task WriteLineAsync(object value, CancellationToken cancellationToken = default);
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

			public async Task WriteAsync(object value, CancellationToken cancellationToken = default)
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

			public async Task WriteLineAsync(CancellationToken cancellationToken = default)
			{
				_builder.AppendLine();
			}

			public async Task WriteLineAsync(object value, CancellationToken cancellationToken = default)
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

			public Task WriteAsync(object value, CancellationToken cancellationToken = default)
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

			public Task WriteLineAsync(CancellationToken cancellationToken = default)
			{
				return _writer.WriteLineAsync();
			}

			public Task WriteLineAsync(object value, CancellationToken cancellationToken = default)
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

		private class LoadFunction : IFunctionBuilder, IFunctionEvaluator
		{
			private readonly Template _template;

			public LoadFunction(Template template)
			{
				_template = template;
			}

			public void Build(FunctionBuildArgs e)
			{
				if (e.Args == null || e.Args.Count != 1 && e.Args.Count != 2) return;
				var path = e.BuildArgs(0);
				if (_template._template is FileInfo fi)
				{
					path = Expression.Call(null, Method_Path_Combine, Expression.Constant(fi.DirectoryName), path);
				}
				var fileInfoConstruct = typeof(FileInfo).GetConstructor(new[] { typeof(string) });
				var fileInfo = Expression.New(fileInfoConstruct, path);
				var template = Expression.Call(Expression.Constant(_template), Method_Template_Clone, fileInfo);
				Expression output = e.BuildContext.Parameters[_template.Options.OutputVar];
				Expression model;
				if (e.Args.Count == 1)
				{
					model = e.BuildContext.Parameters[_template.Options.ModelVar];
				}
				else
				{
					model = e.BuildArgs(1);
				}
				e.Result = Expression.Call(template, Method_TemplateRender_IOutput, model, output);
			}

			public void Eval(FunctionEvalArgs e)
			{
				throw new NotImplementedException();
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
