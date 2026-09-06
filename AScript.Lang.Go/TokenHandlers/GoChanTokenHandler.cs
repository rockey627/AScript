using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言chan类型声明处理器
	/// chan Type
	/// <-chan Type
	/// chan<- Type
	/// </summary>
	public class GoChanTokenHandler : ITokenHandler
	{
		public static readonly GoChanTokenHandler Instance = new GoChanTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			var token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid chan expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}

			// chan Type 或 chan<- Type 或 <-chan Type
			if (token.Value.Value == "<-")
			{
				// <-chan Type
				token = e.TokenReader.Read();
				if (!token.HasValue || token.Value.Value != "chan")
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid chan expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
				}
				token = e.TokenReader.Read();
				// element type
			}
			else if (token.Value.Value == "chan")
			{
				token = e.TokenReader.Read();
				if (token.HasValue && token.Value.Value == "<-")
				{
					// chan<- Type
					token = e.TokenReader.Read();
				}
				// element type
			}
			else
			{
				e.TokenReader.Push(token.Value);
			}

			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new VariableNode("chan"));
			}
		}
	}
}
