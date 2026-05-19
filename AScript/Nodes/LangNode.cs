using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
	public class LangNode : TreeNode
	{
		public string[] Langs { get; set; }
		public ITreeNode Body { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (this.Body == null) return null;
			var oldLangs = scriptContext.Langs;
			scriptContext.Langs = this.Langs;
			try
			{
				return this.Body.Build(buildContext, scriptContext, options);
			}
			finally
			{
				scriptContext.Langs = oldLangs;
			}
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.Body == null)
			{
				returnType = null;
				return null;
			}
			var oldLangs = context.Langs;
			context.Langs = this.Langs;
			try
			{
				return this.Body.Eval(context, options, control, out returnType);
			}
			finally
			{
				context.Langs = oldLangs;
			}
		}

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			if (this.Body == null)
			{
				return default;
			}
			var oldLangs = context.Langs;
			context.Langs = this.Langs;
			try
			{
				return await this.Body.EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				context.Langs = oldLangs;
			}
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Body);

			this.Langs = null;
			this.Body = null;
		}
	}
}
