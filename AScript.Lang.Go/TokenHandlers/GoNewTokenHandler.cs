using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言new函数调用处理器
	/// new(Type)
	/// </summary>
	public class GoNewTokenHandler : ITokenHandler
	{
		public static readonly GoNewTokenHandler Instance = new GoNewTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			// new是一个函数调用，直接解析函数调用
			var call = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);

			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, call);
			}
		}
	}
}
