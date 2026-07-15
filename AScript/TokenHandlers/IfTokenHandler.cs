using System;
using System.Threading;
using System.Threading.Tasks;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class IfTokenHandler : ITokenHandler, IAsyncTokenHandler
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
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{this.ElseToken}' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			BuildIf(analyzer, e);
		}

		public async Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken)
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
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{this.ElseToken}' at {e.CurrentToken.Line},{e.CurrentToken.Column}");
			}
			await BuildIfAsync(analyzer, e, cancellationToken).ConfigureAwait(false);
		}

		private void BuildIf(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			analyzer.ValidateNextToken(e.TokenReader, "(");

			var conditionBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");

			if ((e.Options.CreateFullTreeNode ?? false) || (e.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var statementTreeBuilder = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
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
				analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				TryBuildElse(analyzer, e);
				return;
			}

			var conditionResult = conditionBuilder.Eval(e.ScriptContext, e.Options, e.Control, out _);
			PoolManage.Return(conditionBuilder);
			bool b;
			if (conditionResult is bool cr) b = cr;
			else b = e.ScriptContext.IsTrue(conditionResult);
			//if (!(conditionResult is bool b)) throw new Exceptions.ScriptRuntimeException("condition must be bool type");
			if (b)
			{
				// 执行if语句
				var statementTreeBuilder = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				// 跳过else语句
				TryBuildElse(analyzer, e, ignore: true);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, statementTreeBuilder);
			}
			else
			{
				// 跳过if语句
				analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, ignore: true);
				// 执行else语句
				var elseBuilder = TryBuildElse(analyzer, e);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, elseBuilder);
			}
		}

		private async Task BuildIfAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken = default)
		{
			await analyzer.ValidateNextTokenAsync(e.TokenReader, "(", cancellationToken).ConfigureAwait(false);

			var conditionBuilder = await analyzer.BuildOneStatementAsync(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
			await analyzer.ValidateNextTokenAsync(e.TokenReader, ")", cancellationToken).ConfigureAwait(false);

			if ((e.Options.CreateFullTreeNode ?? false) || (e.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var statementTreeBuilder = await analyzer.BuildOneStatement2Async(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
				var elseTreeBuilder = await TryBuildElseAsync(analyzer, e, cancellationToken: cancellationToken).ConfigureAwait(false);
				if (!e.Ignore)
				{
					var ifNode = new IfNode { Condition = conditionBuilder, Body = statementTreeBuilder, Else = elseTreeBuilder };
					await e.TreeBuilder.AddDataAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, ifNode, cancellationToken).ConfigureAwait(false);
				}
				return;
			}

			if (e.Ignore)
			{
				await analyzer.BuildOneStatement2Async(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
				await TryBuildElseAsync(analyzer, e, cancellationToken: cancellationToken).ConfigureAwait(false);
				return;
			}

			var conditionResult = await conditionBuilder.EvalAsync(e.ScriptContext, e.Options, e.Control, cancellationToken).ConfigureAwait(false);
			PoolManage.Return(conditionBuilder);
			bool b;
			if (conditionResult.Value is bool cr) b = cr;
			else b = e.ScriptContext.IsTrue(conditionResult.Value);
			//if (!(conditionResult.Value is bool b)) throw new Exceptions.ScriptRuntimeException("condition must be bool type");
			if (b)
			{
				// 执行if语句
				var statementTreeBuilder = await analyzer.BuildOneStatement2Async(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
				// 跳过else语句
				await TryBuildElseAsync(analyzer, e, ignore: true, cancellationToken).ConfigureAwait(false);
				await e.TreeBuilder.AddDataAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, statementTreeBuilder, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				// 跳过if语句
				await analyzer.BuildOneStatement2Async(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, ignore: true, cancellationToken: cancellationToken).ConfigureAwait(false);
				// 执行else语句
				var elseBuilder = await TryBuildElseAsync(analyzer, e, cancellationToken: cancellationToken).ConfigureAwait(false);
				await e.TreeBuilder.AddDataAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, elseBuilder, cancellationToken).ConfigureAwait(false);
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
				return analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, ignore ?? e.Ignore);
			}
			e.TokenReader.Push(t.Value);
			return null;
		}

		private async Task<ITreeNode> TryBuildElseAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, bool? ignore = null, CancellationToken cancellationToken = default)
		{
			var t = await e.TokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (!t.HasValue) return null;
			if (t.Value.Value == ";")
			{
				t = await e.TokenReader.ReadAsync(cancellationToken).ConfigureAwait(false);
				if (!t.HasValue) return null;
			}
			if (t.Value.Value == this.ElseToken)
			{
				return await analyzer.BuildOneStatement2Async(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, ignore ?? e.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
			}
			e.TokenReader.Push(t.Value);
			return null;
		}
	}
}
