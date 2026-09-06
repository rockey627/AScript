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
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// 解析select body
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

			if (!e.Ignore)
			{
				var selectNode = new Nodes.SelectNode { Body = body };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, selectNode);
			}
		}
	}
}
