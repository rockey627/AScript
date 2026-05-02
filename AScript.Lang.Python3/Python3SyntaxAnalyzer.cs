using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AScript.Lang.Python3
{
	public class Python3SyntaxAnalyzer : DefaultSyntaxAnalyzer
	{
		public static readonly Python3SyntaxAnalyzer Instance = new Python3SyntaxAnalyzer();

		private static readonly HashSet<string> _EndTokens = new HashSet<string> { "\n" };

		public override ITreeNode BuildMultiStatement(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null)
		{
			if (endTokens == null) endTokens = _EndTokens;
			return base.BuildMultiStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens);
		}

		public override ITreeNode BuildOneStatement(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, bool noblock = false, IEnumerable<string> endTokens = null)
		{
			if (endTokens == null) endTokens = _EndTokens;
			return base.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, noblock, endTokens);
		}

		protected override void ParseIdentifierOrOperator(TokenAnalyzingArgs e, IEnumerable<string> endTokens = null)
		{
			// 变量定义：a : int = 10
			if (e.CurrentToken.Type == ETokenType.Word && e.TreeBuilder.Root == null)
			{
				var nextToken = e.TokenReader.Read();
				if (nextToken.HasValue && nextToken.Value.Type != ETokenType.String && nextToken.Value.Value == ":")
				{
					var typeToken = e.TokenReader.Read();
					if (typeToken.HasValue && typeToken.Value.Type == ETokenType.Word)
					{
						string definedTypeName = typeToken.Value.Value;
						var definedType = e.ScriptContext.EvalType(definedTypeName);
						if (definedType == null)
						{
							throw new Exception($"unknown type '{definedTypeName}' at {typeToken.Value.Line},{typeToken.Value.Column}");
						}
						if (!e.Ignore)
						{
							e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateDefineVarNode(e.CurrentToken.Value, definedTypeName, definedType));
						}
						e.End = !nextToken.HasValue || nextToken.Value.Value != "=";
						return;
					}
					if (typeToken.HasValue)
					{
						e.TokenReader.Push(typeToken.Value);
					}
				}
				if (nextToken.HasValue)
				{
					e.TokenReader.Push(nextToken.Value);
				}
			}
			base.ParseIdentifierOrOperator(e, endTokens);
		}

		protected override ITreeNode BuildBlock(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
		{
			// 解析字典：{ key1:value1, key2:value2 }
			// 解析集合：{ value1, value2 }
			bool? isDict = null;
			var initProperties = ignore ? null : new List<ITreeNode>();

			// 循环解析 key:value 对
			while (true)
			{
				var token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exception($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect '}}'");
				}
				if (token.Value.Value == "}")
				{
					break;
				}
				tokenReader.Push(token.Value);
				var keyNode = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
				//this.ValidateNextToken(tokenReader, ":");
				var nextToken = tokenReader.Read();
				if (!nextToken.HasValue)
				{
					throw new Exception($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect '}}'");
				}
				if (nextToken.Value.Type == ETokenType.String)
				{
					throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect '}}'");
				}
				if (nextToken.Value.Value == "}")
				{
					tokenReader.Push(nextToken.Value);
				}
				else if (nextToken.Value.Value == ":")
				{
					if (!isDict.HasValue) isDict = true;
					else if (!isDict.Value)
					{
						throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ','");
					}
				}
				else if (nextToken.Value.Value == ",")
				{
					if (!isDict.HasValue) isDict = false;
					else if (isDict.Value)
					{
						throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ':'");
					}
					tokenReader.Push(nextToken.Value);
				}
				else
				{
					if (!isDict.HasValue)
					{
						throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect '}}'");
					}
					if (isDict.Value)
					{
						throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ':'");
					}
					throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect ','");
				}
				// 
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

				// 读取 , 或 }
				token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exception("invalid dictionary syntax, expect ',' or '}'");
				}
				if (token.Value.Value == "}")
				{
					break;
				}
				if (token.Value.Value != ",")
				{
					throw new Exception($"invalid dictionary syntax at {token.Value.Line},{token.Value.Column}, expect ',' or '}}'");
				}
			}

			if (ignore) return null;
			return (isDict ?? true) ?
				new NewNode { SystemType = typeof(Dictionary<object, object>), InitProperties = initProperties } :
				new NewNode { SystemType = typeof(HashSet<object>), InitProperties = initProperties };
		}

		protected override object EvalNumber(string num)
		{
			var n = base.EvalNumber(num);
			if (ScriptUtils.IsIntegerType(n.GetType()))
			{
				return Convert.ToInt64(n);
			}
			return Convert.ToDouble(n);
		}
	}
}
