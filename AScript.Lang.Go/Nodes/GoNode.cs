using System;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Nodes
{
	/// <summary>
	/// Go语言的go语句节点（goroutine）
	/// </summary>
	public class GoNode : AScript.Nodes.TreeNode
	{
		public AScript.Nodes.ITreeNode Body { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			throw new NotImplementedException("goroutine requires runtime support");
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			throw new NotImplementedException("goroutine requires runtime support");
		}
	}
}
