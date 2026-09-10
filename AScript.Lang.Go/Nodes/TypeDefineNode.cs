using System;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Nodes
{
	/// <summary>
	/// Go语言的type声明节点
	/// </summary>
	public class TypeDefineNode : AScript.Nodes.TreeNode
	{
		public string Name { get; set; }
		public string Kind { get; set; }
		public AScript.Nodes.ITreeNode Body { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return Body?.Build(buildContext, scriptContext, options) ?? Expression.Empty();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			returnType = typeof(void);
			return null;
		}
	}
}
