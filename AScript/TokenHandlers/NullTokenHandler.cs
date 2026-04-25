using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Linq.Expressions;

namespace AScript.TokenHandlers
{
	public class NullTokenHandler : ITokenHandler
	{
		public static readonly NullTokenHandler Instance = new NullTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.Ignore) return;
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(null));
		}
	}
}
