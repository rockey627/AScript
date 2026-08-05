using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// return a,b,c
	/// </summary>
	public class LuaReturnTokenHandler : ITokenHandler
	{
		public static readonly LuaReturnTokenHandler Instance = new LuaReturnTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue || !nextToken.Value.IsSymbol(","))
			{
				if (nextToken.HasValue)
				{
					e.TokenReader.Push(nextToken.Value);
				}
				if (!e.Ignore)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new ReturnNode { Body = statement });
				}
				return;
			}

			var list = e.Ignore ? null : new List<ITreeNode>();
			list?.Add(statement);
			while (true)
			{
				statement = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				list?.Add(statement);
				nextToken = e.TokenReader.Read();
				if (!nextToken.HasValue || !nextToken.Value.IsSymbol(",")) break;
			}
			if (nextToken.HasValue)
			{
				e.TokenReader.Push(nextToken.Value);
			}
			if (list != null)
			{
				var collectionNode = new CollectionNode
				{
					CollectionType = typeof(object[]),
					Items = list
				};
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new ReturnNode { Body = collectionNode });
			}
		}
	}
}
