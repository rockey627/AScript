using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	/// <summary>
	/// var n=10+x;
	/// var {code,name}=person;
	/// </summary>
	public class JavaScriptVarTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptVarTokenHandler Instance = new JavaScriptVarTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.Type == ETokenType.Word)
			{
				if (!e.Ignore)
				{
					var defineVarNode = PoolManage.CreateDefineVarNode(nextToken.Value.Value, null, systemType: typeof(object));
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineVarNode);
				}
				return;
			}

			if (nextToken.Value.IsSymbol("{"))
			{
				var list = e.Ignore ? null : new List<ITreeNode>();
				while (true)
				{
					nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					list?.Add(PoolManage.CreateDefineVarNode(nextToken.Value.Value, null, systemType: typeof(object)));
					nextToken = analyzer.ValidateNextToken(e.TokenReader);
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol("}")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
				if (!e.Ignore)
				{
					var node = new CallFuncNode { Name = "var", Args = list.ToArray() };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, node);
				}
				analyzer.ValidateNextToken(e.TokenReader, "=");
				if (!e.Ignore)
				{
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateOperatorNode("=", 2, DefaultSyntaxAnalyzer.OperatorPriorities["="]));
				}
				return;
			}

			throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
		}
	}
}
