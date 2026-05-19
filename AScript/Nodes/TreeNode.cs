using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
    public abstract class TreeNode : ITreeNode
	{
		public OperatorNode Parent { get; set; }

		public object Eval(ScriptContext context, BuildOptions options, out Type returnType)
		{
			return Eval(context, options, null, out returnType);
		}

		public abstract object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType);
		public abstract Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options);

		public virtual async Task<object> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			var result = await Eval2Async(context, options, control, cancellationToken).ConfigureAwait(false);
			return result.Value;
		}

		public virtual async Task<EvalResult> Eval2Async(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			var value = Eval(context, options, control, out var type);
			return new EvalResult(value, type);
		}

		public virtual void Clear()
		{
			this.Parent = null;
		}

		public virtual bool IsFull() => true;
	}
}
