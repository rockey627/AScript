using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言type声明处理器
	/// type Name Type
	/// type Name struct { ... }
	/// type Name interface { ... }
	/// </summary>
	public class GoTypeTokenHandler : ITokenHandler
	{
		public static readonly GoTypeTokenHandler Instance = new GoTypeTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// type Name
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			string typeName = nameToken.Value.Value;

			// 检查是否是struct或interface定义
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				return;
			}

			if (nextToken.Value.Value == "struct")
			{
				// struct { ... }
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, noblock: true);
				if (!e.Ignore)
				{
					var typeNode = new Nodes.TypeDefineNode { Name = typeName, Kind = "struct", Body = body };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, typeNode);
				}
			}
			else if (nextToken.Value.Value == "interface")
			{
				// interface { ... }
				var body = analyzer.BuildOneStatement2(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, noblock: true);
				if (!e.Ignore)
				{
					var typeNode = new Nodes.TypeDefineNode { Name = typeName, Kind = "interface", Body = body };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, typeNode);
				}
			}
			else
			{
				// type Name ActualType
				e.TokenReader.Push(nextToken.Value);
				var actualType = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
				if (!e.Ignore)
				{
					var typeNode = new Nodes.TypeDefineNode { Name = typeName, Body = actualType };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, typeNode);
				}
			}
		}
	}
}
