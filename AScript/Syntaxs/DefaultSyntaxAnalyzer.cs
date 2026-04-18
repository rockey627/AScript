using AScript.Nodes;
using AScript.Readers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

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
			// 索引器
			["["] = 210
		};

		public event EventHandler<TokenAnalyzingArgs> TokenAnalyzing;

		public virtual ITreeNode Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader)
		{
			return BuildMultiStatement(buildContext, scriptContext, options, tokenReader, new EvalControl());
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
				if (nextToken.Value.Value == ";" || nextToken.Value.Value == ",") continue;
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

		public virtual ITreeNode BuildOneStatement(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, bool noblock = false, IEnumerable<string> endTokens = null)
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
					treeBuilder.AddData(buildContext, scriptContext, options, control, ScriptUtils.EvalNumber(t.Value.Value), null);
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
					if (treeBuilder != null)
					{
						tokenReader.Push(t.Value);
						break;
					}
					if (noblock)
					{
						var statement = BuildMultiStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
						ValidateNextToken(tokenReader, "}");
						return statement;
					}
					return BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
				}
				else if (t.Value.Value == "(")
				{
					if (treeBuilder != null && treeBuilder.IsFullStatement())
					{
						tokenReader.Push(t.Value);
						break;
					}
					var statement0 = BuildMultiStatement(buildContext, scriptContext, options, tokenReader, control, ignore);
					ValidateNextToken(tokenReader, ")");
					if (!ignore)
					{
						if (treeBuilder == null) treeBuilder = PoolManage.CreateTreeBuilder();
						treeBuilder.AddData(buildContext, scriptContext, options, control, statement0);
					}
				}
				else if (t.Value.Value == "=>")
				{
					if (treeBuilder == null || treeBuilder.Current == null || !(treeBuilder.Current is CallFuncNode funcHead)
						|| funcHead.Args != null && funcHead.Args.Length > 0 && funcHead.Args.Any(a => !(a is DefineVarNode)))
					{
						throw new Exception($"invalid expression '=>' at {t.Value.Line},{t.Value.Column}");
					}
					ParseFuncDefine(buildContext, scriptContext, options, tokenReader, control, treeBuilder, funcHead, ignore);
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
			var result = treeBuilder.EvalRoot(buildContext, scriptContext, options, control);
			PoolManage.Return(treeBuilder);
			return result;
		}

		public virtual Token? ValidateNextToken(TokenReader tokenReader, string nextTokenForValid, int currentLine = -1, int currentColumn = -1)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue)
			{
				if (currentLine > 0)
				{
					throw new Exception($"invalid expression at {currentLine},{currentColumn}, expect {nextTokenForValid}");
				}
				throw new Exception($"invalid expression expect {nextTokenForValid}");
			}
			if (nextToken.Value.Type == ETokenType.String || nextToken.Value.Value != nextTokenForValid)
			{
				throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect {nextTokenForValid}");
			}
			return nextToken;
		}

		public virtual Token? ValidateNextToken(TokenReader tokenReader, ETokenType nextTokenTypeForValid, int currentLine = -1, int currentColumn = -1, string expect = null)
		{
			var nextToken = tokenReader.Read();
			if (!nextToken.HasValue)
			{
				if (currentLine > 0)
				{
					throw new Exception($"invalid expression at {currentLine},{currentColumn}, expect {expect ?? nextTokenTypeForValid.ToString()}");
				}
				throw new Exception($"invalid expression expect {expect ?? nextTokenTypeForValid.ToString()}");
			}
			if (nextToken.Value.Type != nextTokenTypeForValid)
			{
				throw new Exception($"invalid expression at {nextToken.Value.Line},{nextToken.Value.Column}, expect {expect ?? nextTokenTypeForValid.ToString()}");
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
				throw new Exception("invalid expression, expect ')'");
			}
			if (nextToken.Value.Value == ")") return null;
			tokenReader.Push(nextToken.Value);
			var list = ignore ? null : new List<ITreeNode>();
			while (true)
			{
				list.Add(BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore));
				nextToken = tokenReader.Read();
				if (!nextToken.HasValue)
				{
					throw new Exception("invalid expression, expect ')'");
				}
				if (nextToken.Value.Value == ")") break;
				if (nextToken.Value.Value == ",") continue;
				throw new Exception($"invalid expression {nextToken.Value.Value} at {nextToken.Value.Line},{nextToken.Value.Column} expect ')'");
			}
			return list;
		}

		private ITreeNode BuildBlock(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
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

		private void ParseIdentifierOrOperator(TokenAnalyzingArgs e, IEnumerable<string> endTokens = null)
		{
			if (e.Ignore) return;

			// 检查是否是操作符
			if (e.CurrentToken.Type == ETokenType.Operator || OperatorPriorities.TryGetValue(e.CurrentToken.Value, out _))
			{
				ParseOperator(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.TreeBuilder, e.CurrentToken);
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
				ParseFuncCall(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.TreeBuilder, e.CurrentToken.Value, e.Ignore);
				//nextToken = tokenReader.Read();
				nextToken = null;
			}
			else if (!(e.TreeBuilder.Current is OperatorNode opNode && opNode.Name == ".") 
				&& nextToken.HasValue && nextToken.Value.Type == ETokenType.Word 
				&& !(ScriptUtils.Contains(endTokens, nextToken.Value.Value) || ScriptUtils.Contains(endTokens, "\n") && nextToken.Value.Line > e.CurrentToken.Line))
			{
				// 类型定义 (int x 或 int Add(...))
				var currentToken = e.CurrentToken;
				string definedTypeName = currentToken.Value;
				var definedType = e.ScriptContext.EvalType(definedTypeName);
				if (definedType == null)
				{
					throw new Exception($"unknown type '{definedTypeName}' at {currentToken.Line},{currentToken.Column}");
				}
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
					e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateDefineVarNode(currentToken.Value, definedTypeName, definedType));
					e.End = !nextToken.HasValue || nextToken.Value.Value != "=";
				}
			}
			else
			{
				// 变量引用
				e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateVariableData(e.CurrentToken.Value));
			}

			if (nextToken.HasValue)
			{
				e.TokenReader.Push(nextToken.Value);
			}
		}

		private void ParseOperator(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, Token currentToken)
		{
			if (OperatorPriorities.TryGetValue(currentToken.Value, out var currentPriority))
			{
				treeBuilder.AddOperator(buildContext, scriptContext, options, control, currentToken.Value, GetDataCount(currentToken.Value), currentPriority);
				return;
			}
			if (currentToken.Value.Length == 1)
			{
				throw new Exception("unknown operator:" + currentToken);
			}
			// 拆分运算符
			string s0 = currentToken.Value;
			int cc = s0.Length - 1;
			while (s0.Length > 0)
			{
				string s1 = cc == s0.Length ? s0 : s0.Substring(0, cc);
				if (OperatorPriorities.TryGetValue(s1, out var s1Priority))
				{
					//treeBuilder.AddOperator(buildContext, scriptContext, options, control, s1, GetDataCount(s1), s1Priority);
					s0 = s0.Substring(cc);
					//cc = s0.Length;
					tokenReader.Push(new Token(s0, ETokenType.Operator, currentToken.Line, currentToken.Column + s1.Length));
					tokenReader.Push(new Token(s1, ETokenType.Operator, currentToken.Line, currentToken.Column));
					break;
				}
				else
				{
					cc--;
					if (cc == 0)
					{
						throw new Exception("unknown operator:" + currentToken);
					}
				}
			}
		}

		/// <summary>
		/// 解析函数调用
		/// </summary>
		private void ParseFuncCall(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, string funcName, bool ignore = false)
		{
			var createFullTreeNodeOption = new BuildOptions(options) { CreateFullTreeNode = true };
			var args = BuildFuncParams(buildContext, scriptContext, createFullTreeNodeOption, tokenReader, null, ignore);

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

		/// <summary>
		/// 解析函数定义
		/// </summary>
		private void ParseFuncDefine(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, string funcName, string funcReturnType, Type funcReturnSystemType = null, bool ignore = false)
		{
			// 生成自定义函数
			var args = ignore ? null : new List<DefineVarNode>();
			var token = tokenReader.Read();
			while (token.HasValue && token.Value.Value != ")")
			{
				// 参数类型
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exception("invalid arg type:" + funcName + "->" + token.Value.Value);
				}
				var argType = token.Value.Value;
				// 参数名
				token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exception("invalid function define:" + funcName);
				}
				if (token.Value.Value == "[")
				{
					// 数组类型
					token = tokenReader.Read();
					if (!token.HasValue)
					{
						throw new Exception("invalid function define:" + funcName);
					}
					if (token.Value.Value != "]")
					{
						throw new Exception($"invalid function define:{funcName} -> '{token.Value.Value}', expect ']'");
					}
					argType += "[]";
					token = tokenReader.Read();
					if (!token.HasValue)
					{
						throw new Exception("invalid function define:" + funcName);
					}
				}
				if (token.Value.Type != ETokenType.Word)
				{
					throw new Exception("invalid arg name:" + funcName + "->" + token.Value.Value);
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
					throw new Exception("invalid function define:" + funcName);
				}
				if (token.Value.Value == ")") break;
				if (token.Value.Value != ",")
				{
					throw new Exception("invalid function define:" + funcName);
				}
				token = tokenReader.Read();
			}
			//
			if (!token.HasValue)
			{
				throw new Exception("invalid function define, no body:" + funcName);
			}
			token = tokenReader.Read();
			// 函数体
			if (!token.HasValue)
			{
				throw new Exception("invalid function define, no body:" + funcName);
			}
			if (token.Value.Value == "=>")
			{
				//token = tokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exception("invalid function define, no body:" + funcName);
				}
			}
			else
			{
				tokenReader.Push(token.Value);
			}
			var createFullTreeNodeOptions = new BuildOptions(options) { CreateFullTreeNode = true };
			var body = BuildOneStatement(buildContext, scriptContext, createFullTreeNodeOptions, tokenReader, null, ignore, noblock: true);
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
					var result = defineFuncNode.Eval(scriptContext, options, out var resultType);
					treeBuilder.AddData(buildContext, scriptContext, options, null, result, resultType);
				}
			}
		}

		private void ParseFuncDefine(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, TreeBuilder treeBuilder, CallFuncNode funcHead, bool ignore = false)
		{
			var createFullTreeNodeOptions = new BuildOptions(options) { CreateFullTreeNode = true };
			var body = BuildOneStatement(buildContext, scriptContext, createFullTreeNodeOptions, tokenReader, null, ignore, noblock: true);
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
				Args = funcHead.Args?.Select(a => a as DefineVarNode).ToArray(),
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
				var result = defineFuncNode.Eval(scriptContext, options, out var resultType);
				treeBuilder.AddData(buildContext, scriptContext, options, null, result, resultType);
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

		protected virtual void OnTokenAnalyzing(TokenAnalyzingArgs e)
		{
			if (e.IsHandled) return;

			this.TokenAnalyzing?.Invoke(this, e);

			if (e.IsHandled) return;

			e.ScriptContext.HandleToken(this, e);
		}
	}
}
