using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// 执行静态语句：static { ... }
	/// </summary>
	public class StaticTokenHandler : ITokenHandler
	{
		public static readonly StaticTokenHandler Instance = new StaticTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			var options = e.Options;
			if ((options.CompileMode?? ECompileMode.None) == ECompileMode.All)
			{
				options = new BuildOptions(e.Options) { CompileMode = ECompileMode.None };
			}
			var node = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, options, e.TokenReader, e.Control, e.Ignore, noblock: true);
			if (node != null && !e.Ignore)
			{
				var v = node.Eval(e.ScriptContext, options, e.Control, out var type);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(v, type));
			}
		}
	}
}
