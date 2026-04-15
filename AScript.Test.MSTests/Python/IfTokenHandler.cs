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

			// if语句中不能立即执行，所以要先创建完整的if表达式树再编译或执行
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: PythonLang.EndTokens);
			analyzer.ValidateNextToken(e.TokenReader, ":");
			ITreeNode bodyNode = PythonLang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: PythonLang.EndTokens);
			//
			ITreeNode elseNode = null;
			var token = e.TokenReader.Read();
			if (token.HasValue && token.Value.Column == e.CurrentToken.Column)
			{
				if (token.Value.Type != ETokenType.String && token.Value.Value == "elif")
				{
					// else if
					e.TokenReader.Push(new Token("if", ETokenType.Word, token.Value.Line, token.Value.Column));
					elseNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: PythonLang.EndTokens);
				}
				else if (token.Value.Type != ETokenType.String && token.Value.Value == "else")
				{
					analyzer.ValidateNextToken(e.TokenReader, ":");
					elseNode = PythonLang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, endTokens: PythonLang.EndTokens);
				}
				else
				{
					e.TokenReader.Push(token.Value);
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
