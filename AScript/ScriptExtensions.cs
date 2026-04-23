using System;
using System.Collections.Generic;
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

		public static object Eval(this ISyntaxAnalyzer analyzer, ScriptContext context, BuildOptions options, ITokenStream tokenStream, out Type returnType)
		{
			var buildContext = new BuildContext();
			var treeBuilder = analyzer.Build(buildContext, context, options, new Readers.TokenReader(tokenStream, false));
			if (treeBuilder == null)
			{
				returnType = null;
				return null;
			}
			var result = treeBuilder.Eval(context, options, null, out returnType);
			PoolManage.Return(treeBuilder);
			return result;
		}
	}
}
