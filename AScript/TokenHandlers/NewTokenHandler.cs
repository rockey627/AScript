using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	public class NewTokenHandler : ITokenHandler
	{
		public static readonly NewTokenHandler Instance = new NewTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			var funcNameToken = e.TokenReader.Read();
			if (!funcNameToken.HasValue)
			{
				throw new Exception($"invalid expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			if (funcNameToken.Value.Type != ETokenType.Word)
			{
				throw new Exception($"invalid expression '{funcNameToken.Value.Value}' at ({funcNameToken.Value.Line},{funcNameToken.Value.Column})");
			}
			//
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exception($"invalid expression near '{funcNameToken.Value.Value}' at ({funcNameToken.Value.Line},{funcNameToken.Value.Column})");
			}
			List<string> genericTypes = null;
			if (nextToken.Value.Value == "<")
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
						throw new Exception($"invalid expression near '{funcNameToken.Value.Value}', expect '>'");
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
			if (nextToken.HasValue && nextToken.Value.Value == "[")
			{
				contains = true;
				dimension = 1;
				var lengthStatement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control);
				analyzer.ValidateNextToken(e.TokenReader, "]", nextToken.Value.Line, nextToken.Value.Column);
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
			else if (nextToken != null && nextToken.Value.Value == "(")
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
				if (nextToken.HasValue && nextToken.Value.Value != "{")
				{
					e.TokenReader.Push(nextToken.Value);
					nextToken = null;
				}
			}
			IList<ITreeNode> initProperties = null;
			if (nextToken.HasValue && nextToken.Value.Value == "{")
			{
				contains = true;
				initProperties = new List<ITreeNode>();
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
						throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect '}}'");
					}
					if (nextToken2.Value.Value == ",") continue;
					if (nextToken2.Value.Value == "}") break;
					throw new Exception($"invalid expression at {nextToken2.Value.Line},{nextToken2.Value.Column}, expect '}}'");
				}
			}

			if (!contains)
			{
				throw new Exception($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}

			e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, new NewNode { Name = funcNameToken.Value.Value, GenericTypes = genericTypes, Args = args, ArrayDimension = dimension, InitProperties = initProperties });
			e.IsHandled = true;
		}
	}
}
