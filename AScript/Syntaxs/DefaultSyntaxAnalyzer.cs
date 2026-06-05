using AScript.Nodes;
using AScript.Readers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace AScript.Syntaxs
{
	public class DefaultSyntaxAnalyzer : ISyntaxAnalyzer
	{
		public static readonly DefaultSyntaxAnalyzer Instance = new DefaultSyntaxAnalyzer();

		// 赋值操作符优先级固定为50，不要变动
		public const int ASSIGN = 50;

		/// <summary>
		/// 优先级，值越大优先级越高
		/// </summary>
		public static ConcurrentDictionary<string, int> OperatorPriorities { get; set; } = new ConcurrentDictionary<string, int>
		{
			["="] = ASSIGN,
			["+="] = ASSIGN,
			["-="] = ASSIGN,
			["*="] = ASSIGN,
			["**="] = ASSIGN,
			["/="] = ASSIGN,
			["^="] = ASSIGN,
			["&="] = ASSIGN,
			["|="] = ASSIGN,
			["%="] = ASSIGN,
			["?="] = ASSIGN,
			[">>="] = ASSIGN,
			["<<="] = ASSIGN,

			["??"] = 70,

			["?"] = 85,
			//[":"] = 90,

			["|"] = 100,

			["^"] = 110,
			["&"] = 110,

			["||"] = 120,
			["&&"] = 130,

			["=="] = 135,
			["!="] = 135,

			[">"] = 140,
			[">="] = 140,
			["<"] = 140,
			["<="] = 140,

			["<<"] = 150,
			[">>"] = 150,

			["+"] = 160,
			["-"] = 160,
			["~"] = 160,

			["*"] = 170,
			["/"] = 170,
			["%"] = 170,

			["**"] = 175, // 幂运算

			["!"] = 190,

			["++"] = 195,
			["--"] = 195,

			["."] = 200,
			["?."] = 200,
			// 索引器
			["["] = 210
		};

		public event EventHandler<TokenAnalyzingArgs> TokenAnalyzing;

		public virtual ITreeNode Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader)
		{
			return BuildMultiStatement(buildContext, scriptContext, options, tokenReader, new EvalControl());
		}

		public virtual Task<ITreeNode> BuildAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, CancellationToken cancellationToken = default)
		{
			return BuildMultiStatementAsync(buildContext, scriptContext, options, tokenReader, new EvalControl(), cancellationToken: cancellationToken);
		}

		public virtual ITreeNode BuildMultiStatement(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null)
		{
			var treeBuilder = ignore ? null : PoolManage.CreateTreeBuilder();
			while (true)
			{
				if (treeBuilder != null)
				{
					treeBuilder.TryEvalRoot(buildContext, scriptContext, options, control);
				}
				var statement = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens: endTokens);
				if (treeBuilder != null && statement != null)
				{
					treeBuilder.Add(buildContext, scriptContext, options, control, statement);
				}
				// 判断是否结束当前循环
				if (control != null && (control.Break || control.Terminal || control.Continue)) break;
				var nextToken = tokenReader.Read();
				if (!nextToken.HasValue) break;
				if (nextToken.Value.Value == ";" || nextToken.Value.Value == "," || nextToken.Value.Value == ":") continue;
				tokenReader.Push(nextToken.Value);
				if (nextToken.Value.Value == "}" || nextToken.Value.Value == ")" || nextToken.Value.Value == "]") break;
				if (ScriptUtils.Contains(endTokens, nextToken.Value.Value)) break;
			}
			//if (treeBuilder != null)
			//{
			//	treeBuilder.TryEvalRoot(buildContext, scriptContext, options, control);
			//}
			return treeBuilder;
		}

		public virtual async Task<ITreeNode> BuildMultiStatementAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null, CancellationToken cancellationToken = default)
		{
			var treeBuilder = ignore ? null : PoolManage.CreateTreeBuilder();
			while (true)
			{
				if (treeBuilder != null)
				{
					await treeBuilder.TryEvalRootAsync(buildContext, scriptContext, options, control, cancellationToken).ConfigureAwait(false);
				}
				var statement = await BuildOneStatementAsync(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens: endTokens, cancellationToken).ConfigureAwait(false);
				if (treeBuilder != null && statement != null)
				{
					await treeBuilder.AddAsync(buildContext, scriptContext, options, control, statement, cancellationToken).ConfigureAwait(false);
				}
				// 判断是否结束当前循环
				if (control != null && (control.Break || control.Terminal || control.Continue)) break;
				var nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				if (!nextToken.HasValue) break;
				if (nextToken.Value.Value == ";" || nextToken.Value.Value == "," || nextToken.Value.Value == ":") continue;
				tokenReader.Push(nextToken.Value);
				if (nextToken.Value.Value == "}" || nextToken.Value.Value == ")" || nextToken.Value.Value == "]") break;
				if (ScriptUtils.Contains(endTokens, nextToken.Value.Value)) break;
			}
			//if (treeBuilder != null)
			//{
			//	treeBuilder.TryEvalRoot(buildContext, scriptContext, options, control);
			//}
			return treeBuilder;
		}

		public virtual ITreeNode BuildOneStatement(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null)
		{
			var t = tokenReader.Read();
			TreeBuilder treeBuilder = null;
			while (t.HasValue)
			{
				if (t.Value.Type == ETokenType.Number)
				{
					if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
					else if (treeBuilder.IsFullStatement())
					{
						tokenReader.Push(t.Value);
						break;
					}
					treeBuilder.AddData(buildContext, scriptContext, options, control, EvalNumber(t.Value.Value), null);
				}
				else if (t.Value.Type == ETokenType.String)
				{
					if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
					else if (treeBuilder.IsFullStatement())
					{
						tokenReader.Push(t.Value);
						break;
					}
					treeBuilder.AddData(buildContext, scriptContext, options, control, t.Value.Value, typeof(string));
				}
				else if (t.Value.Value == ")" || t.Value.Value == "]" || t.Value.Value == "}" || t.Value.Value == "," || t.Value.Value == ";" || t.Value.Value == ":")
				{
					tokenReader.Push(t.Value);
					break;
				}
				else if (ScriptUtils.Contains(endTokens, t.Value.Value))
				{
					tokenReader.Push(t.Value);
					break;
				}
				else if (t.Value.Value == "{")
				{
					if (treeBuilder != null && treeBuilder.Current != null && treeBuilder.Current is CallFuncNode funcHead
						&& (funcHead.Args == null || funcHead.Args.All(a => a is DefineVarNode)))
					{
						tokenReader.Push(t.Value);
						ParseFuncDefine(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore);
						break;
					}
					if (treeBuilder != null && treeBuilder.Current != null &&
						(!(treeBuilder.Current is OperatorNode opNode) || opNode.IsFull()))
					{
						tokenReader.Push(t.Value);
						break;
					}
					var block = BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
					if (treeBuilder == null) treeBuilder = new TreeBuilder();
					treeBuilder.AddData(buildContext, scriptContext, options, control, block);
				}
				else if (t.Value.Value == "(")
				{
					if (treeBuilder != null && treeBuilder.IsFullStatement())
					{
						tokenReader.Push(t.Value);
						break;
					}
					// 判断类型转换语法：(int?)v (string)(x+b) (long)5
					var typeToken = tokenReader.Read();
					if (!typeToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect ')'");
					}
					if (typeToken.Value.Type == ETokenType.Word)
					{
						var nextToken2 = tokenReader.Read();
						if (!nextToken2.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect ')'");
						}
						Token? nextToken3 = null;
						if (nextToken2.Value.IsSymbol("?"))
						{
							nextToken3 = tokenReader.Read();
							if (!nextToken3.HasValue)
							{
								throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect ')'");
							}
							if (nextToken3.Value.IsSymbol(")"))
							{
								var valueToken = tokenReader.Read();
								if (!valueToken.HasValue)
								{
									throw new Exceptions.ScriptAnalyzingException($"invalid expression '?' at ({nextToken2.Value.Line},{nextToken2.Value.Column})");
								}
								else if (valueToken.Value.Type == ETokenType.Operator)
								{
									throw new Exceptions.ScriptAnalyzingException($"invalid expression '?' at ({nextToken2.Value.Line},{nextToken2.Value.Column})");
								}
								else
								{
									var type0 = scriptContext.EvalType(typeToken.Value.Value);
									if (type0 == null)
									{
										throw new Exceptions.ScriptAnalyzingException($"unkown type '{typeToken.Value.Value}'");
									}
									if (!type0.IsValueType)
									{
										throw new Exceptions.ScriptAnalyzingException($"invalid expression '?' at ({nextToken2.Value.Line},{nextToken2.Value.Column})");
									}
									var type = typeof(Nullable<>).MakeGenericType(type0);
									var typeOpNode = PoolManage.CreateOperatorNode(Functions.ConvertFunction.FORCE_NAME, 2, OperatorPriorities["."] - 1);
									typeOpNode.Left = PoolManage.CreateObjectNode(type);
									if (treeBuilder == null) treeBuilder = new TreeBuilder();
									treeBuilder.AddData(buildContext, scriptContext, options, control, typeOpNode);
									t = valueToken;
									continue;
								}
							}
							else
							{
								tokenReader.Push(nextToken3.Value);
								tokenReader.Push(nextToken2.Value);
								tokenReader.Push(typeToken.Value);
							}
						}
						else if (nextToken2.Value.IsSymbol(")"))
						{
							var valueToken = tokenReader.Read();
							if (!valueToken.HasValue)
							{
								tokenReader.Push(nextToken2.Value);
								tokenReader.Push(typeToken.Value);
							}
							else if (valueToken.Value.Type == ETokenType.Operator)
							{
								tokenReader.Push(valueToken.Value);
								tokenReader.Push(nextToken2.Value);
								tokenReader.Push(typeToken.Value);
							}
							else
							{
								var type = scriptContext.EvalType(typeToken.Value.Value);
								if (type == null)
								{
									throw new Exceptions.ScriptAnalyzingException($"unkown type '{typeToken.Value.Value}'");
								}
								var typeOpNode = PoolManage.CreateOperatorNode(Functions.ConvertFunction.FORCE_NAME, 2, OperatorPriorities["."] - 1);
								typeOpNode.Left = PoolManage.CreateObjectNode(type);
								if (treeBuilder == null) treeBuilder = new TreeBuilder();
								treeBuilder.AddData(buildContext, scriptContext, options, control, typeOpNode);
								t = valueToken;
								continue;
							}
						}
						else
						{
							tokenReader.Push(nextToken2.Value);
							tokenReader.Push(typeToken.Value);
						}
					}
					else
					{
						tokenReader.Push(typeToken.Value);
					}
					// 
					var buildOptions = options;
					if (!(buildOptions.CreateFullTreeNode ?? false))
					{
						var token1 = tokenReader.Read();
						Token? token2 = null;
						if (token1.HasValue && token1.Value.Type == ETokenType.Word)
						{
							token2 = tokenReader.Read();
							if (token2.HasValue && (token2.Value.Type == ETokenType.Word || token2.Value.IsSymbol(",")))
							{
								buildOptions = new BuildOptions(options) { CreateFullTreeNode = true };
							}
						}
						if (token2.HasValue) tokenReader.Push(token2.Value);
						if (token1.HasValue) tokenReader.Push(token1.Value);
					}
					var statement0 = BuildOneStatement(buildContext, scriptContext, buildOptions, tokenReader, control, ignore);
					var nextToken = tokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect ')'");
					}
					if (nextToken.Value.Type == ETokenType.String)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column}), expect ')'");
					}
					// 元组解析：括号内有逗号分隔的多个表达式
					if (nextToken.Value.Value == ",")
					{
						var items = ignore ? null : new List<ITreeNode> { statement0 };
						while (true)
						{
							var item = BuildOneStatement(buildContext, scriptContext, buildOptions, tokenReader, control, ignore);
							if (!ignore) items.Add(item);
							var tok = tokenReader.Read();
							if (!tok.HasValue) throw new Exceptions.ScriptAnalyzingException("invalid tuple expression, expect ')'");
							if (tok.Value.Type == ETokenType.String)
							{
								throw new Exceptions.ScriptAnalyzingException($"invalid tuple expression near '{tok.Value.Value}' at ({tok.Value.Line},{tok.Value.Column}), expect ')'");
							}
							if (tok.Value.Value == ")") break;
							if (tok.Value.Value != ",") throw new Exceptions.ScriptAnalyzingException($"invalid tuple expression near '{tok.Value.Value}' at ({tok.Value.Line},{tok.Value.Column}), expect ',' or ')'");
						}
						if (!ignore)
						{
							var tupleNode = new TupleNode { Items = items };
							if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
							treeBuilder.AddData(buildContext, scriptContext, options, control, tupleNode);
						}
					}
					else
					{
						if (nextToken.Value.Value != ")")
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column}), expect ')'");
						}
						if (!ignore)
						{
							if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
							treeBuilder.AddData(buildContext, scriptContext, options, control, statement0);
						}
					}
				}
				else if (t.Value.Value == "=>")
				{
					if (treeBuilder == null || treeBuilder.Current == null)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '=>' at {t.Value.Line},{t.Value.Column}");
					}
					//BuildOptions buildOptions;
					//if (options.CreateFullTreeNode ?? false)
					//{
					//	buildOptions = options;
					//}
					//else
					//{
					//	buildOptions = new BuildOptions(options) { CreateFullTreeNode = true };
					//}
					if (!(options.CreateFullTreeNode ?? false))
					{
						options = new BuildOptions(options) { CreateFullTreeNode = true };
					}
					// a => body 语法：单个变量作为参数
					if (treeBuilder.Current is DefineVarNode defineVarNode)
					{
						var funcHead = new CallFuncNode
						{
							Name = "_",
							Args = new DefineVarNode[] { defineVarNode }
						};
						ParseFuncDefine(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore);
					}
					else if (treeBuilder.Current is VariableNode varNode)
					{
						var funcHead = new CallFuncNode
						{
							Name = "_",
							Args = new DefineVarNode[] { PoolManage.CreateDefineVarNode(varNode.Name, null, typeof(object)) }
						};
						ParseFuncDefine(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore);
					}
					// func(a, b) => body 语法：函数调用作为参数
					else if (treeBuilder.Current is CallFuncNode funcHead
						&& (funcHead.Args == null || funcHead.Args.Length == 0 || funcHead.Args.All(a => a is VariableNode)))
					{
						ParseFuncDefine(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore);
					}
					else if (treeBuilder.Current is TupleNode tupleNode && tupleNode.Items.All(a => a is VariableNode || a is DefineVarNode))
					{
						var funcHead2 = new CallFuncNode
						{
							Name = "_",
							Args = tupleNode.Items.Select(a => a is DefineVarNode defineVar ? defineVar : PoolManage.CreateDefineVarNode(((VariableNode)a).Name, null, typeof(object))).ToArray()
						};
						ParseFuncDefine(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead2, ignore);
					}
					else
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '=>' at {t.Value.Line},{t.Value.Column}");
					}
					break;
				}
				else
				{
					if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
					var e = TokenAnalyzingArgs.Create(buildContext, scriptContext, options, control, treeBuilder, tokenReader, t.Value);
					try
					{
						e.Ignore = ignore;
						OnTokenAnalyzing(e);
						if (!e.IsHandled)
						{
							e.ScriptContext.HandleToken(this, e);
						}
						if (!e.IsHandled)
						{
							ParseIdentifierOrOperator(e, endTokens);
						}
						if (e.End) break;
					}
					finally
					{
						TokenAnalyzingArgs.Return(e);
					}
				}
				t = tokenReader.Read();
			}
			//if (treeBuilder != null)
			//{
			//	treeBuilder.TryEvalRoot(buildContext, scriptContext, options, control);
			//}
			//return treeBuilder;
			if (treeBuilder == null) return null;
			if (treeBuilder.Current is OperatorNode operatorNode && operatorNode.Name != ";" && !operatorNode.IsFull())
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn})");
			}
			var result = treeBuilder.EvalRoot(buildContext, scriptContext, options, control);
			PoolManage.Return(treeBuilder);
			return result;
		}

		public virtual async Task<ITreeNode> BuildOneStatementAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null, CancellationToken cancellationToken = default)
		{
			var t = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			TreeBuilder treeBuilder = null;
			while (t.HasValue)
			{
				if (t.Value.Type == ETokenType.Number)
				{
					if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
					else if (treeBuilder.IsFullStatement())
					{
						tokenReader.Push(t.Value);
						break;
					}
					await treeBuilder.AddDataAsync(buildContext, scriptContext, options, control, EvalNumber(t.Value.Value), null).ConfigureAwait(false);
				}
				else if (t.Value.Type == ETokenType.String)
				{
					if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
					else if (treeBuilder.IsFullStatement())
					{
						tokenReader.Push(t.Value);
						break;
					}
					await treeBuilder.AddDataAsync(buildContext, scriptContext, options, control, t.Value.Value, typeof(string), cancellationToken).ConfigureAwait(false);
				}
				else if (t.Value.Value == ")" || t.Value.Value == "]" || t.Value.Value == "}" || t.Value.Value == "," || t.Value.Value == ";" || t.Value.Value == ":")
				{
					tokenReader.Push(t.Value);
					break;
				}
				else if (ScriptUtils.Contains(endTokens, t.Value.Value))
				{
					tokenReader.Push(t.Value);
					break;
				}
				else if (t.Value.Value == "{")
				{
					if (treeBuilder != null && treeBuilder.Current != null && treeBuilder.Current is CallFuncNode funcHead
						&& (funcHead.Args == null || funcHead.Args.All(a => a is DefineVarNode)))
					{
						tokenReader.Push(t.Value);
						await ParseFuncDefineAsync(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore, cancellationToken).ConfigureAwait(false);
						break;
					}
					if (treeBuilder != null && treeBuilder.Current != null &&
						(!(treeBuilder.Current is OperatorNode opNode) || opNode.IsFull()))
					{
						tokenReader.Push(t.Value);
						break;
					}
					var block = await BuildBlockAsync(buildContext, scriptContext, options, tokenReader, control, ignore, cancellationToken).ConfigureAwait(false);
					if (treeBuilder == null) treeBuilder = new TreeBuilder();
					await treeBuilder.AddDataAsync(buildContext, scriptContext, options, control, block, cancellationToken).ConfigureAwait(false);
				}
				else if (t.Value.Value == "(")
				{
					if (treeBuilder != null && treeBuilder.IsFullStatement())
					{
						tokenReader.Push(t.Value);
						break;
					}
					// 判断类型转换语法：(int?)v (string)(x+b) (long)5
					var typeToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					if (!typeToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect ')'");
					}
					if (typeToken.Value.Type == ETokenType.Word)
					{
						var nextToken2 = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
						if (!nextToken2.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect ')'");
						}
						Token? nextToken3 = null;
						if (nextToken2.Value.IsSymbol("?"))
						{
							nextToken3 = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
							if (!nextToken3.HasValue)
							{
								throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect ')'");
							}
							if (nextToken3.Value.IsSymbol(")"))
							{
								var valueToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
								if (!valueToken.HasValue)
								{
									throw new Exceptions.ScriptAnalyzingException($"invalid expression '?' at ({nextToken2.Value.Line},{nextToken2.Value.Column})");
								}
								else if (valueToken.Value.Type == ETokenType.Operator)
								{
									throw new Exceptions.ScriptAnalyzingException($"invalid expression '?' at ({nextToken2.Value.Line},{nextToken2.Value.Column})");
								}
								else
								{
									var type0 = scriptContext.EvalType(typeToken.Value.Value);
									if (type0 == null)
									{
										throw new Exceptions.ScriptAnalyzingException($"unkown type '{typeToken.Value.Value}'");
									}
									if (!type0.IsValueType)
									{
										throw new Exceptions.ScriptAnalyzingException($"invalid expression '?' at ({nextToken2.Value.Line},{nextToken2.Value.Column})");
									}
									var type = typeof(Nullable<>).MakeGenericType(type0);
									var typeOpNode = PoolManage.CreateOperatorNode(Functions.ConvertFunction.FORCE_NAME, 2, OperatorPriorities["."] - 1);
									typeOpNode.Left = PoolManage.CreateObjectNode(type);
									if (treeBuilder == null) treeBuilder = new TreeBuilder();
									await treeBuilder.AddDataAsync(buildContext, scriptContext, options, control, typeOpNode, cancellationToken).ConfigureAwait(false);
									t = valueToken;
									continue;
								}
							}
							else
							{
								tokenReader.Push(nextToken3.Value);
								tokenReader.Push(nextToken2.Value);
								tokenReader.Push(typeToken.Value);
							}
						}
						else if (nextToken2.Value.IsSymbol(")"))
						{
							var valueToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
							if (!valueToken.HasValue)
							{
								tokenReader.Push(nextToken2.Value);
								tokenReader.Push(typeToken.Value);
							}
							else if (valueToken.Value.Type == ETokenType.Operator)
							{
								tokenReader.Push(valueToken.Value);
								tokenReader.Push(nextToken2.Value);
								tokenReader.Push(typeToken.Value);
							}
							else
							{
								var type = scriptContext.EvalType(typeToken.Value.Value);
								if (type == null)
								{
									throw new Exceptions.ScriptAnalyzingException($"unkown type '{typeToken.Value.Value}'");
								}
								var typeOpNode = PoolManage.CreateOperatorNode(Functions.ConvertFunction.FORCE_NAME, 2, OperatorPriorities["."] - 1);
								typeOpNode.Left = PoolManage.CreateObjectNode(type);
								if (treeBuilder == null) treeBuilder = new TreeBuilder();
								await treeBuilder.AddDataAsync(buildContext, scriptContext, options, control, typeOpNode, cancellationToken).ConfigureAwait(false);
								t = valueToken;
								continue;
							}
						}
						else
						{
							tokenReader.Push(nextToken2.Value);
							tokenReader.Push(typeToken.Value);
						}
					}
					else
					{
						tokenReader.Push(typeToken.Value);
					}
					// 
					var buildOptions = options;
					if (!(buildOptions.CreateFullTreeNode ?? false))
					{
						var token1 = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
						Token? token2 = null;
						if (token1.HasValue && token1.Value.Type == ETokenType.Word)
						{
							token2 = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
							if (token2.HasValue && (token2.Value.Type == ETokenType.Word || token2.Value.IsSymbol(",")))
							{
								buildOptions = new BuildOptions(options) { CreateFullTreeNode = true };
							}
						}
						if (token2.HasValue) tokenReader.Push(token2.Value);
						if (token1.HasValue) tokenReader.Push(token1.Value);
					}
					var statement0 = await BuildOneStatementAsync(buildContext, scriptContext, buildOptions, tokenReader, control, ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
					var nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}), expect ')'");
					}
					if (nextToken.Value.Type == ETokenType.String)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column}), expect ')'");
					}
					// 元组解析：括号内有逗号分隔的多个表达式
					if (nextToken.Value.Value == ",")
					{
						var items = ignore ? null : new List<ITreeNode> { statement0 };
						while (true)
						{
							var item = await BuildOneStatementAsync(buildContext, scriptContext, buildOptions, tokenReader, control, ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
							if (!ignore) items.Add(item);
							var tok = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
							if (!tok.HasValue) throw new Exceptions.ScriptAnalyzingException("invalid tuple expression, expect ')'");
							if (tok.Value.Type == ETokenType.String)
							{
								throw new Exceptions.ScriptAnalyzingException($"invalid tuple expression near '{tok.Value.Value}' at ({tok.Value.Line},{tok.Value.Column}), expect ')'");
							}
							if (tok.Value.Value == ")") break;
							if (tok.Value.Value != ",") throw new Exceptions.ScriptAnalyzingException($"invalid tuple expression near '{tok.Value.Value}' at ({tok.Value.Line},{tok.Value.Column}), expect ',' or ')'");
						}
						if (!ignore)
						{
							var tupleNode = new TupleNode { Items = items };
							if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
							await treeBuilder.AddDataAsync(buildContext, scriptContext, options, control, tupleNode, cancellationToken).ConfigureAwait(false);
						}
					}
					else
					{
						if (nextToken.Value.Value != ")")
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column}), expect ')'");
						}
						if (!ignore)
						{
							if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
							await treeBuilder.AddDataAsync(buildContext, scriptContext, options, control, statement0, cancellationToken).ConfigureAwait(false);
						}
					}
				}
				else if (t.Value.Value == "=>")
				{
					if (treeBuilder == null || treeBuilder.Current == null)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '=>' at {t.Value.Line},{t.Value.Column}");
					}
					//BuildOptions buildOptions;
					//if (options.CreateFullTreeNode ?? false)
					//{
					//	buildOptions = options;
					//}
					//else
					//{
					//	buildOptions = new BuildOptions(options) { CreateFullTreeNode = true };
					//}
					if (!(options.CreateFullTreeNode ?? false))
					{
						options = new BuildOptions(options) { CreateFullTreeNode = true };
					}
					// a => body 语法：单个变量作为参数
					if (treeBuilder.Current is DefineVarNode defineVarNode)
					{
						var funcHead = new CallFuncNode
						{
							Name = "_",
							Args = new DefineVarNode[] { defineVarNode }
						};
						await ParseFuncDefineAsync(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore, cancellationToken).ConfigureAwait(false);
					}
					else if (treeBuilder.Current is VariableNode varNode)
					{
						var funcHead = new CallFuncNode
						{
							Name = "_",
							Args = new DefineVarNode[] { PoolManage.CreateDefineVarNode(varNode.Name, null, typeof(object)) }
						};
						await ParseFuncDefineAsync(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore, cancellationToken).ConfigureAwait(false);
					}
					// func(a, b) => body 语法：函数调用作为参数
					else if (treeBuilder.Current is CallFuncNode funcHead
						&& (funcHead.Args == null || funcHead.Args.Length == 0 || funcHead.Args.All(a => a is VariableNode)))
					{
						await ParseFuncDefineAsync(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore, cancellationToken).ConfigureAwait(false);
					}
					else if (treeBuilder.Current is TupleNode tupleNode && tupleNode.Items.All(a => a is VariableNode || a is DefineVarNode))
					{
						var funcHead2 = new CallFuncNode
						{
							Name = "_",
							Args = tupleNode.Items.Select(a => a is DefineVarNode defineVar ? defineVar : PoolManage.CreateDefineVarNode(((VariableNode)a).Name, null, typeof(object))).ToArray()
						};
						await ParseFuncDefineAsync(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead2, ignore, cancellationToken).ConfigureAwait(false);
					}
					else
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '=>' at {t.Value.Line},{t.Value.Column}");
					}
					break;
				}
				else
				{
					if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
					var e = TokenAnalyzingArgs.Create(buildContext, scriptContext, options, control, treeBuilder, tokenReader, t.Value);
					try
					{
						e.Ignore = ignore;
						OnTokenAnalyzing(e);
						if (!e.IsHandled)
						{
							await e.ScriptContext.HandleTokenAsync(this, e, cancellationToken).ConfigureAwait(false);
						}
						if (!e.IsHandled)
						{
							await ParseIdentifierOrOperatorAsync(e, endTokens, cancellationToken).ConfigureAwait(false);
						}
						if (e.End) break;
					}
					finally
					{
						TokenAnalyzingArgs.Return(e);
					}
				}
				t = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			}
			//if (treeBuilder != null)
			//{
			//	treeBuilder.TryEvalRoot(buildContext, scriptContext, options, control);
			//}
			//return treeBuilder;
			if (treeBuilder == null) return null;
			if (treeBuilder.Current is OperatorNode operatorNode && operatorNode.Name != ";" && !operatorNode.IsFull())
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn})");
			}
			var result = await treeBuilder.EvalRootAsync(buildContext, scriptContext, options, control, cancellationToken).ConfigureAwait(false);
			PoolManage.Return(treeBuilder);
			return result;
		}

		public ITreeNode BuildOneStatement2(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null, bool noblock = false)
		{
			var token = tokenReader.Read();
			if (!token.HasValue) return null;
			if (token.Value.Type != ETokenType.String && token.Value.Value == "{")
			{
				if (noblock)
				{
					var node = BuildMultiStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens);
					ValidateNextToken(tokenReader, "}");
					return node;
				}
				return BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
			}
			tokenReader.Push(token.Value);
			return BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens);
		}

		public async Task<ITreeNode> BuildOneStatement2Async(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null, bool noblock = false, CancellationToken cancellationToken = default)
		{
			var token = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (!token.HasValue) return null;
			if (token.Value.Type != ETokenType.String && token.Value.Value == "{")
			{
				if (noblock)
				{
					var node = await BuildMultiStatementAsync(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens, cancellationToken).ConfigureAwait(false);
					await ValidateNextTokenAsync(tokenReader, "}", cancellationToken).ConfigureAwait(false);
					return node;
				}
				return BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
			}
			tokenReader.Push(token.Value);
			return BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens);
		}

		public virtual Token? ValidateNextToken(TokenReader tokenReader, string nextTokenForValid)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect {nextTokenForValid}");
			}
			if (nextToken.Value.Type == ETokenType.String || nextToken.Value.Value != nextTokenForValid)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at {nextToken.Value.Line},{nextToken.Value.Column}, expect {nextTokenForValid}");
			}
			return nextToken;
		}

		public virtual Token? ValidateNextToken(TokenReader tokenReader, string nextTokenForValid, StringComparison comparisonType)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect {nextTokenForValid}");
			}
			if (nextToken.Value.Type == ETokenType.String || !nextTokenForValid.Equals(nextToken.Value.Value, comparisonType))
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at {nextToken.Value.Line},{nextToken.Value.Column}, expect {nextTokenForValid}");
			}
			return nextToken;
		}

		public virtual async Task<Token?> ValidateNextTokenAsync(TokenReader tokenReader, string nextTokenForValid, CancellationToken cancellationToken = default)
		{
			var nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect {nextTokenForValid}");
			}
			if (nextToken.Value.Type == ETokenType.String || nextToken.Value.Value != nextTokenForValid)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at {nextToken.Value.Line},{nextToken.Value.Column}, expect {nextTokenForValid}");
			}
			return nextToken;
		}

		public virtual async Task<Token?> ValidateNextTokenAsync(TokenReader tokenReader, string nextTokenForValid, StringComparison comparisonType, CancellationToken cancellationToken = default)
		{
			var nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect {nextTokenForValid}");
			}
			if (nextToken.Value.Type == ETokenType.String || !nextTokenForValid.Equals(nextToken.Value.Value, comparisonType))
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at {nextToken.Value.Line},{nextToken.Value.Column}, expect {nextTokenForValid}");
			}
			return nextToken;
		}

		public virtual Token? ValidateNextToken(TokenReader tokenReader, ETokenType nextTokenTypeForValid)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect {nextTokenTypeForValid.ToString()}");
			}
			if (nextToken.Value.Type != nextTokenTypeForValid)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect {nextTokenTypeForValid.ToString()}");
			}
			return nextToken;
		}

		public virtual async Task<Token?> ValidateNextTokenAsync(TokenReader tokenReader, ETokenType nextTokenTypeForValid, CancellationToken cancellationToken = default)
		{
			var nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn}, expect {nextTokenTypeForValid.ToString()}");
			}
			if (nextToken.Value.Type != nextTokenTypeForValid)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect {nextTokenTypeForValid.ToString()}");
			}
			return nextToken;
		}

		public virtual void TrySkipNextToken(TokenReader tokenReader, string nextTokenForSkip)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue) return;
			if (nextToken.Value.Type != ETokenType.String
				&& nextToken.Value.Value == nextTokenForSkip) return;
			tokenReader.Push(nextToken.Value);
		}

		public virtual async Task TrySkipNextTokenAsync(TokenReader tokenReader, string nextTokenForSkip, CancellationToken cancellationToken = default)
		{
			var nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (!nextToken.HasValue) return;
			if (nextToken.Value.Type != ETokenType.String
				&& nextToken.Value.Value == nextTokenForSkip) return;
			tokenReader.Push(nextToken.Value);
		}

		public virtual void TrySkipNextToken(TokenReader tokenReader, string nextTokenForSkip, StringComparison comparisonType)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue) return;
			if (nextToken.Value.Type != ETokenType.String
				&& nextTokenForSkip.Equals(nextToken.Value.Value, comparisonType)) return;
			tokenReader.Push(nextToken.Value);
		}

		public virtual async Task TrySkipNextTokenAsync(TokenReader tokenReader, string nextTokenForSkip, StringComparison comparisonType, CancellationToken cancellationToken = default)
		{
			var nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (!nextToken.HasValue) return;
			if (nextToken.Value.Type != ETokenType.String
				&& nextTokenForSkip.Equals(nextToken.Value.Value, comparisonType)) return;
			tokenReader.Push(nextToken.Value);
		}

		/// <summary>
		/// 构建函数参数列表
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="tokenReader"></param>
		/// <param name="control"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public virtual IList<ITreeNode> BuildFuncParams(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression, expect ')'");
			}
			if (nextToken.Value.IsSymbol(")")) return null;
			tokenReader.Push(nextToken.Value);
			var list = ignore ? null : new List<ITreeNode>();
			while (true)
			{
				var s = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
				if (!ignore)
				{
					list.Add(s);
				}
				nextToken = tokenReader.Read();
				if (!nextToken.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression, expect ')' at ({tokenReader.CharReader.CurrentLine},{tokenReader.CharReader.CurrentColumn})");
				}
				if (nextToken.Value.IsSymbol(")")) break;
				if (nextToken.Value.IsSymbol(",")) continue;
				throw new Exceptions.ScriptAnalyzingException($"invalid expression {nextToken.Value.Value} at {nextToken.Value.Line},{nextToken.Value.Column} expect ')'");
			}
			return list;
		}

		/// <summary>
		/// 构建函数参数列表
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="tokenReader"></param>
		/// <param name="control"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public virtual async Task<IList<ITreeNode>> BuildFuncParamsAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, CancellationToken cancellationToken = default)
		{
			var nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression, expect ')'");
			}
			if (nextToken.Value.IsSymbol(")")) return null;
			tokenReader.Push(nextToken.Value);
			var list = ignore ? null : new List<ITreeNode>();
			while (true)
			{
				var s = await BuildOneStatementAsync(buildContext, scriptContext, options, tokenReader, control, ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
				if (!ignore)
				{
					list.Add(s);
				}
				nextToken = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				if (!nextToken.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid expression, expect ')'");
				}
				if (nextToken.Value.IsSymbol(")")) break;
				if (nextToken.Value.IsSymbol(",")) continue;
				throw new Exceptions.ScriptAnalyzingException($"invalid expression {nextToken.Value.Value} at {nextToken.Value.Line},{nextToken.Value.Column} expect ')'");
			}
			return list;
		}

		/// <summary>
		/// 构建函数参数列表，无括号
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="tokenReader"></param>
		/// <param name="control"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public virtual IList<ITreeNode> BuildFuncParams2(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue)
			{
				return null;
			}
			if (nextToken.Value.Type == ETokenType.None) return null;
			tokenReader.Push(nextToken.Value);
			var list = ignore ? null : new List<ITreeNode>();
			while (true)
			{
				var s = BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
				if (!ignore)
				{
					list.Add(s);
				}
				nextToken = tokenReader.Read();
				if (!nextToken.HasValue)
				{
					break;
				}
				if (nextToken.Value.IsSymbol(",")) continue;
				break;
			}
			return list;
		}

		protected virtual ITreeNode BuildBlock(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
		{
			if (!(options.CreateFullTreeNode ?? false) && (options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var tempBuildContext = new BuildContext(buildContext);
				var blockBuilder = BuildMultiStatement(tempBuildContext, scriptContext, options, tokenReader, control, ignore);
				ValidateNextToken(tokenReader, "}");
				var blockBody = blockBuilder.Build(tempBuildContext, scriptContext, options);
				var blockExpr = tempBuildContext.BuildBlock(scriptContext, options, blockBody);
				return PoolManage.CreateExpressionNode(blockExpr);
			}
			else
			{
				var tmpScriptContext = ScriptContext.Create(scriptContext);
				var multiStatement = BuildMultiStatement(buildContext, tmpScriptContext, options, tokenReader, control, ignore);
				ValidateNextToken(tokenReader, "}");
				if (!(options.CreateFullTreeNode ?? false))
				{
					return multiStatement;
				}
				if (multiStatement == null) return null;
				if (multiStatement is TreeBuilder treeBuilder)
				{
					multiStatement = treeBuilder.EvalRoot(buildContext, scriptContext, options, control);
					PoolManage.Return(treeBuilder);
				}
				return PoolManage.CreateBlockNode(multiStatement);
			}
		}

		protected virtual async Task<ITreeNode> BuildBlockAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, CancellationToken cancellationToken = default)
		{
			if (!(options.CreateFullTreeNode ?? false) && (options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var tempBuildContext = new BuildContext(buildContext);
				var blockBuilder = await BuildMultiStatementAsync(tempBuildContext, scriptContext, options, tokenReader, control, ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
				await ValidateNextTokenAsync(tokenReader, "}", cancellationToken).ConfigureAwait(false);
				var blockBody = blockBuilder.Build(tempBuildContext, scriptContext, options);
				var blockExpr = tempBuildContext.BuildBlock(scriptContext, options, blockBody);
				return PoolManage.CreateExpressionNode(blockExpr);
			}
			else
			{
				var tmpScriptContext = ScriptContext.Create(scriptContext);
				var multiStatement = await BuildMultiStatementAsync(buildContext, tmpScriptContext, options, tokenReader, control, ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
				await ValidateNextTokenAsync(tokenReader, "}", cancellationToken).ConfigureAwait(false);
				if (!(options.CreateFullTreeNode ?? false))
				{
					return multiStatement;
				}
				if (multiStatement == null) return null;
				if (multiStatement is TreeBuilder treeBuilder)
				{
					multiStatement = await treeBuilder.EvalRootAsync(buildContext, scriptContext, options, control, cancellationToken).ConfigureAwait(false);
					PoolManage.Return(treeBuilder);
				}
				return PoolManage.CreateBlockNode(multiStatement);
			}
		}

		protected virtual void ParseIdentifierOrOperator(TokenAnalyzingArgs e, IEnumerable<string> endTokens = null)
		{
			if (e.IsHandled) return;

			e.IsHandled = true;

			// 检查是否是操作符
			if (e.CurrentToken.Type == ETokenType.Operator)// || OperatorPriorities.TryGetValue(e.CurrentToken.Value, out _))
			{
				ParseOperator(e, endTokens: endTokens);
				return;
			}

			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				e.End = true;
				return;
			}

			// 标识符处理：变量、函数调用、类型定义
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue && nextToken.Value.Value == "(" && !ScriptUtils.Contains(endTokens, nextToken.Value.Value))
			{
				// 函数调用
				ParseFuncCall(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.TreeBuilder, e.CurrentToken.Value, e.Ignore);
				//nextToken = tokenReader.Read();
				nextToken = null;
			}
			else if (!(e.TreeBuilder.Current is OperatorNode opNode && opNode.Name == ".")
				&& nextToken.HasValue && nextToken.Value.Type == ETokenType.Word
				&& !(ScriptUtils.Contains(endTokens, nextToken.Value.Value) || ScriptUtils.Contains(endTokens, "\n") && nextToken.Value.Line > e.CurrentToken.Line)
				&& !e.ScriptContext.IsKeywords(nextToken.Value.Value))
			{
				// 类型定义 (int x 或 int Add(...))
				var currentToken = e.CurrentToken;
				string definedTypeName = currentToken.Value;
				var definedType = e.ScriptContext.EvalType(definedTypeName);
				if (definedType == null)
				{
					//throw new Exceptions.ScriptAnalyzingException($"unknown type '{definedTypeName}' at {currentToken.Line},{currentToken.Column}");
					// 变量引用
					if (!e.Ignore)
					{
						e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateVariableNode(e.CurrentToken.Value));
					}
				}
				else
				{
					currentToken = nextToken.Value;
					nextToken = e.TokenReader.Read();

					if (nextToken.HasValue && nextToken.Value.Value == "(")
					{
						// 函数定义
						ParseFuncDefine(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.TreeBuilder, currentToken.Value, definedTypeName, definedType, e.Ignore);
						nextToken = e.TokenReader.Read();
						e.End = true;
					}
					else
					{
						// 变量定义
						if (!e.Ignore)
						{
							e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateDefineVarNode(currentToken.Value, definedTypeName, definedType));
						}
						e.End = !nextToken.HasValue || nextToken.Value.Value != "=";
					}
				}
			}
			else
			{
				// 变量引用
				if (!e.Ignore)
				{
					e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateVariableNode(e.CurrentToken.Value));
				}
			}

			if (nextToken.HasValue)
			{
				e.TokenReader.Push(nextToken.Value);
			}
		}

		protected virtual async Task ParseIdentifierOrOperatorAsync(TokenAnalyzingArgs e, IEnumerable<string> endTokens = null, CancellationToken cancellationToken = default)
		{
			if (e.IsHandled) return;

			e.IsHandled = true;

			// 检查是否是操作符
			if (e.CurrentToken.Type == ETokenType.Operator)// || OperatorPriorities.TryGetValue(e.CurrentToken.Value, out _))
			{
				await ParseOperatorAsync(e, endTokens: endTokens, cancellationToken).ConfigureAwait(false);
				return;
			}

			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				e.End = true;
				return;
			}

			// 标识符处理：变量、函数调用、类型定义
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue && nextToken.Value.Value == "(")
			{
				// 函数调用
				await ParseFuncCallAsync(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.TreeBuilder, e.CurrentToken.Value, e.Ignore, cancellationToken).ConfigureAwait(false);
				//nextToken = tokenReader.Read();
				nextToken = null;
			}
			else if (!(e.TreeBuilder.Current is OperatorNode opNode && opNode.Name == ".")
				&& nextToken.HasValue && nextToken.Value.Type == ETokenType.Word
				&& !(ScriptUtils.Contains(endTokens, nextToken.Value.Value) || ScriptUtils.Contains(endTokens, "\n") && nextToken.Value.Line > e.CurrentToken.Line)
				&& !e.ScriptContext.IsKeywords(nextToken.Value.Value))
			{
				// 类型定义 (int x 或 int Add(...))
				var currentToken = e.CurrentToken;
				string definedTypeName = currentToken.Value;
				var definedType = e.ScriptContext.EvalType(definedTypeName);
				if (definedType == null)
				{
					throw new Exceptions.ScriptAnalyzingException($"unknown type '{definedTypeName}' at {currentToken.Line},{currentToken.Column}");
				}
				currentToken = nextToken.Value;
				nextToken = await e.TokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);

				if (nextToken.HasValue && nextToken.Value.Value == "(")
				{
					// 函数定义
					await ParseFuncDefineAsync(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.TreeBuilder, currentToken.Value, definedTypeName, definedType, e.Ignore, cancellationToken).ConfigureAwait(false);
					nextToken = await e.TokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					e.End = true;
				}
				else
				{
					// 变量定义
					if (!e.Ignore)
					{
						await e.TreeBuilder.AddAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateDefineVarNode(currentToken.Value, definedTypeName, definedType), cancellationToken).ConfigureAwait(false);
					}
					e.End = !nextToken.HasValue || nextToken.Value.Value != "=";
				}
			}
			else
			{
				// 变量引用
				if (!e.Ignore)
				{
					await e.TreeBuilder.AddAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateVariableNode(e.CurrentToken.Value), cancellationToken).ConfigureAwait(false);
				}
			}

			if (nextToken.HasValue)
			{
				e.TokenReader.Push(nextToken.Value);
			}
		}

		protected void ParseOperator(TokenAnalyzingArgs e, IEnumerable<string> endTokens = null)
		{
			var currentPriority = e.ScriptContext.GetOperatorPriority(e.CurrentToken.Value);
			if (currentPriority.HasValue)
			{
				if (!e.Ignore)
				{
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, e.CurrentToken.Value, GetDataCount(e.CurrentToken.Value), currentPriority.Value);
				}
				return;
			}
			if (e.CurrentToken.Value.Length == 1)
			{
				throw new Exceptions.ScriptAnalyzingException($"unknown operator '{e.CurrentToken.Value}'");
			}
			// 拆分运算符
			string s0 = e.CurrentToken.Value;
			int cc = s0.Length - 1;
			while (s0.Length > 0)
			{
				string s1 = cc == s0.Length ? s0 : s0.Substring(0, cc);
				var s1Priority = e.ScriptContext.GetOperatorPriority(s1);
				if (s1Priority.HasValue)
				{
					//treeBuilder.AddOperator(buildContext, scriptContext, options, control, s1, GetDataCount(s1), s1Priority);
					s0 = s0.Substring(cc);
					//cc = s0.Length;
					e.TokenReader.Push(new Token(s0, ETokenType.Operator, e.CurrentToken.Line, e.CurrentToken.Column + s1.Length));
					e.TokenReader.Push(new Token(s1, ETokenType.Operator, e.CurrentToken.Line, e.CurrentToken.Column));
					break;
				}
				if (ScriptUtils.Contains(endTokens, s1))
				{
					e.End = true;
					s0 = s0.Substring(cc);
					e.TokenReader.Push(new Token(s0, ETokenType.Operator, e.CurrentToken.Line, e.CurrentToken.Column + s1.Length));
					e.TokenReader.Push(new Token(s1, ETokenType.Operator, e.CurrentToken.Line, e.CurrentToken.Column));
					break;
				}
				cc--;
				if (cc == 0)
				{
					throw new Exceptions.ScriptAnalyzingException($"unknown operator '{e.CurrentToken.Value}'");
				}
			}
		}

		protected async Task ParseOperatorAsync(TokenAnalyzingArgs e, IEnumerable<string> endTokens = null, CancellationToken cancellationToken = default)
		{
			var currentPriority = e.ScriptContext.GetOperatorPriority(e.CurrentToken.Value);
			if (currentPriority.HasValue)
			{
				if (!e.Ignore)
				{
					await e.TreeBuilder.AddOperatorAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, e.CurrentToken.Value, GetDataCount(e.CurrentToken.Value), currentPriority.Value, cancellationToken).ConfigureAwait(false);
				}
				return;
			}
			if (e.CurrentToken.Value.Length == 1)
			{
				throw new Exceptions.ScriptAnalyzingException($"unknown operator '{e.CurrentToken.Value}'");
			}
			// 拆分运算符
			string s0 = e.CurrentToken.Value;
			int cc = s0.Length - 1;
			while (s0.Length > 0)
			{
				string s1 = cc == s0.Length ? s0 : s0.Substring(0, cc);
				var s1Priority = e.ScriptContext.GetOperatorPriority(s1);
				if (s1Priority.HasValue)
				{
					//treeBuilder.AddOperator(buildContext, scriptContext, options, control, s1, GetDataCount(s1), s1Priority);
					s0 = s0.Substring(cc);
					//cc = s0.Length;
					e.TokenReader.Push(new Token(s0, ETokenType.Operator, e.CurrentToken.Line, e.CurrentToken.Column + s1.Length));
					e.TokenReader.Push(new Token(s1, ETokenType.Operator, e.CurrentToken.Line, e.CurrentToken.Column));
					break;
				}
				if (ScriptUtils.Contains(endTokens, s1))
				{
					e.End = true;
					s0 = s0.Substring(cc);
					e.TokenReader.Push(new Token(s0, ETokenType.Operator, e.CurrentToken.Line, e.CurrentToken.Column + s1.Length));
					e.TokenReader.Push(new Token(s1, ETokenType.Operator, e.CurrentToken.Line, e.CurrentToken.Column));
					break;
				}
				cc--;
				if (cc == 0)
				{
					throw new Exceptions.ScriptAnalyzingException($"unknown operator '{e.CurrentToken.Value}'");
				}
			}
		}

		/// <summary>
		/// 解析函数调用
		/// </summary>
		protected void ParseFuncCall(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, string funcName, bool ignore = false)
		{
			var createFullTreeNodeOption = new BuildOptions(options) { CreateFullTreeNode = true };
			var args = BuildFuncParams(buildContext, scriptContext, createFullTreeNodeOption, tokenReader, null, ignore);

			if (!ignore)
			{
				// 如果前面有点操作符，则表示调用实例函数或类静态函数
				if (treeBuilder.Current is OperatorNode operatorNode && operatorNode.Name == ".")
				{
					var target = operatorNode.Left;
					treeBuilder.Pop();
					treeBuilder.Add(buildContext, scriptContext, options, control, new CallFuncNode { Name = funcName, Args = args?.ToArray(), Target = target });
				}
				else
				{
					treeBuilder.Add(buildContext, scriptContext, options, null, new CallFuncNode { Name = funcName, Args = args?.ToArray() });
				}
			}
		}

		/// <summary>
		/// 解析函数调用
		/// </summary>
		protected async Task ParseFuncCallAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, string funcName, bool ignore = false, CancellationToken cancellationToken = default)
		{
			var createFullTreeNodeOption = new BuildOptions(options) { CreateFullTreeNode = true };
			var args = await BuildFuncParamsAsync(buildContext, scriptContext, createFullTreeNodeOption, tokenReader, null, ignore, cancellationToken).ConfigureAwait(false);

			if (!ignore)
			{
				// 如果前面有点操作符，则表示调用实例函数或类静态函数
				if (treeBuilder.Current is OperatorNode operatorNode && operatorNode.Name == ".")
				{
					var target = operatorNode.Left;
					treeBuilder.Pop();
					await treeBuilder.AddAsync(buildContext, scriptContext, options, control, new CallFuncNode { Name = funcName, Args = args?.ToArray(), Target = target }, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					await treeBuilder.AddAsync(buildContext, scriptContext, options, null, new CallFuncNode { Name = funcName, Args = args?.ToArray() }, cancellationToken).ConfigureAwait(false);
				}
			}
		}

		/// <summary>
		/// 解析函数定义
		/// </summary>
		protected void ParseFuncDefine(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, string funcName, string funcReturnType, Type funcReturnSystemType = null, bool ignore = false)
		{
			// 生成自定义函数
			var args = ignore ? null : new List<DefineVarNode>();
			var token = tokenReader.Read();
			while (token.HasValue && !token.Value.IsSymbol(")"))
			{
				// 参数类型
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid arg type:" + funcName + "->" + token.Value.Value);
				}
				var argType = token.Value.Value;
				// 参数名
				token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
				}
				if (token.Value.IsSymbol("["))
				{
					// 数组类型
					token = tokenReader.Read();
					if (!token.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
					}
					if (!token.Value.IsSymbol("]"))
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid function define:{funcName} -> '{token.Value.Value}', expect ']'");
					}
					argType += "[]";
					token = tokenReader.Read();
					if (!token.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
					}
				}
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid arg name:" + funcName + "->" + token.Value.Value);
				}
				string argName = token.Value.Value;
				if (!ignore)
				{
					args.Add(PoolManage.CreateDefineVarNode(argName, argType));
				}
				// 逗号
				token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
				}
				if (token.Value.IsSymbol(")")) break;
				if (!token.Value.IsSymbol(","))
				{
					throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
				}
				token = tokenReader.Read();
			}
			//
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid function define, no body:" + funcName);
			}
			token = tokenReader.Read();
			// 函数体
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid function define, no body:" + funcName);
			}
			if (token.Value.IsSymbol("=>"))
			{
				//token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid function define, no body:" + funcName);
				}
			}
			else
			{
				tokenReader.Push(token.Value);
			}
			var createFullTreeNodeOptions = new BuildOptions(options) { CreateFullTreeNode = true };
			var body = BuildOneStatement2(buildContext, scriptContext, createFullTreeNodeOptions, tokenReader, null, ignore, noblock: true);
			if (!ignore)
			{
				if (body is TreeBuilder bodyTreeBuilder)
				{
					body = bodyTreeBuilder.EvalRoot(buildContext, scriptContext, createFullTreeNodeOptions, null);
					PoolManage.Return(bodyTreeBuilder);
				}
				var defineFuncNode = new DefineFuncNode { Name = funcName, ReturnType = funcReturnType, ReturnSystemType = funcReturnSystemType, Args = args.ToArray(), Body = body };
				if (options.CreateFullTreeNode ?? false)
				{
					treeBuilder.Add(buildContext, scriptContext, options, null, defineFuncNode);
				}
				else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
				{
					var bodyExpr = defineFuncNode.Build(buildContext, scriptContext, options);
					treeBuilder.Add(buildContext, scriptContext, options, null, PoolManage.CreateExpressionNode(bodyExpr));
				}
				else
				{
					var result = defineFuncNode.Eval(scriptContext, options, null, out var resultType);
					treeBuilder.AddData(buildContext, scriptContext, options, null, result, resultType);
				}
			}
		}

		/// <summary>
		/// 解析函数定义
		/// </summary>
		protected async Task ParseFuncDefineAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, string funcName, string funcReturnType, Type funcReturnSystemType = null, bool ignore = false, CancellationToken cancellationToken = default)
		{
			// 生成自定义函数
			var args = ignore ? null : new List<DefineVarNode>();
			var token = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			while (token.HasValue && !token.Value.IsSymbol(")"))
			{
				// 参数类型
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid arg type:" + funcName + "->" + token.Value.Value);
				}
				var argType = token.Value.Value;
				// 参数名
				token = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
				}
				if (token.Value.IsSymbol("["))
				{
					// 数组类型
					token = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					if (!token.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
					}
					if (!token.Value.IsSymbol("]"))
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid function define:{funcName} -> '{token.Value.Value}', expect ']'");
					}
					argType += "[]";
					token = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
					if (!token.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
					}
				}
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid arg name:" + funcName + "->" + token.Value.Value);
				}
				string argName = token.Value.Value;
				if (!ignore)
				{
					args.Add(PoolManage.CreateDefineVarNode(argName, argType));
				}
				// 逗号
				token = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
				}
				if (token.Value.IsSymbol(")")) break;
				if (!token.Value.IsSymbol(","))
				{
					throw new Exceptions.ScriptAnalyzingException("invalid function define:" + funcName);
				}
				token = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			}
			//
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid function define, no body:" + funcName);
			}
			token = await tokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			// 函数体
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid function define, no body:" + funcName);
			}
			if (token.Value.IsSymbol("=>"))
			{
				//token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException("invalid function define, no body:" + funcName);
				}
			}
			else
			{
				tokenReader.Push(token.Value);
			}
			var createFullTreeNodeOptions = new BuildOptions(options) { CreateFullTreeNode = true };
			var body = await BuildOneStatement2Async(buildContext, scriptContext, createFullTreeNodeOptions, tokenReader, null, ignore, noblock: true, cancellationToken: cancellationToken).ConfigureAwait(false);
			if (!ignore)
			{
				if (body is TreeBuilder bodyTreeBuilder)
				{
					body = await bodyTreeBuilder.EvalRootAsync(buildContext, scriptContext, createFullTreeNodeOptions, null, cancellationToken).ConfigureAwait(false);
					PoolManage.Return(bodyTreeBuilder);
				}
				var defineFuncNode = new DefineFuncNode { Name = funcName, ReturnType = funcReturnType, ReturnSystemType = funcReturnSystemType, Args = args.ToArray(), Body = body };
				if (options.CreateFullTreeNode ?? false)
				{
					await treeBuilder.AddAsync(buildContext, scriptContext, options, null, defineFuncNode, cancellationToken).ConfigureAwait(false);
				}
				else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
				{
					var bodyExpr = defineFuncNode.Build(buildContext, scriptContext, options);
					await treeBuilder.AddAsync(buildContext, scriptContext, options, null, PoolManage.CreateExpressionNode(bodyExpr), cancellationToken).ConfigureAwait(false);
				}
				else
				{
					var result = await defineFuncNode.EvalAsync(scriptContext, options, null, cancellationToken).ConfigureAwait(false);
					await treeBuilder.AddDataAsync(buildContext, scriptContext, options, null, result.Value, result.Type, cancellationToken).ConfigureAwait(false);
				}
			}
		}

		protected void ParseFuncDefine(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, CallFuncNode funcHead, bool ignore = false)
		{
			var createFullTreeNodeOptions = new BuildOptions(options) { CreateFullTreeNode = true };
			var body = BuildOneStatement2(buildContext, scriptContext, createFullTreeNodeOptions, tokenReader, null, ignore, noblock: true);
			//// 解析 lambda 函数体
			//var body = BuildOneStatement(buildContext, scriptContext, createFullTreeNodeOptions, tokenReader, null, ignore);

			if (ignore) return;

			if (body is TreeBuilder bodyTreeBuilder)
			{
				body = bodyTreeBuilder.EvalRoot(buildContext, scriptContext, createFullTreeNodeOptions, null);
				PoolManage.Return(bodyTreeBuilder);
			}

			var defineFuncNode = new DefineFuncNode
			{
				Name = funcHead.Name,
				Args = funcHead.Args?.Select(a => a is DefineVarNode defineVarNode ? defineVarNode : PoolManage.CreateDefineVarNode(((VariableNode)a).Name, null, typeof(object))).ToArray(),
				Body = body
			};

			treeBuilder.Pop();
			//treeBuilder.AddData(buildContext, scriptContext, options, control, defineFuncNode);
			if (options.CreateFullTreeNode ?? false)
			{
				treeBuilder.Add(buildContext, scriptContext, options, null, defineFuncNode);
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var bodyExpr = defineFuncNode.Build(buildContext, scriptContext, options);
				treeBuilder.Add(buildContext, scriptContext, options, null, PoolManage.CreateExpressionNode(bodyExpr));
			}
			else
			{
				var result = defineFuncNode.Eval(scriptContext, options, null, out var resultType);
				treeBuilder.AddData(buildContext, scriptContext, options, null, result, resultType);
			}
		}

		protected async Task ParseFuncDefineAsync(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, CallFuncNode funcHead, bool ignore = false, CancellationToken cancellationToken = default)
		{
			var createFullTreeNodeOptions = new BuildOptions(options) { CreateFullTreeNode = true };
			var body = await BuildOneStatement2Async(buildContext, scriptContext, createFullTreeNodeOptions, tokenReader, null, ignore, noblock: true, cancellationToken: cancellationToken).ConfigureAwait(false);
			//// 解析 lambda 函数体
			//var body = BuildOneStatement(buildContext, scriptContext, createFullTreeNodeOptions, tokenReader, null, ignore);

			if (ignore) return;

			if (body is TreeBuilder bodyTreeBuilder)
			{
				body = await bodyTreeBuilder.EvalRootAsync(buildContext, scriptContext, createFullTreeNodeOptions, null, cancellationToken).ConfigureAwait(false);
				PoolManage.Return(bodyTreeBuilder);
			}

			var defineFuncNode = new DefineFuncNode
			{
				Name = funcHead.Name,
				Args = funcHead.Args?.Select(a => a is DefineVarNode defineVarNode ? defineVarNode : PoolManage.CreateDefineVarNode(((VariableNode)a).Name, null, typeof(object))).ToArray(),
				Body = body
			};

			treeBuilder.Pop();
			//treeBuilder.AddData(buildContext, scriptContext, options, control, defineFuncNode);
			if (options.CreateFullTreeNode ?? false)
			{
				await treeBuilder.AddAsync(buildContext, scriptContext, options, null, defineFuncNode, cancellationToken).ConfigureAwait(false);
			}
			else if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var bodyExpr = defineFuncNode.Build(buildContext, scriptContext, options);
				await treeBuilder.AddAsync(buildContext, scriptContext, options, null, PoolManage.CreateExpressionNode(bodyExpr), cancellationToken).ConfigureAwait(false);
			}
			else
			{
				var result = await defineFuncNode.EvalAsync(scriptContext, options, null, cancellationToken).ConfigureAwait(false);
				await treeBuilder.AddDataAsync(buildContext, scriptContext, options, null, result.Value, result.Type, cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// 转换表达式参数（编译模式）
		/// </summary>
		internal static Expression[] ConvertArguments(Expression[] args, ParameterInfo[] parameters, int expressionStartIndex = 0)
		{
			if (args == null || parameters == null) return args;
			if (args.Length != parameters.Length) return args;

			var converted = new Expression[args.Length + expressionStartIndex];
			for (int i = 0; i < args.Length; i++)
			{
				var paramType = parameters[i].ParameterType;
				var argType = args[i].Type;

				if (paramType == argType)
				{
					converted[i + expressionStartIndex] = args[i];
				}
				else if (paramType.IsAssignableFrom(argType))
				{
					converted[i + expressionStartIndex] = args[i];
				}
				else
				{
					// 尝试类型转换
					converted[i + expressionStartIndex] = Expression.Convert(args[i], paramType);
				}
			}

			return converted;
		}

		/// <summary>
		/// 转换对象参数（非编译模式）
		/// </summary>
		internal static object[] ConvertObjectArguments(object[] args, ParameterInfo[] parameters)
		{
			if (args == null || parameters == null) return args;
			if (args.Length > parameters.Length) return args;

			var converted = new object[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				var paramType = parameters[i].ParameterType;
				var argValue = args[i];

				if (argValue == null)
				{
					converted[i] = paramType.IsValueType ? Activator.CreateInstance(paramType) : null;
					continue;
				}

				var argType = argValue.GetType();
				if (paramType.IsAssignableFrom(argType))
				{
					converted[i] = argValue;
				}
				else
				{
					// 尝试类型转换
					try
					{
						converted[i] = Convert.ChangeType(argValue, Nullable.GetUnderlyingType(paramType) ?? paramType);
					}
					catch
					{
						converted[i] = argValue;
					}
				}
			}

			return converted;
		}

		/// <summary>
		/// 运算符操作数数量
		/// </summary>
		public static int GetDataCount(string op)
		{
			if (op == "!" || op == "~") return 1;
			if (op == "++" || op == "--") return 1;
			return 2;
		}

		protected virtual object EvalNumber(string num)
		{
			return ScriptUtils.EvalNumber(num);
		}

		protected virtual void OnTokenAnalyzing(TokenAnalyzingArgs e)
		{
			if (e.IsHandled) return;

			this.TokenAnalyzing?.Invoke(this, e);
		}
	}
}
