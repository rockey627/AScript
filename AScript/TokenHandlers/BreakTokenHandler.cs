using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class BreakTokenHandler : ITokenHandler
	{
		public static readonly BreakTokenHandler Instance = new BreakTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			if (e.Ignore) return;

			if (e.Options.CreateFullTreeNode ?? false)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new BreakNode());
			}
			else
			{
				if (e.Control == null)
				{
					throw new Exception("invalid break statement");
				}
				e.Control.Break = true;
			}
		}
	}
}
