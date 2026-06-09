using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	/// <summary>
	/// function 函数名(参数名1, 参数名2) {
	///
	/// }
	/// </summary>
	public class JavaScriptFunctionTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptFunctionTokenHandler Instance = new JavaScriptFunctionTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			// 函数名
			var funcNameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			string funcName = funcNameToken.Value.Value;
			// 参数
			analyzer.ValidateNextToken(e.TokenReader, "(");
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid function {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			List<DefineVarNode> args = null;
			if (!nextToken.Value.IsSymbol(")"))
			{
				args = e.Ignore ? null : new List<DefineVarNode>();
				e.TokenReader.Push(nextToken.Value);
				while (true)
				{
					var argToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					string argName = argToken.Value.Value;
					if (!e.Ignore)
					{
						args.Add(new DefineVarNode(argName) { SystemType = typeof(object) });
					}
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid function {funcName} at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
					}
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol(")")) break;
					throw new Exceptions.ScriptAnalyzingException($"invalid argument name '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
			}
			// 函数体
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
			//
			if (!e.Ignore)
			{
				var defineNode = new DefineFuncNode { Name = funcName, Args = args?.ToArray(), Body = body };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defineNode);
			}
		}
	}
}