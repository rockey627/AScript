using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Lua
{
	public class LuaSyntaxAnalyzer : DefaultSyntaxAnalyzer
	{
		public static readonly LuaSyntaxAnalyzer Instance = new LuaSyntaxAnalyzer();

		protected override ITreeNode BuildBlock(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
		{
			// 解析表：{ key1=value1, key2=value2 } 或 { value1, value2, value3 }
			bool? isDict = null;
			var initProperties = ignore ? null : new List<ITreeNode>();

			while (true)
			{
				var token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect '}}'");
				}
				if (token.Value.Value == "}")
				{
					break;
				}
				tokenReader.Push(token.Value);
				var keyNode = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
				var nextToken = tokenReader.Read();
				if (!nextToken.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect '}}'");
				}
				if (nextToken.Value.Type == ETokenType.String)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect '}}'");
				}
				if (nextToken.Value.Value == "}")
				{
					tokenReader.Push(nextToken.Value);
				}
				else if (nextToken.Value.Value == "=")
				{
					if (!isDict.HasValue) isDict = true;
					else if (!isDict.Value)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ','");
					}
				}
				else if (nextToken.Value.Value == ",")
				{
					if (!isDict.HasValue) isDict = false;
					else if (isDict.Value)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect '='");
					}
					tokenReader.Push(nextToken.Value);
				}
				else
				{
					if (!isDict.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect '}}'");
					}
					if (isDict.Value)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect '='");
					}
					throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ','");
				}

				if (!isDict.Value)
				{
					if (!ignore) initProperties.Add(keyNode);
				}
				else
				{
					var valueNode = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
					if (!ignore)
					{
						var indexAssign = PoolManage.CreateOperatorNode("[]", 2, OperatorPriorities["["]);
						indexAssign.Left = keyNode;
						indexAssign.Right = valueNode;
						initProperties.Add(indexAssign);
					}
				}

				token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid table syntax, expect ',' or '}'");
				}
				if (token.Value.Value == "}")
				{
					break;
				}
				if (token.Value.Value != ",")
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid table syntax at {token.Value.Line},{token.Value.Column}, expect ',' or '}}'");
				}
			}

			if (ignore) return null;
			return (isDict ?? true) ?
				new NewNode { SystemType = typeof(Dictionary<object, object>), InitProperties = initProperties } :
				new NewNode { SystemType = typeof(List<object>), InitProperties = initProperties };
		}

		protected override object EvalNumber(string num)
		{
			// Lua numbers are typically double unless they represent integers
			var n = ScriptUtils.EvalNumber(num, true);
			//if (n is double d)
			//{
			//	// Check if it's an integer
			//	if (d == Math.Floor(d) && d >= long.MinValue && d <= long.MaxValue)
			//	{
			//		return (long)d;
			//	}
			//}
			return n;
		}
	}
}
