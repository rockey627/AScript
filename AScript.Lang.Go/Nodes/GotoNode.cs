using System;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Nodes
{
	/// <summary>
	/// Go语言的goto语句节点
	/// </summary>
	public class GotoNode : AScript.Nodes.TreeNode
	{
		public string Label { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			throw new NotImplementedException("goto requires runtime support");
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			throw new NotImplementedException("goto requires runtime support");
		}
	}
}
