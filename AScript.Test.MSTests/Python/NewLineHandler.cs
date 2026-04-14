using AScript.Syntaxs;
using System;

namespace AScript.Test.MSTests.Python
{
	/// <summary>
	/// 由于AScript解析器会忽略换行符，所以这里要手动插入换行token
	/// </summary>
	public class NewLineHandler : ITokenHandler
	{
		public static readonly NewLineHandler Instance = new NewLineHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue)
			{
				e.TokenReader.Push(nextToken.Value);
				if (nextToken.Value.Line != e.CurrentToken.Line)
				{
					e.TokenReader.Push(new Token("\n", ETokenType.None));
				}
			}
		}
	}
}
