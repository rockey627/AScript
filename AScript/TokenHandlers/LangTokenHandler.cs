using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// <para>#lang lang1,lang2</para>
	/// <para>...</para>
	/// <para>#end</para>
	/// </summary>
	public class LangTokenHandler : ITokenHandler
	{
		public static readonly LangTokenHandler Instance = new LangTokenHandler();

		private static readonly HashSet<string> _EndTokens = new HashSet<string> { "#end" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			var langList = new List<string>();
			var token = e.TokenReader.Read();
			while (token.HasValue && (token.Value.Type == ETokenType.Word || token.Value.Type == ETokenType.String))
			{
				langList.Add(token.Value.Value);
				token = e.TokenReader.Read();
				if (!token.HasValue) break;
				if (token.Value.Type == ETokenType.String || token.Value.Value != ",")
				{
					break;
				}
				token = e.TokenReader.Read();
			}
			if (token.HasValue)
			{
				e.TokenReader.Push(token.Value);
			}
			var oldScriptLangs = e.ScriptContext.Langs;
			var langs = e.ScriptContext.Langs = langList.ToArray();
			ITreeNode body;
			try
			{
				body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _EndTokens);
			}
			finally
			{
				e.ScriptContext.Langs = oldScriptLangs;
			}
			analyzer.ValidateNextToken(e.TokenReader, "#end");
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new LangNode { Langs = langs, Body = body });
			}
		}
	}
}
