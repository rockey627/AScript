using AScript.Nodes;
using AScript.Syntaxs;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言case语句处理器
	/// case expr1, expr2: ...
	/// </summary>
	public class GoCaseTokenHandler : ITokenHandler
	{
		public static readonly GoCaseTokenHandler Instance = new GoCaseTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };

			// 解析case条件列表
			var conditions = new List<ITreeNode>();
			while (true)
			{
				var token = e.TokenReader.Read();
				if (!token.HasValue) break;

				if (token.Value.IsSymbol(":"))
				{
					break;
				}

				e.TokenReader.Push(token.Value);
				var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				conditions.Add(condition);

				token = e.TokenReader.Read();
				if (!token.HasValue) break;
				if (token.Value.IsSymbol(",")) continue;
				if (token.Value.IsSymbol(":"))
				{
					break;
				}
				e.TokenReader.Push(token.Value);
				break;
			}

			// 解析case body
			var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

			if (!e.Ignore)
			{
				var caseNode = new Nodes.CaseNode { Conditions = conditions, Body = body };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, caseNode);
			}
		}
	}
}
