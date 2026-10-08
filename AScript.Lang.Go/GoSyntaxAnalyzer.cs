using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System.Collections.Generic;

namespace AScript.Lang.Go
{
	public class GoSyntaxAnalyzer : DefaultSyntaxAnalyzer
	{
		public static readonly GoSyntaxAnalyzer Instance = new GoSyntaxAnalyzer();

		public override ITreeNode BuildMultiStatement(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null)
		{
			var treeBuilder = ignore ? null : PoolManage.CreateTreeBuilder();
			while (true)
			{
				treeBuilder?.TryEvalRoot(buildContext, scriptContext, options, control);
				// 
				var statementTreeBuilder = ignore ? null : new TreeBuilder();
				if (TryBuildComma(buildContext, scriptContext, options, tokenReader, control, statementTreeBuilder))
				{
					options.CreateFullStatement = null;
					if (treeBuilder != null)
					{
						var statement = statementTreeBuilder.EvalRoot(buildContext, scriptContext, options, control);
						treeBuilder.Add(buildContext, scriptContext, options, control, statement);
					}
				}
				else
				{
					var statement = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens: endTokens, statementTreeBuilder);
					options.CreateFullStatement = null;
					if (treeBuilder != null && statement != null)
					{
						treeBuilder.Add(buildContext, scriptContext, options, control, statement);
					}
				}
				if (control != null && (control.Break || control.Terminal || control.Continue)) break;
				var nextToken = tokenReader.Read();
				if (!nextToken.HasValue) break;
				if (nextToken.Value.Type == ETokenType.String)
				{
					tokenReader.Push(nextToken.Value);
					continue;
				}
				if (nextToken.Value.Value == ";" || nextToken.Value.Value == ",") continue;
				if (nextToken.Value.Value == ":" && !scriptContext.GetOperatorPriority(":").HasValue && !scriptContext.IsKeywords(":"))
				{
					continue;
				}
				tokenReader.Push(nextToken.Value);
				if (nextToken.Value.Value == "}" || nextToken.Value.Value == ")" || nextToken.Value.Value == "]") break;
				if (ScriptUtils.Contains(endTokens, nextToken.Value.Value)) break;
			}
			return treeBuilder;
		}

		/// <summary>
		/// a, arr[1], map['hello'] = 1, 2, 3
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="tokenReader"></param>
		/// <param name="control"></param>
		/// <param name="treeBuilder"></param>
		/// <returns></returns>
		private bool TryBuildComma(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder)
		{
			bool ignore = treeBuilder == null;
			var list = new List<ITreeNode>();

			while (true)
			{
				var wordToken = tokenReader.Read();
				if (!wordToken.HasValue)
				{
					if (list.Count > 0)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn})");
					}
					return false;
				}
				if (wordToken.Value.Type != ETokenType.Word)
				{
					if (list.Count > 0)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '{wordToken.Value.Value}' at ({wordToken.Value.Line},{wordToken.Value.Column})");
					}
					tokenReader.Push(wordToken.Value);
					return false;
				}

				if (scriptContext.IsKeywords(wordToken.Value.Value))
				{
					tokenReader.Push(wordToken.Value);
					return false;
				}

				var nextToken = tokenReader.Read();
				if (!nextToken.HasValue)
				{
					if (list.Count == 0)
					{
						tokenReader.Push(wordToken.Value);
						return false;
					}
					treeBuilder?.AddData(buildContext, scriptContext, options, control, new TupleNode { Items = list });
					return true;
				}

				ITreeNode node = new VariableNode(wordToken.Value.Value);
				if (nextToken.Value.IsSymbol("["))
				{
					while (true)
					{
						// 解析 arr[1] 格式
						var indexNode = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
						ValidateNextToken(tokenReader, "]");
						node = new OperatorNode("[]", 0, 2)
						{
							Left = node,
							Right = indexNode
						};
						nextToken = tokenReader.Read();
						if (!nextToken.HasValue) break;
						if (nextToken.Value.IsSymbol("[")) continue;
						break;
					}
				}

				if (!nextToken.HasValue)
				{
					if (list.Count == 0)
					{
						treeBuilder?.AddData(buildContext, scriptContext, options, control, node);
						return false;
					}
					treeBuilder?.AddData(buildContext, scriptContext, options, control, new TupleNode { Items = list });
					return true;
				}

				if (nextToken.Value.IsSymbol(","))
				{
					list.Add(node);
					continue;
				}

				if (nextToken.Value.IsSymbol("=") || nextToken.Value.IsSymbol(":="))
				{
					string assignOpName = nextToken.Value.Value;
					// 生成TupleNode
					ITreeNode tupleNode;
					if (list.Count == 0) tupleNode = node;
					else
					{
						list.Add(node);
						tupleNode = new TupleNode { Items = list };
					}

					// 解析=号后面的多语句，按逗号分隔
					var valueList = ignore ? null : new List<ITreeNode>();
					var fullOptions = new BuildOptions(options) { CreateFullStatement = true };
					while (true)
					{
						var stmt = BuildOneStatement(buildContext, scriptContext, fullOptions, tokenReader, control, ignore);
						valueList?.Add(stmt);
						nextToken = tokenReader.Read();
						if (!nextToken.HasValue) break;
						if (nextToken.Value.IsSymbol(",")) continue;
						tokenReader.Push(nextToken.Value);
						break;
					}

					// 生成=号OperatorNode
					if (treeBuilder != null)
					{
						if (tupleNode is TupleNode tn && tn.Items.Count == 2
							&& valueList.Count == 1 && valueList[0] is OperatorNode op && op.Name == "[]")
						{
							// v, ok = map[key]
							var assign = PoolManage.CreateOperatorNode("=", 2, 0);
							assign.Left = tn;
							assign.Right = new CallFuncNode { Name = "__GetMapValue__", Args = new[] { op.Left, op.Right } };
							treeBuilder.AddData(buildContext, scriptContext, options, control, assign);
						}
						else
						{
							var assignOp = PoolManage.CreateOperatorNode(assignOpName, 2, ASSIGN);
							assignOp.Left = tupleNode;
							assignOp.Right = valueList.Count == 1 ? valueList[0] : new TupleNode { Items = valueList };
							treeBuilder.Add(buildContext, scriptContext, options, control, assignOp);
						}
					}
					return true;
				}

				tokenReader.Push(nextToken.Value);

				if (list.Count > 0)
				{
					list.Add(node);
					treeBuilder?.AddData(buildContext, scriptContext, options, control, new TupleNode { Items = list });
					return true;
				}

				if (node is VariableNode)
				{
					tokenReader.Push(wordToken.Value);
					return false;
				}

				treeBuilder?.AddData(buildContext, scriptContext, options, control, node);
				return false;
			}
		}
	}
}
