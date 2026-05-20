using System;
using System.Threading;
using System.Threading.Tasks;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// <para>后面的语句延时计算</para>
	/// <para>如??操作符，解析后面的语句时不能实时计算</para>
	/// </summary>
	public class LazyTokenHandler : ITokenHandler, IAsyncTokenHandler
	{
		public static readonly LazyTokenHandler Instance = new LazyTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, e.CurrentToken.Value, 2, DefaultSyntaxAnalyzer.OperatorPriorities[e.CurrentToken.Value]);
			var oneStatement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, new BuildOptions(e.Options) { CreateFullTreeNode = true }, e.TokenReader, e.Control, e.Ignore);
			e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, oneStatement);
		}

		public async Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken)
		{
			e.IsHandled = true;
			await e.TreeBuilder.AddOperatorAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, e.CurrentToken.Value, 2, DefaultSyntaxAnalyzer.OperatorPriorities[e.CurrentToken.Value], cancellationToken).ConfigureAwait(false);
			var oneStatement = await analyzer.BuildOneStatementAsync(e.BuildContext, e.ScriptContext, new BuildOptions(e.Options) { CreateFullTreeNode = true }, e.TokenReader, e.Control, e.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
			await e.TreeBuilder.AddAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, oneStatement, cancellationToken).ConfigureAwait(false);
		}
	}
}
