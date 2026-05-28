using AScript.Lang.Sql.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// insert into xxx (column1, column2, ...) values (value11, value12, ...),(value21, value22, ...),...
	/// </summary>
	public class SqlInsertTokenHandler : ITokenHandler
	{
		public static readonly SqlInsertTokenHandler Instance = new SqlInsertTokenHandler();

		private static readonly HashSet<string> _TableEndTokens = new HashSet<string> { "(" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			analyzer.ValidateNextToken(e.TokenReader, "into", StringComparison.OrdinalIgnoreCase);
			var table = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _TableEndTokens);
			var columns = new List<string>();
			analyzer.ValidateNextToken(e.TokenReader, "(");
			while (true)
			{
				var token = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				columns.Add(token.Value.Value);
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression near insert at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
				}
				if (token.Value.IsSymbol(","))
				{
					continue;
				}
				if (token.Value.IsSymbol(")")) break;
				throw new Exceptions.ScriptAnalyzingException($"invalid expression {token.Value.Value} at ({token.Value.Line},{token.Value.Column})");
			}

			analyzer.ValidateNextToken(e.TokenReader, "values", StringComparison.OrdinalIgnoreCase);
			var values = new List<IList<ITreeNode>>();
			while (true)
			{
				analyzer.ValidateNextToken(e.TokenReader, "(");
				var rowValues = new List<ITreeNode>();
				while (true)
				{
					var valueNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					rowValues.Add(valueNode);
					var token = e.TokenReader.Read();
					if (!token.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near insert at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
					}
					if (token.Value.IsSymbol(","))
					{
						continue;
					}
					if (token.Value.IsSymbol(")")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression {token.Value.Value} at ({token.Value.Line},{token.Value.Column})");
				}
				values.Add(rowValues);

				var nextToken = e.TokenReader.Read();
				if (!nextToken.HasValue) break;
				if (nextToken.Value.IsSymbol(","))
				{
					continue;
				}

				e.TokenReader.Push(nextToken.Value);
				break;
			}

			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new SqlInsertNode
				{
					Source = table,
					Columns = columns,
					Values = values
				});
			}
		}
	}
}
