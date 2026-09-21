using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言import声明处理器
	/// import "path"
	/// import "path"
	/// import name "path"
	/// import ( "path1" "path2" )
	/// </summary>
	public class GoImportTokenHandler : ITokenHandler
	{
		public static readonly GoImportTokenHandler Instance = new GoImportTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			string moduleName;
			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.IsSymbol("("))
			{
				moduleName = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String).Value.Value;
				analyzer.ValidateNextToken(e.TokenReader, ")");
			}
			else if (nextToken.Value.Type == ETokenType.String)
			{
				moduleName = nextToken.Value.Value;
			}
			else
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid '{nextToken.Value.Value}' near '{e.CurrentToken.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			if (!e.Ignore)
			{
				var callFuncNode = new CallFuncNode
				{
					Name = e.CurrentToken.Value,
					Args = new ITreeNode[]
					{
						PoolManage.CreateObjectNode(moduleName)
					}
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, callFuncNode);
			}
		}
	}
}
