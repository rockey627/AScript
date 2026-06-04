using AScript.Lang.Sql.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// delete from table1 where id='1001'
	/// </summary>
	public class SqlDeleteTokenHandler : ITokenHandler
	{
		public static readonly SqlDeleteTokenHandler Instance = new SqlDeleteTokenHandler();

		private static readonly HashSet<string> _TableEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "where" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			analyzer.ValidateNextToken(e.TokenReader, "from", StringComparison.OrdinalIgnoreCase);

			var table = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, _TableEndTokens);
			ITreeNode condition = null;
			var token = e.TokenReader.Read();
			if (token.HasValue)
			{
				if (token.Value.IsSymbol("where", StringComparison.OrdinalIgnoreCase))
				{
					var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
					condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				}
				else
				{
					e.TokenReader.Push(token.Value);
				}
			}

			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new SqlDeleteNode { Source = table, Condition = condition });
			}
		}
	}
}
