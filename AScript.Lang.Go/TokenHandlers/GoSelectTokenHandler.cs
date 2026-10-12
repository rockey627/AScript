using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言select语句处理器
	/// select {
	/// case <-ch1: ...
	/// case ch2 <- val: ...
	/// default: ...
	/// }
	/// </summary>
	public class GoSelectTokenHandler : ITokenHandler
	{
		public static readonly GoSelectTokenHandler Instance = new GoSelectTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			throw new System.NotImplementedException();
		}
	}
}
