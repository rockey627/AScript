using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class ContinueTokenHandler : ITokenHandler
	{
		public static readonly ContinueTokenHandler Instance = new ContinueTokenHandler();

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
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new ContinueNode());
			}
			else
			{
				if (e.Control == null)
				{
					throw new Exception("invalid continue statement");
				}
				e.Control.Continue = true;
			}
		}
	}
}
