using AScript.Lang.Sql.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// title like '%happy%'
	/// </summary>
	public class LikeTokenHandler : ITokenHandler
	{
		public static readonly LikeTokenHandler Instance = new LikeTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			var arg1 = e.TreeBuilder.Pop();
			var arg2String = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String).Value.Value;
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new LikeNode { Arg1 = arg1, Arg2 = arg2String });
			}
		}
	}
}
