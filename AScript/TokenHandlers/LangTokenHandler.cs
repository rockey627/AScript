using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// <para>#lang lang1,lang2</para>
	/// <para>...</para>
	/// <para>#end</para>
	/// </summary>
	public class LangTokenHandler : ITokenHandler, IAsyncTokenHandler
	{
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
					throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken} '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				langList.Add(token.Value.Value);
				var c = charReader.Read();
				if (!c.HasValue) return;
				if (c.Value == ',')
				{
					token = e.TokenReader.Read();
					continue;
				}
				break;
			}
			var oldScriptLangs = e.ScriptContext.Langs;
			var oldTokenStream = e.TokenReader.TokenStream;
			var langs = e.ScriptContext.Langs = langList.ToArray();
			e.TokenReader.TokenStream = e.ScriptContext.GetTokenStream(e.TokenReader.CharReader) ?? oldTokenStream;
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

		public async Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken)
		{
			e.IsHandled = true;
			e.End = true;
			var langList = new List<string>();
			var token = await e.TokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			var charReader = e.TokenReader.CharReader;
			while (token.HasValue)
			{
				if (token.Value.Type != ETokenType.Word && token.Value.Type != ETokenType.String)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken} '{token.Value.Value}' at ({token.Value.Line},{token.Value.Column})");
				}
				langList.Add(token.Value.Value);
				//token = e.TokenReader.Read();
				//if (!token.HasValue) break;
				//if (token.Value.Type == ETokenType.String || token.Value.Value != ",")
				//{
				//	break;
				//}
				var c = await charReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				if (!c.HasValue) return;
				if (c.Value == ',')
				{
					token = await e.TokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					continue;
				}
				//if (!DefaultTokenStream.SpaceChars.Contains(c.Value))
				//{
				//	throw new Exceptions.ScriptAnalyzingException($"invalid {e.CurrentToken} '{c.Value}' at ({charReader.CurrentLine},{charReader.CurrentColumn})");
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
				body = await analyzer2.BuildMultiStatementAsync(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _EndTokens, cancellationToken: cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				e.ScriptContext.Langs = oldScriptLangs;
				e.TokenReader.TokenStream = oldTokenStream;
			}
			await analyzer2.TrySkipNextTokenAsync(e.TokenReader, _EndToken, cancellationToken).ConfigureAwait(false);
			if (!e.Ignore)
			{
				await e.TreeBuilder.AddDataAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, new LangNode { Langs = langs, Body = body }, cancellationToken).ConfigureAwait(false);
			}
		}
	}
}
