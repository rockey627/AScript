using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言if/else语句处理器
	/// if condition { body }
	/// if condition { body } else { body }
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

			// 解析条件
			var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);

			// 解析if或else body
			var createFullOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

			// 检查是否有else
			var elseToken = e.TokenReader.Read();
			if (!elseToken.HasValue)
			{
				if (!e.Ignore)
				{
					var ifNode = new IfNode { Condition = condition, Body = body };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, ifNode);
				}
				return;
			}

			if (elseToken.Value.Value == "else")
			{
				var next = e.TokenReader.Read();
				if (!next.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid else at ({elseToken.Value.Line},{elseToken.Value.Column})");
				}

				ITreeNode elseNode = null;

				if (next.Value.Value == "if")
				{
					// else if - 简化处理，递归调用
					e.TokenReader.Push(next.Value);
					// 构建else if节点
					var elseIfCondition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
					var elseIfBody = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);

					// 检查是否有进一步的else
					var furtherElse = e.TokenReader.Read();
					ITreeNode furtherElseNode = null;
					if (furtherElse.HasValue && furtherElse.Value.Value == "else")
					{
						var nextFurther = e.TokenReader.Read();
						if (nextFurther.HasValue && nextFurther.Value.Value == "if")
						{
							e.TokenReader.Push(nextFurther.Value);
							furtherElseNode = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
						}
						else
						{
							e.TokenReader.Push(nextFurther.Value);
							furtherElseNode = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
						}
					}
					else if (furtherElse.HasValue)
					{
						e.TokenReader.Push(furtherElse.Value);
					}

					if (!e.Ignore)
					{
						var elseIfNode = new IfNode { Condition = elseIfCondition, Body = elseIfBody, Else = furtherElseNode };
						var ifNode = new IfNode { Condition = condition, Body = body, Else = elseIfNode };
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, ifNode);
					}
					return;
				}
				else
				{
					// else { body }
					e.TokenReader.Push(next.Value);
					elseNode = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
				}

				if (!e.Ignore)
				{
					var ifNode = new IfNode { Condition = condition, Body = body, Else = elseNode };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, ifNode);
				}
			}
			else
			{
				e.TokenReader.Push(elseToken.Value);
				if (!e.Ignore)
				{
					var ifNode = new IfNode { Condition = condition, Body = body };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, ifNode);
				}
			}
		}
	}
}
