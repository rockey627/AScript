using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Python3.TokenHandlers
{
	/// <summary>
	/// for x in [1, 2, 3]:
	/// </summary>
	public class Python3ForTokenHandler : ITokenHandler
	{
		public static readonly Python3ForTokenHandler Instance = new Python3ForTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			var token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exception($"invalid {e.CurrentToken.Value} expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			if (token.Value.Type != ETokenType.Word)
			{
				throw new Exception($"invalid '{token.Value.Value}' of {e.CurrentToken.Value} expression at ({token.Value.Line},{token.Value.Column})");
			}
			var varName = token.Value.Value;
			analyzer.ValidateNextToken(e.TokenReader, "in");
			// 
			var listBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: Python3Lang.EndTokens);
			analyzer.ValidateNextToken(e.TokenReader, ":");
			var createFullTreeNodeOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var bodyBuilder = Python3Lang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullTreeNodeOptions, e.TokenReader, e.Control, e.Ignore, endTokens: Python3Lang.EndTokens);

			if (e.Ignore) return;

			var foreachNode = new ForeachNode
			{
				VarDefine = PoolManage.CreateDefineVarNode(varName, type: null, systemType: typeof(object)),
				Collection = listBuilder,
				Body = bodyBuilder
			};
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
		}
	}
}
