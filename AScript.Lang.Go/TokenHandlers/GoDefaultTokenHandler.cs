using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言default语句处理器
	/// default: ...
	/// </summary>
	public class GoDefaultTokenHandler : ITokenHandler
	{
		public static readonly GoDefaultTokenHandler Instance = new GoDefaultTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// 跳过冒号
			analyzer.ValidateNextToken(e.TokenReader, ":");

			// 解析default body
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

			if (!e.Ignore)
			{
				var defaultNode = new Nodes.DefaultNode { Body = body };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defaultNode);
			}
		}
	}
}
