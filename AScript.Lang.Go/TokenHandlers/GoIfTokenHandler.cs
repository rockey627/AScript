using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言if语句处理器
	/// 格式1：if condition { ... }
	/// 格式2：if init; condition { ... }
	/// 格式3：if condition { ... } else { ... }
	/// 格式4：if condition { ... } else if condition { ... }
	/// </summary>
	public class GoIfTokenHandler : ITokenHandler
	{
		public static readonly GoIfTokenHandler Instance = new GoIfTokenHandler();

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

			ITreeNode init = null;
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);

			var token = analyzer.ValidateNextToken(e.TokenReader);
			if (token.Value.IsSymbol(";"))
			{
				init = condition;
				condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			}
			else
			{
				e.TokenReader.Push(token.Value);
			}

			// 构建if body
			var body = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);

			// 构建else
			ITreeNode elseNode = null;
			token = e.TokenReader.Read();
			if (token.HasValue)
			{
				if (token.Value.IsSymbol("else"))
				{
					elseNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				}
				else
				{
					e.TokenReader.Push(token.Value);
				}
			}

			if (!e.Ignore)
			{
				var ifNode = new IfNode
				{
					Condition = init == null ? condition : new MultiNode { Nodes = new[] { init, condition } },
					Body = body,
					Else = elseNode
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, ifNode);
			}
		}
	}
}
