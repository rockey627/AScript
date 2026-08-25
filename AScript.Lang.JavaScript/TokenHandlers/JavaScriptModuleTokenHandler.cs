using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	public class JavaScriptModuleTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptModuleTokenHandler Instance = new JavaScriptModuleTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			if (!e.Ignore)
			{
				var module = JavaScriptExportModule.GetOrCreateInstance(e.ScriptContext);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(module));
			}
		}
	}
}
