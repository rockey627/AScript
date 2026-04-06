using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class BlockNode : TreeNode
	{
		public ITreeNode Block { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.Block != null)
			{
				return this.Block.Eval(ScriptContext.Create(context), options, control, out returnType);
			}
			returnType = null;
			return null;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tempBuildContext = new BuildContext(buildContext);
			var blockBody = this.Block.Build(tempBuildContext, scriptContext, options);
			return tempBuildContext.BuildBlock(scriptContext, options, blockBody);
		}

		public override void Clear()
		{
			base.Clear();

			if (this.Block != null)
			{
				PoolManage.Return(this.Block);
				this.Block = null;
			}
		}
	}
}
