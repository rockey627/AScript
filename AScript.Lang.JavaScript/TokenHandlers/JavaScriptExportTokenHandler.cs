using AScript.Lang.JavaScript.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	/// <summary>
	/// <para>export default { }</para>
	/// <para>export const n = 5</para>
	/// </summary>
	public class JavaScriptExportTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptExportTokenHandler Instance = new JavaScriptExportTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.IsSymbol("default"))
			{
				var obj1 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				if (!e.Ignore)
				{
					var exportNode = new JavaScriptExportNode { Default = true, Value = obj1 };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, exportNode);
				}
				return;
			}

			// export const name = '';
			// export function sum(a, b) { a + b }
			if (nextToken.Value.IsSymbol("const")
				|| nextToken.Value.IsSymbol("var")
				|| nextToken.Value.IsSymbol("let")
				|| nextToken.Value.IsSymbol("function"))
			{
				var varNameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				string varName2 = varNameToken.Value.Value;
				e.TokenReader.Push(varNameToken.Value);
				e.TokenReader.Push(nextToken.Value);
				var obj2 = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				if (!e.Ignore)
				{
					var exportNode = new JavaScriptExportNode { Name = varName2, Value = obj2 };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, exportNode);
				}
				return;
			}

			// export add = (a, b) => a + b;
			var nextToken2 = analyzer.ValidateNextToken(e.TokenReader, "=");
			string varName = nextToken.Value.Value;
			e.TokenReader.Push(nextToken2.Value);
			e.TokenReader.Push(nextToken.Value);
			var obj = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			if (!e.Ignore)
			{
				var exportNode = new JavaScriptExportNode { Name = varName, Value = obj };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, exportNode);
			}
		}



	}
}
