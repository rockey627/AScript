using AScript.Lang.Sql.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// from xxx where xxx
	/// </summary>
	public class FromTokenHandler : ITokenHandler
	{
		public static readonly FromTokenHandler Instance = new FromTokenHandler();

		private static readonly HashSet<string> _EndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "where", "as" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			var table = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _EndTokens);
			string itemName = null;
			ITreeNode where = null;
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue)
			{
				if ("as".Equals(nextToken.Value.Value, StringComparison.OrdinalIgnoreCase))
				{
					nextToken = e.TokenReader.Read();
					itemName = nextToken.Value.Value;
					nextToken = e.TokenReader.Read();
				}
			}
			if (nextToken.HasValue)
			{
				if ("where".Equals(nextToken.Value.Value, StringComparison.OrdinalIgnoreCase))
				{
					var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
					where = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				}
				else
				{
					throw new Exception($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column}), expect 'where'");
				}
			}

#if NETSTANDARD
			var tables = new[]{ (table, itemName)};
#else
			var tables = new[] { Tuple.Create(table, itemName) };
#endif
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new FromNode { Tables = tables, Where = where });
		}
	}
}
