using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlInsertNode : TreeNode
	{
		public ITreeNode Source { get; set; }
		public IList<VariableNode> Columns { get; set; }
		public IList<IList<ITreeNode>> Values { get; set; }

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
