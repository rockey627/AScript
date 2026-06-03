using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Sql.TokenHandlers
{
	public class SqlIsNullTokenHandler : ITokenHandler
	{
		public static readonly SqlIsNullTokenHandler Instance = new SqlIsNullTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			var nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			if ("not".Equals(nextToken.Value.Value, StringComparison.OrdinalIgnoreCase))
			{
				analyzer.ValidateNextToken(e.TokenReader, "null", StringComparison.OrdinalIgnoreCase);
				if (!e.Ignore)
				{
					var left = e.TreeBuilder.Pop();
					var op = PoolManage.CreateOperatorNode("!=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["!="]);
					op.Left = left;
					op.Right = PoolManage.CreateObjectNode(null);
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
				}
				return;
			}
			if ("null".EndsWith(nextToken.Value.Value, StringComparison.OrdinalIgnoreCase))
			{
				if (!e.Ignore)
				{
					var left = e.TreeBuilder.Pop();
					var op = PoolManage.CreateOperatorNode("==", 2, DefaultSyntaxAnalyzer.OperatorPriorities["=="]);
					op.Left = left;
					op.Right = PoolManage.CreateObjectNode(null);
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
				}
				return;
			}
			throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
		}
	}
}
