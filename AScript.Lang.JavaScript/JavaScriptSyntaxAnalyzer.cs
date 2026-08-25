using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace AScript.Lang.JavaScript
{
	public class JavaScriptSyntaxAnalyzer : DefaultSyntaxAnalyzer
	{
		public static readonly JavaScriptSyntaxAnalyzer Instance = new JavaScriptSyntaxAnalyzer();

		protected override ITreeNode BuildBlock(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
		{
			// 校验是否对象
			var wordToken = tokenReader.Read();
			if (!wordToken.HasValue) return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
			if (wordToken.Value.IsSymbol("}"))
			{
				// 空对象
				return PoolManage.CreateObjectNode(new ExpandoObject());
			}
			if (wordToken.Value.Type != ETokenType.Word && wordToken.Value.Type != ETokenType.String)
			{
				tokenReader.Push(wordToken.Value);
				return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
			}
			var token2 = tokenReader.Read();
			if (!token2.HasValue)
			{
				tokenReader.Push(wordToken.Value);
				return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
			}
			tokenReader.Push(token2.Value);
			tokenReader.Push(wordToken.Value);
			if (!token2.Value.IsSymbol(":") && !token2.Value.IsSymbol(","))
			{
				return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
			}
			return BuildObject(buildContext, scriptContext, options, tokenReader, control, ignore);

			//// 解析字典：{ key1:value1, key2:value2 }
			//var initProperties = ignore ? null : new List<ITreeNode>();
			//while (true)
			//{
			//	var nameToken = ValidateNextToken(tokenReader, ETokenType.Word | ETokenType.String);
			//	ValidateNextToken(tokenReader, ":");
			//	var value = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
			//	initProperties?.Add(new OperatorNode("=", OperatorPriorities["="], 2)
			//	{
			//		Left = new VariableNode(nameToken.Value.Value),
			//		Right = value
			//	});
			//	var nextToken = tokenReader.Read();
			//	if (!nextToken.HasValue)
			//	{
			//		throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect )");
			//	}
			//	if (nextToken.Value.IsSymbol(",")) continue;
			//	if (nextToken.Value.IsSymbol("}")) break;
			//	throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			//}

			//if (ignore) return null;
			//return new NewNode { SystemType = typeof(ExpandoObject), InitProperties = initProperties };
		}

		protected override object EvalNumber(string num)
		{
			return ScriptUtils.EvalNumber(num, true);
		}

		private NewNode BuildObject(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore)
		{
			//analyzer.ValidateNextToken(e.TokenReader, "{");
			//var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			//if (nextToken.Value.IsSymbol("}"))
			//{
			//	if (e.Ignore) return null;
			//	return new NewNode { SystemType = typeof(ExpandoObject) };
			//}
			//e.TokenReader.Push(nextToken.Value);
			var initProperties = ignore ? null : new List<ITreeNode>();
			Token? nextToken;
			while (true)
			{
				var token = ValidateNextToken(tokenReader, ETokenType.Word | ETokenType.String);
				if (token.Value.IsSymbol("function"))
				{
					var funcNameToken = ValidateNextToken(tokenReader, ETokenType.Word);
					var funcName = funcNameToken.Value.Value;
					tokenReader.Push(funcNameToken.Value);
					tokenReader.Push(token.Value);
					var func = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
					initProperties?.Add(new OperatorNode("=", OperatorPriorities["="], 2)
					{
						Left = new VariableNode(funcName),
						Right = func
					});
					// 
					nextToken = tokenReader.Read();
					if (!nextToken.HasValue) break;
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol("}")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
				nextToken = tokenReader.Read();
				if (!nextToken.HasValue || nextToken.Value.IsSymbol("}"))
				{
					initProperties?.Add(new OperatorNode("=", OperatorPriorities["="], 2)
					{
						Left = new VariableNode(token.Value.Value),
						Right = new VariableNode(token.Value.Value)
					});
					break;
				}
				if (nextToken.Value.IsSymbol(","))
				{
					initProperties?.Add(new OperatorNode("=", OperatorPriorities["="], 2)
					{
						Left = new VariableNode(token.Value.Value),
						Right = new VariableNode(token.Value.Value)
					});
					continue;
				}
				if (nextToken.Value.IsSymbol(":"))
				{
					var value = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
					initProperties?.Add(new OperatorNode("=", OperatorPriorities["="], 2)
					{
						Left = new VariableNode(token.Value.Value),
						Right = value
					});
					// 
					nextToken = tokenReader.Read();
					if (!nextToken.HasValue) break;
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol("}")) break;
				}
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			if (ignore) return null;
			return new NewNode { SystemType = typeof(ExpandoObject), InitProperties = initProperties };
		}
	}
}
