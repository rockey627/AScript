using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AScript.Nodes;

namespace AScript
{
	public static class ScriptExtensions
	{
		public static IList<Token> ParseAll(this ITokenStream parser)
		{
			var list = new List<Token>();
			Token? token = parser.Next();
			while (token.HasValue)
			{
				list.Add(token.Value);
				token = parser.Next();
			}
			return list;
		}

		public static async Task<IList<Token>> ParseAllAsync(this ITokenStream parser, CancellationToken cancellationToken = default)
		{
			var list = new List<Token>();
			Token? token = await parser.NextAsync(cancellationToken).ConfigureAwait(false);
			while (token.HasValue)
			{
				list.Add(token.Value);
				token = await parser.NextAsync(cancellationToken).ConfigureAwait(false);
			}
			return list;
		}

		public static object Eval(this ISyntaxAnalyzer analyzer, ScriptContext context, BuildOptions options, ITokenStream tokenStream, out Type returnType)
		{
			var buildContext = new BuildContext();
			var treeBuilder = analyzer.Build(buildContext, context, options, new Readers.TokenReader(tokenStream, false));
			if (treeBuilder == null)
			{
				returnType = null;
				return null;
			}
			var value = treeBuilder.Eval(context, options, null, out returnType);
			PoolManage.Return(treeBuilder);
			return value;
		}

		public static async Task<EvalResult> EvalAsync(this ISyntaxAnalyzer analyzer, ScriptContext context, BuildOptions options, ITokenStream tokenStream, CancellationToken cancellationToken = default)
		{
			var buildContext = new BuildContext();
			var treeBuilder = await analyzer.BuildAsync(buildContext, context, options, new Readers.TokenReader(tokenStream, false), cancellationToken).ConfigureAwait(false);
			if (treeBuilder == null)
			{
				return default;
			}
			var result = await treeBuilder.EvalAsync(context, options, null, cancellationToken).ConfigureAwait(false);
			PoolManage.Return(treeBuilder);
			return result;
		}
	}
}
