using AScript;
using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Test.Consoles.中文
{
	/// <summary>
	/// 如果 ... 则 ... 否则 ...
	/// </summary>
	public class 如果语法处理器 : ITokenHandler
	{
		private static readonly HashSet<string> _StatementEndTokens = new HashSet<string> { "则", "否则" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			// 如果前面有语句，则返回
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _StatementEndTokens);
			analyzer.ValidateNextToken(e.TokenReader, "则");
			var createAllOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createAllOptions, e.TokenReader, e.Control, e.Ignore, endTokens: _StatementEndTokens);
			var node = new IfNode { Condition = condition, Body = body };
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue && nextToken.Value.Value == ";")
			{
				nextToken = e.TokenReader.Read();
			}
			if (nextToken.HasValue)
			{
				if (nextToken.Value.Value == "否则")
				{
					node.Else = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createAllOptions, e.TokenReader, e.Control, e.Ignore);
				}
				else
				{
					e.TokenReader.Push(nextToken.Value);
				}
			}
			e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, node);
		}
	}
}
