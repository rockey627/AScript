using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// repeat
	///     body
	/// until condition
	/// </summary>
	public class LuaRepeatTokenHandler : ITokenHandler
	{
		public static readonly LuaRepeatTokenHandler Instance = new LuaRepeatTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, LuaLang.EndTokens_until);
			analyzer.ValidateNextToken(e.TokenReader, "until");
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);

			if (!e.Ignore)
			{
				// repeat...until 等价于 do { body } while(!condition)
				// 即条件为真时退出循环，等价于 IsDoWhile=true 但条件取反
				var notCondition = new OperatorNode
				{
					Name = "not",
					Prefix = true,
					Right = condition
				};
				var whileNode = new WhileNode { Condition = notCondition, Body = body, IsDoWhile = true };
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, whileNode);
			}
		}
	}
}
