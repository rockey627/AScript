using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Text;

namespace AScript.Lang.JavaScript.TokenHandlers
{
	/// <summary>
	/// <![CDATA[
	/// 解析正则表达式：/abc/gi
	/// ]]>
	/// </summary>
	public class JavaScriptRegexPatternTokenHandler : ITokenHandler
	{
		public static readonly JavaScriptRegexPatternTokenHandler Instance = new JavaScriptRegexPatternTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			if (e.TreeBuilder.Current != null && !(e.TreeBuilder.Current is OperatorNode))
			{
				return;
			}

			var sb = new StringBuilder();
			bool success = false;
			var charReader = e.TokenReader.CharReader;
			var tokenStream = e.TokenReader.TokenStream as DefaultTokenStream;
			while (true)
			{
				var c = charReader.Read();
				if (!c.HasValue) break;
				if (c.Value == '/')
				{
					success = true;
					sb.Append(c.Value);
					continue;
				}
				if (c.Value == '\\')
				{
					// 转义
					if (success)
					{
						charReader.Push(c.Value);
						break;
					}
					c = charReader.Read();
					if (!c.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid regex pattern at ({charReader.CurrentLine},{charReader.CurrentColumn})");
					}
					if (c.Value == '\\') sb.Append(c.Value);
					else if (c.Value == 'n') sb.Append('\n');
					else if (c.Value == 'r') sb.Append('\r');
					else if (c.Value == 't') sb.Append('\t');
					else if (c.Value == '/') sb.Append('/');
					else
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid regex pattern '\\{c.Value}' at ({charReader.CurrentLine},{charReader.CurrentColumn})");
					}
				}
				if (c.Value == '\n' && !success)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid regex pattern at ({charReader.CurrentLine},{charReader.CurrentColumn})");
				}
				if (IsSpaceOrOperator(c.Value, tokenStream) && success)
				{
					charReader.Push(c.Value);
					break;
				}
				sb.Append(c.Value);
			}
			if (!success)
			{
				if (sb.Length > 0)
				{
					e.TokenReader.CharReader.Push(sb);
				}
				return;
			}

			e.IsHandled = true;
			if (!e.Ignore)
			{
				sb.Insert(0, e.CurrentToken.Value);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(new JavaScriptRegexPattern(sb.ToString())));
			}
		}

		private static bool IsSpaceOrOperator(char c, DefaultTokenStream tokenStream)
		{
			return tokenStream.IsSpace(c) || tokenStream.IsOperator(c) || tokenStream.IsSingleChar(c);
		}
	}
}
