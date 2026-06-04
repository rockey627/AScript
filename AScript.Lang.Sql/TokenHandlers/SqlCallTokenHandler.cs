using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Linq;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// MySql执行存储过程或者外部方法：
	/// call 存储过程名称(参数值1, 参数值2)
	/// </summary>
	public class SqlCallTokenHandler : ITokenHandler
	{
		public static readonly SqlCallTokenHandler Instance = new SqlCallTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			analyzer.ValidateNextToken(e.TokenReader, "(");
			var args = analyzer.BuildFuncParams(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new CallFuncNode { Name = nameToken.Value.Value, Args = args.ToArray() });
			}
		}
	}
}
