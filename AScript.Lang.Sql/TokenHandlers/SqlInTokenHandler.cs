using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// id in ('123','124')
	/// </summary>
	public class SqlInTokenHandler : ITokenHandler
	{
		public static readonly SqlInTokenHandler Instance = new SqlInTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
			}

			if (!e.Ignore)
			{
				var opNode = PoolManage.CreateOperatorNode("in", 2, DefaultSyntaxAnalyzer.OperatorPriorities["."] - 1);
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, opNode);
			}

			if (nextToken.Value.IsSymbol("("))
			{
				var token = e.TokenReader.Read();
				if (!token.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
				}
				if (token.Value.IsSymbol("select", StringComparison.OrdinalIgnoreCase))
				{
					e.TokenReader.Push(token.Value);
					var selectNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					analyzer.ValidateNextToken(e.TokenReader, ")");
					if (!e.Ignore)
					{
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, selectNode);
					}
				}
				else
				{
					// 解析列表
					e.TokenReader.Push(token.Value);
					var list = e.Ignore ? null : new List<ITreeNode>();
					while (true)
					{
						var node = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
						list?.Add(node);
						var token2 = e.TokenReader.Read();
						if (!token2.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
						if (token2.Value.IsSymbol(")")) break;
						if (token2.Value.IsSymbol(",")) continue;
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '{token2.Value}' at ({token2.Value.Line},{token2.Value.Column})");
					}
					if (!e.Ignore)
					{
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new CollectionNode { CollectionType = typeof(HashSet<>), Items = list });
					}
				}
			}
			else
			{
				e.TokenReader.Push(nextToken.Value);
			}
		}
	}
}
