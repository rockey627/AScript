using AScript.Lang.CSharp;
using System;
using System.Collections.Concurrent;
using System.IO;
using AScript.Nodes;

namespace AScript
{
	/// <summary>
	/// 脚本执行
	/// </summary>
	public class Script : ScriptEngine, IScriptProvider
	{
		/// <summary>
		/// 脚本语言列表
		/// </summary>
		public static readonly ConcurrentDictionary<string, ScriptLang> Langs = new ConcurrentDictionary<string, ScriptLang>(StringComparer.OrdinalIgnoreCase);

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
			Langs["CSharp"] = CSharpLang.Instance;
		}

		/// <summary>
		/// 词法分析器
		/// </summary>
		public ILexicalAnalyzer LexicalAnalyzer { get; set; }
		/// <summary>
		/// 语法分析器
		/// </summary>
		public ISyntaxAnalyzer SyntaxAnalyzer { get; set; }

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
			this.ScriptProvider = this;
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
			return (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Eval(this.Context, options, tokenStream, out returnType);
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
				buildOptions = new BuildOptions(this.Options) { CompileMode = ECompileMode.All };
			}
			var node = (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Build(buildContext, this.Context, buildOptions, new Readers.TokenReader(tokenStream, false));
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
			var buildContext = new BuildContext();
			var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression);
			var node = (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Build(buildContext, this.Context, new BuildOptions(this.Options) { CreateFullTreeNode = true }, new Readers.TokenReader(tokenStream, false));
			if (node is TreeBuilder treeBuilder)
			{
				return treeBuilder.Root;
			}
			return node;
		}

		/// <summary>
		/// 构建表达式树
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public ITreeNode BuildNode(Stream expression)
		{
			var buildContext = new BuildContext();
			var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression, true);
			var node = (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Build(buildContext, this.Context, new BuildOptions(this.Options) { CreateFullTreeNode = true }, new Readers.TokenReader(tokenStream, false));
			if (node is TreeBuilder treeBuilder)
			{
				return treeBuilder.Root;
			}
			return node;
		}

		object IScriptProvider.Eval(ScriptContext context, BuildOptions options, string expression, out Type returnType)
		{
			return (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Eval(context, options, (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression), out returnType);
		}

		object IScriptProvider.Eval(ScriptContext context, BuildOptions options, Stream expression, out Type returnType)
		{
			return (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Eval(context, options, (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression, true), out returnType);
		}

		Delegate IScriptProvider.Compile(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string expression)
		{
			var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression);
			var node = (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Build(buildContext, scriptContext, options, new Readers.TokenReader(tokenStream, false));
			var body = node.Build(buildContext, scriptContext, options);
			PoolManage.Return(node);
			return buildContext.Compile(scriptContext, options, body);
		}

		Delegate IScriptProvider.Compile(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, Stream expression)
		{
			var tokenStream = (this.LexicalAnalyzer ?? DefaultLexicalAnalyzer).Create(expression, true);
			var node = (this.SyntaxAnalyzer ?? DefaultSyntaxAnalyzer).Build(buildContext, scriptContext, options, new Readers.TokenReader(tokenStream, false));
			var body = node.Build(buildContext, scriptContext, options);
			PoolManage.Return(node);
			return buildContext.Compile(scriptContext, options, body);
		}
	}
}
