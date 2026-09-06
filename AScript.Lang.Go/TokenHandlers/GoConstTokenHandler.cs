using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言常量声明处理器
	/// const name type = value
	/// const name = value
	/// const name1, name2 = value1, value2
	/// </summary>
	public class GoConstTokenHandler : ITokenHandler
	{
		public static readonly GoConstTokenHandler Instance = new GoConstTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// const name [type] = value
			var nameToken = e.TokenReader.Read();
			if (!nameToken.HasValue || nameToken.Value.Type != ETokenType.Word)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid const name at ({nameToken.Value.Line},{nameToken.Value.Column})");
			}
			string varName = nameToken.Value.Value;

			// 检查是否有类型或赋值
			var token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid const declaration for {varName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}

			string typeName = null;
			ITreeNode value = null;

			if (token.Value.Type == ETokenType.Word && !token.Value.IsSymbol("="))
			{
				// 类型注解
				typeName = token.Value.Value;
				token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					return;
				}
			}

			if (token.Value.IsSymbol("="))
			{
				value = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			}
			else
			{
				e.TokenReader.Push(token.Value);
			}

			if (!e.Ignore)
			{
				var systemType = typeof(object);
				if (!string.IsNullOrEmpty(typeName))
				{
					systemType = e.ScriptContext.EvalType(typeName) ?? typeof(object);
				}
				var varNode = PoolManage.CreateDefineVarNode(varName, typeName, systemType: systemType);
				if (value != null)
				{
					var opNode = PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]);
					opNode.Left = varNode;
					opNode.Right = value;
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, opNode);
				}
			}
		}
	}
}
