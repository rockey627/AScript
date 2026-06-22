using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	/// <summary>
	/// <![CDATA[
	/// 格式1：for(var i=0; i<10; i++) { }
	/// 格式2：for(var i in list) { var item=list[i]; }
	/// 格式3：for(var item of list) { }
	/// ]]>
	/// </summary>
	public class JavaScriptForTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptForTokenHandler Instance = new JavaScriptForTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.Root != null)
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			analyzer.ValidateNextToken(e.TokenReader, "(");

			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };

			// Look ahead to determine which format
			var token1 = e.TokenReader.Read();
			if (!token1.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid for expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			if (token1.Value.Type != ETokenType.Word)
			{
				e.TokenReader.Push(token1.Value);
				// 格式1：for(init; condition; post) { }
				BuildTraditionalFor(analyzer, e, createFullOptions);
				return;
			}
			var token2 = e.TokenReader.Read();
			if (!token2.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid for expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			if (token2.Value.Type != ETokenType.Word)
			{
				e.TokenReader.Push(token2.Value);
				e.TokenReader.Push(token1.Value);
				// 格式1：for(init; condition; post) { }
				BuildTraditionalFor(analyzer, e, createFullOptions);
				return;
			}
			var token3 = e.TokenReader.Read();
			if (!token3.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid for expression at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}
			if (token3.Value.IsSymbol("in"))
			{
				// 格式2：for(var key in collection) { }
				var list = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				analyzer.ValidateNextToken(e.TokenReader, ")");
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				if (!e.Ignore)
				{
					var forInNode = new ForeachNode
					{
						ForeachKey = true,
						VarDefine = PoolManage.CreateDefineVarNode(token2.Value.Value, null, systemType: typeof(object)),
						Collection = list,
						Body = body
					};
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forInNode);
				}
				return;
			}
			if (token3.Value.IsSymbol("of"))
			{
				// 格式3：for(var item of list) { }
				var list = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				analyzer.ValidateNextToken(e.TokenReader, ")");
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
				if (!e.Ignore)
				{
					var forInNode = new ForeachNode
					{
						VarDefine = PoolManage.CreateDefineVarNode(token2.Value.Value, null, systemType: typeof(object)),
						Collection = list,
						Body = body
					};
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forInNode);
				}
				return;
			}

			// 格式1：for(init; condition; post) { }
			e.TokenReader.Push(token3.Value);
			e.TokenReader.Push(token2.Value);
			e.TokenReader.Push(token1.Value);
			BuildTraditionalFor(analyzer, e, createFullOptions);
		}

		private void BuildTraditionalFor(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, BuildOptions createFullOptions)
		{
			// 执行初始化语句
			var initBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.TrySkipNextToken(e.TokenReader, ";");
			// 获取条件语句
			var conditionBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.TrySkipNextToken(e.TokenReader, ";");
			// 获取后置语句
			var postBuilder = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			analyzer.ValidateNextToken(e.TokenReader, ")");
			// 获取循环体语句
			var bodyBuilder = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, noblock: true);
			if (!e.Ignore)
			{
				var forNode = new ForNode { Init = initBuilder, Condition = conditionBuilder, Body = bodyBuilder, Post = postBuilder };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, forNode);
			}
		}
	}
}
