using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// <para>两种方式引用模块：</para>
	/// <para>require 'module'</para>
	/// <para>require('module')</para>
	/// </summary>
	public class LuaRequireTokenHandler : ITokenHandler
	{
		public static readonly LuaRequireTokenHandler Instance = new LuaRequireTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			string moduleName;
			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.IsSymbol("("))
			{
				moduleName = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String).Value.Value;
				analyzer.ValidateNextToken(e.TokenReader, ")");
			}
			else if (nextToken.Value.Type == ETokenType.String)
			{
				moduleName = nextToken.Value.Value;
			}
			else
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid '{nextToken.Value.Value}' near '{e.CurrentToken.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			if (!e.Ignore)
			{
				var callFuncNode = new CallFuncNode
				{
					Name = e.CurrentToken.Value,
					Args = new ITreeNode[]
					{
						PoolManage.CreateObjectNode(moduleName)
					}
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, callFuncNode);
			}
		}
	}
}
