using System;
using System.Threading;
using System.Threading.Tasks;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class BoolTokenHandler : ITokenHandler, IAsyncTokenHandler
	{
		public static readonly BoolTokenHandler Instance = new BoolTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.Ignore) return;

			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, Convert.ToBoolean(e.CurrentToken.Value), typeof(bool));
		}

		public async Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken)
		{
			e.IsHandled = true;
			if (e.Ignore) return;

			await e.TreeBuilder.AddDataAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, Convert.ToBoolean(e.CurrentToken.Value), typeof(bool), cancellationToken).ConfigureAwait(false);
		}
	}
}
