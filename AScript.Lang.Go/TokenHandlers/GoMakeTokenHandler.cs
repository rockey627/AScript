using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言make函数调用处理器
	/// make(chan Type, bufferSize)
	/// make(map[K]V)
	/// make([]Type, length, capacity)
	/// </summary>
	public class GoMakeTokenHandler : ITokenHandler
	{
		public static readonly GoMakeTokenHandler Instance = new GoMakeTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			// make是一个函数调用，直接解析函数调用
			var call = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);

			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, call);
			}
		}
	}
}
