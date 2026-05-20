using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.TokenHandlers
{
	public class AwaitTokenHandler : ITokenHandler, IAsyncTokenHandler
	{
		public static readonly AwaitTokenHandler Instance = new AwaitTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("await", 1, DefaultSyntaxAnalyzer.OperatorPriorities["."] - 1));
			}
		}

		public async Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				await e.TreeBuilder.AddOperatorAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("await", 1, DefaultSyntaxAnalyzer.OperatorPriorities["."] - 1), cancellationToken).ConfigureAwait(false);
			}
		}
	}
}
