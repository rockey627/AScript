using System;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Nodes
{
	/// <summary>
	/// Go语言的package声明节点
	/// </summary>
	public class PackageNode : AScript.Nodes.TreeNode
	{
		public string Name { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return Expression.Empty();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			returnType = typeof(void);
			return null;
		}
	}
}
