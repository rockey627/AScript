using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Test.MSTests.Python
{
	/// <summary>
	/// <para>if expression :</para>
	/// <para>    statement1</para>
	/// <para>    statement2</para>
	/// <para>elif expression :</para>
	/// <para>    statement1</para>
	/// <para>    statement2</para>
	/// <para>else :</para>
	/// <para>    statement1</para>
	/// <para>    statement2</para>
	/// <para>other</para>
	/// </summary>
	public class IfTokenHandler : ITokenHandler
	{
		public static readonly IfTokenHandler Instance = new IfTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: PythonLang.EndTokens);
			analyzer.ValidateNextToken(e.TokenReader, ":");
			analyzer.TrySkipNextToken(e.TokenReader, "\n");
			ITreeNode bodyNode = null;
			ITreeNode elseNode = null;
			var token = e.TokenReader.Read();
			if (token.HasValue && token.Value.Column > e.CurrentToken.Column)
			{
				TreeBuilder builder = new TreeBuilder();
				int column = token.Value.Column;
				while (token.HasValue && token.Value.Column == column)
				{
					e.TokenReader.Push(token.Value);
					var statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: PythonLang.EndTokens);
					builder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, statement);
					analyzer.TrySkipNextToken(e.TokenReader, "\n");
					token = e.TokenReader.Read();
				}
				bodyNode = builder;
			}
			// 
			if (token.HasValue && token.Value.Column == e.CurrentToken.Column)
			{
				if (token.Value.Type != ETokenType.String && token.Value.Value == "elif")
				{
					// else if
					e.TokenReader.Push(new Token("if", ETokenType.Word, token.Value.Line, token.Value.Column));
					elseNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: PythonLang.EndTokens);
				}
				else if (token.Value.Type != ETokenType.String && token.Value.Value == "else")
				{
					analyzer.ValidateNextToken(e.TokenReader, ":");
					analyzer.TrySkipNextToken(e.TokenReader, "\n");

					token = e.TokenReader.Read();
					if (token.HasValue && token.Value.Column > e.CurrentToken.Column)
					{
						TreeBuilder builder = new TreeBuilder();
						int column = token.Value.Column;
						while (token.HasValue && token.Value.Column == column)
						{
							e.TokenReader.Push(token.Value);
							var statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: PythonLang.EndTokens);
							builder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, statement);
							analyzer.TrySkipNextToken(e.TokenReader, "\n");
							token = e.TokenReader.Read();
						}
						elseNode = builder;
					}
				}
			}
			else
			{
				e.TokenReader.Push(token.Value);
			}

			e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, new IfNode { Condition = condition, Body = bodyNode, Else = elseNode });
		}


	}
}
