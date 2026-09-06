using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// Go语言import声明处理器
	/// import "path"
	/// import "path"
	/// import name "path"
	/// import ( "path1" "path2" )
	/// </summary>
	public class GoImportTokenHandler : ITokenHandler
	{
		public static readonly GoImportTokenHandler Instance = new GoImportTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var token = e.TokenReader.Read();
			if (!token.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid import at ({e.CurrentToken.Line},{e.CurrentToken.Column})");
			}

			// import ( ... ) 多重导入
			if (token.Value.IsSymbol("("))
			{
				while (true)
				{
					token = e.TokenReader.Read();
					if (!token.HasValue) break;
					if (token.Value.IsSymbol(")")) break;
					if (token.Value.Type == ETokenType.String)
					{
						if (!e.Ignore)
						{
							var importNode = new Nodes.ImportNode { Path = token.Value.Value };
							e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, importNode);
						}
					}
					else if (token.Value.Type == ETokenType.Word)
					{
						// import alias "path"
						string alias = token.Value.Value;
						token = e.TokenReader.Read();
						if (token.HasValue && token.Value.Type == ETokenType.String)
						{
							if (!e.Ignore)
							{
								var importNode = new Nodes.ImportNode { Path = token.Value.Value, Alias = alias };
								e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, importNode);
							}
						}
					}
				}
			}
			else if (token.Value.Type == ETokenType.String)
			{
				// import "path"
				if (!e.Ignore)
				{
					var importNode = new Nodes.ImportNode { Path = token.Value.Value };
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, importNode);
				}
			}
			else if (token.Value.Type == ETokenType.Word)
			{
				// import alias "path"
				string alias = token.Value.Value;
				token = e.TokenReader.Read();
				if (token.HasValue && token.Value.Type == ETokenType.String)
				{
					if (!e.Ignore)
					{
						var importNode = new Nodes.ImportNode { Path = token.Value.Value, Alias = alias };
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, importNode);
					}
				}
			}
		}
	}
}
