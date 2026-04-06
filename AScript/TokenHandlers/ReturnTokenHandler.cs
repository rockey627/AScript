using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class ReturnTokenHandler : ITokenHandler
	{
		public static readonly ReturnTokenHandler Instance = new ReturnTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var returnBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, null, e.Ignore);

			if (e.Ignore)
			{
				return;
			}

			if (e.Options.CreateFullTreeNode ?? false)
			{
				var returnNode = new ReturnNode { Body = returnBuilder };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, returnNode);
				return;
			}

			if (e.Control == null)
			{
				throw new Exception("unsupport return");
			}
			e.Control.Terminal = true;
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, returnBuilder);
		}
	}
}
