using AScript.Lang.CSharp;
using System;
using System.IO;
using AScript.Nodes;
using System.Linq.Expressions;
using AScript.Readers;
using System.Threading.Tasks;
using System.Threading;

namespace AScript
{
	/// <summary>
	/// 脚本执行（非线程安全）
	/// </summary>
	public class Script
	{
		/// <summary>
		/// 脚本语言列表
		/// </summary>
		public static readonly ScriptLangCollection Langs = new ScriptLangCollection();

		/// <summary>
		/// 默认词法分析器
		/// </summary>
		public static ILexicalAnalyzer DefaultLexicalAnalyzer = Lexicals.DefaultLexicalAnalyzer.Instance;
		/// <summary>
		/// 默认语法分析器
		/// </summary>
		public static ISyntaxAnalyzer DefaultSyntaxAnalyzer = Syntaxs.DefaultSyntaxAnalyzer.Instance;

		/// <summary>
		/// 默认编译选项
		/// </summary>
		public static readonly BuildOptions DefaultOptions = new BuildOptions { ThrowIfVariableNotExists = true };

		/// <summary>
		/// 缓存
		/// </summary>
		public static readonly Cache<Delegate> Cache = new Cache<Delegate>();

		/// <summary>
		/// BuildOptions.Standalone为true时，编译结果使用该缓存
		/// </summary>
		public static readonly Cache<Delegate> StandaloneCache = new Cache<Delegate>();

		/// <summary>
		/// 匿名类型管理
		/// </summary>
		public static readonly AnonymousTypeManager AnonymousTypes = new AnonymousTypeManager();

		/// <summary>
		/// 上下文
		/// </summary>
		public ScriptContext Context { get; set; }

		/// <summary>
		/// 编译选项
		/// </summary>
		public BuildOptions Options { get; private set; } = new BuildOptions(DefaultOptions);

		static Script()
		{
			Langs.Set("CSharp", CSharpLang.Instance, true);
		}

		/// <summary>
		/// 
		/// </summary>
		public Script() : this(ScriptContext.Create()) { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public Script(ScriptContext context)
		{
			this.Context = context;
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public object Eval(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return Eval(null, this.Context, this.Options, expression, cacheTime, cacheKey, cacheVersion);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<EvalResult> EvalAsync(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			return EvalAsync(null, this.Context, this.Options, expression, cacheTime, cacheKey, cacheVersion, cancellationToken);
		}

		/// <summary>
		/// 计算表达式，返回结果和类型（结果可能为null，此时returnType可以判断返回类型）
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="returnType"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public object Eval(string expression, out Type returnType, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return Eval(null, this.Context, this.Options, expression, out returnType, cacheTime, cacheKey, cacheVersion);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public T Eval<T>(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (string.IsNullOrEmpty(expression)) return default;
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
				return func(this.Context);
			}
			return (T)Eval(this.Context, this.Options, expression, out _);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<T> EvalAsync<T>(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(expression)) return default;
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = await CompileGlobalAsync<T>(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
				return func(this.Context);
			}
			return (T)(await EvalAsync(this.Context, this.Options, expression, cancellationToken).ConfigureAwait(false)).Value;
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public object Eval(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return Eval(expression, out _, cacheTime, cacheKey, cacheVersion);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<EvalResult> EvalAsync(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null || expression.Length == 0L)
			{
				return default;
			}
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey)
				|| (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = await CompileGlobalAsync(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
				var value = (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
				return new EvalResult(value, func.Method.ReturnType);
			}
			return await EvalAsync(this.Context, this.Options, expression, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// 计算表达式，返回结果和类型（结果可能为null，此时returnType可以判断返回类型）
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="returnType"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public object Eval(Stream expression, out Type returnType, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null || expression.Length == 0L)
			{
				returnType = null;
				return null;
			}
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey)
				|| (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
				returnType = func.Method.ReturnType;
				return (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
			}
			return Eval(this.Context, this.Options, expression, out returnType);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime"></param>
		/// <param name="cacheKey"></param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public T Eval<T>(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null || expression.Length == 0L)
			{
				return default;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
				return func(this.Context);
			}
			return (T)Eval(this.Context, this.Options, expression, out _);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime"></param>
		/// <param name="cacheKey"></param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<T> EvalAsync<T>(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null || expression.Length == 0L)
			{
				return default;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = await CompileGlobalAsync<T>(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
				return func(this.Context);
			}
			return (T)(await EvalAsync(this.Context, this.Options, expression, cancellationToken).ConfigureAwait(false)).Value;
		}

		public object Eval(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return Eval(expression, out _, cacheTime, cacheKey, cacheVersion);
		}

		public object Eval(Func<string> expression, out Type returnType, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null)
			{
				returnType = null;
				return null;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
				returnType = func.Method.ReturnType;
				return (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
			}
			return Eval(this.Context, this.Options, expression(), out returnType);
		}

		public async Task<EvalResult> EvalAsync(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null)
			{
				return default;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = await CompileGlobalAsync(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
				var value = (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
				return new EvalResult(value, func.Method.ReturnType);
			}
			return await EvalAsync(this.Context, this.Options, expression(), cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public T Eval<T>(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null)
			{
				return default;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
				return func(this.Context);
			}
			return (T)Eval(this.Context, this.Options, expression(), out _);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<T> EvalAsync<T>(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null)
			{
				return default;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = await CompileGlobalAsync<T>(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
				return func(this.Context);
			}
			return (T)(await EvalAsync(this.Context, this.Options, expression(), cancellationToken).ConfigureAwait(false)).Value;
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public object Eval(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return Eval(expression, out _, cacheTime, cacheKey, cacheVersion);
		}

		/// <summary>
		/// 计算表达式，返回结果和类型（结果可能为null，此时returnType可以判断返回类型）
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="returnType"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public object Eval(Func<Stream> expression, out Type returnType, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null)
			{
				returnType = null;
				return null;
			}
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey)
				|| (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
				returnType = func.Method.ReturnType;
				return (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
			}
			return Eval(this.Context, this.Options, expression(), out returnType);
		}

		public async Task<EvalResult> EvalAsync(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null)
			{
				return default;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = await CompileGlobalAsync(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
				var value = (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
				return new EvalResult(value, func.Method.ReturnType);
			}
			return await EvalAsync(this.Context, this.Options, expression(), cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime"></param>
		/// <param name="cacheKey"></param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public T Eval<T>(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null)
			{
				return default;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
				return func(this.Context);
			}
			return (T)Eval(this.Context, this.Options, expression(), out _);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<T> EvalAsync<T>(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null)
			{
				return default;
			}
			if (cacheTime != 0 || (this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = await CompileGlobalAsync<T>(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
				return func(this.Context);
			}
			return (T)(await EvalAsync(this.Context, this.Options, expression(), cancellationToken).ConfigureAwait(false)).Value;
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="compileMode"></param>
		/// <returns></returns>
		public object Eval(string expression, ECompileMode compileMode)
		{
			return Eval(expression, out _, compileMode);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="returnType"></param>
		/// <param name="compileMode"></param>
		/// <returns></returns>
		public object Eval(string expression, out Type returnType, ECompileMode compileMode)
		{
			if (string.IsNullOrEmpty(expression))
			{
				returnType = null;
				return null;
			}
			if (compileMode == ECompileMode.All)
			{
				var func = CompileGlobal(expression);
				returnType = func.Method.ReturnType;
				return (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return Eval(this.Context, options, expression, out returnType);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="compileMode"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<EvalResult> EvalAsync(string expression, ECompileMode compileMode, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return default;
			}
			if (compileMode == ECompileMode.All)
			{
				var func = await CompileGlobalAsync(expression, cancellationToken: cancellationToken).ConfigureAwait(false);
				var value = (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
				return new EvalResult(value, func.Method.ReturnType);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return await EvalAsync(this.Context, options, expression, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="compileMode"></param>
		/// <returns></returns>
		public T Eval<T>(string expression, ECompileMode compileMode)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return default;
			}
			if (compileMode == ECompileMode.All)
			{
				Compile<T>(expression);
				var func = CompileGlobal<T>(expression);
				return func(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return (T)Eval(this.Context, options, expression, out _);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="compileMode"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<T> EvalAsync<T>(string expression, ECompileMode compileMode, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return default;
			}
			if (compileMode == ECompileMode.All)
			{
				var func = await CompileGlobalAsync<T>(expression, cancellationToken: cancellationToken).ConfigureAwait(false);
				return func(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return (T)(await EvalAsync(this.Context, options, expression, cancellationToken).ConfigureAwait(false)).Value;
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="compileMode"></param>
		/// <returns></returns>
		public object Eval(Stream expression, ECompileMode compileMode)
		{
			return Eval(expression, out _, compileMode);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="returnType"></param>
		/// <param name="compileMode"></param>
		/// <returns></returns>
		public object Eval(Stream expression, out Type returnType, ECompileMode compileMode)
		{
			if (expression == null || expression.Length == 0L)
			{
				returnType = null;
				return null;
			}
			if (compileMode == ECompileMode.All)
			{
				var func = CompileGlobal(expression);
				returnType = func.Method.ReturnType;
				return (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return Eval(this.Context, options, expression, out returnType);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="compileMode"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<EvalResult> EvalAsync(Stream expression, ECompileMode compileMode, CancellationToken cancellationToken = default)
		{
			if (expression == null || expression.Length == 0L)
			{
				return default;
			}
			if (compileMode == ECompileMode.All)
			{
				var func = await CompileGlobalAsync(expression, cancellationToken: cancellationToken).ConfigureAwait(false);
				var value = (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
				return new EvalResult(value, func.Method.ReturnType);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return await EvalAsync(this.Context, options, expression, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="compileMode"></param>
		/// <returns></returns>
		public T Eval<T>(Stream expression, ECompileMode compileMode)
		{
			if (expression == null || expression.Length == 0L)
			{
				return default;
			}
			if (compileMode == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression);
				return func(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return (T)Eval(this.Context, options, expression, out _);
		}

		/// <summary>
		/// 异步计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="compileMode"></param>
		/// <returns></returns>
		public async Task<T> EvalAsync<T>(Stream expression, ECompileMode compileMode, CancellationToken cancellationToken = default)
		{
			if (expression == null || expression.Length == 0L)
			{
				return default;
			}
			if (compileMode == ECompileMode.All)
			{
				var func = await CompileGlobalAsync<T>(expression, cancellationToken: cancellationToken).ConfigureAwait(false);
				return func(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return (T)(await EvalAsync(this.Context, options, expression, cancellationToken).ConfigureAwait(false)).Value;
		}

		/// <summary>
		/// 计算表达式树，返回结果
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public object Eval(ITreeNode node)
		{
			return Eval(node, out _);
		}

		/// <summary>
		/// 计算表达式树，返回结果和类型
		/// </summary>
		/// <param name="node"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		public object Eval(ITreeNode node, out Type returnType)
		{
			if (node == null)
			{
				returnType = null;
				return null;
			}
			if ((this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal(node);
				returnType = func.Method.ReturnType;
				return (this.Options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(this.Context);
			}
			return node.Eval(this.Context, this.Options, new EvalControl(), out returnType);
		}

		/// <summary>
		/// 计算表达式树，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node"></param>
		/// <returns></returns>
		public T Eval<T>(ITreeNode node)
		{
			if (node == null)
			{
				return default;
			}
			if ((this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal<T>(node);
				return func(this.Context);
			}
			return (T)node.Eval(this.Context, this.Options, new EvalControl(), out _);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="tokenStream"></param>
		/// <returns></returns>
		public object Eval(ITokenStream tokenStream)
		{
			return Eval(tokenStream, out _);
		}

		/// <summary>
		/// 计算表达式，返回结果和类型
		/// </summary>
		/// <param name="tokenStream"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		public object Eval(ITokenStream tokenStream, out Type returnType)
		{
			return Eval(this.Context, this.Options, tokenStream, out returnType);
		}

		/// <summary>
		/// 异步计算表达式，返回结果和类型
		/// </summary>
		/// <param name="tokenStream"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<EvalResult> EvalAsync(ITokenStream tokenStream, CancellationToken cancellationToken = default)
		{
			return EvalAsync(this.Context, this.Options, tokenStream, cancellationToken);
		}

		public Delegate CompileGlobal(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return CompileGlobal(null, this.Context, this.Options, expression, cacheTime, cacheKey, cacheVersion);
		}

		public Task<Delegate> CompileGlobalAsync(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			return CompileGlobalAsync(null, this.Context, this.Options, expression, cacheTime, cacheKey, cacheVersion, cancellationToken);
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Delegate CompileGlobal(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return CompileGlobal(null, this.Context, this.Options, expression, cacheTime, cacheKey, cacheVersion);
		}

		public Task<Delegate> CompileGlobalAsync(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			return CompileGlobalAsync(null, this.Context, this.Options, expression, cacheTime, cacheKey, cacheVersion, cancellationToken);
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Delegate CompileGlobal(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null) return null;

			string s = null;
			Cache<Delegate> cache = null;
			if (cacheTime != 0)
			{
				if (string.IsNullOrEmpty(cacheKey))
				{
					s = expression();
					if (string.IsNullOrEmpty(s)) return null;
					cacheKey = s;
				}
				cache = (this.Options.Standalone ?? false) ? StandaloneCache : Cache;
				if (cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

			var func = Compile(null, this.Context, this.Options, s ?? expression());

			if (cacheTime != 0)
			{
				cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
			}

			return func;
		}

		/// <summary>
		/// 异步编译生成委托
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Delegate> CompileGlobalAsync(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null) return null;

			string s = null;
			Cache<Delegate> cache = null;
			if (cacheTime != 0)
			{
				if (string.IsNullOrEmpty(cacheKey))
				{
					s = expression();
					if (string.IsNullOrEmpty(s)) return null;
					cacheKey = s;
				}
				cache = (this.Options.Standalone ?? false) ? StandaloneCache : Cache;
				if (cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

			var func = await CompileAsync(null, this.Context, this.Options, s ?? expression(), cancellationToken).ConfigureAwait(false);

			if (cacheTime != 0)
			{
				cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
			}

			return func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Delegate CompileGlobal(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null) return null;

			Cache<Delegate> cache = null;
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				cache = (this.Options.Standalone ?? false) ? StandaloneCache : Cache;
				if (cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

			var func = Compile(null, this.Context, this.Options, expression());

			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
			}

			return func;
		}

		/// <summary>
		/// 异步编译生成委托
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Delegate> CompileGlobalAsync(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null) return null;

			Cache<Delegate> cache = null;
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				cache = (this.Options.Standalone ?? false) ? StandaloneCache : Cache;
				if (cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

			var func = await CompileAsync(null, this.Context, this.Options, expression(), cancellationToken).ConfigureAwait(false);

			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
			}

			return func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public Delegate CompileGlobal(ITreeNode node)
		{
			if (node == null) return null;
			var buildContext = new BuildContext();
			BuildOptions buildOptions;
			if ((this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = this.Options;
			}
			else
			{
				buildOptions = new BuildOptions(this.Options) { CompileMode = ECompileMode.All };
			}
			var body = node.Build(buildContext, this.Context, buildOptions);
			PoolManage.Return(node);
			return buildContext.Compile(this.Context, buildOptions, body);
		}

		public Delegate CompileGlobal(string expression, Type[] argTypes, string[] argNames)
		{
			if (string.IsNullOrEmpty(expression)) return null;
			int argTypesCount = argTypes == null ? 0 : argTypes.Length;
			int argNamesCount = argNames == null ? 0 : argNames.Length;
			if (argTypesCount != argNamesCount)
			{
				throw new Exceptions.ScriptAnalyzingException($"argTypes数量[{argTypesCount}]与argNames数量[{argNamesCount}]不一致");
			}

			var buildContext = new BuildContext();
			if (argTypesCount > 0)
			{
				for (int i = 0; i < argTypesCount; i++)
				{
					string name = argNames[i];
					Type type = argTypes[i];
					buildContext.Parameters.Add(name, System.Linq.Expressions.Expression.Parameter(type, name));
				}
			}
			return Compile(buildContext, this.Context, this.Options, expression);
		}

		public Delegate CompileGlobal(Stream expression, Type[] argTypes, string[] argNames)
		{
			if (expression == null) return null;
			int argTypesCount = argTypes == null ? 0 : argTypes.Length;
			int argNamesCount = argNames == null ? 0 : argNames.Length;
			if (argTypesCount != argNamesCount)
			{
				throw new Exceptions.ScriptAnalyzingException($"argTypes数量[{argTypesCount}]与argNames数量[{argNamesCount}]不一致");
			}

			var buildContext = new BuildContext();
			if (argTypesCount > 0)
			{
				for (int i = 0; i < argTypesCount; i++)
				{
					string name = argNames[i];
					Type type = argTypes[i];
					buildContext.Parameters.Add(name, System.Linq.Expressions.Expression.Parameter(type, name));
				}
			}
			return Compile(buildContext, this.Context, this.Options, expression);
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Func<ScriptContext, T> CompileGlobal<T>(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Func<ScriptContext, T>> CompileGlobalAsync<T>(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			var func = await CompileGlobalAsync(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Func<ScriptContext, T> CompileGlobal<T>(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Func<ScriptContext, T>> CompileGlobalAsync<T>(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			var func = await CompileGlobalAsync(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Func<ScriptContext, T> CompileGlobal<T>(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		/// <summary>
		/// 异步编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Func<ScriptContext, T>> CompileGlobalAsync<T>(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			var func = await CompileGlobalAsync(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Func<ScriptContext, T> CompileGlobal<T>(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		/// <summary>
		/// 异步编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Func<ScriptContext, T>> CompileGlobalAsync<T>(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			var func = await CompileGlobalAsync(expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node"></param>
		/// <returns></returns>
		public Func<ScriptContext, T> CompileGlobal<T>(ITreeNode node)
		{
			var func = CompileGlobal(node);
			if (func == null) return null;
			if (func is Func<ScriptContext, T> t) return t;
			if (func.Method.ReturnType != typeof(T))
			{
				T targetFunc(ScriptContext c) => (T)func.DynamicInvoke(c);
				return targetFunc;
			}
			return (Func<ScriptContext, T>)func;
		}

		public Delegate CompileGlobal(ITokenStream tokenStream)
		{
			var buildContext = new BuildContext();
			BuildOptions buildOptions;
			if ((this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = this.Options;
			}
			else
			{
				buildOptions = new BuildOptions(this.Options)
				{
					CompileMode = ECompileMode.All
				};
			}
			var node = GetSyntaxAnalyzer(this.Context).Build(buildContext, this.Context, buildOptions, new Readers.TokenReader(tokenStream, false));
			var body = node.Build(buildContext, this.Context, buildOptions);
			PoolManage.Return(node);
			return buildContext.Compile(this.Context, buildOptions, body);
		}

		public async Task<Delegate> CompileGlobalAsync(ITokenStream tokenStream, CancellationToken cancellationToken = default)
		{
			var buildContext = new BuildContext();
			BuildOptions buildOptions;
			if ((this.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = this.Options;
			}
			else
			{
				buildOptions = new BuildOptions(this.Options)
				{
					CompileMode = ECompileMode.All
				};
			}
			var node = await GetSyntaxAnalyzer(this.Context).BuildAsync(buildContext, this.Context, buildOptions, new Readers.TokenReader(tokenStream, false), cancellationToken).ConfigureAwait(false);
			var body = node.Build(buildContext, this.Context, buildOptions);
			PoolManage.Return(node);
			return buildContext.Compile(this.Context, buildOptions, body);
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Func<T> Compile<T>(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
			if (func == null) return null;
			T targetFunc() => func(this.Context);
			return targetFunc;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Func<T> Compile<T>(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
			if (func == null) return null;
			T targetFunc() => func(this.Context);
			return targetFunc;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Func<T> Compile<T>(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
			if (func == null) return null;
			T targetFunc() => func(this.Context);
			return targetFunc;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则不缓存）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Func<T> Compile<T>(Func<Stream> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
			if (func == null) return null;
			T targetFunc() => func(this.Context);
			return targetFunc;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node"></param>
		/// <returns></returns>
		public Func<T> Compile<T>(ITreeNode node)
		{
			var func = CompileGlobal<T>(node);
			if (func == null) return null;
			T targetFunc() => func(this.Context);
			return targetFunc;
		}

		public Delegate Compile(string expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			return Lambda(expression, argTypes, argNames, returnType)?.Compile();
		}

		public Delegate Compile(Stream expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			return Lambda(expression, argTypes, argNames, returnType)?.Compile();
		}

		public Delegate Compile(ITreeNode expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			return Lambda(expression, argTypes, argNames, returnType)?.Compile();
		}

		public TDelegate Compile<TDelegate>(string expression, string[] argNames) where TDelegate : Delegate
		{
			return Lambda<TDelegate>(expression, argNames)?.Compile();
		}

		public TDelegate Compile<TDelegate>(Stream expression, string[] argNames) where TDelegate : Delegate
		{
			return Lambda<TDelegate>(expression, argNames)?.Compile();
		}

		public TDelegate Compile<TDelegate>(ITreeNode expression, string[] argNames) where TDelegate : Delegate
		{
			return Lambda<TDelegate>(expression, argNames)?.Compile();
		}

		public Func<T1, TReturn> Compile<T1, TReturn>(string expression, string argName)
		{
			return (Func<T1, TReturn>)Compile(expression, new[] { typeof(T1) }, new[] { argName }, typeof(TReturn));
		}

		public Func<T1, TReturn> Compile<T1, TReturn>(Stream expression, string argName)
		{
			return (Func<T1, TReturn>)Compile(expression, new[] { typeof(T1) }, new[] { argName }, typeof(TReturn));
		}

		public Func<T1, T2, TReturn> Compile<T1, T2, TReturn>(string expression, string argName1, string argName2)
		{
			return (Func<T1, T2, TReturn>)Compile(expression, new[] { typeof(T1), typeof(T2) }, new[] { argName1, argName2 }, typeof(TReturn));
		}

		public Func<T1, T2, TReturn> Compile<T1, T2, TReturn>(Stream expression, string argName1, string argName2)
		{
			return (Func<T1, T2, TReturn>)Compile(expression, new[] { typeof(T1), typeof(T2) }, new[] { argName1, argName2 }, typeof(TReturn));
		}

		public Func<T1, T2, T3, TReturn> Compile<T1, T2, T3, TReturn>(string expression, string argName1, string argName2, string argName3)
		{
			return (Func<T1, T2, T3, TReturn>)Compile(expression, new[] { typeof(T1), typeof(T2), typeof(T3) }, new[] { argName1, argName2, argName3 }, typeof(TReturn));
		}

		public Func<T1, T2, T3, TReturn> Compile<T1, T2, T3, TReturn>(Stream expression, string argName1, string argName2, string argName3)
		{
			return (Func<T1, T2, T3, TReturn>)Compile(expression, new[] { typeof(T1), typeof(T2), typeof(T3) }, new[] { argName1, argName2, argName3 }, typeof(TReturn));
		}

		public Func<T1, T2, T3, T4, TReturn> Compile<T1, T2, T3, T4, TReturn>(string expression, string argName1, string argName2, string argName3, string argName4)
		{
			return (Func<T1, T2, T3, T4, TReturn>)Compile(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new[] { argName1, argName2, argName3, argName4 }, typeof(TReturn));
		}

		public Func<T1, T2, T3, T4, TReturn> Compile<T1, T2, T3, T4, TReturn>(Stream expression, string argName1, string argName2, string argName3, string argName4)
		{
			return (Func<T1, T2, T3, T4, TReturn>)Compile(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new[] { argName1, argName2, argName3, argName4 }, typeof(TReturn));
		}

		public Func<T1, T2, T3, T4, T5, TReturn> Compile<T1, T2, T3, T4, T5, TReturn>(string expression, string argName1, string argName2, string argName3, string argName4, string argName5)
		{
			return (Func<T1, T2, T3, T4, T5, TReturn>)Compile(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, new[] { argName1, argName2, argName3, argName4, argName5 }, typeof(TReturn));
		}

		public Func<T1, T2, T3, T4, T5, TReturn> Compile<T1, T2, T3, T4, T5, TReturn>(Stream expression, string argName1, string argName2, string argName3, string argName4, string argName5)
		{
			return (Func<T1, T2, T3, T4, T5, TReturn>)Compile(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, new[] { argName1, argName2, argName3, argName4, argName5 }, typeof(TReturn));
		}

		public LambdaExpression Lambda(string expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			return Lambda(this.Context, this.Options, expression, argTypes, argNames, returnType);
		}

		public LambdaExpression Lambda(Stream expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			return Lambda(this.Context, this.Options, expression, argTypes, argNames, returnType);
		}

		public LambdaExpression Lambda(ITreeNode expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			return Lambda(this.Context, this.Options, expression, argTypes, argNames, returnType);
		}

		public Expression<TDelegate> Lambda<TDelegate>(string expression, string[] argNames) where TDelegate : Delegate
		{
			if (string.IsNullOrEmpty(expression)) return null;
			var delegateType = typeof(TDelegate);
			var argTypes = delegateType.GenericTypeArguments;
			Type returnType;
			if (delegateType.Name.StartsWith("Func"))
			{
				returnType = argTypes[argTypes.Length - 1];
				var tmpTypes = new Type[argTypes.Length - 1];
				Array.Copy(argTypes, 0, tmpTypes, 0, tmpTypes.Length);
				argTypes = tmpTypes;
			}
			else
			{
				returnType = typeof(void);
			}
			return (Expression<TDelegate>)Lambda(expression, argTypes, argNames, returnType);
		}

		public Expression<TDelegate> Lambda<TDelegate>(Stream expression, string[] argNames) where TDelegate : Delegate
		{
			if (expression == null) return null;
			var delegateType = typeof(TDelegate);
			var argTypes = delegateType.GenericTypeArguments;
			Type returnType;
			if (delegateType.Name.StartsWith("Func"))
			{
				returnType = argTypes[argTypes.Length - 1];
				var tmpTypes = new Type[argTypes.Length - 1];
				Array.Copy(argTypes, 0, tmpTypes, 0, tmpTypes.Length);
				argTypes = tmpTypes;
			}
			else
			{
				returnType = typeof(void);
			}
			return (Expression<TDelegate>)Lambda(expression, argTypes, argNames, returnType);
		}

		public Expression<TDelegate> Lambda<TDelegate>(ITreeNode expression, string[] argNames) where TDelegate : Delegate
		{
			if (expression == null) return null;
			var delegateType = typeof(TDelegate);
			var argTypes = delegateType.GenericTypeArguments;
			Type returnType;
			if (delegateType.Name.StartsWith("Func"))
			{
				returnType = argTypes[argTypes.Length - 1];
				var tmpTypes = new Type[argTypes.Length - 1];
				Array.Copy(argTypes, 0, tmpTypes, 0, tmpTypes.Length);
				argTypes = tmpTypes;
			}
			else
			{
				returnType = typeof(void);
			}
			return (Expression<TDelegate>)Lambda(expression, argTypes, argNames, returnType);
		}

		public Expression<Func<T1, TReturn>> Lambda<T1, TReturn>(string expression, string argName)
		{
			return (Expression<Func<T1, TReturn>>)Lambda(expression, new[] { typeof(T1) }, new[] { argName }, typeof(TReturn));
		}

		public Expression<Func<T1, TReturn>> Lambda<T1, TReturn>(Stream expression, string argName)
		{
			return (Expression<Func<T1, TReturn>>)Lambda(expression, new[] { typeof(T1) }, new[] { argName }, typeof(TReturn));
		}

		public Expression<Func<T1, T2, TReturn>> Lambda<T1, T2, TReturn>(string expression, string argName1, string argName2)
		{
			return (Expression<Func<T1, T2, TReturn>>)Lambda(expression, new[] { typeof(T1), typeof(T2) }, new[] { argName1, argName2 }, typeof(TReturn));
		}

		public Expression<Func<T1, T2, TReturn>> Lambda<T1, T2, TReturn>(Stream expression, string argName1, string argName2)
		{
			return (Expression<Func<T1, T2, TReturn>>)Lambda(expression, new[] { typeof(T1), typeof(T2) }, new[] { argName1, argName2 }, typeof(TReturn));
		}

		public Expression<Func<T1, T2, T3, TReturn>> Lambda<T1, T2, T3, TReturn>(string expression, string argName1, string argName2, string argName3)
		{
			return (Expression<Func<T1, T2, T3, TReturn>>)Lambda(expression, new[] { typeof(T1), typeof(T2), typeof(T3) }, new[] { argName1, argName2, argName3 }, typeof(TReturn));
		}

		public Expression<Func<T1, T2, T3, TReturn>> Lambda<T1, T2, T3, TReturn>(Stream expression, string argName1, string argName2, string argName3)
		{
			return (Expression<Func<T1, T2, T3, TReturn>>)Lambda(expression, new[] { typeof(T1), typeof(T2), typeof(T3) }, new[] { argName1, argName2, argName3 }, typeof(TReturn));
		}

		public Expression<Func<T1, T2, T3, T4, TReturn>> Lambda<T1, T2, T3, T4, TReturn>(string expression, string argName1, string argName2, string argName3, string argName4)
		{
			return (Expression<Func<T1, T2, T3, T4, TReturn>>)Lambda(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new[] { argName1, argName2, argName3, argName4 }, typeof(TReturn));
		}

		public Expression<Func<T1, T2, T3, T4, TReturn>> Lambda<T1, T2, T3, T4, TReturn>(Stream expression, string argName1, string argName2, string argName3, string argName4)
		{
			return (Expression<Func<T1, T2, T3, T4, TReturn>>)Lambda(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new[] { argName1, argName2, argName3, argName4 }, typeof(TReturn));
		}

		public Expression<Func<T1, T2, T3, T4, T5, TReturn>> Lambda<T1, T2, T3, T4, T5, TReturn>(string expression, string argName1, string argName2, string argName3, string argName4, string argName5)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, TReturn>>)Lambda(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, new[] { argName1, argName2, argName3, argName4, argName5 }, typeof(TReturn));
		}

		public Expression<Func<T1, T2, T3, T4, T5, TReturn>> Lambda<T1, T2, T3, T4, T5, TReturn>(Stream expression, string argName1, string argName2, string argName3, string argName4, string argName5)
		{
			return (Expression<Func<T1, T2, T3, T4, T5, TReturn>>)Lambda(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, new[] { argName1, argName2, argName3, argName4, argName5 }, typeof(TReturn));
		}

		/// <summary>
		/// 构建表达式树
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public ITreeNode BuildNode(string expression)
		{
			return BuildNode(null, this.Context, this.Options, expression);
		}

		/// <summary>
		/// 构建表达式树
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public ITreeNode BuildNode(Stream expression)
		{
			var buildContext = new BuildContext();
			//var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression, true);
			var tokenStream = GetTokenStream(this.Context, expression);
			var node = GetSyntaxAnalyzer(this.Context).Build(buildContext, this.Context, new BuildOptions(this.Options) { CreateFullTreeNode = true }, new Readers.TokenReader(tokenStream, false));
			if (node is TreeBuilder treeBuilder)
			{
				return treeBuilder.Root;
			}
			return node;
		}

		/// <summary>
		/// 异步构建表达式树
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public async Task<ITreeNode> BuildNodeAsync(Stream expression, CancellationToken cancellationToken = default)
		{
			var buildContext = new BuildContext();
			//var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression, true);
			var tokenStream = GetTokenStream(this.Context, expression);
			var node = await GetSyntaxAnalyzer(this.Context).BuildAsync(buildContext, this.Context, new BuildOptions(this.Options) { CreateFullTreeNode = true }, new Readers.TokenReader(tokenStream, false), cancellationToken).ConfigureAwait(false);
			if (node is TreeBuilder treeBuilder)
			{
				return treeBuilder.Root;
			}
			return node;
		}

		/// <summary>
		/// 构建表达式树
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ITreeNode BuildNode(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression)
		{
			if (buildContext == null) buildContext = new BuildContext();
			var tokenStream = GetTokenStream(scriptContext, expression);
			BuildOptions buildOptions;
			if (options.CreateFullTreeNode ?? false) buildOptions = options;
			else buildOptions = new BuildOptions(options) { CreateFullTreeNode = true };
			var node = GetSyntaxAnalyzer(scriptContext).Build(buildContext, scriptContext, buildOptions, new TokenReader(tokenStream, false));
			if (node is TreeBuilder treeBuilder)
			{
				return treeBuilder.Root;
			}
			return node;
		}

		/// <summary>
		/// 异步构建表达式树
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static async Task<ITreeNode> BuildNodeAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression, CancellationToken cancellationToken = default)
		{
			if (buildContext == null) buildContext = new BuildContext();
			var tokenStream = GetTokenStream(scriptContext, expression);
			BuildOptions buildOptions;
			if (options.CreateFullTreeNode ?? false) buildOptions = options;
			else buildOptions = new BuildOptions(options) { CreateFullTreeNode = true };
			var node = await GetSyntaxAnalyzer(scriptContext).BuildAsync(buildContext, scriptContext, buildOptions, new TokenReader(tokenStream, false), cancellationToken).ConfigureAwait(false);
			if (node is TreeBuilder treeBuilder)
			{
				return treeBuilder.Root;
			}
			return node;
		}

		public static object Eval(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return Eval(buildContext, scriptContext, options, expression, out _, cacheTime, cacheKey, cacheVersion);
		}

		/// <summary>
		/// 计算表达式，返回结果和类型（结果可能为null，此时returnType可以判断返回类型）
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <param name="returnType"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public static object Eval(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression, out Type returnType, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (string.IsNullOrEmpty(expression))
			{
				returnType = null;
				return null;
			}
			if (cacheTime != 0 || (options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = CompileGlobal(buildContext, scriptContext, options, expression, cacheTime, cacheKey, cacheVersion);
				returnType = func.Method.ReturnType;
				try
				{
					return (options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(scriptContext);
				}
				catch (System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}
			return Eval(scriptContext, options, expression, out returnType);
		}

		/// <summary>
		/// 异步计算表达式，返回结果和类型（结果可能为null，此时returnType可以判断返回类型）
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public static async Task<EvalResult> EvalAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return default;
			}
			if (cacheTime != 0 || (options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var func = await CompileGlobalAsync(buildContext, scriptContext, options, expression, cacheTime, cacheKey, cacheVersion, cancellationToken).ConfigureAwait(false);
				var value = (options.Standalone ?? false) ? func.DynamicInvoke() : func.DynamicInvoke(scriptContext);
				return new EvalResult(value, func.Method.ReturnType);
			}
			return await EvalAsync(scriptContext, options, expression, cancellationToken).ConfigureAwait(false);
		}

		public static object Eval(ScriptContext context, BuildOptions options, string expression, out Type returnType)
		{
			var tokenStream = GetTokenStream(context, expression);
			if (options == null) options = new BuildOptions(DefaultOptions);
			return GetSyntaxAnalyzer(context).Eval(context, options, tokenStream, out returnType);
		}

		public static async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, string expression, CancellationToken cancellationToken = default)
		{
			var tokenStream = GetTokenStream(context, expression);
			if (options == null) options = new BuildOptions(DefaultOptions);
			return await GetSyntaxAnalyzer(context).EvalAsync(context, options, tokenStream, cancellationToken).ConfigureAwait(false);
		}

		public static object Eval(ScriptContext context, BuildOptions options, Stream expression, out Type returnType)
		{
			var tokenStream = GetTokenStream(context, expression);
			if (options == null) options = new BuildOptions(DefaultOptions);
			return GetSyntaxAnalyzer(context).Eval(context, options, tokenStream, out returnType);
		}

		public static async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, Stream expression, CancellationToken cancellationToken = default)
		{
			var tokenStream = GetTokenStream(context, expression);
			if (options == null) options = new BuildOptions(DefaultOptions);
			return await GetSyntaxAnalyzer(context).EvalAsync(context, options, tokenStream, cancellationToken).ConfigureAwait(false);
		}

		public static object Eval(ScriptContext context, BuildOptions options, ITokenStream expression, out Type returnType)
		{
			if (options == null) options = new BuildOptions(DefaultOptions);
			return GetSyntaxAnalyzer(context).Eval(context, options, expression, out returnType);
		}

		public static Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, ITokenStream expression, CancellationToken cancellationToken = default)
		{
			return GetSyntaxAnalyzer(context).EvalAsync(context, options, expression, cancellationToken);
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public static Delegate CompileGlobal(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (string.IsNullOrEmpty(expression)) return null;

			Cache<Delegate> cache = null;
			if (cacheTime != 0)
			{
				if (string.IsNullOrEmpty(cacheKey)) cacheKey = expression;
				cache = (options?.Standalone ?? false) ? StandaloneCache : Cache;
				if (cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

			var func = Compile(buildContext, scriptContext, options, expression);

			if (cacheTime != 0)
			{
				cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
			}

			return func;
		}

		/// <summary>
		/// 异步编译生成委托
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<Delegate> CompileGlobalAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(expression)) return null;

			Cache<Delegate> cache = null;
			if (cacheTime != 0)
			{
				if (string.IsNullOrEmpty(cacheKey)) cacheKey = expression;
				cache = (options?.Standalone ?? false) ? StandaloneCache : Cache;
				if (cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

			var func = await CompileAsync(buildContext, scriptContext, options, expression, cancellationToken).ConfigureAwait(false);

			if (cacheTime != 0)
			{
				cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
			}

			return func;
		}

		/// <summary>
		/// 编译生成委托
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public static Delegate CompileGlobal(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null || expression.Length == 0L) return null;

			Cache<Delegate> cache = null;
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				cache = (options?.Standalone ?? false) ? StandaloneCache : Cache;
				if (cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

			var func = Compile(buildContext, scriptContext, options, expression);

			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
			}

			return func;
		}

		/// <summary>
		/// 异步编译生成委托
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <param name="cacheTime">
		/// <para>缓存时长</para>
		/// <para>为0表示不使用缓存（默认）；</para>
		/// <para>-1表示永久缓存；</para>
		/// <para>大于0表示缓存时长（单位：毫秒）</para>
		/// </param>
		/// <param name="cacheKey">
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<Delegate> CompileGlobalAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null, CancellationToken cancellationToken = default)
		{
			if (expression == null || expression.Length == 0L) return null;

			Cache<Delegate> cache = null;
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				cache = (options?.Standalone ?? false) ? StandaloneCache : Cache;
				if (cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

			var func = await CompileAsync(buildContext, scriptContext, options, expression, cancellationToken).ConfigureAwait(false);

			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
			}

			return func;
		}

		public static Delegate Compile(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression)
		{
			var lambda = Lambda(buildContext, scriptContext, options, expression);
			return lambda.Compile();
		}

		public static async Task<Delegate> CompileAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression, CancellationToken cancellationToken = default)
		{
			return (await LambdaAsync(buildContext, scriptContext, options, expression, cancellationToken).ConfigureAwait(false)).Compile();
		}

		public static Delegate Compile(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression)
		{
			return Lambda(buildContext, scriptContext, options, expression).Compile();
		}

		public static async Task<Delegate> CompileAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression, CancellationToken cancellationToken = default)
		{
			return (await LambdaAsync(buildContext, scriptContext, options, expression, cancellationToken).ConfigureAwait(false)).Compile();
		}

		public static Delegate Compile(ScriptContext context, BuildOptions options, ITreeNode expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			return Lambda(context, options, expression, argTypes, argNames, returnType)?.Compile();
		}

		public static LambdaExpression Lambda(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression)
		{
			if (buildContext == null) buildContext = new BuildContext();
			BuildOptions buildOptions;
			if (options == null)
			{
				buildOptions = new BuildOptions(DefaultOptions) { CompileMode = ECompileMode.All };
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = options;
			}
			else
			{
				buildOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
			}
			var tokenStream = GetTokenStream(scriptContext, expression);
			var node = GetSyntaxAnalyzer(scriptContext).Build(buildContext, scriptContext, buildOptions, new TokenReader(tokenStream, false));
			var body = node.Build(buildContext, scriptContext, buildOptions);
			PoolManage.Return(node);
			var bodys = body == null ? null : new[] { body };
			return buildContext.Build(scriptContext, buildOptions, bodys);
		}

		public static async Task<LambdaExpression> LambdaAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression, CancellationToken cancellationToken = default)
		{
			if (buildContext == null) buildContext = new BuildContext();
			BuildOptions buildOptions;
			if (options == null)
			{
				buildOptions = new BuildOptions(DefaultOptions) { CompileMode = ECompileMode.All };
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = options;
			}
			else
			{
				buildOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
			}
			var tokenStream = GetTokenStream(scriptContext, expression);
			var node = await GetSyntaxAnalyzer(scriptContext).BuildAsync(buildContext, scriptContext, buildOptions, new TokenReader(tokenStream, false), cancellationToken).ConfigureAwait(false);
			var body = node.Build(buildContext, scriptContext, buildOptions);
			PoolManage.Return(node);
			var bodys = body == null ? null : new[] { body };
			return buildContext.Build(scriptContext, buildOptions, bodys);
		}

		public static LambdaExpression Lambda(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression)
		{
			if (buildContext == null) buildContext = new BuildContext();
			BuildOptions buildOptions;
			if (options == null)
			{
				buildOptions = new BuildOptions(DefaultOptions) { CompileMode = ECompileMode.All };
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = options;
			}
			else
			{
				buildOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
			}
			var tokenStream = GetTokenStream(scriptContext, expression);
			var node = GetSyntaxAnalyzer(scriptContext).Build(buildContext, scriptContext, buildOptions, new TokenReader(tokenStream, false));
			var body = node.Build(buildContext, scriptContext, buildOptions);
			PoolManage.Return(node);
			var bodys = body == null ? null : new[] { body };
			return buildContext.Build(scriptContext, buildOptions, bodys);
		}

		public static async Task<LambdaExpression> LambdaAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression, CancellationToken cancellationToken = default)
		{
			if (buildContext == null) buildContext = new BuildContext();
			BuildOptions buildOptions;
			if (options == null)
			{
				buildOptions = new BuildOptions(DefaultOptions) { CompileMode = ECompileMode.All };
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = options;
			}
			else
			{
				buildOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
			}
			var tokenStream = GetTokenStream(scriptContext, expression);
			var node = await GetSyntaxAnalyzer(scriptContext).BuildAsync(buildContext, scriptContext, buildOptions, new TokenReader(tokenStream, false), cancellationToken).ConfigureAwait(false);
			var body = node.Build(buildContext, scriptContext, buildOptions);
			PoolManage.Return(node);
			var bodys = body == null ? null : new[] { body };
			return buildContext.Build(scriptContext, buildOptions, bodys);
		}

		public static LambdaExpression Lambda(ScriptContext context, BuildOptions options, string expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			if (string.IsNullOrEmpty(expression)) return null;
			int argTypesCount = argTypes == null ? 0 : argTypes.Length;
			int argNamesCount = argNames == null ? 0 : argNames.Length;
			if (argTypesCount != argNamesCount)
			{
				throw new Exceptions.ScriptAnalyzingException($"argTypes数量[{argTypesCount}]与argNames数量[{argNamesCount}]不一致");
			}

			var buildContext = new BuildContext(null)
			{
				ScriptContextParameter = Expression.Variable(typeof(ScriptContext)),
				ReturnType = returnType
			};
			if (argTypesCount > 0)
			{
				for (int i = 0; i < argTypesCount; i++)
				{
					string name = argNames[i];
					Type type = argTypes[i];
					buildContext.Parameters.Add(name, Expression.Parameter(type, name));
				}
			}
			BuildOptions buildOptions;
			if (options == null)
			{
				buildOptions = new BuildOptions(DefaultOptions) { CompileMode = ECompileMode.All };
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = options;
			}
			else
			{
				buildOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
			}
			return Lambda(buildContext, context, buildOptions, expression);
		}

		public static LambdaExpression Lambda(ScriptContext context, BuildOptions options, Stream expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			if (expression == null) return null;
			int argTypesCount = argTypes == null ? 0 : argTypes.Length;
			int argNamesCount = argNames == null ? 0 : argNames.Length;
			if (argTypesCount != argNamesCount)
			{
				throw new Exceptions.ScriptAnalyzingException($"argTypes数量[{argTypesCount}]与argNames数量[{argNamesCount}]不一致");
			}

			var buildContext = new BuildContext(null)
			{
				ScriptContextParameter = Expression.Variable(typeof(ScriptContext)),
				ReturnType = returnType
			};
			if (argTypesCount > 0)
			{
				for (int i = 0; i < argTypesCount; i++)
				{
					string name = argNames[i];
					Type type = argTypes[i];
					buildContext.Parameters.Add(name, Expression.Parameter(type, name));
				}
			}
			BuildOptions buildOptions;
			if (options == null)
			{
				buildOptions = new BuildOptions(DefaultOptions) { CompileMode = ECompileMode.All };
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = options;
			}
			else
			{
				buildOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
			}
			return Lambda(buildContext, context, buildOptions, expression);
		}

		public static LambdaExpression Lambda(ScriptContext context, BuildOptions options, ITreeNode expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			return Lambda(null, context, options, expression, argTypes, argNames, returnType);
		}

		public static LambdaExpression Lambda(Type delegateType, ScriptContext context, BuildOptions options, ITreeNode expression, Type[] argTypes, string[] argNames, Type returnType = null)
		{
			if (expression == null) return null;
			int argTypesCount = argTypes == null ? 0 : argTypes.Length;
			int argNamesCount = argNames == null ? 0 : argNames.Length;
			if (argTypesCount != argNamesCount)
			{
				throw new Exceptions.ScriptAnalyzingException($"argTypes数量[{argTypesCount}]与argNames数量[{argNamesCount}]不一致");
			}

			if (delegateType == typeof(Delegate))
			{
				delegateType = null;
			}

			var buildContext = new BuildContext(null)
			{
				ScriptContextParameter = Expression.Variable(typeof(ScriptContext)),
				DelegateType = delegateType,
				ReturnType = returnType
			};
			if (argTypesCount > 0)
			{
				var delTypes = delegateType?.GetMethod("Invoke").GetParameters();
				for (int i = 0; i < argTypesCount; i++)
				{
					string name = argNames[i];
					Type type = delTypes == null ? argTypes[i] : delTypes[i].ParameterType;
					buildContext.Parameters.Add(name, Expression.Parameter(type, name));
				}
			}
			BuildOptions buildOptions;
			if (options == null)
			{
				buildOptions = new BuildOptions(DefaultOptions) { CompileMode = ECompileMode.All };
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				buildOptions = options;
			}
			else
			{
				buildOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
			}
			var body = expression.Build(buildContext, context, buildOptions);
			PoolManage.Return(expression);
			var bodys = body == null ? null : new[] { body };
			return buildContext.Build(context, buildOptions, bodys);
		}

		//public static LambdaExpression Lambda(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, ITreeNode expression, Type[] argTypes, string[] argNames, Type returnType = null)
		//{
		//	if (expression == null) return null;
		//	int argTypesCount = argTypes == null ? 0 : argTypes.Length;
		//	int argNamesCount = argNames == null ? 0 : argNames.Length;
		//	if (argTypesCount != argNamesCount)
		//	{
		//		throw new Exceptions.ScriptAnalyzingException($"argTypes数量[{argTypesCount}]与argNames数量[{argNamesCount}]不一致");
		//	}

		//	if (argTypesCount > 0)
		//	{
		//		for (int i = 0; i < argTypesCount; i++)
		//		{
		//			string name = argNames[i];
		//			Type type = argTypes[i];
		//			buildContext.Parameters.Add(name, Expression.Parameter(type, name));
		//		}
		//	}
		//	BuildOptions buildOptions;
		//	if (options == null)
		//	{
		//		buildOptions = new BuildOptions(DefaultOptions) { CompileMode = ECompileMode.All };
		//	}
		//	else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
		//	{
		//		buildOptions = options;
		//	}
		//	else
		//	{
		//		buildOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
		//	}
		//	var body = expression.Build(buildContext, scriptContext, buildOptions);
		//	PoolManage.Return(expression);
		//	var bodys = body == null ? null : new[] { body };
		//	return buildContext.Build(scriptContext, buildOptions, bodys);
		//}

		private static ITokenStream GetTokenStream(ScriptContext context, string expression)
		{
			var charReader = new CharReader(new StringCharStream(expression), true);
			return context.GetTokenStream(charReader) ?? DefaultLexicalAnalyzer.Create(charReader);
		}

		private static ITokenStream GetTokenStream(ScriptContext context, Stream expression)
		{
			var charReader = new CharReader(new StreamCharStream(expression, true), true);
			return context.GetTokenStream(charReader) ?? DefaultLexicalAnalyzer.Create(charReader);
		}

		private static ISyntaxAnalyzer GetSyntaxAnalyzer(ScriptContext context)
		{
			return context.GetSyntaxAnalyzer() ?? DefaultSyntaxAnalyzer;
		}
	}
}
