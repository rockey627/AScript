using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlUpdateNode : TreeNode
	{
		public ITreeNode Source { get; set; }
		public IList<string> Fields { get; set; }
		public IList<ITreeNode> Values { get; set; }
		public ITreeNode Condition { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			throw new NotImplementedException();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			throw new NotImplementedException();
		}
	}
}
