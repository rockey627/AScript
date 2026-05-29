using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// 
	/// </summary>
	public class OperatorTokenHandler : ITokenHandler, IAsyncTokenHandler
	{
		/// <summary>
		/// 优先级操作符（使用该操作符来获取优先级）
		/// </summary>
		public string PriorityOperator { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public int DataCount { get; set; } = 2;
		/// <summary>
		/// 操作符，如果为空则使用当前解析的符号
		/// </summary>
		public string TargetOperator { get; private set; }

		public bool? Prefix { get; set; }

		public OperatorTokenHandler(string priorityOperator)
		{
			this.PriorityOperator = priorityOperator;
		}
		public OperatorTokenHandler(string priorityOperator, string targetOperator)
		{
			this.PriorityOperator = priorityOperator;
			this.TargetOperator = targetOperator;
		}

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				var op = new OperatorNode(string.IsNullOrEmpty(this.TargetOperator) ? e.CurrentToken.Value : this.TargetOperator, DefaultSyntaxAnalyzer.OperatorPriorities[this.PriorityOperator], this.DataCount)
				{
					Prefix = this.Prefix
				};
				e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
			}
		}

		public async Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				var op = new OperatorNode(string.IsNullOrEmpty(this.TargetOperator) ? e.CurrentToken.Value : this.TargetOperator, DefaultSyntaxAnalyzer.OperatorPriorities[this.PriorityOperator], this.DataCount)
				{
					Prefix = this.Prefix
				};
				await e.TreeBuilder.AddOperatorAsync(e.BuildContext, e.ScriptContext, e.Options, e.Control, op, cancellationToken).ConfigureAwait(false);
			}
		}
	}
}
