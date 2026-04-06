using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    /// <summary>
    /// 函数调用节点
    /// </summary>
    public class CallFuncNode : TreeNode
	{
		public string Name { get; set; }
		public ITreeNode[] Args { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			ITreeNode[] args = null;
			if (this.Args != null && this.Args.Length > 0)
			{
				args = new ITreeNode[this.Args.Length];
				for (int i = 0; i < this.Args.Length; i++)
				{
					var arg = this.Args[i];
					if (arg == null || arg is ObjectNode)
					{
						args[i] = arg;
					}
					else
					{
						var v = arg.Eval(context, options, control, out var type);
						args[i] = PoolManage.CreateObjectData(v, type);
					}
				}
			}
			var tmpContext = ScriptContext.Create(context);
			return tmpContext.EvalFunc(options, control, this.Name, args, out returnType);
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			ITreeNode[] args = null;
			if (this.Args != null && this.Args.Length > 0)
			{
				args = new ITreeNode[this.Args.Length];
				for (int i = 0; i < this.Args.Length; i++)
				{
					var arg = this.Args[i];
					if (arg == null || arg is ExpressionNode)
					{
						args[i] = arg;
					}
					else
					{
						var v = arg.Build(buildContext, scriptContext, options);
						args[i] = PoolManage.CreateExpressionNode(v);
					}
				}
			}
			return scriptContext.BuildFunc(buildContext, options, null, this.Name, false, args);
			//return ExpressionUtils.BuildCall(buildContext, scriptContext, options, this.Name, this.Args);
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Args);

			this.Name = null;
			this.Args = null;
		}
	}
}
