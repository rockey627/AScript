using AScript.Lang.Lua.Nodes;
using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Lua
{
	public class LuaSyntaxAnalyzer : DefaultSyntaxAnalyzer
	{
		public static readonly LuaSyntaxAnalyzer Instance = new LuaSyntaxAnalyzer();

		protected override ITreeNode BuildBlock(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
		{
			// 解析表：{ value1, key1=value2, key2=value3, value4 }
			var items = ignore ? null : new List<ITreeNode>();
			var token = ValidateNextToken(tokenReader);
			if (!token.Value.IsSymbol("}"))
			{
				tokenReader.Push(token.Value);
				var createFullOptions = (options.CreateFullTreeNode ?? false) ? options : new BuildOptions(options) { CreateFullTreeNode = true };
				while (true)
				{
					var item = BuildOneStatement(buildContext, scriptContext, createFullOptions, tokenReader, control, ignore);
					items?.Add(item);
					token = ValidateNextToken(tokenReader);
					if (token.Value.IsSymbol(",")) continue;
					if (token.Value.IsSymbol("}")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid table syntax at {token.Value.Line},{token.Value.Column}, expect ',' or '}}'");
				}
			}
			if (ignore) return null;
			return new LuaTableNode { Items = items };
		}

		protected override object EvalNumber(string num)
		{
			// Lua numbers are typically double unless they represent integers
			var n = ScriptUtils.EvalNumber(num, true);
			//if (n is double d)
			//{
			//	// Check if it's an integer
			//	if (d == Math.Floor(d) && d >= long.MinValue && d <= long.MaxValue)
			//	{
			//		return (long)d;
			//	}
			//}
			return n;
		}
	}
}
