using AScript.Lang.CSharp;
using System;
using System.IO;
using AScript.Nodes;
using System.Linq.Expressions;
using AScript.Readers;

namespace AScript
{
	/// <summary>
	/// 脚本执行（非线程安全）
	/// </summary>
	public class Script : ScriptEngine
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
		public Script(ScriptContext context) : base(context)
		{
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
			return Eval(this.Options, tokenStream, out returnType);
		}

		/// <summary>
		/// 计算表达式，返回结果和类型
		/// </summary>
		/// <param name="options"></param>
		/// <param name="tokenStream"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected object Eval(BuildOptions options, ITokenStream tokenStream, out Type returnType)
		{
			return GetSyntaxAnalyzer(this.Context).Eval(this.Context, options, tokenStream, out returnType);
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
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ITreeNode BuildNode(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression)
		{
			if (buildContext == null) buildContext = new BuildContext();
			//if (scriptContext == null) scriptContext = this.Context;
			//var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression);
			var tokenStream = GetTokenStream(scriptContext, expression);
			var node = GetSyntaxAnalyzer(scriptContext).Build(buildContext, scriptContext, new BuildOptions(options) { CreateFullTreeNode = true }, new Readers.TokenReader(tokenStream, false));
			if (node is TreeBuilder treeBuilder)
			{
				return treeBuilder.Root;
			}
			return node;
		}

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

		public static object Eval(ScriptContext context, BuildOptions options, string expression, out Type returnType)
		{
			var tokenStream = GetTokenStream(context, expression);
			return GetSyntaxAnalyzer(context).Eval(context, options, tokenStream, out returnType);
		}

		public static object Eval(ScriptContext context, BuildOptions options, Stream expression, out Type returnType)
		{
			//var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression, true);
			var tokenStream = GetTokenStream(context, expression);
			return GetSyntaxAnalyzer(context).Eval(context, options, tokenStream, out returnType);
		}

		public static Delegate Compile(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression)
		{
			//var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression);
			//var node = (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Build(buildContext, scriptContext, options, new Readers.TokenReader(tokenStream, false));
			//var body = node.Build(buildContext, scriptContext, options);
			//PoolManage.Return(node);
			//return buildContext.Compile(scriptContext, options, body);
			return Lambda(buildContext, scriptContext, options, expression).Compile();
		}

		public static Delegate Compile(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression)
		{
			//var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression, true);
			//var node = GetSyntaxAnalyzer().Build(buildContext, scriptContext, options, new Readers.TokenReader(tokenStream, false));
			//var body = node.Build(buildContext, scriptContext, options);
			//PoolManage.Return(node);
			//return buildContext.Compile(scriptContext, options, body);
			return Lambda(buildContext, scriptContext, options, expression).Compile();
		}

		public static LambdaExpression Lambda(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression)
		{
			//var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression);
			var tokenStream = GetTokenStream(scriptContext, expression);
			var node = GetSyntaxAnalyzer(scriptContext).Build(buildContext, scriptContext, options, new Readers.TokenReader(tokenStream, false));
			var body = node.Build(buildContext, scriptContext, options);
			PoolManage.Return(node);
			var bodys = body == null ? null : new[] { body };
			return buildContext.Build(scriptContext, options, bodys);
		}

		public static LambdaExpression Lambda(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression)
		{
			//var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression, true);
			var tokenStream = GetTokenStream(scriptContext, expression);
			var node = GetSyntaxAnalyzer(scriptContext).Build(buildContext, scriptContext, options, new Readers.TokenReader(tokenStream, false));
			var body = node.Build(buildContext, scriptContext, options);
			PoolManage.Return(node);
			var bodys = body == null ? null : new[] { body };
			return buildContext.Build(scriptContext, options, bodys);
		}
	}
}
