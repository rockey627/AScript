using AScript.Lang.JavaScript.Nodes;
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

			List<string> defaultVariables = null;
			List<JavaScriptImportNode.VariableItem> variables = null;

			while (true)
			{
				var nextToken = analyzer.ValidateNextToken(e.TokenReader);
				if (nextToken.Value.Type == ETokenType.Word)
				{
					if (defaultVariables == null) defaultVariables = new List<string>();
					defaultVariables.Add(nextToken.Value.Value);
				}
				else if (nextToken.Value.IsSymbol("{"))
				{
					if (variables == null) variables = new List<JavaScriptImportNode.VariableItem>();
					BuildDestructuringVariables(analyzer, e, variables);
				}
				// 
				nextToken = analyzer.ValidateNextToken(e.TokenReader);
				if (nextToken.Value.IsSymbol(",")) continue;
				if (nextToken.Value.IsSymbol("from")) break;
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' near 'import' at ({nextToken.Value.Line},{nextToken.Value.Column}), expect from");
			}

			var module = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String);
			if (!e.Ignore)
			{
				string moduleName = module.Value.Value;
				var importNode = new JavaScriptImportNode
				{
					DefaultVariables = defaultVariables,
					Variables = variables,
					FromModule = moduleName
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, importNode);
			}

			// 查看下一个token判断是默认导入还是解构导入
			//var nextToken = analyzer.ValidateNextToken(e.TokenReader);

			//if (nextToken.Value.IsSymbol("{"))
			//{
			//	// 解构导入: import { a, b as b1 } from 'modulename'
			//	BuildDestructuringImport(analyzer, e);
			//}
			//else if (nextToken.Value.Type == ETokenType.Word)
			//{
			//	// 默认导入: import m from 'modulename'
			//	BuildDefaultImport(analyzer, e, nextToken.Value.Value);
			//}
			//else
			//{
			//	throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({nextToken.Value.Line},{nextToken.Value.Column}), expect identifier or '{{'");
			//}
		}

		//private void BuildDefaultImport(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, string varName)
		//{
		//	// 验证 from 关键字
		//	analyzer.ValidateNextToken(e.TokenReader, "from");

		//	// 读取模块名
		//	var moduleToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String);
		//	string moduleName = moduleToken.Value.Value;

		//	if (!e.Ignore)
		//	{
		//		var moduleContext = new ScriptContext { Langs = e.ScriptContext.Langs };
		//		var module = moduleContext.InstallModule(moduleName);

		//		// 创建赋值操作节点：varName = require(moduleName)
		//		var assignNode = PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]);
		//		assignNode.Left = PoolManage.CreateDefineVarNode(varName, null, systemType: typeof(object));
		//		assignNode.Right = PoolManage.CreateObjectNode(module);

		//		e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assignNode);
		//	}
		//}

		private void BuildDestructuringVariables(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, List<JavaScriptImportNode.VariableItem> variables)
		{
			// 解析 { a, b as b1 }
			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.IsSymbol("}")) return;
			e.TokenReader.Push(nextToken.Value);
			while (true)
			{
				var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				string name = nameToken.Value.Value;
				string alias = name;

				nextToken = analyzer.ValidateNextToken(e.TokenReader);
				if (nextToken.Value.IsSymbol("as"))
				{
					var aliasToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					alias = aliasToken.Value.Value;
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
				}

				variables.Add(new JavaScriptImportNode.VariableItem(name, alias));

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

			//// 验证 from 关键字
			//analyzer.ValidateNextToken(e.TokenReader, "from");

			//// 读取模块名
			//var moduleToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String);
			//string moduleName = moduleToken.Value.Value;

			//if (!e.Ignore)
			//{
			//	//// 创建 require 调用节点
			//	//var requireCall = new CallFuncNode
			//	//{
			//	//	Name = "require",
			//	//	Args = new ITreeNode[]
			//	//	{
			//	//		PoolManage.CreateObjectNode(moduleName)
			//	//	}
			//	//};
			//	var module = new ScriptContext(e.ScriptContext).InstallModule(moduleName);

			//	var statements = new List<ITreeNode>();

			//	if (imports.Count > 0)
			//	{
			//		// 多个导入: import { add, multiply } from 'mymodule'
			//		// 为每个导入创建赋值语句
			//		foreach (var item in imports)
			//		{
			//			var assignNode = PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]);
			//			assignNode.Left = PoolManage.CreateDefineVarNode(item.Alias, null, systemType: typeof(object));
			//			assignNode.Right = CreatePropertyAccess(PoolManage.CreateObjectNode(module), item.Name);
			//			statements.Add(assignNode);
			//		}

			//		var multiNode = new MultiNode { Nodes = statements };
			//		e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, multiNode);
			//	}
			//}
		}

		//private ITreeNode CreatePropertyAccess(ITreeNode target, string propertyName)
		//{
		//	var opNode = PoolManage.CreateOperatorNode(".", 2, DefaultSyntaxAnalyzer.OperatorPriorities["."]);
		//	opNode.Left = target;
		//	opNode.Right = PoolManage.CreateVariableNode(propertyName);
		//	return opNode;
		//}
	}
}
