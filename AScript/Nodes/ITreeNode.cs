using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	/// <summary>
	/// 表达式树节点
	/// </summary>
    public interface ITreeNode
	{
		/// <summary>
		/// 上级操作节点
		/// </summary>
		OperatorNode Parent { get; set; }

		/// <summary>
		/// 清空节点
		/// </summary>
		void Clear();

		/// <summary>
		/// 运算
		/// </summary>
		/// <param name="context"></param>
		/// <param name="options"></param>
		/// <param name="control"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType);

		/// <summary>
		/// 编译
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options);
	}
}
