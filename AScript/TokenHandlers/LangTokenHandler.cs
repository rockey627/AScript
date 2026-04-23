using AScript.Nodes;
using AScript.Readers;
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
		public static readonly LangTokenHandler Instance = new LangTokenHandler("#end");

		private string _EndToken;
		private readonly HashSet<string> _EndTokens;

		public LangTokenHandler(string endToken)
		{
			_EndToken = endToken;
			_EndTokens = new HashSet<string> { endToken };
		}

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			var langList = new List<string>();
			var token = e.TokenReader.Read();
			var charReader = e.TokenReader.CharReader;
			while (token.HasValue)
			{
				if (token.Value.Type != ETokenType.Word && token.Value.Type != ETokenType.String)
				{
					throw new Exception($"invalid {e.CurrentToken} '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				langList.Add(token.Value.Value);
				//token = e.TokenReader.Read();
				//if (!token.HasValue) break;
				//if (token.Value.Type == ETokenType.String || token.Value.Value != ",")
				//{
				//	break;
				//}
				var c = charReader.Read();
				if (!c.HasValue) return;
				if (c.Value == ',')
				{
					token = e.TokenReader.Read();
					continue;
				}
				//if (!DefaultTokenStream.SpaceChars.Contains(c.Value))
				//{
				//	throw new Exception($"invalid {e.CurrentToken} '{c.Value}' at ({charReader.CurrentLine},{charReader.CurrentColumn})");
				//}
				break;
			}
			//if (token.HasValue)
			//{
			//	e.TokenReader.Push(token.Value);
			//}
			var oldScriptLangs = e.ScriptContext.Langs;
			var oldTokenStream = e.TokenReader.TokenStream;
			var langs = e.ScriptContext.Langs = langList.ToArray();
			e.TokenReader.TokenStream = e.ScriptContext.GetTokenStream(charReader) ?? oldTokenStream;
			var analyzer2 = (DefaultSyntaxAnalyzer)(e.ScriptContext.GetSyntaxAnalyzer() ?? analyzer);
			ITreeNode body;
			try
			{
				body = analyzer2.BuildMultiStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _EndTokens);
			}
			finally
			{
				e.ScriptContext.Langs = oldScriptLangs;
				e.TokenReader.TokenStream = oldTokenStream;
			}
			analyzer2.TrySkipNextToken(e.TokenReader, _EndToken);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new LangNode { Langs = langs, Body = body });
			}
		}
	}
}
