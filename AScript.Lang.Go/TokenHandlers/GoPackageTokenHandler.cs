using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言package声明处理器
	/// package name
	/// </summary>
	public class GoPackageTokenHandler : ITokenHandler
	{
		public static readonly GoPackageTokenHandler Instance = new GoPackageTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// package name
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			if (!e.Ignore)
			{
				var packageNode = new Nodes.PackageNode { Name = nameToken.Value.Value };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, packageNode);
			}
		}
	}
}
