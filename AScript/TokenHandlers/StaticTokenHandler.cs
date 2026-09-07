using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// 执行静态语句：static { ... }
	/// </summary>
	public class StaticTokenHandler : ITokenHandler, IAsyncTokenHandler
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
			// 如果当前为编译模式，则改为使用执行模式
			bool isCompileMode = false;
			if ((options.CompileMode?? ECompileMode.None) == ECompileMode.All)
			{
				isCompileMode = true;
				options = new BuildOptions(e.Options) { CompileMode = ECompileMode.None };
			}
			var staticScriptContext = isCompileMode ? e.BuildContext.GetOrCreateStaticScriptContext(e.ScriptContext) : e.ScriptContext;
			var node = analyzer.BuildOneStatement2(e.BuildContext, staticScriptContext, options, e.TokenReader, e.Control, e.Ignore, noblock: true);
			if (node != null && !e.Ignore)
			{
				if (!isCompileMode || node is ObjectNode)
				{
					e.TreeBuilder.AddData(e.BuildContext, staticScriptContext, e.Options, e.Control, node);
				}
				else
				{
					// 执行并返回结果
					var v = node.Eval(staticScriptContext, options, e.Control, out var type);
					e.TreeBuilder.AddData(e.BuildContext, staticScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(v, type));
				}
			}
		}

		public async Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			var options = e.Options;
			// 如果当前为编译模式，则改为使用执行模式
			bool isCompileMode = false;
			if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				isCompileMode = true;
				options = new BuildOptions(e.Options) { CompileMode = ECompileMode.None };
			}
			var staticScriptContext = isCompileMode ? e.BuildContext.GetOrCreateStaticScriptContext(e.ScriptContext) : e.ScriptContext;
			var node = await analyzer.BuildOneStatement2Async(e.BuildContext, staticScriptContext, options, e.TokenReader, e.Control, e.Ignore, noblock: true, cancellationToken: cancellationToken).ConfigureAwait(false);
			if (node != null && !e.Ignore)
			{
				if (!isCompileMode || node is ObjectNode)
				{
					await e.TreeBuilder.AddDataAsync(e.BuildContext, staticScriptContext, e.Options, e.Control, node, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					// 执行并返回结果
					var v = await node.EvalAsync(staticScriptContext, options, e.Control, cancellationToken).ConfigureAwait(false);
					await e.TreeBuilder.AddDataAsync(e.BuildContext, staticScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(v.Value, v.Type), cancellationToken).ConfigureAwait(false);
				}
			}
		}
	}
}
