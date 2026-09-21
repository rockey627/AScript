using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言import声明处理器
	/// import "path"
	/// import "path"
	/// import name "path"
	/// import ( "path1" "path2" )
	/// </summary>
	public class GoImportTokenHandler : ITokenHandler
	{
		public static readonly GoImportTokenHandler Instance = new GoImportTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.Type == ETokenType.Word)
			{
				// import name "path"
				var name = nextToken.Value.Value;
				var moduleName = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String);
				if (!e.Ignore)
				{
					var assign = PoolManage.CreateOperatorNode("=", 2, 0);
					assign.Left = PoolManage.CreateDefineVarNode(name, null);
					assign.Right = new CallFuncNode
					{
						Name = e.CurrentToken.Value,
						Args = new ITreeNode[]
						{
							PoolManage.CreateObjectNode(moduleName)
						}
					};
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assign);
				}
				return;
			}
			// 
			var moduleNameNodes = e.Ignore ? null : new List<ITreeNode>();
			if (nextToken.Value.IsSymbol("("))
			{
				while (true)
				{
					var moduleName = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String).Value.Value;
					moduleNameNodes?.Add(PoolManage.CreateObjectNode(moduleName));
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
					if (nextToken.Value.IsSymbol(")")) break;
					e.TokenReader.Push(nextToken.Value);
				}
			}
			else if (nextToken.Value.Type == ETokenType.String)
			{
				var moduleName = nextToken.Value.Value;
				moduleNameNodes?.Add(PoolManage.CreateObjectNode(moduleName));
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
					Args = moduleNameNodes.ToArray()
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, callFuncNode);
			}
		}
	}
}
