using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	/// <summary>
	/// <para>import m from 'modulename'</para>
	/// <para>import { a, b as b1 } from 'modulename'</para>
	/// </summary>
	public class JavaScriptImportTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptImportTokenHandler Instance = new JavaScriptImportTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// 查看下一个token判断是默认导入还是解构导入
			var nextToken = analyzer.ValidateNextToken(e.TokenReader);

			if (nextToken.Value.IsSymbol("{"))
			{
				// 解构导入: import { a, b as b1 } from 'modulename'
				BuildDestructuringImport(analyzer, e);
			}
			else if (nextToken.Value.Type == ETokenType.Word)
			{
				// 默认导入: import m from 'modulename'
				BuildDefaultImport(analyzer, e, nextToken.Value.Value);
			}
			else
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({nextToken.Value.Line},{nextToken.Value.Column}), expect identifier or '{{'");
			}
		}

		private void BuildDefaultImport(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, string varName)
		{
			// 验证 from 关键字
			analyzer.ValidateNextToken(e.TokenReader, "from");

			// 读取模块名
			var moduleToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String);
			string moduleName = moduleToken.Value.Value;

			if (!e.Ignore)
			{
				var module = new ScriptContext(e.ScriptContext).InstallModule(moduleName);

				// 创建赋值操作节点：varName = require(moduleName)
				var assignNode = PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]);
				assignNode.Left = PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object));
				assignNode.Right = PoolManage.CreateObjectNode(module);

				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assignNode);
			}
		}

		private void BuildDestructuringImport(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			// 解析 { a, b as b1 }
			var imports = new List<ImportItem>();

			while (true)
			{
				var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				string name = nameToken.Value.Value;
				string alias = name;

				var nextToken = analyzer.ValidateNextToken(e.TokenReader);
				if (nextToken.Value.IsSymbol("as"))
				{
					var aliasToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					alias = aliasToken.Value.Value;
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
				}

				imports.Add(new ImportItem { Name = name, Alias = alias });

				if (nextToken.Value.IsSymbol(","))
				{
					continue;
				}
				if (nextToken.Value.IsSymbol("}"))
				{
					break;
				}
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({nextToken.Value.Line},{nextToken.Value.Column}), expect ',' or '}}'");
			}

			// 验证 from 关键字
			analyzer.ValidateNextToken(e.TokenReader, "from");

			// 读取模块名
			var moduleToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String);
			string moduleName = moduleToken.Value.Value;

			if (!e.Ignore)
			{
				//// 创建 require 调用节点
				//var requireCall = new CallFuncNode
				//{
				//	Name = "require",
				//	Args = new ITreeNode[]
				//	{
				//		PoolManage.CreateObjectNode(moduleName)
				//	}
				//};
				var module = new ScriptContext(e.ScriptContext).InstallModule(moduleName);

				var statements = new List<ITreeNode>();

				if (imports.Count > 0)
				{
					// 多个导入: import { add, multiply } from 'mymodule'
					// 为每个导入创建赋值语句
					foreach (var item in imports)
					{
						var assignNode = PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]);
						assignNode.Left = PoolManage.CreateDefineVarNode(item.Alias, null, systemType: typeof(object));
						assignNode.Right = CreatePropertyAccess(PoolManage.CreateObjectNode(module), item.Name);
						statements.Add(assignNode);
					}

					var multiNode = new MultiNode { Nodes = statements };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, multiNode);
				}
			}
		}

		private ITreeNode CreatePropertyAccess(ITreeNode target, string propertyName)
		{
			var opNode = PoolManage.CreateOperatorNode(".", 2, DefaultSyntaxAnalyzer.OperatorPriorities["."]);
			opNode.Left = target;
			opNode.Right = PoolManage.CreateVariableNode(propertyName);
			return opNode;
		}

		private class ImportItem
		{
			public string Name { get; set; }
			public string Alias { get; set; }
		}
	}
}
