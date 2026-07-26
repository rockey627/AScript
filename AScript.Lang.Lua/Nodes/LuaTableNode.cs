using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Lang.Lua.Nodes
{
	/// <summary>
	/// { 'h1', a=5, 'h2' } => 1:'h1', a:5, 2:'h2'
	/// </summary>
	public class LuaTableNode : TreeNode
	{
		public IList<ITreeNode> Items { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			throw new NotImplementedException();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var dict = new Dictionary<object, object>();
			returnType = dict.GetType();
			if (this.Items == null || this.Items.Count == 0)
			{
				return dict;
			}
			long index = 1L;
			foreach (var item in this.Items)
			{
				if (item is OperatorNode op && op.Name == "=")
				{
					var key = ((VariableNode)op.Left).Name;
					var value = op.Right.Eval(context, options, control, out _);
					dict[key] = value;
				}
				else
				{
					dict[index++] = item?.Eval(context, options, control, out _);
				}
			}
			return dict;
		}

		public override void Clear()
		{
			base.Clear();

			this.Items = null;
		}
	}
}
