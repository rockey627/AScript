using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AScript.TokenHandlers
{
	public class NewTokenHandler : ITokenHandler
	{
		public static readonly NewTokenHandler Instance = new NewTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			var typeNameToken = e.TokenReader.Read();
			if (!typeNameToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			if (typeNameToken.Value.IsSymbol("{"))
			{
				// 匿名类型
				var initProperties0 = ParseInitProperties(analyzer, e);
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, new NewNode { InitProperties = initProperties0 });
				e.IsHandled = true;
				return;
			}
			if (typeNameToken.Value.IsSymbol("["))
			{
				var lengthStatement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control);
				analyzer.ValidateNextToken(e.TokenReader, "]");
				List<ITreeNode> lengthArgs = null;
				if (lengthStatement != null)
				{
					lengthArgs = new List<ITreeNode> { lengthStatement };
				}
				analyzer.ValidateNextToken(e.TokenReader, "{");
				var initProperties0 = ParseInitProperties(analyzer, e);
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, 
					new NewNode { ArrayDimension = 1, InitProperties = initProperties0, Args = lengthArgs });
				e.IsHandled = true;
				return;
			}
			if (typeNameToken.Value.Type != ETokenType.Word)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{typeNameToken.Value.Value}' at ({typeNameToken.Value.Line},{typeNameToken.Value.Column})");
			}
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{typeNameToken.Value.Value}' at ({typeNameToken.Value.Line},{typeNameToken.Value.Column})");
			}
			List<string> genericTypes = null;
			if (nextToken.Value.IsSymbol("<"))
			{
				// 泛型
				genericTypes = new List<string>();
				while (true)
				{
					nextToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					genericTypes.Add(nextToken.Value.Value);
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{typeNameToken.Value.Value}', expect '>'");
					}
					if (nextToken.Value.Value == ",") continue;
					if (nextToken.Value.Value == ">") break;
				}
				nextToken = e.TokenReader.Read();
			}
			IList<ITreeNode> args = null;
			bool contains = false;
			int dimension = 0;
			// 处理数组类型: Type[] 或 Type[length]
			if (nextToken.HasValue && nextToken.Value.IsSymbol("["))
			{
				contains = true;
				dimension = 1;
				var lengthStatement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control);
				analyzer.ValidateNextToken(e.TokenReader, "]");
				if (lengthStatement != null)
				{
					args = new List<ITreeNode> { lengthStatement };
				}
				//nextToken = e.TokenReader.Read();
				//if (nextToken.HasValue && nextToken.Value.Value != "{")
				//{
				//	e.TokenReader.Back(nextToken.Value);
				//	nextToken = null;
				//}
			}
			else if (nextToken != null && nextToken.Value.IsSymbol("("))
			{
				contains = true;
				args = analyzer.BuildFuncParams(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				//nextToken = e.TokenReader.Read();
				//if (nextToken.HasValue && nextToken.Value.Value != "{")
				//{
				//	e.TokenReader.Back(nextToken.Value);
				//	nextToken = null;
				//}
			}
			if (contains)
			{
				nextToken = e.TokenReader.Read();
				if (nextToken.HasValue && !nextToken.Value.IsSymbol("{"))
				{
					e.TokenReader.Push(nextToken.Value);
					nextToken = null;
				}
			}
			IList<ITreeNode> initProperties = null;
			if (nextToken.HasValue && nextToken.Value.IsSymbol("{"))
			{
				contains = true;
				initProperties = ParseInitProperties(analyzer, e);
			}

			if (!contains)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}

			e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, new NewNode { Name = typeNameToken.Value.Value, GenericTypes = genericTypes, Args = args, ArrayDimension = dimension, InitProperties = initProperties });
			e.IsHandled = true;
		}

		private IList<ITreeNode> ParseInitProperties(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			var initProperties = new List<ITreeNode>();
			var createTreeNodeOnlyOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			while (true)
			{
				var statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createTreeNodeOnlyOptions, e.TokenReader, e.Control);
				if (statement != null)
				{
					if (statement is TreeBuilder tb)
					{
						statement = tb.Root;
					}
					if (statement is OperatorNode op && op.Name == ";")
					{
						statement = op.Left;
					}
					initProperties.Add(statement);
				}
				var nextToken2 = e.TokenReader.Read();
				if (!nextToken2.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression at {e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}, expect '}}'");
				}
				if (nextToken2.Value.IsSymbol(",")) continue;
				if (nextToken2.Value.IsSymbol("}")) break;
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken2.Value.Line},{nextToken2.Value.Column}, expect '}}'");
			}
			return initProperties;
		}
	}
}
