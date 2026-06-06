using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// IF(condition, trueValue, falseValue)
	/// </summary>
	public class SqlIfTokenHandler : ITokenHandler
	{
		public static readonly SqlIfTokenHandler Instance = new SqlIfTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			analyzer.ValidateNextToken(e.TokenReader, "(");
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ",");
			var trueValue = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ",");
			var falseValue = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			if (!e.Ignore)
			{
				var ifNode = new IfNode { ReturnValue = true, Condition = condition, Body = trueValue, Else = falseValue };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, ifNode);
			}
		}
	}
}
