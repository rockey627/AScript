using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言switch语句处理器（简化版）
	/// switch expr {
	/// case x: ...
	/// }
	/// </summary>
	public class GoSwitchTokenHandler : ITokenHandler
	{
		public static readonly GoSwitchTokenHandler Instance = new GoSwitchTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// 尝试读取switch的表达式（可选）
			var token = e.TokenReader.Read();
			ITreeNode switchExpr = null;

			if (token.HasValue && !token.Value.IsSymbol("{"))
			{
				e.TokenReader.Push(token.Value);
				switchExpr = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			}
			else if (token.HasValue)
			{
				e.TokenReader.Push(token.Value);
			}

			// 跳过 { ...
			token = e.TokenReader.Read();
			if (token.HasValue && token.Value.IsSymbol("{"))
			{
				// 简化处理 - 跳过到 }
				while (true)
				{
					token = e.TokenReader.Read();
					if (!token.HasValue || token.Value.IsSymbol("}"))
					{
						break;
					}
				}
			}

			if (!e.Ignore)
			{
				// 使用现有的SwitchNode
				var switchNode = new SwitchNode { SwitchValue = switchExpr };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, switchNode);
			}
		}
	}
}
