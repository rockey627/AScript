using AScript.Lang.Sql.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// update table1 set Name='tom',Age=20 where id='1001'
	/// </summary>
	public class SqlUpdateTokenHandler : ITokenHandler
	{
		public static readonly SqlUpdateTokenHandler Instance = new SqlUpdateTokenHandler();

		private static readonly HashSet<string> _SetEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "where" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			var tableNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _SetEndTokens);
			analyzer.ValidateNextToken(e.TokenReader, "set", StringComparison.OrdinalIgnoreCase);

			var fields = e.Ignore ? null : new List<string>();
			var values = e.Ignore ? null : new List<ITreeNode>();
			ITreeNode condition = null;
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };

			while (true)
			{
				var fieldToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				fields?.Add(fieldToken.Value.Value);

				analyzer.ValidateNextToken(e.TokenReader, "=");

				var valueNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				values?.Add(valueNode);

				var token = e.TokenReader.Read();
				if (!token.HasValue) break;
				if (token.Value.IsSymbol(","))
				{
					continue;
				}
				if (token.Value.IsSymbol("where", StringComparison.OrdinalIgnoreCase))
				{
					condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					break;
				}
				e.TokenReader.Push(token.Value);
				break;
			}

			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new SqlUpdateNode
				{
					Source = tableNode,
					Fields = fields,
					Values = values,
					Condition = condition
				});
			}
		}
	}
}
