using System;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Nodes
{
	/// <summary>
	/// Go语言的import声明节点
	/// </summary>
	public class ImportNode : AScript.Nodes.TreeNode
	{
		public string Path { get; set; }
		public string Alias { get; set; }

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
