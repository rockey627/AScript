using AScript.Nodes;
using System;
using System.IO;

namespace AScript
{
	public class ScriptEngine
	{
		public const string SCRIPT_ENGINE_VAR_NAME = "__ScriptEngine__";

		/// <summary>
		/// 默认编译选项
		/// </summary>
		public static readonly BuildOptions DefaultOptions = new BuildOptions();

		/// <summary>
		/// 缓存
		/// </summary>
		public static readonly Cache<Delegate> Cache = new Cache<Delegate>();

		/// <summary>
		/// 上下文
		/// </summary>
		public ScriptContext Context { get; private set; }

		/// <summary>
		/// 编译选项
		/// </summary>
		public BuildOptions Options { get; private set; } = new BuildOptions(DefaultOptions);

		public IScriptProvider ScriptProvider { get; protected set; }

		/// <summary>
		/// 
		/// </summary>
		protected ScriptEngine() : this(ScriptContext.Create()) { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		protected ScriptEngine(ScriptContext context)
		{
			this.Context = context;
			this.Context.SetTempVar(SCRIPT_ENGINE_VAR_NAME, this, false);
		}
		public ScriptEngine(IScriptProvider scriptProvider) : this()
		{
			this.ScriptProvider = scriptProvider;
		}
		public ScriptEngine(IScriptProvider scriptProvider, ScriptContext context) : this(context)
		{
			this.ScriptProvider = scriptProvider;
		}

		public static ScriptEngine GetCurrent(ScriptContext context)
		{
			return (ScriptEngine)context?.EvalVar(SCRIPT_ENGINE_VAR_NAME);
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
			return Eval(this.Context, expression, cacheTime, cacheKey, cacheVersion);
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
			return Eval(this.Context, expression, out returnType, cacheTime, cacheKey, cacheVersion);
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
			return Eval<T>(this.Context, expression, cacheTime, cacheKey, cacheVersion);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <param name="context"></param>
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
		public object Eval(ScriptContext context, string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			return Eval(context, expression, out _, cacheTime, cacheKey, cacheVersion);
		}

		/// <summary>
		/// 计算表达式，返回结果和类型（结果可能为null，此时returnType可以判断返回类型）
		/// </summary>
		/// <param name="context"></param>
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
		public object Eval(ScriptContext context, string expression, out Type returnType, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (string.IsNullOrEmpty(expression))
			{
				returnType = null;
				return null;
			}
			var compileMode = this.Options.CompileMode ?? ECompileMode.None;
			if (cacheTime != 0 || compileMode == ECompileMode.All)
			{
				var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
				returnType = func.Method.ReturnType;
				return func.DynamicInvoke(context);
			}
			return this.ScriptProvider.Eval(context, this.Options, expression, out returnType);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
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
		public T Eval<T>(ScriptContext context, string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return default;
			}
			var compileMode = this.Options.CompileMode ?? ECompileMode.None;
			if (cacheTime != 0 || compileMode == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
				return func(context);
			}
			return (T)this.ScriptProvider.Eval(context, this.Options, expression, out _);
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
			var compileMode = this.Options.CompileMode ?? ECompileMode.None;
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey)
				|| compileMode == ECompileMode.All)
			{
				var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
				returnType = func.Method.ReturnType;
				return func.DynamicInvoke(this.Context);
			}
			return this.ScriptProvider.Eval(this.Context, this.Options, expression, out returnType);
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
			var compileMode = this.Options.CompileMode ?? ECompileMode.None;
			if (cacheTime != 0 || compileMode == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
				return func(this.Context);
			}
			return (T)this.ScriptProvider.Eval(this.Context, this.Options, expression, out _);
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
			var compileMode = this.Options.CompileMode ?? ECompileMode.None;
			if (cacheTime != 0 || compileMode == ECompileMode.All)
			{
				var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
				returnType = func.Method.ReturnType;
				return func.DynamicInvoke(this.Context);
			}
			return this.ScriptProvider.Eval(this.Context, this.Options, expression(), out returnType);
		}

		/// <summary>
		/// 计算表达式，返回结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
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
			var compileMode = this.Options.CompileMode ?? ECompileMode.None;
			if (cacheTime != 0 || compileMode == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
				return func(this.Context);
			}
			return (T)this.ScriptProvider.Eval(this.Context, this.Options, expression(), out _);
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
			var compileMode = this.Options.CompileMode ?? ECompileMode.None;
			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey)
				|| compileMode == ECompileMode.All)
			{
				var func = CompileGlobal(expression, cacheTime, cacheKey, cacheVersion);
				returnType = func.Method.ReturnType;
				return func.DynamicInvoke(this.Context);
			}
			return this.ScriptProvider.Eval(this.Context, this.Options, expression(), out returnType);
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
			var compileMode = this.Options.CompileMode ?? ECompileMode.None;
			if (cacheTime != 0 || compileMode == ECompileMode.All)
			{
				var func = CompileGlobal<T>(expression, cacheTime, cacheKey, cacheVersion);
				return func(this.Context);
			}
			return (T)this.ScriptProvider.Eval(this.Context, this.Options, expression(), out _);
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
				return func.DynamicInvoke(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return this.ScriptProvider.Eval(this.Context, options, expression, out returnType);
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
				var func = CompileGlobal<T>(expression);
				return func(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			//return (T)Eval(options, (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression), out _);
			return (T)this.ScriptProvider.Eval(this.Context, options, expression, out _);
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
				return func.DynamicInvoke(this.Context);
			}
			var options = new BuildOptions(this.Options) { CompileMode = compileMode };
			return this.ScriptProvider.Eval(this.Context, options, expression, out returnType);
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
			return (T)this.ScriptProvider.Eval(this.Context, options, expression, out _);
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
				return func.DynamicInvoke(this.Context);
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
		public Delegate CompileGlobal(string expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (string.IsNullOrEmpty(expression)) return null;

			if (cacheTime != 0)
			{
				if (string.IsNullOrEmpty(cacheKey)) cacheKey = expression;
				if (Cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

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
			var func = this.ScriptProvider.Compile(buildContext, this.Context, buildOptions, expression);

			if (cacheTime != 0)
			{
				//_cache[cacheKey] = func;
				Cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
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
		public Delegate CompileGlobal(Stream expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null || expression.Length == 0L) return null;

			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey)
				&& Cache.TryGetValue(cacheKey, cacheVersion, out var d))
			{
				return d;
			}

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
			var func = this.ScriptProvider.Compile(buildContext, this.Context, buildOptions, expression);

			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				Cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
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
		/// 缓存key（如果为空则取表达式字符串）
		/// </param>
		/// <param name="cacheVersion"></param>
		/// <returns></returns>
		public Delegate CompileGlobal(Func<string> expression, int cacheTime = 0, string cacheKey = null, string cacheVersion = null)
		{
			if (expression == null) return null;

			string s = null;
			if (cacheTime != 0)
			{
				if (string.IsNullOrEmpty(cacheKey))
				{
					s = expression();
					if (string.IsNullOrEmpty(s)) return null;
					cacheKey = s;
				}
				if (Cache.TryGetValue(cacheKey, cacheVersion, out var d))
				{
					return d;
				}
			}

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
			var func = this.ScriptProvider.Compile(buildContext, this.Context, buildOptions, s ?? expression());

			if (cacheTime != 0)
			{
				Cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
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

			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey)
				&& Cache.TryGetValue(cacheKey, cacheVersion, out var d))
			{
				return d;
			}

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
			var func = this.ScriptProvider.Compile(buildContext, this.Context, buildOptions, expression());

			if (cacheTime != 0 && !string.IsNullOrEmpty(cacheKey))
			{
				//_cache[cacheKey] = func;
				Cache.SetValue(cacheKey, func, cacheTime, cacheVersion);
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
	}
}
