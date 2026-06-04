using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Sql.TokenHandlers
{
	public class SqlDeclareTokenHandler : ITokenHandler
	{
		public static readonly SqlDeclareTokenHandler Instance = new SqlDeclareTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			var typeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateDefineVarNode(varToken.Value.Value, typeToken.Value.Value));
			}
		}
	}
}
