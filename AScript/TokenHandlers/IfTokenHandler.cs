using System;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class IfTokenHandler : ITokenHandler
	{
		public static readonly IfTokenHandler Instance = new IfTokenHandler();

		public string ElseToken { get; set; } = "else";

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			if (e.CurrentToken.Value == this.ElseToken)
			{
				throw new Exception($"invalid expression '{this.ElseToken}' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			BuildIf(analyzer, e);
		}

		private void BuildIf(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			analyzer.ValidateNextToken(e.TokenReader, "(", e.CurrentToken.Line, e.CurrentToken.Column);

			var conditionBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")", e.CurrentToken.Line, e.CurrentToken.Column);

			if ((e.Options.CreateFullTreeNode ?? false) || (e.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var statementTreeBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				var elseTreeBuilder = TryBuildElse(analyzer, e);
				if (!e.Ignore)
				{
					var ifNode = new IfNode { Condition = conditionBuilder, Body = statementTreeBuilder, Else = elseTreeBuilder };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, ifNode);
				}
				return;
			}

			if (e.Ignore)
			{
				analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				TryBuildElse(analyzer, e);
				return;
			}

			var conditionResult = conditionBuilder.Eval(e.ScriptContext, e.Options, e.Control, out _);
			PoolManage.Return(conditionBuilder);
			if (!(conditionResult is bool b)) throw new Exception("condition must be bool type");
			if (b)
			{
				// 执行if语句
				var statementTreeBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				// 跳过else语句
				TryBuildElse(analyzer, e, ignore: true);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, statementTreeBuilder);
			}
			else
			{
				// 跳过if语句
				analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, ignore: true);
				// 执行else语句
				var elseBuilder = TryBuildElse(analyzer, e);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, elseBuilder);
			}
		}

		private ITreeNode TryBuildElse(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, bool? ignore = null)
		{
			var t = e.TokenReader.Read();
			if (!t.HasValue) return null;
			if (t.Value.Value == ";")
			{
				t = e.TokenReader.Read();
				if (!t.HasValue) return null;
			}
			if (t.Value.Value == this.ElseToken)
			{
				return analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, ignore ?? e.Ignore);
			}
			e.TokenReader.Push(t.Value);
			return null;
		}

	}
}
