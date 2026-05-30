using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Sql.TokenHandlers
{
	public class SqlCreateTokenHandler : ITokenHandler
	{
		public static readonly SqlCreateTokenHandler Instance = new SqlCreateTokenHandler();

		private static readonly HashSet<string> _ProcedureEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "end" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			var actionToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			if ("procedure".Equals(actionToken.Value.Value))
			{
				e.CurrentToken = actionToken.Value;
				BuildProcedure(analyzer, e);
				return;
			}

			throw new Exceptions.ScriptAnalyzingException($"invalid expression '{actionToken.Value.Value}' at ({actionToken.Value.Line},{actionToken.Value.Column})");
		}

		/// <summary>
		/// create procedure 存储过程名称(参数1 类型, 参数2 类型)
		/// as
		/// begin
		/// 
		/// end
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		private void BuildProcedure(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
			}
			var args = e.Ignore ? null : new List<DefineVarNode>();
			if (nextToken.Value.IsSymbol("("))
			{
				// mysql 解析参数
				nextToken = e.TokenReader.Read();
				if (nextToken.HasValue && nextToken.Value.IsSymbol(")"))
				{
					nextToken = e.TokenReader.Read();
				}
				else
				{
					e.TokenReader.Push(nextToken.Value);
					while (true)
					{
						var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						var typeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						if (args != null)
						{
							args.Add(PoolManage.CreateDefineVarNode(varToken.Value.Value, typeToken.Value.Value));
						}
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
						if (nextToken.Value.IsSymbol(",")) continue;
						if (nextToken.Value.IsSymbol(")"))
						{
							nextToken = e.TokenReader.Read();
							break;
						}
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
					}
				}
			}
			else
			{
				// sqlserver 参数解析
				if (nextToken.Value.IsSymbol("as", StringComparison.OrdinalIgnoreCase))
				{
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
					}
				}
				else
				{
					e.TokenReader.Push(nextToken.Value);
					while (true)
					{
						var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						var typeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						if (args != null)
						{
							args.Add(PoolManage.CreateDefineVarNode(varToken.Value.Value, typeToken.Value.Value));
						}
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
						if (nextToken.Value.IsSymbol(",")) continue;
						if (nextToken.Value.IsSymbol("as"))
						{
							nextToken = e.TokenReader.Read();
							break;
						}
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
					}
				}
			}
			if (!nextToken.Value.IsSymbol("begin", StringComparison.OrdinalIgnoreCase))
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _ProcedureEndTokens);
			analyzer.ValidateNextToken(e.TokenReader, "end", StringComparison.OrdinalIgnoreCase);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new DefineFuncNode { Args = args.ToArray(), Body = body });
			}
		}
	}
}
