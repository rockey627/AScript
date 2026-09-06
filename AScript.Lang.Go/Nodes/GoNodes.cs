using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Nodes
{
	/// <summary>
	/// Go语言的fallthrough语句节点
	/// </summary>
	public class FallthroughNode : AScript.Nodes.TreeNode
	{
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

	/// <summary>
	/// Go语言的defer语句节点
	/// </summary>
	public class DeferNode : AScript.Nodes.TreeNode
	{
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

	/// <summary>
	/// Go语言的select语句节点
	/// </summary>
	public class SelectNode : AScript.Nodes.TreeNode
	{
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

	/// <summary>
	/// Go语言的case语句节点
	/// </summary>
	public class CaseNode : AScript.Nodes.TreeNode
	{
		public IList<AScript.Nodes.ITreeNode> Conditions { get; set; }
		public AScript.Nodes.ITreeNode Body { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return Body?.Build(buildContext, scriptContext, options) ?? Expression.Empty();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (Body == null)
			{
				returnType = typeof(void);
				return null;
			}
			return Body.Eval(context, options, control, out returnType);
		}
	}

	/// <summary>
	/// Go语言的default语句节点
	/// </summary>
	public class DefaultNode : AScript.Nodes.TreeNode
	{
		public AScript.Nodes.ITreeNode Body { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return Body?.Build(buildContext, scriptContext, options) ?? Expression.Empty();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (Body == null)
			{
				returnType = typeof(void);
				return null;
			}
			return Body.Eval(context, options, control, out returnType);
		}
	}
}
