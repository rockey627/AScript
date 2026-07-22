using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Lua.TokenHandlers
{
	public class LuaLocalTokenHandler : ITokenHandler
	{
		public static readonly LuaLocalTokenHandler Instance = new LuaLocalTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			var token = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			if (token.Value.IsSymbol("function"))
			{
				e.TokenReader.Push(token.Value);
				return;
			}
			if (!e.Ignore)
			{
				var defineVarNode = PoolManage.CreateDefineVarNode(token.Value.Value, null, typeof(object));
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineVarNode);
			}
		}
	}
}
