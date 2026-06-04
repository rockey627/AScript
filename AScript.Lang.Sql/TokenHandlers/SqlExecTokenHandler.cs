using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Linq;

namespace AScript.Lang.Sql.TokenHandlers
{
	/// <summary>
	/// SqlServer执行存储过程或者外部方法：
	/// exec 存储过程名称 参数1,参数2
	/// </summary>
	public class SqlExecTokenHandler : ITokenHandler
	{
		public static readonly SqlExecTokenHandler Instance = new SqlExecTokenHandler();

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
			var args = analyzer.BuildFuncParams2(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new CallFuncNode { Name = nameToken.Value.Value, Args = args.ToArray() });
			}
		}
	}
}
