using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// set a = 10
	/// </summary>
	public class SqlSetTokenHandler : ITokenHandler
	{
		public static readonly SqlSetTokenHandler Instance = new SqlSetTokenHandler();

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
			analyzer.ValidateNextToken(e.TokenReader, "=");
			var statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			if (!e.Ignore)
			{
				var op = PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]);
				op.Left = PoolManage.CreateVariableNode(varToken.Value.Value);
				op.Right = statement;
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
			}
		}
	}
}
