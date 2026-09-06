using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言go语句处理器（goroutine）
	/// go functionCall()
	/// </summary>
	public class GoGoTokenHandler : ITokenHandler
	{
		public static readonly GoGoTokenHandler Instance = new GoGoTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			// 解析go后面的函数调用
			var call = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);

			if (!e.Ignore)
			{
				var goNode = new Nodes.GoNode { Body = call };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, goNode);
			}
		}
	}
}
