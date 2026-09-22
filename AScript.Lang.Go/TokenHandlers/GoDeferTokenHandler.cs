using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	public class GoDeferTokenHandler : ITokenHandler
	{
		public static readonly GoDeferTokenHandler Instance = new GoDeferTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var deferStatements = e.Ignore ? null : new List<ITreeNode>();
			while (true)
			{
				var deferStatement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				deferStatements?.Insert(0, deferStatement);
				var deferToken = e.TokenReader.Read();
				if (!deferToken.HasValue) break;
				if (deferToken.Value.IsSymbol(e.CurrentToken.Value))
				{
					continue;
				}
				e.TokenReader.Push(deferToken.Value);
				break;
			}

			var multiStatement = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			if (!e.Ignore)
			{
				var tryNode = new TryNode
				{
					TryBody = multiStatement,
					FinallyBody = deferStatements.Count == 1 ? deferStatements[0] : new MultiNode { Nodes = deferStatements }
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, tryNode);
			}
		}
	}
}
