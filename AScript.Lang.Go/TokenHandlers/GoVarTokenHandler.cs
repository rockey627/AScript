using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言变量声明处理器
	/// var name type = value
	/// var name = value
	/// var name type
	/// var name1, name2 = value1, value2
	/// </summary>
	public class GoVarTokenHandler : ITokenHandler
	{
		public static readonly GoVarTokenHandler Instance = new GoVarTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			string varName = nameToken.Value.Value;

			// 检查是否有类型注解或赋值
			var token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				if (!e.Ignore)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateDefineVarNode(varName, null, typeof(object)));
				}
				return;
			}

			if (token.Value.Type == ETokenType.Word)
			{
				// 类型判断
				string typeName = token.Value.Value;
				var type = e.ScriptContext.EvalType(typeName);
				if (type == null)
				{
					e.End = true;
					e.TokenReader.Push(token.Value);
					typeName = null;
					type = typeof(object);
				}
				if (!e.Ignore)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateDefineVarNode(varName, typeName, type));
				}
				return;
			}

			e.TokenReader.Push(token.Value);

			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateDefineVarNode(varName, null, typeof(object)));
			}
		}
	}
}
