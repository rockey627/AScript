using AScript.Nodes;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	public class JavaScriptTryTokenHandler : TryTokenHandler
	{
		public static readonly JavaScriptTryTokenHandler Instance = new JavaScriptTryTokenHandler();

		protected override Tuple<DefineVarNode, ITreeNode> BuildCatch(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullTreeNodeOptions)
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
					// catch(ex)
					if (nextToken.Value.Type == ETokenType.Word)
					{
						e.TokenReader.Read();
						var exVarName = nextToken.Value.Value;
						exVarNode = PoolManage.CreateDefineVarNode(exVarName, null);
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
