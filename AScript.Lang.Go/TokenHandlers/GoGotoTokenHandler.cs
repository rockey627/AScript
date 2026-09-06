using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言goto语句处理器
	/// </summary>
	public class GoGotoTokenHandler : ITokenHandler
	{
		public static readonly GoGotoTokenHandler Instance = new GoGotoTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			var labelToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			if (!e.Ignore)
			{
				var gotoNode = new Nodes.GotoNode { Label = labelToken.Value.Value };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, gotoNode);
			}
		}
	}
}
