using AScript.Nodes;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// 数组：[5]int/[5]int{}/[]int{}/[...]int{}
	/// 下标：arr[5]
	/// 切片：arr[2:5]
	/// </summary>
	public class GoBracketTokenHandler : BracketTokenHandler
	{
		public static readonly GoBracketTokenHandler Instance = new GoBracketTokenHandler();

		public GoBracketTokenHandler() : base(typeof(Array))
		{
		}

		public override void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			if (e.TreeBuilder.Current == null || e.TreeBuilder.Current is OperatorNode opNode && !opNode.IsFull())
			{
				// 数组
				var arrType = GoLang.ParseArrayType(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Ignore);
				var arrValue = GoLang.ParseArrayValue(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, arrType);
				if (!e.Ignore)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, arrValue);
				}
			}
			else
			{
				// 下标、切片
				base.Build(analyzer, e);
			}
		}
	}
}
