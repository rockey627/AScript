using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
	public class MultiNode : TreeNode
	{
		public IList<ITreeNode> Nodes { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (this.Nodes == null || this.Nodes.Count == 0)
			{
				return null;
			}
			var exprs = new Expression[this.Nodes.Count];
			for (int i = 0; i < this.Nodes.Count; i++)
			{
				exprs[i] = this.Nodes[i].Build(buildContext, scriptContext, options);
			}
			return Expression.Block(exprs);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.Nodes == null || this.Nodes.Count == 0)
			{
				returnType = null;
				return null;
			}
			int count = this.Nodes.Count - 1;
			for (int i = 0; i < count; i++)
			{
				this.Nodes[i].Eval(context, options, control, out _);
			}
			return this.Nodes[count].Eval(context, options, control, out returnType);
		}

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			if (this.Nodes == null || this.Nodes.Count == 0)
			{
				return default;
			}
			int count = this.Nodes.Count - 1;
			for (int i = 0; i < count; i++)
			{
				await this.Nodes[i].EvalAsync(context, options, control, cancellationToken);
			}
			return await this.Nodes[count].EvalAsync(context, options, control, cancellationToken);
		}

		public override void Clear()
		{
			base.Clear();

			if (this.Nodes != null)
			{
				PoolManage.Return(this.Nodes);
				this.Nodes.Clear();
				this.Nodes = null;
			}
		}
	}
}
