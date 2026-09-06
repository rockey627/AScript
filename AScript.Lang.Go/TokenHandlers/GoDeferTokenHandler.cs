using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言defer语句处理器
	/// defer functionCall()
	/// </summary>
	public class GoDeferTokenHandler : ITokenHandler
	{
		public static readonly GoDeferTokenHandler Instance = new GoDeferTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			// 解析defer后面的函数调用
			var call = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);

			if (!e.Ignore)
			{
				var deferNode = new Nodes.DeferNode { Body = call };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, deferNode);
			}
		}
	}
}
