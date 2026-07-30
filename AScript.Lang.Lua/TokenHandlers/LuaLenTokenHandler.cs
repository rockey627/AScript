using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Lua.TokenHandlers
{
	public class LuaLenTokenHandler : ITokenHandler
	{
		public static readonly LuaLenTokenHandler Instance = new LuaLenTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			if (!e.Ignore)
			{
				var op = new OperatorNode(e.CurrentToken.Value, DefaultSyntaxAnalyzer.OperatorPriorities["."] - 1, 1)
				{
					Prefix = true
				};
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
			}
		}
	}
}
