using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// from a in query1
	/// </summary>
	public class FromTokenHandler : ITokenHandler
	{
		public static readonly FromTokenHandler Instance = new FromTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			//if (e.TreeBuilder.IsFullStatement())
			//{
			//	e.End = true;
			//	e.TokenReader.Push(e.CurrentToken);
			//	return;
			//}

			var varToken = e.TokenReader.Read();
			if (!varToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at {e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}");
			}
			if (varToken.Value.Type != ETokenType.Word)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' {varToken.Value.Value} at {varToken.Value.Line},{varToken.Value.Column}");
			}
			analyzer.ValidateNextToken(e.TokenReader, "in");
			var buildOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var source = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, buildOptions, e.TokenReader, e.Control, e.Ignore, QueryNode.Keywords);

			if (e.Ignore) return;

			if (e.TreeBuilder.Current is QueryNode qNode)
			{
				qNode.AddFrom(varToken.Value.Value, source);
			}
			else
			{
				var queryNode = new QueryNode();
				queryNode.AddFrom(varToken.Value.Value, source);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, queryNode);
			}
		}
	}
}
