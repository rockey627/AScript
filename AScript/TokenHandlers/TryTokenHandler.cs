using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// try { ... } catch { ...} finally { ... }
	/// </summary>
	public class TryTokenHandler : ITokenHandler
	{
		public static readonly TryTokenHandler Instance = new TryTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var createFullTreeNodeOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };

			// try {
			analyzer.ValidateNextToken(e.TokenReader, "{");
			var tryBody = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullTreeNodeOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, "}");

			// Parse catch/finally blocks
			ITreeNode finallyBody = null;
			var catchNodes = new List<Tuple<DefineVarNode, ITreeNode>>();
			while (true)
			{
				var t = e.TokenReader.Read();
				if (!t.HasValue) break;

				if (t.Value.IsSymbol("catch"))
				{
					catchNodes.Add(BuildCatch(analyzer, e, createFullTreeNodeOptions));
					continue;
				}

				if (t.Value.IsSymbol("finally"))
				{
					analyzer.ValidateNextToken(e.TokenReader, "{");
					finallyBody = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullTreeNodeOptions, e.TokenReader, e.Control, false);
					analyzer.ValidateNextToken(e.TokenReader, "}");
					break;
				}

				e.TokenReader.Push(t.Value);
				break;
			}

			if (catchNodes.Count == 0 && finallyBody == null)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near try at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}), expect catch/finally");
			}

			if (!e.Ignore)
			{
				var tryNode = new TryNode { TryBody = tryBody, CatchNodes = catchNodes, FinallyBody = finallyBody };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, tryNode);
			}
		}

		protected virtual Tuple<DefineVarNode, ITreeNode> BuildCatch(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullTreeNodeOptions)
		{
			DefineVarNode exVarNode = null;

			var nextToken = e.TokenReader.Peek();
			if (nextToken.HasValue && nextToken.Value.IsSymbol("{"))
			{
				// catch{}
				//e.TokenReader.Read();
			}
			else
			{
				// catch(...) or catch(...)
				analyzer.ValidateNextToken(e.TokenReader, "(");
				nextToken = e.TokenReader.Peek();
				if (!nextToken.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid catch expression at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn}), expect ')' or exception type");
				}

				if (!nextToken.Value.IsSymbol(")"))
				{
					// catch(Exception) or catch(Exception ex)
					if (nextToken.Value.Type == ETokenType.Word)
					{
						e.TokenReader.Read();
						var exTypeName = nextToken.Value.Value;

						var peekToken = e.TokenReader.Peek();
						string exVarName = null;
						if (peekToken.HasValue && peekToken.Value.Type == ETokenType.Word)
						{
							e.TokenReader.Read();
							exVarName = peekToken.Value.Value;
						}
						exVarNode = PoolManage.CreateDefineVarNode(exVarName, exTypeName);
					}
				}

				analyzer.ValidateNextToken(e.TokenReader, ")");
			}

			// catch body
			analyzer.ValidateNextToken(e.TokenReader, "{");
			var catchBody = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullTreeNodeOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, "}");

			return Tuple.Create(exVarNode, catchBody);
		}
	}
}
