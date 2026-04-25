using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Python3.TokenHandlers
{
	/// <summary>
	/// for x in [1, 2, 3]:
	/// </summary>
	public class Python3ForTokenHandler : ITokenHandler
	{
		public static readonly Python3ForTokenHandler Instance = new Python3ForTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exception($"invalid {e.CurrentToken.Value} expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			if (nextToken.Value.Type != ETokenType.Word)
			{
				throw new Exception($"invalid '{nextToken.Value.Value}' of {e.CurrentToken.Value} expression at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			var varName = nextToken.Value.Value;
			List<DefineVarNode> varDefines = null;

			// 检查是否是变量名列表 (for x, y, z in ...)
			nextToken = e.TokenReader.Read();

			if (nextToken.HasValue && nextToken.Value.Type != ETokenType.String && nextToken.Value.Value == ",")
			{
				varDefines = new List<DefineVarNode>();
				varDefines.Add(PoolManage.CreateDefineVarNode(varName, null, typeof(object)));
				while (nextToken.HasValue && nextToken.Value.Type != ETokenType.String && nextToken.Value.Value == ",")
				{
					var varToken = e.TokenReader.Read();
					if (!varToken.HasValue || varToken.Value.Type != ETokenType.Word || varToken.Value.Value == "in")
					{
						throw new Exception($"invalid variable name at ({varToken.Value.Line},{varToken.Value.Column})");
					}
					varName = varToken.Value.Value;
					varDefines.Add(PoolManage.CreateDefineVarNode(varName, null, typeof(object)));

					nextToken = e.TokenReader.Read();
				}
			}

			if (!nextToken.HasValue)
			{
				throw new Exception($"invalid {e.CurrentToken.Value} expression at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
			}
			if (nextToken.Value.Type == ETokenType.String || nextToken.Value.Value != "in")
			{
				throw new Exception($"invalid {nextToken.Value.Value} of {e.CurrentToken.Value} expression at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			//analyzer.ValidateNextToken(e.TokenReader, "in");
			// 
			var listBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: Python3Lang.EndTokens);
			analyzer.ValidateNextToken(e.TokenReader, ":");
			var createFullTreeNodeOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var bodyBuilder = Python3Lang.BuildSubBlock(e.CurrentToken.Column, analyzer, e.BuildContext, e.ScriptContext, createFullTreeNodeOptions, e.TokenReader, e.Control, e.Ignore, endTokens: Python3Lang.EndTokens);

			if (e.Ignore) return;

			var foreachNode = new ForeachNode
			{
				VarDefine = varDefines == null ? PoolManage.CreateDefineVarNode(varName, null, typeof(object)) : null,
				VarDefines = varDefines,
				Collection = listBuilder,
				Body = bodyBuilder
			};
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, foreachNode);
		}
	}
}
