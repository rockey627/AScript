using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// local varName = value
	/// </summary>
	public class LuaLocalTokenHandler : ITokenHandler
	{
		public static readonly LuaLocalTokenHandler Instance = new LuaLocalTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// 读取变量名
			var token = e.TokenReader.Read();
			if (!token.HasValue || token.Value.Type != ETokenType.Word)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid local variable at ({e.CurrentToken.Line},{e.CurrentToken.Column}), expect variable name");
			}
			string varName = token.Value.Value;

			// 检查下一个token
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid local variable at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}

			// 如果是 = 运算符，进行赋值
			if (nextToken.Value.Value == "=")
			{
				var valueExpr = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: LuaLang.EndTokens);
				if (!e.Ignore)
				{
					var assignNode = PoolManage.CreateOperatorNode("=", 2, 1);
					assignNode.Left = new VariableNode(varName);
					assignNode.Right = valueExpr;
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assignNode);
				}
			}
			else
			{
				// 简单的局部变量声明（不赋值）
				e.TokenReader.Push(nextToken.Value);
				if (!e.Ignore)
				{
					var defNode = PoolManage.CreateDefineVarNode(varName, null, typeof(object));
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defNode);
				}
			}
		}
	}
}
