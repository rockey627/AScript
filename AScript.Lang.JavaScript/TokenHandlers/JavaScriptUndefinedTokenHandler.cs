using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	public class JavaScriptUndefinedTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptUndefinedTokenHandler Instance = new JavaScriptUndefinedTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(JavaScriptUndefined.Instance));
			}
		}
	}
}
