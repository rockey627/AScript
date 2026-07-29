using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

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

			// 检查是否有逗号，即多变量声明如 local a,b=1,2
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue && nextToken.Value.IsSymbol(","))
			{
				// 多变量声明，创建 TupleNode
				var varNames = e.Ignore ? null : new List<string> { token.Value.Value };
				bool hasAssign = false;
				while (true)
				{
					var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					varNames?.Add(nameToken.Value.Value);

					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						break;
					}
					if (nextToken.Value.IsSymbol(","))
					{
						continue;
					}
					if (nextToken.Value.IsSymbol("="))
					{
						hasAssign = true;
						break;
					}
					e.TokenReader.Push(nextToken.Value);
					break;
				}

				if (hasAssign)
				{
					var values = e.Ignore ? null : new List<ITreeNode>();
					while (true)
					{
						var statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
						values?.Add(statement);
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue) break;
						if (nextToken.Value.IsSymbol(",")) continue;
						e.TokenReader.Push(nextToken.Value);
						break;
					}
					if (!e.Ignore)
					{
						// 创建 TupleNode 包含所有 DefineVarNode
						var defineItems = new List<ITreeNode>();
						foreach (var varName in varNames)
						{
							defineItems.Add(PoolManage.CreateDefineVarNode(varName, null, typeof(object)));
						}
						var defineNode = new TupleNode { Items = defineItems };
						// 创建values
						var valueNode = new TupleNode { Items = values };
						var assignNode = PoolManage.CreateOperatorNode("=", 2, 0);
						assignNode.Left = defineNode;
						assignNode.Right = valueNode;
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assignNode);
					}
				}
				else if (!e.Ignore)
				{
					// 创建 TupleNode 包含所有 DefineVarNode
					var tupleItems = new List<ITreeNode>();
					foreach (var varName in varNames)
					{
						tupleItems.Add(PoolManage.CreateDefineVarNode(varName, null, typeof(object)));
					}
					var tupleNode = new TupleNode { Items = tupleItems };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, tupleNode);
				}
				return;
			}
			else
			{
				// 单变量声明不赋值 local x
				if (nextToken.HasValue)
				{
					e.TokenReader.Push(nextToken.Value);
				}
				if (!e.Ignore)
				{
					var defineVarNode = PoolManage.CreateDefineVarNode(token.Value.Value, null, typeof(object));
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineVarNode);
				}
			}
		}
	}
}
