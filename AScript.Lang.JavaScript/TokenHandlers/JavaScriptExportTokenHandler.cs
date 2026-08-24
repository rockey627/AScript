using AScript.Nodes;
using AScript.Syntaxs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	/// <summary>
	/// <para>export default { }</para>
	/// <para>export const n = 5</para>
	/// </summary>
	public class JavaScriptExportTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptExportTokenHandler Instance = new JavaScriptExportTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.IsSymbol("default"))
			{
				e.End = true;
				/*
				export default {
					sum,
					add: function(a, b) { a+b },
					function fib(a) { a-1 }
				}
				*/
				var obj = BuildObject(analyzer, e);
				if (!e.Ignore)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, obj);
				}
			}
			else
			{
				e.TokenReader.Push(nextToken.Value);
			}
		}

		private static NewNode BuildObject(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			analyzer.ValidateNextToken(e.TokenReader, "{");
			var nextToken = analyzer.ValidateNextToken(e.TokenReader);
			if (nextToken.Value.IsSymbol("}"))
			{
				if (e.Ignore) return null;
				return new NewNode { SystemType = typeof(ExpandoObject) };
			}
			e.TokenReader.Push(nextToken.Value);
			var initProperties = e.Ignore ? null : new List<ITreeNode>();
			while (true)
			{
				var token = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				if (token.Value.IsSymbol("function"))
				{
					var funcNameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					var funcName = funcNameToken.Value.Value;
					e.TokenReader.Push(funcNameToken.Value);
					e.TokenReader.Push(token.Value);
					var func = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					initProperties?.Add(new OperatorNode("=", DefaultSyntaxAnalyzer.OperatorPriorities["="], 2)
					{
						Left = new VariableNode(funcName),
						Right = func
					});
					// 
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue) break;
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol("}")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
				nextToken = e.TokenReader.Read();
				if (!nextToken.HasValue || nextToken.Value.IsSymbol("}"))
				{
					initProperties?.Add(new OperatorNode("=", DefaultSyntaxAnalyzer.OperatorPriorities["="], 2)
					{
						Left = new VariableNode(token.Value.Value),
						Right = new VariableNode(token.Value.Value)
					});
					break;
				}
				if (nextToken.Value.IsSymbol(","))
				{
					initProperties?.Add(new OperatorNode("=", DefaultSyntaxAnalyzer.OperatorPriorities["="], 2)
					{
						Left = new VariableNode(token.Value.Value),
						Right = new VariableNode(token.Value.Value)
					});
					continue;
				}
				if (nextToken.Value.IsSymbol(":"))
				{
					var value = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					initProperties?.Add(new OperatorNode("=", DefaultSyntaxAnalyzer.OperatorPriorities["="], 2)
					{
						Left = new VariableNode(token.Value.Value),
						Right = value
					});
					// 
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue) break;
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol("}")) break;
				}
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}
			if (e.Ignore) return null;
			return new NewNode { SystemType = typeof(ExpandoObject), InitProperties = initProperties };
		}

	}
}
