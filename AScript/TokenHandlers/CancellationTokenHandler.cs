using AScript.Syntaxs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.TokenHandlers
{
	public class CancellationTokenHandler : ITokenHandler, IAsyncTokenHandler
	{
		public static readonly CancellationTokenHandler Instance = new CancellationTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, CancellationToken.None);
			}
		}

		public async Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, cancellationToken);
			}
		}
	}
}
