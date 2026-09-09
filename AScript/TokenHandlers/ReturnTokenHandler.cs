using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AScript.Nodes;
using AScript.Syntaxs;

namespace AScript.TokenHandlers
{
	public class ReturnTokenHandler : ITokenHandler, IAsyncTokenHandler
	{
		public static readonly ReturnTokenHandler Instance = new ReturnTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, null, e.Ignore);
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue)
			{
				if (nextToken.Value.IsSymbol(","))
				{
					var tuple = e.Ignore ? null : new TupleNode
					{
						Items = new List<ITreeNode> { statement }
					};
					while (true)
					{
						statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, null, e.Ignore);
						tuple?.Items.Add(statement);
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue) break;
						if (nextToken.Value.IsSymbol(",")) continue;
						e.TokenReader.Push(nextToken.Value);
						break;
					}
					statement = tuple;
				}
				else
				{
					e.TokenReader.Push(nextToken.Value);
				}
			}

			if (e.Ignore) return;

			if (e.Options.CreateFullTreeNode ?? false || (e.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var returnNode = new ReturnNode { Body = statement };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, returnNode);
				return;
			}

			if (e.Control == null)
			{
				throw new Exceptions.ScriptAnalyzingException("unsupport return");
			}
			e.Control.Terminal = true;
			e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, statement);
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

			var returnBuilder = await analyzer.BuildOneStatementAsync(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, null, e.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);

			if (e.Ignore)
			{
				return;
			}

			if (e.Options.CreateFullTreeNode ?? false || (e.Options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var returnNode = new ReturnNode { Body = returnBuilder };
				await e.TreeBuilder.AddDataAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, returnNode, cancellationToken).ConfigureAwait(false);
				return;
			}

			if (e.Control == null)
			{
				throw new Exceptions.ScriptAnalyzingException("unsupport return");
			}
			e.Control.Terminal = true;
			await e.TreeBuilder.AddDataAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, returnBuilder, cancellationToken).ConfigureAwait(false);
		}
	}
}
