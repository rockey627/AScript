using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言fallthrough语句处理器
	/// </summary>
	public class GoFallthroughTokenHandler : ITokenHandler
	{
		public static readonly GoFallthroughTokenHandler Instance = new GoFallthroughTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			if (e.Ignore) return;

			var fallthroughNode = new Nodes.FallthroughNode();
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, fallthroughNode);
		}
	}
}
